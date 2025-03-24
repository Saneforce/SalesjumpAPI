using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationApi.Models;
using Microsoft.AspNetCore.Cors;
using System.Net;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using System;
using Newtonsoft.Json;
using System.Reflection;
using System.Net.Mail;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnimedApiController : ControllerBase
    {
        public UnimedApiController() { }

        [EnableCors("AllowOrigin")]
        [HttpGet]
        [Route("syncShipingAddress")]
        public async Task<string> syncShipingAddress()
        {
            DB_EReporting dbER = new DB_EReporting("UNIMED");
            string msg = "Imported Successfully";
            //string url = "http://api.kckpharma.com/api/syncDeliveryAddress?DBID=AED_UNIMED_T";
            string url = "http://api.unimed.com.my:55404/api/syncDeliveryAddress?DBID=AED_UNIMED_SB";
            HttpClient nClient = new HttpClient();
            try
            {
                string ApiData = await nClient.GetStringAsync(url);
                
                dynamic dynObj = JsonConvert.DeserializeObject(ApiData);

                string sQry = "delete from UNIShipAdd";
                dbER.ExecQry(sQry);

                foreach (dynamic result in dynObj)
                {
                    sQry = " insert into UNIShipAdd(AccNo,BranchCode,BranchName,Address1,Address2,Address3,Address4,PostCode,Contact,Phone1,Phone2,Fax1,Fax2,AreaCode,SalesAgent,PurchaseAgent,EmailAddress) " +
                        " select '" + result.AccNo + "','" + result.BranchCode + "','" + result.BranchName + "','" + result.Address1 + "','" + result.Address2 + "','" + result.Address3 + "','" + result.Address4 + "','" + result.PostCode + "','" + result.Contact + "','" + result.Phone1 + "','" + result.Phone2 + "','" + result.Fax1 +
                        "','" + result.Fax2 + "','" + result.AreaCode + "','" + result.SalesAgent + "','" + result.PurchaseAgent + "','" + result.EmailAddress + "' ";
                    dbER.ExecQry(sQry);
                }

                url = "http://api.unimed.com.my:55404/api/syncDeliveryAddress?DBID=AED_KCK_LIVE";
                ApiData = await nClient.GetStringAsync(url);
                dynamic dynObj1 = JsonConvert.DeserializeObject(ApiData);

                foreach (dynamic result in dynObj1)
                {
                    sQry = " insert into UNIShipAdd(AccNo,BranchCode,BranchName,Address1,Address2,Address3,Address4,PostCode,Contact,Phone1,Phone2,Fax1,Fax2,AreaCode,SalesAgent,PurchaseAgent,EmailAddress) " +
                        " select '" + result.AccNo + "','" + result.BranchCode + "','" + result.BranchName + "','" + result.Address1 + "','" + result.Address2 + "','" + result.Address3 + "','" + result.Address4 + "','" + result.PostCode + "','" + result.Contact + "','" + result.Phone1 + "','" + result.Phone2 + "','" + result.Fax1 +
                        "','" + result.Fax2 + "','" + result.AreaCode + "','" + result.SalesAgent + "','" + result.PurchaseAgent + "','" + result.EmailAddress + "' ";
                    dbER.ExecQry(sQry);
                }

                url = "http://api.unimed.com.my:55404/api/syncDeliveryAddress?DBID=AED_FUJI";
                ApiData = await nClient.GetStringAsync(url);
                dynamic dynObj2 = JsonConvert.DeserializeObject(ApiData);

                foreach (dynamic result in dynObj2)
                {
                    sQry = " insert into UNIShipAdd(AccNo,BranchCode,BranchName,Address1,Address2,Address3,Address4,PostCode,Contact,Phone1,Phone2,Fax1,Fax2,AreaCode,SalesAgent,PurchaseAgent,EmailAddress) " +
                        " select '" + result.AccNo + "','" + result.BranchCode + "','" + result.BranchName + "','" + result.Address1 + "','" + result.Address2 + "','" + result.Address3 + "','" + result.Address4 + "','" + result.PostCode + "','" + result.Contact + "','" + result.Phone1 + "','" + result.Phone2 + "','" + result.Fax1 +
                        "','" + result.Fax2 + "','" + result.AreaCode + "','" + result.SalesAgent + "','" + result.PurchaseAgent + "','" + result.EmailAddress + "' ";
                    dbER.ExecQry(sQry);
                }

                sQry = "exec updShipAddress";
                dbER.ExecQry(sQry);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        [EnableCors("AllowOrigin")]
        [HttpGet]
        [Route("syncSchemePrice")]
        public async Task<string> syncSchemePrice()
        {
            DB_EReporting dbER = new DB_EReporting("UNIMED");
            string msg = "Imported Successfully";
            string url = "http://api.unimed.com.my:55404/api/syncSchemePrice?DBID=AED_UNIMED_SB";
            HttpClient nClient = new HttpClient();
            try
            {

                string sQry = "delete from uplScheme";
                dbER.ExecQry(sQry);

                string ApiData = await nClient.GetStringAsync(url);
                dynamic dynObj = JsonConvert.DeserializeObject(ApiData);
                foreach (dynamic result in dynObj)
                {
                    sQry = "insert into uplScheme(AccNo,ItemCode,UOM,UseFixedPrice,Qty,price,perc) " +
                        " select '" + result.AccNo + "','" + result.ItemCode + "','" + result.UOM + "','" + result.UseFixedPrice + "','" + result.Qty + "','" + result.price + "','" + result.perc + "'";
                    dbER.ExecQry(sQry);
                }

                url = "http://api.unimed.com.my:55404/api/syncSchemePrice?DBID=AED_KCK_LIVE";
                ApiData = await nClient.GetStringAsync(url);
                dynamic dynObj1 = JsonConvert.DeserializeObject(ApiData);
                foreach (dynamic result in dynObj1)
                {
                    sQry = "insert into uplScheme(AccNo,ItemCode,UOM,UseFixedPrice,Qty,price,perc) " +
                        " select '" + result.AccNo + "','" + result.ItemCode + "','" + result.UOM + "','" + result.UseFixedPrice + "','" + result.Qty + "','" + result.price + "','" + result.perc + "'";
                    dbER.ExecQry(sQry);
                }

                url = "http://api.unimed.com.my:55404/api/syncSchemePrice?DBID=AED_FUJI";
                ApiData = await nClient.GetStringAsync(url);
                dynamic dynObj2 = JsonConvert.DeserializeObject(ApiData);
                foreach (dynamic result in dynObj2)
                {
                    sQry = "insert into uplScheme(AccNo,ItemCode,UOM,UseFixedPrice,Qty,price,perc) " +
                        " select '" + result.AccNo + "','" + result.ItemCode + "','" + result.UOM + "','" + result.UseFixedPrice + "','" + result.Qty + "','" + result.price + "','" + result.perc + "'";
                    dbER.ExecQry(sQry);
                }

                sQry = "exec syncSpecialPrice";
                dbER.ExecQry(sQry);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
    }
}
