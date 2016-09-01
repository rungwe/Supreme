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
    public class OrderProductsController : ApiController
    {
        /// <summary>
        /// This encapsulates products in an oder
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/OrderProducts
        /// <summary>
        /// Neccessary only for debugging
        /// </summary>
        /// <returns></returns>
        public IQueryable<OrderProduct> GetOrderProducts()
        {
            return db.OrderProducts;
        }

        // GET: api/OrderProducts/5
        /// <summary>
        /// Get order product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(OrderProduct))]
        public async Task<IHttpActionResult> GetOrderProduct(int id)
        {
            OrderProduct orderProduct = await db.OrderProducts.FindAsync(id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            return Ok(orderProduct);
        }
        /// <summary>
        /// retrieves order products, not implemented
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(OrderProduct))]
        [Route("GetOrderProducts")]
        public async Task<IHttpActionResult> GetOrderProducts(int id)
        {
            OrderProduct orderProduct = await db.OrderProducts.FindAsync(id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            return Ok(orderProduct);
        }

        // PUT: api/OrderProducts/5
        /// <summary>
        /// This is meant for editing 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderProduct"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOrderProduct(int id, OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderProduct.id)
            {
                return BadRequest();
            }

            db.Entry(orderProduct).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderProductExists(id))
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

        // POST: api/OrderProducts
        /// <summary>
        /// Creates a product for an order
        /// </summary>
        /// <param name="orderProduct"></param>
        /// <returns></returns>
        [ResponseType(typeof(OrderProduct))]
        public async Task<IHttpActionResult> PostOrderProduct(OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderProducts.Add(orderProduct);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = orderProduct.id }, orderProduct);
        }

        // DELETE: api/OrderProducts/5
        /// <summary>
        /// Remove an order product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(OrderProduct))]
        public async Task<IHttpActionResult> DeleteOrderProduct(int id)
        {
            OrderProduct orderProduct = await db.OrderProducts.FindAsync(id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            db.OrderProducts.Remove(orderProduct);
            await db.SaveChangesAsync();

            return Ok(orderProduct);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderProductExists(int id)
        {
            return db.OrderProducts.Count(e => e.id == id) > 0;
        }
    }
}