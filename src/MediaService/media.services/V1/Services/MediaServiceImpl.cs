using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using media.models.V1.Db;
using media.models.V1.Dto;
using media.repositories.V1.Contracts;
using media.services.V1.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using shared.V1.HelperClasses;
using shared.V1.HelperClasses.Contracts;
using shared.V1.Models;

namespace media.services.V1.Services;

public class MediaServiceImpl(IUnitOfWork _unitOfWork,
            IMapper _mapper,
            IAuthServiceProxy _authServiceProxy,
            IMemoryCache _memoryCache,
            IBlobServiceClientProvider _blobClientProvider,
            IConfiguration _configuration) : IMediaService
{

    private readonly string _containerName = _configuration["AzureBlobStorage:ContainerName"]!; // configured in dev.appconfig.env
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _sasTokenDuration = TimeSpan.FromDays(7);
    private readonly BlobServiceClient _blobServiceClient = _blobClientProvider.Client;

    private string GenerateSasToken(BlobClient blobClient)
    {
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresOn = DateTimeOffset.UtcNow.Add(_sasTokenDuration),
            Protocol = SasProtocol.Https
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        return blobClient.GenerateSasUri(sasBuilder).Query;
    }

    private async Task<(StreamContent? FileContent, string? FileName, string? ContentType)> ExtractFileFromMultipart(MultipartFormDataContent content, CancellationToken cancellationToken)
    {
        var fileContent = content.FirstOrDefault(c => c.Headers.ContentDisposition?.Name?.Trim('"') == "file");
        if (fileContent == null)
            return (null, null, null);

        var streamContent = fileContent as StreamContent ?? new StreamContent(await fileContent.ReadAsStreamAsync(cancellationToken));
        var fileName = fileContent.Headers.ContentDisposition?.FileName?.Trim('"') ?? "unknown";
        var contentType = fileContent.Headers.ContentType?.MediaType ?? "application/octet-stream";

        return (streamContent, fileName, contentType);
    }

    public async Task<Response<MediaResponseDto>> UploadAsync(MultipartFormDataContent content, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteMedia, cancellationToken))
            return Response<MediaResponseDto>.Fail("Permission denied", 403);

        var (fileContent, fileName, contentType) = await ExtractFileFromMultipart(content, cancellationToken);
        if (fileContent == null)
            return Response<MediaResponseDto>.Fail("No file provided", 400);

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            using var stream = await fileContent.ReadAsStreamAsync(cancellationToken);
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            }, cancellationToken);

            var sasToken = GenerateSasToken(blobClient);

            var media = new Media
            {
                FileName = fileName!,
                FileUrl = blobClient.Uri.ToString(),
                ContentType = contentType!
            };

            await _unitOfWork.MediaRepository.AddAsync(media, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            _memoryCache.Remove($"Media_{media.MediaId}");

            var mediaDto = _mapper.Map<MediaResponseDto>(media);
            mediaDto.FileUrlWithSas = $"{media.FileUrl}{sasToken}";
            return Response<MediaResponseDto>.Ok(mediaDto);
        }
        catch (Exception ex)
        {
            return Response<MediaResponseDto>.Fail($"Failed to upload media: {ex.Message}", 500);
        }
        finally
        {
            fileContent?.Dispose();
        }
    }

    public async Task<Response<IEnumerable<MediaResponseDto>>> GetByIdsAsync(IEnumerable<int> mediaIds, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadMedia, cancellationToken))
            return Response<IEnumerable<MediaResponseDto>>.Fail("Permission denied", 403);

        var cacheKey = $"Media_{string.Join("_", mediaIds.OrderBy(id => id))}";
        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<MediaResponseDto>? cachedMedia))
            return Response<IEnumerable<MediaResponseDto>>.Ok(cachedMedia!);

        try
        {
            var media = await _unitOfWork.MediaRepository.GetByIdsAsync(mediaIds, cancellationToken);
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            var mediaDtos = media.Select(m =>
            {
                var blobClient = containerClient.GetBlobClient(Path.GetFileName(m.FileUrl));
                var sasToken = GenerateSasToken(blobClient);
                var dto = _mapper.Map<MediaResponseDto>(m);
                dto.FileUrlWithSas = $"{m.FileUrl}{sasToken}";
                return dto;
            }).ToList();

            _memoryCache.Set(cacheKey, mediaDtos, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            });

            return Response<IEnumerable<MediaResponseDto>>.Ok(mediaDtos);
        }
        catch (Exception ex)
        {
            return Response<IEnumerable<MediaResponseDto>>.Fail($"Failed to retrieve media: {ex.Message}", 500);
        }
    }

}
