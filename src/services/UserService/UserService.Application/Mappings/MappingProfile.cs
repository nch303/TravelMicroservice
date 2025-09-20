using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Requests;
using UserService.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdateUserRequest, User>();


            //CreateMap<ClassTeacher, ClassTeacherResponse>()
            //    .ForMember(dest => dest.TeacherID, opt => opt.MapFrom(src => src.TeacherID))
            //    .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teachers!.FullName));

        }
    }
}
