using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Supreme.Models;

namespace Supreme.Controllers
{
    public class VatController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Vat
        public IQueryable<Vat> GetVats()
        {
            return db.Vats;
        }

        // GET: api/Vat/5
        [ResponseType(typeof(Vat))]
        public async Task<IHttpActionResult> GetVat(int id)
        {
            Vat vat = await db.Vats.FindAsync(id);
            if (vat == null)
            {
                return NotFound();
            }

            return Ok(vat);
        }

        // PUT: api/Vat/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutVat(int id, Vat vat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vat.id)
            {
                return BadRequest();
            }

            db.Entry(vat).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Vat
        [ResponseType(typeof(Vat))]
        public async Task<IHttpActionResult> PostVat(Vat vat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Vats.Add(vat);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = vat.id }, vat);
        }

        // DELETE: api/Vat/5
        [ResponseType(typeof(Vat))]
        public async Task<IHttpActionResult> DeleteVat(int id)
        {
            Vat vat = await db.Vats.FindAsync(id);
            if (vat == null)
            {
                return NotFound();
            }

            db.Vats.Remove(vat);
            await db.SaveChangesAsync();

            return Ok(vat);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VatExists(int id)
        {
            return db.Vats.Count(e => e.id == id) > 0;
        }
    }
}