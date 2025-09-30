using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Enums;

namespace ScheduleService.Domain.Entities
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public string SharedCode { get; set; }
        public Guid OwnerId { get; set; }
        public string Title { get; set; }
        public string StartLocation { get; set; }
        public string Destination { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ParticipantsCount { get; set; }
        public string Notes { get; set; }
        public bool IsShared { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ScheduleStatus Status { get; set; }

        public Schedule()
        {
            SharedCode = GenerateRandomCode();
        }
        public string GenerateRandomCode()
        {
            // Lấy Guid, chuyển sang số và giữ 10 ký tự số
            string digits = new string(Guid.NewGuid().ToString("N")
                                        .Where(char.IsDigit)
                                        .ToArray());
            return digits.Substring(0, 10);
        }

        // Quan hệ 1-N với ScheduleParticipant
        public ICollection<ScheduleParticipant> ScheduleParticipants { get; set; }
        // Quan hệ 1-N với ScheduleActivity
        public ICollection<ScheduleActivity> ScheduleActivities { get; set; }
        // Quan hệ 1-N với CheckList
        public ICollection<CheckedItem> CheckLists { get; set; }
        // Quan hệ 1-N với ScheduleMedia
        public ICollection<ScheduleMedia> ScheduleMedias { get; set; }

    }


}
