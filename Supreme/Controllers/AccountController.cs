using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Supreme.Models;
using Supreme.Providers;
using Supreme.Results;
using System.Web.Http.Description;
using System.Linq;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System.Net;
using System.Data.Entity.Validation;
using Supreme.Library;

namespace Supreme.Controllers
{
    /// <summary>
    /// Handles users accounts
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }


        // POST api/Account/ChangePassword
        /// <summary>
        /// Change the user password, users are allowed to change passwords on their own
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        /// <summary>
        /// Set user password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
        

        //set up admin
        /// <summary>
        /// This method does register and initialise an administrator in the system, no secrete keys are required, its for debugging purposes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("setAdmin")]
        public async Task<IHttpActionResult> setAdmin(RegisterBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, "Supreme123");


            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }


            using (var context = new ApplicationDbContext())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                await userManager.AddToRoleAsync(user.Id, "administrator");
            }

            var person = db.Users.Where(b => b.Email == model.Email).First();
            Profile prof = new Profile
            {
                email = model.Email,
                firstname = model.firstname,
                lastname = model.lastname,
                userid = person.Id,
                position = "administrator",
                registrationDate = DateTime.Now
            };

            db.Profiles.Add(prof);
            await db.SaveChangesAsync();
            Administrator admin = new Administrator()
            {
                profileId=prof.id
            };

            db.Administrators.Add(admin);
            await db.SaveChangesAsync();
            return Ok();

        }

        // POST api/Account/Register
        /// <summary>
        /// This is used to register users into the system, only administrators are authorized, to register another administrator, access_key and secrete keys fields are required only on this scenerio otherwise they are not needed 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "administrator")]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            string access_key = "dhgubefbcdbjskbvbsbcksbvsvhdbbhkbrhghoritutuiiw29283746@@@#$%6&*TFRDTRSRTYUSERTVHGDWWSFGHJTRESVHG";
            string secrete_key = "bsvdsvhksvhsfvyievfyifevhfyavhfyciefvhyefhvgy79t957t79t52y275t727t95t7t734rtgyegyaghcvh vjjvjv";

            if (model.role.Equals("administrator"))
            {
                if (model.secrete_key != null && model.access_key != null)
                {
                    if (model.secrete_key.Equals(secrete_key) && model.access_key.Equals(access_key))
                    {

                    }
                    else
                    {
                        return StatusCode(HttpStatusCode.Forbidden);
                    }
                }
                else
                {
                    return StatusCode(HttpStatusCode.Forbidden);
                }
            }

            string[] roles = { "sales", "accountant", "driver", "administrator", "merchant", "stock_controller" };
            // string Password = RandomPassword(8);
            string Password = "supreme123";

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            bool check = false;
            for (int i = 0; i < roles.Length; i++)
            {
                if (roles[i].Equals(model.role))
                {
                    check = true;
                }

            }

            if (check == false)
            {
                return BadRequest(ModelState);
            }
           

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, Password);


            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }


            using (var context = new ApplicationDbContext())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                await userManager.AddToRoleAsync(user.Id, model.role);
            }
            var person = db.Users.Where(b => b.Email == model.Email).First();
            // send email with password
            string body = "<html><h3>Hi " + model.firstname + " " + model.lastname + "</h3><br><b>Your Login Credentials</b><br><br><b>username: </b> " + model.Email + "<br><b>password: </b> " + Password + "<br><br><br>you can always change the password in the app.<br><br>Kind regards<br>Supreme brands team</html>";
            if (!Email.sendEmail(model.Email, "Supreme Brands App details", body))
            {

                db.Users.Remove(person);
                await db.SaveChangesAsync();

                return StatusCode(HttpStatusCode.ExpectationFailed);

            };
            string err = "";
            Profile prof = new Profile()
            {
                email = model.Email,
                firstname = model.firstname,
                lastname = model.lastname,
                userid = person.Id,
                position = model.role,
                registrationDate = DateTime.Now,


            };
            try
            {
               db.Profiles.Add(prof);
               await db.SaveChangesAsync();
            }


            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        err = "Property: " + validationError.PropertyName + "  error: " + validationError.ErrorMessage;
                    }
                }
            }

            if (model.role == "sales")
            {
                SalesRep sales = new SalesRep
                {
                    profileId = prof.id,
                    userid = person.Id
                };

                db.SalesReps.Add(sales);
                await db.SaveChangesAsync();
            }

            else if (model.role == "accountant")
            {
                Accountant acc = new Accountant
                {
                    profileId = prof.id,
                    user_id = person.Id
                };

                db.Accountants.Add(acc);
                await db.SaveChangesAsync();
            }

            else if (model.role == "stock_controller")
            {
                StockManager manager = new StockManager
                {
                    profileId = prof.id,
                   
                };

                db.StockManagers.Add(manager);
                await db.SaveChangesAsync();
            }

            else if (model.role == "driver")
            {
                Driver manager = new Driver
                {
                    profileId = prof.id,
                    

                };

                db.Drivers.Add(manager);
                await db.SaveChangesAsync();
            }

            else if (model.role == "administrator")
            {
                Administrator manager = new Administrator
                {
                    profileId = prof.id,
                    

                };

                db.Administrators.Add(manager);
                await db.SaveChangesAsync();
            }

            else if (model.role == "merchant")
            {
                Merchant manager = new Merchant
                {
                    profileId = prof.id

                };

                db.Merchants.Add(manager);
                await db.SaveChangesAsync();
            }


            return Ok();






        }

       

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

 
    

        private static Random random = new Random();
        [ApiExplorerSettings(IgnoreApi = true)]
        public string RandomPassword(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion
    }
}
