using AutoMapper;
using CategoryService.Application.DTOs;
using CategoryService.Domain.Entities;

namespace CategoryService.Application.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();

            CreateMap<CategoryWriteDto, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); ;

            CreateMap<Category, CategoryMinimalDto>();
        }
    }
}
