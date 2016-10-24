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
    public class SalesLedgersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/SalesLedgers
        public IQueryable<SalesLedger> GetSalesLedgers()
        {
            return db.SalesLedgers;
        }

        // GET: api/SalesLedgers/5
        [ResponseType(typeof(SalesLedger))]
        public async Task<IHttpActionResult> GetSalesLedger(int id)
        {
            SalesLedger salesLedger = await db.SalesLedgers.FindAsync(id);
            if (salesLedger == null)
            {
                return NotFound();
            }

            return Ok(salesLedger);
        }

        // PUT: api/SalesLedgers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSalesLedger(int id, SalesLedger salesLedger)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesLedger.id)
            {
                return BadRequest();
            }

            db.Entry(salesLedger).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesLedgerExists(id))
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

        // POST: api/SalesLedgers
        [ResponseType(typeof(SalesLedger))]
        public async Task<IHttpActionResult> PostSalesLedger(SalesLedger salesLedger)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SalesLedgers.Add(salesLedger);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = salesLedger.id }, salesLedger);
        }

        // DELETE: api/SalesLedgers/5
        [ResponseType(typeof(SalesLedger))]
        public async Task<IHttpActionResult> DeleteSalesLedger(int id)
        {
            SalesLedger salesLedger = await db.SalesLedgers.FindAsync(id);
            if (salesLedger == null)
            {
                return NotFound();
            }

            db.SalesLedgers.Remove(salesLedger);
            await db.SaveChangesAsync();

            return Ok(salesLedger);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SalesLedgerExists(int id)
        {
            return db.SalesLedgers.Count(e => e.id == id) > 0;
        }
    }
}