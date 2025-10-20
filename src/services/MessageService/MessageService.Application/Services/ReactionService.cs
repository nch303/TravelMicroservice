using MessageService.Application.IServices;
using MessageService.Domain.Entities;
using MessageService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _reactionRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IChatMessageRepository _chatMessageRepository;

        public ReactionService(IReactionRepository reactionRepository, IChatParticipantRepository chatParticipantRepository
            , IChatMessageRepository chatMessageRepository)
        {
            _reactionRepository = reactionRepository;
            _chatParticipantRepository = chatParticipantRepository;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task AddReactionAsync(Guid userId, MessageReaction messageReaction)
        {
            // Get message to check if it exists
            var message = await _chatMessageRepository.GetMessageByIdAsync(messageReaction.MessageId);
            if (message == null)
            {
                throw new Exception("Message not found.");
            }

            // Check if the user is a participant of the chat group
            var participant = await _chatParticipantRepository.GetParticipantAsync(message.GroupId, userId);
            if (participant == null)
            {
                throw new Exception("You are not a participant of this chat group.");
            }

            // Check if the user has already reacted to the message, update type of the existing reaction
            var existingReactions = await _reactionRepository.GetReactionsByMessageIdAndUserIdAsync(messageReaction.MessageId, messageReaction.ParticipantId);
            if (existingReactions != null)
            {
                existingReactions.ReactionType = messageReaction.ReactionType;
                await _reactionRepository.SaveChanges();
            }
            else
            {
                messageReaction.ParticipantId = participant.Id;
                await _reactionRepository.AddReactionAsync(messageReaction);
            }
        }

        public async Task<MessageReaction> RemoveReactionAsync(Guid reactionId, Guid userId)
        {
            // Check if the user has already reacted to the message, update type of the existing reaction
            var existingReaction = await _reactionRepository.GetReactionsByIdAsync(reactionId);
            if (existingReaction != null && existingReaction.Participant.UserId == userId)
            {
                existingReaction.IsDeleted = true;
                await _reactionRepository.SaveChanges();
            }
            else
            {
                throw new Exception("Reaction not found or you are not authorized to remove this reaction.");
            }
            return existingReaction;
        }

        public async Task<List<MessageReaction>?> GetReactionsByMessageIdAsync(Guid messageId)
        {
            // Get message to check if it exists
            var message = await _chatMessageRepository.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                throw new Exception("Message not found.");
            }

            return await _reactionRepository.GetReactionsByMessageIdAsync(messageId);
        }
    }
}
