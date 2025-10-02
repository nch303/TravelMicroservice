using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;

namespace ScheduleService.Application.IServices
{
    public interface ICheckItemParticipantService
    {
        Task ToggleCheckAsync(int checkedItemId, bool isChecked);
    }
}
