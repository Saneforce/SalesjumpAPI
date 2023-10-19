using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using WebApplicationApi.Models;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        // POST api/<InvoiceController>
        [HttpPost]
        //[Route("InsertInvoiceHaderDeatils")]
        public string Post(string senderID, List<Testmodel> headerDetails)
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
                            PName = "InserttestDetails";

                            SqlParameter[] parameters =
                            {
                                new SqlParameter("@Fieldcode", Convert.ToString(headerDetails[i].Fieldcode)),
                                new SqlParameter("@Filedname", Convert.ToString(headerDetails[i].Filedname)),
                                new SqlParameter("@state", Convert.ToString(headerDetails[i].state)),
                                new SqlParameter("@Insertdate", Convert.ToString(headerDetails[i].Insertdate))                               
                            };

                            mesg = dbEr.SP_Exec_NonQueryWithParam(PName, parameters);

                        }

                        if (mesg == "Success")
                        { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"InsertedDetails\":\"" + mesg.ToString() + "\"}"; }
                        else
                        { mesg = HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"FieldInsertedDetails\":\"" + mesg.ToString() + "\"}"; }
                    }
                }
            }
            catch (Exception ex)
            { mesg = HttpStatusCode.InternalServerError + "," + "{\"Success\":\"false\",\"Error\":\"" + ex.Message + "\"}"; }

            //return headerDetails == null ? TypedResults.NotFound() : TypedResults.Ok(headerDetails);

            return mesg;
        }
    }
}
