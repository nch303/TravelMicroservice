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
                .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => src.MessageType))
                .ForMember(dest => dest.ReactionCount, opt => opt.MapFrom(src => src.Reactions.Count))
                .ForMember(dest => dest.ReactionSummary, opt => opt.MapFrom(src =>
                    src.Reactions
                        .GroupBy(r => r.ReactionType)
                        .Select(g => new ReactionSummaryResponse
                        {
                            Type = g.Key.ToString(),
                            Count = g.Count()
                        })
                        .ToList()
                ))
                .ForMember(dest => dest.ReaderSummary, opt => opt.MapFrom(src =>
                    src.Reads
                        .Select(r => new ReaderSummaryResponse
                        {
                            UserId = r.Reader.UserId,
                            ReaderName = "",
                            ReadAt = r.ReadAt
                        }
                        ).ToList()
                ));

            CreateMap<EditMessageRequest, ChatMessage>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.NewContent));

            CreateMap<CreateReactionRequest, MessageReaction>()
                .ForMember(dest => dest.ReactionType, opt => opt.MapFrom(src => src.ReactionType))
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId));

            CreateMap<MessageReaction, ReactionResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ReactionType, opt => opt.MapFrom(src => src.ReactionType))
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId))
                .ForMember(dest => dest.ParticipantId, opt => opt.MapFrom(src => src.ParticipantId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));


            CreateMap<MessageRead, ReaderResponse>()
                .ForMember(dest => dest.ReaderId, opt => opt.MapFrom(src => src.ReaderId))
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId)) 
                .ForMember(dest => dest.ReadAt, opt => opt.MapFrom(src => src.ReadAt));

            
            CreateMap<CreateNotificationRequest, Notification>()
                .ForMember(dest => dest.ChatMessageId, opt => opt.MapFrom(src => src.ChatMessageId))
                .ForMember(dest => dest.ChatGroupId, opt => opt.MapFrom(src => src.ChatGroupId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<Notification, NotificationResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ChatGroupId, opt => opt.MapFrom(src => src.ChatGroupId))
                .ForMember(dest => dest.ChatGroupName, opt => opt.MapFrom(src => src.ChatGroup != null ? src.ChatGroup.Name : null))
                .ForMember(dest => dest.ChatMessageId, opt => opt.MapFrom(src => src.ChatMessageId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
