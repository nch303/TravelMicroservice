using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.IServices
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName);
    }
}
