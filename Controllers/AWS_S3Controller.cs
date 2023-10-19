using Microsoft.AspNetCore.Mvc;
using WebApplicationApi.Models;
using Amazon;
using Amazon.S3;
//using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using System.Collections.Specialized;
//using AutoCount.Data;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AWS_S3Controller : ControllerBase
    {
        // GET: api/<AWS_S3Controller>

        [HttpGet]
        [Route("GetProfilePicture")]
        public async Task<IActionResult> GetProfilePicture(string key, string fileName)
        {
            try
            {
                // You can generate the 'access key id' and 'secret key id, when you create the IAM user in AWS.
                // Region endpoint should be same as the bucket region

                NameValueCollection markStatus = new NameValueCollection();
                markStatus.Add("secureAppSettings", "secureAppSettings");
                var section = (markStatus as NameValueCollection);
                string accessKey = section["accessKey"], accessSecret = section["accessSecret"];

                using (var amazonS3client = new AmazonS3Client(accessKey, accessSecret, RegionEndpoint.APSoutheast1))
                {
                    var transferUtility = new TransferUtility(amazonS3client);

                    var response = await transferUtility.S3Client.GetObjectAsync(new GetObjectRequest()
                    {
                        // Bucket Name
                        BucketName = key,
                        // File Name
                        Key = fileName
                    });

                    // Return File not found if the file doesn't exist
                    if (response.ResponseStream == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        using (var responseStream = response.ResponseStream)
                        {
                            var stream = new MemoryStream();
                            await responseStream.CopyToAsync(stream);
                            stream.Position = 0;
                            //return stream;
                            return File(stream, response.Headers.ContentType, fileName);
                        }
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the AWS Credentials.");
                }
                else
                {
                    throw new Exception(amazonS3Exception.Message);
                }
            }
        }

        [HttpGet]
        [Route("testConnectio")]
        public ActionResult TEStConnectionb()
        {
            //AutoCount.Data.DBSetting dbSet = new AutoCount.Data.DBSetting(DBServerType.SQL2000, "175.136.249.41\\A2006", "AED_UNIMED_SB");
           // BCE.Data.DBSetting dbSet = BCE.AutoCount.MainEntry.Startup.Default.SubProjectStartupWithLogin("", "");
            //BCE.Data.DBSetting dbSet = new BCE.Data.DBSetting(BCE.Data.DBServerType.SQL2000, "175.136.249.41\\A2006", "AED_UNIMED_SB");
            //BCE.AutoCount.MainEntry.Startup.Default.SubProjectStartup(dbSet);
            //if (dbSet != null && BCE.AutoCount.Authentication.UserAuthentication.GetOrCreate(dbSet).Login("Administrator", "Twenty400!!"))
            //{
            //    //Start from this point
            //    // Application.Run(new FormImportGLOpening(dbSet));
            //    return Ok("success");
            //}
            return Ok("Fail");
        }

        /*public void NewAPInvoiceEntry(BCE.Data.DBSetting dbSetting)
        {
            string userID = BCE.AutoCount.Authentication.UserAuthentication.GetOrCreate(dbSetting).LoginUserID;
            BCE.AutoCount.ARAP.APInvoice.APInvoiceDataAccess cmd = BCE.AutoCount.ARAP.APInvoice.APInvoiceDataAccess.Create(dbSetting);
            BCE.AutoCount.ARAP.APInvoice.APInvoiceEntity doc = cmd.NewAPInvoice();
            BCE.AutoCount.ARAP.APInvoice.APInvoiceDTLEntity dtl = null;

            doc.DocNo = "<<New>>";
            doc.CreditorCode = "400-X001";
            doc.DocDate = new DateTime(2018, 5, 28);
            doc.Description = "Purchase Generated";
            doc.PurchaseAgent = "TOM";
            doc.JournalType = "PURCHASE";

            doc.RoundingMethod = BCE.AutoCount.Document.DocumentRoundingMethod.LineByLine_Ver2;
            //Document Level Inclusive Tax
            doc.InclusiveTax = true;

            //Add two lines of detail
            dtl = doc.NewDetail();
            dtl.AccNo = "700-1010";
            dtl.Description = "Raw Material Metal";
            dtl.ProjNo = DBNull.Value;
            dtl.Amount = 1000.00M;

            dtl = doc.NewDetail();
            dtl.AccNo = "700-1010";
            dtl.Description = "Process Cost";
            dtl.ProjNo = DBNull.Value;
            dtl.Amount = 150.00M;

            try
            {
                cmd.SaveAPInvoice(doc, userID);
                //log success
                //BCE.Application.AppMessage.ShowMessage(string.Format("{0} is created.", doc.DocNo));
            }
            catch (BCE.Application.AppException ex)
            {
                //log ex.Message
                //BCE.Application.AppMessage.ShowMessage(ex.Message);
            }
        }*/

        [HttpPost]
        [Route("GetBase64data")]
        public ActionResult GetBase64data(string Name, string FileName)
        {
            NameValueCollection markStatus = new NameValueCollection();
            markStatus.Add("secureAppSettings", "secureAppSettings");
            //var section = (WebConfigurationManager.GetSection("secureAppSettings") as NameValueCollection);
            var section = (markStatus as NameValueCollection);
            string accessKey = section["accessKey"], accessSecret = section["accessSecret"];
            AmazonS3Client client = new AmazonS3Client(accessKey, accessSecret, Amazon.RegionEndpoint.APSouth1);

            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = Name,
                    Key = FileName
                };

                GetObjectResponse response = new GetObjectResponse();

                using (var getObjectResponse = client.GetObjectAsync(request))
                {
                    using (var responseStream = getObjectResponse.Result)
                    {
                        using (StreamReader reader = new StreamReader(responseStream.ResponseStream))
                        {
                            string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                            string contentType = response.Headers["Content-Type"];
                            responseBody = reader.ReadToEnd(); // Now you process the response body.
                        }
                    }
                }
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            }

            return Ok(responseBody);
            //return Json(responseBody);
        }

        //[HttpPost]
        //[Route("FolderExistenceCheckAsync")]
        //public static async Task<bool> FolderExistenceCheckAsync(string FolderName)
        //{
        //    ListObjectsResponse response = null;
        //    try
        //    { 
        //        var awsKey = "AKIA5OS74MUCASG7HSCG";
        //        var awsSecretKey = "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";
        //        var bucketRegion = RegionEndpoint.APSouth1;
        //        bool folderchk = false;
        //        //AmazonS3Client client = new AmazonS3Client(awsKey, awsSecretKey, bucketRegion);
        //        string path = @"" + FolderName + "";

        //        using (AmazonS3Client client = new AmazonS3Client(awsKey, awsSecretKey, Amazon.RegionEndpoint.USEast1))
        //        {
        //            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(client, FolderName);
        //            ListObjectsRequest request = new ListObjectsRequest
        //            {
        //                BucketName = "<bucket-name>",
        //                Prefix = FolderName
        //            };


        //            //S3FileInfo s3FileInfo = new S3FileInfo();
        //            response = await client.ListObjectsAsync(request);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return (response != null && response.S3Objects != null && response.S3Objects.Count > 0);
        //}

        [HttpPost]
        [Route("UploadBade64dataAsync")]
        public ActionResult UploadBade64dataAsync([FromBody] AwsModel model)
        {
            var awsKey = "AKIA5OS74MUCASG7HSCG";
            var awsSecretKey = "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";
            var bucketRegion = RegionEndpoint.APSouth1;
            // Create a client
            AmazonS3Client client = new AmazonS3Client(awsKey, awsSecretKey, bucketRegion);

            var stream = new MemoryStream(Convert.FromBase64String(model.Value));

            // Create a PutObject request
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = "happic/FMCG/" + model.FolderName + "",
                Key = model.FileName,
                ContentType = "image/jpeg",
                InputStream = stream
            };

            //PutObjectResponse response = client.PutObject(request);
            //if (response.ETag != null)
            //{
            //    string etag = response.ETag;
            //    string versionID = response.VersionId;
            //    Response.BodyWriter = "<script>alert('File uploaded to S3 Bucket Successfully.');</script>";
            //}

            PutObjectResponse response = client.PutObjectAsync(request).Result;
            if (response.ETag != null)
            {
                string etag = response.ETag;
                string versionID = response.VersionId;
                
                //Response.BodyWriter = "<script>alert('File uploaded to S3 Bucket Successfully.');</script>";
            }

            //var response = client.PutObjectAsync(request);           
            //PutObjectResponse response = client.PutObjectAsync(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("FolderExistenceCheckAsync")]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public bool FolderExistenceCheckAsync(string FolderName)
        {
            var awsKey = "AKIA5OS74MUCASG7HSCG";
            var awsSecretKey = "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";
            var bucketRegion = RegionEndpoint.APSouth1;
            bool folderchk = false;
            AmazonS3Client client = new AmazonS3Client(awsKey, awsSecretKey, bucketRegion);
            string path = client +  "happic"  + @"" + FolderName + "";

            //string path = @"" + client + "happic" + FolderName + "";

            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                di.Create();
                folderchk = true;//di.CreateSubdirectory(FolderName);
            }
            return folderchk;
        }
    }
}