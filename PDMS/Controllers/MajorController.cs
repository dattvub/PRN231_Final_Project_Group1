using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Shared.DTO;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.Major;

namespace PDMS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        private readonly IPdmsDbContext _context;
        private readonly IMapper _mapper;
        public MajorController(IPdmsDbContext context, IMapper mapper)
        {
            this._context = context;
            _mapper = mapper;
        }
        [EnableCors("allowAll")]
        [HttpGet("ListMajor")]
        public async Task<ActionResult<PaginationDto<MajorDTO>>> GetMajors([FromQuery] GetMajorsDto getMajorsDto)
        {
            var query = _context.Majors.Where(x => x.Status);

            if (getMajorsDto.Query != null)
            {
                if (getMajorsDto.QueryByName)
                {
                    query = query.Where(x => x.MajorName.Contains(getMajorsDto.Query));
                }
                else
                {
                    query = query.Where(x => x.MajorCode.Contains(getMajorsDto.Query));
                }
            }

            var total = await query.CountAsync();

            var majors = await query
                .OrderByDescending(x => x.MajorId)
                .Skip((getMajorsDto.Page - 1) * getMajorsDto.Quantity)
                .Take(getMajorsDto.Quantity)
                .Select(x => _mapper.Map<MajorDTO>(x))
                .ToListAsync();

            return new PaginationDto<MajorDTO>(majors, total)
            {
                Page = getMajorsDto.Page,
                ItemsPerPage = getMajorsDto.Quantity,
                Query = getMajorsDto.Query
            };
        }
        [EnableCors("allowAll")]
        [HttpPost("add")]
        public async Task<ActionResult<MajorDTO>> CreateMajor([FromBody] CreateMajorDTO createMajorDTO)
        {
            var major = _mapper.Map<Major>(createMajorDTO);
            await _context.Majors.AddAsync(major);

            try
            {
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(CreateMajor),
                    new { id = major.MajorId },
                    _mapper.Map<MajorDTO>(major)
                );
            }
            catch (DbUpdateException)
            {
                if (!await CodeExisted(major.MajorCode)) throw;

                return ValidationError.BadRequest400("Dupplicate major code");
            }
            catch (Exception e)
            {
                return ValidationError.InternalServerError500(e.Message);
            }
        }

        [EnableCors("allowAll")]
        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult<MajorDTO>> UpdateMajor(
       [FromRoute] int id,
       [FromBody] CreateMajorDTO createMajorDTO
   )
        {
            var major = await _context.Majors.FirstOrDefaultAsync(x => x.MajorId == id && x.Status);
            if (major == null)
            {
                return ValidationError.BadRequest400($"Brand with id {id} is not exist");
            }

            var newInfoMajor = _mapper.Map<Major>(createMajorDTO);
            major.MajorCode = newInfoMajor.MajorCode;
            major.MajorName = newInfoMajor.MajorName;
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return ValidationError.BadRequest400("Error occur while update major");
                }
            }
            catch (DbUpdateException)
            {
                if (!await CodeExisted(major.MajorCode)) throw;

                return ValidationError.BadRequest400("Dupplicate major code");
            }
            catch (Exception e)
            {
                return ValidationError.InternalServerError500(e.Message);
            }

            return Ok(_mapper.Map<MajorDTO>(major));
        }
        [EnableCors("allowAll")]
        [HttpDelete("{id:int:min(1)}")]
        public async Task<ActionResult<MajorDTO>> DeleteMajor([FromRoute] int id)
        {
            var major = await _context.Majors.FirstOrDefaultAsync(x => x.MajorId == id && x.Status);
            if (major == null)
            {
                return ValidationError.BadRequest400($"Major with id {id} is not exist");
            }

            major.Status = false;
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return ValidationError.InternalServerError500("Error occur while delete major");
            }

            return Ok(_mapper.Map<MajorDTO>(major));
        }
        private async Task<bool> CodeExisted(string code)
        {
            return await _context.Majors.AnyAsync(x => x.MajorCode == code);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<MajorDTO>> GetBrand([FromRoute] int id)
        {
            var mrj = await _context.Majors.FirstOrDefaultAsync(x => x.MajorId == id);
            if (mrj == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<MajorDTO>(mrj);
            return dto;
        }
    }
}
