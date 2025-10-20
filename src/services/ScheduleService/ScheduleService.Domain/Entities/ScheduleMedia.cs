using ScheduleService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.Entities
{
    public class ScheduleMedia
    {
        public int Id { get; set; }
        public MediaType MediaType { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime UploadedAt { get; set; }
        public MediaMethod UploadMethod { get; set; } 


        // Foreign key to Schedule (1-N)
        public Guid? ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }
        public int? ActivityId { get; set; }
        public ScheduleActivity? Activity { get; set; }
        public Guid ParticipantId { get; set; }
        public ScheduleParticipant ScheduleParticipant { get; set; }
    }
}
