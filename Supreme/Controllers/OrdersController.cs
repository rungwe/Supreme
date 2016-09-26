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
    public class OrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static readonly object Lock = new object();

        // GET: api/Orders
        /// <summary>
        /// Retrieves all orders made, for the administrators only
        /// </summary>
        /// <returns>200</returns>
        [Authorize(Roles ="administrator")]
        public IQueryable<OrderDTO> GetOrders()
        {
            var orders=  from b in db.Orders select new OrderDTO {
                id= b.id,
                price= b.price,
                date = b.date,
                status = b.status,
                salesRep = new SalesRepDTO () {id= b.salesRep.id, firstname=b.salesRep.profile.firstname, middlename = b.salesRep.profile.middlename, lastname= b.salesRep.profile.lastname},
                branch = new BranchDTO() { id=b.branch.id, address = b.branch.address, name= b.branch.name, email= b.branch.email, regionId= b.branch.regionId,telephone= b.branch.telephone},
                customer = new CustomerDTO { id=b.branch.customer.id, tradingName = b.branch.customer.tradingName, registrationDate= b.branch.customer.registrationDate },
                invoiceNumber= b.invoiceNumber,
                orderNumber=b.orderNumber
            };

            return orders;
        }

        // GET: api/Orders/5
        /// <summary>
        /// This retrieves an order by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200</returns>
        [ResponseType(typeof(OrderDTO))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            Order b = await db.Orders.FindAsync(id) ;
            if (b == null)
            {
                return NotFound();
            }

            var order = new OrderDTO
            {
                id = b.id,
                price = b.price,
                date = b.date,
            
                status = b.status,
                salesRep = new SalesRepDTO() { id = b.salesRep.id, firstname = b.salesRep.profile.firstname, middlename = b.salesRep.profile.middlename, lastname = b.salesRep.profile.lastname },
                branch = new BranchDTO() { id = b.branch.id, address = b.branch.address, name = b.branch.name, email = b.branch.email, regionId = b.branch.regionId, telephone = b.branch.telephone },
                customer = new CustomerDTO { id = b.branch.customer.id, tradingName = b.branch.customer.tradingName, registrationDate = b.branch.customer.registrationDate },
                invoiceNumber = b.invoiceNumber,
                orderNumber = b.orderNumber
            };
            return Ok(order);
        }


        /// <summary>
        /// This retrieves all the pending orders for the current Sales Rep, only authorized to Sales Rep
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles ="sales")]
        [Route("api/SalesRepMyPendingOrders")]
        [HttpGet]
        public async Task<IHttpActionResult> SalesRepMyPendingOrders()
        {
            string reg = User.Identity.GetUserId();
            SalesRep salesRep = await db.SalesReps.Where(b => b.userid == reg).SingleOrDefaultAsync();
            var order =  from d in db.Orders.Where(d=>d.salesRepId==salesRep.id && d.status=="pending").OrderByDescending(d=>d.date)
                         select new OrderDTO()
                         {
                             id = d.id,
                             status = d.status,
                             branch = new BranchDTO { id=d.branch.id, address=d.branch.address, name=d.branch.name,email=d.branch.email,regionId=d.branch.regionId,telephone=d.branch.telephone},
                             customer= new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                             date = d.date,
                             orderNumber = d.orderNumber
                         };

            return Ok(order);
        }


        /// <summary>
        /// This retrieves all the pending orders for the current Sales Rep, only authorized to Sales Rep
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "merchant")]
        [Route("api/MerchantMyPendingOrders")]
        [HttpGet]
        public async Task<IHttpActionResult> MerchantMyPendingOrders()
        {
            
            string reg = User.Identity.GetUserId();
            Merchant merchant = await db.Merchants.Where(b => b.profile.userid == reg).SingleOrDefaultAsync();
            //SalesRep salesRep = await db.SalesReps.Where(b => b.userid == reg).SingleOrDefaultAsync();
            var order = from d in db.Orders.Where(d => d.branch.merchantId == merchant.id && d.status == "pending").OrderByDescending(d => d.date)
                        select new OrderDTO()
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            orderNumber = d.orderNumber
                        };

            return Ok(order);
        }

        /// <summary>
        /// Retrieves all pending orders, only accessible by the accountants and the administrators
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles ="accountant,administrator")]
        [Route("api/AllPendingOrders")]
        [HttpGet]
        public IQueryable<OrderDTO> AllPendingOrders()
        {
            
            var order = from d in db.Orders.Where(d => d.status == "pending").OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            price=d.price,
                            salesRep = new SalesRepDTO() { id=d.salesRepId,firstname=d.salesRep.profile.firstname,lastname= d.salesRep.profile.lastname,middlename= d.salesRep.profile.middlename },
                           
                            orderNumber = d.orderNumber
                        };

            return order;
        }

        /// <summary>
        /// this returns all the order under processing for the current logged in Sales rep, only sales reps are authorized
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "sales")]
        [Route("api/SalesRepMyOrdersInProcess")]
        [HttpGet]
        public async Task<IHttpActionResult> SalesRepMyOrdersInProcess()
        {
            string reg = User.Identity.GetUserId();
            SalesRep salesRep = await db.SalesReps.Where(b => b.userid == reg).SingleOrDefaultAsync();
            var order =from d in db.Orders.Where(d => d.salesRepId == salesRep.id && d.status != "pending" && d.status!="delivered").OrderByDescending(d => d.date)
                       select new OrderDTO()
                       {
                           id = d.id,
                           status = d.status,
                           branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                           customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                           date = d.date,
                           invoiceNumber = d.invoiceNumber,
                           orderNumber = d.orderNumber

                       };

            return Ok(order);
        }



        /// <summary>
        /// this returns all the order under processing for the current logged in Merchant, these are orders made by all his Sales Reps, only sales reps are authorized
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "merchant")]
        [Route("api/MerchantMyOrdersInProcess")]
        [HttpGet]
        public async Task<IHttpActionResult> MerchantMyOrdersInProcess()
        {
            string reg = User.Identity.GetUserId();
            Merchant merchant = await db.Merchants.Where(b => b.profile.userid == reg).SingleOrDefaultAsync();
            var order = from d in db.Orders.Where(d => d.branch.merchantId == merchant.id && d.status != "pending" && d.status != "delivered").OrderByDescending(d => d.date)
                        select new OrderDTO()
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber

                        };

            return Ok(order);
        }



        /// <summary>
        /// this retrieves all the orders that have been approved and invoiced by the accountant, only stock_controllers, accountants and administrators are authorized
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "stock_controller,administrator,accountant")]
        [Route("api/AllInProcessOrders")]
        [HttpGet]
        public IQueryable<OrderDTO> AllInProcessOrders()
        {

            var order = from d in db.Orders.Where(d => d.status != "pending" && d.status!="delivered").OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber
                        };

            return order;
        }



        /// <summary>
        /// this retrieves all the orders that have been approved and invoiced by the accountant, only stock_controllers, accountants and administrators are authorized
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "stock_controller,administrator,accountant")]
        [Route("api/AllInProcessingDispatchOrders")]
        [HttpGet]
        public IQueryable<OrderDTO> AllInProcessingInDispatchOrders()
        {

            var order = from d in db.Orders.Where(d => d.status == "processing").OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber
                        };

            return order;
        }



        /// <summary>
        /// this retrieves all the orders that have been approved and invoiced by the accountant for the specific warehouse, stock controllers authorized
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "stock_controller")]
        [Route("api/StockControllerProcessingInDispatchOrders")]
        [HttpGet]
        public async Task<IHttpActionResult> StockControllerProcessingInDispatchOrders()
        {
            string reg = User.Identity.GetUserId();
            StockManager manager = await db.StockManagers.Where(b => b.profile.userid == reg).SingleOrDefaultAsync();
            var order =  from d in db.Orders.Where(d => d.status == "processing" && d.warehouseLocation==manager.warehouseLocation).OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber
                        };

            return Ok(order);
        }



        /// <summary>
        /// This retrieves all delivered orders initated by the current logged in sales rep, authorized to only sales reps
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "sales")]
        [Route("api/SalesRepMyDeliveredOrders")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMyDeliveredOrders()
        {
            string reg = User.Identity.GetUserId();
            SalesRep salesRep = await db.SalesReps.Where(b => b.userid == reg).SingleOrDefaultAsync();
            var order = from d in db.Orders.Where(d => d.salesRepId == salesRep.id && d.status == "delivered").OrderByDescending(d => d.date)
                        select new OrderDTO()
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber
                        };

            return Ok(order);
        }


        /// <summary>
        /// This retrieves all delivered orders initated by the current logged in sales rep, authorized to only sales reps
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "merchant")]
        [Route("api/GetMerchantDeliveredOrders")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMerchantDeliveredOrders()
        {
            string reg = User.Identity.GetUserId();
            Merchant merchant = await db.Merchants.Where(b => b.profile.userid == reg).SingleOrDefaultAsync();
            var order = from d in db.Orders.Where(d => d.branch.merchantId == merchant.id && d.status == "delivered").OrderByDescending(d => d.date)
                        select new OrderDTO()
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber
                        };

            return Ok(order);
        }


        /// <summary>
        /// It returns all delivered orders
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize]
        [Route("api/AllDeliveredOrders")]
        [HttpGet]
        public IQueryable<OrderDTO> AllDeliveredOrders()
        {

            var order = from d in db.Orders.Where(d => d.status == "delivered").OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber
                        };

            return order;
        }


        /// <summary>
        /// It retrieves orders cancelled by the logged in Salles Rep, authorized to sales reps
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "sales")]
        [Route("api/GetMyCancelledOrders")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCancelledOrders()
        {
            string reg = User.Identity.GetUserId();
            SalesRep salesRep = await db.SalesReps.Where(b => b.userid == reg).SingleOrDefaultAsync();
            var order = from d in db.Orders.Where(d => d.salesRepId == salesRep.id && d.status == "cancelled").OrderByDescending(d => d.date)
                        select new OrderDTO()
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber
                        };

            return Ok(order);
        }

        /// <summary>
        /// View all the cancelled orders, only the accountants and administrators are authorized
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles ="administrator,accountant")]
        [Route("api/AllCancelledOrders")]
        [HttpGet]
        public IQueryable<OrderDTO> AllCancelledOrders()
        {

            var order = from d in db.Orders.Where(d => d.status == "cancelled").OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber
                        };

            return order;
        }



        /// <summary>
        /// View all the orders in transit, only the accountants and administrators are authorized
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "administrator,accountant,stock_controller,driver")]
        [Route("api/AllTransitOrders")]
        [HttpGet]
        public IQueryable<OrderDTO> AllTransitOrders()
        {

            var order = from d in db.Orders.Where(d => d.status == "transit").OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.id,
                            status = d.status,
                            branch = new BranchDTO { id = d.branch.id, address = d.branch.address, name = d.branch.name, email = d.branch.email, regionId = d.branch.regionId, telephone = d.branch.telephone },
                            customer = new CustomerDTO { id = d.branch.customer.id, tradingName = d.branch.customer.tradingName, registrationDate = d.branch.customer.registrationDate },
                            date = d.date,
                            invoiceNumber = d.invoiceNumber,
                            orderNumber = d.orderNumber
                        };

            return order;
        }

        /// <summary>
        /// View all the orders in transit for the current logged in driver, only the drivers are authorized
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "driver")]
        [Route("api/MyTransitOrders")]
        [HttpGet]
        public IQueryable<OrderDTO> MyTransitOrders()
        {
            string reg = User.Identity.GetUserId();

            Driver driver = db.Drivers.Where(b => b.profile.userid == reg).SingleOrDefault();
            if (driver == null)
            {
                return null;
            }
            var order = from d in db.DeliveryNotes.Where(d => d.order.status == "transit" && d.driverId==driver.id).OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.order.id,
                            status = d.order.status,
                            branch = new BranchDTO { id = d.order.branch.id, address = d.order.branch.address, name = d.order.branch.name, email = d.order.branch.email, regionId = d.order.branch.regionId, telephone = d.order.branch.telephone },
                            customer = new CustomerDTO { id = d.order.branch.customer.id, tradingName = d.order.branch.customer.tradingName, registrationDate = d.order.branch.customer.registrationDate },
                            date = d.order.date,
                            invoiceNumber = d.order.invoiceNumber,
                            orderNumber = d.order.orderNumber
                        };

            return order;
        }

        /// <summary>
        /// View all the orders in transit for the current logged in driver, only the drivers are authorized
        /// </summary>
        /// <returns>200</returns>
        [ResponseType(typeof(ICollection<OrderDTO>))]
        [Authorize(Roles = "driver")]
        [Route("api/DriverMyDeliveredOrders")]
        [HttpGet]
        public IQueryable<OrderDTO> DriverMyDeliveredOrders()
        {
            string reg = User.Identity.GetUserId();

            Driver driver = db.Drivers.Where(b => b.profile.userid == reg).SingleOrDefault();
            if (driver == null)
            {
                return null;
            }
            var order = from d in db.DeliveryNotes.Where(d => d.order.status == "delivered" && d.driverId == driver.id).OrderByDescending(d => d.date)
                        select new OrderDTO
                        {
                            id = d.order.id,
                            status = d.order.status,
                            branch = new BranchDTO { id = d.order.branch.id, address = d.order.branch.address, name = d.order.branch.name, email = d.order.branch.email, regionId = d.order.branch.regionId, telephone = d.order.branch.telephone },
                            customer = new CustomerDTO { id = d.order.branch.customer.id, tradingName = d.order.branch.customer.tradingName, registrationDate = d.order.branch.customer.registrationDate },
                            date = d.order.date,
                            invoiceNumber = d.order.invoiceNumber,
                            orderNumber = d.order.orderNumber
                        };

            return order;
        }

        /*8
        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        /// <summary>
        /// Make an order, only sales reps are authorized
        /// </summary>
        /// <param name="orderData"></param>
        /// <returns></returns>
        [Authorize(Roles ="sales,merchant")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostOrder(OrderCreateDTO orderData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Branch branch = await db.Branches.FindAsync(orderData.branchId);
            if (branch == null)
            {
                return BadRequest("Branch does not Exist");
            }

            Customer customer = await db.Customers.FindAsync(branch.customerId);
            if (customer == null)
            {
                return BadRequest("Customer does not Exist");
            }

           if (customer.id != branch.customerId)
            {
                return BadRequest("This branch does not belong to this customer");
            }

            //ProductPrice price = await db.ProductPrices.Where(b => b.productId==product.id).SingleOrDefaultAsync();
        
            string reg = User.Identity.GetUserId();
            SalesRep sales = await db.SalesReps.Where(a =>a.userid == reg).SingleAsync();
            TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");
            DateTime userTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeInfo);
            Order order = new Order
            {
                date = userTime,
                customerId = branch.customerId,
                branchId = orderData.branchId,
                status = "pending",
                salesRepId = sales.id,
                
            };
            Order odr= db.Orders.Add(order);
            await db.SaveChangesAsync();

            double totalPrice = 0;
            List<OrderProduct> orderItems = new List<OrderProduct>();
            foreach (OrderItem orderItem in orderData.orderItem)
            {
                double currentPrice = 0;
                Product product = await db.Products.FindAsync(orderItem.productId);
                if (product == null)
                {
                    return BadRequest("Product of id " + orderItem.productId + " does not Exist");
                }

                ProductPrice price = await db.ProductPrices.Where(b => b.productId == product.id && b.customerId == customer.id).SingleOrDefaultAsync();

                if (price == null)
                {
                    return BadRequest("This retailer does not have this product");
                }

                currentPrice = price.amount;
                totalPrice += currentPrice*orderItem.quantity;

                
                orderItems.Add(new OrderProduct { orderId = odr.id, price = currentPrice, productId = orderItem.productId, quantity = orderItem.quantity,sku =price.sku });
                //db.OrderProducts.

            }
            lock (Lock)
            {

                string year = DateTime.Now.Year.ToString();
                string reference = "PO-" + year + "-";
                int ordNum = db.Orders.Count();
                if (ordNum == 0)
                {
                    ordNum += 1;
                    reference += String.Format("{0:00000}", ordNum);
                }
                else
                {
                    ordNum = db.Orders.Count(b => b.orderNumber.Substring(3, 4) == year);
                    ordNum += 1;
                    reference += String.Format("{0:00000}", ordNum);
                }
                while (db.Orders.Count(d => d.orderNumber == reference) != 0)
                {
                    reference = "INV-" + year + "-" + String.Format("{0:00000}", ++ordNum);
                }
               

                odr.price = totalPrice;
                odr.orderNumber = reference;
                db.Entry(odr).State = EntityState.Modified;
                db.OrderProducts.AddRange(orderItems);
                db.SaveChanges();
            }
           

            return Ok(odr.orderNumber);
        }

        // DELETE: api/Orders/5
        /// <summary>
        /// Cancel an order, only the sales rep who made the order can cancel, any accountant can cancel an order, if it was in pending state it will be completely removed otherwise it will be marked as cancelled
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles ="accountant,sales")]
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
            
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            //if its a sales rep, check if he/she is the creator otherwise throw an error
            string reg = User.Identity.GetUserId();
            SalesRep sales = await db.SalesReps.Where(b=>b.userid==reg).SingleOrDefaultAsync();

            if (sales != null)
            {
                if (sales.id !=order.id)
                {
                    return Unauthorized();
                }
            }
            //SalesRep sales1 = await db.SalesReps.FindAsync(order.salesRepId);

            order.status = "cancelled";
            db.Entry(order).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok();
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.id == id) > 0;
        }
    }
}