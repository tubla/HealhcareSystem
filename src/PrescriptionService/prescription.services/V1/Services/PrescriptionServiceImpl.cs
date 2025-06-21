using AutoMapper;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using prescription.models.V1.Db;
using prescription.models.V1.Dto;
using prescription.repositories.V1.Contracts;
using prescription.services.V1.Contracts;
using shared.V1.Events;
using shared.V1.HelperClasses;
using shared.V1.HelperClasses.Contracts;
using shared.V1.Models;
using System.Text;
using System.Text.Json;

namespace prescription.services.V1.Services
{
    public class PrescriptionServiceImpl(IUnitOfWork _unitOfWork,
            IMapper _mapper,
            IMediaServiceProxy _mediaServiceProxy,
            IMedicationServiceProxy _medicationServiceProxy,
            IAppointmentServiceProxy _appointmentServiceProxy,
            IAuthServiceProxy _authServiceProxy,
            IMemoryCache _memoryCache,
            IEventHubClientProvider _eventHubClientProvider) : IPrescriptionService
    {
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
        private readonly EventHubProducerClient _eventHubClient = _eventHubClientProvider.GetClient(EventNames.MediaDeleted);
        public async Task<Response<PrescriptionResponseDto>> CreateAsync(CreatePrescriptionRequestDto dto, int userId, CancellationToken cancellationToken = default)
        {
            if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WritePrescription, cancellationToken))
                return Response<PrescriptionResponseDto>.Fail("Permission denied", 403);

            try
            {
                if (!await _appointmentServiceProxy.CheckAppointmentExistsAsync(dto.AppointmentId, cancellationToken))
                    return Response<PrescriptionResponseDto>.Fail("Appointment not found", 404);
            }
            catch
            {
                return Response<PrescriptionResponseDto>.Fail("Appointment not found", 404);
            }

            try
            {

                if (!await _medicationServiceProxy.CheckMedicationExistsAsync(dto.MedicationId, cancellationToken))
                    return Response<PrescriptionResponseDto>.Fail("Medication not found", 404);
            }
            catch
            {
                return Response<PrescriptionResponseDto>.Fail("Medication not found", 404);
            }

            var prescription = _mapper.Map<Prescription>(dto);
            await _unitOfWork.PrescriptionsRepository.AddAsync(prescription, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            _memoryCache.Remove($"Prescription_{prescription.PrescriptionId}");

            var prescriptionDto = _mapper.Map<PrescriptionResponseDto>(prescription);
            prescriptionDto.Media = new List<MediaResponseDto>();
            return Response<PrescriptionResponseDto>.Ok(prescriptionDto);
        }

