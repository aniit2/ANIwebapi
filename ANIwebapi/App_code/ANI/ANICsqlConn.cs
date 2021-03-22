
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace ANI.code
{
    class ANICsqlConn
    {
        MySqlConnection mycon;
        /// <summary>
        /// 打开mySQL
        /// </summary>
        /// 
        public string dbHost = "192.168.1.2";
        public string dbPort = "3306";
        public string dbUser = "root";//資料庫使用者帳號
        public string dbPass = "nfc25531";//資料庫使用者密碼
        public string dbName = "ani_smp_temp";//資料庫名稱
        public string db_testName = "azure_test";//資料庫名稱
        public bool is_test = false;
        public void OpenMySql()
        {
            Debug.Print(dbName);

            mycon = new MySqlConnection("server=" + dbHost + "; PORT= " + dbPort + " ;user id=" + dbUser + "; password=" + dbPass + "; database=" +(is_test == false?dbName:db_testName )+ "; CharSet=utf8mb4;Connection Timeout=3600");
            Debug.Print(dbName);
 
            //MySqlCommand command = conn.CreateCommand();

            //conn.Open();

            //string constr = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
            //MySqlConnection conn = new MySqlConnection(connStr);

            //mycon = new MySqlConnection(constr);
            mycon.Open();
        }


        public string testmysql() {
            string state = "";
            try
            {
                mycon = new MySqlConnection("server=" + dbHost + "; PORT= " + dbPort + " ;user id=" + dbUser + "; password=" + dbPass + "; database=" + dbName + "; CharSet=utf8");
                mycon.Open();
                state = "OK";
                //isConn = true;
            }
            catch (ArgumentException a_ex)
            {

                state = "NO";
            }
            catch (MySqlException ex)
            {

                state = "NO";
                switch (ex.Number)
                {
                    case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        break;
                    case 0: // Access denied (Check DB name,username,password)
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                if (mycon.State == ConnectionState.Open)
                {
                    mycon.Close();
                }

            }
            return state;
        }
        /// <summary>
        /// 关闭mySql
        /// </summary>
        public void CloseMySql()
        {
            mycon.Close();
        }
        /// <summary>
        /// 執行返回數據的存儲過程
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetProcedure(string ProcName, string[] Pars, string[] vals, List<int> lst)
        {
            DataSet ds = new DataSet();
            MySqlCommand sqlcom = new MySqlCommand();
            sqlcom.Connection = mycon;
            sqlcom.CommandText = ProcName;
            sqlcom.CommandType = CommandType.StoredProcedure;
            //设置参数，添加到数据库  
            for (int i = 0; i < Pars.Length; i++)
            {
                MySqlParameter par;
                if (lst != null && lst.Contains(i))
                    par = new MySqlParameter(Pars[i], MySqlDbType.VarChar, 2000);
                else
                    par = new MySqlParameter(Pars[i], MySqlDbType.VarChar, 20);
                par.Value = vals[i];
                sqlcom.Parameters.Add(par);
            }
            MySqlDataAdapter mysqlda = new MySqlDataAdapter(sqlcom);
            mysqlda.Fill(ds);
            mysqlda.Dispose();
            sqlcom.Dispose();
            return ds;
        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="ProcName">存儲過程名</param>
        /// <param name="Pars">參數</param>
        /// <param name="vals">值</param>
        public void ExecProcedure(string ProcName, string[] Pars, string[] vals, List<int> lst)
        {
            MySqlCommand sqlcom = new MySqlCommand();
            sqlcom.Connection = mycon;
            sqlcom.CommandText = ProcName;
            sqlcom.CommandType = CommandType.StoredProcedure;
            //设置参数，添加到数据库  
            for (int i = 0; i < Pars.Length; i++)
            {
                MySqlParameter par;
                if (lst != null && lst.Contains(i))
                    par = new MySqlParameter(Pars[i], MySqlDbType.VarChar, 2000);
                else
                    par = new MySqlParameter(Pars[i], MySqlDbType.VarChar, 20);
                par.Value = vals[i];
                sqlcom.Parameters.Add(par);
            }
            sqlcom.ExecuteNonQuery();
        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="ProcName">存儲過程名</param>
        /// <param name="Pars">參數</param>
        /// <param name="vals">值</param>
        public string ExecProcedure(string ProcName, string[] Pars, string[] vals, List<int> lst, int idx)
        {
            MySqlCommand sqlcom = new MySqlCommand();
            sqlcom.Connection = mycon;




            sqlcom.CommandText = ProcName;
            sqlcom.CommandType = CommandType.StoredProcedure;
            //设置参数，添加到数据库  
            for (int i = 0; i < Pars.Length; i++)
            {
                MySqlParameter par;
                if (lst != null && lst.Contains(i))
                    par = new MySqlParameter(Pars[i], MySqlDbType.VarChar, 2000);
                else
                    par = new MySqlParameter(Pars[i], MySqlDbType.VarChar, 20);
                if (i == idx)
                    par.Direction = ParameterDirection.Output;
                else
                    par.Value = vals[i];
                sqlcom.Parameters.Add(par);
            }
            sqlcom.ExecuteNonQuery();
            return sqlcom.Parameters[idx].Value.ToString();
        }
        public MySqlDataReader GetData(String sql)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mycon);
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            return reader;
        }

        public DataSet GetDataset(string sql)
        {
            DataSet ds = new DataSet();
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mycon);
            MySqlDataAdapter mysqlda = new MySqlDataAdapter(mySqlCommand);
            mysqlda.Fill(ds);
            mysqlda.Dispose();
            mySqlCommand.Dispose();
            return ds;
        }

        public int ExecSql(String sql)
        {
            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(sql, mycon);
                int ret = mySqlCommand.ExecuteNonQuery();
                mySqlCommand.Dispose();
                return ret;
            }
            catch (Exception)
            {

                return -1;
            }
        }
        public int ExecSql_transaction(String sql)
        {
            MySqlTransaction transaction = mycon.BeginTransaction();

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(sql, mycon, transaction);
                int ret = mySqlCommand.ExecuteNonQuery();
                transaction.Commit();
                mySqlCommand.Dispose();
                return ret;
            }

            catch (MySqlException ex)
            {

                //取消交易，復原至交易前
                transaction.Rollback();


                return -1;

                //列出訊息
            }

            catch (Exception)
            {
                transaction.Rollback();

                return -1;
            }
        }
        public int ExecSql_transaction2(String sql1, String sql2, bool checkSql1ExecutedResultCnt)
        {
            MySqlTransaction Master_trans = mycon.BeginTransaction();
            MySqlCommand mySqlCommand = new MySqlCommand(sql1, mycon, Master_trans);
            try
            {
                int ret = mySqlCommand.ExecuteNonQuery();
                if (string.IsNullOrWhiteSpace(sql2))
                {
                    //just only sql1 
                    Master_trans.Commit();
                    mySqlCommand.Dispose();
                }
                else if (checkSql1ExecutedResultCnt && ret > 0 || !checkSql1ExecutedResultCnt && ret >= 0)
                {
                    MySqlCommand mySqlCommand2 = new MySqlCommand(sql2, mycon, Master_trans);
                    ret = mySqlCommand2.ExecuteNonQuery();
                    Master_trans.Commit();
                    mySqlCommand2.Dispose();
                    mySqlCommand.Dispose();
                }
                else
                {
                    Master_trans.Rollback();
                    ret = -1;
                }
                return ret;
            }
            catch (MySqlException ex)
            {
                //取消交易，復原至交易前
                Master_trans.Rollback();
                return -1;
                //列出訊息
            }
            catch (Exception ex)
            {
                Master_trans.Rollback();
                return -1;
            }
        }
        public int ExecSql_with(string sql, byte[] bytecontent, string id)
        {
            try
            {

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mycon);



                MySqlParameter fileContentParameter = new MySqlParameter("?rawData", MySqlDbType.Blob, bytecontent.Length);

                fileContentParameter.Value = bytecontent;

                mySqlCommand.Parameters.Add(fileContentParameter);

                //mySqlCommand.Parameters.AddWithValue("@text_data", bytecontent);

                int ret = mySqlCommand.ExecuteNonQuery();
                mySqlCommand.Dispose();
                return ret;
            }
            catch (Exception)
            {

                return -1;
            }
        }


        public int ExecSqlBold(string sql, Image img, System.Drawing.Imaging.ImageFormat imgfmat)
        {
            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(sql, mycon);
                MemoryStream imageStream = new MemoryStream();
                img.Save(imageStream, imgfmat);
                byte[] imageByte = imageStream.ToArray();
                mySqlCommand.Parameters.Add(new MySqlParameter("?parval", MySqlDbType.Blob, imageByte.Length)).Value = imageByte;
                int ret = mySqlCommand.ExecuteNonQuery();
                mySqlCommand.Dispose();
                imageStream.Dispose();
                return ret;
            }
            catch (Exception e)
            {

                return -1;
            }
        }
        public int ExecSqlBold(string sql, byte[] bytes)
        {
            MySqlTransaction transaction = mycon.BeginTransaction();

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(sql, mycon, transaction);
                mySqlCommand.Parameters.Add(new MySqlParameter("?parval", MySqlDbType.Blob, bytes.Length)).Value = bytes;
                int ret = mySqlCommand.ExecuteNonQuery();
                transaction.Commit();
                mySqlCommand.Dispose();
                return ret;
            }

            catch (MySqlException ex)
            {

                //取消交易，復原至交易前
                transaction.Rollback();


                return -1;

                //列出訊息
            }

            catch (Exception)
            {
                transaction.Rollback();

                return -1;
            }
  
        }
    }
}
