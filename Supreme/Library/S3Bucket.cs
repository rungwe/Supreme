using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace Supreme.Library
{
    public class S3Bucket
    {
        
        public static bool UploadFileToS3(string uploadAsFileName, Stream ImageStream, string toWhichBucketName)
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

        
        public static bool deleteFile(string keyName, string bucketName)
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