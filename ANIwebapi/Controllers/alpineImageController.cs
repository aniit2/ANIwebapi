using Ani_Test_WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
namespace Ani_Test_WebApi.Controllers
{
    public class alpineImageController : ApiController
    {
        [HttpGet]
        public ClientImageInfo GetUserImageByID(string PhotoID)
        {
            CExecSql sql = new CExecSql();
            ClientImageInfo ret = new ClientImageInfo();
            ret.PhotoData = sql.GetImage(PhotoID);
            ret.PhotoID = PhotoID;
            return ret;
        }
        /*
        [HttpPost]
        public Boolean PostImageByID(String PhotoID, String PhotoBytes)
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];

            CExecSql sql = new CExecSql();
            sql.SaveImage(PhotoID, Convert.FromBase64String(PhotoBytes));
            return true;
        }

        [HttpPost]
        public post_observation_record post_wound_observation_photo(string observation_id, string examined_by)
        {

            //  public post_observation_record post_wound_observation_photo(String observation_id, String examined_by, String photo_data)
            //   {Encription
            // Encription
            Encription encription = new Encription();
            ANICshare share = new ANICshare();
            
            string dbHost = "192.168.0.10";
            string dbPort = "3366";
            string dbUser = "alpine";//資料庫使用者帳號
            string dbPass = "nfc11573";//資料庫使用者密碼
            string dbName = "alpine";//資料庫名稱
            string db_testName = "alpine_test";//資料庫名稱

            string LoginName = "100053";//資料庫名稱
            string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱


            share.dbHost = dbHost;
            share.dbPort = dbPort;
            share.dbUser = dbUser;
            share.dbPass = dbPass;
            share.dbName = dbName;
            share.db_testName = db_testName;
            share.refresh_connection();



            observation_id = share.dencry_value(observation_id);
            examined_by = share.dencry_value(examined_by);



            var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            using (var sr = new StreamReader(context.Request.InputStream, Encoding.UTF8, true, 1024, true))
            {

                string[] bodyValues = sr.ReadToEnd().Split(';');

                if (bodyValues[0] == "admin" && bodyValues[1] == "123456")
                {
                    //    CExecSql sql = new CExecSql();

                    //     sql.SaveImage(bodyValues[2], Convert.FromBase64String(bodyValues[3]));
                    StringBuilder sql = new StringBuilder();

                    bool insert = false;
                    String photo_id_str = "";

                    string photo_id = share.Get_mysql_database_MaxID("care_wound_document", "wound_photo_id");

                    string ordering = share.Get_mysql_database_MaxID_ordering("care_wound_document", "ordering", string.Format("observation_id = {0}", observation_id));
                    string[] values = new string[] { photo_id, observation_id, ordering, examined_by };


                    sql.Append("Insert into care_wound_document(wound_photo_id,observation_id,ordering,created_by,created_datetime,document_photo) Values({0},{1},{2},'{3}',now(),?parval);");
                    // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
                    // sql.Append("select document_photo  from client_documents2 Where client_photo_id = '{0}' and valid = 'Y';");
                    string comm = string.Format(sql.ToString(), values);

                    //byte[] image_bytes = Convert.FromBase64String(photo_data);
                    int a = share.ExecBoldSql(sql.ToString(), bodyValues[2]);
                    if (a > 0)
                    {
                        insert = true;
                    }





                    //    SELECT* FROM wahhei.care_wound_document;
                    //    wound_photo_id, observation_id, document_photo, valid





                    if (insert == false)
                    {

                        return null;

                    }
                    else
                    {



                        String str = "YES";
                        post_observation_record insertstate =
                        new post_observation_record()
                        {

                            is_update = share.encry_value(str),
                            //nextdate = share.encry_value(nextdt.ToString(ANICshare.sql_timeformat_nosecond))

                        };
                        return insertstate;





                    }
                }
                else
                {
                    return null;
                }








            }



        }
        */
        ////[HttpPost]
        ////public String PostImageByID()
        ////{
        ////    Encription encription = new Encription();
        ////    ANICshare share = new ANICshare();


        ////    string dbHost = "192.168.0.10";
        ////    string dbPort = "3366";
        ////    string dbUser = "anidemo";//資料庫使用者帳號
        ////    string dbPass = "nfc22531";//資料庫使用者密碼
        ////    string dbName = "anidemo";//資料庫名稱
        ////    string db_testName = "anidemo_test";//資料庫名稱

        ////    string LoginName = "100004";//資料庫名稱
        ////    string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱



