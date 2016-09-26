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
    public class DispatchesController : ApiController
    {
        /// <summary>
        /// Methods to handle dispatches
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Dispatches
        /// <summary>
        /// Retrieves all the Dispaches, Necessary for drivers
        /// </summary>
        /// <returns></returns>
        public IQueryable<DispatchDTO> GetDispatches()
        {
            var dispatches = from b in db.Dispatches
                             select new DispatchDTO()
                             {
                                 id = b.id,
                                 orderId = b.orderId,
                                 stockManager = new StockManagerDTO() { firstname = b.stockManager.profile.firstname, id = b.stockManager.id, lastname = b.stockManager.profile.lastname, middlename = b.stockManager.profile.middlename }
                             };
            return dispatches;
        }

        // GET: api/Dispatches/5
        /// <summary>
        /// Get a dispach by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(DispatchDTO))]
        public async Task<IHttpActionResult> GetDispatch(int id)
        {
            Dispatch b = await db.Dispatches.FindAsync(id);
            if (b == null)
            {
                return NotFound();
            }
            DispatchDTO dispatch = new DispatchDTO()
            {
                id = b.id,
                orderId = b.orderId,
                stockManager = new StockManagerDTO() { firstname = b.stockManager.profile.firstname, id = b.stockManager.id, lastname = b.stockManager.profile.lastname, middlename = b.stockManager.profile.middlename }
            };
            return Ok(dispatch);
        }

        /*
        // PUT: api/Dispatches/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDispatch(int id, Dispatch dispatch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dispatch.id)
            {
                return BadRequest();
            }

            db.Entry(dispatch).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DispatchExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }**/
        

        // POST: api/Dispatches
        /// <summary>
        /// This is used by stock controllers to signal drivers when an order is ready for deivery, Authorized to Stock controllers
        /// </summary>
        /// <param name="dispatchData"></param>
        /// <returns>200</returns>
        [Authorize(Roles ="stock_controller")]
        [ResponseType(typeof(Dispatch))]
        public async Task<IHttpActionResult> PostDispatch(DispatchCreateDTO dispatchData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string reg = User.Identity.GetUserId();
            Profile prof = db.Profiles.Where(b => b.userid==reg).SingleOrDefault();
            StockManager manager = db.StockManagers.Where(b => b.profileId == prof.id).SingleOrDefault();
            Dispatch dispatch = new Dispatch() { orderId = dispatchData.orderId, stockManagerId = manager.id,date=DateTime.Now };
            Order order = db.Orders.Find(dispatchData.orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.status = "dispatched";
            db.Entry(order).State = EntityState.Modified;
            db.Dispatches.Add(dispatch);
            await db.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// View all orders dispatched by the current logged in stock controller, only the stock Manager
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "stock_controller")]
        [Route("api/GetMyDispatchedOrders")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDispatchedOrders()
        {
            string reg = User.Identity.GetUserId();
            StockManager manager = await db.StockManagers.Where(b => b.profile.userid == reg).SingleOrDefaultAsync();
            var order = from d in db.Dispatches.Where(d => d.stockManagerId == manager.id && d.order.status == "dispatched").OrderByDescending(d => d.id)
                        select new OrderDTO()
                        {
                            id = d.order.id,
                            status = d.order.status,
                            branch = new BranchDTO { id = d.order.branch.id, address = d.order.branch.address, name = d.order.branch.name, email = d.order.branch.email, regionId = d.order.branch.regionId, telephone = d.order.branch.telephone },
                            customer = new CustomerDTO { id = d.order.branch.customer.id, tradingName = d.order.branch.customer.tradingName, registrationDate = d.order.branch.customer.registrationDate },
                            date = d.order.date,
                            orderNumber = d.order.orderNumber,
                            price = d.order.price,
                            invoiceNumber = d.order.invoiceNumber,
                            warehouseLocation = d.order.warehouseLocation
                        };

            return Ok(order);
        }

        /// <summary>
        /// View all the dispatched orders
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize]
        [Route("api/AllDispatchedOrders")]
        [HttpGet]
        public IQueryable<OrderDTO> AllDispatchedOrders()
        {

            var order = from d in db.Orders.Where(d => d.status == "dispatched").OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            orderNumber= d.orderNumber,
                            price = d.price,
                            invoiceNumber = d.invoiceNumber,
                            warehouseLocation = d.warehouseLocation
                        };

            return order;
        }

        // DELETE: api/Dispatches/5
        /// <summary>
        /// Businnes logic not yet defined
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Dispatch))]
        public async Task<IHttpActionResult> DeleteDispatch(int id)
        {
            Dispatch dispatch = await db.Dispatches.FindAsync(id);
            if (dispatch == null)
            {
                return NotFound();
            }

            db.Dispatches.Remove(dispatch);
            await db.SaveChangesAsync();

            return Ok(dispatch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DispatchExists(int id)
        {
            return db.Dispatches.Count(e => e.id == id) > 0;
        }
    }
}