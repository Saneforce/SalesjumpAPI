using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using WebApplicationApi.Models;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        // GET: api/<InvoiceController>

        [HttpGet]
        [Route("getInvoiceOrderDetails")]
        public List<IODetails> getInvoiceOrderDetails(string senderID, string CustomerCode, string InvoiceDate = "")
        {
            DB_EReporting dbER = new DB_EReporting(senderID);
            List<IODetails> IODetails = new List<IODetails>();

            if (CustomerCode == "" || CustomerCode == null)
            {
                CustomerCode = "";
            }

            if (InvoiceDate == "" || InvoiceDate == null)
            {
                InvoiceDate = "";
            }

            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@CustomerCode", Convert.ToString(CustomerCode)),
                new SqlParameter("@InvoiceDate", Convert.ToString(InvoiceDate))
           };

            DataSet ds = dbER.Exec_DataSetWithParam("GetInvoiceHeaderOrderDetails", parameters);

            DataTable dt = new DataTable();

            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        IODetails.Add(new IODetails
                        {
                            CustomerNo = Convert.ToInt32(dtRow["CustomerNo"]),
                            CustomerCode = Convert.ToString(dtRow["CustomerCode"]),
                            CustomerName = Convert.ToString(dtRow["CustomerName"]),
                            Series = Convert.ToString(dtRow["Series"]),
                            SeriesName = Convert.ToString(dtRow["SeriesName"]),
                            InvoiceNum = Convert.ToString(dtRow["InvoiceNum"]),
                            InvoiceDate = Convert.ToDateTime(dtRow["InvoiceDate"]),
                            SaleEmpCode = Convert.ToString(dtRow["SaleEmpCode"]),
                            StateCode = Convert.ToString(dtRow["StateCode"]),
                            GSTIN = Convert.ToString(dtRow["GSTIN"]),
                            ItemCode = Convert.ToString(dtRow["ItemCode"]),
                            ItemName = Convert.ToString(dtRow["ItemName"]),
                            BatchNum = Convert.ToString(dtRow["BatchNum"]),
                            Quantity = Convert.ToDecimal(dtRow["Quantity"]),
                            Price = Convert.ToDecimal(dtRow["Price"]),
                            PackSize = Convert.ToString(dtRow["PackSize"]),
                            UoM = Convert.ToString(dtRow["UoM"]),
                            TaxCode = Convert.ToString(dtRow["TaxCode"]),
                            TaxRate = Convert.ToDecimal(dtRow["TaxRate"])
                        });
                    }
                }
            }

            return IODetails.ToList();
        }

        // GET api/<InvoiceController>/5
        [HttpGet("{id}")]
        //[Route("GetInvoiceOrderDetailsById")]
        public List<InvoiceHeaderDetails> Get(string senderID, string id)
        {
            DB_EReporting dbER = new DB_EReporting(senderID);

            List<InvoiceHeaderDetails> details = new List<InvoiceHeaderDetails>();
            if (id == "" || id == null)
            { id = ""; }

            DataSet ds = new DataSet();
            ds = dbER.MExec_DataSet("exec Get_Invoice_HeaderOrderDetailsById '" + id + "'");

            DataTable hdt = ds.Tables[0];
            DataTable rdt = ds.Tables[1];
            List<InvoiceRowDetails> rd = new List<InvoiceRowDetails>();
            if (rdt.Rows.Count > 0)
            {              
                foreach (DataRow dtRow in rdt.Rows)
                {
                    rd.Add(new InvoiceRowDetails
                    {
                        ItemNo = Convert.ToInt32(dtRow["ItemNo"]),
                        ItemCode = Convert.ToString(dtRow["ItemCode"]),
                        ItemName = Convert.ToString(dtRow["ItemName"]),
                        BatchNum = Convert.ToString(dtRow["BatchNum"]),
                        Quantity = Convert.ToString(dtRow["Quantity"]),
                        Price = Convert.ToString(dtRow["Price"]),
                        PackSize = Convert.ToString(dtRow["PackSize"]),
                        UoM = Convert.ToString(dtRow["UoM"]),
                        TaxCode = Convert.ToString(dtRow["TaxCode"]),
                        TaxRate = Convert.ToString(dtRow["TaxRate"])
                    });
                }
            }
            
            if (hdt.Rows.Count > 0)
            {
                foreach (DataRow dtRow in hdt.Rows)
                {
                    details.Add(new InvoiceHeaderDetails
                    {
                        CustomerNo = Convert.ToInt32(dtRow["CustomerNo"]),
                        CustomerCode = Convert.ToString(dtRow["CustomerCode"]),
                        CustomerName = Convert.ToString(dtRow["CustomerName"]),
                        Series = Convert.ToString(dtRow["Series"]),
                        SeriesName = Convert.ToString(dtRow["SeriesName"]),
                        InvoiceNum = Convert.ToString(dtRow["InvoiceNum"]),
                        InvoiceDate = Convert.ToString(dtRow["InvoiceDate"]),
                        SaleEmpCode = Convert.ToString(dtRow["SaleEmpCode"]),
                        StateCode = Convert.ToString(dtRow["StateCode"]),
                        GSTIN = Convert.ToString(dtRow["GSTIN"]),
                        rowDetails = rd.ToList()
                    });
                }                
            }
            else
            {
                details = null;
            }

            return details.ToList();
            //return "details";
        }

        // POST api/<InvoiceController>
        [HttpPost]
        //[Route("InsertInvoiceHaderDeatils")]
        public string Post(string senderID, List<InvoiceHeaderDetails> headerDetails)
        {
            string mesg = ""; string PName = "";

            try
            {
                DB_EReporting dbEr = new DB_EReporting(senderID);

                //int iReturn = -1;
                //senderID = "everclean";

                if (headerDetails != null)
                {
                    if (headerDetails.Count > 0)
                    {
                        for (int i = 0; i < headerDetails.Count; i++)
                        {
                            PName = "";
                            PName = "Insert_Invoice_HeaderDetails";

                            SqlParameter[] parameters =
                            {
                                new SqlParameter("@CustomerCode", Convert.ToString(headerDetails[i].CustomerCode)),
                                new SqlParameter("@CustomerName", Convert.ToString(headerDetails[i].CustomerName)),
                                new SqlParameter("@Series", Convert.ToString(headerDetails[i].Series)),
                                new SqlParameter("@SeriesName", Convert.ToString(headerDetails[i].SeriesName)),
                                new SqlParameter("@InvoiceDate", Convert.ToString(headerDetails[i].InvoiceDate)),
                                new SqlParameter("@InvoiceNum", Convert.ToString(headerDetails[i].InvoiceNum)),
                                new SqlParameter("@SaleEmpCode", Convert.ToString(headerDetails[i].SaleEmpCode)),
                                new SqlParameter("@StateCode", Convert.ToString(headerDetails[i].StateCode)),
                                new SqlParameter("@GSTIN", Convert.ToString(headerDetails[i].GSTIN))
                            };

                            mesg = dbEr.SP_Exec_NonQueryWithParam(PName, parameters);

                            var rowDetails = headerDetails[i].rowDetails.ToList();
                            if (rowDetails != null && rowDetails.Count > 0)
                            {
                                string SPName = "";
                                for (int j = 0; j < rowDetails.Count; j++)
                                {

                                    SPName = "Insert_Invoice_RowDetails";
                                    SqlParameter[] prams =
                                    {
                                        new SqlParameter("@ItemCode", Convert.ToString(rowDetails[j].ItemCode)),
                                        new SqlParameter("@ItemName", Convert.ToString(rowDetails[j].ItemName)),
                                        new SqlParameter("@BatchNum", Convert.ToString(rowDetails[j].BatchNum)),
                                        new SqlParameter("@Quantity", Convert.ToString(rowDetails[j].Quantity)),
                                        new SqlParameter("@Price", Convert.ToString(rowDetails[j].Price)),
                                        new SqlParameter("@PackSize", Convert.ToString(rowDetails[j].PackSize)),
                                        new SqlParameter("@UoM", Convert.ToString(rowDetails[j].UoM)),
                                        new SqlParameter("@TaxCode", Convert.ToString(rowDetails[j].TaxCode)),
                                        new SqlParameter("@TaxRate", Convert.ToString(rowDetails[j].TaxRate))
                                    };
                                    mesg = dbEr.SP_Exec_NonQueryWithParam(SPName, prams);
                                }
                            }
                        }

                        if (mesg == "Success")
                        { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"InsertedInvoiceDetails\":\"" + mesg.ToString() + "\"}"; }
                        else
                        { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"FieldInsertedInvoiceDetails\":\"" + mesg.ToString() + "\"}"; }
                    }
                }
            }
            catch (Exception ex)
            { mesg = HttpStatusCode.InternalServerError + "," + "{\"Success\":\"false\",\"Error\":\"" + ex.Message + "\"}"; }

            //return headerDetails == null ? TypedResults.NotFound() : TypedResults.Ok(headerDetails);

            return mesg;
        }

        // PUT api/<InvoiceController>/5
        [HttpPut("{id}")]
        //[Route("UpdateInvoiceHaderDeatils")]
        public string Put(int id, string senderID, [FromBody] List<InvoiceHeaderDetails> headerDetails)
        {
            //senderID = "everclean";
            //string message = "";
            string mesg = ""; string PName = "";
            //int iReturn = -1;
            try
            {
                DB_EReporting dbEr = new DB_EReporting(senderID);

                if (headerDetails != null)
                {
                    if (headerDetails.Count > 0)
                    {
                        for (int i = 0; i < headerDetails.Count; i++)
                        {
                            PName = "";
                            PName = "Update_InvoiceOrder_HeaderDetails";

                            SqlParameter[] parameters =
                            {
                                new SqlParameter("@CustomerNo", Convert.ToInt32(id)),
                                new SqlParameter("@CustomerCode", Convert.ToString(headerDetails[i].CustomerCode)),
                                new SqlParameter("@CustomerName", Convert.ToString(headerDetails[i].CustomerName)),
                                new SqlParameter("@Series", Convert.ToString(headerDetails[i].Series)),
                                new SqlParameter("@SeriesName", Convert.ToString(headerDetails[i].SeriesName)),
                                new SqlParameter("@InvoiceDate", Convert.ToString(headerDetails[i].InvoiceDate)),
                                new SqlParameter("@InvoiceNum", Convert.ToString(headerDetails[i].InvoiceNum)),
                                new SqlParameter("@SaleEmpCode", Convert.ToString(headerDetails[i].SaleEmpCode)),
                                new SqlParameter("@StateCode", Convert.ToString(headerDetails[i].StateCode)),
                                new SqlParameter("@GSTIN", Convert.ToString(headerDetails[i].GSTIN))
                            };

                            mesg = dbEr.SP_Exec_NonQueryWithParam(PName, parameters);

                            var rowDetails = headerDetails[i].rowDetails.ToList();
                            if (rowDetails != null && rowDetails.Count > 0)
                            {
                                string SPName = "";
                                for (int j = 0; j < rowDetails.Count; j++)
                                {

                                    SPName = "Update_InvoiceOrder_RowDetails";
                                    SqlParameter[] prams =
                                    {
                                        new SqlParameter("@ItemNo", Convert.ToInt32(rowDetails[j].ItemNo)),
                                        new SqlParameter("@ItemCode", Convert.ToString(rowDetails[j].ItemCode)),
                                        new SqlParameter("@ItemName", Convert.ToString(rowDetails[j].ItemName)),
                                        new SqlParameter("@BatchNum", Convert.ToString(rowDetails[j].BatchNum)),
                                        new SqlParameter("@Quantity", Convert.ToString(rowDetails[j].Quantity)),
                                        new SqlParameter("@Price", Convert.ToString(rowDetails[j].Price)),
                                        new SqlParameter("@PackSize", Convert.ToString(rowDetails[j].PackSize)),
                                        new SqlParameter("@UoM", Convert.ToString(rowDetails[j].UoM)),
                                        new SqlParameter("@TaxCode", Convert.ToString(rowDetails[j].TaxCode)),
                                        new SqlParameter("@TaxRate", Convert.ToString(rowDetails[j].TaxRate))
                                    };
                                    mesg = dbEr.SP_Exec_NonQueryWithParam(SPName, prams);
                                }
                            }
                        }

                        if (mesg == "Success")
                        {
                            mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"UpdatedInvoiceOrderDetails\":\"" + mesg.ToString() + "\"}";
                        }
                        else
                        {
                            mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"FieldUpdatedInvoiceOrderDetails\":\"" + mesg.ToString() + "\"}";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                mesg = HttpStatusCode.InternalServerError + "," + "{\"Success\":\"false\",\"Error\":\"" + ex.Message + "\"}";

            }

            return mesg;
        }

        //// DELETE api/<InvoiceController>/5
        [HttpDelete("{id}")]
        public string Delete(string senderID, string id)
        {
            if (id == null || id == "") { id = "0"; }
            int CustomerNo = 0;
            CustomerNo = Convert.ToInt32(id);
            string mesg = "";

            try
            {
                string squery = "";
                squery = "Select *from Trans_Invoice_RowDetails Where CustomerNo = " + CustomerNo + "";
                DB_EReporting dbEr = new DB_EReporting(senderID);
                DataSet ds = dbEr.Exec_DataSet(squery);
                if (ds.Tables[0].Rows.Count > 0)
                { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"RecordsExists\":\"" + mesg.ToString() + "\"}"; }
                else
                {
                    squery = "Delete from Trans_Invoice_HeaderDetails Where CustomerNo = " + CustomerNo + "";

                    int i = dbEr.ExecQry(squery);

                    if (i > 0)
                    { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"RecordsDeletedSuccessfully\":\"" + mesg.ToString() + "\"}"; }
                    else
                    { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"FieldDeletedInvoiceOrderDetails\":\"" + mesg.ToString() + "\"}"; }
                }
            }
            catch (Exception ex) { mesg = HttpStatusCode.InternalServerError + "," + "{\"Success\":\"false\",\"Error\":\"" + ex.Message + "\"}"; }

            return mesg;
        }
    }
}
