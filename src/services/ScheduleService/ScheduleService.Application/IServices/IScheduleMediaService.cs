using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.DTOs.Requests;
using ScheduleService.Domain.Entities;

namespace ScheduleService.Application.IServices
{
    public interface IScheduleMediaService
    {
        Task<ScheduleMedia> UploadAsync(UploadScheduleMediaRequest request);
    }
}
