using PDMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Infrastructure.Persistence;
using AutoMapper;
using PDMS.Shared.DTO;

namespace PDMS.Controllers
{
    //test
    [Route("api/[controller]")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        private readonly PdmsDbContext _context;
        private readonly IMapper _mapper;
        public MajorController(PdmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("Major")]
        public async Task<IActionResult> GetallMajor()
        {
            try
            {               
                var major = await _context.Majors.ToListAsync();
                return Ok(major);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("add")]
        public async Task<IActionResult> Addmajor(MajorRequest major)
        {
            var map = _mapper.Map<PDMS.Domain.Entities.Major>(major);
            var res = await _context.Majors.AddAsync(map);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("update")]
        public async Task<IActionResult> Updatemajo(MajorRequest major)
        {
            var res = await _context.Majors.FirstOrDefaultAsync(i => i.MajorId == major.MajorId);
            var map = _mapper.Map(major, res);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
