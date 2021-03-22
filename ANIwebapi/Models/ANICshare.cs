
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

using System.Xml;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
 
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace Ani_Test_WebApi.Models
{

    class ANICshare
    {
        public static string sql_timeformat = "yyyy-MM-dd HH:mm:ss";
        public static string sql_timeformat_nosecond = "yyyy-MM-dd HH:mm";
        public static string sql_dateformat = "yyyy-MM-dd";

        public static string calandar_dateformat = "yyyy/MM/dd";
        public static string calandar_timeformat = "yyyy/MM/dd HH:mm";
        public ANICsqlConn conn = new ANICsqlConn();

        public string dbHost = "192.168.1.2";
        public string dbPort = "3306";
        public string dbUser = "root";//資料庫使用者帳號
        public string dbPass = "nfc25531";//資料庫使用者密碼
        public string dbName = "azure";//資料庫名稱
        public string db_testName = "azure_test";//資料庫名稱
        public bool set_test
        {
            set { conn.is_test = value; }
            get { return conn.is_test; }
        }
        public bool encrytion = true;

        public void refresh_connection()
        {
            conn.dbHost = dbHost;
            conn.dbPort = dbPort;
            conn.dbUser = dbUser;
            conn.dbPass = dbPass;
            conn.dbName = dbName;
            conn.db_testName = db_testName;
        }


        /// <summary>
        /// 执行SQL语句 返回执行结果
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int EcxeSql(string sql)
        {

            conn.OpenMySql();
            System.Diagnostics.Trace.WriteLine(sql);
            int ret = conn.ExecSql(sql);
            conn.CloseMySql();
            return ret;
        }
        public int Ecxe_transaction_Sql(string sql)
        {

            conn.OpenMySql();
            System.Diagnostics.Trace.WriteLine(sql);
            Debug.Print(sql);
            int ret = conn.ExecSql_transaction(sql);
            conn.CloseMySql();
            return ret;
        }
        public bool bool_Ecxe_transaction_Sql(string sql)
        {

            conn.OpenMySql();
            System.Diagnostics.Trace.WriteLine(sql);
            Debug.Print(sql);
            int ret = conn.ExecSql_transaction(sql);
            conn.CloseMySql();
            if (ret > 0)
            {
                return true;
            }
            return false;

        }
        public void ExecBoldSql(string sql, System.Drawing.Image img, System.Drawing.Imaging.ImageFormat imgfmat)
        {

            conn.OpenMySql();
            conn.ExecSqlBold(sql.ToString(), img, imgfmat);
            conn.CloseMySql();
        }
        public int ExecBoldSql(string sql, string base64_str)
        {
            byte[] bytes = Convert.FromBase64String(base64_str);
            
            conn.OpenMySql();
            int i = conn.ExecSqlBold(sql.ToString(), bytes);
            conn.CloseMySql();
            return i;
        }
        public int ExecbyteBoldSql(string sql, string imageplace, byte[] image_data)
        {

            conn.OpenMySql();
            //int insert = conn.ExecSql_with(sql.ToString(), imageplace, image_data);
            int insert = 0;
            conn.CloseMySql();
            return insert;
        }
        /// <summary\>
        /// 执行SQL查询，获得返回集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string testdatabase(string sql)
        {
            return conn.testmysql();
        }

        public DataSet GetDataSource(string sql)
        {

            DataSet ds;
            Debug.Print(sql);
            System.Diagnostics.Trace.WriteLine(sql);
            conn.OpenMySql();
            ds = conn.GetDataset(sql);
            conn.CloseMySql();
            return ds;
        }


        /// <summary>
        /// 執行存儲過程，返回數據
        /// </summary>
        /// <param name="proANIme"></param>
        /// <param name="pars"></param>
        /// <param name="vals"></param>
        /// <param name="lst"></param>
        public DataSet GetProcedure(string proANIme, string[] pars, string[] vals, List<int> lst)
        {
            DataSet ds;

            conn.OpenMySql();
            ds = conn.GetProcedure(proANIme, pars, vals, lst);
            conn.CloseMySql();
            return ds;
        }
        /// <summary>
        /// 执行存儲過程
        /// </summary>
        /// <param name="proANIme"></param>
        /// <param name="pars"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public void ExecProcedure(string proANIme, string[] pars, string[] vals, List<int> lst)
        {

            conn.OpenMySql();
            conn.ExecProcedure(proANIme, pars, vals, lst);
            conn.CloseMySql();
        }

        public string ExecRetProcedure(string proANIme, string[] pars, string[] vals, List<int> lst, int idx)
        {

            conn.OpenMySql();
            string ret = conn.ExecProcedure(proANIme, pars, vals, lst, idx);
            conn.CloseMySql();
            return ret;
        }

        public string testmysql()
        {
            return conn.testmysql();

        }




        /// <summary>
        /// 得到表記錄
        /// </summary>
        /// <param name="Tabname"></param>
        /// <returns></returns>
        public DataSet GetTableData(String Tabname)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("Select tchi_value from {0}", Tabname);
            return GetDataSource(sql.ToString());
        }
        /// <summary>
        /// 得到插入字符
        /// </summary>
        /// <param name="TabName"></param>
        /// <returns></returns>


        public List<string> Get_SQL_item(string table_name, string coloum)
        {
            List<string> item_list = new List<string>();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select {1} from {0} where valid = 'Y';", table_name, coloum);
            DataSet ds = GetDataSource(sql.ToString());
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                item_list.Add(item[0].ToString());
            }
            return item_list;
        }
        public string GetInsertTableString(string TabName)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("Insert {0}(tchi_value)", TabName);
            sql.Append(" values('{0}')");
            return sql.ToString();
        }
        /// <summary>
        /// 限制只允许输入保留小数点两位的数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        //陽歷轉農歷
        // System.Globalization.ChineseLunisolarCalendar cCal = new ChineseLunisolarCalendar();
        public string Get_mysql_database_MaxID_ordering(string database_name, string coloumn_name, string condition = "")
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select max(" + coloumn_name + ")+1 from " + database_name);
            if (condition.Length > 0)
            {
                sql.AppendFormat(" where {0} ", condition);
            }
            DataSet ds = GetDataSource(sql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (!ds.Tables[0].Rows[0][0].ToString().Equals(""))
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "1";
                }
            }
            else
            {
                return "1";
            }
        }
        public string Get_mysql_database_MaxID(string database_name, string coloumn_name)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select max(" + coloumn_name + ")+1 from " + database_name);
            DataSet ds = GetDataSource(sql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (!ds.Tables[0].Rows[0][0].ToString().Equals(""))
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "100000";
                }
            }
            else
            {
                return "100000";
            }
        }
        public string Get_mysql_trans_payment_MaxID(string database_name, string coloumn_name)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select max(" + coloumn_name + ")+1 from " + database_name + " where " + coloumn_name + ">= 800000");
            DataSet ds = GetDataSource(sql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (!ds.Tables[0].Rows[0][0].ToString().Equals(""))
                {

                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "800000";
                }
            }
            else
            {
                return "800000";
            }
        }
        public string Get_mysql_database_value(string database_table, string id_coloumn_name, string id, string target_coloum, string condition = "")
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select " + target_coloum + " from " + database_table + " where " + id_coloumn_name + " = '" + id + "' " + condition);
            DataSet ds = GetDataSource(sql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (!ds.Tables[0].Rows[0][0].ToString().Equals(""))
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public string get_current_time()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select DATE_FORMAT(now(), '%Y-%m-%d %H:%i:%s');");
            DataSet ds = GetDataSource(sql.ToString());
            string time = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                time = ds.Tables[0].Rows[0][0].ToString();
            }


            //string time = Cshare.get_current_datetime().ToString("yyyy-MM-dd HH:mm:ss");
            return time;
        }
        public DateTime get_current_datetime()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select DATE_FORMAT(now(), '%Y-%m-%d %H:%i:%s');");
            DataSet ds = GetDataSource(sql.ToString());
            string time = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                DateTime dt = DateTime.ParseExact(ds.Tables[0].Rows[0][0].ToString(), "yyyy-MM-dd HH:mm:ss", null);
                return dt;
                //DateTime.ParseExact
            }
            else
            {
                return DateTime.Now;
            }
        }

        public static string add_comma(string value)
        {
            if (value.Length > 0)
            {
                value = value + ";";
            }
            return value;
        }
        public static string add_space(string value)
        {
            if (value.Length > 0)
            {
                value = " " + value;
            }
            return value;
        }


        public static void trace_value(object value)
        {
            System.Diagnostics.Trace.WriteLine(value);
        }
        public string[] obj_arr_to_string_arr(object[] obj)
        {
            string[] arr = new string[obj.Count()];
            for (int i = 0; i < obj.Count(); i++)
            {
                arr[i] = encry_value(obj[i].ToString());
            }
            return arr;
        }


        public object[] encry_value(object[] value)
        {
            if (encrytion)
            {
                object[] new_values = new object[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    new_values[i] = Encrypt(value[i].ToString());
                }
                return new_values;
            }
            else
            {
                return value;
            }

        }
        public List<string> encry_value(List<string> list)
        {
            if (encrytion)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = Encrypt(list[i]);
                }
                return list;
            }
            else
            {
                return list;
            }

        }
        public string encry_value(string value)
        {

            try
            {


                if (encrytion)
                {
                    string new_value = Encrypt(value);
                    return new_value;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        public string[] encry_value(string[] value)
        {

            try
            {


                if (encrytion)
                {

                    for (int i = 0; i < value.Length; i++)
                    {
                        value[i] = Encrypt(value[i]);
                    }
                    return value;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        public string dencry_value(string value)
        {
            try
            {


                if (encrytion)
                {
                    string new_value = Decrypt(value);
                    return new_value;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception)
            {

                return null;
            }

        }
        public static String key = "ANIEncryptionLibxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        public static string keyStr = "ANIEncryptionLibxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        private static string Encrypt(string PlainText)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;

            // It is equal in java 
            /// Cipher _Cipher = Cipher.getInstance("AES/CBC/PKCS5PADDING");    
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] keyArr = Convert.FromBase64String(keyStr);
            byte[] KeyArrBytes32Value = new byte[32];
            Array.Copy(keyArr, KeyArrBytes32Value, 32);

            // Initialization vector.   
            // It could be any value or generated using a random number generator.
            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, 16);

            aes.Key = KeyArrBytes32Value;
            aes.IV = IVBytes16Value;

            ICryptoTransform encrypto = aes.CreateEncryptor();

            byte[] plainTextByte = ASCIIEncoding.UTF8.GetBytes(PlainText);
            byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
            return Convert.ToBase64String(CipherText);

        }

        private static string Decrypt(string CipherText)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] keyArr = Convert.FromBase64String(keyStr);
            byte[] KeyArrBytes32Value = new byte[32];
            Array.Copy(keyArr, KeyArrBytes32Value, 32);

            // Initialization vector.   
            // It could be any value or generated using a random number generator.
            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, 16);

            aes.Key = KeyArrBytes32Value;
            aes.IV = IVBytes16Value;

            ICryptoTransform decrypto = aes.CreateDecryptor();

            byte[] encryptedBytes = Convert.FromBase64CharArray(CipherText.ToCharArray(), 0, CipherText.Length);
            byte[] decryptedData = decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return ASCIIEncoding.UTF8.GetString(decryptedData);
        }

        public static DataRow Get_dataset_row_by_id(DataTable ds, int column_index, string input)
        {
            string value = "";

            for (int i = 0; i < ds.Rows.Count; i++)
            {
                if (ds.Rows[i][column_index].ToString() == input)
                {
                    return ds.Rows[i];

                }
            }

            return null;
        }

        public static string Null_date_check(string datetime)
        {
            string result = datetime == "1900/05/16" ? "" : datetime;

            return result;

        }
        public static string Null_date_check_hyphen(string datetime)
        {
            string result = datetime == "1900-05-16" ? "" : datetime;

            return result;

        }
        public static string Null_time_check_no_second_hyphen(string datetime)
        {
            string result = datetime == "1900-05-16 00:00" ? "" : datetime;

            return result;

        }
        public static string Null_time_check_no_second(string datetime)
        {
            string result = datetime == "1900/05/16 00:00" ? "" : datetime;

            return result;

        }
        public static string Null_time_check(string datetime)
        {
            string result = datetime == "1900/05/16 00:00:00" ? "" : datetime;

            return result;

        }
    }
}
