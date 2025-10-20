using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.IServices
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName);
    }
}
