using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;
using ScheduleService.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Infrastructure.Repositories
{
    public class ActivityAttendanceRepository : IActivityAttendanceRepository
    {
        private readonly AppDbContext _context;

        public ActivityAttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAttendanceAsync(ActivityAttendance activityAttendance)
        {
            await _context.ActivityAttendances.AddAsync(activityAttendance);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ActivityAttendance?> GetAttendanceByActivityAndParticipantAsync(int activityId, Guid participantId)
        {
            return await Task.FromResult(_context.ActivityAttendances
                .FirstOrDefault(a => a.ActivityId == activityId && a.ParticipantId == participantId));
        }
    }
}
