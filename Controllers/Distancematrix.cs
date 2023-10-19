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


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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



        //[HttpGet]        
        //[Route("TranslatlongDetails")]
        //public List<TransHistoryDistance> TranslatlongDetails(string senderID, string Hdate, string DivCode)
        //{
        //    List<TransHistoryDistance> rd = new List<TransHistoryDistance>();

        //    DB_EReporting dbER = new DB_EReporting(senderID);
        //    SqlParameter[] parameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@Hdate", Convert.ToString(Hdate)),
        //        new SqlParameter("@DivCode", Convert.ToString(DivCode))
        //    };

        //    DataSet ds = dbER.Exec_DataSetWithParam("TransLoginHistoryDistance", parameters);

        //    if (ds.Tables.Count > 0)
        //    {
        //        DataTable dt = ds.Tables[0];

        //        if (dt.Rows.Count > 0)
        //        {
        //            foreach (DataRow dtRow in dt.Rows)
        //            {
        //                rd.Add(new TransHistoryDistance
        //                {
        //                    Sf_Code = Convert.ToString(dtRow["Sf_Code"]),
        //                    ListedDrCode = Convert.ToInt32(dtRow["ListedDrCode"]),
        //                    ListedDrName = Convert.ToString(dtRow["ListedDr_Name"]),
        //                    ModTime = Convert.ToString(dtRow["ModTime"]),
        //                    TLati = Convert.ToDouble(dtRow["TLati"]),
        //                    TLong = Convert.ToDouble(dtRow["TLong"]),
        //                    POBValue = Convert.ToDouble(dtRow["POB_Value"])
        //                });
        //            }
        //        }
        //    }

        //    return rd.ToList();
        //}

        //POST api/<Distancematrix>

        //[HttpPost]
        //[Route("DistanceDetails")]
        //public void DistanceDetails(string departure_time, string origins, string destinations, string mode)
        //{

        //}



        //// PUT api/<Distancematrix>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<Distancematrix>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }


}
