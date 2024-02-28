using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PDMS.Domain.Entities;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.Customer;
using PDMS.Shared.DTO.Supplier;

namespace PDMS.Application.AutoMapper {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<CreateBrandDto, Brand>()
                .AfterMap(
                    ((src, dst, ctx) => {
                        dst.BrandId = 0;
                        dst.Status = true;
                    })
                );
            CreateMap<Brand, BrandDto>().ReverseMap();

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
        }
    }
}