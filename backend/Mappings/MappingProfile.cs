using AutoMapper;
using HouseholdBudgetApi.DTOs.Auth;
using HouseholdBudgetApi.DTOs.Category;
using HouseholdBudgetApi.DTOs.Dashboard;
using HouseholdBudgetApi.DTOs.Household;
using HouseholdBudgetApi.Entities;

namespace HouseholdBudgetApi.Mappings;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth DTOs
        CreateMap<User, CurrentUserDto>()
            .ReverseMap();

        // Category DTOs
        CreateMap<Category, CategoryDto>()
            .ReverseMap();

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // Household DTOs
        CreateMap<Household, HouseholdDto>()
            .ReverseMap();

        CreateMap<User, HouseholdMemberDto>();
    }
}
