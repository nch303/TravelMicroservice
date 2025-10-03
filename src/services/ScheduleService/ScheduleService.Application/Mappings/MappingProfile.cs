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
            CreateMap<UpdateScheduleRequest, Schedule>();
            CreateMap<CreateScheduleRequest, Schedule>();
            CreateMap<ScheduleActivity, ScheduleActivityResponse>();
            CreateMap<CreateScheduleActivityRequest, ScheduleActivity>();
            CreateMap<CreateCheckedItemRequest, CheckedItem>();
            CreateMap<CheckedItem, CheckedItemResponse>();
            CreateMap<ScheduleMedia, ScheduleMediaResponse>();

            CreateMap<Schedule, LeaveScheduleResponse>()
                .ForMember(dest => dest.ParticipantCounts, opt => opt.MapFrom(src => src.ParticipantsCount))
                .ForMember(dest => dest.ScheduleParticipantResponses, opt => opt.MapFrom(src => src.ScheduleParticipants));

            CreateMap<ScheduleParticipant, ScheduleParticipantResponse>();

            CreateMap<UpdateActivityRequest, ScheduleActivity>();

            
        }
    }
}
