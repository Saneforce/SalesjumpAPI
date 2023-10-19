using Microsoft.AspNetCore.Mvc;
using WebApplicationApi.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using Microsoft.AspNetCore.Cors;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // GET: api/<OrderController>
        [EnableCors("AllowOrigin")]
        [HttpGet]
        [Route("GetPendingSalesOrders")]
        public IActionResult GetPendingSalesOrders(string senderID, string distributorCode, string Fromdate = "", string Todate = "")
        {
            DB_EReporting dbER = new DB_EReporting(senderID);
            DataTable dt = new DataTable();
            dt = dbER.Exec_DataTable("exec getOrderHeadForAPI '" + distributorCode + "','" + Fromdate + "','" + Todate + "'");
            DataTable dtt = new DataTable();
            dtt = dbER.Exec_DataTable("exec getOrderDetailsForAPI '" + distributorCode + "','" + Fromdate + "','" + Todate + "'");
            string msg = string.Empty;

            List<TransOrderHead> result = new List<TransOrderHead>();
            TransOrderHead od = new TransOrderHead();
            TransOrderDetail odd = new TransOrderDetail();

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRows in dt.Rows)
                    {
                        od = new TransOrderHead();
                        od.OrderTakenBy = Convert.ToString(dtRows["OrderTakenBy"]);
                        od.DistributorCode = Convert.ToString(dtRows["DISTRIBUTORCODE"]);
                        od.DocNumber = Convert.ToString(dtRows["DOCNUMBER"]);
                        od.DocDate = Convert.ToString(dtRows["DOCDATE"]);
                        od.TransType = "Secondary Sales order";
                        od.OrderNo = Convert.ToString(dtRows["DOCNUMBER"]);
                        od.OrderDate = Convert.ToString(dtRows["ORDERDATE"]);
                        od.CustomerId = Convert.ToString(dtRows["CUSTOMERID"]);
                        od.CustomerName = Convert.ToString(dtRows["CUSTOMERNAME"]);
                        od.ShippingAddress = Convert.ToString(dtRows["SHIPPINGADDRESS"]);
                        od.BillingAddress = Convert.ToString(dtRows["SHIPPINGADDRESS"]);
                        od.GstinNo = Convert.ToString(dtRows["GSTINNO"]);
                        od.Placeofsupply = Convert.ToString(dtRows["STATENAME"]);
                        od.StateName = Convert.ToString(dtRows["STATENAME"]);
                        od.OrderValue = Convert.ToString(dtRows["order_value"]);

                        string? dno = Convert.ToString(dtRows["DOCNUMBER"]);

                        if (dtt.Rows.Count > 0)
                        {
                            foreach (DataRow dttRow in dtt.Rows)
                            {
                                if (dno == Convert.ToString(dttRow["DOCNUMBER"]))
                                {
                                    odd = new TransOrderDetail();

                                    odd.ProductCode = Convert.ToString(dttRow["PRODUCTCODE"]);
                                    odd.ProductName = Convert.ToString(dttRow["PRODUCTNAME"]);
                                    odd.UOM = Convert.ToString(dttRow["UOM"]);
                                    odd.ActualQty = Convert.ToString(dttRow["BILLEDQTY"]);
                                    odd.BilledQty = Convert.ToString(dttRow["BILLEDQTY"]);
                                    odd.CloseingStock = Convert.ToString(dttRow["CloseingStock"]);
                                    odd.Rate = Convert.ToString(dttRow["RATE"]);
                                    odd.Amount = Convert.ToString(dttRow["AMOUNT"]);
                                    odd.TaxCode = "";
                                    odd.TaxPer = "";
                                    odd.TaxAmount = "0.00";

                                    od.TransDetails.Add(odd);
                                }

                            }
                        }

                        result.Add(od);
                    }
                }
            }

            return Ok(result);
        }

        // GET api/<OrderController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<OrderController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{ }

        // GET api/<OrderController>/5
        [EnableCors("AllowOrigin")]
        [HttpPut("{id}")]
        public string UpdateSalesOrders(string senderID, string OrderID)
        {            
            string Success = "";
            try
            {               
                int iReturn = -1;
                DB_EReporting dbEr = new DB_EReporting(senderID);
                iReturn = dbEr.ExecQry("exec UpdateSyncedOrders '" + OrderID + "'");

                if (iReturn > 0)
                {
                    Success = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"UpdatedOrders\":\"" + iReturn.ToString() + "\"}";
                    //return Request.CreateResponse(HttpStatusCode.OK, "{\"Success\":\"true\",\"UpdatedOrders\":\"" + iReturn.ToString() + "\"}");
                }
                else
                {
                    Success = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"UpdatedOrders\":\"" + iReturn.ToString() + "\"}";
                    //return Request.CreateResponse(HttpStatusCode.OK, "{\"Success\":\"false\",\"UpdatedOrders\":\"" + iReturn.ToString() + "\"}");
                }
            }
            catch (Exception ex)
            {
                //Success = ex.Message.ToString();
                Success = HttpStatusCode.InternalServerError + "," + "{\"Success\":\"false\",\"Error\":\"" + ex.Message + "\"}";
                //return Request.CreateResponse(HttpStatusCode.InternalServerError, "{\"Success\":\"false\",\"Error\":\"" + ex.Message + "\"}");
            }

            return Success;
        }

        // DELETE api/<OrderController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{ }
    }
}
