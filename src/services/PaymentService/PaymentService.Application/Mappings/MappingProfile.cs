using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaymentService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<CreateChatGroupRequest, ChatGroup>()
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
            //    .ForMember(dest => dest.GroupType, opt => opt.MapFrom(src => src.GroupType));
        }
    }
}
