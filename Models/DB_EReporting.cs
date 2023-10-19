using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApplicationApi.Models
{
    public class DB_EReporting
    {
        ConfigurationManager Configuration = new ConfigurationManager();

        //private string strConn = System.Configuration.ConfigurationManager.AppSettings["Ereportcon"];
        //string connString = ConfigurationExtensions.GetConnectionString(Configuration, "AppDb");
        //private string strConn = System.Configuration.ConfigurationSettings.ConnectionStrings["Ereportcon"].ToString();

        private int iReturn = -1;
        private string iReturn1 = "";
        private string strConn = "";

        public DB_EReporting(string SenderID)
        {
            string sUSR = SenderID.ToLower();
            string sServerIP = "37.61.220.198,1433";
            string ServUSerID = "sa";
            string ServPWD = "sG3jMzft?9wPV8N";
            string DBName = "FMCG_Live";
            if (sUSR == "dev")
            {
                sServerIP = "37.61.220.198,1433";
                ServUSerID = "sa";
                ServPWD = "sG3jMzft?9wPV8N";
                DBName = "FMCG_Dev";
            }
            else
            {
                if (sUSR == "fmcg")
                {
                    DBName = "FMCG_Live";
                }
                else if (sUSR == "arasan")
                {
                    DBName = "FMCG_ArasanSanitry";
                }
                else if (sUSR == "tiesar")
                {
                    DBName = "FMCG_TSR";
                }
                else if (sUSR == "pgdb")
                {
                    DBName = "FMCG_BGDB";
                }
                else if (sUSR == "pgkala")
                {
                    DBName = "FMCG_Kala";
                }
                else if (sUSR == "allen")
                {
                    DBName = "FMCG_AllenLab";
                }
                else if (sUSR == "afripipe")
                {
                    DBName = "FMCG_Afripipes";
                }
                else
                {
                    DBName = "FMCG_" + sUSR.ToUpper();
                }
                sServerIP = "13.200.61.175,10433";
                //sServerIP = "10.0.2.50";
                ServUSerID = "sa";
                ServPWD = "SanMedia#123";
            }
            strConn = "Server=" + sServerIP + ";Database=" + DBName + "; User ID=" + ServUSerID + "; pwd=" + ServPWD;
        }
        public DataTable Exec_DataTable(string strQry)
        {
            DataSet ds_EReport = new DataSet();
            DataTable dt_EReport = new DataTable();

            try
            {
                ds_EReport = Exec_DataSet(strQry);
                dt_EReport = ds_EReport.Tables[0];
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return dt_EReport;
        }

        public DataSet MExec_DataSet(string strQry)
        {
            DataSet ds_EReport = new DataSet();

            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    SqlCommand selectCMD = new SqlCommand(strQry, _conn);
                    selectCMD.CommandTimeout = 120;

                    SqlDataAdapter da_EReport = new SqlDataAdapter();
                    da_EReport.SelectCommand = selectCMD;
                    if (_conn.State == ConnectionState.Closed)
                    {
                        _conn.Open();

                        da_EReport.Fill(ds_EReport);
                    }

                    // _conn.Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close();
                        _conn.Dispose();
                    }

                }
            }
            return ds_EReport;
        }

        public DataSet Exec_DataSet(string strQry)
        {
            DataSet ds_EReport = new DataSet();

            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    SqlCommand selectCMD = new SqlCommand(strQry, _conn);
                    selectCMD.CommandTimeout = 120;

                    SqlDataAdapter da_EReport = new SqlDataAdapter();
                    da_EReport.SelectCommand = selectCMD;
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(selectCMD))
                        {
                            da.Fill(ds_EReport);
                        }
                    }
                    //if (_conn.State == ConnectionState.Closed)
                    //{
                    //    _conn.Open();

                    //    da_EReport.Fill(ds_EReport, "Customers");
                    //}

                    // _conn.Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close();
                        _conn.Dispose();
                    }

                }
            }
            return ds_EReport;
        }

        public string InvoiceRowExecQry(string ItemCode, string ItemName, string BatchNum, string Quantity, string Price, string PackSize, string UoM,
         string TaxCode, string TaxRate)
        {
            string msg = string.Empty;

            iReturn = -1;
            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    System.Data.SqlClient.SqlCommand cmd;

                    string query = "InsertRowDetails";
                    //_conn.Open();
                    cmd = new System.Data.SqlClient.SqlCommand(query, _conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ItemCode", ItemCode.ToString());
                    cmd.Parameters.AddWithValue("@ItemName", ItemName.ToString());
                    cmd.Parameters.AddWithValue("@BatchNum", BatchNum.ToString());
                    cmd.Parameters.AddWithValue("@Quantity", Quantity.ToString());
                    cmd.Parameters.AddWithValue("@Price", Price.ToString());
                    cmd.Parameters.AddWithValue("@PackSize", PackSize.ToString());
                    cmd.Parameters.AddWithValue("@UoM", UoM.ToString());
                    cmd.Parameters.AddWithValue("@TaxCode", TaxCode.ToString());
                    cmd.Parameters.AddWithValue("@TaxRate", TaxRate.ToString());
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn = cmd.ExecuteNonQuery();
                        //_conn.Close();
                        msg = "Success";
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close();
                        _conn.Dispose();
                    }

                }
            }
            return msg;
        }

        public int PriRowExecQry(string ItemCode, string ItemName, string BatchNum, string Quantity, string Price, string PackSize, string UoM,
         string TaxCode, string TaxRate)
        {
            iReturn = -1;
            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    System.Data.SqlClient.SqlCommand cmd;

                    string query = "InsertPriOrderRowDetails";
                    cmd = new System.Data.SqlClient.SqlCommand(query, _conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ItemCode", ItemCode.ToString());
                    cmd.Parameters.AddWithValue("@ItemName", ItemName.ToString());
                    cmd.Parameters.AddWithValue("@BatchNum", BatchNum.ToString());
                    cmd.Parameters.AddWithValue("@Quantity", Quantity.ToString());
                    cmd.Parameters.AddWithValue("@Price", Price.ToString());
                    cmd.Parameters.AddWithValue("@PackSize", PackSize.ToString());
                    cmd.Parameters.AddWithValue("@UoM", UoM.ToString());
                    cmd.Parameters.AddWithValue("@TaxCode", TaxCode.ToString());
                    cmd.Parameters.AddWithValue("@TaxRate", TaxRate.ToString());
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn = cmd.ExecuteNonQuery();
                    }
                    // _conn.Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close(); _conn.Dispose();
                    }
                }
            }
            return iReturn;
        }

        public DataSet Exec_DataSet(string strQry, SqlCommand cmd)
        {
            DataSet ds_EReport = new DataSet();

            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    //SqlConnection _conn = new SqlConnection(strConn);

                    //SqlCommand selectCMD = new SqlCommand(strQry, _conn);
                    cmd.Connection = _conn;
                    cmd.CommandText = strQry;
                    cmd.CommandTimeout = 30;

                    SqlDataAdapter da_EReport = new SqlDataAdapter();
                    da_EReport.SelectCommand = cmd;
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();

                        da_EReport.Fill(ds_EReport, "Customers");
                    }

                    //_conn.Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close(); _conn.Dispose();
                    }
                }
            }
            return ds_EReport;
        }

        public DataSet Exec_DataSetWithParam(string CommandName, SqlParameter[] param)
        {
            DataSet table = new DataSet();

            using (SqlConnection con = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //cmd.CommandType = cmdType;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(param);

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(table);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close(); con.Dispose();
                        }
                    }
                }
            }

            return table;
        }

        public int Exec_Scalar(string strQry)
        {
            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    iReturn = -1;
                    // SqlConnection _conn = new SqlConnection(strConn);
                    SqlCommand selectCMD = new SqlCommand(strQry, _conn);
                    selectCMD.CommandTimeout = 30;
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn = Convert.ToInt32(selectCMD.ExecuteScalar());
                    }
                    // _conn.Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close(); _conn.Dispose();
                    }
                }
            }
            return iReturn;
        }

        public string Exec_Scalar_s(string strQry)
        {
            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    iReturn1 = "";
                    // SqlConnection _conn = new SqlConnection(strConn);
                    SqlCommand selectCMD = new SqlCommand(strQry, _conn);
                    selectCMD.CommandTimeout = 30;
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn1 = Convert.ToString(selectCMD.ExecuteScalar());
                    }
                    // _conn.Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close(); _conn.Dispose();
                    }
                }
            }
            return iReturn1;
        }

        public string SP_Exec_NonQueryWithParam(string CommandName, SqlParameter[] pars)
        {
            int result = 0; string msg = "";

            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlTransaction Trans = null;
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(pars);

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                            //Trans = con.BeginTransaction();
                            result = cmd.ExecuteNonQuery();
                            //Trans.Commit();
                        }
                        if (result > 0)
                        {
                            msg = "Success";
                        }
                        else
                        {
                            msg = "Error";
                        }
                    }
                    catch (Exception ex)
                    {
                        //Trans.Rollback();
                        msg = "Error : " + ex.Message.ToString();
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                            con.Dispose();
                        }
                    }
                }
            }

            return msg;
        }

        public int Exec_Scalar(string strQry, SqlCommand cmd)
        {
            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    iReturn = -1;
                    //SqlConnection _conn = new SqlConnection(strConn);
                    //SqlCommand selectCMD = new SqlCommand(strQry, _conn);
                    cmd.Connection = _conn;
                    cmd.CommandText = strQry;
                    cmd.CommandTimeout = 30;
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    //_conn.Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close();
                        _conn.Dispose();
                    }
                }
            }
            return iReturn;
        }

        public string SExecQry(string sQry)
        {
            string msg = string.Empty;
            iReturn = -1;
            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    //SqlConnection _conn = new SqlConnection(strConn);
                    System.Data.SqlClient.SqlCommand cmd;
                    cmd = new System.Data.SqlClient.SqlCommand();
                    cmd.Connection = _conn;
                    cmd.CommandText = sQry;
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn = cmd.ExecuteNonQuery();
                    }

                    msg = "Success";
                }
                catch (Exception ex)
                {
                    msg = ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close();
                        _conn.Dispose();
                    }

                }
            }
            return msg;
        }

        public int ExecQry(string sQry)
        {
            iReturn = -1;
            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    //SqlConnection _conn = new SqlConnection(strConn);
                    System.Data.SqlClient.SqlCommand cmd;
                    cmd = new System.Data.SqlClient.SqlCommand();
                    cmd.Connection = _conn;

                    cmd.CommandText = sQry;

                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn = cmd.ExecuteNonQuery();
                    }
                    // _conn.Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    {
                        _conn.Close();
                        _conn.Dispose();
                    }
                }
            }
            return iReturn;
        }

        public int ExecQry(string sQry, SqlCommand cmd)
        {
            iReturn = -1;
            using (SqlConnection _conn = new SqlConnection(strConn))
            {
                try
                {
                    // SqlConnection _conn = new SqlConnection(strConn);
                    //System.Data.SqlClient.SqlCommand cmd;
                    //cmd = new System.Data.SqlClient.SqlCommand();
                    cmd.Connection = _conn;
                    cmd.CommandText = sQry;
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn = cmd.ExecuteNonQuery();
                    }
                    // _conn.Close();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    { _conn.Close(); _conn.Dispose(); }
                }
            }
            return iReturn;
        }

        public int ExecQry(string sQry, SqlConnection _conn, SqlTransaction tran)
        {
            iReturn = -1;
            using (_conn = new SqlConnection(strConn))
            {
                try
                {
                    //SqlConnection _conn = new SqlConnection(strConn);
                    System.Data.SqlClient.SqlCommand cmd;
                    cmd = new System.Data.SqlClient.SqlCommand();
                    cmd.Connection = _conn;
                    cmd.CommandText = sQry;
                    cmd.Transaction = tran;
                    //_conn.Open();
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn = cmd.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    { _conn.Close(); _conn.Dispose(); }
                }
            }
            return iReturn;
        }

        public int Exec_Scalar(string strQry, SqlConnection _conn, SqlTransaction tran)
        {
            using (_conn = new SqlConnection(strConn))
            {
                try
                {
                    iReturn = -1;
                    //SqlConnection _conn = new SqlConnection(strConn);
                    SqlCommand selectCMD = new SqlCommand(strQry, _conn, tran);
                    selectCMD.CommandTimeout = 30;
                    if (_conn.State != ConnectionState.Open)
                    {
                        _conn.Open();
                        iReturn = Convert.ToInt32(selectCMD.ExecuteScalar());
                    }
                    //_conn.Close();
                }
                catch (Exception ex) { ex.Message.ToString(); }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                    { _conn.Close(); _conn.Dispose(); }
                }
            }
            return iReturn;
        }

        internal DataTable Exec_DataTableWithParam(string CommandName, CommandType cmdType, SqlParameter[] param)
        {
            DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(param);

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(table);
                            }
                        }
                    }
                    catch (Exception ex) { ex.Message.ToString(); }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        { con.Close(); con.Dispose(); }
                    }
                }
            }

            return table;
        }

        internal bool Exec_NonQueryWithParam(string CommandName, CommandType cmdType, SqlParameter[] pars)
        {
            int result = 0;

            using (SqlConnection con = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(pars);

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                            result = cmd.ExecuteNonQuery();
                        }

                    }
                    catch (Exception ex) { ex.Message.ToString(); }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        { con.Close(); con.Dispose(); }
                    }
                }
            }

            return (result > 0);
        }
    }
}
