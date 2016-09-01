using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Supreme.Models;
using System.Web.Http.Description;
using System.IO;
using System.Data.Entity;
using System.Web;

namespace Supreme.Controllers
{
    public class UploadsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        [HttpPost]
        [Route("profilePic")]
        [ResponseType(typeof(FileDTO))]
        public async Task<IHttpActionResult> profilePic()
        {
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                string reg = User.Identity.GetUserId();
              
                Profile user = await db.Profiles.Where(d => d.userid == reg).SingleOrDefaultAsync();

                // Get the uploaded image from the Files collection
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                Stream img = httpPostedFile.InputStream;
                if (user != null)
                {


                    if (httpPostedFile != null)
                    {
                        string oldname = user.profile_pic;
                        string filename = Guid.NewGuid().ToString() + httpPostedFile.FileName.Replace(" ", "-");

                        bool status = UploadFileToS3(filename, img, "supremebrands");
                        if (status)
                        {
                            user.profile_pic = "https://s3-us-west-2.amazonaws.com/supremebrands/" + filename;
                            db.Entry(user).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            if (oldname != null)
                            {
                                deleteFile(oldname.Replace("https://s3-us-west-2.amazonaws.com/supremebrands/", ""), "supremebrands");
                            }
                            return Ok(new FileDTO() { url= user.profile_pic });
                        }
                        else
                        {
                            return StatusCode(HttpStatusCode.NotAcceptable);
                        }
                    }
                }

                else 
                {
                    return StatusCode(HttpStatusCode.Forbidden);
                }



            }

            return StatusCode(HttpStatusCode.ExpectationFailed);

        }




        [ApiExplorerSettings(IgnoreApi = true)]
        public bool UploadFileToS3(string uploadAsFileName, Stream ImageStream, string toWhichBucketName)
        {


            AmazonS3Client client = new AmazonS3Client("AKIAJENNMZYOVK4V7XJA", "AnNyJMBTBAGvhcUF1N6gVfW1vQtmyC4allr1NVCf", Amazon.RegionEndpoint.USWest2);

            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(client);

                // 3. Upload data from a type of System.IO.Stream.

                
                
                fileTransferUtility.Upload(ImageStream,toWhichBucketName, uploadAsFileName);
                fileTransferUtility.S3Client.MakeObjectPublic(toWhichBucketName, uploadAsFileName, true);


                TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                  Key = uploadAsFileName,
                  InputStream = ImageStream,
                  BucketName = toWhichBucketName,
                  CannedACL = S3CannedACL.PublicRead,
                  StorageClass = S3StorageClass.Standard
                };

                

            }
            catch (Exception e)
            {

                return false;
            }
            return true;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool deleteFile(string keyName, string bucketName)
        {

            AmazonS3Client client = new AmazonS3Client("AKIAJENNMZYOVK4V7XJA", "AnNyJMBTBAGvhcUF1N6gVfW1vQtmyC4allr1NVCf", Amazon.RegionEndpoint.USWest2);

            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(client);

                fileTransferUtility.S3Client.Delete(bucketName, keyName, null);

            }

            catch (Exception)
            {

                return false;
            }
            return true;
        }

    }



}
