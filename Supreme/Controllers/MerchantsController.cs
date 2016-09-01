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
    public class MerchantsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Merchants
        public IQueryable<Merchant> GetMerchants()
        {
            return db.Merchants;
        }

        // GET: api/Merchants/5
        [ResponseType(typeof(Merchant))]
        public async Task<IHttpActionResult> GetMerchant(int id)
        {
            Merchant merchant = await db.Merchants.FindAsync(id);
            if (merchant == null)
            {
                return NotFound();
            }

            return Ok(merchant);
        }

        // PUT: api/Merchants/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMerchant(int id, Merchant merchant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != merchant.id)
            {
                return BadRequest();
            }

            db.Entry(merchant).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MerchantExists(id))
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

        // POST: api/Merchants
        [ResponseType(typeof(Merchant))]
        public async Task<IHttpActionResult> PostMerchant(Merchant merchant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Merchants.Add(merchant);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = merchant.id }, merchant);
        }

        // DELETE: api/Merchants/5
        [ResponseType(typeof(Merchant))]
        public async Task<IHttpActionResult> DeleteMerchant(int id)
        {
            Merchant merchant = await db.Merchants.FindAsync(id);
            if (merchant == null)
            {
                return NotFound();
            }

            db.Merchants.Remove(merchant);
            await db.SaveChangesAsync();

            return Ok(merchant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MerchantExists(int id)
        {
            return db.Merchants.Count(e => e.id == id) > 0;
        }
    }
}