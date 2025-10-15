using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Responses
{
    public class CheckedItemParticipantResponse
    {
        public int CheckedItemId { get; set; }
        public string CheckedItemName { get; set; }
        public bool IsChecked { get; set; } = false;
        public DateTime CheckedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid ScheduleParticipantId { get; set; }
    }
}
