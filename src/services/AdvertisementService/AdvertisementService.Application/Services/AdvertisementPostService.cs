using AdvertisementService.Application.DTOs.Requests;
using AdvertisementService.Application.IServices;
using AdvertisementService.Domain.Entities;
using AdvertisementService.Domain.Enums;
using AdvertisementService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.Services
{
    public class AdvertisementPostService : IAdvertisementPostService
    {
        private readonly IAdvertisementPostRepository _postRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IPartnerPackagePurchaseRepository _purchaseRepository;

        public AdvertisementPostService(IAdvertisementPostRepository postRepository, ICloudinaryService cloudinaryService
            , IPartnerPackagePurchaseRepository purchaseRepository)
        {
            _postRepository = postRepository;
            _cloudinaryService = cloudinaryService;
            _purchaseRepository = purchaseRepository;
        }

        public async Task<AdvertisementPost> CreatePostAsync(Guid partnerId, CreateAdvertisementPostRequest request)
        {
            if (request.MediaFiles == null || !request.MediaFiles.Any())
                throw new Exception("Vui lòng chọn ít nhất một hình ảnh hoặc video.");

            if (request.MediaTypes == null || !request.MediaTypes.Any())
                throw new Exception("Vui lòng chọn loại media cho từng tệp.");

            if (request.MediaFiles.Count != request.MediaTypes.Count)
                throw new Exception("Số lượng file và loại media không khớp.");

            // Kiểm tra giao dịch mua gói quảng cáo
            var purchase = await _purchaseRepository.GetByIdAsync(request.PackagePurchaseId);
            if (purchase == null || purchase.PartnerId != partnerId)
                throw new Exception("Giao dịch mua gói quảng cáo không tồn tại hoặc không hợp lệ.");

            // Kiểm tra trạng thái và số lượng bài đăng còn lại
            if (purchase.Status != PartnerPackagePurchaseStatus.Active || purchase.RemainingPostCount <= 0 || purchase.EndDate < DateTime.UtcNow)
                throw new Exception("Bạn đã hết lượt đăng hoặc gói đã hết hạn sử dụng.");

            var mediaList = new List<AdvertisementMedia>();

            for (int i = 0; i < request.MediaFiles.Count; i++)
            {
                var file = request.MediaFiles[i];
                var mediaType = request.MediaTypes[i];

                if (file == null || file.Length == 0)
                    continue;

                await using var stream = file.OpenReadStream();

                string fileUrl = mediaType switch
                {
                    0 => await _cloudinaryService.UploadImageAsync(stream, file.FileName),
                    1 => await _cloudinaryService.UploadVideoAsync(stream, file.FileName),
                    _ => throw new Exception("Loại media không được hỗ trợ.")
                };

                mediaList.Add(new AdvertisementMedia
                {
                    Id = Guid.NewGuid(),
                    MediaUrl = fileUrl,
                    MediaType = (MediaType)mediaType,
                    UploadedAt = DateTime.UtcNow
                });
            }

            if (!mediaList.Any())
                throw new Exception("Không thể tạo bài đăng vì không có tệp hợp lệ được tải lên.");

            var post = new AdvertisementPost
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Title = request.Title,
                Description = request.Description,
                PackagePurchaseId = request.PackagePurchaseId,
                PostedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Status = AdvertisementStatus.Pending,
                MediaItems = mediaList
            };

            purchase.RemainingPostCount -= 1;
            await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();
            await _purchaseRepository.SaveChangesAsync();

            return post;
        }



        public async Task<IEnumerable<AdvertisementPost>> GetPostsByPartnerAsync(Guid partnerId)
        {
            return await _postRepository.GetByPartnerIdAsync(partnerId);
        }

        public async Task<AdvertisementPost?> GetPostByIdAsync(Guid id)
        {
            return await _postRepository.GetByIdAsync(id);
        }

        public async Task<AdvertisementPost> UpdatePostAsync(Guid postId, UpdateAdvertisementPostRequest request)
        {
            // Lấy post từ database
            var existingPost = await _postRepository.GetByIdAsync(postId);

            if (existingPost == null)
                throw new Exception("Không tìm thấy bài đăng.");

            // Cập nhật thông tin cơ bản
            existingPost.Title = request.Title ?? existingPost.Title;
            existingPost.Description = request.Description ?? existingPost.Description;
            existingPost.PackagePurchaseId = request.PackagePurchaseId ?? existingPost.PackagePurchaseId;

            // === Cập nhật danh sách media ===
            var updatedMediaList = new List<AdvertisementMedia>();

            // Giữ lại media cũ nếu người dùng chọn giữ lại
            if (request.KeepMediaIds != null && request.KeepMediaIds.Any())
            {
                var keptMedias = existingPost.MediaItems
                    .Where(m => request.KeepMediaIds.Contains(m.Id))
                    .ToList();

                updatedMediaList.AddRange(keptMedias);
            }

            // Thêm media mới nếu có
            if (request.NewMediaFiles != null && request.NewMediaFiles.Any())
            {
                if (request.NewMediaTypes == null || request.NewMediaTypes.Count != request.NewMediaFiles.Count)
                    throw new Exception("Số lượng file và loại media không khớp.");

                for (int i = 0; i < request.NewMediaFiles.Count; i++)
                {
                    var file = request.NewMediaFiles[i];
                    var mediaType = request.NewMediaTypes[i];

                    if (file == null || file.Length == 0)
                        continue;

                    await using var stream = file.OpenReadStream();

                    string fileUrl = mediaType switch
                    {
                        0 => await _cloudinaryService.UploadImageAsync(stream, file.FileName),
                        1 => await _cloudinaryService.UploadVideoAsync(stream, file.FileName),
                        _ => throw new Exception("Loại media không được hỗ trợ.")
                    };

                    updatedMediaList.Add(new AdvertisementMedia
                    {
                        Id = Guid.NewGuid(),
                        MediaUrl = fileUrl,
                        MediaType = (MediaType)mediaType,
                        UploadedAt = DateTime.UtcNow
                    });
                }
            }

            // Cập nhật lại danh sách media
            existingPost.MediaItems = updatedMediaList;

            await _postRepository.SaveChangesAsync();

            return existingPost;
        }
    }
}
