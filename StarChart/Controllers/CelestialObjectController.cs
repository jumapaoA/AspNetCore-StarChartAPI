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
            if (result == null)
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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestial)
        {
            var result = _context.CelestialObjects.Find(id);
            if(result == null)
            {
                return NotFound();
            }

            result.Name = celestial.Name;
            result.OrbitalPeriod = celestial.OrbitalPeriod;
            result.OrbitedObjectId = celestial.OrbitedObjectId;

            _context.Update(result);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var result = _context.CelestialObjects.Find(id);
            if (result == null)
            {
                return NotFound();
            }
            result.Name = name;
            _context.Update(result);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id).ToList();
            if (!result.Any())
                return NotFound();

            _context.RemoveRange(result);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
