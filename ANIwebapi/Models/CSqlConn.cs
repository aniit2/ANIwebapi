using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Ani_Test_WebApi.Models
{
    public class CSqlConn
    {
        static string  SqlConnectString = "server=localhost;port=3366;User Id=root;charset=utf8;password=nfc25531;Database=anidemo;SslMode=none;";
        public CSqlConn()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            
        }
        MySqlConnection mycon;

        
        /// <summary>
        /// 连接Autoupdate库
        /// </summary>
        public static string SqlConnectStringUpdate { set; get; }

        /// <summary>
        /// 打開mysql數據庫
        /// </summary>
        public void OpenMySql()
        {
            mycon = new MySqlConnection(SqlConnectString);
            mycon.Open();
        }
        /// <summary>
        /// 打開自动更新數據庫
        /// </summary>
        public void OpenMySqlAotoUpdate()
        {
            mycon = new MySqlConnection(SqlConnectStringUpdate);
            mycon.Open();
        }
        /// <summary>
        /// 打開客戶端mysql數據庫
        /// </summary>
        /// <param name="SqlClientConnectString"></param>
        public void OpenMySql(string SqlClientConnectString)
        {
            mycon = new MySqlConnection(SqlClientConnectString);
            mycon.Open();
        }

        /// <summary>
        /// 关闭mySql
        /// </summary>
        public void CloseMySql()
        {
            mycon.Close();
            mycon.Dispose();
        }

        /// <summary>
        /// 得到查詢數據集
        /// </summary>
        /// <param name="mySqlCmd"></param>
        /// <returns></returns>
        public DataSet GetDataset(MySqlCommand mySqlCmd)
        {
            DataSet ds = new DataSet();
            mySqlCmd.Connection = mycon;
            MySqlDataAdapter mysqlda = new MySqlDataAdapter(mySqlCmd);
            mysqlda.Fill(ds);
            mysqlda.Dispose();
            mySqlCmd.Dispose();
            return ds;
        }

        /// <summary>
        /// 執行Sql命令
        /// </summary>
        /// <param name="mySqlCmd"></param>
        /// <returns></returns>
        public int ExecSql(MySqlCommand mySqlCmd)
        {
            MySqlTransaction transaction = mycon.BeginTransaction();
            try
            {
                mySqlCmd.Connection = mycon;
                mySqlCmd.Transaction = transaction;
                int ret = mySqlCmd.ExecuteNonQuery();
                transaction.Commit();
                return ret;
            }
            catch
            {
                transaction.Rollback();
                return -1;
            }
        }
    }
}