using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PDMS.Domain.Entities;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.CustomerType;
using PDMS.Shared.DTO.OrderDetail;
using PDMS.Shared.DTO.Customer;
using PDMS.Shared.DTO.Supplier;
using PDMS.Shared.DTO.Major;
using PDMS.Shared.DTO.User;

namespace PDMS.Application.AutoMapper {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<User, UserDto>();
            
            CreateMap<CreateBrandDto, Brand>()
                .AfterMap(
                    ((src, dst, ctx) => {
                        dst.BrandId = 0;
                        dst.Status = true;
                    })
                );
            CreateMap<Brand, BrandDto>().ReverseMap();

            CreateMap<CreateCustomerTypeDto, CustomerType>()
                .AfterMap(
                ((src, dst, ctx) => {
                    dst.CustomerTypeId = 0;
                    dst.Status = true;
                    })
                );

            CreateMap<CustomerType, CustomerTypeDto>().ReverseMap();

            CreateMap<CreateOrderDetailDto, OrderDetail>()
                .BeforeMap(((src, dst, ctx) =>
                {

                }));
            CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();
            CreateMap<CreateSupplierDto, Supplier>()
                .AfterMap(
                    ((src, dst, ctx) => {
                        dst.SupplierId = 0;
                        dst.Status = true;
                    })
                );
            CreateMap<Supplier, SupplierDto>().ReverseMap();

            CreateMap<CreateCustomerDto, Customer>()
                .AfterMap(
                    ((src, dst, ctx) => {
                        dst.CustomerId = 0;
                        //dst.Status = true;
                    })
                );
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Major, MajorDTO>().ReverseMap();
            CreateMap<CreateMajorDTO, Major>()
               .AfterMap(
                   ((src, dst, ctx) => {
                       dst.MajorId = 0;
                       dst.Status = true;
                   })
               );
        }
    }
}