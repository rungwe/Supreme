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

        // GET: api/Invoices
        public IQueryable<Invoice> GetInvoices()
        {
            return db.Invoices;
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
            int inv = await db.Invoices.CountAsync(b => b.invoiceNumber== invoiceData.invoiceNumber);
            if (inv != 0)
            {
                return BadRequest("Invoice number exists");
            }
            if (accountant == null)
            {
                
                return BadRequest("Accountant does not exist");
            }

            
            Invoice invoice = new Invoice
            {
                accountant_id = accountant.id,
                date = DateTime.Now,
                invoiceNumber= invoiceData.invoiceNumber,
                orderid = invoiceData.orderid,
                status = "not_paid",
               
            };
            order.status = "processing";
            db.Entry(order).State = EntityState.Modified;
            db.Invoices.Add(invoice);
            await db.SaveChangesAsync();

            return Ok();
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