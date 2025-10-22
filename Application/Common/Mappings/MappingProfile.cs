using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Motorcycle mappings
        CreateMap<Motorcycle, MotorcycleDto>()
            .ForMember(dest => dest.Identificador, opt => opt.MapFrom(src => src.Identifier))
            .ForMember(dest => dest.Ano, opt => opt.MapFrom(src => src.Year))
            .ForMember(dest => dest.Modelo, opt => opt.MapFrom(src => src.Model))
            .ForMember(dest => dest.Placa, opt => opt.MapFrom(src => src.LicensePlate));
            
        CreateMap<CreateMotorcycleDto, Motorcycle>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Identifier, opt => opt.MapFrom(src => src.Identificador))
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Ano))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Modelo))
            .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.Placa))
            .ForMember(dest => dest.Rentals, opt => opt.Ignore());

        // Courier mappings
        CreateMap<Courier, CourierDto>()
            .ForMember(dest => dest.Identificador, opt => opt.MapFrom(src => src.Identifier))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.CNPJ))
            .ForMember(dest => dest.DataNascimento, opt => opt.MapFrom(src => src.BirthDate.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.NumeroCnh, opt => opt.MapFrom(src => src.DriverLicenseNumber))
            .ForMember(dest => dest.TipoCnh, opt => opt.MapFrom(src => src.DriverLicenseType.ToString()))
            .ForMember(dest => dest.ImagemCnh, opt => opt.MapFrom(src => src.DriverLicenseImagePath));
            
        CreateMap<CreateCourierDto, Courier>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Identifier, opt => opt.MapFrom(src => src.Identificador))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.CNPJ, opt => opt.MapFrom(src => src.Cnpj))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DataNascimento)))
            .ForMember(dest => dest.DriverLicenseNumber, opt => opt.MapFrom(src => src.NumeroCnh))
            .ForMember(dest => dest.DriverLicenseType, opt => opt.MapFrom(src => ParseDriverLicenseType(src.TipoCnh)))
            .ForMember(dest => dest.DriverLicenseImagePath, opt => opt.MapFrom(src => src.ImagemCnh))
            .ForMember(dest => dest.Rentals, opt => opt.Ignore());

        // Rental mappings
        CreateMap<Rental, RentalDto>()
            .ForMember(dest => dest.Identificador, opt => opt.MapFrom(src => src.Identifier))
            .ForMember(dest => dest.ValorDiaria, opt => opt.MapFrom(src => src.DailyCost))
            .ForMember(dest => dest.EntregadorId, opt => opt.MapFrom(src => src.Courier != null ? src.Courier.Identifier : src.CourierId.ToString()))
            .ForMember(dest => dest.MotoId, opt => opt.MapFrom(src => src.Motorcycle != null ? src.Motorcycle.Identifier : src.MotorcycleId.ToString()))
            .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.StartDate.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.DataTermino, opt => opt.MapFrom(src => src.ExpectedEndDate.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.DataPrevisaoTermino, opt => opt.MapFrom(src => src.ExpectedEndDate.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.DataDevolucao, opt => opt.MapFrom(src => src.ActualEndDate.HasValue ? src.ActualEndDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null));
            
        CreateMap<CreateRentalDto, Rental>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Identifier, opt => opt.Ignore())
            .ForMember(dest => dest.CourierId, opt => opt.Ignore()) // Set manually in handler
            .ForMember(dest => dest.MotorcycleId, opt => opt.Ignore()) // Set manually in handler
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DataInicio)))
            .ForMember(dest => dest.ExpectedEndDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DataPrevisaoTermino)))
            .ForMember(dest => dest.PlanType, opt => opt.MapFrom(src => (RentalPlanType)src.Plano))
            .ForMember(dest => dest.DailyCost, opt => opt.Ignore())
            .ForMember(dest => dest.TotalCost, opt => opt.Ignore())
            .ForMember(dest => dest.ActualEndDate, opt => opt.Ignore())
            .ForMember(dest => dest.Courier, opt => opt.Ignore())
            .ForMember(dest => dest.Motorcycle, opt => opt.Ignore());
    }

    private static DriverLicenseType ParseDriverLicenseType(string type)
    {
        return type.ToUpper() switch
        {
            "A" => DriverLicenseType.A,
            "B" => DriverLicenseType.B,
            "A+B" or "AB" => DriverLicenseType.AB,
            _ => throw new ArgumentException($"Invalid driver license type: {type}")
        };
    }
}
