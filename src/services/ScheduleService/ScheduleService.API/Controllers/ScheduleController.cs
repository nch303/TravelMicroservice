using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleService.Application.DTOs.Requests;
using ScheduleService.Application.DTOs.Responses;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;

namespace ScheduleService.API.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IScheduleParticipantService _scheduleParticipantService;
        private readonly ICheckItemParticipantService _checkItemParticipantService;
        private readonly IScheduleActivityService _scheduleActivityService;
        private readonly ICheckedItemService _checkedItemService;
        private readonly IScheduleMediaService _scheduleMediaService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IActivityAttendanceService _activityAttendanceService;
        private readonly INotificationService _notificationService;
        private readonly INotificationRecipientService _notificationRecipientService;

        public ScheduleController(IScheduleService scheduleService, IMapper mapper, IAuthServiceClient authServiceClient
            , IScheduleParticipantService scheduleParticipantService, IScheduleActivityService scheduleActivityService,
            ICheckedItemService checkedItemService, IScheduleMediaService scheduleMediaService, ICheckItemParticipantService checkItemParticipantService
            , IUserServiceClient userServiceClient, IActivityAttendanceService activityAttendanceService, INotificationService notificationService
            , INotificationRecipientService notificationRecipientService)
        {
            _scheduleService = scheduleService;
            _mapper = mapper;
            _authServiceClient = authServiceClient;
            _scheduleParticipantService = scheduleParticipantService;
            _scheduleActivityService = scheduleActivityService;
            _checkedItemService = checkedItemService;
            _scheduleMediaService = scheduleMediaService;
            _checkItemParticipantService = checkItemParticipantService;
            _userServiceClient = userServiceClient;
            _activityAttendanceService = activityAttendanceService;
            _notificationService = notificationService;
            _notificationRecipientService = notificationRecipientService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(Guid id)
        {
            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(id);
                var scheduleResponse = _mapper.Map<ScheduleResponse>(schedule);
                return Ok(scheduleResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{scheduleId}/activities/date(dd-MM-yyyy)/{date}")]
        public async Task<IActionResult> GetActivitiesByDate(Guid scheduleId, DateTime date)
        {
            try
            {
                // ✅ Parse input manually in "dd/MM/yyyy" format
                //if (!DateTime.TryParseExact(
                //        dateStr,
                //        "dd-MM-yyyy",
                //        System.Globalization.CultureInfo.InvariantCulture,
                //        System.Globalization.DateTimeStyles.None,
                //        out DateTime date))
                //{
                //    return BadRequest(new { message = "Invalid date format. Please use dd-MM-yyyy." });
                //}

                // ✅ Call service
                var activities = await _scheduleActivityService.GetActivitiesByDateAsync(scheduleId, date);
                var response = _mapper.Map<List<ScheduleActivityResponse>>(activities);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("share/{id}")]
        public async Task<IActionResult> ShareSchedule(Guid id)
        {
            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(id);
                var shareCode = await _scheduleService.ShareScheduleAsync(id);

                return Ok(new { sharedCode = shareCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("join")]
        [Authorize]
        public async Task<IActionResult> JoinSchedule([FromBody] JoinScheduleRequest request)
        {
            try
            {
                var user = await _authServiceClient.GetCurrentAccountAsync();
                await _scheduleService.JoinScheduleAsync(request.ShareCode, user!.Id);
                return Ok(new { message = "Successfully joined the schedule" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("participant/schedules/{participantId}")]
        public async Task<IActionResult> GetAllScheduleByParticipantId(Guid participantId)
        {
            try
            {
                var schedules = await _scheduleParticipantService.GetAllScheduleByParticipantIdAsync(participantId);
                var result = new List<Schedule>();
                foreach (var item in schedules)
                {
                    var schedule = await _scheduleService.GetScheduleByIdAsync(item.ScheduleId);
                    result.Add(schedule);
                }
                var schedulesResponse = _mapper.Map<List<ScheduleResponse>>(result);
                return Ok(schedulesResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("schedule/{id}")]
        public async Task<IActionResult> UpdateSchedule(Guid id, [FromBody] UpdateScheduleRequest newSchedule)
        {
            try
            {
                var updated = await _scheduleService.UpdateScheduleByIdAsync(_mapper.Map<Schedule>(newSchedule), id);
                var result = _mapper.Map<ScheduleResponse>(updated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest request)
        {
            try
            {
                var user = await _authServiceClient.GetCurrentAccountAsync();
                var schedule = _mapper.Map<Schedule>(request);
                schedule.OwnerId = user!.Id;
                schedule.Status = ScheduleStatus.Active;
                schedule.CreatedAt = DateTime.UtcNow;
                await _scheduleService.CreateScheduleAsync(schedule);
                // Add owner as a participant
                var participant = new ScheduleParticipant
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedule.Id,
                    UserId = user.Id,
                    Role = ParticipantRole.Owner,
                    Status = ParticipantStatus.Active
                };
                await _scheduleParticipantService.AddScheduleParticipantAsync(participant);
                await _scheduleService.SaveChangesAsync();
                var scheduleResponse = _mapper.Map<ScheduleResponse>(schedule);
                return Ok(scheduleResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("schedule/{id}/cancel")]
        public async Task<IActionResult> CancelSchedule(Guid id)
        {
            try
            {
                var canceled = await _scheduleService.CancelScheduleAsync(id);
                if (canceled == true)
                {
                    return Ok(new { message = "Schedule canceled" });
                }
                else
                {
                    return BadRequest(new { message = "Schedule cancel unsuccessfull" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("schedule/{id}/restore")]
        public async Task<IActionResult> RestoreSchedule(Guid id)
        {
            try
            {
                var restored = await _scheduleService.RestoreScheduleAsync(id);
                if (restored == true)
                {
                    return Ok(new { message = "Schedule restored" });
                }
                else
                {
                    return BadRequest(new { message = "Schedule restore unsuccessfull" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("activity/add")]
        [Authorize]
        public async Task<IActionResult> AddActivityToSchedule([FromBody] CreateScheduleActivityRequest request)
        {
            try
            {
                var activity = _mapper.Map<ScheduleActivity>(request);
                await _scheduleActivityService.AddActivityAsync(activity);
                var activityResponse = _mapper.Map<ScheduleActivityResponse>(activity);
                return Ok(activityResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{scheduleId}/leave/{userId}")]
        [Authorize]
        public async Task<IActionResult> LeaveSchedule(Guid scheduleId)
        {
            try
            {
                var result = await _scheduleParticipantService.LeaveScheduleAsync(scheduleId);
                var response = _mapper.Map<LeaveScheduleResponse>(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{scheduleId}/kick/{participantId}")]
        [Authorize]
        public async Task<IActionResult> KickParticipantOutOfSchedule(Guid scheduleId, Guid participantId)
        {
            try
            {
                var result = await _scheduleParticipantService.KickParticipantAsync(scheduleId, participantId);
                var response = _mapper.Map<LeaveScheduleResponse>(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{scheduleId}/change-role/{participantId}")]
        [Authorize]
        public async Task<IActionResult> ChangeParticipantRole(Guid scheduleId, Guid participantId)
        {
            try
            {
                var participant = await _scheduleParticipantService.ChangeParticipantRoleAsync(participantId, scheduleId);
                var response = _mapper.Map<ScheduleParticipantResponse>(participant);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("activities/getAvailable/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetAvailableActivitiesByScheduleId(Guid scheduleId)
        {
            try
            {
                var activities = await _scheduleActivityService.GetActivitiesByScheduleIdAsync(scheduleId);
                var activitiesResponse = _mapper.Map<List<ScheduleActivityResponse>>(activities);
                return Ok(activitiesResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("activities/getAll/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetAllActivitiesByScheduleId(Guid scheduleId)
        {
            try
            {
                var activities = await _scheduleActivityService.GetAllActivitiesByScheduleIdAsync(scheduleId);
                var activitiesResponse = _mapper.Map<List<ScheduleActivityResponse>>(activities);
                return Ok(activitiesResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/activities")]
        public async Task<IActionResult> GetActivities(Guid id)
        {
            try
            {
                var activities = await _scheduleActivityService.GetActivitiesByScheduleIdAsync(id);
                var response = _mapper.Map<List<ScheduleActivityResponse>>(activities);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("checked-items/add")]
        [Authorize]
        public async Task<IActionResult> AddCheckedItemToActivity([FromBody] List<CreateCheckedItemRequest> request)
        {
            try
            {
                var checkedItems = _mapper.Map<List<CheckedItem>>(request);
                await _checkedItemService.AddCheckedItemsAsync(checkedItems);
                var checkedItemResponse = _mapper.Map<List<CheckedItemResponse>>(checkedItems);
                return Ok(checkedItemResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("activities/{activityId}")]
        public async Task<IActionResult> UpdateActivity(int activityId, [FromBody] UpdateActivityRequest newActivity)
        {
            try
            {
                var updated = await _scheduleActivityService.UpdateActivityById(_mapper.Map<ScheduleActivity>(newActivity), activityId);
                var response = _mapper.Map<ScheduleActivityResponse>(updated);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("activities/{newIndex}/{activityId}")]
        public async Task<IActionResult> UpdateActivityIndexActivity(int activityId, int newIndex)
        {
            try
            {
                await _scheduleActivityService.UpdateOrderIndexById(newIndex, activityId);
                return Ok(new { message = "Update order successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("activities/{activityId}")]
        public async Task<IActionResult> DeleteActivity(int activityId)
        {
            try
            {
                await _scheduleActivityService.DeleteActivityById(activityId);
                return Ok(new { message = "Activity deleted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("activities/restore/{activityId}")]
        public async Task<IActionResult> RestoreActivity(int activityId)
        {
            try
            {
                await _scheduleActivityService.RestoreActivityById(activityId);
                return Ok(new { message = "Activity restored" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("activities/check-in")]
        [Authorize]
        public async Task<IActionResult> CheckInActivity([FromBody] AttendanceRequest request)
        {
            try
            {
                var user = await _authServiceClient.GetCurrentAccountAsync();
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                await _activityAttendanceService.CheckInAsync(request.ActivityId, user.Id);
                return Ok(new { message = "Check-in successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("activities/check-out")]
        [Authorize]
        public async Task<IActionResult> CheckOutActivity([FromBody] AttendanceRequest request)
        {
            try
            {
                var user = await _authServiceClient.GetCurrentAccountAsync();
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                await _activityAttendanceService.CheckOutAsync(request.ActivityId, user.Id);
                return Ok(new { message = "Check-out successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("checkitems/{checkedItemId}/participants/toggle")]
        [Authorize]
        public async Task<IActionResult> ToggleCheck(int checkedItemId, [FromQuery] bool isChecked)
        {
            try
            {
                await _checkItemParticipantService.ToggleCheckAsync(checkedItemId, isChecked);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("checkitems/bulk")]
        public async Task<IActionResult> DeleteManyCheckedItems([FromBody] List<int> checkItemIds)
        {
            try
            {
                await _checkedItemService.DeleteManyById(checkItemIds);
                return Ok(new { message = "Delete chosen items successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("checked-items/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetCheckedItemsByScheduleId(Guid scheduleId)
        {
            try
            {
                var items = await _checkedItemService.GetByScheduleIdAsync(scheduleId);
                var response = _mapper.Map<List<CheckedItemResponse>>(items);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("available-checked-items/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetAvailableCheckedItemsByScheduleId(Guid scheduleId)
        {
            try
            {
                var items = await _checkedItemService.GetAvailableByScheduleIdAsync(scheduleId);
                var response = _mapper.Map<List<CheckedItemResponse>>(items);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("media/upload")]
        [Authorize]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> UploadMedia([FromForm] UploadScheduleMediaRequest request)
        {
            try
            {
                var media = await _scheduleMediaService.UploadAsync(request);
                var response = _mapper.Map<ScheduleMediaResponse>(media);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("participants/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetParticipantsByScheduleId(Guid scheduleId)
        {
            try
            {
                var (participants, users) = await _scheduleParticipantService.GetAllParticipantByScheduleIdAsync(scheduleId);

                var response = participants.Select(p =>
                {
                    var user = users.FirstOrDefault(u => u.Id == p.UserId)
                               ?? new UserServiceClientResponse { Id = p.UserId, Name = "Unknown" };

                    return _mapper.Map<GetAllParticipantsResponse>((p, user));
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{scheduleId}/add-participants-by-email")]
        [Authorize]
        public async Task<IActionResult> AddParticipantByEmail(Guid scheduleId, [FromQuery] string email)
        {
            try
            {
                var participant = await _scheduleParticipantService.AddParticipantByEmailAsync(scheduleId, email);
                var response = _mapper.Map<ScheduleParticipantResponse>(participant);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("notification/create")]
        [Authorize]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            try
            {
                var user = await _authServiceClient.GetCurrentAccountAsync();
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var notification = _mapper.Map<Notification>(request);
                notification.SenderId = user.Id;
                notification.CreatedAt = DateTime.UtcNow;
                notification.Type = NotificationType.OwnerAnnouncement;

                await _notificationService.CreateNotificationAsync(notification);
                return Ok(new { message = "Create notification successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("notification/recipients")]
        [Authorize]
        public async Task<IActionResult> GetAllNotificationRecipientsByUserId()
        {
            try
            {
                var currentUser = await _authServiceClient.GetCurrentAccountAsync();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var profile = new ProfileResponse();
                var userServiceClientResponse = new List<UserServiceClientResponse>();
                var recipients = await _notificationRecipientService.GetAllNotificationRecipientsByUserIdAsync(currentUser.Id);
                var responses = _mapper.Map<List<NotificationRecipientResponse>>(recipients);
                foreach (var recipient in responses)
                {
                    userServiceClientResponse = await _userServiceClient.GetUsersByIdsAsync(new List<Guid> { recipient.SenderId });
                    recipient.SenderName = userServiceClientResponse.FirstOrDefault(ur => ur.Id == recipient.SenderId)!.Name;
                }
                responses.OrderByDescending(r => r.CreatedAt);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("notification/recipients/read")]
        [Authorize]
        public async Task<IActionResult> UpdateNoticationRecipient(ReadNotificationRecipientRequest request)
        {
            try
            {
                await _notificationRecipientService.UpdateNoticationRecipientAsync(request.notificationRecipientId);
                return Ok(new { message = "Read notification recipient successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
