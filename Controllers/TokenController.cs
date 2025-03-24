using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using WebApplicationApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Runtime.Intrinsics.Arm;
using System;
using System.Web.Http.Cors;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        private readonly HttpClient _httpClient;

        public object JSON { get; private set; }

        public TokenController()
        {

            _httpClient = new HttpClient();
        }

        [HttpPost]
        [Route("GetSapinterdiscount")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<string> GenerateTokenAsync(string IM_DATE)
        {
            try
            {
                var senderID = "RAD";
                DB_EReporting dbEr = new DB_EReporting(senderID);
                var requestUrl = "https://iamhc.ril.com/oauth2/token";


                var requestData = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");


                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "UjhneENlc2J2QVNleU5ScGhvR19DRmVoa1BFYTp0RFp1VEJleVZqeG9nY0U4SGRBb3h1TmV5Zm9h");


                var response = await _httpClient.PostAsync(requestUrl, requestData);


                //if (response.IsSuccessStatusCode)
                //{
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseObject = JsonConvert.DeserializeObject<JObject>(responseContent);

                var accessToken = responseObject["access_token"]?.ToString();
                //var accessToken = "868334cf-824c-3ea7-ae57-bf2dd1183c5c";

                var requestedUrl = "https://apigwhc.ril.com/CU_NJ_NodeRFC_RNM/1.0/callSAPCommon";


                var json = $@"{{
                    ""functionModule"": ""Z_PTC_SHOWDISCOUNTSCHEMES"",
                    ""im_sap_source"": ""P25CLNT400"",
                    ""data"": {{
                        ""TRACK"": ""PTC"",
                        ""IM_DATE"": ""{IM_DATE}""
                    }}
                }}";



                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                var requestedData = new StringContent(json, Encoding.UTF8, "application/json");

                var responses = await _httpClient.PostAsync(requestedUrl, requestedData);




                if (responses.IsSuccessStatusCode)
                {
                    var responsesContent = await responses.Content.ReadAsStringAsync();
                    var responseOb = JsonConvert.DeserializeObject<JObject>(responsesContent);
                    // var jsonResponses = JSON.stringify(responsesContent);
                    string jsonResponse = responseOb.ToString();


                    var responseobj = JObject.Parse(jsonResponse);
                    var responseobjec = JObject.Parse(jsonResponse);

                    var etDetails = responseobj["result"]["EX_PAYLOAD_JSON"]["ET_DETAILS"].ToList();

                    var datesde = responseobjec["result"]["IM_PAYLOAD_INP"].ToString();

                    var datesObject = System.Text.Json.JsonSerializer.Deserialize<YourModel>(datesde);

                    var seldate = datesObject.IM_DATE.ToString();

                    if (etDetails.ToList().Count > 0)
                    {
                        string Category = "";
                        string sxml = "<ROOT>";
                        for (int j = 0; j < etDetails.Count; j++)
                        {

                            string materialcode = etDetails[j]["MATERIAL"].ToString();
                            string material_Name = etDetails[j]["MATERIAL_DESC"].ToString();
                            string MinQty = etDetails[j]["MINIMUM_QUANTITY"].ToString();
                            string Free_Qty = etDetails[j]["FREE_GOODS_QUANTIY"].ToString();
                            string Add_Free_Qty = etDetails[j]["ADDITIONAL_QTY_FOR_FREE_GOODS"].ToString();
                            string Categorys = etDetails[j]["FREE_GOODS_CATEGORY"].ToString();
                            string Datest = seldate;
                            if (Categorys == "Exclusive")
                            { Category = "Y"; }
                            else { Category = "N"; }

                            sxml += "<Prod materialcode=\"" + materialcode + "\"  MinQty=\"" + MinQty + "\"  Free_Qty=\"" + Free_Qty + "\" Add_Free_Qty=\"" + Add_Free_Qty + "\" Category=\"" + Category + "\" Datest=\"" + Datest + "\"   />";

                        }
                        sxml += "</ROOT>";
                        string strQry = "exec  Sp_insertsap_scheme '" + sxml + "'";
                        int result = 0;

                        result = dbEr.ExecQry(strQry);
                    }


                    return responsesContent;
                }
                else
                {
                    return null;
                }

                //}
                //else
                //{

                //    return null;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        [Route("GetSapprice")]
        public async Task<string> GenerateSapprice()
        {
            try
            {
                var senderID = "RAD";
                DB_EReporting dbEr = new DB_EReporting(senderID);

                var requestUrl = "https://iamhc.ril.com/oauth2/token";


                var requestData = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");


                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "UjhneENlc2J2QVNleU5ScGhvR19DRmVoa1BFYTp0RFp1VEJleVZqeG9nY0U4SGRBb3h1TmV5Zm9h");


                var response = await _httpClient.PostAsync(requestUrl, requestData);


                //if (response.IsSuccessStatusCode)
                //{
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseObject = JsonConvert.DeserializeObject<JObject>(responseContent);

                var accessToken = responseObject["access_token"]?.ToString();

                DataTable rdt = new DataTable();
                rdt = dbEr.Exec_DataTable("Select Sale_Erp_Code MATERIAL ,1.000 QUANTITY from Mas_Product_Detail where Product_Active_Flag=0 ");

                List<Productmaster> rdS = new List<Productmaster>();
                if (rdt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in rdt.Rows)
                    {
                        rdS.Add(new Productmaster
                        {
                            MATERIAL = Convert.ToString(dtRow["MATERIAL"]),
                            QUANTITY = Convert.ToString(dtRow["QUANTITY"])
                        });
                    }
                }
                var array = rdS.ToList();

                var requestedUrl = "https://apigwhc.ril.com/CU_NJ_NodeRFC_RNM/1.0/callSAPCommon";
                var data = new
                {
                    functionModule = "Z_PTC_PRICE_SIMULATION",
                    im_sap_source = "P25CLNT400",
                    data = new
                    {
                        TRACK = "PTC",
                        IM_INPUT = array,
                        IM_SOLDTO_PARTY = "0020111029",
                        IM_BILLTO_PARTY = "0040116064",
                        IM_SHIPTO_PARTY = "0030155167",
                        IM_PAYER = "0005003024"
                    }
                };
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                _httpClient.DefaultRequestHeaders.Add("Cookie", "MYSSO=f0e6f443e0631f535487508b7241ac3d397ab7530973ac9c81cb11710b3f224cd029522806692c366a90262ce60ae7c7dd4b53baa4c28926a6cad41d0c5a285e0e7db84890b3226965274ac196529b1da329c11b4288b71622317a51e97b245f722c118dafb751be1fb44067911c55d9fc39d46dcf199b309c6b88ac4ed55eee7849a56c109b06ba2ff622b01edb6af25f3ae4878cf7de973cd8ee16ded32691d75149fbbf78db95556341ccf08cf33f623c3596bef8e9b61bf60ce6ddfa59fe35e9196d2bccbe35c08374cf695813e1daf12c36169a2ea59ce8e9c37f2597e7e2cffb9f9dec52fc9fbf08e70a1cfa86c93613a4f6f20baa2e6f7ded9574eb26826a43bc952329958f8d2c6926830e4f0a3d771b3df45c5ebda7f2e8221bdc97c4752a445c64611672d68c71c96f715041017f96d4b884f598fd2285cafb624e336831aa056f891b39074ae4fc669da3; USER=%7B%22domain_id%22%3A%22rahul.kathuria%22%2C%22email%22%3A%22rahul.kathuria%40ril.com%22%2C%22fullname%22%3A%22Rahul%20Kathuria%22%2C%22env%22%3A%22dev%22%2C%22internal%22%3Atrue%7D");

                var requestedData = new StringContent(json, Encoding.UTF8, "application/json");

                var responses = await _httpClient.PostAsync(requestedUrl, requestedData);

                if (responses.IsSuccessStatusCode)
                {
                    var responsesContent = await responses.Content.ReadAsStringAsync();
                    var responseOb = JsonConvert.DeserializeObject<JObject>(responsesContent);
                    // var jsonResponses = JSON.stringify(responsesContent);
                    string jsonResponse = responseOb.ToString(); // Replace with your actual JSON response


                    var responseobj = JObject.Parse(jsonResponse);
                    var responseobjec = JObject.Parse(jsonResponse);

                    var etDetails = responseobj["result"]["EX_PAYLOAD_JSON"]["EX_OUTPUT"].ToString();
                    var items = JsonConvert.DeserializeObject<List<svorders>>(etDetails);
                    DataTable rrdt = new DataTable();
                    rrdt = dbEr.Exec_DataTable("Select Division_Code,State_Code from Mas_Division ");
                    var div = Convert.ToString(rrdt.Rows[0]["Division_Code"]);
                    var statecodes = Convert.ToString(rrdt.Rows[0]["State_Code"]);
                    string statecode = statecodes.TrimEnd(',');
                    string[] state = statecode.Split(',');
                    string sxml = "<ROOT>";
                    for (int j = 0; j < state.Length; j++)
                    {
                        int stateno = 0;
                        DataTable stdt = new DataTable();
                        stdt = dbEr.Exec_DataTable("sELECT MAX(Max_State_Sl_No) statecon from Mas_Product_State_Rates Where State_Code= " + state[j] + "");
                        string statenos = stdt.Rows[0]["statecon"].ToString();
                        if (statenos == "" || statenos == "NULL")
                        {
                            stateno = 1;
                        }
                        else
                        {
                            stateno = (int.Parse(statenos)) + 1;
                        }

                        for (int k = 0; k < items.Count; k++)
                        {
                            sxml += "<Prod divcode=\"" + div + "\"  stateno=\"" + stateno + "\"  statecode=\"" + state[j] + "\"  Matcode=\"" + items[k].MATERIAL + "\"  Qty=\"" + items[k].TOTAL_QUANTITY + "\" baseuom=\"" + items[k].BASE_UOM + "\" baseprice=\"" + items[k].BASE_PRICE + "\" totvalue=\"" + items[k].TOTAL_VALUE + "\" Discount=\"" + items[k].DISCOUNT + "\" totassesval=\"" + items[k].TOTAL_ASSES_VALUE + "\" centtax=\"" + items[k].CENTRAL_TAX + "\" stattax=\"" + items[k].STATE_TAX + "\" inttax=\"" + items[k].INTEGRATED_TAX + "\"  invoiceval=\"" + items[k].TOTAL_INVOICE_VALUE + "\"  />";

                        }
                    }
                    sxml += "</ROOT>";

                    string strQry = "exec  Sp_Insertrate_sapprice '" + sxml + "'";
                    int result = 0;

                    result = dbEr.ExecQry(strQry);


                    return responsesContent;
                }
                else
                {
                    return null;
                }

                //}
                //else
                //{

                //    return null;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        [Route("Getorder_creation")]
        public async Task<string> ORDER_CREATION(string Order_id)
        {
            if (Order_id != "12345")
            {
                try
                {
                    var senderID = "RAD";
                    DB_EReporting dbEr = new DB_EReporting(senderID);

                    var requestUrl = "https://iamhc.ril.com/oauth2/token";


                    var requestData = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");


                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "UjhneENlc2J2QVNleU5ScGhvR19DRmVoa1BFYTp0RFp1VEJleVZqeG9nY0U4SGRBb3h1TmV5Zm9h");


                    var response = await _httpClient.PostAsync(requestUrl, requestData);


                    //if (response.IsSuccessStatusCode)
                    //{
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeObject<JObject>(responseContent);

                    var accessToken = responseObject["access_token"]?.ToString();

                    string Retcode = "";
                    string Trans_id = "";
                    string SP = "";
                    string SH = "";
                    string PY = "";
                    string BP = "";
                    string Order_No = "";
                    DataTable rdt = new DataTable();
                    rdt = dbEr.Exec_DataTable("exec getorder_det_sapsd '" + Order_id + "'");
                    List<ordermaster> rd = new List<ordermaster>();
                    if (rdt.Rows.Count > 0)
                    {
                        Retcode = rdt.Rows[0]["Cust_Code"].ToString();
                        Trans_id = rdt.Rows[0]["MerchantLinkId"].ToString();
                        SP = rdt.Rows[0]["SP"].ToString();
                        SH = rdt.Rows[0]["SH"].ToString();
                        PY = rdt.Rows[0]["PY"].ToString();
                        BP = rdt.Rows[0]["BP"].ToString();
                        Order_No = rdt.Rows[0]["Trans_Sl_No"].ToString();
                        foreach (DataRow dtRow in rdt.Rows)
                        {
                            rd.Add(new ordermaster
                            {
                                MATNR = Convert.ToString(dtRow["Product_Code"]),
                                REQ_QTY = Convert.ToString(dtRow["qty"])
                            });
                        }
                    }
                    var array = rd.ToList();

                    var requestedUrl = "https://apigwhc.ril.com/CU_NJ_NodeRFC_RNM/1.0/callSAPCommon";



                    var data = new
                    {
                        functionModule = "Z_PTC_CRM_ORDER_CREATION",
                        im_sap_source = "P25CLNT400",
                        data = new
                        {
                            TRACK = "PTC",
                            IM_TAB = array,
                            SOLD_TO_PARTY = SP,
                            SHIP_TO_PARTY = SH,
                            BILL_TO_PARTY = BP,
                            PAYER = PY,
                            IM_TRANSID = Trans_id


                        }
                    };
                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    _httpClient.DefaultRequestHeaders.Add("Cookie", "MYSSO=f0e6f443e0631f535487508b7241ac3d397ab7530973ac9c81cb11710b3f224cd029522806692c366a90262ce60ae7c7dd4b53baa4c28926a6cad41d0c5a285e0e7db84890b3226965274ac196529b1da329c11b4288b71622317a51e97b245f722c118dafb751be1fb44067911c55d9fc39d46dcf199b309c6b88ac4ed55eee7849a56c109b06ba2ff622b01edb6af25f3ae4878cf7de973cd8ee16ded32691d75149fbbf78db95556341ccf08cf33feb7b6e50449026a66aad8dacc1dc121560b8f9bdf3dc6ad2ea85285d3ea158b6ecc9e917dc5c36281d787e7cfccebf20d5eb593854c9b238e499e6df11e0f389b216afe2e7dca963811fe77a9220e30ea2d8c88219524d86cd20d7502f7f6ff9206af2efb8b67d11e387f5100b28c5224ca2a84fe6002d6c14be1832b96720c6f84de9d874f224ade18eefca040817ad72965b5bdf6408a58a2106e2f4c49b58; USER=%7B%22domain_id%22%3A%22rahul.kathuria%22%2C%22email%22%3A%22rahul.kathuria%40ril.com%22%2C%22fullname%22%3A%22Rahul%20Kathuria%22%2C%22env%22%3A%22dev%22%2C%22internal%22%3Atrue%7D");

                    var requestedData = new StringContent(json, Encoding.UTF8, "application/json");

                    var responses = await _httpClient.PostAsync(requestedUrl, requestedData);

                    if (responses.IsSuccessStatusCode)
                    {
                        var responsesContent = await responses.Content.ReadAsStringAsync();

                        var resp = JsonConvert.DeserializeObject<JObject>(responsesContent);
                        var SALESDOCUMENT = resp["result"]["EX_PAYLOAD_JSON"]["SALESDOCUMENT"].ToString();
                        var MSG = resp["result"]["EX_PAYLOAD_JSON"]["RETURN"]["MESSAGE"].ToString();
                        if (SALESDOCUMENT != "")
                        {
                            DataTable rfdt = new DataTable();
                            rfdt = dbEr.Exec_DataTable("Update Trans_Payment_Link_Details set LinkStatus='Sales Order Created',Saledoc_No= '" + SALESDOCUMENT + "'  where MerchantLinkId='" + Trans_id + "'");
                            
                            var MobileNom = "";
                            var Transaction_ID = "";
                            var Templateids = "";
                            var Messages = "";
                            DataTable smt = new DataTable();
                            smt = dbEr.Exec_DataTable("Select MerchantLinkId,InvoiceNo,MobileNo from Trans_Payment_Link_Details where InvoiceNo='" + Order_id + "'");
                            List<smsintegrate> rsd = new List<smsintegrate>();
                            if (smt.Rows.Count > 0)
                            {
                                MobileNom = smt.Rows[0]["MobileNo"].ToString();
                                Transaction_ID = smt.Rows[0]["MerchantLinkId"].ToString();
                                Templateids = "1207170166589230838";
                                Messages = "Thank you for choosing ReliVet. Your booking is confirmed:" + SALESDOCUMENT + ".";
                            }

                            var payloade = new
                            {
                                username = "jiocx-relbio",
                                password = "PIcBApQDmMq3E9K4",
                                APIKey = "41195",
                                secretKey = "41195",
                                applicationId = 41195,
                                Templateid = Templateids,
                                MobileNo = MobileNom,
                                Message = Messages,
                                TransactionID = Transaction_ID,
                                CLIName = "RLSVET",
                                sms_type = "T",
                                sms_language = "ENG",
                                dlt_entity_id = "1201158529429286471"
                            };
                            var jsonPayloade = JsonConvert.SerializeObject(payloade);
                            var requestsUrl = "https://relbio.jiocx.com/apggw/relbio/sms/v1/send";
                            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var requess = new StringContent(jsonPayloade, Encoding.UTF8, "application/json");

                            var resposms = await _httpClient.PostAsync(requestsUrl, requess);

                            var responsms = await resposms.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            DataTable rfdt = new DataTable();
                            rfdt = dbEr.Exec_DataTable("Update Trans_Payment_Link_Details set LinkStatus= '" + MSG.Replace("'","''") + "'  where MerchantLinkId='" + Trans_id + "'");

                        }
                        return responsesContent ;
                    }
                    else
                    {
                        var responsesConten = await responses.Content.ReadAsStringAsync();
                        return responsesConten;
                    }

                    //}
                    //else
                    //{

                    //    return null;
                    //}
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                var MSG = "SUCCESS....The API is Working";
                return MSG;
            }
        }
        [HttpPost]
        [Route("Getinvoice_singstatus")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //public async Task<string> invoice_singatus(string invoice_id)
        public async Task<string?> invoice_singatus([FromBody] List<string> belArray)

        {
            invoiceinteg od = new invoiceinteg();

            //if (belArray == null || belArray.Count == 0)
            //{
            //    return BadRequest("Input array is empty");
            //}

            List<invoiceinteg> outputArray = belArray.Select(item => new invoiceinteg { VBELN = item }).ToList();

            var array = outputArray.ToList();
            
            //var merchant_Id = _invoiceinteg.merchantId;
            try
            {
                var senderID = "RAD";
                DB_EReporting dbEr = new DB_EReporting(senderID);

                var requestUrl = "https://iamhc.ril.com/oauth2/token";


                var requestData = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");


                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "UjhneENlc2J2QVNleU5ScGhvR19DRmVoa1BFYTp0RFp1VEJleVZqeG9nY0U4SGRBb3h1TmV5Zm9h");


                var response = await _httpClient.PostAsync(requestUrl, requestData);


                //if (response.IsSuccessStatusCode)
                //{
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseObject = JsonConvert.DeserializeObject<JObject>(responseContent);

                var accessToken = responseObject["access_token"]?.ToString();

                var requestedUrl = "https://apigwhc.ril.com/CU_NJ_NodeRFC_RNM/1.0/callSAPCommon";


                //var data = new
                //{
                //    functionModule = "Z_PTC_ORDER_STATUS",
                //    im_sap_source = "P25CLNT400",
                //    data = new
                //    {
                //        TRACK = "PTC",
                //        IT_ORDER_NUMBERS = new[]
                //        {
                //            new
                //            {
                //                VBELN = Invoice_ID

                //            }
                //        }
                //    }
                //};
                var data = new
                {
                    functionModule = "Z_PTC_ORDER_STATUS",
                    im_sap_source = "P25CLNT400",
                    data = new
                    {
                        TRACK = "PTC",
                        IT_ORDER_NUMBERS = array
                    }
                };

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var requestedData = new StringContent(json, Encoding.UTF8, "application/json");

                var responses = await _httpClient.PostAsync(requestedUrl, requestedData);

                if (responses.IsSuccessStatusCode)
                {
                    //var responsesContent = await responses.Content.ReadAsStringAsync();

                    //var resp = JsonConvert.DeserializeObject<JObject>(responsesContent);

                    var responsesContent = await responses.Content.ReadAsStringAsync();
                    var responseOb = JsonConvert.DeserializeObject<JObject>(responsesContent);

                    string jsonResponse = responseOb.ToString(); // Replace with your actual JSON response


                    var responseobj = JObject.Parse(jsonResponse);
                    var responseobjec = JObject.Parse(jsonResponse);

                    var etDetails = responseobj["result"]["EX_PAYLOAD_JSON"]["ET_ORDER_STATUS"].ToString();
                    var items = JsonConvert.DeserializeObject<List<svinvoice>>(etDetails);


                    for (int k = 0; k < items.Count; k++)
                    {
                        string strQry = "exec saveinvoistatsap '" + items[k].VBELN + "','" + items[k].STATUS + "' ";
                        int result = 0;

                        result = dbEr.ExecQry(strQry);
                    }
                    return responsesContent;
                }
                else
                {
                    return null;
                }

                //}
                //else
                //{

                //    return null;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

    public class YourModel
    {
        public string IM_FM_NAME { get; set; }
        public long unique_no { get; set; }
        public string TRACK { get; set; }
        public string IM_DATE { get; set; }
    }

    //public class ordermaster
    //{
    //    public string MATNR { get; set; }
    //    public string REQ_QTY { get; set; }
    //}
}