        public async Task<Response<PrescriptionResponseDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadPrescription, cancellationToken))
                return Response<PrescriptionResponseDto>.Fail("Permission denied", 403);

            var cacheKey = $"Prescription_{id}";
            if (_memoryCache.TryGetValue(cacheKey, out PrescriptionResponseDto? cachedPrescription))
                return Response<PrescriptionResponseDto>.Ok(cachedPrescription!);

            var prescription = await _unitOfWork.PrescriptionsRepository.GetByIdAsync(id, cancellationToken);
            if (prescription == null)
                return Response<PrescriptionResponseDto>.Fail($"Prescription {id} not found", 404);

            var prescriptionDto = _mapper.Map<PrescriptionResponseDto>(prescription);
            try
            {
                var mediaIds = await _unitOfWork.PrescriptionsRepository.GetMediaIdsAsync(prescriptionDto.PrescriptionId, cancellationToken);

                if (mediaIds.Any())
                {
                    var result = await _mediaServiceProxy.GetAllMediaAsync(mediaIds, cancellationToken);
                    prescriptionDto.Media = result != null ? result.Success && result.Data != null ? result.Data : [] : ([]);
                }
                else
                {
                    prescriptionDto.Media = [];
                }
            }
            catch
            {
                prescriptionDto.Media = [];
            }

            _memoryCache.Set(cacheKey, prescriptionDto, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            });

            return Response<PrescriptionResponseDto>.Ok(prescriptionDto);
        }

        public async Task<Response<PrescriptionResponseDto>> UpdateAsync(int id, UpdatePrescriptionRequestDto dto, int userId, CancellationToken cancellationToken = default)
        {
            if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WritePrescription, cancellationToken))
                return Response<PrescriptionResponseDto>.Fail("Permission denied", 403);

            var prescription = await _unitOfWork.PrescriptionsRepository.GetByIdAsync(id, cancellationToken);
            if (prescription == null)
                return Response<PrescriptionResponseDto>.Fail($"Prescription {id} not found", 404);

            if (dto.IsAppointmentIdSet && dto.AppointmentId.HasValue && dto.AppointmentId.Value > 0)
            {
                try
                {
                    if (!await _appointmentServiceProxy.CheckAppointmentExistsAsync(dto.AppointmentId.Value, cancellationToken))
                        return Response<PrescriptionResponseDto>.Fail("Appointment not found", 404);
                }
                catch
                {
                    return Response<PrescriptionResponseDto>.Fail("Appointment not found", 404);
                }

                prescription.AppointmentId = dto.AppointmentId.Value;
            }

            if (dto.IsMedicationIdSet && dto.MedicationId.HasValue && dto.MedicationId.Value > 0)
            {
                try
                {

                    if (!await _medicationServiceProxy.CheckMedicationExistsAsync(dto.MedicationId.Value, cancellationToken))
                        return Response<PrescriptionResponseDto>.Fail("Medication not found", 404);
                }
                catch
                {
                    return Response<PrescriptionResponseDto>.Fail("Medication not found", 404);
                }

                prescription.MedicationId = dto.MedicationId.Value;
            }

            if (dto.IsDurationSet && !string.IsNullOrWhiteSpace(dto.Duration))
                prescription.Duration = dto.Duration;
            if (dto.IsDosageSet && !string.IsNullOrWhiteSpace(dto.Dosage))
                prescription.Dosage = dto.Dosage;

            await _unitOfWork.CompleteAsync(cancellationToken);

            _memoryCache.Remove($"Prescription_{id}");

            var prescriptionDto = _mapper.Map<PrescriptionResponseDto>(prescription);
            try
            {
                var mediaIds = await _unitOfWork.PrescriptionsRepository.GetMediaIdsAsync(prescriptionDto.PrescriptionId, cancellationToken);

                if (mediaIds.Any())
                {
                    var result = await _mediaServiceProxy.GetAllMediaAsync(mediaIds, cancellationToken);
                    prescriptionDto.Media = result != null ? result.Success && result.Data != null ? result.Data : [] : ([]);
                }
                else
                {
                    prescriptionDto.Media = [];
                }

            }
            catch
            {
                prescriptionDto.Media = [];
            }

            return Response<PrescriptionResponseDto>.Ok(prescriptionDto);
        }

        public async Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WritePrescription, cancellationToken))
                return Response<bool>.Fail("Permission denied", 403);

            var prescription = await _unitOfWork.PrescriptionsRepository.GetByIdAsync(id, cancellationToken);
            if (prescription == null)
                return Response<bool>.Fail($"Prescription {id} not found", 404);

            await _unitOfWork.PrescriptionsRepository.DeleteAsync(id, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            // Publish event to Event Hub
            try
            {
                var @event = new MediaDeletedEvent { MediaIds = new List<int>() { id } };
                var eventData = new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event)));
                await _eventHubClient.SendAsync(new[] { eventData }, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log error but proceed (eventual consistency)
                Console.WriteLine($"Failed to publish event: {ex.Message}");
            }

            _memoryCache.Remove($"Prescription_{id}");

            return Response<bool>.Ok(true);
        }

        public async Task<Response<MediaResponseDto>> UploadMediaAsync(int prescriptionId, IFormFile file, int userId, CancellationToken cancellationToken = default)
        {
            if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WritePrescription, cancellationToken))
                return Response<MediaResponseDto>.Fail("Permission denied", 403);

            var prescription = await _unitOfWork.PrescriptionsRepository.GetByIdAsync(prescriptionId, cancellationToken);
            if (prescription == null)
                return Response<MediaResponseDto>.Fail($"Prescription {prescriptionId} not found", 404);

            if (file == null || file.Length == 0)
                return Response<MediaResponseDto>.Fail("No file provided", 400);

            try
            {
                using var content = new MultipartFormDataContent();
                using var stream = file.OpenReadStream();
                content.Add(new StreamContent(stream), "file", file.FileName);
                content.Add(new StringContent(prescriptionId.ToString()), "prescriptionId");

                var response = await _mediaServiceProxy.UploadMediaAsync(content, cancellationToken);
                if (!response.Success)
                    return Response<MediaResponseDto>.Fail("Failed to upload media", (int)response.StatusCode);

                if (response.Data == null)
                    return Response<MediaResponseDto>.Fail("Failed to retrieve uploaded media details", 500);

                _memoryCache.Remove($"Prescription_{prescriptionId}");

                return Response<MediaResponseDto>.Ok(response.Data);
            }
            catch (Exception ex)
            {
                return Response<MediaResponseDto>.Fail($"Failed to upload media: {ex.Message}", 500);
            }
        }

        public async Task<Response<IEnumerable<MediaResponseDto>>> GetMediaAsync(int prescriptionId, int userId, CancellationToken cancellationToken = default)
        {
            if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadPrescription, cancellationToken))
                return Response<IEnumerable<MediaResponseDto>>.Fail("Permission denied", 403);

            var prescription = await _unitOfWork.PrescriptionsRepository.GetByIdAsync(prescriptionId, cancellationToken);
            if (prescription == null)
                return Response<IEnumerable<MediaResponseDto>>.Fail($"Prescription {prescriptionId} not found", 404);

            try
            {
                var mediaIds = await _unitOfWork.PrescriptionsRepository.GetMediaIdsAsync(prescriptionId, cancellationToken);

                if (!mediaIds.Any())
                    return Response<IEnumerable<MediaResponseDto>>.Ok([]);

                var result = await _mediaServiceProxy.GetAllMediaAsync(mediaIds, cancellationToken);
                var mediaResponse = result != null ? result.Success && result.Data != null ? result.Data : [] : ([]);

                return Response<IEnumerable<MediaResponseDto>>.Ok(mediaResponse);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<MediaResponseDto>>.Fail($"Failed to retrieve media: {ex.Message}", 500);
            }
        }
    }
}
