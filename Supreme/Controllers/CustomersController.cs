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
    /// <summary>
    /// Methods to deal with customer model
    /// </summary>
    public class CustomersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Customers
        /// <summary>
        /// Retrieves the list of all the customers
        /// </summary>
        /// <returns>200</returns>
        public IQueryable<CustomerDTO> GetCustomers()
        {

            var customers = from b in db.Customers
                            select new CustomerDTO
                            {
                                 id = b.id,
                                 registrationDate= b.registrationDate,
                                 tradingName = b.tradingName,
                                 reference= b.reference
                            };
            return customers;
        }

        // GET: api/Customers
        /// <summary>
        /// Retrieves the customer branches associated with the current logged in Sales Rep or merchant, Sales Rep and Merchants Authorized
        /// </summary>
        /// <returns>200</returns>
        [Route("api/GetMyCustomers")]
        [HttpGet]
        [Authorize(Roles ="sales,merchant")]
        public IQueryable<BranchDTO2> GetMyCustomers()
        {
            string user = User.Identity.GetUserId();

            Merchant merchant = db.Merchants.Where(b => b.user_id == user).SingleOrDefault();

            int salesId = db.SalesReps.Where(b=>b.userid==user).SingleOrDefault().id;
            if (merchant == null)
            {
                var branches = from b in db.Branches.Where(b => b.salesRepId == salesId)
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
                                   location = b.location,
                                   branchManager =b.branchManager,
                                   


                               };
                return branches;
            }
            else
            {
                var branches = from b in db.Branches.Where(b => b.merchantId == merchant.id)
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
                                   location = b.location,
                                   branchManager = b.branchManager,


                               };
                return branches;
            }
            
        }

        // GET: api/Customers/5
        /// <summary>
        /// Get customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(CustomerDTO))]
        public async Task<IHttpActionResult> GetCustomer(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            CustomerDTO cus = new CustomerDTO
            {
                id = customer.id,
                registrationDate = customer.registrationDate,
                tradingName = customer.tradingName,
                reference = customer.reference
            };

            return Ok(cus);
        }

        // PUT: api/Customers/5
        /// <summary>
        /// Edit Customer information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customer"></param>
        /// <returns>200</returns>
        [Authorize(Roles ="accountant")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.id)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        /// <summary>
        /// Create a customer, Administrators and accountants are authorized
        /// </summary>
        /// <param name="customerData"></param>
        /// <returns>200</returns>
        [Authorize(Roles = "accountant,administrator")]
        [ResponseType(typeof(CustomerCreateDTO))]
        public async Task<IHttpActionResult> PostCustomer(CustomerCreateDTO customerData)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Customer customer = new Customer
            {
                tradingName = customerData.tradingName,
                registrationDate = DateTime.Now,
                //reference = customerData.reference
            };

            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = customer.id }, customer);
        }

        // DELETE: api/Customers/5
        /// <summary>
        /// Business logic not yet discussed
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> DeleteCustomer(int id)
        {
            return StatusCode(HttpStatusCode.NotImplemented);
            /*
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            await db.SaveChangesAsync();

            return Ok(customer);*/
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.id == id) > 0;
        }
    }
}