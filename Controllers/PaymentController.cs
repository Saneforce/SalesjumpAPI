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
    public class PaymentController : ControllerBase
    {
        // GET: api/<PaymentController>       
      
        [HttpPost]
        [Route("PaymentResponseInsert1")]        
        public string PaymentResponseInsert1(string senderID,  Paymentparam param)
        {            
            string get_transaction_id = string.Empty;
            string get_order_id = string.Empty;
            string get_payment_id = string.Empty;
            string get_sign = string.Empty;
            string statuscode = string.Empty;
            string PaymentAmnt = string.Empty;
            string mesg = "";
                             
            try
            {
                DB_EReporting dbER = new DB_EReporting(senderID);

                string responsemsg = param.msg;
                string[] strsplit = responsemsg.Split('|');

                if (strsplit.Length > 0)
                {
                    get_transaction_id = strsplit[1];
                    get_payment_id = strsplit[2];
                    PaymentAmnt = strsplit[4];
                    int checksum = (strsplit.Length) - 1;
                    get_sign = strsplit[checksum];
                    statuscode = strsplit[14];
                }
                get_order_id = get_transaction_id.Substring(0, get_transaction_id.Length - 14);//a.substring(0, a.Length - 14)
               
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@pay_order_id", get_order_id),
                    new SqlParameter("@pay_payment_id",get_payment_id),
                    new SqlParameter("@pay_signature_id", get_sign),
                    new SqlParameter("@status_code", statuscode),
                    new SqlParameter("@Payment_Amount", PaymentAmnt),
                    new SqlParameter("@transaction_id", get_transaction_id)
                };
                
                mesg = dbER.SP_Exec_NonQueryWithParam("Save_Primary_Payment_billdesk", parameters);
                //var response = Request.CreateResponse(HttpStatusCode.Redirect, "{success:true}");
                //response.Headers.Location = new Uri("https://govind.sanfmcg.com/Stockist/Payment_List.aspx");
                //return response;
                if (mesg != null && mesg == "Success")
                {
                    mesg = HttpStatusCode.Redirect + "," + "{success:true}"; // HttpStatusCode.OK + "," + "{\"Success\":\"true\",\"Message\":\"" + mesg.ToString() + "\"}";
                }
                else
                {
                    mesg = HttpStatusCode.Redirect + "," + "{success:false}";
                }               
                var response = HttpStatusCode.Redirect + "," + "{\"Success\":\"true\",\"Message\":\"" + mesg.ToString() + "\"}";                
            }
            catch (Exception ex)
            {
                mesg = HttpStatusCode.InternalServerError + "," + "{success:false,errormessage:" + ex.Message + ex.StackTrace + "}";
            }

            return  mesg;
        }

        [HttpPost]
        [Route("PaymentResponseInsertt")]
        public string PaymentResponseInsert(string senderID, string Stk_Code, string div_code, float payment_amount, string payment_type, string payment_type_code, string order_date, Paymentparam param)
        {
            string get_order_id = string.Empty;
            string get_transaction_id = string.Empty;
            string get_payment_id = string.Empty;
            string get_sign = string.Empty;
            string statuscode = string.Empty;
            string PaymentAmnt = string.Empty;
            string Url = "";
            try
            {
                string responsemsg = param.msg;
                string[] strsplit = responsemsg.Split('|');
                if (strsplit.Length > 0)
                {
                    get_transaction_id = strsplit[1];
                    get_payment_id = strsplit[2];
                    PaymentAmnt = strsplit[4];
                    int checksum = (strsplit.Length) - 1;
                    get_sign = strsplit[checksum];
                    statuscode = strsplit[14];
                }
                get_order_id = get_transaction_id.Substring(0, get_transaction_id.Length - 14);

                DB_EReporting dbER = new DB_EReporting(senderID);
                int dt = 0;
                dt = dbER.ExecQry("exec Save_Primary_Payment_billdesk_1 '" + get_order_id + "','" + payment_type + "','" + payment_type_code + "','" + get_transaction_id + "','" + payment_amount + "','" + order_date + "','Online','','" + div_code + "','" + Stk_Code + "','" + get_order_id + "','" + get_payment_id + "','" + get_sign + "','" + statuscode + "','" + responsemsg + "'");
                if (dt > 0)
                {
                    Url = HttpStatusCode.Redirect + "," + "{success:true}" + "http://" + senderID + ".sanfmcg.com/Stockist/Payment_List.aspx";
                }
                else
                {
                    Url = HttpStatusCode.Redirect + "," + "{success:false}" + "http://" + senderID + ".sanfmcg.com/Stockist/Payment_List.aspx"; 
                }
                //Uri u = new Uri("http://" + senderID + ".sanfmcg.com/Stockist/Payment_List.aspx");

            }
            catch (Exception ex)
            {

                Url = HttpStatusCode.InternalServerError + "," + "{success:false,errormessage:" + ex.Message + ex.StackTrace + "}" + "http://" + senderID + ".sanfmcg.com/Stockist/Payment_List.aspx"; 
            }
            return Url;
        }
    }
}
