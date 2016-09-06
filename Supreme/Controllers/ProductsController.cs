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
    public class ProductsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Gets the products for a particular customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        [Route("api/GetCustomerProducts")]
        [HttpGet]
        public IQueryable<ProductDTO> GetCustomerProducts(int customerId, string productType)

        {
            var productPrices = from b in db.ProductPrices.Where(b => b.customerId == customerId && b.product.type==productType)
                                                     select new ProductDTO()
                                                     {
                                                         id = b.product.id,
                                                         description = b.product.description,
                                                         productName = b.product.name,
                                                         sku = b.product.sku
                                                     };

            return productPrices;
        }

        // GET: api/Products
        /// <summary>
        /// it retreives all products in the System
        /// </summary>
        /// <returns></returns>
        public IQueryable<Product> GetProducts()
{
            return db.Products;
        }

        // GET: api/Products/5
        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        /// <summary>
        /// Edit product, not yet refined
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        /// <summary>
        /// Create products, authorized to accountants
        /// </summary>
        /// <param name="productData"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(ProductCreateDTO productData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int count = db.Products.Count(b => b.sku == productData.sku);
            if (count != 0)
            {
                return BadRequest("Duplicate sku not allowed");
            }
            Product product = new Product
            {
                name = productData.name,
                description = productData.description,
                sku = productData.sku,
                type = productData.type
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = product.id }, product);
        }

        // DELETE: api/Products/5
        /// <summary>
        /// Delete a product, not yet implemented
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.id == id) > 0;
        }
    }
}