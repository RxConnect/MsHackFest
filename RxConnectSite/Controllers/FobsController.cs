using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RxConnectSite.Data;
using RxConnectSite.Models;

namespace RxConnectSite.Controllers
{
    [Produces("application/json")]
    [Route("api/Fobs")]
    public class FobsController : Controller
    {
        private readonly RxConnectSiteContext _context;

        public FobsController(RxConnectSiteContext context)
        {
            _context = context;
        }

        // GET: api/Fobs
        [HttpGet]
        public IEnumerable<string> GetFobs()
        {
            return new string[] { _context.Fobs.First().Command.ToString() };
        }

        // GET: api/Fobs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFobs([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fobs = await _context.Fobs.SingleOrDefaultAsync(m => m.FobId == id);

            if (fobs == null)
            {
                return NotFound();
            }

            return Ok(fobs);
        }

        // PUT: api/Fobs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFobs([FromRoute] Guid id, [FromBody] Fobs fobs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fobs.FobId)
            {
                return BadRequest();
            }

            _context.Entry(fobs).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FobsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Fobs
        [HttpPost]
        public async Task<IActionResult> PostFobs([FromBody] Fobs fobs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Fobs.Add(fobs);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFobs", new { id = fobs.FobId }, fobs);
        }

        // DELETE: api/Fobs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFobs([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fobs = await _context.Fobs.SingleOrDefaultAsync(m => m.FobId == id);
            if (fobs == null)
            {
                return NotFound();
            }

            _context.Fobs.Remove(fobs);
            await _context.SaveChangesAsync();

            return Ok(fobs);
        }

        private bool FobsExists(Guid id)
        {
            return _context.Fobs.Any(e => e.FobId == id);
        }
    }
}