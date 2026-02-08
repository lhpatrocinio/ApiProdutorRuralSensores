using AutoMapper;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.DTOs.Response;
using ProdutorRuralSensores.Domain.Entities;

namespace ProdutorRuralSensores.Api.Extensions.Mappers;

/// <summary>
/// Perfil de mapeamento AutoMapper para a API de Sensores
/// </summary>
public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // Sensor mappings
        CreateMap<SensorCreateRequest, Sensor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Ativo, opt => opt.Ignore())
            .ForMember(dest => dest.UltimaLeitura, opt => opt.Ignore())
            .ForMember(dest => dest.Leituras, opt => opt.Ignore());

        CreateMap<SensorUpdateRequest, Sensor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TalhaoId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UltimaLeitura, opt => opt.Ignore())
            .ForMember(dest => dest.Leituras, opt => opt.Ignore());

        CreateMap<Sensor, SensorResponse>();

        CreateMap<Sensor, SensorComLeiturasResponse>()
            .ForMember(dest => dest.Leituras, opt => opt.MapFrom(src => src.Leituras));

        // Leitura mappings
        CreateMap<LeituraCreateRequest, LeituraSensor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Sensor, opt => opt.Ignore())
            .ForMember(dest => dest.DataHoraLeitura, opt => opt.MapFrom(src => src.DataHoraLeitura ?? DateTime.UtcNow));

        CreateMap<LeituraSensor, LeituraResponse>()
            .ForMember(dest => dest.CodigoSensor, opt => opt.MapFrom(src => src.Sensor != null ? src.Sensor.Codigo : null));
    }
}
