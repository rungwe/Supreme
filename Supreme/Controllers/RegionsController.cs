﻿using System;
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
    public class RegionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Regions
        public IQueryable<Region> GetRegions()
        {
            return db.Regions;
        }

        // GET: api/Regions/5
        [ResponseType(typeof(Region))]
        public async Task<IHttpActionResult> GetRegion(int id)
        {
            Region region = await db.Regions.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            return Ok(region);
        }

        // PUT: api/Regions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRegion(int id, Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != region.id)
            {
                return BadRequest();
            }

            db.Entry(region).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegionExists(id))
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

        // POST: api/Regions
        [ResponseType(typeof(Region))]
        public async Task<IHttpActionResult> PostRegion(RegionDTO regionData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int count = await db.Regions.CountAsync(b => b.name == regionData.name);

            if (count != 0)
            {
                return BadRequest("Region Exist already");
            }
            Region region = new Region
            {
                name = regionData.name
            };

            db.Regions.Add(region);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = region.id }, region);
        }

        // DELETE: api/Regions/5
        [ResponseType(typeof(Region))]
        public async Task<IHttpActionResult> DeleteRegion(int id)
        {
            Region region = await db.Regions.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            db.Regions.Remove(region);
            await db.SaveChangesAsync();

            return Ok(region);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RegionExists(int id)
        {
            return db.Regions.Count(e => e.id == id) > 0;
        }
    }
}