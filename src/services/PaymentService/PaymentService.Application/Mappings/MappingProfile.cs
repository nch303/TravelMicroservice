using AutoMapper;
using PaymentService.Application.DTOs.Responses;
using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaymentService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, TransactionResponse>();
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
            //    .ForMember(dest => dest.GroupType, opt => opt.MapFrom(src => src.GroupType));
        }
    }
}
