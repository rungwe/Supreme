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
    public class ReturnsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Returns
        public IQueryable<Return> GetReturns()
        {
            return db.Returns;
        }

        // GET: api/Returns/5
        [ResponseType(typeof(Return))]
        public async Task<IHttpActionResult> GetReturn(int id)
        {
            Return @return = await db.Returns.FindAsync(id);
            if (@return == null)
            {
                return NotFound();
            }

            return Ok(@return);
        }

        // PUT: api/Returns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReturn(int id, Return @return)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != @return.id)
            {
                return BadRequest();
            }

            db.Entry(@return).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReturnExists(id))
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

        // POST: api/Returns
        [ResponseType(typeof(Return))]
        public async Task<IHttpActionResult> PostReturn(Return @return)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Returns.Add(@return);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = @return.id }, @return);
        }

        // DELETE: api/Returns/5
        [ResponseType(typeof(Return))]
        public async Task<IHttpActionResult> DeleteReturn(int id)
        {
            Return @return = await db.Returns.FindAsync(id);
            if (@return == null)
            {
                return NotFound();
            }

            db.Returns.Remove(@return);
            await db.SaveChangesAsync();

            return Ok(@return);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReturnExists(int id)
        {
            return db.Returns.Count(e => e.id == id) > 0;
        }
    }
}