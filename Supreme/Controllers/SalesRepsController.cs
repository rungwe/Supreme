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
    public class SalesRepsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/SalesReps
        /// <summary>
        /// For debugging purposes, it returns a collection of all Sales Rep
        /// </summary>
        /// <returns></returns>
        public IQueryable<SalesRepDTO> GetSalesReps()
        {
            var sales = from b in db.SalesReps
                        select
                     new SalesRepDTO
                     {
                         firstname = b.profile.firstname,
                         lastname = b.profile.lastname,
                         middlename = b.profile.middlename,

                     };
             return sales;
        }

        // GET: api/SalesReps/5
        /// <summary>
        /// Get sales rep info using their id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(SalesRep))]
        public async Task<IHttpActionResult> GetSalesRep(int id)
        {
            SalesRep salesRep = await db.SalesReps.FindAsync(id);
            if (salesRep == null)
            {
                return NotFound();
            }

            return Ok(salesRep);
        }

        // PUT: api/SalesReps/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSalesRep(int id, SalesRep salesRep)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesRep.id)
            {
                return BadRequest();
            }

            db.Entry(salesRep).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesRepExists(id))
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

        // POST: api/SalesReps
        [ResponseType(typeof(SalesRep))]
        public async Task<IHttpActionResult> PostSalesRep(SalesRep salesRep)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SalesReps.Add(salesRep);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = salesRep.id }, salesRep);
        }

        // DELETE: api/SalesReps/5
        [ResponseType(typeof(SalesRep))]
        public async Task<IHttpActionResult> DeleteSalesRep(int id)
        {
            SalesRep salesRep = await db.SalesReps.FindAsync(id);
            if (salesRep == null)
            {
                return NotFound();
            }

            db.SalesReps.Remove(salesRep);
            await db.SaveChangesAsync();

            return Ok(salesRep);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SalesRepExists(int id)
        {
            return db.SalesReps.Count(e => e.id == id) > 0;
        }
    }
}