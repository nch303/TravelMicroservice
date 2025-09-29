using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.Entities
{
    public class CheckedItemParticipant
    {  
        public bool IsChecked { get; set; } = false;
        public DateTime CheckedAt { get; set; }

        // Foreign key to CheckedItem (N-1)
        public int CheckedItemId { get; set; }
        public CheckedItem CheckedItem { get; set; }

        // Foreign key to ScheduleParticipant (N-1)
        public Guid ScheduleParticipantId { get; set; }
        public ScheduleParticipant ScheduleParticipant { get; set; }
    }
}
