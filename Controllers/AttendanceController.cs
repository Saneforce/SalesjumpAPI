using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplicationApi.Models;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : Controller
    {
        [HttpGet]
        [Route("GetAttendanceDetails")]
        public List<Attendance> GetPendingSalesOrders(string senderID, string date = "yyyy-mm-dd")
        {
            DB_EReporting dbER = new DB_EReporting(senderID);
            DataTable dt = new DataTable();
            dt = dbER.Exec_DataTable("exec Attendancedetails_api  '" + date + "'");
            string msg = string.Empty;

            List<Attendance> TOHDetails = new List<Attendance>();
            Attendance od = new Attendance();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRows in dt.Rows)
                    {
                        od = new Attendance();
                        od.SlNo = Convert.ToString(dtRows["SerialNumber"]);
                        od.EmpCode = Convert.ToString(dtRows["sf_emp_id"]);
                        od.EntryDate = Convert.ToString(dtRows["login_date"]);
                        od.InOutFlag = Convert.ToString(dtRows["InOutFlag"]);
                        od.EntryTime = Convert.ToString(dtRows["entry_time"]);
                        od.TrfFlag = Convert.ToString(dtRows["TrfFlag"]);
                        od.Location = Convert.ToString(dtRows["Location"]);
                        TOHDetails.Add(od);
                    }

                }
            }

            return TOHDetails;
        }
    }
}
