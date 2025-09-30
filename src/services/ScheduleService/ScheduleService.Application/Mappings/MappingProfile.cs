using AutoMapper;
using ScheduleService.Application.DTOs.Requests;
using ScheduleService.Application.DTOs.Responses;
using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Schedule, ScheduleResponse>();
            CreateMap<CreateScheduleRequest, Schedule>();
            CreateMap<ScheduleActivity, ScheduleActivityResponse>();
            CreateMap<CreateScheduleActivityRequest, ScheduleActivity>();
            CreateMap<CreateCheckedItemRequest, CheckedItem>();
            CreateMap<CheckedItem, CheckedItemResponse>();

        }
    }
}
