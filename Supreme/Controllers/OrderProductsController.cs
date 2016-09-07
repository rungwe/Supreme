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
    /// This encapsulates products in an order
    /// </summary>
    public class OrderProductsController : ApiController
    {
       
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/OrderProducts
        /// <summary>
        /// Neccessary only for debugging
        /// </summary>
        /// <returns>200</returns>
        public IQueryable<OrderProductDTO> GetOrderProducts()
        {
            return from b in db.OrderProducts
                   select
                        new OrderProductDTO
                        {
                        productId = b.productId,
                        productName = b.product.name,
                        price = b.price,
                        productDescription = b.product.description,
                        quantity = b.quantity

                        };
        }

        /// <summary>
        /// Retrieve products of an order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>200</returns>
        [Route("api/GetOrderProducts")]
        [HttpGet]
        public IQueryable<OrderProductDTO> GetOrderProducts(int orderId)
        {
            return from b in db.OrderProducts.Where(b=>b.orderId==orderId) select
                   new OrderProductDTO
                   {
                       productId = b.productId,
                       productName = b.product.name,
                       price = b.price,
                       productDescription = b.product.description,
                       quantity = b.quantity
                       
                   }
                   ;
        }

        // GET: api/OrderProducts/5
        /// <summary>
        /// Get order product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(OrderProduct))]
        public async Task<IHttpActionResult> GetOrderProductInstance(int id)
        {
            OrderProduct b = await db.OrderProducts.FindAsync(id);
            if (b == null)
            {
                return NotFound();
            }
            OrderProductDTO orderProduct = new OrderProductDTO
            {
                productId = b.productId,
                productName = b.product.name,
                price = b.price,
                productDescription = b.product.description,
                quantity = b.quantity

            };
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