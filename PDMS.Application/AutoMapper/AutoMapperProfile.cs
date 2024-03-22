﻿using System;
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
using PDMS.Shared.DTO.Employee;
using PDMS.Shared.DTO.Group;
using PDMS.Shared.DTO.Supplier;
using PDMS.Shared.DTO.Major;
using PDMS.Shared.DTO.Product;
using PDMS.Shared.DTO.User;
using System.Runtime.InteropServices;
using PDMS.Shared.DTO.CustomerGroup;

namespace PDMS.Application.AutoMapper {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<Group, GroupDto>().ReverseMap();
            
            CreateMap<User, UserDto>();
            
            CreateMap<User, EmployeeDto>();
            
            CreateMap<Group, EmployeeDto>();

            CreateMap<Employee, EmployeeDto>()
                .IncludeMembers(x => x.User, x => x.Group);

            CreateMap<User, CustomerDto>();

            CreateMap<Customer, CustomerDto>()
                .IncludeMembers(x => x.User);

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

            CreateMap<CreateProductDto, Product>()
                .AfterMap(
                ((src, dst, ctx) =>
                {
                    dst.Quantity = 0;
                    dst.CreatedTime = DateTime.Now;
                    dst.LastModifiedTime = DateTime.Now;
                    dst.Status = true;
                    dst.ProductCode = dst.ProductName + "-" + new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5)
        .Select(s => s[new Random().Next(s.Length)]).ToArray());
                }));

            CreateMap<UpdateProductDto, Product>()
                .AfterMap(
                ((src, dst, ctx) =>
                {
                    dst.LastModifiedTime = DateTime.Now;
                }));

            CreateMap<Product, Shared.DTO.Product.ProductDto>().ReverseMap();

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

            CreateMap<CreateCustomerGroupsDto, CustomerGroup>()
                .AfterMap(
                    ((src, dst, ctx) => {
                        dst.CustomerGroupId = 0;
                        dst.Status = true;
                    })
                );
            CreateMap<CustomerGroup, CustomerGroupDto>().ReverseMap();

            CreateMap<CreateCustomerDto, Customer>()
                .AfterMap(
                    ((src, dst, ctx) => {
                        dst.CustomerId = 0;
                        //dst.Status = true;
                    })
                );
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