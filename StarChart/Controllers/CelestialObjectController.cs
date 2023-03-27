using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
    }
}
