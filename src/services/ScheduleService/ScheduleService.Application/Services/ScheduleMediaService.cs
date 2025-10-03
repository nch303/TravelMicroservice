using System;
using System.IO;
using System.Threading.Tasks;
using ScheduleService.Application.DTOs.Requests;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;

namespace ScheduleService.Application.Services
{
    public class ScheduleMediaService : IScheduleMediaService
    {
        private readonly IMediaStorageService _storage;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleActivityRepository _scheduleActivityRepository;
        private readonly IScheduleParticipantRepository _participantRepository;
        private readonly IScheduleMediaRepository _mediaRepository;

        public ScheduleMediaService(
            IMediaStorageService storage,
            IAuthServiceClient authServiceClient,
            IScheduleRepository scheduleRepository,
            IScheduleActivityRepository scheduleActivityRepository,
            IScheduleParticipantRepository participantRepository,
            IScheduleMediaRepository mediaRepository)
        {
            _storage = storage;
            _authServiceClient = authServiceClient;
            _scheduleRepository = scheduleRepository;
            _scheduleActivityRepository = scheduleActivityRepository;
            _participantRepository = participantRepository;
            _mediaRepository = mediaRepository;
        }

        public async Task<ScheduleMedia> UploadAsync(UploadScheduleMediaRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                throw new Exception("File không hợp lệ");

            // Chọn media type theo extension
            var ext = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            var mediaType = (ext == ".mp4" || ext == ".mov" || ext == ".avi" || ext == ".mkv") ? MediaType.Video : MediaType.Image;

            // Chỉ cho phép một trong hai
            if ((request.ScheduleId.HasValue && request.ActivityId.HasValue) ||
                (!request.ScheduleId.HasValue && !request.ActivityId.HasValue))
                throw new Exception("Phải chọn ScheduleId hoặc ActivityId (chỉ một)");

            var user = await _authServiceClient.GetCurrentAccountAsync();
            Guid? scheduleId = request.ScheduleId;

            if (request.ActivityId.HasValue)
            {
                // Lấy activity, suy ra scheduleId để kiểm tra quyền
                var activity = await _scheduleActivityRepository.GetActivytyByIdAsync(request.ActivityId.Value);
                if (activity == null) throw new Exception("Activity không tồn tại");
                scheduleId = activity.ScheduleId;
            }

            // Kiểm tra schedule tồn tại
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId!.Value);
            if (schedule == null) throw new Exception("Schedule không tồn tại");

            // Kiểm tra user là participant
            var participant = await _participantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, schedule.Id);
            if (participant == null)
                throw new Exception("Bạn không có quyền upload media vào lịch này");

            // Lưu file
            var url = await _storage.SaveAsync(request.File);

            var media = new ScheduleMedia
            {
                MediaType = mediaType,
                Url = url,
                Description = request.Description ?? string.Empty,
                UploadedAt = DateTime.UtcNow,
                UploadedUserId = user.Id,
                UploadMethod = request.UploadMethod,
                ScheduleId = request.ScheduleId,
                ActivityId = request.ActivityId
            };

            await _mediaRepository.AddAsync(media);
            return media;
        }
    }
}