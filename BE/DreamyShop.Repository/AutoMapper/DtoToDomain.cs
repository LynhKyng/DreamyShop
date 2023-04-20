﻿using AutoMapper;
using DreamyShop.Domain;
using DreamyShop.Domain.Shared.Dtos;
using Microsoft.Data.SqlClient;

namespace DreamyShop.Repository.AutoMapper
{
    public class DtoToDomain : Profile
    {
        public DtoToDomain()
        {
            CreateMap<UserDto, User>()
                .ForMember(u => u.Password, option => option.Ignore())
                .ForMember(u => u.StoredSalt, option => option.Ignore())
                .ForMember(u => u.IdentityID, option => option.Ignore())
                .ForMember(u => u.Occupation, option => option.Ignore())
                .ForMember(u => u.Country, option => option.Ignore())
                .ForPath(u => u.Roles, act => act.MapFrom(src => src.RoleTypes));
            CreateMap<UserUpdateDto, User>()
                .ForMember(u => u.Id, option => option.DoNotUseDestinationValue())
                .ForMember(u => u.Password, option => option.DoNotUseDestinationValue())
                .ForMember(u => u.StoredSalt, option => option.DoNotUseDestinationValue())
                .ForMember(u => u.Roles, option => option.DoNotUseDestinationValue());

            CreateMap<ProductCreateUpdateDto, Product>()
                .ForMember(u => u.Id, option => option.DoNotUseDestinationValue())
                .ForMember(u => u.ProductReviews, option => option.DoNotUseDestinationValue())
                .ForMember(u => u.ProductTags, option => option.DoNotUseDestinationValue())
                .ForPath(u => u.Manufacturer.Name, option => option.MapFrom(p => p.ManufacturerName))
                .ForPath(u => u.ProductCategory.Name, option => option.MapFrom(p => p.CategoryName));

            //CreateMap<ProductDto, Product>()
            //    .ForMember(u => u.Manufacturer, option => option.Ignore())
            //    .ForMember(u => u.ProductCategory, option => option.Ignore())
            //    .ForMember(u => u.ProductReviews, option => option.Ignore())
            //    .ForMember(u => u.ProductTags, option => option.Ignore());
            CreateMap<ProductVariantDto, ProductVariant>();

            CreateMap<ProductAttributeDto, Domain.Attribute>();

            CreateMap<ManufacturerDto, Manufacturer>()
                .ForMember(u => u.Products, option => option.DoNotUseDestinationValue());

            CreateMap<ManufacturerCreateUpdateDto, Manufacturer>();

            CreateMap<CategoryDto, ProductCategory>()
               .ForMember(u => u.Products, option => option.DoNotUseDestinationValue());

            CreateMap<CategoryCreateUpdateDto, ProductCategory>();
        }
    }
}