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
using Microsoft.AspNet.Identity;

namespace Supreme.Controllers
{
    
    [Authorize]
    public class ProfilesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Profiles
        /// <summary>
        /// Get all the users in the system
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "administrator")]
        public IQueryable<Profile> GetProfiles()
        {
            return db.Profiles;
        }



        // GET: api/Profiles/5
        /// <summary>
        /// get employees profile by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Profile))]
        public async Task<IHttpActionResult> GetProfile(int id)
        {
            Profile profile = await db.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        /// <summary>
        /// Get the profile of the current logged in user
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(Profile))]
        [Route("api/myProfile")]
        [HttpGet]
        public async Task<IHttpActionResult> myProfile()
        {
            string reg = User.Identity.GetUserId();
            Profile profile = await db.Profiles.Where(d => d.userid == reg).SingleOrDefaultAsync();
            if (profile == null)
            {
                return NotFound();
            }

            // role fix
            /*
            if(profile.position == "merchant")
            {
                profile.position = "sales";
            }

            else if (profile.position == "sales")
            {
                profile.position = "merchant";
            }**/

            return Ok(profile);
        }

        // PUT: api/Profiles/5
        /// <summary>
        /// Edit profile, not yet refined
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProfile(int id, Profile profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != profile.id)
            {
                return BadRequest();
            }

            db.Entry(profile).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
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

        // DELETE: api/Profiles/5
        /// <summary>
        /// Lazy deletion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Profile))]
        public async Task<IHttpActionResult> DeleteProfile(int id)
        {
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return NotFound();
            }

            profile.status = "suspended";
            db.Entry(profile).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(profile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileExists(int id)
        {
            return db.Profiles.Count(e => e.id == id) > 0;
        }
    }
}