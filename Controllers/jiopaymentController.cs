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
            // List<Paymentintegrate> TOHDetails = new List<Paymentintegrate>();
            Paymentintegrate od = new Paymentintegrate();

            //od = new Paymentintegrate();
            var Invoice_ID = _Paymentintegrate.Invoice_ID;
            var Order_Value = _Paymentintegrate.Order_Value;
            var Mobile_No = _Paymentintegrate.Mobile_No;
            var Cus_Name = _Paymentintegrate.Cus_Name;
            var mail_Id = _Paymentintegrate.mail_Id;
            var Describ = _Paymentintegrate.Describ;




            // TOHDetails.Add(od);

            var requestedUrl = "https://pp-apig.jiomoney.com/jfs/v1/app/authenticate";

            var json = @"{
                ""application"": {
                    ""clientId"": ""93b0534f5143d7c7d22c5c85a7ac5e6c""
                },
                ""authenticateList"": [
                    {
                        ""mode"": 22,
                        ""value"": ""018ca62c40cba5a5cb0e557221631fd1ddcf5570539ac825551d3435e322af41""
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
                var payload = new
                {
                    merchantId = "100001000294075",
                    merchantLinkId = $"MER{DateTime.Now:yyyyMMddHHmmss}",
                    amount = Order_Value,
                    description = Describ,
                    pushSmsTo = Mobile_No,
                    pushEmailTo = mail_Id,
                    invoice = Invoice_ID,
                    customerName = Cus_Name,
                    merchantReturnUrl = "https://www.merchant-website.com/homepage",
                    merchantCallBackUrl = "https://www.merchant-website.com/callback",
                    properties = new
                    {
                        udf1 = "A1",
                        udf2 = "A2",
                        udf3 = "A3",
                        udf4 = "A4",
                        udf5 = "A5"
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
                                }
                            }
                        }
                    }
                };

                // Serialize the payload to JSON
                var jsonPayload = JsonConvert.SerializeObject(payload);

                var requestUrl = "https://pp-apig.jiomoney.com/payments/jfs/cl/get_short_link";

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
            Jiopaymentstatus od = new Jiopaymentstatus();
            var merchant_Id = _Jiopaymentstatus.merchantId;
            var merchantLink_Id = _Jiopaymentstatus.merchantLinkId;
            var jioLink_Id = _Jiopaymentstatus.jioLinkId;
            var requestedUrl = "https://pp-apig.jiomoney.com/jfs/v1/app/authenticate";
            var json = @"{
                ""application"": {
                    ""clientId"": ""93b0534f5143d7c7d22c5c85a7ac5e6c""
                },
                ""authenticateList"": [
                    {
                        ""mode"": 22,
                        ""value"": ""018ca62c40cba5a5cb0e557221631fd1ddcf5570539ac825551d3435e322af41""
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
                var requestUrl = " https://pp-apig.jiomoney.com/payments/jfs/cl/check_collect_link_status ";
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
                return respon;
            }
            else
            {
                return null;
            }
        }
    }
}
