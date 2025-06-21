using AutoMapper;
using department.models.V1.Db;
using department.models.V1.Dto;

namespace department.services.V1.Mappings;

public class DepartmentMappingProfile : Profile
{
    public DepartmentMappingProfile()
    {
        CreateMap<CreateDepartmentRequestDto, Department>();
        CreateMap<Department, DepartmentResponseDto>();
    }
}
