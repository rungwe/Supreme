using Amazon.S3;
using Amazon.S3.Transfer;
using Supreme.Library;
using Supreme.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Supreme.Controllers
{
    public class InvoiceTestController : ApiController
    {   /*
        [HttpPost]
        [Route("api/invoiceTest")]
        [ResponseType(typeof(FileDTO))]
        public async Task<IHttpActionResult>testInvoice()
        {
            InvoiceGenerator inv = new InvoiceGenerator();
            Stream invPdf = inv.CreateInvoicePDF(15);
            FileDTO file;
            string filename = "invoice7.pdf";

            string bucket = "supremebrands";

            StringWriter s = new StringWriter();
           
            if (UploadFileToS3(filename, invPdf, bucket))
            {
                file = new FileDTO() { url = "https://s3-us-west-2.amazonaws.com/supremebrands/" + filename };

                string body = string.Format(@"
                <!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN""      ""http://www.w3.org/TR/html4/strict.dtd"">
                <html>
                <body>
                    <h2> Dear sir/madam</h2>
                    <br>
                    <p>This email contains the link of the Invoice pdf created by the system</p>
                    <p><a href='{0}'>Click here</a> to open the pdf </p>
                    <p>Or copy and paste this link in your browser </p>
                    <p>{1}</b>
                    
                </body>
                </html>
                ", file.url,file.url);

                Email.sendEmail("faraimose@gmail.com", "invoice test", body);

                return Ok(file);
            }
            else
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }

        }
        **/
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool UploadFileToS3(string uploadAsFileName, Stream ImageStream, string toWhichBucketName)
        {


            AmazonS3Client client = new AmazonS3Client("AKIAJENNMZYOVK4V7XJA", "AnNyJMBTBAGvhcUF1N6gVfW1vQtmyC4allr1NVCf", Amazon.RegionEndpoint.USWest2);

            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(client);

                // 3. Upload data from a type of System.IO.Stream.



                fileTransferUtility.Upload(ImageStream, toWhichBucketName, uploadAsFileName);
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
