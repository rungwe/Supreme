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
    public class BranchProductPricesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/BranchProductPrices
        public IQueryable<BranchProductPrice> GetBranchProductPrices()
        {
            return db.BranchProductPrices;
        }

        // GET: api/BranchProductPrices/5
        [ResponseType(typeof(BranchProductPrice))]
        public async Task<IHttpActionResult> GetBranchProductPrice(int id)
        {
            BranchProductPrice branchProductPrice = await db.BranchProductPrices.FindAsync(id);
            if (branchProductPrice == null)
            {
                return NotFound();
            }

            return Ok(branchProductPrice);
        }

        // PUT: api/BranchProductPrices/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBranchProductPrice(int id, BranchProductPrice branchProductPrice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != branchProductPrice.id)
            {
                return BadRequest();
            }

            db.Entry(branchProductPrice).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchProductPriceExists(id))
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

        // POST: api/BranchProductPrices
        [ResponseType(typeof(BranchProductPrice))]
        public async Task<IHttpActionResult> PostBranchProductPrice(BranchProductPrice branchProductPrice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BranchProductPrices.Add(branchProductPrice);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = branchProductPrice.id }, branchProductPrice);
        }

        // DELETE: api/BranchProductPrices/5
        [ResponseType(typeof(BranchProductPrice))]
        public async Task<IHttpActionResult> DeleteBranchProductPrice(int id)
        {
            BranchProductPrice branchProductPrice = await db.BranchProductPrices.FindAsync(id);
            if (branchProductPrice == null)
            {
                return NotFound();
            }

            db.BranchProductPrices.Remove(branchProductPrice);
            await db.SaveChangesAsync();

            return Ok(branchProductPrice);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BranchProductPriceExists(int id)
        {
            return db.BranchProductPrices.Count(e => e.id == id) > 0;
        }
    }
}