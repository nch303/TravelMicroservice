using AdvertisementService.Application.DTOs.Requests;
using AdvertisementService.Application.DTOs.Responses;
using AdvertisementService.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreatePackageRequest, AdvertisementPackage>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
               .ForMember(dest => dest.DurationInDays, opt => opt.MapFrom(src => src.DurationInDays))
               .ForMember(dest => dest.MaxPostCount, opt => opt.MapFrom(src => src.MaxPostCount));

            CreateMap<AdvertisementPackage, PackageResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DurationInDays, opt => opt.MapFrom(src => src.DurationInDays))
                .ForMember(dest => dest.MaxPostCount, opt => opt.MapFrom(src => src.MaxPostCount))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<UpdatePackageRequest, AdvertisementPackage>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DurationInDays, opt => opt.MapFrom(src => src.DurationInDays))
                .ForMember(dest => dest.MaxPostCount, opt => opt.MapFrom(src => src.MaxPostCount));

            CreateMap<PartnerPackagePurchase, PurchaseResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PartnerId, opt => opt.MapFrom(src => src.PartnerId))
                .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.RemainingPostCount, opt => opt.MapFrom(src => src.RemainingPostCount))
                .ForMember(dest => dest.PaymentTransactionId, opt => opt.MapFrom(src => src.PaymentTransactionId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package.Name));

            CreateMap<AdvertisementPost, AdvertisementPostResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PartnerId, opt => opt.MapFrom(src => src.PartnerId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.PostedAt, opt => opt.MapFrom(src => src.PostedAt))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.ApprovedAt, opt => opt.MapFrom(src => src.ApprovedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.PackagePurchaseId, opt => opt.MapFrom(src => src.PackagePurchaseId))
                .ForMember(dest => dest.MediaIds, opt => opt.MapFrom(src => src.MediaItems.Select(m => m.Id).ToList()))
                .ForMember(dest => dest.MediaUrls, opt => opt.MapFrom(src => src.MediaItems.Select(m => m.MediaUrl).ToList()));
        }
    }
}
