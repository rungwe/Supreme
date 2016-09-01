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
    [Authorize]
    public class AccountantsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Accountants
        public IQueryable<Accountant> GetAccountants()
        {
            return db.Accountants;
        }

        // GET: api/Accountants/5
        [ResponseType(typeof(Accountant))]
        public async Task<IHttpActionResult> GetAccountant(int id)
        {
            Accountant accountant = await db.Accountants.FindAsync(id);
            if (accountant == null)
            {
                return NotFound();
            }

            return Ok(accountant);
        }

        // PUT: api/Accountants/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAccountant(int id, Accountant accountant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != accountant.id)
            {
                return BadRequest();
            }

            db.Entry(accountant).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountantExists(id))
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

        // POST: api/Accountants
        [ResponseType(typeof(Accountant))]
        public async Task<IHttpActionResult> PostAccountant(Accountant accountant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Accountants.Add(accountant);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = accountant.id }, accountant);
        }

        // DELETE: api/Accountants/5
        [ResponseType(typeof(Accountant))]
        public async Task<IHttpActionResult> DeleteAccountant(int id)
        {
            Accountant accountant = await db.Accountants.FindAsync(id);
            if (accountant == null)
            {
                return NotFound();
            }

            db.Accountants.Remove(accountant);
            await db.SaveChangesAsync();

            return Ok(accountant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountantExists(int id)
        {
            return db.Accountants.Count(e => e.id == id) > 0;
        }
    }
}