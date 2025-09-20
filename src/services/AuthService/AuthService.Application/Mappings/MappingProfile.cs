using AuthService.Application.DTOs.Requests;
using AuthService.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, Account>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
                
            CreateMap<RegisterRequest, CreateProfileRequest>();

            //CreateMap<ClassTeacher, ClassTeacherResponse>()
            //    .ForMember(dest => dest.TeacherID, opt => opt.MapFrom(src => src.TeacherID))
            //    .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teachers!.FullName));

        }
    }
}
