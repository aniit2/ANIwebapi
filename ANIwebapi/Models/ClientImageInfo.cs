using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Ani_Test_WebApi.Models
{
    public class ClientImageInfo
    {

        public string ClientID { get; set; }
        public string PhotoID { get; set; }
        public byte[] PhotoData { get; set; }

        public string dbHost = "192.168.1.17";
        public string dbPort = "3366";
        public string dbUser = "anidemo";//資料庫使用者帳號
        public string dbPass = "nfc22531";//資料庫使用者密碼
        public string dbName = "anidemo";//資料庫名稱
        public string db_testName = "anidemo_test";//資料庫名稱

        public string LoginName = "100004";//資料庫名稱
        public string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱


        static string uploadPath = HttpContext.Current.Server.MapPath("~/App_Data/");
        public String CreateNewFile(String WriteString)
        {
            string ret = string.Empty;
            try
            {
                // 如果目录不存在则要先创建
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                //得到文件路径和名
                string FilePath = String.Format("{2}AniTemp_{1}_{0}.txt", dbName, DateTime.Now.Ticks, uploadPath);

                if (File.Exists(FilePath)) File.Delete(FilePath);
                //创建一个新的文件，
                File.WriteAllText(FilePath, WriteString);

                ret = FilePath;
            }
            catch (Exception e) { ret = e.Message.ToString(); }
            return ret;
        }

        /// <summary>
        /// 追加到指定文件末尾
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="WriteString"></param>
        /// <returns></returns>
        public String AppandToFile(String FilePath, string startIndex, String WriteString)
        {
            int index = 0;
            int.TryParse(startIndex, out index);
            string ret = string.Empty;
            if (File.Exists(FilePath))
            {
                FileInfo info = new System.IO.FileInfo(FilePath);
                if (info.Length <= index)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath, true))
                    {
                        file.Write(WriteString);//直接追加文件末尾，不换行
                    }
                    ret = info.Length + " VS " + index;
                }
                else
                    ret = info.Length + " VS " + index;
            }
            else
                ret = "not Existst ->" + FilePath;
            return ret;
        }

        /// <summary>
        /// 完成上传文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="imageID"></param>
        /// <param name="WriteString"></param>
        /// <returns></returns>
        public String CmpleteUpLoadFile(String FilePath, String mysqlID, String examainedby, String WriteString)
        {
            using (StreamReader sr = new StreamReader(FilePath))
            {
                WriteMysql(mysqlID, examainedby, sr.ReadToEnd() + WriteString);
                sr.Close();
            };
            try
            {
                //删除上传文件
                File.Delete(FilePath);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "True";
        }

        /// <summary>
        /// 写入Mysql数据
        /// </summary>
        /// <param name="examinedby"></param>
        /// <param name="WriteString"></param>
        public void WriteMysql(string observation_id,
            string examined_by, String WriteString)
        {



            Encription encription = new Encription();
            ANICshare share = new ANICshare();




            share.dbHost = dbHost;
            share.dbPort = dbPort;
            share.dbUser = dbUser;
            share.dbPass = dbPass;
            share.dbName = dbName;
            share.db_testName = db_testName;
            share.refresh_connection();





            //    CExecSql sql = new CExecSql();
            //     sql.SaveImage(examinedby, Convert.FromBase64String(WriteString));


            StringBuilder sql = new StringBuilder();
            bool insert = false;
            String photo_id_str = "";

            string photo_id = share.Get_mysql_database_MaxID("care_wound_document", "wound_photo_id");

            //   string ordering = share.Get_mysql_database_MaxID_ordering("care_wound_document", "ordering", string.Format("observation_id = {0}", observation_id));
            string ordering = share.Get_mysql_database_MaxID_ordering("care_wound_document", "ordering", string.Format("observation_id = {0}", observation_id));
            string[] values = new string[] { photo_id, observation_id, ordering, examined_by };


            sql.Append("Insert into care_wound_document(wound_photo_id,observation_id,ordering,created_by,created_datetime,document_photo) Values({0},{1},{2},'{3}',now(),?parval);");
            // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
            // sql.Append("select document_photo  from client_documents2 Where client_photo_id = '{0}' and valid = 'Y';");
            string comm = string.Format(sql.ToString(), values);

            //byte[] image_bytes = Convert.FromBase64String(photo_data);
            int a = share.ExecBoldSql(comm.ToString(), WriteString);
            if (a > 0)
            {
                insert = true;
            }




        }

        //删除以前的文件
        public void DeleteOldDayFile()
        {

            DirectoryInfo dyInfo = new DirectoryInfo(uploadPath);
            //获取文件夹下所有的文件
            foreach (FileInfo feInfo in dyInfo.GetFiles())
            {
                //判断文件日期是否小于今天，是则删除
                if (DateTime.Compare(feInfo.CreationTime, (DateTime.Now.AddDays(-1))) < 0)
                    feInfo.Delete();
            }
        }
    }

    public class post_observation_record
    {
        public string observation_id;
        public string is_update;
        public string nextdate;
    }






}