using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Data;
using WebApplicationApi.Models;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        // GET: api/<MasterDataController>
        // GET: api/<MasterDataController>

        [EnableCors("AllowOrigin")]
        [HttpGet]
        [Route("getProductDetails")]
        public IActionResult getProductDetails(string senderID)
        {

            DB_EReporting dbER = new DB_EReporting(senderID);

            if (senderID == "" || senderID == null)
            {
                senderID = "";
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@divName", Convert.ToString(senderID))
            };

            DataSet ds = dbER.Exec_DataSetWithParam("getProductMasterAPI", parameters);
            DataTable dt = new DataTable();
            List<ProductMasters> prom = new List<ProductMasters>();
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                prom = (from DataRow dr in dt.Rows
                        select new ProductMasters()
                        {
                            ProductCode = Convert.ToString(dr["ProductCode"]),
                            ProductName = Convert.ToString(dr["ProductName"]),
                            Short_Name = dr["Short_Name"].ToString(),
                            ProductDescription = dr["ProductDescription"].ToString(),
                            ConversionFactor = dr["ConversionFactor"].ToString(),
                            Grossweight = dr["Grossweight"].ToString(),
                            Netweight = dr["Netweight"].ToString(),
                            UOM = dr["UOM"].ToString(),
                            Base_UOM = dr["Base_UOM"].ToString(),
                            Sub_Division_Name = dr["Sub_Division_Name"].ToString(),
                            ERP_Code = dr["ERP_Code"].ToString(),
                            Brand = dr["Brand"].ToString(),
                            Product_Cat_Name = dr["Product_Cat_Name"].ToString()
                        }).ToList();
            }
            var response = prom.ToList();
            return Ok(response.ToList());
        }


        [EnableCors("AllowOrigin")]
        [HttpGet]
        [Route("getSalesForceDetails")]
        public IActionResult  getSalesForceDetails(string senderID)
        {
            DB_EReporting dbER = new DB_EReporting(senderID);
           

            if (senderID == "" || senderID == null)
            {
                senderID = "";
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@divName", Convert.ToString(senderID))
            };

            DataSet ds = dbER.Exec_DataSetWithParam("getSalesForcelisApi", parameters);
            DataTable dt = new DataTable();
            List<SalesForceMaster> flist = new List<SalesForceMaster>();
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                flist = (from DataRow dr in dt.Rows
                        select new SalesForceMaster()
                        {
                            Employee_Id = Convert.ToString(dr["Employee_Id"]),
                            Employee_Name = Convert.ToString(dr["Employee_Name"]),
                            Designation = dr["Designation"].ToString(),
                            Sf_HQ = dr["Sf_HQ"].ToString(),
                            StateName = dr["StateName"].ToString(),
                            MobileNumber = dr["MobileNumber"].ToString(),
                            Manager_Name = dr["Manager_Name"].ToString(),
                            Territory = dr["Territory"].ToString(),
                            DOB = dr["DOB"].ToString(),
                            Total_Beats = dr["Total_Beats"].ToString(),
                            DOJ = dr["DOJ"].ToString(),
                            Email = dr["Email"].ToString(),
                            Address = dr["Address"].ToString(),
                            Status = dr["Status"].ToString(),
                            Appversion = dr["Appversion"].ToString()
                        }).ToList();

            }

            var response = flist.ToList();

            return Ok(response.ToList());
        }


        [EnableCors("AllowOrigin")]
        [HttpGet]
        [Route("getDistributorDetails")]
        public IActionResult  getDistributorDetails(string senderID)
        {
            DB_EReporting dbER = new DB_EReporting(senderID);
            
            if (senderID == "" || senderID == null)
            {
                senderID = "";
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@divName", Convert.ToString(senderID))
            };

            DataSet ds = dbER.Exec_DataSetWithParam("getDistributorMasterAPI", parameters);
            DataTable dt = new DataTable();
            List<DistributorMaster> dlist = new List<DistributorMaster>();

            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];

                dlist = (from DataRow dr in dt.Rows
                         select new DistributorMaster()
                         {
                             Distributor_Code = Convert.ToString(dr["Distributor_Code"]),
                             ERP_Code = Convert.ToString(dr["ERP_Code"]),
                             Distributor_Name = dr["Distributor_Name"].ToString(),
                             ContactPerson = dr["ContactPerson"].ToString(),
                             Address = dr["Address"].ToString(),
                             Mobile = dr["Mobile"].ToString(),
                             FieldForce_Name = dr["FieldForce_Name"].ToString(),
                             Territory = dr["Territory"].ToString(),
                             Dist_Name = dr["Dist_Name"].ToString(),
                             Taluk_Name = dr["Taluk_Name"].ToString(),
                             StateName = dr["StateName"].ToString(),
                             CategoryName = dr["CategoryName"].ToString(),
                             Type = dr["Type"].ToString(),
                             EmailID = dr["EmailID"].ToString(),
                             GSTN = dr["GSTN"].ToString(),
                             Vendor_Code = dr["Vendor_Code"].ToString(),
                             Username = dr["Username"].ToString()
                         }).ToList();
            }

            var response = dlist.ToList();
            return Ok(response.ToList());
        }


        [EnableCors("AllowOrigin")]
        [HttpGet]
        [Route("getRouteDetails")]
        public IActionResult getRouteDetails(string senderID)
        {
            DB_EReporting dbER = new DB_EReporting(senderID);
            List<DataRow> flist = new List<DataRow>();

            if (senderID == "" || senderID == null)
            {
                senderID = "";
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@divName", Convert.ToString(senderID))
            };

            DataSet ds = dbER.Exec_DataSetWithParam("getRouteMasterAPI", parameters);
            DataTable dt = new DataTable();
            List<RouteMaster> ruom = new List<RouteMaster>();
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];

                ruom = (from DataRow dr in dt.Rows
                        select new RouteMaster()
                        {
                            Territory_Code = Convert.ToString(dr["Territory_Code"]),
                            Route_Code = Convert.ToString(dr["Route_Code"]),
                            Route_Name = dr["Route_Name"].ToString(),
                            Target = dr["Target"].ToString(),
                            Territory_name = dr["Territory_name"].ToString(),
                            Create_Date = dr["Create_Date"].ToString(),
                            StateName = dr["StateName"].ToString(),
                            SF_Name = dr["SF_Name"].ToString(),
                            Emp_Id = dr["sf_emp_id"].ToString(),
                            Distributor_Name = dr["Distributor_Name"].ToString()
                        }).ToList();


            }

            var response = ruom.ToList();
            
            return Ok(response.ToList());
        }

        [EnableCors("AllowOrigin")]
        [HttpGet]
        [Route("getRetailerDetails")]
        public IActionResult getRetailerDetails(string senderID)
        {

            DB_EReporting dbER = new DB_EReporting(senderID);

            if (senderID == "" || senderID == null)
            {
                senderID = "";
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@divName", Convert.ToString(senderID))
            };

            DataSet ds = dbER.Exec_DataSetWithParam("getRetailerMasterAPI", parameters);
            DataTable dt = new DataTable();
            List<RetailerMaster> dlist = new List<RetailerMaster>();

            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                dlist = (from DataRow dr in dt.Rows
                         select new RetailerMaster()
                         {
                             Created_Date = Convert.ToString(dr["Created_Date"]),
                             Retailer_code = Convert.ToString(dr["Retailer_code"]),
                             Retailer_Name = dr["Retailer_Name"].ToString(),
                             ContactPerson = dr["ContactPerson"].ToString(),
                             Mobile_No = dr["Mobile_No"].ToString(),
                             DOA = dr["DOA"].ToString(),
                             DOB = dr["DOB"].ToString(),
                             Retailer_Channel = dr["Retailer_Channel"].ToString(),
                             Retailer_Class = dr["Retailer_Class"].ToString(),
                             Route_Name = dr["Route_Name"].ToString(),
                             Territory_Name = dr["Territory_Name"].ToString(),
                             AreaName = dr["AreaName"].ToString(),
                             Address = dr["Address"].ToString(),
                             City = dr["City"].ToString(),
                             PinCode = dr["PinCode"].ToString(),
                             StateName = dr["StateName"].ToString(),
                             FiledForce = dr["FiledForce"].ToString(),
                             HQ = dr["HQ"].ToString(),
                             Designation = dr["Designation"].ToString(),
                             DistributorName = dr["DistributorName"].ToString(),
                             GSTNO = dr["GSTNO"].ToString(),
                             ERP_Code = dr["ERP_Code"].ToString(),
                             Latitude = dr["Latitude"].ToString(),
                             Longitude = dr["Longitude"].ToString(),
                             Profilepic = dr["Profilepic"].ToString()
                         }).ToList();
            }            
            var response = dlist.ToList();
            return Ok(response.ToList());
        }
    }
}
