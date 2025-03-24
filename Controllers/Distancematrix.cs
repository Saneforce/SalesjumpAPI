using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using WebApplicationApi.Models;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json.Linq;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("Policy1")]
    [ApiController]
    public class Distancematrix : ControllerBase
    {
        // GET: api/<Distancematrix>
        [HttpGet]
        [Route("DistancelatlongDetails")]
        public List<DistanceMatrixDetails> DistancelatlongDetails(string senderID, string Hdate, string DivCode, string sf_code)
        {

            List<Originslatlong> Olist = new List<Originslatlong>();
            List<Distinationslatlong> dlist = new List<Distinationslatlong>();

            List<DistanceMatrixDetails> dslist = new List<DistanceMatrixDetails>();

            DB_EReporting dbER = new DB_EReporting(senderID);

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DivCode", Convert.ToString(DivCode)),
                new SqlParameter("@Sf_code", Convert.ToString(sf_code)),
                new SqlParameter("@Hdate", Convert.ToString(Hdate))
            };

            DataSet ds = dbER.Exec_DataSetWithParam("LoginHistoryDistance", parameters);
            DataTable tdt = new DataTable();
            DataTable dt = new DataTable();

            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0]; tdt = ds.Tables[1];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sfcode = dt.Rows[i]["Sf_Code"].ToString();
                    double StartLat = Convert.ToDouble(dt.Rows[i]["Start_Lat"]);
                    double StartLong = Convert.ToDouble(dt.Rows[i]["Start_Long"]);
                    string Start_Time = Convert.ToString(dt.Rows[i]["Start_Time"]);
                    string ETime = Convert.ToString(dt.Rows[i]["End_Time"]);
                    string ELat = Convert.ToString(dt.Rows[i]["End_Lat"]);
                    string ELong = Convert.ToString(dt.Rows[i]["End_Long"]);
                    string arravialTime = "";
                    string sll = StartLat + "," + "-" + StartLong + "";
                    string[] Olist1 = new string[] { sll };
                    Olist = new List<Originslatlong>();
                    Olist.Clear();
                    Olist.Add(new Originslatlong
                    {
                        latitude = StartLat,
                        longitude = StartLong
                    });

                    if (tdt.Rows.Count > 0)
                    {
                        dlist = new List<Distinationslatlong>();
                        dlist.Clear();
                        foreach (DataRow dr in tdt.Rows)
                        {
                            dlist.Add(new Distinationslatlong
                            {
                                latitude = Convert.ToDouble(dr["TLati"]),
                                longitude = Convert.ToDouble(dr["TLong"])
                            });
                            arravialTime = Convert.ToString(dr["ModTime"]);
                        }
                    }

                    dslist.Add(new DistanceMatrixDetails
                    {
                        sfcode = sfcode,
                        departureTime = Start_Time,
                        origins = Olist.ToList(),
                        destinations = dlist.ToList(),
                        mode = "bike"
                    });
                }
            }

            return dslist.ToList();
        }

        [HttpGet]
        [Route("DistancelatlongDetail")]
        public List<ApiResponse> DistancelatlongDetail(string senderID, string Hdate, string DivCode, string sf_code)
        {

            List<Originslatlong> Olist = new List<Originslatlong>();
            List<Distinationslatlong> dlist = new List<Distinationslatlong>();

            List<DistanceMatrixDetails> dslist = new List<DistanceMatrixDetails>();

            DB_EReporting dbER = new DB_EReporting(senderID);

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DivCode", Convert.ToString(DivCode)),
                new SqlParameter("@Sf_code", Convert.ToString(sf_code)),
                new SqlParameter("@Hdate", Convert.ToString(Hdate))
            };


            DataSet ds = dbER.Exec_DataSetWithParam("LoginHistoryDistance1", parameters);
            DataTable tdt = new DataTable();
            DataTable dt = new DataTable();
            string waypoints = "";
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0]; tdt = ds.Tables[1];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sfcode = dt.Rows[i]["Sf_Code"].ToString();
                    //double StartLat = Convert.ToDouble(dt.Rows[i]["Start_Lat"]);
                    //double StartLong = Convert.ToDouble(dt.Rows[i]["Start_Long"]);
                    string Start_Time = Convert.ToString(dt.Rows[i]["Start_Time"]);
                    string StartLatLong = Convert.ToString(dt.Rows[i]["Startlatlong"]);
                    string EndLatLong = Convert.ToString(dt.Rows[i]["EndLatLong"]);

                    string arravialTime = "";

                    if (tdt.Rows.Count > 0)
                    {
                        string wayp = "";
                        dlist = new List<Distinationslatlong>();
                        dlist.Clear();
                        foreach (DataRow dr in tdt.Rows)
                        {
                            wayp += Convert.ToString(dr["TLong"]) + "|";

                            //waypoints += StartLatLong + "|" + Convert.ToString(wayp);
                        }

                        waypoints = StartLatLong + "|" + Convert.ToString(wayp);
                    }

                }
            }

            List<ApiResponse> resp = new List<ApiResponse>();

            resp.Add(new ApiResponse
            {
                StatusCode = 200,
                Message = "Succesfully get merchants",
                Data = waypoints
            });

            return resp;
        }

        [HttpGet]
        [Route("UpdateTravelDistance")]
        public async Task<string?> DistancelatlongDetail(string date)
        {
            DB_EReporting dbER = new DB_EReporting();

            string sSQl = "select Cust_DBName from master..Mas_Customers where Cust_Status = 0 and GeoDistance=1 group by Cust_DBName"; // 
            using (DataSet dsComp = dbER.Exec_DataSet(sSQl))
            {
                for (int il = 0; il < dsComp.Tables[0].Rows.Count; il++)
                {
                    DataRow rw = dsComp.Tables[0].Rows[il];
                    string? sCmp = rw["Cust_DBName"].ToString();
                    if (sCmp != null)
                    {
                        DB_EReporting dbusr = new DB_EReporting(sCmp.Replace("FMCG_",""));
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@dt", Convert.ToString(date))
                        };

                        using (DataSet ds = dbusr.Exec_DataSetWithParam("getWayPoints", parameters))
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                string wayp = Convert.ToString(dr["wayPoints"]) + "|";
                                string origin = wayp.Substring(0, wayp.IndexOf("|"));
                                wayp = wayp.Substring(wayp.IndexOf("|"), wayp.Length- (wayp.IndexOf("|")));

                                string revWayp = StrConverter.reverseStr(wayp);
                                revWayp = revWayp.TrimStart('|')+"|";
                                string destination = StrConverter.reverseStr(revWayp.Substring(0, revWayp.IndexOf("|")));
                                string waypoints = StrConverter.reverseStr(revWayp.Substring(revWayp.IndexOf("|"), revWayp.Length- revWayp.IndexOf("|")).Trim('|'));

                                if (origin != "" && destination != "")
                                {
                                    string sUrl = "https://api.nextbillion.io/directions/json?key=ca82b61e0b5243a6adfd31ab4256013d";
                                    string data = "{\"origin\":\"" + origin + "\",\"destination\":\"" + destination + "\",\"mode\": \"car\",\"waypoints\":\"" + waypoints + "\"}";
                                    Logger.WriteLogFile(sCmp.Replace("FMCG_", ""),"DAPI Call:" + data);
                                    HttpClient _httpClient = new HttpClient();
                                    _httpClient.DefaultRequestHeaders.Clear();

                                    var requestedData = new StringContent(data, Encoding.UTF8, "application/json");
                                    var responses = await _httpClient.PostAsync(sUrl, requestedData);
                                    if (responses.IsSuccessStatusCode)
                                    {
                                        string ApiData = await responses.Content.ReadAsStringAsync();
                                        jRepos dynObj = JsonConvert.DeserializeObject<jRepos>(ApiData);

                                        string sSQL = "delete from DistanceTbl where Sf_Code='" + dr["sf_code"] + "' and Pln_Date='" + dr["PlnDate"] + "'";
                                        dbusr.ExecQry(sSQL);

                                        string distance = dynObj.routes[0].distance;
                                        string LegsPoints = dynObj.routes[0].geometry;

                                        sSQL = "insert into DistanceReqs(sf,edt,dt,st,dest,wypt) select '" + dr["sf_code"] + "','" + dr["PlnDate"] + "',getdate(),'" + origin + "','" + destination + "','" + waypoints + "'";
                                        dbusr.ExecQry(sSQL);
                                        
                                        sSQL= "insert into DistanceTbl(Sf_Code,Pln_Date,UDistance,txRoute,sLatlng,eLatlng,roadline) select '" + dr["sf_code"] + "','" + dr["PlnDate"] + "','" + distance + "','" + waypoints + "','" + origin + "','" + destination + "','" + LegsPoints + "'";
                                        dbusr.ExecQry(sSQL);
                                    }
                                }
                                
                            }

                        }

                    }
                }
            }
            return "Completed";
        }
        [HttpGet]
        [Route("{sender}/UpdateTravelDistance")]
        public async Task<string?> updateDistanceDetailBySF(string sender,string SF,string date)
        {

            DB_EReporting dbusr = new DB_EReporting(sender);
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@SF", SF),
                new SqlParameter("@dt", Convert.ToString(date))
            };

            using (DataSet ds = dbusr.Exec_DataSetWithParam("getWayPointsBySF", parameters))
            {
                if (ds.Tables[0].Rows.Count<1)
                {
                    return "No Location Points";
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string wayp = Convert.ToString(dr["wayPoints"]) + "|";
                    string origin = wayp.Substring(0, wayp.IndexOf("|"));
                    wayp = wayp.Substring(wayp.IndexOf("|"), wayp.Length - (wayp.IndexOf("|")));

                    string revWayp = StrConverter.reverseStr(wayp);
                    revWayp = revWayp.TrimStart('|') + "|";
                    string destination = StrConverter.reverseStr(revWayp.Substring(0, revWayp.IndexOf("|")));
                    string waypoints = StrConverter.reverseStr(revWayp.Substring(revWayp.IndexOf("|"), revWayp.Length - revWayp.IndexOf("|")).Trim('|'));

                    if (origin != "" && destination != "")
                    {
                        string sUrl = "https://api.nextbillion.io/directions/json?key=ca82b61e0b5243a6adfd31ab4256013d";
                        string data = "{\"origin\":\"" + origin + "\",\"destination\":\"" + destination + "\",\"mode\": \"car\",\"waypoints\":\"" + waypoints + "\"}";
                        Logger.WriteLogFile(sender, $"Single {SF} DAPI Call:{data}");
                        HttpClient _httpClient = new HttpClient();
                        _httpClient.DefaultRequestHeaders.Clear();

                        var requestedData = new StringContent(data, Encoding.UTF8, "application/json");
                        var responses = await _httpClient.PostAsync(sUrl, requestedData);
                        if (responses.IsSuccessStatusCode)
                        {
                            string ApiData = await responses.Content.ReadAsStringAsync();
                            jRepos dynObj = JsonConvert.DeserializeObject<jRepos>(ApiData);

                            string sSQL = "delete from DistanceTbl where Sf_Code='" + dr["sf_code"] + "' and Pln_Date='" + dr["PlnDate"] + "'";
                            dbusr.ExecQry(sSQL);

                            string distance = dynObj.routes[0].distance;
                            string LegsPoints = dynObj.routes[0].geometry;

                            sSQL = "insert into DistanceReqs(sf,edt,dt,st,dest,wypt) select '" + dr["sf_code"] + "','" + dr["PlnDate"] + "',getdate(),'" + origin + "','" + destination + "','" + waypoints + "'";
                            dbusr.ExecQry(sSQL);

                            sSQL = "insert into DistanceTbl(Sf_Code,Pln_Date,UDistance,txRoute,sLatlng,eLatlng,roadline) select '" + dr["sf_code"] + "','" + dr["PlnDate"] + "','" + distance + "','" + waypoints + "','" + origin + "','" + destination + "','" + LegsPoints + "'";
                            dbusr.ExecQry(sSQL);
                            return "Updated";
                        }
                    }
                    else {

                        return "Invalid Start or End";
                    }

                }

            }

            return "Invalid Call";
        }
    }
    public class StrConverter {
        public static string reverseStr(string str)
        {

            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    public class jRepos
    {

        public string status { get; set; }
        public List<DistanceDetails> routes { get; set; }
    }
    public class DistanceDetails
    {

        public string geometry { get; set; }
        public string distance{ get; set; }
    }
}
