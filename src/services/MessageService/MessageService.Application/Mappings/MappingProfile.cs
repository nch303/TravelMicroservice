using AutoMapper;
using MessageService.Application.DTOs.Requests;
using MessageService.Application.DTOs.Responses;
using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateChatGroupRequest, ChatGroup>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
                .ForMember(dest => dest.GroupType, opt => opt.MapFrom(src => src.GroupType));

            CreateMap<ChatGroup, ChatGroupResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
                .ForMember(dest => dest.GroupType, opt => opt.MapFrom(src => src.GroupType))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<SendMessageRequest, ChatMessage>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId));

            CreateMap<ChatMessage, ChatMessageResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.EditAt, opt => opt.MapFrom(src => src.EditAt))
                .ForMember(dest => dest.ParentMessageId, opt => opt.MapFrom(src => src.ParentMessageId))
                .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => src.MessageType));

            CreateMap<EditMessageRequest, ChatMessage>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.NewContent));

            //CreateMap<ClassTeacher, ClassTeacherResponse>()
            //    .ForMember(dest => dest.TeacherID, opt => opt.MapFrom(src => src.TeacherID))
            //    .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teachers!.FullName));

        }
    }
}
