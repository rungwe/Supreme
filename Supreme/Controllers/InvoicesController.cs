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
    /// Methods which deals with invoices
    /// </summary>
    public class InvoicesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static readonly object Lock = new object();
        // GET: api/Invoices
        /// <summary>
        /// Get all invoices, only accountants and administrators are authorised
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="administrator,accountant")]
        public IQueryable<InvoiceDTO> GetInvoices()
        {

            return from b in db.Invoices select 
                   
                   new InvoiceDTO {
                       accountant = new AccountantDTO { Id=b.accountant_id,firstname=b.account.profile.firstname, lastname= b.account.profile.lastname,middlename=b.account.profile.middlename},
                       id= b.id,
                       date = b.date,
                       invoiceNumber = b.invoiceNumber,
                       orderid = b.orderid,
                       order = new OrderDTO
                       {
                           id = b.orderid,
                           price = b.order.price,
                           date = b.order.date,
                           status = b.order.status,
                           salesRep = new SalesRepDTO() { id = b.order.salesRep.id, firstname = b.order.salesRep.profile.firstname, middlename = b.order.salesRep.profile.middlename, lastname = b.order.salesRep.profile.lastname },
                           branch = new BranchDTO() { id = b.order.branch.id, address = b.order.branch.address, name = b.order.branch.name, email = b.order.branch.email, regionId = b.order.branch.regionId, telephone = b.order.branch.telephone },
                           customer = new CustomerDTO { id = b.order.branch.customer.id, tradingName = b.order.branch.customer.tradingName, registrationDate = b.order.branch.customer.registrationDate },
                          
                       },
                       status= b.status
                   };
        }

        // GET: api/Invoices/5
        [ResponseType(typeof(Invoice))]
        public async Task<IHttpActionResult> GetInvoice(int id)
        {
            Invoice invoice = await db.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        

        /// <summary>
        /// Only accountants are authorized, this a method to update into the system when an order is paid
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>201</returns>
        
        [Authorize(Roles ="accountant")]
        [ResponseType(typeof(void))]
        [Route("UpdatePaidInvoice")]
        [HttpPut]
        public async Task<IHttpActionResult> PutInvoicePaid(int invoiceId)
        {
            Invoice invoice =await db.Invoices.FindAsync(invoiceId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (invoice==null)
            {
                return BadRequest();
            }
            invoice.status = "paid";
            db.Entry(invoice).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// This is used to undo the change of invoice paid status to unpaid in the event of an error 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        [Authorize(Roles = "administrator,accountant")]
        [ResponseType(typeof(void))]
        [Route("UpdateNotPaidInvoice")]
        [HttpPut]
        public async Task<IHttpActionResult> PutInvoiceNotPaid(int invoiceId)
        {
            Invoice invoice = await db.Invoices.FindAsync(invoiceId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (invoice == null)
            {
                return BadRequest();
            }
            invoice.status = "not_paid";
            db.Entry(invoice).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        // POST: api/Invoices
        /// <summary>
        /// Used to create an invoice for a particular order, only for accountants, the invoice is initialised to a not paid
        /// </summary>
        /// <param name="invoiceData"></param>
        /// <returns>200</returns>
        [Authorize(Roles ="accountant")]
        [ResponseType(typeof(void))]
        [Route("api/CreateInvoice")]
        public async Task<IHttpActionResult> PostInvoice(InvoiceCreateDTO invoiceData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string reg = User.Identity.GetUserId();
            Order order = await db.Orders.FindAsync(invoiceData.orderid);
            Accountant accountant = db.Accountants.Where(b => b.user_id == reg).SingleOrDefault();
            /*int inv = await db.Invoices.CountAsync(b => b.invoiceNumber== invoiceData.invoiceNumber || b.orderid==order.id);
            if (inv != 0)
            {
                return BadRequest("Invoice number exists or invoice for this order was already made");
            }**/
            if (accountant == null)
            {
                
                return BadRequest("Accountant does not exist");
            }

            TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");
            DateTime userTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeInfo);

            Invoice invoice = new Invoice
            {
                accountant_id = accountant.id,
                date = userTime,
               // invoiceNumber= invoiceData.invoiceNumber,
                orderid = invoiceData.orderid,
                status = "not_paid",
               
            };

            // critical section generate invoice number
            lock (Lock)
            {
                
                string year = DateTime.Now.Year.ToString();
                string reference = "INV-"+year+"-";
                int invo = db.Invoices.Count();
                if(invo == 0)
                {
                    invo += 1;
                    reference += String.Format("{0:00000}", invo);
                }
                else
                {
                    invo = db.Invoices.Count(b => b.invoiceNumber.Substring(4, 4) == year);
                    invo += 1;
                    reference += String.Format("{0:00000}", invo);
                }
                while (db.Invoices.Count(d => d.invoiceNumber == reference) != 0)
                {
                    reference = "INV-" + year + "-"+ String.Format("{0:00000}", ++invo); 
                }
                order.status = "processing";
                order.warehouseLocation = invoiceData.warehouseLocation;
                order.invoiceNumber = reference;
                db.Entry(order).State = EntityState.Modified;
                invoice.invoiceNumber = reference;
                db.Invoices.Add(invoice);
                db.SaveChanges();

            }

            

            return Ok(invoice.invoiceNumber);
        }

        // DELETE: api/Invoices/5
        [ResponseType(typeof(Invoice))]
        public async Task<IHttpActionResult> DeleteInvoice(int id)
        {
            Invoice invoice = await db.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            db.Invoices.Remove(invoice);
            await db.SaveChangesAsync();

            return Ok(invoice);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool InvoiceExists(int id)
        {
            return db.Invoices.Count(e => e.id == id) > 0;
        }
    }
}