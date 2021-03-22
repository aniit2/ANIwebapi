using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Ani_Test_WebApi.Models
{
    public class CExecSql
    {
        public CExecSql()
        {

        }

        public bool ClientLoginOut(byte[] ImgBytes)
        {
            //定義Sql查詢語句
            StringBuilder sql = new StringBuilder();
            sql.Append("Update sqlLoginInHistory set OutDateTime=Now() Where ID=@ID;");

            //定義mysql
            CSqlConn sqlcon = new CSqlConn();
            sqlcon.OpenMySql();

            //定義Mysql命令
            MySqlCommand cmd = new MySqlCommand(sql.ToString());

            //獲取執行mySql語句返回值
            int ret = sqlcon.ExecSql(cmd);

            //關閉Mysql 
            sqlcon.CloseMySql();

            //返回值
            return true;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="photoID"></param>
        /// <param name="imgBytes"></param>
        public void SaveImage(string photoID, byte[] imgBytes)
        {
            StringBuilder sql = new StringBuilder();
            if (photoID == null || photoID.Length < 1)
                sql.Append("Insert into client_documents2(client_photo_id, document_photo) Values(@PhotoID,?parval);");
            else
                sql.Append("Update client_documents2 set document_photo=?parval  where client_photo_id=@photoID;");
            //定義mysql
            CSqlConn sqlcon = new CSqlConn();
            sqlcon.OpenMySql();

            MySqlCommand cmd = new MySqlCommand(sql.ToString());
            cmd.Parameters.Add(new MySqlParameter("@PhotoID", MySqlDbType.String)).Value = photoID;
            // cmd.Parameters.Add(new MySqlParameter("@ClientID", MySqlDbType.String)).Value = clientID;
            cmd.Parameters.Add(new MySqlParameter("?parval", MySqlDbType.Blob, imgBytes.Length)).Value = imgBytes;

            //獲取執行mySql語句返回值
            int ret = sqlcon.ExecSql(cmd);

            //關閉Mysql 
            sqlcon.CloseMySql();
        }

        public byte[] GetImage(string photoID)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("Select document_photo from client_documents2 ");
            sql.Append(" where client_photo_id =@PhotoID ");

            //定義mysql
            CSqlConn sqlcon = new CSqlConn();
            sqlcon.OpenMySql();

            MySqlCommand cmd = new MySqlCommand(sql.ToString());
            cmd.Parameters.Add(new MySqlParameter("@PhotoID", MySqlDbType.String)).Value = photoID;
            DataSet ds = sqlcon.GetDataset(cmd);

            //關閉Mysql 
            sqlcon.CloseMySql();

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0].GetType().Equals(typeof(byte[])))
                {
                    return (byte[])ds.Tables[0].Rows[0][0];
                }
                else
                    return null;
            }
            return null;
        }
    }
}