        ////    share.dbHost = dbHost;
        ////    share.dbPort = dbPort;
        ////    share.dbUser = dbUser;
        ////    share.dbPass = dbPass;
        ////    share.dbName = dbName;
        ////    share.db_testName = db_testName;
        ////    share.refresh_connection();



        ////    //  observation_id = share.dencry_value(observation_id);
        ////    //  examined_by = share.dencry_value(examined_by);




        ////    var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
        ////    context.Request.InputStream.Seek(0, SeekOrigin.Begin);
        ////    using (var sr = new StreamReader(context.Request.InputStream, Encoding.UTF8, true, 1024, true))
        ////    {
        ////        string observation_id = "";
        ////        string examined_by = "";
        ////        string[] bodyValues = sr.ReadToEnd().Split(';');
        ////        try
        ////        {
        ////            observation_id = share.dencry_value(bodyValues[1]);
        ////            examined_by = share.dencry_value(bodyValues[2]);
        ////        }
        ////        catch (Exception)
        ////        {

        ////            return share.dencry_value("False");
        ////        }





        ////        if (bodyValues[0].Equals(dbUser))
        ////        {
        ////            // CExecSql sql = new CExecSql();
        ////            //  sql.SaveImage(bodyValues[2], Convert.FromBase64String(bodyValues[3]));

        ////            StringBuilder sql = new StringBuilder();
        ////            bool insert = false;
        ////            String photo_id_str = "";

        ////            string photo_id = share.Get_mysql_database_MaxID("care_wound_document", "wound_photo_id");

        ////            //   string ordering = share.Get_mysql_database_MaxID_ordering("care_wound_document", "ordering", string.Format("observation_id = {0}", observation_id));
        ////            string ordering = share.Get_mysql_database_MaxID_ordering("care_wound_document", "ordering", string.Format("observation_id = {0}", observation_id));
        ////            string[] values = new string[] { photo_id, observation_id, ordering, examined_by };


        ////            sql.Append("Insert into care_wound_document(wound_photo_id,observation_id,ordering,created_by,created_datetime,document_photo) Values({0},{1},{2},'{3}',now(),?parval);");
        ////            // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
        ////            // sql.Append("select document_photo  from client_documents2 Where client_photo_id = '{0}' and valid = 'Y';");
        ////            string comm = string.Format(sql.ToString(), values);

        ////            //byte[] image_bytes = Convert.FromBase64String(photo_data);
        ////            int a = share.ExecBoldSql(comm.ToString(), bodyValues[3]);
        ////            if (a > 0)
        ////            {
        ////                insert = true;
        ////            }





        ////            //    SELECT* FROM wahhei.care_wound_document;
        ////            //    wound_photo_id, observation_id, document_photo, valid





        ////            if (insert == false)
        ////            {

        ////                return null;

        ////            }
        ////            else
        ////            {



        ////                String str = "YES";
        ////                post_observation_record insertstate =
        ////                new post_observation_record()
        ////                {

        ////                    is_update = share.encry_value(str),
        ////                    //nextdate = share.encry_value(nextdt.ToString(ANICshare.sql_timeformat_nosecond))

        ////                };
        ////                return "true";





        ////            }

        ////        }
        ////    }

        ////    return "false";
        ////}
        /*
        [HttpPost]
        public String PostImageByID()
        {
            Encription encription = new Encription();
            ANICshare share = new ANICshare();

            string dbHost = "192.168.0.10";
            string dbPort = "3366";
            string dbUser = "alpine";//資料庫使用者帳號
            string dbPass = "nfc11573";//資料庫使用者密碼
            string dbName = "alpine";//資料庫名稱
            string db_testName = "alpine_test";//資料庫名稱

            string LoginName = "100053";//資料庫名稱
            string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱

            share.dbHost = dbHost;
            share.dbPort = dbPort;
            share.dbUser = dbUser;
            share.dbPass = dbPass;
            share.dbName = dbName;
            share.db_testName = db_testName;
            share.refresh_connection();



            //  observation_id = share.dencry_value(observation_id);
            //  examined_by = share.dencry_value(examined_by);




            var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            using (var sr = new StreamReader(context.Request.InputStream, Encoding.UTF8, true, 1024, true))
            {
                string observation_id = "";
                string examined_by = "";
                string[] bodyValues = sr.ReadToEnd().Split(';');
                try
                {
                    observation_id = share.dencry_value(bodyValues[1]);
                    examined_by = share.dencry_value(bodyValues[2]);
                }
                catch (Exception)
                {

                    return share.dencry_value("False");
                }





                if (share.dencry_value(bodyValues[0]).Equals(dbUser))
                {
                    // CExecSql sql = new CExecSql();
                    //  sql.SaveImage(bodyValues[2], Convert.FromBase64String(bodyValues[3]));

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
                    int a = share.ExecBoldSql(comm.ToString(), bodyValues[3]);
                    if (a > 0)
                    {
                        insert = true;
                    }





                    //    SELECT* FROM wahhei.care_wound_document;
                    //    wound_photo_id, observation_id, document_photo, valid





                    if (insert == false)
                    {

                        return null;

                    }
                    else
                    {



                        String str = "YES";
                        post_observation_record insertstate =
                        new post_observation_record()
                        {

                            is_update = share.encry_value(str),
                            //nextdate = share.encry_value(nextdt.ToString(ANICshare.sql_timeformat_nosecond))

                        };
                        return "true";





                    }

                }
            }

            return "true";
        }

        */


