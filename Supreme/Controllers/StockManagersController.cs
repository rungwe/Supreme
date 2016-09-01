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
    public class StockManagersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/StockManagers
        public IQueryable<StockManager> GetStockManagers()
        {
            return db.StockManagers;
        }

        // GET: api/StockManagers/5
        [ResponseType(typeof(StockManager))]
        public async Task<IHttpActionResult> GetStockManager(int id)
        {
            StockManager stockManager = await db.StockManagers.FindAsync(id);
            if (stockManager == null)
            {
                return NotFound();
            }

            return Ok(stockManager);
        }

        // PUT: api/StockManagers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStockManager(int id, StockManager stockManager)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stockManager.id)
            {
                return BadRequest();
            }

            db.Entry(stockManager).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockManagerExists(id))
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

        // POST: api/StockManagers
        [ResponseType(typeof(StockManager))]
        public async Task<IHttpActionResult> PostStockManager(StockManager stockManager)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.StockManagers.Add(stockManager);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = stockManager.id }, stockManager);
        }

        // DELETE: api/StockManagers/5
        [ResponseType(typeof(StockManager))]
        public async Task<IHttpActionResult> DeleteStockManager(int id)
        {
            StockManager stockManager = await db.StockManagers.FindAsync(id);
            if (stockManager == null)
            {
                return NotFound();
            }

            db.StockManagers.Remove(stockManager);
            await db.SaveChangesAsync();

            return Ok(stockManager);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StockManagerExists(int id)
        {
            return db.StockManagers.Count(e => e.id == id) > 0;
        }
    }
}