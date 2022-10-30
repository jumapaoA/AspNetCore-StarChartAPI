using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var result = _context.CelestialObjects.Find(id);
            if(result == null)
                return NotFound();
            result.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == id).ToList();
            return Ok(result);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var result = _context.CelestialObjects.Where(obj => obj.Name == name);
            if (!result.Any())
                return NotFound();

            result.ForEachAsync(res => 
                res.Satellites = _context.CelestialObjects
                .Where(context => context.OrbitedObjectId == res.Id).ToList());

            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _context.CelestialObjects.ToList();
                result.ForEach(res => res.Satellites = _context.CelestialObjects
                .Where(context => context.OrbitedObjectId == res.Id).ToList());
            return Ok(result);
        }
    }
}
