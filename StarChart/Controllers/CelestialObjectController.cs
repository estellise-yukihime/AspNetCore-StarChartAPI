using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
            var celestial = _context.CelestialObjects.Find(id);

            if (celestial == null)
            {
                return NotFound();
            }

            var orbitedCelestials = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            celestial.Satellites = orbitedCelestials;

            return Ok(celestial);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestials = _context.CelestialObjects.Where(x => x.Name == name).ToList();

            if (celestials.Count == 0)
            {
                return NotFound();
            }

            foreach (var celestial in celestials)
            {
                var orbitedCelestials = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestial.Id).ToList();

                celestial.Satellites = orbitedCelestials;
            }
            
            return Ok(celestials);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestials = _context.CelestialObjects.ToList();

            foreach (var celestial in celestials)
            {
                var orbitedCelestials = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestial.Id).ToList();

                celestial.Satellites = orbitedCelestials;
            }
            
            return Ok(celestials);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingCelestial = _context.CelestialObjects.Find(id);

            if (existingCelestial == null)
            {
                return NotFound();
            }

            existingCelestial.Name = celestialObject.Name;
            existingCelestial.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingCelestial.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(existingCelestial);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestial = _context.CelestialObjects.Find(id);

            if (celestial == null)
            {
                return NotFound();
            }

            celestial.Name = name;

            _context.CelestialObjects.Update(celestial);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestial = _context.CelestialObjects.Find(id);
            var orbitedObjects = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            if (celestial == null && orbitedObjects.Count == 0)
            {
                return NotFound();
            }

            if (celestial != null)
            {
                orbitedObjects.Add(celestial);
            }
            
            _context.CelestialObjects.RemoveRange(orbitedObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
