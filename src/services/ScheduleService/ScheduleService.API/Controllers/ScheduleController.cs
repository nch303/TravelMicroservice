using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleService.Application.DTOs.Requests;
using ScheduleService.Application.DTOs.Responses;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Application.Services;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        private readonly IScheduleActivityService _scheduleActivitiesService;

        public ScheduleController(IScheduleService scheduleService, IMapper mapper, IAuthServiceClient authServiceClient
            , IScheduleParticipantService scheduleParticipantService)
        {
            _scheduleService = scheduleService;
            _mapper = mapper;
            _authServiceClient = authServiceClient;
            _scheduleParticipantService = scheduleParticipantService;
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

        [HttpPatch("schedule/{id}/cancel")]
        public async Task<IActionResult> CancelSchedule(Guid id)
        {
            try
            {
                var canceled = await _scheduleService.CancelScheduleAsync(id);
                if(canceled == true)
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

        [HttpPost("schedule/{scheduleId}/leave/{userId}")]
        public async Task<IActionResult> LeaveSchedule(Guid scheduleId, Guid userId)
        {
            await _scheduleParticipantService.LeaveScheduleAsync(scheduleId, userId);
            return NoContent();
        }

        [HttpGet("schedule/{id}/activities")]
        public async Task<IActionResult> GetActivities(Guid id)
        {
            var activities = await _scheduleActivitiesService.GetActiviyListByScheduleId(id);
            return Ok(activities);
        }

        [HttpPut("schedule/activities/{activityId}")]
        public async Task<IActionResult> UpdateActivity(int activityId, [FromBody] ScheduleActivity newActivity)
        {
            var updated = await _scheduleActivitiesService.UpdateActivityById(newActivity, activityId);
            return Ok(updated);
        }

        [HttpDelete("schedule/activities/{activityId}")]
        public async Task<IActionResult> DeleteActivity(int activityId)
        {
            await _scheduleActivitiesService.DeleteActivityById(activityId);
            return NoContent();
        }

        [HttpPut("schedule/checkitems")]
        public async Task<IActionResult> UpdateCheckedItem([FromBody] CheckedItemParticipant entity)
        {
            var updated = await _checkItemParticipantService.UpdateAsync(entity);
            return Ok(updated);
        }

        [HttpPatch("schedule/checkitems/{checkedItemId}/participants/{participantId}/toggle")]
        public async Task<IActionResult> ToggleCheck(int checkedItemId, Guid participantId, [FromQuery] bool isChecked)
        {
            await _checkItemParticipantService.ToggleCheckAsync(checkedItemId, participantId, isChecked);
            return NoContent();
        }

        [HttpDelete("schedule/checkitems/bulk")]
        public async Task<IActionResult> DeleteManyCheckedItems([FromBody] List<(int checkedItemId, Guid scheduleParticipantId)> keys)
        {
            await _checkItemParticipantService.DeleteManyAsync(keys);
            return NoContent();
        }
    }
}
