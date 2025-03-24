using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using WebApplicationApi.Models;
using Newtonsoft.Json;
using System.Text.Json;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Net;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class jiopaymentController : Controller
    {
        private readonly HttpClient _httpClient;


        public object JSON { get; private set; }

        public jiopaymentController()
        {

            _httpClient = new HttpClient();
        }
        [HttpPost]
        [Route("jiopaymentint")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<string?> GetpaymentgateAsync([FromBody] Paymentintegrate _Paymentintegrate)
        {
            var senderID = "RAD";
            DB_EReporting dbEr = new DB_EReporting(senderID);
           
            Paymentintegrate od = new Paymentintegrate();

            //od = new Paymentintegrate();
            var Invoice_ID = _Paymentintegrate.Invoice_ID;
            var Order_Value = _Paymentintegrate.Order_Value;
            var Mobile_No = _Paymentintegrate.Mobile_No;
            var Cus_Name = _Paymentintegrate.Cus_Name;
            var mail_Id = _Paymentintegrate.mail_Id;
            var Describ = _Paymentintegrate.Describ;
            string link_id = "";
            string inputString = Invoice_ID.Replace("-", "");
            string RetName = ""; string Address = "";
            if (inputString.Length < 20)
            {
                link_id = inputString.PadLeft(20, '0');
            }
            else
            {
                link_id = inputString.Substring(inputString.Length - 20);
            }
            DataTable rdt = new DataTable();
            rdt = dbEr.Exec_DataTable("Select dr.ListedDr_Name,er.Code,er.PY,dr.ListedDr_Mobile,dr.ListedDr_Address1 from Mas_ListedDr dr inner join listeddr_erps er on dr.ListedDrCode=er.Listeddr_code\r\n Inner join Trans_Order_Head ih on ih.Cust_Code=dr.ListedDrCode\r\n Where ih.Trans_Sl_No='" + Invoice_ID + "' ");
            string RetNames = rdt.Rows[0]["ListedDr_Name"].ToString();
            if (RetNames.Length > 29)
            {
                RetName = RetNames.Substring(0, 29);
            }
            else
            {
                RetName = RetNames;
            }

            string Code = rdt.Rows[0]["PY"].ToString();
            string Addresss = rdt.Rows[0]["ListedDr_Address1"].ToString();
            if (Addresss.Length > 29)
            {
                Address = Addresss.Substring(0, 29);
            }
            else
            {
                Address = Addresss;
            }


            // TOHDetails.Add(od);

            var requestedUrl = "https://apig.jiomoney.com/jfs/v1/app/authenticate";

            var json = @"{
                ""application"": {                    
                 ""clientId"": ""c99ec2e37b603daabd072701121831a3""
                },
                ""authenticateList"": [
                    {
                        ""mode"": 22,                       
                        ""value"": ""2fc2d0070471ca3aa12a29f9cb73b68422bd0d3ae2ae06a4f5524c51a646312c""
                    }
                ],
                ""scope"": ""SESSION"",
                ""purpose"": 2
            }";

            _httpClient.DefaultRequestHeaders.Add("x-trace-id", "01c570cf-2bdf-49d0-a126-baec7038bbd1");

            //_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            var requestedData = new StringContent(json, Encoding.UTF8, "application/json");

            var responses = await _httpClient.PostAsync(requestedUrl, requestedData);

            if (responses.IsSuccessStatusCode)
            {
                var responsesContent = await responses.Content.ReadAsStringAsync();

                var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responsesContent);



                if (jsonResponse.TryGetProperty("session", out JsonElement sessionElement))
                {
                    if (sessionElement.TryGetProperty("accessToken", out JsonElement accessTokenElement))
                    {
                        if (accessTokenElement.TryGetProperty("tokenValue", out JsonElement tokenValueElement))
                        {
                            var tokenValue = tokenValueElement.GetString();
                            _httpClient.DefaultRequestHeaders.Add("x-app-access-token", tokenValue);

                        }
                    }

                    if (sessionElement.TryGetProperty("appIdentifierToken", out JsonElement appIdentifierTokenElement))
                    {
                        var appIdentifierToken = appIdentifierTokenElement.GetString();
                        _httpClient.DefaultRequestHeaders.Add("x-appid-token", appIdentifierToken);

                    }
                }
                DateTime dt = DateTime.Now.AddDays(1);
                string strDate = dt.Year + "" + ((dt.Month < 10) ? "0" : "") + dt.Month + "" + ((dt.Day < 10) ? "0" : "") + dt.Day + "" + ((dt.Hour < 10) ? "0" : "") + dt.Hour + "" + ((dt.Minute < 10) ? "0" : "") + dt.Minute + "" + ((dt.Second < 10) ? "0" : "") + dt.Second;

                var payload = new
                {
                    merchantId = "100001006699559",
                    merchantLinkId = link_id,
                    amount = Order_Value,
                    description = Describ,
                    pushSmsTo = Mobile_No,
                    pushEmailTo = mail_Id,
                    invoice = Invoice_ID,
                    customerName = Cus_Name,
                    merchantReturnUrl = "",
                    merchantCallBackUrl = "",
                    linkExpiryAt = strDate,
                    //properties = new
                    //{
                    //    udf1 = Code,
                    //    udf2 = RetName,
                    //    udf3 = Address,
                    //    udf4 = Mobile_No,
                    //    udf5 = "A5"
                    //},
                    udf = new Dictionary<string, object>
                     {
                        { "01", Code },
                        { "02", RetName },
                        { "03", Address },
                        { "04", Mobile_No },
                        { "05", "A5" }
                    },
                    metadata = new
                    {
                        product = new[]
                        {
                            new
                            {
                                productId = "4911899271",
                                validationKey = "Serial Number",
                                validationValue = "AEDSHJ07HG779",
                                transactionAmount = "5000"
                            }
                        },
                        checkout = new
                        {
                            allowed = new[]
                            {
                                new
                                {
                                    methodType = "110",
                                    methodSubType = "582"
                                },
                                new
                                {
                                    methodType = "110",
                                    methodSubType = "566"
                                },
                                new
                                {
                                    methodType = "110",
                                    methodSubType = "581"
                                },
                                new
                                {
                                    methodType = "110",
                                    methodSubType = "579"
                                }
                            }
                        }
                    }
                };

                // Serialize the payload to JSON
                var jsonPayload = JsonConvert.SerializeObject(payload);

                var requestUrl = "https://apig.jiomoney.com/payments/jfs/cl/get_short_link";

                _httpClient.DefaultRequestHeaders.Add("x-trace-id", "01c570cf-2bdf-49d0-a126-baec7038bbd1");


                var reques = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var respo = await _httpClient.PostAsync(requestUrl, reques);

                var respon = await respo.Content.ReadAsStringAsync();

                //Paymentintegrate _response = new Paymentintegrate
                //{
                //    Payment_Link = respon,
                //    Status = "Suceess"

                //};
                var Status = "Suceess";
                return respon;
            }
            else
            {
                return null;
            }

        }
        [HttpPost]
        [Route("jiopaymentstatus")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<string?> GetpaymentstatusAsync([FromBody] Jiopaymentstatus _Jiopaymentstatus)
        {
            var senderID = "RAD";
            DB_EReporting dbEr = new DB_EReporting(senderID);
            Jiopaymentstatus od = new Jiopaymentstatus();
            var merchant_Id = _Jiopaymentstatus.merchantId;
            var merchantLink_Id = _Jiopaymentstatus.merchantLinkId;
            var jioLink_Id = _Jiopaymentstatus.jioLinkId;
            var requestedUrl = "https://apig.jiomoney.com/jfs/v1/app/authenticate";
            var json = @"{
                ""application"": {                    
                    ""clientId"": ""c99ec2e37b603daabd072701121831a3""
                },
                ""authenticateList"": [
                    {
                        ""mode"": 22,                      
                        ""value"": ""2fc2d0070471ca3aa12a29f9cb73b68422bd0d3ae2ae06a4f5524c51a646312c""
                    }
                ],
                ""scope"": ""SESSION"",
                ""purpose"": 2
            }";
            _httpClient.DefaultRequestHeaders.Add("x-trace-id", "01c570cf-2bdf-49d0-a126-baec7038bbd1");
            //_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var requestedData = new StringContent(json, Encoding.UTF8, "application/json");
            var responses = await _httpClient.PostAsync(requestedUrl, requestedData);
            if (responses.IsSuccessStatusCode)
            {
                var responsesContent = await responses.Content.ReadAsStringAsync();
                var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responsesContent);
                if (jsonResponse.TryGetProperty("session", out JsonElement sessionElement))
                {
                    if (sessionElement.TryGetProperty("accessToken", out JsonElement accessTokenElement))
                    {
                        if (accessTokenElement.TryGetProperty("tokenValue", out JsonElement tokenValueElement))
                        {
                            var tokenValue = tokenValueElement.GetString();
                            _httpClient.DefaultRequestHeaders.Add("x-app-access-token", tokenValue);

                        }
                    }
                    if (sessionElement.TryGetProperty("appIdentifierToken", out JsonElement appIdentifierTokenElement))
                    {
                        var appIdentifierToken = appIdentifierTokenElement.GetString();
                        _httpClient.DefaultRequestHeaders.Add("x-appid-token", appIdentifierToken);

                    }
                }
                
                var requestUrl = " https://apig.jiomoney.com/payments/jfs/cl/check_collect_link_status ";
                _httpClient.DefaultRequestHeaders.Add("x-trace-id", "01c570cf-2bdf-49d0-a126-baec7038bbd1");
                var linkstst = new
                {
                    merchantId = merchant_Id,
                    merchantLinkId = merchantLink_Id,
                    jioLinkId = jioLink_Id
                };
                var jsonlinkstst = JsonConvert.SerializeObject(linkstst);
                var reques = new StringContent(jsonlinkstst, Encoding.UTF8, "application/json");
                var respo = await _httpClient.PostAsync(requestUrl, reques);

                var respon = await respo.Content.ReadAsStringAsync();
                var respond = JsonConvert.DeserializeObject<JObject>(respon);
                var status = respond["linkStatus"]?.ToString();
                var link_id = respond["merchantLinkId"]?.ToString();
                var invoice = respond["invoice"]?.ToString();
                if (status == "PAID")
                {
                    DataTable rdt = new DataTable();
                    rdt = dbEr.Exec_DataTable("Update Trans_Payment_Link_Details set LinkStatus='PAID' where MerchantLinkId='" + link_id + "'  " +
                                                 "Update Trans_Order_Head set isPaid='PAID' where Trans_Sl_No='" + invoice + "'");

                }
                return respon;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [Route("CheckRADSMSpayments")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<string?> Getpaymentstatus()
        {
            var senderID = "RAD";
            var respon = "";
            DB_EReporting dbEr = new DB_EReporting(senderID);
            DataTable rdt = new DataTable();
            rdt = dbEr.Exec_DataTable("select JioLinkId,MerchantId,MerchantLinkId from Trans_Payment_Link_Details where LinkStatus = 'ISSUED'  and DATEDIFF(hour,LinkCreatedDate,getdate())<25");
            //rdt = dbEr.Exec_DataTable("select JioLinkId,MerchantId,MerchantLinkId from Trans_Payment_Link_Details where MerchantLinkId='000RADMGR00062425SO5'");
            //rdt = dbEr.Exec_DataTable("select JioLinkId,MerchantId,MerchantLinkId from Trans_Payment_Link_Details where LinkStatus = 'PAID' ");
            foreach (DataRow dtRow in rdt.Rows)
            {
                string merchant_Id = Convert.ToString(dtRow["MerchantId"]);
                string merchantLink_Id = Convert.ToString(dtRow["MerchantLinkId"]);
                string jioLink_Id = Convert.ToString(dtRow["JioLinkId"]);

                Jiopaymentstatus od = new Jiopaymentstatus();
                _httpClient.DefaultRequestHeaders.Clear();
                var requestedUrl = "https://apig.jiomoney.com/jfs/v1/app/authenticate";
                var json = @"{
                    ""application"": {                    
                        ""clientId"": ""c99ec2e37b603daabd072701121831a3""
                    },
                    ""authenticateList"": [
                        {
                            ""mode"": 22,                      
                            ""value"": ""2fc2d0070471ca3aa12a29f9cb73b68422bd0d3ae2ae06a4f5524c51a646312c""
                        }
                    ],
                    ""scope"": ""SESSION"",
                    ""purpose"": 2
                }";
                _httpClient.DefaultRequestHeaders.Add("x-trace-id", "01c570cf-2bdf-49d0-a126-baec7038bbd1");
                //_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var requestedData = new StringContent(json, Encoding.UTF8, "application/json");
                var responses = await _httpClient.PostAsync(requestedUrl, requestedData);
                if (responses.IsSuccessStatusCode)
                {
                    var responsesContent = await responses.Content.ReadAsStringAsync();
                    var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responsesContent);
                    if (jsonResponse.TryGetProperty("session", out JsonElement sessionElement))
                    {
                        if (sessionElement.TryGetProperty("accessToken", out JsonElement accessTokenElement))
                        {
                            if (accessTokenElement.TryGetProperty("tokenValue", out JsonElement tokenValueElement))
                            {
                                var tokenValue = tokenValueElement.GetString();
                                _httpClient.DefaultRequestHeaders.Add("x-app-access-token", tokenValue);

                            }
                        }
                        if (sessionElement.TryGetProperty("appIdentifierToken", out JsonElement appIdentifierTokenElement))
                        {
                            var appIdentifierToken = appIdentifierTokenElement.GetString();
                            _httpClient.DefaultRequestHeaders.Add("x-appid-token", appIdentifierToken);

                        }
                    }

                    var requestUrl = " https://apig.jiomoney.com/payments/jfs/cl/check_collect_link_status ";
                    _httpClient.DefaultRequestHeaders.Add("x-trace-id", "01c570cf-2bdf-49d0-a126-baec7038bbd1");
                    var linkstst = new
                    {
                        merchantId = merchant_Id,
                        merchantLinkId = merchantLink_Id,
                        jioLinkId = jioLink_Id
                    };
                    var jsonlinkstst = JsonConvert.SerializeObject(linkstst);
                    var reques = new StringContent(jsonlinkstst, Encoding.UTF8, "application/json");
                    var respo = await _httpClient.PostAsync(requestUrl, reques);

                    respon = await respo.Content.ReadAsStringAsync();
                    try {
                    var respond = JsonConvert.DeserializeObject<JObject>(respon);
                    var status = respond["linkStatus"]?.ToString();
                    var link_id = respond["merchantLinkId"]?.ToString();
                    var invoice = respond["invoice"]?.ToString();
                    if (status == "PAID")
                    {
                        dbEr.ExecQry("Update Trans_Payment_Link_Details set LinkStatus='PAID' where MerchantLinkId='" + link_id + "'  " +
                                                     "Update Trans_Order_Head set isPaid='PAID' where Trans_Sl_No='" + invoice + "'");

                            string apiUrl = "https://api.salesjump.in/api/Token/Getorder_creation?Order_id=" + invoice + "";
                            //string apiUrl = "http://localhost:5053/api/Token/Getorder_creation?Order_id=" + invoice + "";


                            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            return respon;
        }


        [HttpGet]
        [Route("SyncSalesOrder")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<string?> SyncSalesOrder(string OrderNoPayLinkNo)
        {
            var senderID = "RAD";
            var respon = "";
            DB_EReporting dbEr = new DB_EReporting(senderID);
            DataTable rdt = new DataTable();
            rdt = dbEr.Exec_DataTable("select JioLinkId,MerchantId,MerchantLinkId from Trans_Payment_Link_Details where MerchantLinkId='"+OrderNoPayLinkNo+"'");
            foreach (DataRow dtRow in rdt.Rows)
            {
                string merchant_Id = Convert.ToString(dtRow["MerchantId"]);
                string merchantLink_Id = Convert.ToString(dtRow["MerchantLinkId"]);
                string jioLink_Id = Convert.ToString(dtRow["JioLinkId"]);

                Jiopaymentstatus od = new Jiopaymentstatus();
                _httpClient.DefaultRequestHeaders.Clear();
                var requestedUrl = "https://apig.jiomoney.com/jfs/v1/app/authenticate";
                var json = @"{
                    ""application"": {                    
                        ""clientId"": ""c99ec2e37b603daabd072701121831a3""
                    },
                    ""authenticateList"": [
                        {
                            ""mode"": 22,                      
                            ""value"": ""2fc2d0070471ca3aa12a29f9cb73b68422bd0d3ae2ae06a4f5524c51a646312c""
                        }
                    ],
                    ""scope"": ""SESSION"",
                    ""purpose"": 2
                }";
                _httpClient.DefaultRequestHeaders.Add("x-trace-id", "01c570cf-2bdf-49d0-a126-baec7038bbd1");
               
                var requestedData = new StringContent(json, Encoding.UTF8, "application/json");
                var responses = await _httpClient.PostAsync(requestedUrl, requestedData);
                if (responses.IsSuccessStatusCode)
                {
                    var responsesContent = await responses.Content.ReadAsStringAsync();
                    var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responsesContent);
                    if (jsonResponse.TryGetProperty("session", out JsonElement sessionElement))
                    {
                        if (sessionElement.TryGetProperty("accessToken", out JsonElement accessTokenElement))
                        {
                            if (accessTokenElement.TryGetProperty("tokenValue", out JsonElement tokenValueElement))
                            {
                                var tokenValue = tokenValueElement.GetString();
                                _httpClient.DefaultRequestHeaders.Add("x-app-access-token", tokenValue);

                            }
                        }
                        if (sessionElement.TryGetProperty("appIdentifierToken", out JsonElement appIdentifierTokenElement))
                        {
                            var appIdentifierToken = appIdentifierTokenElement.GetString();
                            _httpClient.DefaultRequestHeaders.Add("x-appid-token", appIdentifierToken);

                        }
                    }

                    var requestUrl = " https://apig.jiomoney.com/payments/jfs/cl/check_collect_link_status ";
                    _httpClient.DefaultRequestHeaders.Add("x-trace-id", "01c570cf-2bdf-49d0-a126-baec7038bbd1");
                    var linkstst = new
                    {
                        merchantId = merchant_Id,
                        merchantLinkId = merchantLink_Id,
                        jioLinkId = jioLink_Id
                    };
                    var jsonlinkstst = JsonConvert.SerializeObject(linkstst);
                    var reques = new StringContent(jsonlinkstst, Encoding.UTF8, "application/json");
                    var respo = await _httpClient.PostAsync(requestUrl, reques);

                    respon = await respo.Content.ReadAsStringAsync();
                    try
                    {
                        var respond = JsonConvert.DeserializeObject<JObject>(respon);
                        var status = respond["linkStatus"]?.ToString();
                        var link_id = respond["merchantLinkId"]?.ToString();
                        var invoice = respond["invoice"]?.ToString();
                        if (status == "PAID")
                        {
                            dbEr.ExecQry("Update Trans_Payment_Link_Details set LinkStatus='PAID' where MerchantLinkId='" + link_id + "'  " +
                                                         "Update Trans_Order_Head set isPaid='PAID' where Trans_Sl_No='" + invoice + "'");

                            string apiUrl = "https://api.salesjump.in/api/Token/Getorder_creation?Order_id=" + invoice + "";
                            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            return respon;
        }


        [HttpPost]
        [Route("RADSMSgateway")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<string?> RadSMSgateway([FromBody] smsintegrate _smsintegrate)
        {
            var Templateid = _smsintegrate.Templateid;
            var MobileNo = _smsintegrate.MobileNo;
            var Message = _smsintegrate.Message;
            var TransactionID = _smsintegrate.TransactionID;
            var payload = new
            {
                username = "jiocx-relbio",
                password = "PIcBApQDmMq3E9K4",
                APIKey = "41195",
                secretKey = "41195",
                applicationId = 41195,
                Templateid = Templateid,
                MobileNo = MobileNo,
                Message = Message,
                TransactionID = TransactionID,
                CLIName = "RLSVET",
                sms_type = "T",
                sms_language = "ENG",
                dlt_entity_id = "1201158529429286471"
            };
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var requestUrl = "https://relbio.jiocx.com/apggw/relbio/sms/v1/send";
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var reques = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var respo = await _httpClient.PostAsync(requestUrl, reques);

            var respon = await respo.Content.ReadAsStringAsync();
            return respon;

        }
    }
}
