using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using WebApplicationApi.Models;

namespace WebApplicationApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class RIL : ControllerBase
    {
        public class EnvType
        {
            public const string Development = "d";
            public const string Quality = "q";
            public const string Production = "p";
        }

        public class Result
        {
            public string access_token { get; set; }
            public string scope { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
        }

        [EnableCors("AllowOrigin")]
        [HttpGet]
        [Route("syncCustomer")]
        public async Task<string> syncCustomer(string divCode, string custcode,string envirType)
        {
            
            DataSet dsAdmin = new DataSet();
            DataSet tc = new DataSet();
            string msg = "";
            try
            {
                DB_EReporting db_ER = new DB_EReporting("RAD");
                dsAdmin = getRetailerApprovalDetails(custcode);
                if (dsAdmin.Tables[0].Rows.Count > 0)
                {
                    DataRow CustRow = dsAdmin.Tables[0].Rows[0];

                    string ListedDrCode = replaceString(CustRow["ListedDrCode"].ToString() ?? "");
                    string Sf_Code = CustRow["Sf_Code"].ToString() ?? "";
                    string ListedDr_Name = replaceString(CustRow["ListedDr_Name"].ToString() ?? "");
                    string ListedDr_PinCode = replaceString(CustRow["ListedDr_PinCode"].ToString() ?? "");
                    string ListedDr_Mobile = replaceString(CustRow["ListedDr_Mobile"].ToString() ?? "");
                    string ListedDr_Email = replaceString(CustRow["ListedDr_Email"].ToString() ?? "");
                    string Territory_Code = replaceString(CustRow["Territory_Code"].ToString() ?? "");
                    string cityname = replaceString(CustRow["cityname"].ToString() ?? "");
                    string areaname = replaceString(CustRow["areaname"].ToString() ?? "");
                    string street = replaceString(CustRow["Street"].ToString() ?? "");
                    string Address1 = replaceString(CustRow["Address1"].ToString() ?? "");
                    string GST = replaceString(CustRow["GST"].ToString() ?? "");

                    string Add1 = replaceString(CustRow["Address1"].ToString() ?? "");
                    string Add2 = replaceString(CustRow["Address2"].ToString() ?? ".");
                    string Add3 = replaceString(CustRow["Address3"].ToString() ?? ".");
                    string Add4 = replaceString((string) CustRow["Address4"]) ?? Add2;
                    string Add5 = replaceString((string) CustRow["Address5"]) ?? Add3;

                    string contactperson = replaceString(CustRow["contactperson"].ToString() ?? "");
                    string ListedDr_Phone = replaceString(CustRow["ListedDr_Phone"].ToString() ?? "");
                    string contactperson2 = replaceString(CustRow["contactperson2"].ToString() ?? "");
                    string ListedDr_Phone2 = replaceString(CustRow["ListedDr_Phone2"].ToString() ?? "");
                    string contactperson3 = replaceString(CustRow["contactperson3"].ToString() ?? "");
                    string ListedDr_Phone3 = replaceString(CustRow["ListedDr_Phone3"].ToString() ?? "");

                    tc = getRetailerApprovalTerrDetails(ListedDrCode, Territory_Code);

                    string State_Name = replaceString(tc.Tables[0].Rows[0]["StateName"].ToString() ?? "");
                    string Country_name = replaceString(tc.Tables[0].Rows[0]["Country_name"].ToString() ?? "");
                    string Pan_No = replaceString(CustRow["PanNo"].ToString() ?? "");

                    string SStreet = replaceString(CustRow["SStreet"].ToString() ?? "");
                    string SCity = replaceString(CustRow["SCity"].ToString() ?? "");
                    string SState = replaceString(CustRow["SState"].ToString() ?? "");
                    string SPinCode = replaceString(CustRow["SPinCode"].ToString() ?? "");


                    //PAN DOCUMENT
                    string PanFileName = CustRow["PanCard"].ToString() ?? "";
                    string PanBase64 = "";
                    if ((PanFileName == null && PanFileName == ""))
                    { PanBase64 = ""; }
                    else
                    { PanBase64 = await RetriveBase64Format(PanFileName, divCode); }

                    //GST NUMBER  DOCUMENT
                    string GstFileName = CustRow["GSTNumberDocument"].ToString() ?? "";
                    string GstBase64 = "";
                    if ((GstFileName == null && GstFileName == ""))
                    { GstBase64 = ""; }
                    else
                    { GstBase64 = await RetriveBase64Format(GstFileName, divCode); }

                    //GST Declaration
                    string GstDFileName = CustRow["GSTDeclaration"].ToString() ?? "";
                    string GstDBase64 = "";
                    if ((GstDFileName == null && GstDFileName == ""))
                    { GstDBase64 = ""; }
                    else
                    { GstDBase64 = await RetriveBase64Format(GstDFileName, divCode); }

                    //V R Certificate
                    string DrugLicenseFN = CustRow["VRCertificate"].ToString() ?? "";
                    string DrugLicenseBase64 = "";
                    if ((DrugLicenseFN == null && DrugLicenseFN == ""))
                    { DrugLicenseBase64 = ""; }
                    else
                    { DrugLicenseBase64 = await RetriveBase64Format(DrugLicenseFN, divCode); }


                    //Address Proof
                    string AddressProofFn = CustRow["AddressProof"].ToString() ?? "";
                    string AddressProofBase64 = "";
                    if ((AddressProofFn == null && AddressProofFn == "")) 
                    { AddressProofBase64 = ""; }
                    else
                    { AddressProofBase64 = await RetriveBase64Format(AddressProofFn, divCode); }

                    string SPCODE = ""; string SHCODE = ""; string BPCODE = ""; string PYCODE = ""; string GCCODE = "";
                    string accessToken = ""; string scope = ""; string token_type = ""; string expires_in = "";

                    DataSet ds = new DataSet();
                    string sapqry = " SELECT * FROM listeddr_erps WHERE Listeddr_code='" + custcode + "'";
                    ds = db_ER.Exec_DataSet(sapqry);

                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    var request = new HttpRequestMessage();
                    // Production Authorization
                    if (envirType == EnvType.Production)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", String.Format("Basic d1JHNGphRldZOVhCOWZTMVBiR3FvTm8xSXVZYTpBUjRzZ0N5UEczQ2k1V3N2Y1lvY1BDcWZ3QVFh"));
                        //KEY
                        request = new HttpRequestMessage(HttpMethod.Post, "https://iamhc.ril.com/oauth2/token");
                    }
                    // Quality 
                    if (envirType == EnvType.Quality)
                    {
                        //Check Authorization
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", String.Format("Basic QjdKUjRtaVlJeUNxTzJ2WEJ5cVRfUENnU25FYTp1Mk84ZW9vWXdmNHRhWnpnWERabWJ2SVFBQ1lh"));

                        // Check KEY
                        request = new HttpRequestMessage(HttpMethod.Post, "https://iamhcqa.ril.com/oauth2/token");
                    }
                    // Development Authorization
                    if (envirType == EnvType.Development)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", String.Format("Basic bTdsTnlTYl9SQmFLcGJxeXVJN0RYZXNPX1lFYTpPUUZ2MXlicTZkWmdnenE5T1BQR1JxeWJodG9h"));
                        request = new HttpRequestMessage(HttpMethod.Post, "https://iamhcdev.ril.com/oauth2/token");
                    }
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

                    var content = new StringContent("{\"grant_type\":\"client_credentials\"}", Encoding.UTF8, "application/json");
                    request.Content = content;

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    Task<HttpResponseMessage> responseTask = client.SendAsync(request);
                    HttpResponseMessage response = responseTask.Result;
                    SPCODE = ""; SHCODE = ""; BPCODE = ""; PYCODE = ""; GCCODE = "";
                    string ReqType = "N";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        SPCODE = ds.Tables[0].Rows[0]["SP"].ToString();
                        SHCODE = ds.Tables[0].Rows[0]["SH"].ToString();
                        BPCODE = ds.Tables[0].Rows[0]["BP"].ToString();
                        PYCODE = ds.Tables[0].Rows[0]["PY"].ToString();
                        GCCODE = ds.Tables[0].Rows[0]["GC"].ToString();
                        ReqType = "M";
                    }
                    if ((PanBase64 == "" && AddressProofBase64 == "" && DrugLicenseBase64 == ""))
                    {
                        msg = "Documents Not Available"; Logger.WriteLogFile("RIL", msg);
                    }
                    else
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            Task<string> resultTask = response.Content.ReadAsStringAsync();
                            string result = resultTask.Result;

                            Result rs = new Result();
                            rs = JsonConvert.DeserializeObject<Result>(result);

                            accessToken = rs.access_token;
                            Logger.WriteLogFile("RIL","Access Token -" + accessToken);
                            scope = rs.scope;
                            token_type = rs.token_type;
                            expires_in = rs.expires_in;

                            var request2 = new HttpRequestMessage();
                            // Production SAP
                            if (envirType == EnvType.Production)
                            {
                                 request2 = new HttpRequestMessage(HttpMethod.Post, "https://apigwhc.ril.com/HC_SB_RFCAnyWhere/1.0.0/callRFC");
                            }
                            // Quality Check  SAP
                            if (envirType == EnvType.Quality)
                            {
                                 request2 = new HttpRequestMessage(HttpMethod.Post, "https://apigwhcqa.ril.com/commonsapfeature/1.0.0/RFCAnyWhere/callRFC");
                            }
                            // Development SAP
                            if (envirType == EnvType.Development)
                            {
                                 request2 = new HttpRequestMessage(HttpMethod.Post, "https://apigwhcdev.ril.com/commonsapfeature/1.0.0/RFCAnyWhere/callRFC");
                            }

                            request2.Headers.Add("Authorization", "Bearer " + accessToken + "");

                            string sCont = "{\"systemID\":\"CUS01\"," +
                                "\"payload\":{" +
                                "\"IM_SOURCE\":\"SANEFORCE\"," +
                                "\"IM_EXTREQUEST\":" + ListedDrCode + "," +
                                "\"IM_EXTCUSTID\":\"" + ListedDrCode + "\"," +
                                "\"IM_REQTYPE\": \"" + ReqType + "\"," +
                                "\"IM_STG_REQ\": {" +
                                "\"REQ_SRNO\": 0," +

                                "\"SP_REQTYPE\": \"" + ReqType + "\"," +
                                "\"SP_CODE\": \"" + SPCODE + "\"," +
                                "\"SP_NAME1\": \"" + ListedDr_Name + "\"," +
                                "\"SP_NAME2\": \"" + Add1 + "\"," +
                                "\"SP_NAME3\": \"" + Add2 + "\"," +
                                "\"SP_NAME4\": \"" + Add3 + "\"," +
                                "\"SP_STREET\": \"" + street + "\"," +
                                "\"SP_CITY\": \"" + cityname + "\"," +
                                "\"SP_PINCODE\": \"" + ListedDr_PinCode + "\"," +
                                "\"SP_STATE\": \"" + State_Name + "\"," +
                                "\"SP_COUNTRY\": \"" + Country_name + "\"," +
                                "\"SP_PAN\": \"" + Pan_No + "\"," +
                                "\"SP_MOBILE\": \"" + ListedDr_Mobile + "\"," +
                                "\"SP_EMAIL\": \"" + ListedDr_Email + "\"," +
                                "\"SP_CONTPERS_NAME\": \"" + contactperson + "\"," +
                                "\"SP_CONTPERS_MOBILE\": \"" + ListedDr_Mobile + "\"," +
                                "\"SP_CONTPERS_EMAIL\": \"" + ListedDr_Email + "\"," +

                                "\"SH_CODE\": \"" + SHCODE + "\"," +
                                "\"SH_NAME1\": \"" + ListedDr_Name + "\"," +
                                "\"SH_NAME2\": \"" + Add1 + "\"," +
                                "\"SH_NAME3\": \"" + Add4 + "\"," +
                                "\"SH_NAME4\": \"" + Add5 + "\"," +
                                "\"SH_STREET\": \"" + SStreet + "\"," +
                                "\"SH_CITY\": \"" + SCity + "\"," +
                                "\"SH_PINCODE\": \"" + SPinCode + "\"," +
                                "\"SH_STATE\": \"" + SState + "\"," +
                                "\"SH_COUNTRY\": \"" + Country_name + "\"," +
                                "\"SH_GST\": \"" + GST + "\"," +
                                "\"SH_MOBILE\": \"" + ListedDr_Mobile + "\"," +
                                "\"SH_EMAIL\": \"" + ListedDr_Email + "\"," +
                                "\"SH_CONTPERS_NAME\": \"" + contactperson + "\"," +
                                "\"SH_CONTPERS_MOBILE\": \"" + ListedDr_Mobile + "\"," +
                                "\"SH_CONTPERS_EMAIL\":\"" + ListedDr_Email + "\"," +

                                "\"BP_CODE\": \"" + BPCODE + "\"," +
                                "\"BP_NAME1\": \"" + ListedDr_Name + "\"," +
                                "\"BP_NAME2\":\"" + Add1 + "\"," +
                                "\"BP_NAME3\": \"" + Add2 + "\"," +
                                "\"BP_NAME4\": \"" + Add3 + "\"," +
                                "\"BP_STREET\": \"" + street + "\"," +
                                "\"BP_CITY\": \"" + cityname + "\"," +
                                "\"BP_PINCODE\": \"" + ListedDr_PinCode + "\"," +
                                "\"BP_STATE\":\"" + State_Name + "\"," +
                                "\"BP_COUNTRY\":\"" + Country_name + "\"," +
                                "\"BP_GST\":\"" + GST + "\"," +
                                "\"BP_MOBILE\":\"" + ListedDr_Mobile + "\"," +
                                "\"BP_EMAIL\": \"" + ListedDr_Email + "\"," +
                                "\"BP_CONTPERS_NAME\":\"" + contactperson + "\"," +
                                "\"BP_CONTPERS_MOBILE\":\"" + ListedDr_Mobile + "\"," +
                                "\"BP_CONTPERS_EMAIL\": \"" + ListedDr_Email + "\"," +

                                "\"PY_CODE\": \"" + PYCODE + "\"," +
                                "\"PY_NAME1\": \"" + ListedDr_Name + "\"," +
                                "\"PY_NAME2\": \"" + Add1 + "\"," +
                                "\"PY_NAME3\": \"" + Add2 + "\"," +
                                "\"PY_NAME4\":\"" + Add3 + "\"," +
                                "\"PY_STREET\":\"" + street + "\"," +
                                "\"PY_CITY\":\"" + cityname + "\"," +
                                "\"PY_PINCODE\":\"" + ListedDr_PinCode + "\"," +
                                "\"PY_STATE\":\"" + State_Name + "\"," +
                                "\"PY_COUNTRY\": \"" + Country_name + "\"," +
                                "\"PY_PAN\": \"" + Pan_No + "\"," +
                                "\"PY_GST\": \"" + GST + "\"," +
                                "\"PY_MOBILE\": \"" + ListedDr_Mobile + "\"," +
                                "\"PY_EMAIL\": \"" + ListedDr_Email + "\"," +
                                "\"PY_CONTPERS_NAME\": \"" + contactperson + "\"," +
                                "\"PY_CONTPERS_MOBILE\": \"" + ListedDr_Mobile + "\"," +
                                "\"PY_CONTPERS_EMAIL\": \"" + ListedDr_Email + "\"}," +

                                "\"IM_STG_KNBW\": []," +
                                "\"IM_STG_KNBK\": []," +

                                "\"IM_DMS\": [{" +
                                "\"KTOKD\":\"ZCSP\"," +
                                "\"DMS_SRNO\":\"1\"," +
                                "\"KUNNR\":\"" + PYCODE + "\"," +
                                "\"DOC_TYPE\":\"PAN\"," +
                                "\"FILE_NAME\":\"" + PanFileName + "\"," +
                                "\"CONTENT\":\"" + PanBase64 + "\"}," +

                                "{\"KTOKD\":\"ZCSP\"," +
                                "\"DMS_SRNO\":\"2\"," +
                                "\"KUNNR\":\"" + PYCODE + "\"," +
                                "\"DOC_TYPE\":\"CVF\"," +
                                "\"FILE_NAME\":\"" + AddressProofFn + "\"," +
                                "\"CONTENT\":\"" + AddressProofBase64 + "\"}," +

                                "{\"KTOKD\":\"ZCSP\",\"DMS_SRNO\":\"3\"," +
                                "\"KUNNR\":\"" + PYCODE + "\"," +
                                "\"DOC_TYPE\":\"CVF\"," +
                                "\"FILE_NAME\":\"" + DrugLicenseFN + "\"," +
                                "\"CONTENT\":\"" + DrugLicenseBase64 + "\"}";
                            int dRno = 4; 
                            if (GstBase64 != "")
                            {
                                sCont += ",{\"KTOKD\": \"ZCSP\"," +
                                          "\"DMS_SRNO\": \""+ dRno + "\"," +
                                          "\"KUNNR\":\"" + PYCODE + "\"," +
                                          "\"DOC_TYPE\":\"GST\"," +
                                          "\"FILE_NAME\":\"" + GstFileName + "\"," +
                                          "\"CONTENT\":\"" + GstBase64 + "\"}";
                                dRno++;
                            }
                            if (GstDBase64 != "")
                            {
                                sCont += ",{\"KTOKD\": \"ZCSP\"," +
                                          "\"DMS_SRNO\": \"" + dRno + "\"," +
                                          "\"KUNNR\":\"" + PYCODE + "\"," +
                                          "\"DOC_TYPE\":\"CVF\"," +
                                          "\"FILE_NAME\":\"" + GstDFileName + "\"," +
                                          "\"CONTENT\":\"" + GstDBase64 + "\"}";
                            }
                            sCont += "],\"IT_STG_REQ\": []," +
                                    "\"IT_STG_KNBW\": []," +
                                    "\"IT_STG_KNBK\": [] }}";

                            var content2 = new StringContent(sCont, Encoding.UTF8, "application/json");
                            request2.Content = content2;

                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                            Task<HttpResponseMessage> responseTask1 = client.SendAsync(request2);
                            HttpResponseMessage response2 = responseTask1.Result;

                            if (response2.IsSuccessStatusCode == true)
                            {
                                Task<string> resultTask1 = response2.Content.ReadAsStringAsync();
                                string result1 = resultTask1.Result;

                                dynamic rdynObj = JsonConvert.DeserializeObject(result1);
                                foreach (var li in rdynObj.responseData.ET_CUSTDETAIL)
                                {
                                    SPCODE = li.SP_CODE;
                                    SHCODE = li.SH_CODE;
                                    BPCODE = li.BP_CODE;
                                    PYCODE = li.PY_CODE;
                                    GCCODE = li.GC_CODE;

                                    string eqry = "EXEC SPCodeGenerationforRetailer  '" + ListedDrCode + "', '" + ListedDr_Name + "',";
                                    eqry += "'" + PYCODE + "','" + SPCODE + "','" + SHCODE + "','" + BPCODE + "','" + PYCODE + "','" + GCCODE + "'";
                                    int i = db_ER.ExecQry(eqry);
                                }

                                msg = "Retailer details sent to sap..!";
                            }
                            else
                            {
                                msg = $"{response2.StatusCode} -{response2.RequestMessage}";
                                Logger.WriteLogFile("RIL", msg);
                            }
                        }
                        else
                        {
                            msg = $"{response.StatusCode} -{response.RequestMessage}";
                            Logger.WriteLogFile("RIL", msg);
                        }
                    }
                }
                else { msg = $"{custcode} - Customer not available in the master"; Logger.WriteLogFile("RIL", msg); }
            }
            catch (Exception ex) {
                Logger.WriteLogFile("RIL Error:", ex.Message);
            }
            return msg;
        }
        public static string replaceString(string str) { 
            return str.Replace("\"","").Replace("\'", "");
        }
        public static DataSet getRetailerApprovalDetails(string custCode)
        {
            DataSet dsAdmin = new DataSet();

            DB_EReporting? db_ER = new DB_EReporting("RAD");
            try
            {
                string strQry = " EXEC GETRetailerDetailsForSAP '" + custCode + "'";
                dsAdmin = db_ER.Exec_DataSet(strQry);
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally { db_ER = null; }
            return dsAdmin;
        }
        public static DataSet getRetailerApprovalTerrDetails(string custCode, string Territory_Code)
        {
            DataSet dsAdmin = new DataSet();
            DB_EReporting? db_ER = new DB_EReporting("RAD");

            string strQry = " SELECT TOP 1 ms.ShortName StateName,StateName ShortName,";
            strQry += " mc.S_name Country_name  ";
            strQry += " FROM  Mas_ListedDr ld (NOLOCK)     ";
            strQry += " INNER JOIN  Mas_State ms  (NOLOCK) ON ld.State_Id = ms.State_Code  ";
            strQry += " INNER JOIN mas_Country mc (NOLOCK) ON ms.Country_code = mc.Country_code ";
            strQry += " WHERE ListedDrCode = "+ Convert.ToString(custCode) + "";

            try
            {
                dsAdmin = db_ER.Exec_DataSet(strQry);
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return dsAdmin;
        }

        private static async Task<string> RetriveBase64Format(string fileName, string div_code)
        {
            string accessKey = Secrets.accessKey;
            string accessSecret = Secrets.accessSecret;

            AmazonS3Client s3Client = new AmazonS3Client(accessKey, accessSecret, Amazon.RegionEndpoint.APSouth1); // Ensure you've configured your credentials (profile or env vars)
            /*try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = "happic",
                    Prefix = "rad/" + fileName, // Specify the folder name (including trailing slash if needed)
                    Delimiter = "/"
                };

                var response = await s3Client.ListObjectsV2Async(request);

                Console.WriteLine("Objects in bucket:");
                foreach (S3Object entry in response.S3Objects)
                {
                    Console.WriteLine($" - {entry.Key}");
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error encountered on server. Message:'{e.Message}'");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown error encountered. Message:'{e.Message}'");
            }*/
            try
            {


                DataSet dsDivision = getStatePerDivision(div_code);

                string Folder = Convert.ToString(dsDivision.Tables[0].Rows[0]["Url_Short_Name"]);
                Folder = Folder.ToString().ToLower() + "_" + "Retailer";
                string bucketName = "happic";
                string objectKey = Folder + "/" + fileName;
                // Get the object from S3
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };
                using (var response = await s3Client.GetObjectAsync(request))
                using (var memoryStream = new MemoryStream())
                {
                    // Copy the object content to a memory stream
                    await response.ResponseStream.CopyToAsync(memoryStream);

                    // Convert the image to a Base64 string
                    var base64String = Convert.ToBase64String(memoryStream.ToArray());

                    return base64String;
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error encountered on server. Message: '{e.Message}'");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown encountered on server. Message: '{e.Message}'");
                return null;
            }
        }
        public static DataSet getStatePerDivision(string div_code)
        {
            DataSet dsAdmin = new DataSet();
            DB_EReporting? db_ER = new DB_EReporting("RAD");
            string strQry = "SELECT State_Code,Division_Name,Division_SName,Url_Short_Name  FROM Mas_Division ";
            strQry += " Where Division_Code = " + div_code + "  GROUP BY State_Code,Division_Name,Division_SName,Url_Short_Name ";

            try
            {
                dsAdmin = db_ER.Exec_DataSet(strQry);
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return dsAdmin;
        }
        
    }
}
