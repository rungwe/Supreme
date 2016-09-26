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
    /// <summary>
    /// Contains all the methods that deals with branches
    /// </summary>
    public class BranchesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Branches
        /// <summary>
        /// Retrieves all the branches in the system
        /// </summary>
        /// <returns>200</returns>
        public IQueryable<BranchDTO2> GetBranches()
        {
            var branches = from b in db.Branches
                           select new BranchDTO2
                           {
                               id = b.id,
                               name = b.name,
                               address = b.address,
                               email = b.email,
                               regionId = b.regionId,
                               telephone = b.telephone,
                               tradingName = b.customer.tradingName,
                               customer = new CustomerDTO2 { id = b.customer.id, tradingName = b.customer.tradingName, registrationDate = b.customer.registrationDate },
                               branchManager=b.branchManager,
                               location = b.location,
                               monthlyBudget = b.monthlyBudget,
                               telephone2 = b.telephone2,
                               merchant = new MerchantDTO { id= b.merchantId, firstname=b.merchant.profile.firstname, lastname = b.merchant.profile.lastname, middlename= b.merchant.profile.middlename},
                               salesRep = new SalesRepDTO { id = b.salesRepId, firstname= b.salesRep.profile.firstname, middlename = b.salesRep.profile.middlename, lastname= b.salesRep.profile.lastname}
                               

                           };
            return branches;
        }

        // GET: api/Branches/5
        /// <summary>
        /// Gets the branch specified by the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(BranchDTO2))]
        public async Task<IHttpActionResult> GetBranch(int id)
        {
            Branch b = await db.Branches.FindAsync(id);
            if (b == null)
            {
                return NotFound();
            }
            BranchDTO2 branch =new BranchDTO2
            {
                id = b.id,
                name = b.name,
                address = b.address,
                email = b.email,
                regionId = b.regionId,
                telephone = b.telephone,
                tradingName = b.customer.tradingName,
                customer = new CustomerDTO2 { id = b.customer.id, tradingName = b.customer.tradingName, registrationDate = b.customer.registrationDate },
                branchManager = b.branchManager,
                location = b.location,
                monthlyBudget = b.monthlyBudget,
                telephone2 = b.telephone2,
                merchant = new MerchantDTO { id = b.merchantId, firstname = b.merchant.profile.firstname, lastname = b.merchant.profile.lastname, middlename = b.merchant.profile.middlename },
                salesRep = new SalesRepDTO { id = b.salesRepId, firstname = b.salesRep.profile.firstname, middlename = b.salesRep.profile.middlename, lastname = b.salesRep.profile.lastname }


            };
            return Ok(branch);
        }



        /// <summary>
        /// Gets the customer branch by using the customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<BranchDTO2>))]
        public async Task<IHttpActionResult> GetCustomerBranch(int customerId)
        {
            
            var branch =  from b in db.Branches.Where(b=>b.customerId==customerId) select new BranchDTO2
            {
                id = b.id,
                name = b.name,
                address = b.address,
                email = b.email,
                regionId = b.regionId,
                telephone = b.telephone,
                tradingName = b.customer.tradingName,
                customer = new CustomerDTO2 { id = b.customer.id, tradingName = b.customer.tradingName, registrationDate = b.customer.registrationDate },
                branchManager = b.branchManager,
                location = b.location,
                monthlyBudget = b.monthlyBudget,
                telephone2 = b.telephone2,
                merchant = new MerchantDTO { id = b.merchantId, firstname = b.merchant.profile.firstname, lastname = b.merchant.profile.lastname, middlename = b.merchant.profile.middlename },
                salesRep = new SalesRepDTO { id = b.salesRepId, firstname = b.salesRep.profile.firstname, middlename = b.salesRep.profile.middlename, lastname = b.salesRep.profile.lastname }


            };
            return Ok(branch);
        }


        // PUT: api/Branches/5
        /// <summary>
        /// Assign a sales rep to a branch
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="salesRepId"></param>
        /// <returns></returns>
        [Route("api/AssignSalesRep")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBranch(int branchId, int salesRepId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Branch branch = await db.Branches.FindAsync(salesRepId);
            SalesRep salesRep = await db.SalesReps.FindAsync(salesRepId);

            if (salesRep == null)
            {
                return BadRequest("Sales rep does not exist");
            }


            if (branch==null)
            {
                return BadRequest("branch does not exist");
            }

            branch.salesRepId = salesRepId;
            db.Entry(branch).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                  throw;
                
            }

            return StatusCode(HttpStatusCode.NoContent);
        }



        /// <summary>
        /// Assign a merchant to a branch
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        [Route("api/AssignMerchant")]
        [ResponseType(typeof(void))]
        [HttpPut]
        public async Task<IHttpActionResult> AssignMerchant(int branchId, int merchantId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Branch branch = await db.Branches.FindAsync(merchantId);
            Merchant merchant = await db.Merchants.FindAsync(merchantId);

            if (merchant == null)
            {
                return BadRequest("Merchant does not exist");
            }


            if (branch == null)
            {
                return BadRequest("branch does not exist");
            }

            branch.merchantId = merchantId;
            db.Entry(branch).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;

            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        // POST: api/Branches
        /// <summary>
        /// Create a branch for a particular company, Accountants authorized
        /// </summary>
        /// <param name="branchData"></param>
        /// <returns></returns>
        [ResponseType(typeof(Branch))]
        public async Task<IHttpActionResult> PostBranch(BranchCreateDTO branchData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Branch branch = new Branch
            {
                name = branchData.name,
                customerId = branchData.customerId,
                regionId =  branchData.regionId,
                email = branchData.email,
                address = branchData.address,
                telephone = branchData.telephone,
                salesRepId = branchData.salesRepId,
                merchantId = branchData.merchantId,
                branchManager = branchData.branchManager,
                monthlyBudget = branchData.monthlyBudget,
                telephone2 = branchData.telephone2,
                location = branchData.location
            };

            db.Branches.Add(branch);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = branch.id }, branch);
        }

        // DELETE: api/Branches/5
        /// <summary>
        /// Deletes a branch, not yet tested
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Branch))]
        public async Task<IHttpActionResult> DeleteBranch(int id)
        {
            Branch branch = await db.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            db.Branches.Remove(branch);
            await db.SaveChangesAsync();

            return Ok(branch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BranchExists(int id)
        {
            return db.Branches.Count(e => e.id == id) > 0;
        }
    }
}