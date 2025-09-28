using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.Entities
{
    public class CheckedItemUser
    {   
        public int CheckedItemId { get; set; }
        public Guid UserId { get; set; }
        public bool IsChecked { get; set; } = false;
        public DateTime CheckedAt { get; set; }
    }
}
