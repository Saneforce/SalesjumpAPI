using Microsoft.AspNetCore.Mvc;
using WebApplicationApi.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using Microsoft.AspNetCore.Http.Metadata;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriOrderController : ControllerBase
    {
        // GET: api/<PriOrderController>

        [HttpGet]
        //[Route("getPriOrdersDetails")]
        public List<PriOrderHeader> Get(string senderID, string CustomerCode, string date = "")
        {
            DB_EReporting dbER = new DB_EReporting(senderID);          
            string query = "";

            if (CustomerCode == "" || CustomerCode == null)
            { CustomerCode = ""; }
            if (date == "" || date == null)
            { date = ""; }

            query = "EXEC GetPriOrdersRowDetails '" + CustomerCode + "','" + date + "'";

            DataTable dt = dbER.Exec_DataTable(query);          

            List<PriOrderHeader> POHD = new List<PriOrderHeader>();

            PriOrderHeader hds = new PriOrderHeader();
            
            if (dt.Rows.Count > 0)
            {
                // On all tables' rows
                foreach (DataRow dtRow in dt.Rows)
                {
                    hds = new PriOrderHeader();
                    hds.CustomerNo = Convert.ToInt32(dtRow["CustomerNo"]);
                    hds.CustomerCode = Convert.ToString(dtRow["CustomerCode"]);
                    hds.CustomerName = Convert.ToString(dtRow["CustomerName"]);
                    hds.Series = Convert.ToString(dtRow["Series"]);
                    hds.SeriesName = Convert.ToString(dtRow["SeriesName"]);
                    hds.InvoiceNum = Convert.ToString(dtRow["InvoiceNum"]);
                    hds.InvoiceDate = Convert.ToString(dtRow["InvoiceDate"]);
                    hds.SaleEmpCode = Convert.ToString(dtRow["SaleEmpCode"]);
                    hds.StateCode = Convert.ToString(dtRow["StateCode"]);
                    hds.GSTIN = Convert.ToString(dtRow["GSTIN"]);

                    hds.ItemCode = Convert.ToString(dtRow["ItemCode"]);
                    hds.ItemName = Convert.ToString(dtRow["ItemName"]);
                    hds.BatchNum = Convert.ToString(dtRow["BatchNum"]);
                    hds.Quantity = Convert.ToString(dtRow["Quantity"]);
                    hds.Price = Convert.ToString(dtRow["Price"]);
                    hds.PackSize = Convert.ToString(dtRow["PackSize"]);
                    hds.UoM = Convert.ToString(dtRow["UoM"]);
                    hds.TaxCode = Convert.ToString(dtRow["TaxCode"]);
                    hds.TaxRate = Convert.ToString(dtRow["TaxRate"]);

                    POHD.Add(hds);
                }
            }
            return POHD;
        }

        // GET api/<PriOrderController>/5        
        [HttpGet("{id}")]
        //[Route("GetPriOrderDetailsById")]
        public List<PriOrdersHeaderDetails> PriOrderHeaderDetailsById(string senderID, string id)
        {
            DB_EReporting dbER = new DB_EReporting(senderID);

            List<PriOrdersHeaderDetails> details = new List<PriOrdersHeaderDetails>();
            if (id == "" || id == null)
            { id = ""; }

            DataSet ds = new DataSet();
            ds = dbER.MExec_DataSet("exec Get_Invoice_PriOrderHeaderDetailsById '" + id + "'");

            DataTable hdt = ds.Tables[0];
            DataTable rdt = ds.Tables[1];
            List<PriOrdersRowDetails> rd = new List<PriOrdersRowDetails>();
            if (rdt.Rows.Count > 0)
            {
                foreach (DataRow dtRow in rdt.Rows)
                {
                    rd.Add(new PriOrdersRowDetails
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
                    details.Add(new PriOrdersHeaderDetails
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

            return details;
            //return "details";
        }

        // POST api/<PriOrderController>
        [HttpPost]
        //[Route("InsertPriOrderHaderDeatils")]
        public string InsertPriOrderDeatails(string senderID, [FromBody] List<PriOrdersHeaderDetails> headerDetails)
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
                            PName = "Insert_PriOrder_HeaderDetails";

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
                                    SPName = "Insert_PriOrder_RowDetails";
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
                        { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"InsertedPriOrderDetails\":\"" + mesg.ToString() + "\"}"; }
                        else
                        { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"FieldInsertedPriOrderDetails\":\"" + mesg.ToString() + "\"}"; }
                    }
                }
            }
            catch (Exception ex)
            { mesg = HttpStatusCode.InternalServerError + "," + "{\"Success\":\"false\",\"Error\":\"" + ex.Message + "\"}"; }

            return mesg;
        }

        // PUT api/<PriOrderController>/5
        [HttpPut("{id}")]
        //[Route("UpdatePriOrderHaderDeatils")]
        public string UpdatePriOrderDeatails(int id, string senderID, [FromBody] List<PriOrdersHeaderDetails> headerDetails )
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
                            PName = "Update_PriOrder_HeaderDetails";

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
                                    SPName = "Update_PriOrder_RowDetails";
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
                        { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"UpdatedPriOrderDetails\":\"" + mesg.ToString() + "\"}"; }
                        else
                        { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"FieldUpdatedPriOrderDetails\":\"" + mesg.ToString() + "\"}"; }
                    }
                }
            }
            catch (Exception ex)
            { mesg = HttpStatusCode.InternalServerError + "," + "{\"Success\":\"false\",\"Error\":\"" + ex.Message + "\"}"; }

            return mesg;
        }

        // DELETE api/<PriOrderController>/5
        [HttpDelete("{id}")]
        public string DeletePriOrderDeatails(string senderID, string id)
        {          
            if (id == null || id == "") { id = "0"; }
            int CustomerNo = 0;
            CustomerNo = Convert.ToInt32(id);
            string mesg = "";
                    
            try
            {
                string squery = "";
                squery = "Select *from Trans_PriOrder_RowDetail Where CustomerNo = " + CustomerNo + "";

                DB_EReporting dbEr = new DB_EReporting(senderID);
                DataSet ds = dbEr.Exec_DataSet(squery);

                if (ds.Tables[0].Rows.Count > 0)
                { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"RecordsExists\":\"" + mesg.ToString() + "\"}"; }
                else
                {
                    squery = "Delete from Trans_PriOrder_HeaderDetail Where CustomerNo = " + CustomerNo + "";

                    int i = dbEr.ExecQry(squery);

                    if (i > 0)
                    { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"RecordsDeletedSuccessfully\":\"" + mesg.ToString() + "\"}"; }
                    else
                    { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"FieldDeletedPriOrderDetails\":\"" + mesg.ToString() + "\"}"; }
                }
            }
            catch(Exception ex) { mesg = HttpStatusCode.InternalServerError + "," + "{\"Success\":\"false\",\"Error\":\"" + ex.Message + "\"}"; }

            return mesg;
        }
    }
}
