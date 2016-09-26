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
    public class ProductPricesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ProductPrices
        public IQueryable<ProductPricingDTO> GetProductPrices()
        {
            return from b in db.ProductPrices
                   select new ProductPricingDTO {
                       id= b.id,
                       amount = b.amount,
                       customerId = b.customerId,
                       description = b.description,
                       productId = b.productId,
                       sku = b.sku
                   };
        }

        // GET: api/ProductPrices/5
        /// <summary>
        /// It gets all pricing, necessary for debugging
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ProductPricingDTO))]
        public async Task<IHttpActionResult> GetProductPrice(int id)
        {
            ProductPrice b = await db.ProductPrices.FindAsync(id);
            if (b == null)
            {
                return NotFound();
            }
            ProductPricingDTO productPrice = new ProductPricingDTO
            {
                id = b.id,
                amount = b.amount,
                customerId = b.customerId,
                description = b.description,
                productId = b.productId,
                sku = b.sku
            };
            return Ok(productPrice);
        }

        // PUT: api/ProductPrices/5
        /// <summary>
        /// Edit Product pricing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productPrice"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProductPrice(int id, ProductPrice productPrice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productPrice.id)
            {
                return BadRequest();
            }

            db.Entry(productPrice).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductPriceExists(id))
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

        // POST: api/ProductPrices
        /// <summary>
        /// Create a product pricing, Authorized to accountants
        /// </summary>
        /// <param name="productPriceData"></param>
        /// <returns></returns>
        [Authorize(Roles ="accountant,administrator")]
        [ResponseType(typeof(ProductPrice))]
        public async Task<IHttpActionResult> PostProductPrice(ProductPriceCreateDTO productPriceData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int count = await db.ProductPrices.CountAsync(b => b.customerId == productPriceData.customerId && b.productId == productPriceData.productId);
            if (count != 0)
            {
                return BadRequest("Product Pricing already exists");
            }

            count = db.ProductPrices.Count(b => b.sku == productPriceData.sku);
            if (count != 0)
            {
                return BadRequest("Duplicate sku not allowed");
            }

            ProductPrice productPrice = new ProductPrice
            {
                customerId = productPriceData.customerId,
                amount = productPriceData.amount,
                productId = productPriceData.productId,
                sku = productPriceData.sku,
                description= productPriceData.description
            };
            db.ProductPrices.Add(productPrice);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = productPrice.id }, productPrice);
        }

        // DELETE: api/ProductPrices/5
        /// <summary>
        /// Remove a product pricing
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(ProductPrice))]
        public async Task<IHttpActionResult> DeleteProductPrice(int id)
        {
            ProductPrice productPrice = await db.ProductPrices.FindAsync(id);
            if (productPrice == null)
            {
                return NotFound();
            }

            db.ProductPrices.Remove(productPrice);
            await db.SaveChangesAsync();

            return Ok(productPrice);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductPriceExists(int id)
        {
            return db.ProductPrices.Count(e => e.id == id) > 0;
        }
    }
}