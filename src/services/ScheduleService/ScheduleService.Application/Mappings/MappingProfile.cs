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

            CreateMap<(ScheduleParticipant participant, UserServiceClientResponse user), GetAllParticipantsResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.participant.UserId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.user.Name))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.participant.Role.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.participant.Status.ToString()))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.user.AvatarUrl));

            CreateMap<CreateNotificationRequest, Notification>()
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
                .ForMember(dest => dest.RecipientId, opt => opt.MapFrom(src => src.RecipientId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));

            CreateMap<NotificationRecipient, NotificationRecipientResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Notification.Title))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Notification.Message))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Notification.Type))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Notification.CreatedAt))
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.Notification.SenderId))
                .ForMember(dest => dest.RecipientId, opt => opt.MapFrom(src => src.RecipientId))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.ReadAt, opt => opt.MapFrom(src => src.ReadAt));

            CreateMap<CheckedItemParticipant, CheckedItemParticipantResponse>()
                .ForMember(dest => dest.CheckedItemName, opt => opt.MapFrom(src => src.CheckedItem.Name));

            CreateMap<ActivityAttendance, AttendanceResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


        }
    }
}