        static string uploadPath = HttpContext.Current.Server.MapPath("~/App_Data/");


        [HttpPost]
        public String PostFile()
        {
            ANICshare share = new ANICshare();

            string dbHost = "192.168.0.10";
            string dbPort = "3366";
            string dbUser = "alpine";//資料庫使用者帳號
            string dbPass = "nfc11573";//資料庫使用者密碼
            string dbName = "alpine";//資料庫名稱
            string db_testName = "alpine_test";//資料庫名稱

            string LoginName = "100053";//資料庫名稱
            string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱

            share.dbHost = dbHost;
            share.dbPort = dbPort;
            share.dbUser = dbUser;
            share.dbPass = dbPass;
            share.dbName = dbName;
            share.db_testName = db_testName;
            share.refresh_connection();

 



            var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            String ret = string.Empty;
            using (var sr = new StreamReader(context.Request.InputStream, Encoding.UTF8, true, 1024, true))
            {

                string observation_id = "";
                string examined_by = "";
                string command = "";
                string[] bodyValues = sr.ReadToEnd().Split(';');

                try
                {
                    command = share.dencry_value(bodyValues[0]);
                    observation_id = share.dencry_value(bodyValues[2]);
                    examined_by = share.dencry_value(bodyValues[3]);

                }
                catch (Exception)
                {

                    throw;
                }
                if (!share.dencry_value(bodyValues[1]).Equals(dbUser))
                {
                    return null;
                }

                ///0; db_name; oberserver_id; examineby; path; StartIndex; PhotoBytes
  

                switch (command)
                {
                    case "0":
                        //登陆
                        //      if (bodyValues[1] == "admin" && bodyValues[2] == "123456")
                        //      {
                        ret = CreateNewFile(bodyValues[6]);
                        //删除除了今天创建的文件。保存目录下只留下今天的文件。
                        try
                        {
                            DeleteOldDayFile();
                        }
                        catch (Exception e)
                        {
                            return e.Message;
                        }
                        return ret;
                        //     }
                        break;
                    case "1":
                        //追到到指定文件当中

                        ret = AppandToFile(bodyValues[4], bodyValues[5], bodyValues[6]);

                        break;
                    case "2":
                        //完成文件的上传。
                        ret = CmpleteUpLoadFile(bodyValues[4], observation_id, examined_by, bodyValues[6]);

                        break;
                    case "3":
                        //完成文件的上传。
                        ret = "Cancel Upload";
                        try
                        {
                            File.Delete(@bodyValues[3]);
                        }
                        catch (Exception e)
                        {
                            return e.Message + "\n" + @bodyValues[3];
                        }
                        break;
                }
            }

            return "true";
        }

        private String CreateNewFile(String WriteString)
        {
            string ret = string.Empty;
            try
            {
                // 如果目录不存在则要先创建
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                //得到文件路径和名
                string FilePath = String.Format("{2}AniTemp_{1}_{0}.txt", Guid.NewGuid(), DateTime.Now.Ticks, uploadPath);

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
        private String AppandToFile(String FilePath, string startIndex, String WriteString)
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
        private String CmpleteUpLoadFile(String FilePath, String mysqlID, String examainedby, String WriteString)
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
        private void WriteMysql(string observation_id,
            string examined_by, String WriteString)
        {



            Encription encription = new Encription();
            ANICshare share = new ANICshare();

            string dbHost = "192.168.0.10";
            string dbPort = "3366";
            string dbUser = "alpine";//資料庫使用者帳號
            string dbPass = "nfc11573";//資料庫使用者密碼
            string dbName = "alpine";//資料庫名稱
            string db_testName = "alpine_test";//資料庫名稱

            string LoginName = "100053";//資料庫名稱
            string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱


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
        private void DeleteOldDayFile()
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
}