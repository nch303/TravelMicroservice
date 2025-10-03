using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;
using ScheduleService.Infrastructure.Configurations;

namespace ScheduleService.Infrastructure.Repositories
{
    public class ScheduleMediaRepository : IScheduleMediaRepository
    {
        private readonly AppDbContext _context;
        public ScheduleMediaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleMedia> AddAsync(ScheduleMedia media)
        {
            await _context.ScheduleMedias.AddAsync(media);
            await _context.SaveChangesAsync();
            return media;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
