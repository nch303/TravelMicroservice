using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.Entities
{
    public class CheckedItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign key to Schedule (1-N)
        public Guid ScheduleId { get; set; }
        public Schedule Schedule { get; set; }

        // Quan hệ N-N thông qua CheckedItemParticipant
        public ICollection<CheckedItemParticipant> CheckedItemParticipants { get; set; }
    }
}
