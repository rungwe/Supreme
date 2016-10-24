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
using Supreme.Library;

namespace Supreme.Controllers
{
    /// <summary>
    /// Delivery Note methods
    /// </summary>
    public class DeliveryNotesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static readonly object Lock = new object();

        // GET: api/DeliveryNotes
        /// <summary>
        /// Get all delivery notes, for debugging purposes
        /// </summary>
        /// <returns>200</returns>
        public IQueryable<DeliveryNoteDTO> GetDeliveryNotes()
        {
            var deliveryNotes =db.DeliveryNotes;
            IQueryable<DeliveryNoteDTO> notes = from d in deliveryNotes
                        select
                                 new DeliveryNoteDTO
                                 {
                                     id = d.id,
                                     date = d.date,
                                     driverId = d.driverId,
                                     orderId = d.orderId,
                                     status = d.status
                                 };
            return notes;
            
        }
        
        // GET: api/DeliveryNotes/5
        /// <summary>
        /// Get a delivery not by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(DeliveryNoteDTO))]
        public async Task<IHttpActionResult> GetDeliveryNote(int id)
        {
            DeliveryNote d = await db.DeliveryNotes.FindAsync(id);
            if (d == null)
            {
                return NotFound();
            }
            DeliveryNoteDTO deliveryNote = new DeliveryNoteDTO
            {
                id = d.id,
                date = d.date,
                driverId = d.driverId,
                orderId = d.orderId,
                status = d.status
            };
            return Ok(deliveryNote);
        }

        /// <summary>
        /// Get delivery notes for a particular driver
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns>200</returns>
        [HttpGet]
        [ResponseType(typeof(ICollection<DeliveryNote>))]
        [Route("api/GetDriverDeliveryNotes")]
        public async Task<IHttpActionResult> GetDriverDeliveryNote(int driverId)
        {
            Driver driver = await db.Drivers.FindAsync(driverId);
            if (driver == null)
            {
                return NotFound();
            }
            var deliveryNote = await db.DeliveryNotes.Where(b=>b.driverId==driverId).OrderByDescending(b=>b.date).ToListAsync();

            var notes = from d in deliveryNote
                        select
                                 new DeliveryNoteDTO
                                 {
                                     id = d.id,
                                     date = d.date,
                                     driverId = d.driverId,
                                     orderId = d.orderId,
                                     status = d.status
                                 };
            return Ok(notes);
        }


