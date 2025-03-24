using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Administration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using WebApplicationApi.Models;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientSiteController : ControllerBase
    {

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> createNew(Company company)
        {

            iResponse response = new iResponse();
            string msg = "";
            if (company == null) {
                response.Success = false;
                response.Message="Client details missing ";
            }
            else {
                try
                {
                    DB_EReporting db = new DB_EReporting();
                    SqlParameter[] sp = new SqlParameter[] {
                        new SqlParameter("@Comp",company.CompanyName),
                        new SqlParameter("@SHNM",company.shortName),
                        new SqlParameter("@Url",company.Url),
                        new SqlParameter("@States",company.States),
                        new SqlParameter("@CountryCd",company.CountryCd),
                        new SqlParameter("@UserCnt",company.UserCnt),
                        new SqlParameter("@Cnt",company.Cnt),
                        new SqlParameter("@Rate",company.Rate)
                    };
                    db.Exec_NonQueryWithParam("CreateNewSiteDB", CommandType.StoredProcedure, sp);
                    msg = "Database Created...\n";
                    ServerManager serverMgr = new ServerManager();
                    string strhostname = "salesjump.in"; //abc.com
                    string strWebsitename = company.Url.ToLower() + "." + strhostname; // abc
                    string strApplicationPool = company.Url.ToLower() + "." + strhostname;  // set your deafultpool :4.0 in IIS
                    string bindinginfo = "*:80:" + strhostname;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    Boolean bWebsite = IsWebsiteExists(strWebsitename, serverMgr);
                    if (!bWebsite)
                    {
                        serverMgr.ApplicationPools.Add(strApplicationPool);
                        Site mySite = serverMgr.Sites.Add(strWebsitename.ToString(), "http", bindinginfo, "D:\\Websites\\SalesJump\\E-Report_DotNet\\"); //"D:\\Website\\fmcg"
                        mySite.ApplicationDefaults.ApplicationPoolName = strApplicationPool;

                        mySite.Bindings.Clear();
                        mySite.Bindings.Add(string.Format("{0}:{2}:{1}", "*", strWebsitename, "80"), "http");
                        mySite.Bindings.Add(string.Format("{0}:{2}:www.{1}", "*", strWebsitename, "80"), "http");
                        serverMgr.CommitChanges();
                        response.Success = true;
                        response.Message = "Site Created Successfully";
                    }
                    else {
                        msg = "Site Already Created...\n";
                        response.Success = true;
                        response.Message = msg;
                    }

                }
                catch(Exception ex) {

                    response.Success = true;
                    response.Message = msg + "\n Site Creation Error : "+ex.Message;
                }
            }
            return new JsonResult(response);
        }
        public static bool IsWebsiteExists(string strWebsitename, ServerManager serverMgr)
        {
            Boolean flagset = false;
            SiteCollection sitecollection = serverMgr.Sites;
            foreach (Site site in sitecollection)
            {
                if (site.Name == strWebsitename.ToString())
                {
                    flagset = true;
                    break;
                }
                else
                {
                    flagset = false;
                }
            }
            return flagset;
        }
    }
}
