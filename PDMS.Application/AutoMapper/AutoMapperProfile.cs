using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PDMS.Domain.Entities;
using PDMS.Shared.DTO;


namespace PDMS.Application.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Domain.Entities.Major, MajorDTO>().ReverseMap();
            CreateMap<Domain.Entities.Major, MajorRequest>().ReverseMap();
        }
    }
}
