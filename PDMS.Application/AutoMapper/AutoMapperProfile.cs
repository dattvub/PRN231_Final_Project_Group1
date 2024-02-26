using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PDMS.Domain.Entities;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.Major;

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