        /// <summary>
        /// Get delivery notes for the current logged in driver
        /// </summary>
        /// <returns>200</returns>
        [HttpGet]
        [ResponseType(typeof(ICollection<DeliveryNote>))]
        [Route("api/GetMyDeliveryNotes")]
        public async Task<IHttpActionResult> GetMyDeliveryNote()
        {
            string reg = User.Identity.GetUserId();
            Driver driver = db.Drivers.Where(b=>b.profile.userid==reg).SingleOrDefault();
            
            var deliveryNote = await db.DeliveryNotes.Where(b => b.driverId == driver.id).OrderByDescending(b => b.date).ToListAsync();
            var notes = from d in deliveryNote
                        select
                                 new DeliveryNoteDTO
                                 {
                                     id=d.id,
                                     date=d.date,
                                     driverId=d.driverId,
                                     orderId=d.orderId,
                                     status=d.status
                                 };
            return Ok(notes);
        }


        
        //PUT: api/DeliveryNotes/5
        /// <summary>
        /// This is used by the driver to tell the system that the order has been delivered, it will send an email to the customer with the delivery note and invoice
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("api/OrderDelivered")]
        public async Task<IHttpActionResult> PutDeliveryNote(int orderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            DeliveryNote deliveryNote = await db.DeliveryNotes.Where(b=>b.orderId==orderId).SingleOrDefaultAsync();
            if (deliveryNote == null)
            {
                return BadRequest("Delivery note not created");
            }

            deliveryNote.status = "delivered";
            db.Entry(deliveryNote).State = EntityState.Modified;

            
            Order order = await db.Orders.FindAsync(deliveryNote.orderId);
            if (order == null)
            {
                return NotFound();
            }
            if (order.status == "delivered")
            {
               return BadRequest("Order has been delivered already");
            }
            order.status = "delivered";
            db.Entry(order).State = EntityState.Modified;

            Invoice invoice = await db.Invoices.Where(b => b.orderid == order.id).SingleOrDefaultAsync();
            if (invoice == null)
            {
                return NotFound();
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }
            //send delivery note email
            string subject = "Supreme Brands Delivery Note (Delivery Note Number: "+deliveryNote.deliveryNoteNumber ;
            string body = "<html><head>";
            body += "<style> table {font - family: arial, sans - serif;border - collapse: collapse;width: 100 %; }td, th { border: 1px solid #dddddd; text - align: left; padding: 8px;} tr: nth - child(even) { background - color: #dddddd;}</style> ";
            body += "</head><body><h3>Dear Sir/Madama</h3><br><p>This serves as a delivery note for "+order.branch.customer.tradingName+" for the "+order.branch.name+" branch.</p>";
            body += "<h4>Invoice Number:      " + order.invoiceNumber + "</h4>";
            body += "<h4>Order Number:      " + order.orderNumber + "</h4>";
            body += "<h4>Delivery Note Number:      " + deliveryNote.deliveryNoteNumber + "</h4>";
            body += "<h4>Order Date:      " + order.date.ToShortDateString()+ "</h4><br>";

            body += "<table><tr><th> Order Item name </th> <th> Quantity </th> <th>Price per unit ($)</th> <th>Amount ($)</th> </tr>";
            double total = 0;
            foreach(OrderProduct item in order.orderProducts)
            {
                double cost = item.price * item.quantity;
                body += "<tr> <td> "+item.product.name+" </td><td>"+item.quantity+" </td> <td> "+item.price+" </td><td> "+cost+" </td></tr>";
                total += cost;
            }
            body += "<tr> <td> <b>Total</b> </td><td> </td> <td>  </td><td> <b>" + total+ "</b> </td></tr> </table>";
            body += "<br><br>Kind Regards<br> Supreme Brands Sales</body></html>";
            if (order.branch.email != null)
            {
                //Email.sendEmail(order.branch.email, subject, body);
            }
            

            //send invoice email

            string invoiceSubject = "Supreme Brands Invoice (Invoice number: " + invoice.invoiceNumber + ")";
            string invoiceBody = "<html><head>";
            invoiceBody += "<style> table {font - family: arial, sans - serif;border - collapse: collapse;width: 100 %; }td, th { border: 1px solid #dddddd; text - align: left; padding: 8px;} tr: nth - child(even) { background - color: #dddddd;}</style> ";
            invoiceBody +="</head><body><h3>Dear Sir/Madama</h3><br><br><p>This servers as an Invoice to " + order.branch.customer.tradingName + " for the " + order.branch.name + " branch</p>";
            invoiceBody += "<h4>Invoice Number:      " + invoice.invoiceNumber + "</h4>";
            invoiceBody += "<h4>Order Date:      " + invoice.date.ToShortDateString() + "</h4><br>";

            invoiceBody += "<table><tr><th> Order Item name </th> <th> Quantity </th> <th>Price per unit ($)</th> <th>Amount ($)</th> </tr> ";
           
            foreach (OrderProduct item in order.orderProducts)
            {
                double cost = item.price * item.quantity;
                invoiceBody += "<tr> <td> " + item.product.name + " </td><td>" + item.quantity + " </td> <td> " + item.price + " </td ><td> " + cost + " </td></tr>";
                
            }
            invoiceBody += "<tr> <td> <b>Total</b> </td><td> </td> <td>  </td><td> <b>" + total + "</b> </td></tr> </table>";
            invoiceBody += "<br><p> <a href='"+invoice.invoiceUrl+"'>Click Here</a> to download the copy of the invoice</p>";
            invoiceBody += "<br><br>Kind Regards<br> Supreme Brands Sales</body></html>";

            if (order.branch.email != null)
            {
                //Email.sendEmail(order.branch.email, invoiceSubject, invoiceBody);
            }

            //update vat
            TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");
            DateTime userTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeInfo);
            Vat vat = new Vat()
            {
                date = userTime,
                initialVat = total*0.15,
                remainingVat = total * 0.15,
                branch = order.branch,
                invoice = invoice
            };

            db.Vats.Add(vat);

            //update sales ledger
            SalesLedger sale = new SalesLedger()
            {
                order = order,
                date = userTime,
                deliveryNote = deliveryNote

            };
            db.SalesLedgers.Add(sale);

            //update debtors ledger

            db.SaveChanges();


            return Ok();
        }
       

        // POST: api/DeliveryNotes
        /// <summary>
        /// Create a delivery note, only drivers are authorized, the delivery note is initialised to pending
        /// </summary>
        /// <param name="deliveryNoteData"></param>
        /// <returns>200</returns>
        [Authorize(Roles ="driver")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostDeliveryNote(DeliveryNoteCreateDTO deliveryNoteData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string reg = User.Identity.GetUserId();
            Driver driver = db.Drivers.Where(b => b.profile.userid == reg).SingleOrDefault();
            if (driver == null)
            {
                return BadRequest("Driver does not exist");
            }
            TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");
            DateTime userTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeInfo);
            DeliveryNote deliveryNote = new DeliveryNote() { orderId = deliveryNoteData.orderId,driverId=driver.id,date= userTime,status="pending" };


            lock (Lock)
            {

                string year = DateTime.Now.Year.ToString();
                string reference = "DN-" + year + "-";
                int deliv = db.DeliveryNotes.Count();
                if (deliv == 0)
                {
                    deliv += 1;
                    reference += String.Format("{0:00000}", deliv);
                }
                else
                {
                    deliv = db.DeliveryNotes.Count(b => b.deliveryNoteNumber.Substring(3, 4) == year);
                    deliv += 1;
                    reference += String.Format("{0:00000}", deliv);
                }
                while (db.DeliveryNotes.Count(d => d.deliveryNoteNumber == reference) != 0)
                {
                    reference = "INV-" + year + "-" + String.Format("{0:00000}", ++deliv);
                }


                deliveryNote.deliveryNoteNumber = reference;
                db.DeliveryNotes.Add(deliveryNote);
                Order order = db.Orders.Find(deliveryNote.orderId);
                order.status = "transit";
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }


           

            return Ok();
        }

        // DELETE: api/DeliveryNotes/5
        /// <summary>
        /// This removes a delivery note by its id, only admin and accountants are authorized
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(DeliveryNote))]
        public async Task<IHttpActionResult> DeleteDeliveryNote(int id)
        {
            DeliveryNote deliveryNote = await db.DeliveryNotes.FindAsync(id);
            if (deliveryNote == null)
            {
                return NotFound();
            }

            db.DeliveryNotes.Remove(deliveryNote);
            await db.SaveChangesAsync();

            return Ok(deliveryNote);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DeliveryNoteExists(int id)
        {
            return db.DeliveryNotes.Count(e => e.id == id) > 0;
        }
    }
}