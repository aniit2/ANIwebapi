
using AlarmMonitorClient.code;
using AndroidServerClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace ANI.code
{

    public class ANImoblie
    {

        ANICsqlConn conn = new ANICsqlConn();
        ANICshare share = new ANICshare();
        public string dbHost = "192.168.1.2";
        public string dbPort = "3306";
        public string dbUser = "root";//資料庫使用者帳號
        public string dbPass = "nfc25531";//資料庫使用者密碼
        public string dbName = "azure";//資料庫名稱
        public string db_testName = "azure_test";//資料庫名稱
        public string LoginName = "100000";//資料庫名稱
        public string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱
        public bool panel_exist =false;


        public ANImoblie( bool encrytion)
        {
            share.encrytion = encrytion;
            refresh_connection();
        } 

        public bool set_test
        {
            set { conn.is_test = share.set_test = value; }
            get { return share.set_test; }
        }
        public void refresh_connection()
        {
            share.dbHost = dbHost;
            share.dbPort = dbPort;
            share.dbUser = dbUser;
            share.dbPass = dbPass;
            share.dbName = dbName;
            share.db_testName = db_testName;
            share.refresh_connection();
 
      
        }
        private ANICshare getshare()
        {
            ANICshare sh = new ANICshare();
            sh.dbHost = dbHost;
            sh.dbPort = dbPort;
            sh.dbUser = dbUser;
            sh.dbPass = dbPass;
            sh.dbName = dbName;
            sh.db_testName = db_testName;
            sh.refresh_connection();
            return sh;
        }

        public class logining : object
        {
            public string loginsuccess;
            public string c_name;
            public string limit;
            public string level;
            public string quick_search;
            public string quick_search_index;

        }

        public class logout
        {
            public string result;
        }

        public class ANI_server
        {
            public string server_state;
         
        }
        public class ANI_mobile
        {
            public string update_state;
        }
        public ANI_server get_server_state(string test)
        {

            if (!dbName.Equals(share.dencry_value( test)))
            {
                return null;
            }
            string state = share.testmysql();
            ANI_server ani = new ANI_server()
            { server_state =share.encry_value( state )};
            return ani;
        }
        public ANI_mobile check_moblie_update(  string version)
        {
            //FileInfo myInfo = new FileInfo(@"C:\newfile.txt");
            version = share.dencry_value(version);
            string up = "";

            DataSet ds = share.GetDataSource(Clogin.get_moblie_version(string.Format("and version >{0}", version)));

            if (ds.Tables[0].Rows.Count > 0)
            {
                up = share.encry_value("YES");

               
                //FileInfo fileInfo = new FileInfo(Server.MapPath("~/apk/") + "app.apk");
            }
            else
            {
                up = share.encry_value("NO");
            }

        
 

            ANI_mobile mobile = new ANI_mobile()
            {
                update_state = up
            }
           ;
            return mobile;

            /*
            var response = Context.Response;
            response.ContentType = "application/octet-stream";
            response.AppendHeader("Content-Disposition", "attachment; filename=" + "app.apk");
         //   response.AddHeader("Content-Length", fileInfo.Length.ToString());
            using (FileStream fs = new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/"), "apk/app.apk"), FileMode.Open))
            {
                Byte[] buffer = new Byte[256];
                Int32 readed = 0;

                while ((readed = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    response.OutputStream.Write(buffer, 0, readed);
                    response.Flush();
                }
            }
            response.End();
            */
        }
        public FileInfo get_moblie_update(string device, string version)
    {
            //FileInfo myInfo = new FileInfo(@"C:\newfile.txt");
            version = share.dencry_value(version);
            device = share.dencry_value(device);

            DataSet ds = share.GetDataSource(Clogin.get_moblie_version(string.Format("and version >{0}",version)));

            if (ds.Tables[0].Rows.Count>0)
            {
                string[] values = new string[] { share.Get_mysql_database_MaxID("user_moblie_log", "log_id"), device, version, Environment.MachineName, share.get_current_time() };
                share.bool_Ecxe_transaction_Sql(Clogin.insert_moblie_update_log(values));
                DataRow row = ds.Tables[0].Rows[0];
                string path = row[1].ToString();
                string class_name = this.dbName;
                FileInfo fileInfo = null;
                if (!string.IsNullOrWhiteSpace(path))
                    fileInfo= new FileInfo(string.Format(path, class_name));

                return fileInfo;
            }

            return null;

 

        /*
        var response = Context.Response;
        response.ContentType = "application/octet-stream";
        response.AppendHeader("Content-Disposition", "attachment; filename=" + "app.apk");
     //   response.AddHeader("Content-Length", fileInfo.Length.ToString());
        using (FileStream fs = new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/"), "apk/app.apk"), FileMode.Open))
        {
            Byte[] buffer = new Byte[256];
            Int32 readed = 0;

            while ((readed = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                response.OutputStream.Write(buffer, 0, readed);
                response.Flush();
            }
        }
        response.End();
        */
    }

        public logining getlogin(string username, string password)
        {
            username = share.dencry_value(username);
            password = share.dencry_value(password);

            string apassword = "";
            string auserid = "";
            string ac_name = "";
            string user_level = "";

            string client_level = "";
            string medicine_level = "";
            string medical_level = "";
            string nursing_level = "";
            string assessment_level = "";
            Encription encr = new  Encription();

            StringBuilder sql = new StringBuilder();
            sql.Append(Clogin.getlogin(username, password));
            sql.Append(Clogin.get_version());
            DataSet ds =  share.GetDataSource(Clogin.getlogin(username,password));
            if (ds.Tables[0].Rows.Count>0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                apassword =   row["user_password"].ToString();
                auserid = row["user_id"].ToString();
                ac_name = row["Chinese_Name"].ToString();
                //user_level = reader["user_level"].ToString();
                client_level = row["client"].ToString();
                medicine_level = row["medicine"].ToString();
                medical_level = row["medical"].ToString();
                nursing_level = row["nursing"].ToString();
                assessment_level = row["assessment"].ToString();
                string[] level = new string[] { client_level, medicine_level, medical_level, nursing_level, assessment_level };
                user_level = string.Join(";", level);
            }
            //KillSleepingConnections(2000);
            if (ac_name.Length <= 0)
            {
                ac_name = username;
            }
            if (password != "")
            {
                if (password == encr.decrypt(apassword))
                {
                    string quick_search_str = "";
                    string quick_search_index_str = "";
                    DataSet dq = share.GetDataSource(Cclient.GetClientRoom(""));
                    if (dq.Tables[0].Rows.Count>0)
                    {

                        List<string> keywords = new List<string>();
                        List<string> keywords_index = new List<string>();
                        foreach (DataRow row in dq.Tables[0].Rows)
                        {
                            keywords.Add(row[1].ToString());
                            keywords_index.Add("2");
                        }
                        if (dq.Tables[1].Rows.Count > 0)
                        {
                            foreach (DataRow row in dq.Tables[1].Rows)
                            {
                                keywords.Add(row[0].ToString());
                                keywords_index.Add("2");
                            }

                        }
                        if (keywords.Count>0)
                        {

                            keywords.Add("本月生日");
                            keywords_index.Add("4");

                            keywords.Add("留醫中");
                            keywords_index.Add("3");



                            quick_search_str = string.Join(";",keywords.ToArray());
                            quick_search_index_str = string.Join(";", keywords_index.ToArray());
                        }

                    }



                    logining alogin = new logining{


                loginsuccess = share.encry_value("true"),
                c_name = share.encry_value(ac_name),
                level = share.encry_value(user_level),
                        quick_search = share.encry_value(quick_search_str),
                        quick_search_index = share.encry_value(quick_search_index_str)


                    };

                    return alogin;

                }
                else
                {
                    logining alogin = new logining{
         
                loginsuccess = share.encry_value("false")

                
                };


                    return alogin;

                }
            }

            else
            {

                logining alogin = new logining{
  
                loginsuccess = share.encry_value("false")

            
            };
                return alogin;
            }

        }
        public logining get_app_login(string username, string password, string device_id, string location)
        {
            username = share.dencry_value(username);
            password = share.dencry_value(password);
            device_id = share.dencry_value(device_id);
            location = share.dencry_value(location);
            string apassword = "";
            string auserid = "";
            string ac_name = "";
            string user_level = "";

            string client_level = "";
            string medicine_level = "";
            string medical_level = "";
            string nursing_level = "";
            string assessment_level = "";

            string limit_str = "";

            Encription encr = new Encription();



            StringBuilder sql = new StringBuilder();
            sql.Append(Clogin.getlogin(username, password));
            sql.Append(Clogin.get_version());
            sql.Append(Clogin.get_moblie_limit());
            DataSet ds = share.GetDataSource(sql.ToString());
            if (ds.Tables[1].Rows.Count == 0)
            {
                logining alogin = new logining
                {

                    loginsuccess = share.encry_value("invalid")


                };

                return alogin;
            }
            else
            {
                DataRow row = ds.Tables[1].Rows[0];
                if (row[6].ToString().Equals("N"))
                {
                    logining alogin = new logining
                    {

                        loginsuccess = share.encry_value("invalid")


                    };
                    return alogin;
                }

            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                apassword = row["user_password"].ToString();
                auserid = row["user_id"].ToString();
                ac_name = row["Chinese_Name"].ToString();
                //user_level = reader["user_level"].ToString();
                client_level = row["client"].ToString();
                medicine_level = row["medicine"].ToString();
                medical_level = row["medical"].ToString();
                nursing_level = row["nursing"].ToString();
                assessment_level = row["assessment"].ToString();
                string[] level = new string[] { client_level, medicine_level, medical_level, nursing_level, assessment_level };
                user_level = string.Join(";", level);
            }
            if (ds.Tables[2].Rows.Count > 0)
            {
                limit_str = ds.Tables[2].Rows[0][2].ToString();
            }
            //KillSleepingConnections(2000);
            if (ac_name.Length <= 0)
            {
                ac_name = auserid;
            }
            if (password != "")
            {
                if (password == encr.decrypt(apassword))
                {
                    string quick_search_str = "";
                    string quick_search_index_str = "";

                    string[] values = new string[] { ac_name, share.get_current_time(), location, device_id };
                    share.bool_Ecxe_transaction_Sql(Clogin.insert_LoginUser(values));

                    DataSet dq = share.GetDataSource(Cclient.GetClientRoom(""));
                    if (dq.Tables[0].Rows.Count > 0)
                    {

                        List<string> keywords = new List<string>();
                        List<string> keywords_index = new List<string>();
                        foreach (DataRow row in dq.Tables[0].Rows)
                        {
                            keywords.Add(row[1].ToString());
                            keywords_index.Add("5");
                        }
                        if (dq.Tables[1].Rows.Count > 0)
                        {
                            foreach (DataRow row in dq.Tables[1].Rows)
                            {
                                keywords.Add(row[0].ToString());
                                keywords_index.Add("6");
                            }

                        }
                        if (keywords.Count > 0)
                        {

                            keywords.Add("本月生日");
                            keywords_index.Add("4");

                            keywords.Add("留醫中");
                            keywords_index.Add("3");



                            quick_search_str = string.Join(";", keywords.ToArray());
                            quick_search_index_str = string.Join(";", keywords_index.ToArray());
                        }

                    }



                    logining alogin = new logining
                    {


                        loginsuccess = share.encry_value("true"),
                        c_name = share.encry_value(ac_name),
                        level = share.encry_value(user_level),
                        limit = share.encry_value(limit_str),
                        quick_search = share.encry_value(quick_search_str),
                        quick_search_index = share.encry_value(quick_search_index_str)


                    };

                    return alogin;

                }
                else
                {
                    logining alogin = new logining
                    {

                        loginsuccess = share.encry_value("false")


                    };


                    return alogin;

                }
            }

            else
            {

                logining alogin = new logining
                {

                    loginsuccess = share.encry_value("false")


                };
                return alogin;
            }

        }
        public logining get_app_login2(string username, string password, string device_id, string location, string version)
        {
            username = share.dencry_value(username);
            password = share.dencry_value(password);
            device_id = share.dencry_value(device_id);
            location = share.dencry_value(location);
            version = share.dencry_value(version);
            string apassword = "";
            string auserid = "";
            string ac_name = "";
            string user_level = "";

            string client_level = "";
            string medicine_level = "";
            string medical_level = "";
            string nursing_level = "";
            string assessment_level = "";

            string limit_str = "";

            Encription encr = new Encription();



            StringBuilder sql = new StringBuilder();
            sql.Append(Clogin.getlogin(username, password));
            sql.Append(Clogin.get_version());
            sql.Append(Clogin.get_moblie_limit());
            DataSet ds = share.GetDataSource(sql.ToString());
            if (ds.Tables[1].Rows.Count == 0)
            {
                logining alogin = new logining
                {

                    loginsuccess = share.encry_value("invalid")


                };

                return alogin;
            }
            else
            {
                DataRow row = ds.Tables[1].Rows[0];
                if (row[6].ToString().Equals("N"))
                {
                    logining alogin = new logining
                    {

                        loginsuccess = share.encry_value("invalid")


                    };
                    return alogin;
                }

            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                apassword = row["user_password"].ToString();
                auserid = row["user_id"].ToString();
                ac_name = row["Chinese_Name"].ToString();
                //user_level = reader["user_level"].ToString();
                client_level = row["client"].ToString();
                medicine_level = row["medicine"].ToString();
                medical_level = row["medical"].ToString();
                nursing_level = row["nursing"].ToString();
                assessment_level = row["assessment"].ToString();
                string[] level = new string[] { client_level, medicine_level, medical_level, nursing_level, assessment_level };
                user_level = string.Join(";", level);
            }
            if (ds.Tables[2].Rows.Count > 0)
            {
                limit_str = ds.Tables[2].Rows[0][2].ToString();
            }
            //KillSleepingConnections(2000);
            if (ac_name.Length <= 0)
            {
                ac_name = auserid;
            }
            if (password != "")
            {
                if (password == encr.decrypt(apassword))
                {
                    string quick_search_str = "";
                    string quick_search_index_str = "";

                    string[] values = new string[] { ac_name, share.get_current_time(), location, device_id, version };
                    share.bool_Ecxe_transaction_Sql(Clogin.insert_LoginUser2(values));

                    DataSet dq = share.GetDataSource(Cclient.GetClientRoom(""));
                    if (dq.Tables[0].Rows.Count > 0)
                    {

                        List<string> keywords = new List<string>();
                        List<string> keywords_index = new List<string>();
                        foreach (DataRow row in dq.Tables[0].Rows)
                        {
                            keywords.Add(row[1].ToString());
                            keywords_index.Add("5");
                        }
                        if (dq.Tables[1].Rows.Count > 0)
                        {
                            foreach (DataRow row in dq.Tables[1].Rows)
                            {
                                keywords.Add(row[0].ToString());
                                keywords_index.Add("6");
                            }

                        }
                        if (keywords.Count > 0)
                        {

                            // Raymond @20201023
                            //keywords.Add("本月生日");
                            //keywords_index.Add("4");

                            keywords.Add("留醫中");
                            keywords_index.Add("3");

                            keywords.Add("洗傷口");
                            keywords_index.Add("8");

                            quick_search_str = string.Join(";", keywords.ToArray());
                            quick_search_index_str = string.Join(";", keywords_index.ToArray());
                        }

                    }



                    logining alogin = new logining
                    {


                        loginsuccess = share.encry_value("true"),
                        c_name = share.encry_value(ac_name),
                        level = share.encry_value(user_level),
                        limit = share.encry_value(limit_str),
                        quick_search = share.encry_value(quick_search_str),
                        quick_search_index = share.encry_value(quick_search_index_str)


                    };

                    return alogin;

                }
                else
                {
                    logining alogin = new logining
                    {

                        loginsuccess = share.encry_value("false")


                    };


                    return alogin;

                }
            }

            else
            {

                logining alogin = new logining
                {

                    loginsuccess = share.encry_value("false")


                };
                return alogin;
            }

        }

        public logout get_app_logout(string username, string device_id)  // Raymond @20201216
        {
            username = share.dencry_value(username);
            device_id = share.dencry_value(device_id);

            Encription encr = new Encription();

            StringBuilder sql = new StringBuilder();
            sql.Append(Clogin.getlogindata(username, device_id));

            DataSet ds = share.GetDataSource(sql.ToString());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                String user_id = row["user_id"].ToString();
                String login_datetime = row["login_datetime"].ToString();
                String login_ip = row["login_ip"].ToString();
                String device_name = row["device_name"].ToString();
                share.bool_Ecxe_transaction_Sql(Clogin.update_logout_datetime(user_id, login_datetime, login_ip, device_name, share.get_current_time()));


                logout logout = new logout
                {
                    result = share.encry_value("true"),

                };

                return logout;
            }


            logout alogout = new logout
            {
                result = share.encry_value("false"),

            };

            return alogout;
        }


        /*
        public void KillSleepingConnections(int iMinSecondsToExpire)
        {

            MySqlConnection conn = new MySqlConnection("server=" + dbHost + "; PORT= " + dbPort + " ;user id=" + dbUser + "; password=" + dbPass + "; database=" + dbName + "; CharSet=utf8");
            MySqlCommand command = conn.CreateCommand();
            //String cmdText = "select TOP(5) * from care_blood_pressure  where client_id = (@id) order by examination_datetime desc";
            //String cmdText = "SELECT * FROM care_blood_pressure ORDER BY examination_datetime DESC LIMIT 5";
            //String cmdText = "Insert into care_body_turning(client_id, body_turning , examination_datetime, examined_by ) Values(@id, @body_turning, @timestamp ,@handlingperson)";
            //String cmdText = "Insert into care_body_turning(client_id, body_turning , examination_datetime, examined_by ) Values(@id, @body_turning, @timestamp ,@handlingperson)";
            String cmdText = "show processlist";
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);

            MySqlDataReader MyReader = null;



            //String cmdText = "show processlist";
            System.Collections.ArrayList m_ProcessesToKill = new System.Collections.ArrayList();

            //OdbcConnection myConn = new OdbcConnection(Global.strDBServer);
            //OdbcCommand myCmd = new OdbcCommand(strSQL, myConn);
            //OdbcDataReader MyReader = null;

            try
            {
                conn.Open();

                // Get a list of processes to kill.
                MyReader = cmd.ExecuteReader();
                while (MyReader.Read())
                {
                    // Find all processes sleeping with a timeout value higher than our threshold.
                    int iPID = Convert.ToInt32(MyReader["Id"].ToString());
                    string strState = MyReader["Command"].ToString();
                    int iTime = Convert.ToInt32(MyReader["Time"].ToString());
                    Debug.Print(strState);
                    if (strState == "Sleep" && iTime >= iMinSecondsToExpire && iPID > 0)
                    {
                        // This connection is sitting around doing nothing. Kill it.
                        m_ProcessesToKill.Add(iPID);
                    }
                }

                MyReader.Close();

                foreach (int aPID in m_ProcessesToKill)
                {
                    cmdText = "kill " + aPID;
                    cmd.CommandText = cmdText;
                    cmd.ExecuteNonQuery();

                    //strSQL = "kill " + aPID;
                    //myCmd.CommandText = strSQL;
                    //myCmd.ExecuteNonQuery();
                }
            }
            catch (Exception excep)
            {
            }
            finally
            {
                if (MyReader != null && !MyReader.IsClosed)
                {
                    MyReader.Close();
                }

                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            //Debug.Print(m_ProcessesToKill.Count.ToString);

            //sadfsa
            //Debug.Print
            //return m_ProcessesToKill.Count;



        }
        */
        public dash_board get_dash_board(string username)
        {
            username = share.dencry_value(username);
            //password = share.dencry_value(password);

            string apassword = "";
            string auserid = "";
            string ac_name = "";
            string user_level = "";

            string client_level = "";
            string medicine_level = "";
            string medical_level = "";
            string nursing_level = "";
            string assessment_level = "";
            List<dash_board_event> dash_list = new List<dash_board_event>();
            Encription encr = new Encription();


            DataSet ds = share.GetDataSource(Clogin.getlogin(username, ""));
            if (ds.Tables[0].Rows.Count > 0)
            {



                DataRow row = ds.Tables[0].Rows[0];
                apassword = row["user_password"].ToString();
                auserid = row["user_id"].ToString();
                ac_name = row["Chinese_Name"].ToString();
                //user_level = reader["user_level"].ToString();
                client_level = row["client"].ToString();
                medicine_level = row["medicine"].ToString();
                medical_level = row["medical"].ToString();
                nursing_level = row["nursing"].ToString();
                assessment_level = row["assessment"].ToString();

                string[] right = new string[] { "Y", client_level, medical_level, medicine_level, nursing_level };
                dash_list = get_dash_event_right(right, dash_list);
                dash_board board = new dash_board()
                {
                    events = dash_list.ToArray()
                };


                return board;
            }
            else
            {
                return null;
            }

        }
        private List<dash_board_event> get_dash_event_right(string[] right, List<dash_board_event> dash_list)
        {

            for (int i = 0; i < right.Length; i++)
            {
                if (right[i]=="Y")
                {
                    if (i==0)
                    {
                        dash_list = get_dash_event(0, dash_list);
                    }
                    else if (i == 3)
                    {
                        dash_list = get_dash_event(1, dash_list);
                        dash_list = get_dash_event(2, dash_list);
                        dash_list = get_dash_event(3, dash_list);
                    }
 
            
                }
            

            }

            return dash_list;

        }
        private List<dash_board_event> get_dash_event(int index ,List<dash_board_event> dash_list)
        {
            List<client_info> client_list = new List<client_info>();
            string table_name = "";
            string accessible_str = "";
            if (index == 0)
            {

                DataSet ds = share.GetDataSource(Cbook.get_book_info(" and a.book_status = '預訂中' "));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    accessible_str = "N";
                    table_name = "訂位";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow row = ds.Tables[0].Rows[i];
                        string client_name_str = row[2].ToString();
                        string date_str = row[5].ToString();
                        string sex_str = row[11].ToString();
                        string picture_id_str = row[14].ToString();
                        string bed = string.Format("BD;預計入住床位;{0}", row[4].ToString());
                        string client_id_str = row[15].ToString();
                        client_list.Add(new client_info()
                        {
                            client_name = client_name_str,
                            sex = sex_str,
                            client_id = client_id_str,
                            date = date_str,
                            client_picture_id = picture_id_str,
                            contents = new string[] { bed }
                        }
                        );

                    }
                    dash_list.Add(get_dash_event(table_name, accessible_str, client_list.ToArray()));
                }
            }
            else if (index == 1)
            {
                DataSet ds = share.GetDataSource(Cmedical.get_revisit_brief("and per.active_status = 'Y' and rev.revisit_status  = '預約中' and date( rev.revisit_planned_datetime) <= CURDATE() + INTERVAL 1 DAY ",false));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    accessible_str = "Y";
                    table_name = "覆診";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow row = ds.Tables[0].Rows[i];

                        string client_name_str = row["client_name"].ToString();
                        string date_str = row["revisit_planned_date"].ToString() + " " + row["revisit_planned_time"].ToString(); 
                        string sex_str = row["sex"].ToString();
                        string picture_id_str = row["photo_id"].ToString();
                        string client_id_str = row["client_id"].ToString();
                        string address = string.Format("AD;地點;{0}", row["addr_org_chi_name"].ToString());
                        string spe = string.Format("SP;專科;{0}", row["specialties_code"].ToString());
                        string tran = string.Format("TP;交通;{0}", row["transport"].ToString());
                        string accompany = string.Format("PP;陪診員;{0}", row["revisit_accompany"].ToString());

                        /*
                        string client_name_str = row[1].ToString();
                        string date_str = row[3].ToString()+" "+ row[4].ToString();
                        string sex_str = row[13].ToString();
                        string picture_id_str = row[14].ToString();
                   
                        string client_id_str = row[2].ToString();


                        string address = string.Format("AD;地點;{0}", row[6].ToString());

 
                        string spe = string.Format("SP;專科;{0}", row[7].ToString());

                        string tran = string.Format("TP;交通;{0}", row[9].ToString());
                        string accompany = string.Format("PP;陪診員;{0}", row[11].ToString());
                        */

                        client_list.Add(new client_info()
                        {
                            client_name = client_name_str,
                            sex = sex_str,
                            client_id = client_id_str,
                            date = date_str,
                            client_picture_id = picture_id_str,
                            contents = new string[] { address,spe,tran,accompany }
                        }
                        );
                    }
                    dash_list.Add(get_dash_event(table_name, accessible_str, client_list.ToArray()));
                }
            }
            else if (index == 2)
            {
                DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    accessible_str = "Y";
                    table_name = "急症";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow row = ds.Tables[0].Rows[i];
                        string client_name_str = row["client_name"].ToString();
                        string sex_str = row["sex"].ToString();
                        string picture_id_str = row["client_photo_id"].ToString();


                        //string[] timearray = row[5].ToString().Split(' ');
                        string date_str = row["ae_in_date"].ToString() + " " + row["ae_in_time"].ToString();

                        string client_id_str = row[2].ToString();


                        string address = string.Format("AD;地點;{0}", row["addr_org_chi_name"].ToString());

                        ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                        //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                        string reason = string.Format("DC;急症原因;{0}", row["ae_in_reason"].ToString());

                        string bed = string.Format("BD;留醫病房／床號;{0}", row["ae_in_bed_num"].ToString());

                        /*
                        string client_name_str = row[1].ToString();
                        string sex_str = row[13].ToString();
                        string picture_id_str = row[14].ToString();


                        //string[] timearray = row[5].ToString().Split(' ');
                        string date_str = row[5].ToString();

                        string client_id_str = row[2].ToString();


                        string address = string.Format("AD;地點;{0}", row[7].ToString());
 
                        ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                        //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                        string reason = string.Format("DC;急症原因;{0}", row[4].ToString());

                        string bed = string.Format("BD;留醫病房／床號;{0}", row[8].ToString());
                        //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());
                        */

                        client_list.Add(new client_info()
                        {
                            client_name = client_name_str,
                            sex = sex_str,
                            client_id = client_id_str,
                            date = date_str,
                            client_picture_id = picture_id_str,
                            contents = new string[] { address,  reason, bed }
                        }
                        );
                    }
                    dash_list.Add(get_dash_event(table_name, accessible_str, client_list.ToArray()));
                }
            }
            else if (index == 3)
            {
                DataSet ds = share.GetDataSource(Cmedical.get_event_grid_table(" and per.active_status = 'Y' and rev.event_status  ='預約中'" +
                " and eve.dash_shown = 'Y' " +
                " and ( date(rev.event_planned_datetime) <= (date(now()) + INTERVAL (eve.reminder_day-1) DAY ))  "));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    accessible_str = "Y";
                    table_name = "";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow row = ds.Tables[0].Rows[i];

                        if (table_name != row[0].ToString())
                        {
                            if (client_list.Count > 0)
                            {
                                dash_list.Add(get_dash_event(table_name, accessible_str, client_list.ToArray()));
                            }


                            table_name = row[0].ToString();
                            client_list = new List<client_info>();

                            string client_name_str = row[3].ToString();
                            string sex_str = row[2].ToString();
                            string picture_id_str = row[12].ToString();
                            //string[] timearray = row[5].ToString().Split(' ');

                            string date_str = row[6].ToString();

                            string client_id_str = row[4].ToString();

                            //string date = string.Format("CA;日期;{0}", row[6].ToString());

                            string time_str = string.Format("TI;時間;{0}", row[7].ToString());
                            string state = string.Format("ST;狀態;{0}", row[10].ToString());

                            //string bed = string.Format("BD;留醫病房／床號:{0}", row[8].ToString());
                            //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());


                            client_list.Add(new client_info()
                            {
                                client_name = client_name_str,
                                sex = sex_str,
                                client_id = client_id_str,
                                date = date_str,
                                client_picture_id = picture_id_str,
                                contents = new string[] {  time_str,state }
                            }

                            );
                        }
                        else
                        {

                            string date = string.Format("CA;日期;{0}", row[6].ToString());
                            string time_str = string.Format("TI;時間;{0}", row[7].ToString());
                            string state = string.Format("ST;狀態;{0}", row[10].ToString());

                            client_list.Add(new client_info()
                            {
                                client_name = row[3].ToString(),
                                sex = row[2].ToString(),
                                client_id = row[4].ToString(),
                                date = row[6].ToString(),
                                client_picture_id = row[12].ToString(),
                                contents = new string[] { time_str, state }
                            });
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                dash_list.Add(get_dash_event(table_name, accessible_str, client_list.ToArray()));
                            }
                        }

                    }
                }
            }
 
             return dash_list;
        
        }
        private dash_board_event get_dash_event(string table_name, string accessible_str, client_info[] client_list)
        {
            for (int i = 0; i < client_list.Length; i++)
            {
                if (client_list[i].client_picture_id.Length==0)
                {
                    client_list[i].client_picture_id = "0";
                }

                client_list[i].client_id = share.encry_value(client_list[i].client_id);
                client_list[i].client_picture_id = share.encry_value(client_list[i].client_picture_id);
                client_list[i].client_name = share.encry_value(client_list[i].client_name);
                client_list[i].sex = share.encry_value(client_list[i].sex);
                client_list[i].date = share.encry_value(client_list[i].date);

                for (int a = 0; a < client_list[i].contents.Length;a++)
                {
                    client_list[i].contents[a] = share.encry_value(client_list[i].contents[a]);
                }
            }
            table_name = share.encry_value(table_name);
            accessible_str = share.encry_value(accessible_str);

            dash_board_event board = new dash_board_event()
            {
                event_name = table_name,
                clients = client_list,
                accessible = accessible_str
            };
            return board;

        }
        public client_id_in_nfc get_patrol_client_id(string nfc_id, string device, string user)
        {
            string label_usage_id_str = "";
            string label_usage_code_str = "";
            string client_id_str = "";
            string display_name = "";

            string nfc_id_str = share.dencry_value(nfc_id);
            string device_name_str = share.dencry_value(device);
            string examined_by_str = share.dencry_value(user);
            
            string time = share.get_current_time();



            DataSet ds = share.GetDataSource(Cclient.get_client_id_in_nfc(nfc_id_str));
            if (ds.Tables[0].Rows.Count > 0)
            {

                int i = 0;
                DataRow row = ds.Tables[0].Rows[0];


                client_id_str = row[i++].ToString();
                label_usage_code_str = row[i++].ToString();
                label_usage_id_str = row[i++].ToString();
                display_name = row[i++].ToString();

            }
            if (label_usage_code_str == "BED")
            {
                client_id_str = share.Get_mysql_database_value("sys_company_bed", "bed_id", label_usage_id_str, "client_id");
                share.bool_Ecxe_transaction_Sql(Clogin.insert_moblie_patrol_log(new string[] { device_name_str, display_name, label_usage_id_str, client_id_str, examined_by_str, time }));

                string message_str = "已新增巡更記錄";
                //client_id
                client_id_in_nfc info =
    new client_id_in_nfc()
    {


        label_usage_code = share.encry_value(label_usage_code_str),
        label_usage_id = share.encry_value(label_usage_id_str),

        client_id = share.encry_value(client_id_str),
        message = share.encry_value(message_str),
                    //picturename = picturenamestr

                };

                return info;
      
            }
            else 
            {


                return get_usage_in_nfc(  nfc_id,   device,   user);
                //client_id_str = share.Get_mysql_database_value("sys_company_bed", "bed_id", label_usage_id_str, "client_id");
                //client_id
            }
        }
        public client_id_in_nfc  get_client_id_in_nfc(string nfc_id)
        {
            string label_usage_id_str = "";
            string label_usage_code_str = "";
            string client_id_str = "";
            nfc_id = share.dencry_value(nfc_id);
 
            DataSet ds = share.GetDataSource(Cclient.get_client_id_in_nfc(nfc_id));
            if (ds.Tables[0].Rows.Count > 0)
            {

                int i = 0;
                DataRow row = ds.Tables[0].Rows[0];
           

                client_id_str = row[i++].ToString();
                label_usage_code_str = row[i++].ToString();
                label_usage_id_str = row[i++].ToString();

            }
            if (label_usage_code_str == "BED")
            {
                client_id_str = share.Get_mysql_database_value("sys_company_bed", "bed_id", label_usage_id_str, "client_id");
                //client_id
            }
            else if (label_usage_code_str == "ALM")
            {



                client_id_str = share.Get_mysql_database_value("sys_company_bed", "bed_id", label_usage_id_str, "client_id");
                //client_id
            }
 
            client_id_in_nfc info =  
                new client_id_in_nfc()
                {


                label_usage_code = share.encry_value(label_usage_code_str),
                label_usage_id = share.encry_value(label_usage_id_str),

                client_id = share.encry_value(client_id_str),
                //picturename = picturenamestr
 
        };

            return info;



        }
        public client_id_in_nfc get_usage_in_nfc(string nfc_id,string device,string user )
        {
            string label_usage_id_str = "";
            string label_usage_code_str = "";
            string client_id_str = "";
            string message_str ="";
            nfc_id = share.dencry_value(nfc_id);
            device = share.dencry_value(device);
            user = share.dencry_value(user);
            DataSet ds = share.GetDataSource(Cclient.get_client_id_in_nfc(nfc_id));
            if (ds.Tables[0].Rows.Count > 0)
            {

                int i = 0;
                DataRow row = ds.Tables[0].Rows[0];


                client_id_str = row[i++].ToString();
                label_usage_code_str = row[i++].ToString();
                label_usage_id_str = row[i++].ToString();


            }
            if (label_usage_code_str == "BED")
            {
                client_id_str = share.Get_mysql_database_value("sys_company_bed", "bed_id", label_usage_id_str, "client_id");
                //client_id
            }
            else if (label_usage_code_str == "ALM")
            {
                CAlarm panel = new CAlarm();
                // get_alarm_server_info
                string  device_id = share.Get_mysql_database_value("client_alarm_device", "device_id", label_usage_id_str, "serial_num");
                // "serial_num", 
                if (device_id==null)
                {
                    return null;
                }
                DataSet alarm_ds = share.GetDataSource(
                    panel.get_alarm_server_info("")+
                 panel.get_alarm_info(string.Format("and alarm.activate_device_id = {0} and alarm.alarming = 'Y' ", label_usage_id_str), "order by alarm.activate_datetime desc limit 1 ") );



                //DataSet alarm_ds = share.GetDataSource(arm.get_alarm_info(string.Format("and alarm.activate_device_id = '{0}' and  alarm.alarming= 'Y' ", device_id), "order by alarm.activate_datetime desc limit 1"));
                if (alarm_ds.Tables[1].Rows.Count == 0)
                {
                    message_str = "Count == 0!";
                }

                else if  (alarm_ds.Tables[1].Rows.Count>0)
                {




                    bool RESET = alarm_call_update(alarm_ds, device_id);
                    if (RESET)
                    {




                        //worker = new Thread(() => alarm_call_update(alarm_ds, device_id));
                       // worker.Start();
                      //  worker.Join();

                        //  SELECT* FROM azure.client_alarm;
                        //  alarm_id, bed_id, client_id, device_id, activate_id, activate_datetime, cancal_action_id, alarming, repeat, cancel_device, cancel_by, cancel_datetime, modified_by, modified_datetime, created_by, created_datetime, valid
                        List<string> alarm_id = new List<string>();


                        foreach (DataRow item in alarm_ds.Tables[1].Rows)
                        {
                            alarm_id.Add(item[0].ToString());
                        }
                        ///  string[] values = { string.Join(",",alarm_id.ToArray()),"100003", device, user,share.get_current_time() };
                        //  bool update = share.bool_Ecxe_transaction_Sql(panel.update_alarm_call(values));

                        //   index = 2;
                        //  string alarm_id = alarm_ds.Tables[0].Rows[0][0].ToString();

                        //  new_values = new string[]{ alarm_id,device_id,values[2], row[index++].ToString(),
                        //   row[index++].ToString(),row[5].ToString(),values[15],values[16] };
                        // arm.update_alarm_msg(values, 0);
                        DataRow datarow = alarm_ds.Tables[1].Rows[0];


                        string[] new_values = new string[]{ datarow[0].ToString(),"",device,  "0", "0", "100003", user, share.get_current_time()
                      };
                        bool update = share.bool_Ecxe_transaction_Sql(panel.update_alarm_msg(new_values, 2));
                        if (update)
                        {
                            message_str = "Deactive Alarm!";
                            inform_panel();
                        }
                    }
                    else
                    {
                        message_str = "RESET false!";
                    }
                }
                else
                {
                    message_str = "else!";
                    // DataSet bed_ds = share.GetDataSource(C"")
                }



              //  client_id_str = share.Get_mysql_database_value("sys_company_bed", "bed_id", label_usage_id_str, "client_id");
                //client_id
            }

            else if (label_usage_code_str == "DPL")
            {
                CPanel panel = new CPanel();
                //  DataSet nfc_ds = share.GetDataSource( panel.get_all_panel((string.Format("and page_index  = 2  ", ""))));
                DataSet nfc_ds = share.GetDataSource(panel.get_all_panel((string.Format("and display_id='{0}' ", label_usage_id_str))));
                // get_all_panel
                //   Init(Work,nfc_ds);
                worker = new Thread(() => call_update(nfc_ds,54));
                worker.Start();
                worker.Join();

           //     ASDFA
           //     client_id_str = share.Get_mysql_database_value("sys_company_bed", "bed_id", label_usage_id_str, "client_id");
                //client_id
            }
            client_id_in_nfc info =
                new client_id_in_nfc()
                {


                    label_usage_code = share.encry_value(label_usage_code_str),
                    label_usage_id = share.encry_value(label_usage_id_str),

                    client_id = share.encry_value(client_id_str), 
                    message = share.encry_value(message_str),
                    //picturename = picturenamestr

                };

            return info;



        }

        public client_briefing get_client_briefing(string client_id)
        {
            client_id = share.dencry_value(client_id);
            string[] str_arr = client_id.Split(';');
            if (str_arr.Length > 1)
            {


                if (str_arr[1] == "BED")
                {
                    DataSet da = share.GetDataSource(Cclient.get_empty_bed_info(str_arr[0]));
                    string empty_bed = "";
                    if (da.Tables[0].Rows.Count > 0)
                    {
                        empty_bed = da.Tables[0].Rows[0][0].ToString();
                    }


                    client_briefing empty_brief = new client_briefing
                    {



                        //label_usage_code = label_usage_code_str,
                        //label_usage_id = label_usage_id_str,
           
                        name = share.encry_value(""),
                        sex = share.encry_value(""),
                        age = share.encry_value(""),
                        bednum = share.encry_value(empty_bed),
                        //relative = relative_name_str,
                        remark = share.encry_value(""),
                        client_id = share.encry_value("0"),
                        //picturename = picturenamestr
                        picture_id = share.encry_value(""),
                        personal_ability = share.encry_value(""),
                        birth_date = share.encry_value(""),
                        wound_exist = share.encry_value(""),

                        meal_type = share.encry_value(""),

                        assessment_result = share.encry_value(""),
                        drug_ADR = share.encry_value(""),
                        drug_allergic = share.encry_value(""),
                        other_allergic = share.encry_value(""),
                        disease = share.encry_value(""),
                        absence = share.encry_value(""),
                        account_items = null
                    };




                    return empty_brief;

                }
            }


 
            string namestr = "";
            string sexstr = "";
       
            string birth_datestr = "";


 
            string remarkstr = "";
            string personal_ability_str = "";
            string bedloc = "";
            string absence_str = "";
            string wound_exist_str = "";

            string meal_Type_str = "";

            string pictureid = "";


            List<string> account_list = new List<string>();

            DateTime bday = new DateTime();
            DataSet ds = share.GetDataSource(Cclient.get_client_briefing(client_id));
            if(ds.Tables[0].Rows.Count>0) 
            {

                int i = 0;
                DataRow row = ds.Tables[0].Rows[0];
                namestr = row[i++].ToString() ;  
                sexstr = row[i++].ToString().Equals("M") ? "男" : "女";
                //bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);
                birth_datestr = row[i++].ToString();
                bday = DateTime.ParseExact(birth_datestr, "yyyy/MM/dd", null);
                bedloc = row[i++].ToString();
                personal_ability_str = row[i++].ToString();

                wound_exist_str = row[i++].ToString().Length>0?"Y":"N";

                absence_str = row[i++].ToString();
                if (absence_str.Length==0)
                {
                    absence_str = "NO";
                }
                if (row[20].ToString().Length>0)
                {
                    meal_Type_str = string.Format("用餐 {0}\n", row[20].ToString());
                }
                meal_Type_str = meal_Type_str + row[i++].ToString();
                pictureid = row[i++].ToString();
                if (pictureid.Length==0)
                {
                    pictureid = "0";
                }
            }
 
            if (namestr.Equals(""))
            {
                return null;
            }


            int ageint = 0;
            if (bday != null)
            {
                DateTime today = DateTime.Today;
                ageint = today.Year - bday.Year;
                if (bday > today.AddYears(-ageint)) ageint--;
            }
            string assessment_str = "";

            string drug_allergic_str = "";
            string adr_str = "";
            string other_allergic_str = "";
            string disease_str = "";
 
            if (ds.Tables[1].Rows.Count != 0)
            {


                drug_allergic_str = ds.Tables[1].Rows[0][7].ToString();
                adr_str = ds.Tables[1].Rows[0][8].ToString();
                other_allergic_str = ds.Tables[1].Rows[0][9].ToString() +(ds.Tables[1].Rows[0][10].ToString().Length>0?" "+ ds.Tables[1].Rows[0][10].ToString():"");

                disease_str = ANICshare.add_comma( ds.Tables[1].Rows[0][4].ToString())  + ANICshare.add_comma(ds.Tables[1].Rows[0][5].ToString())
                    +  ds.Tables[1].Rows[0][6].ToString();

                assessment_str = ds.Tables[1].Rows[0][11].ToString();



            }
 

            client_briefing  brief = new client_briefing { 
                name = share.encry_value( namestr),
                sex = share.encry_value(sexstr),
                age = share.encry_value(ageint.ToString()),
                bednum = share.encry_value(bedloc),
                //relative = relative_name_str,
                remark = share.encry_value(remarkstr),
                client_id = share.encry_value(client_id),
                //picturename = picturenamestr
                picture_id = share.encry_value(pictureid),
                personal_ability = share.encry_value(personal_ability_str),
                birth_date = share.encry_value(birth_datestr),
                wound_exist = share.encry_value(wound_exist_str),
       
                meal_type = share.encry_value(meal_Type_str),

                assessment_result = share.encry_value(assessment_str),
                drug_ADR = share.encry_value(adr_str),
                drug_allergic = share.encry_value(drug_allergic_str),
                other_allergic = share.encry_value(other_allergic_str),
                disease = share.encry_value(disease_str),
                absence = share.encry_value(absence_str),
                account_items = share.encry_value(account_list)
                
        };

            return brief;


        }
        public client_briefing2 get_client_briefing2_old(string client_id)
        {
            client_id = share.dencry_value(client_id);
            string[] str_arr = client_id.Split(';');
            if (str_arr.Length > 1)
            {


                if (str_arr[1] == "BED")
                {
                    DataSet da = share.GetDataSource(Cclient.get_empty_bed_info(str_arr[0]));
                    string empty_bed = "";
                    if (da.Tables[0].Rows.Count > 0)
                    {
                        empty_bed = da.Tables[0].Rows[0][0].ToString();
                    }


                    client_briefing2 empty_brief = new client_briefing2
                    {



                        //label_usage_code = label_usage_code_str,
                        //label_usage_id = label_usage_id_str,

                        name = share.encry_value(""),
                        sex = share.encry_value(""),
                        age = share.encry_value(""),
                        bednum = share.encry_value(empty_bed),
                        //relative = relative_name_str,
                        remark = share.encry_value(""),
                        client_id = share.encry_value("0"),
                        //picturename = picturenamestr
                        picture_id = share.encry_value(""),
                  
                        birth_date = share.encry_value(""),
              
                        absence = share.encry_value(""),
                   
                    };




                    return empty_brief;

                }
            }



            string namestr = "";
            string sexstr = "";
            string age = "";
            string birth_datestr = "";



            string remarkstr = "";
            string personal_ability_str = "";
            string bedloc = "";
            string absence_str = "";
            string wound_exist_str = "";
            string dining_str = "";
            string meal_Type_str = "";

            string pictureid = "";


            string self_care_str = "";
            string eating_str = "";
            string cognitive_str = "";
            string communicate_str = "";
            string visual_str = "";
            string audio_str = "";
            string incontinence_str = "";
            string lifting_str = "";
            string fake_teeth = "";
            string tools_str = "";
            string restrain_str = "";
            string memo_str = "";
            string diaper_str = "";

   
            string contact_precaution_str = "";
            string oxygen_str = "";

            string assessment_str = "";
      



            List<string> account_list = new List<string>();

            DateTime bday = new DateTime();
            DataSet ds = share.GetDataSource(Cclient.get_client_briefing2_old(client_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
             
                int i = 0;
                DataRow row = ds.Tables[0].Rows[0];
                namestr = row[i++].ToString();
                sexstr = row[i++].ToString().Equals("M") ? "男" : "女";
                birth_datestr = row[i++].ToString();
                age = row[i++].ToString();
                //bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);
      
        
                bedloc = row[i++].ToString();


 
 
                assessment_str = row[i++].ToString();
                dining_str = row[i++].ToString();
                meal_Type_str = row[i++].ToString();
                tools_str =   row[i++].ToString();
                self_care_str = row[i++].ToString();

                eating_str = row[i++].ToString();
                cognitive_str = row[i++].ToString();
                communicate_str = row[i++].ToString();
                visual_str = row[i++].ToString();
                audio_str = row[i++].ToString();
                incontinence_str = row[i++].ToString();
               diaper_str = row[i++].ToString();

 
                absence_str = row[i++].ToString();
                fake_teeth = row[i++].ToString();
                pictureid = row[i++].ToString();
                lifting_str = row[i++].ToString();
                 memo_str =row[i++].ToString();
                restrain_str = row[i++].ToString();

                wound_exist_str = row[i++].ToString()  ;
                contact_precaution_str = row[i++].ToString().Equals("Y") ? "contact_precaution" : "";
                oxygen_str = row[i++].ToString().Equals("Y") ? "氧氣機" : "";
                
                if (wound_exist_str.Length>0)
                {
                    wound_exist_str = string.Format("傷口(" + wound_exist_str+")");
                }
                
                if (absence_str.Length == 0)
                {
                    absence_str = "NO";
                }
 
         
                if (pictureid.Length == 0)
                {
                    pictureid = "0";
                }
            }

            if (namestr.Equals(""))
            {
                return null;
            }


 
            string drug_allergic_str = "";
            string adr_str = "";
            string other_allergic_str = "";
            string disease_str = "";

            if (ds.Tables[1].Rows.Count != 0)
            {


                drug_allergic_str = ds.Tables[1].Rows[0][7].ToString();
                adr_str = ds.Tables[1].Rows[0][8].ToString();
                other_allergic_str = ds.Tables[1].Rows[0][9].ToString() + (ds.Tables[1].Rows[0][10].ToString().Length > 0 ? " " + ds.Tables[1].Rows[0][10].ToString() : "");

                disease_str = ANICshare.add_comma(ds.Tables[1].Rows[0][4].ToString()) + ANICshare.add_comma(ds.Tables[1].Rows[0][5].ToString())
                    + ds.Tables[1].Rows[0][6].ToString();

                assessment_str = ds.Tables[1].Rows[0][11].ToString();



            }
            string[] other_content = new string[] { wound_exist_str,  contact_precaution_str, oxygen_str };
            List<string> otherlist = new List<string>();
            for (int i = 0; i < other_content.Length; i++)
            {
                if (other_content[i].Length>0)
                {
                    otherlist.Add(other_content[i]);
                }
            }



            string[] detail_title = new string[] { "評核","用餐","飲食","輔助工具","自理能力",
                    "進食能力", "認知能力","溝通能力", "視覺",  "聽覺",
                    "排泄","尿片","扶抱","假牙" ,"備忘錄",
                "約束物品","其他" };
            string[] detail_status = new string[] {
                    assessment_str,dining_str,"",tools_str,self_care_str,
                eating_str,cognitive_str,communicate_str,visual_str,audio_str,
                incontinence_str,   "",lifting_str,fake_teeth,"",
                "",""};
            string[] detail_description = new string[] {
                    "","",meal_Type_str,"","",
                      "","","","","",
                   "" ,diaper_str,"","",memo_str,
                restrain_str, string.Join(",",otherlist) };

            string[] detail_codes = new string[] {
                    "","","","","",
                      "","","","","",
                    "","","","","",
                "","" };

            string[] content_title = new string[] {
                    "院友資料","病歷",};
            List<client_content> contents_list = new List<client_content>();


            for (int i = 0; i < content_title.Length; i++)
            {
                List<client_detail> details_list = new List<client_detail>();
                if (i == 0)
                {

                    for (int a = 0; a < detail_title.Length; a++)
                    {
                        client_detail detail = new client_detail()
                        {
                            title = share.encry_value(detail_title[a]),
                            status = share.encry_value(detail_status[a]),
                            description = share.encry_value(detail_description[a]),
                            code = share.encry_value(detail_codes[a])

                        };
                        details_list.Add(detail);
                    }

                    client_content client_Content = new client_content()
                    {

                        details = details_list.ToArray(),
              
                        title = share.encry_value(content_title[i]) 
                    };

                    contents_list.Add(client_Content);

                }
                else
                {
                    detail_title = new string[] { "ADR", "藥物過敏", "其他過敏", "病歷" };


                    drug_allergic_str = ds.Tables[1].Rows[0][7].ToString();
                    adr_str = ds.Tables[1].Rows[0][8].ToString();
                    other_allergic_str = ds.Tables[1].Rows[0][9].ToString() + (ds.Tables[1].Rows[0][10].ToString().Length > 0 ? " " + ds.Tables[1].Rows[0][10].ToString() : "");

                    disease_str = ANICshare.add_comma(ds.Tables[1].Rows[0][4].ToString()) + ANICshare.add_comma(ds.Tables[1].Rows[0][5].ToString())
                        + ds.Tables[1].Rows[0][6].ToString();


                    detail_description = new string[] {
                     adr_str,drug_allergic_str,other_allergic_str,disease_str
                  };


                    detail_status = new string[] {
                    "","","","",  };

                    detail_codes = new string[] {
                    "","","","",  };
                    for (int a = 0; a < detail_title.Length; a++)
                    {
                        client_detail detail = new client_detail()
                        {
                            title = share.encry_value(detail_title[a]),
                            status = share.encry_value(detail_status[a]),
                            description = share.encry_value(detail_description[a]),
                            code = share.encry_value(detail_codes[a])

                        };
                        details_list.Add(detail);
                    }

                    client_content client_Content = new client_content()
                    {

                        details = details_list.ToArray(),
                        title = share.encry_value(content_title[i])
                    };

                    contents_list.Add(client_Content);

                }

            }


            client_briefing2 brief = new client_briefing2
            {
                name = share.encry_value(namestr),
                sex = share.encry_value(sexstr),
                age = share.encry_value(age),
                birth_date = share.encry_value(birth_datestr),
                bednum = share.encry_value(bedloc),

                //relative = relative_name_str,
                remark = share.encry_value(remarkstr),
                client_id = share.encry_value(client_id),
                //picturename = picturenamestr
                picture_id = share.encry_value(pictureid),



                absence = share.encry_value(absence_str),
                contents = contents_list.ToArray()

            };

            return brief;


        }

        public client_briefing2 get_client_briefing2(string client_id)
        {
            client_id = share.dencry_value(client_id);
            string[] str_arr = client_id.Split(';');
            if (str_arr.Length > 1)
            {


                if (str_arr[1] == "BED")
                {
                    DataSet da = share.GetDataSource(Cclient.get_empty_bed_info(str_arr[0]));
                    string empty_bed = "";
                    if (da.Tables[0].Rows.Count > 0)
                    {
                        empty_bed = da.Tables[0].Rows[0][0].ToString();
                    }


                    client_briefing2 empty_brief = new client_briefing2
                    {



                        //label_usage_code = label_usage_code_str,
                        //label_usage_id = label_usage_id_str,

                        name = share.encry_value(""),
                        sex = share.encry_value(""),
                        age = share.encry_value(""),
                        bednum = share.encry_value(empty_bed),
                        //relative = relative_name_str,
                        remark = share.encry_value(""),
                        client_id = share.encry_value("0"),
                        //picturename = picturenamestr
                        picture_id = share.encry_value(""),

                        birth_date = share.encry_value(""),

                        absence = share.encry_value(""),

                    };




                    return empty_brief;

                }
            }



            string namestr = "";
            string sexstr = "";
            string age = "";
            string birth_datestr = "";



            string remarkstr = "";
            string personal_ability_str = "";
            string bedloc = "";
            string absence_str = "";
            string wound_exist_str = "";
            string dining_str = "";
            string meal_Type_str = "";

            string pictureid = "";


            string self_care_str = "";
            string eating_str = "";
            string cognitive_str = "";
            string communicate_str = "";
            string visual_str = "";
            string audio_str = "";
            string incontinence_str = "";
            string lifting_str = "";
            string fake_teeth = "";
            string tools_str = "";
            string restrain_str = "";
            string memo_str = "";
            string diaper_str = "";


            string contact_precaution_str = "";
            string oxygen_str = "";

            string assessment_str = "";




            List<string> account_list = new List<string>();

            DateTime bday = new DateTime();
            DataSet ds = share.GetDataSource(Cclient.get_client_briefing2(client_id));

            if (ds != null && ds.Tables.Count > 1)
            {
                DataTable dt1 = ds.Tables[1];
                DataTable dt2 = ds.Tables[2];

                foreach (DataRow h_row in dt1.Rows)
                {
                    if (h_row["client_id"] != null)
                    {
                        foreach (DataRow m_row in dt2.AsEnumerable().Cast<DataRow>().Where(x => x["client_id"].ToString() == h_row["client_id"].ToString()))
                        {
                            if (m_row["client_id"] != null && h_row["client_id"].ToString() == m_row["client_id"].ToString())
                            {
                                h_row["drug_adverse"] = m_row["drug_adverse"];
                                h_row["drug_allergen"] = m_row["drug_allergen"];
                            }
                        }

                    }
                }
            }

            if (ds.Tables[0].Rows.Count > 0)
            {

                int i = 0;
                DataRow row = ds.Tables[0].Rows[0];
                namestr = row[i++].ToString();
                sexstr = row[i++].ToString().Equals("M") ? "男" : "女";
                birth_datestr = row[i++].ToString();
                age = row[i++].ToString();
                //bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);


                bedloc = row[i++].ToString();




                assessment_str = row[i++].ToString();
                dining_str = row[i++].ToString();
                meal_Type_str = row[i++].ToString();
                tools_str = row[i++].ToString();
                self_care_str = row[i++].ToString();

                eating_str = row[i++].ToString();
                cognitive_str = row[i++].ToString();
                communicate_str = row[i++].ToString();
                visual_str = row[i++].ToString();
                audio_str = row[i++].ToString();
                incontinence_str = row[i++].ToString();
                diaper_str = row[i++].ToString();


                absence_str = row[i++].ToString();
                fake_teeth = row[i++].ToString();
                pictureid = row[i++].ToString();
                lifting_str = row[i++].ToString();
                memo_str = row[i++].ToString();
                restrain_str = row[i++].ToString();

                wound_exist_str = row[i++].ToString();
                contact_precaution_str = row[i++].ToString().Equals("Y") ? "contact_precaution" : "";
                oxygen_str = row[i++].ToString().Equals("Y") ? "氧氣機" : "";

                if (wound_exist_str.Length > 0)
                {
                    wound_exist_str = string.Format("傷口(" + wound_exist_str + ")");
                }

                if (absence_str.Length == 0)
                {
                    absence_str = "NO";
                }


                if (pictureid.Length == 0)
                {
                    pictureid = "0";
                }
            }

            if (namestr.Equals(""))
            {
                return null;
            }



            string drug_allergic_str = "";
            string adr_str = "";
            string food_allergic_str = "";
            string other_allergic_str = "";
            string disease_str = "";
            string icd9_str = "";

            if (ds.Tables[1].Rows.Count != 0)
            {


                drug_allergic_str = ds.Tables[1].Rows[0]["drug_allergen"].ToString();
                adr_str = ds.Tables[1].Rows[0]["drug_adverse"].ToString();
                food_allergic_str = ds.Tables[1].Rows[0]["food_allergen"].ToString();
                other_allergic_str = ds.Tables[1].Rows[0]["other_allergen"].ToString();

                disease_str = ANICshare.add_comma(ds.Tables[1].Rows[0]["sickness_brief"].ToString()) + ANICshare.add_comma(ds.Tables[1].Rows[0]["diagnosis"].ToString());
                icd9_str = ds.Tables[1].Rows[0]["icd9"].ToString();

                assessment_str = ds.Tables[1].Rows[0]["assessment_result"].ToString();



            }
            string[] other_content = new string[] { wound_exist_str, contact_precaution_str, oxygen_str };
            List<string> otherlist = new List<string>();
            for (int i = 0; i < other_content.Length; i++)
            {
                if (other_content[i].Length > 0)
                {
                    otherlist.Add(other_content[i]);
                }
            }



            string[] detail_title = new string[] { "評核","用餐","飲食","輔助工具","自理能力",
                    "進食能力", "認知能力","溝通能力", "視覺",  "聽覺",
                    "排泄","尿片","扶抱","假牙" ,"備忘錄",
                "約束物品","其他" };
            string[] detail_status = new string[] {
                    assessment_str,dining_str,"",tools_str,self_care_str,
                eating_str,cognitive_str,communicate_str,visual_str,audio_str,
                incontinence_str,   "",lifting_str,fake_teeth,"",
                "",""};
            string[] detail_description = new string[] {
                    "","",meal_Type_str,"","",
                      "","","","","",
                   "" ,diaper_str,"","",memo_str,
                restrain_str, string.Join(",",otherlist) };

            string[] detail_codes = new string[] {
                    "","","","","",
                      "","","","","",
                    "","","","","",
                "","" };

            string[] content_title = new string[] {
                    "院友資料","病歷",};
            List<client_content> contents_list = new List<client_content>();


            for (int i = 0; i < content_title.Length; i++)
            {
                List<client_detail> details_list = new List<client_detail>();
                if (i == 0)
                {

                    for (int a = 0; a < detail_title.Length; a++)
                    {
                        client_detail detail = new client_detail()
                        {
                            title = share.encry_value(detail_title[a]),
                            status = share.encry_value(detail_status[a]),
                            description = share.encry_value(detail_description[a]),
                            code = share.encry_value(detail_codes[a])

                        };
                        details_list.Add(detail);
                    }

                    client_content client_Content = new client_content()
                    {

                        details = details_list.ToArray(),

                        title = share.encry_value(content_title[i])
                    };

                    contents_list.Add(client_Content);

                }
                else
                {
                    detail_title = new string[] { "ADR", "藥物過敏", "食物過敏", "其他過敏", "病歷", "ICD9" };


                    drug_allergic_str = ds.Tables[1].Rows[0]["drug_allergen"].ToString();
                    adr_str = ds.Tables[1].Rows[0]["drug_adverse"].ToString();
                    food_allergic_str = ds.Tables[1].Rows[0]["food_allergen"].ToString();
                    other_allergic_str = ds.Tables[1].Rows[0]["other_allergen"].ToString();

                    disease_str = ANICshare.add_comma(ds.Tables[1].Rows[0]["sickness_brief"].ToString()) + ANICshare.add_comma(ds.Tables[1].Rows[0]["diagnosis"].ToString());
                    icd9_str = ds.Tables[1].Rows[0]["icd9"].ToString();



                    detail_description = new string[] {
                     adr_str,drug_allergic_str,food_allergic_str,other_allergic_str,disease_str,icd9_str
                  };


                    detail_status = new string[] {
                    "","","","", "","", };

                    detail_codes = new string[] {
                    "","","","", "","", };
                    for (int a = 0; a < detail_title.Length; a++)
                    {
                        client_detail detail = new client_detail()
                        {
                            title = share.encry_value(detail_title[a]),
                            status = share.encry_value(detail_status[a]),
                            description = share.encry_value(detail_description[a]),
                            code = share.encry_value(detail_codes[a])

                        };
                        details_list.Add(detail);
                    }

                    client_content client_Content = new client_content()
                    {

                        details = details_list.ToArray(),
                        title = share.encry_value(content_title[i])
                    };

                    contents_list.Add(client_Content);

                }

            }


            client_briefing2 brief = new client_briefing2
            {
                name = share.encry_value(namestr),
                sex = share.encry_value(sexstr),
                age = share.encry_value(age),
                birth_date = share.encry_value(birth_datestr),
                bednum = share.encry_value(bedloc),

                //relative = relative_name_str,
                remark = share.encry_value(remarkstr),
                client_id = share.encry_value(client_id),
                //picturename = picturenamestr
                picture_id = share.encry_value(pictureid),



                absence = share.encry_value(absence_str),
                contents = contents_list.ToArray()

            };

            return brief;


        }


        public client_account_briefing get_client_account_briefing(string client_id)
        {
            client_id = share.dencry_value(client_id);

        
            DataSet ds = share.GetDataSource(CAccount.get_acc_uploaded_item(client_id));
            List<string> account_list = new List<string>();
            if (ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow items in ds.Tables[0].Rows)
                {
                    string item = ANICshare.add_comma(items[1].ToString()) +
                         ANICshare.add_comma(items[0].ToString()) +
                       items[2].ToString();
                    account_list.Add(item);
                }


            }
     

            client_account_briefing brief = new client_account_briefing
            {
                account_items = share.encry_value(account_list)
            };

            return brief;


        }

        public client_account_briefing2 get_client_account_briefing2(string client_id)
        {
            client_id = share.dencry_value(client_id);

            // sql.Append("select b.charge_item_name, a.charge_quantity," +
            //     "date_format(a.charge_datetime,'%Y/%m/%d'), created_by,date_format(a.created_datetime,'%Y/%m/%d %H:%i') ");

            DataSet ds = share.GetDataSource(CAccount.get_acc_uploaded_item2(client_id));
            List<client_account_detail> account_list = new List<client_account_detail>();
            if (ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow items in ds.Tables[0].Rows)
                {

                    client_account_detail detail = new client_account_detail()
                    {
                        title = share.encry_value(string.Format("{0}\n{2} * {1}", items[1].ToString(), items[2].ToString(), items[3].ToString())),
                        description = share.encry_value(string.Format("記錄 : {0} {1} ", items[4].ToString(), items[5].ToString())),
                        code = share.encry_value(""),
                        status = share.encry_value("")

                    };

                    string item = ANICshare.add_comma(items[2].ToString()) +
                         ANICshare.add_comma(items[1].ToString()) +
                       items[3].ToString();
                    account_list.Add(detail);
                }


            }


            client_account_briefing2 brief = new client_account_briefing2
            {
                account_items = account_list.ToArray()
            };

            return brief;
        }

        public client_account_briefing3 get_client_account_briefing3(string client_id)
        {
            client_id = share.dencry_value(client_id);

            // sql.Append("select b.charge_item_name, a.charge_quantity," +
            //     "date_format(a.charge_datetime,'%Y/%m/%d'), created_by,date_format(a.created_datetime,'%Y/%m/%d %H:%i') ");

            DataSet ds = share.GetDataSource(CAccount.get_acc_uploaded_item2(client_id));
            List<client_account_detail2> account_list = new List<client_account_detail2>();
            if (ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow items in ds.Tables[0].Rows)
                {

                    client_account_detail2 detail = new client_account_detail2()
                    {
                        id = share.encry_value(items["consumed_id"]?.ToString()),
                        title = share.encry_value(items["charge_item_name"]?.ToString()),
                        description = share.encry_value(string.Format("記錄 : {0} {1} ", items["created_by"].ToString(), items["created_datetime"].ToString())),
                        code = share.encry_value(""),
                        status = share.encry_value(""),
                        record_qty = share.encry_value( items["charge_quantity"].ToString()),
                        record_date = share.encry_value(items["record_date"].ToString()),
                        created_by = share.encry_value( items["created_by"].ToString()),
                        created_datetime = share.encry_value(items["created_datetime"].ToString())
                    };

                    string item = ANICshare.add_comma(items[1].ToString()) +
                         ANICshare.add_comma(items[0].ToString()) +
                       items[2].ToString();
                    account_list.Add(detail);
                }


            }


            client_account_briefing3 brief = new client_account_briefing3
            {
                account_items = account_list.ToArray()
            };

            return brief;
        }
        public string get_client_picture_id(string client_id)
        {
            client_id = share.dencry_value(client_id);
            string id = "0";
            DataSet dp =  share.GetDataSource(Cclient.get_client_picture_id(client_id));
            if (dp.Tables[0].Rows.Count > 0)
            {
                id = share.encry_value(dp.Tables[0].Rows[0][0].ToString());
            }
            return id;
        }
        public insert_client_photo post_client_photo_data(string client_id, String photo_data)
        {
            ///SELECT* FROM ani_smp.client_documents;
            //client_id, document_photo
            //int next_id_int = 0;
            ///string next_id_str = "0";
            client_id = share.dencry_value(client_id);
            string current_id = share.Get_mysql_database_value("client_personal2", "client_id", client_id, "client_number", " and client_number !='' ");
            if (current_id==null)
            {
                return null;
            }
            string current = share.get_current_time();
            string id = share.Get_mysql_database_MaxID("client_documents2", "client_photo_id");

            string[] values = new string[] { id, client_id };
            //byte[] image_bytes = Convert.FromBase64String(photo_data);
            int a = share.ExecBoldSql(Cclient.post_client_photo(values), photo_data);
            if (a == 0)
            {
         
                return null;

            }
            else
            {
                if (panel_exist)
                {
                    inform_panel_client(client_id);

                }



                String str = "YES";
                insert_client_photo insertstate =
                new insert_client_photo()
                {

                    client_photo_update = str

                };
                return insertstate;
       

            }
            //share.ExecBoldSql(eny.decrypt(sql), image);
            /*
            string monthly_charge_id = share.Get_mysql_database_value("acc_monthly_count_charge", "charge_item_id", charge_item_id, "monthly_count_charge_id");
            if (monthly_charge_id == null)
            {


                string acc_num = share.Get_mysql_database_value("acc_account2", "client_id", client_id, "account_num");
                string unit_price_str = share.Get_mysql_database_value("acc_charge_item2", "charge_item_id", charge_item_id, "charge_item_unit_price");

                float unit_price_float = float.Parse(unit_price_str);
                float total_amount = unit_price_float * int.Parse(quantity);

                string charge_id = share.Get_mysql_database_MaxID("acc_charge3", "charge_id");
                string[] value = { charge_id, acc_num, "0", charge_item_id, "P", quantity, total_amount.ToString(),
            timestamp, "", handlingperson, timestamp };
 
                int a = share.Ecxe_transaction_Sql(CAccount.post_charge_item(value));

                if (a > 0)
                {
                    String str = "YES";
                    insert_acc_data insertstate = new insert_acc_data
                    {

                        result = str
                    };
                    return insertstate;
                }
                else
                {
                    return null;
                }
            }
            else
            {

                string consume_id = Cshare.Get_mysql_database_MaxID("inv_consumed_item", "inv_consumed_id");
                string inv_item_id = Cshare.Get_mysql_database_value("inv_item", "charge_item_id", charge_item_id, "inv_item_id");
                //string inv_item_id = Cshare.Get_mysql_database_MaxID("inv_consumed_item", "inv_consumed_id");

                string[] value = { consume_id, inv_item_id, client_id, charge_item_id, "0", quantity, "N","Y",
                handlingperson, timestamp };
                StringBuilder sql = new StringBuilder();
                sql.Append("insert inv_consumed_item(inv_consumed_id, inv_item_id, client_id, charge_item_id, charge_id, inv_quantity, inv_item_charge_status, valid,");
                sql.Append("created_by, created_datetime)");
                sql.Append("VALUES({0},'{1}',{2},{3},'{4}',{5},'{6}','{7}','{8}','{9}')");

                int a = Cshare.Ecxe_transaction_Sql(string.Format(sql.ToString(), value));

                if (a > 0)
                {
                    String str = "YES";
                    insert_acc_data insertstate = new insert_acc_data
                    {

                        result = share.encry_value(str)
                    };
                    return insertstate;
                }
                else
                {
                    return null;
                }
            }
            */





        }
        public measure_briefing get_measure_briefing(string client_id)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";

            client_id = share.dencry_value(client_id);
            string blood_pressure_str = "";
            string blood_oxygen_str = "";
            string blood_glucose_str = "";
            string respiration_rate_str = "";
            string body_temperature_str = "";
            string body_weight_str = "";

            List<string> blood_pressure_record = new List<string>();
            List<string> blood_oxygen_record = new List<string>();
            List<string> blood_glucose_record = new List<string>();
            List<string> respiration_rate_record = new List<string>();
            List<string> temperature_record = new List<string>();
            List<string> weight_record = new List<string>();

            List<List<string>> alllist = new List<List<string>>() { blood_pressure_record,blood_oxygen_record,
                blood_glucose_record,respiration_rate_record,temperature_record,weight_record };
            string[] brief_arr = new string[] { blood_pressure_str, blood_oxygen_str, blood_glucose_str,
                respiration_rate_str, body_temperature_str, body_weight_str };
 
 

            DataSet ds = share.GetDataSource(Cmedical.get_measure_brief(client_id));

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (ds.Tables[i].Rows.Count != 0)
                {
                    brief_arr[i] = ds.Tables[i].Rows[0][0].ToString() ;
                    foreach (DataRow item in ds.Tables[i].Rows)
                    {
                        string[] record = { item[0].ToString(), item[0].ToString(), item[0].ToString() };
                        alllist[i].Add(string.Join(";", record));
                    }
  
                    //if (brief_arr[i].Split(';').Length==0)
                   // {
                   //     brief_arr[i] = "0";
                   // }
                }
                else
                {
                  //  brief_arr[i] = "0";
                }
            }

            measure_briefing  brief = new measure_briefing  (){
                    blood_pressure = share.encry_value(brief_arr[0]),
                    blood_oxygen = share.encry_value(brief_arr[1] ),
                    blood_glucose = share.encry_value(brief_arr[2]) ,
                    respiration_rate = share.encry_value(brief_arr[3]) ,
                    body_temperature = share.encry_value(brief_arr[4]) ,
                    body_weight = share.encry_value(brief_arr[5]),
                blood_pressure_list = share.encry_value( blood_pressure_record),
                blood_oxygen_list = share.encry_value(blood_oxygen_record),
                blood_glucose_list = share.encry_value(blood_glucose_record),
                respiration_rate_list = share.encry_value(respiration_rate_record),
                temperature_list = share.encry_value(temperature_record),
                weight_list = share.encry_value(weight_record)

            };

            return brief;
        }
       
        public wound_events get_wound_briefing(string client_id)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";

            client_id = share.dencry_value(client_id);
 

     
            List<wound_event> woundrecord = new List<wound_event>();


 
      

            DataSet ds = share.GetDataSource(CWound.get_wound_brief(string.Format("and per.client_id = {0} and wound.recovery = 'N' ", client_id)));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                wound_event eve = new wound_event();
 
        /*
        sql.Append("select wound.care_wound_id,IFNULL(concat(per.chi_surname,per.chi_name),''),wound.client_id,");
                sql.Append(" concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),wound.recovery,");
                sql.Append(" wound.wound_position_id,pos.position_chi_name,wound.clean_frequency,");

                sql.Append("DATE_FORMAT(wound.next_clean_datetime, '%Y-%m-%d %H:%i'),");

                sql.Append("DATE_FORMAT(wound.start_datetime, '%Y-%m-%d'),");
                sql.Append("DATE_FORMAT(wound.end_datetime, '%Y-%m-%d'),wound.wound_report_source ,count(obs.observation_id),");
                sql.Append(" group_concat(dres.care_wound_material_id order by dres.preset_id separator ';') ,");
                sql.Append(" group_concat(mat.care_wound_material_id,' ',dres.remark order by dres.preset_id separator ';') ,");
                sql.Append("substring_index(group_concat(DISTINCT obs.observation_id order by obs.observation_date separator ';'),';', 5) as records,");


                sql.Append("  obs1.concat( obs1.wound_length,';', obs1.wound_width,';', obs1.wound_depth),");
                sql.Append("  obs1.wound_color,  obs1.wound_fluid_type,  obs1.wound_smell,  obs1.fluid_quantity,  ");
                sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime  ");
   */
                DataRow row = ds.Tables[0].Rows[i];
                int index   = 0;
                eve.care_wound_id = share.encry_value(row[index++].ToString());
                index++;
                   index++;
                   index++;
                     index++;
                //string wound_state = row[indexe++] ==? "N" ? "未復原" : "已復原";
                eve.wound_position_id = share.encry_value(row[index++].ToString());
                eve.wound_position = share.encry_value(row[index++].ToString());
                eve.frequency = share.encry_value(row[index++].ToString());
                eve.clean_days = share.encry_value(row[index++].ToString());
                eve.clean_times = share.encry_value(row[index++].ToString());
                eve.next_date = share.encry_value(row[index++].ToString());
                eve.start_date = share.encry_value(row[index++].ToString());
                index++;
                index++;
                eve.count = share.encry_value(row[index++].ToString());


                List<wound_dressing> dressing = new List<wound_dressing>();
                string dress_ids_str = row[index++].ToString();
                if (dress_ids_str.Length > 0)
                {


                    string[] dressing_ids = dress_ids_str.Split(new[] { "@dress_sep@" }, StringSplitOptions.None);

                    //    string []dressing_name = row[index++].ToString().Split(';'); ;
 
                    for (int a = 0; a < dressing_ids.Length; a++)
                    {
                        wound_dressing dress = new wound_dressing();
                        //    string[] id_array = dressing_ids[a].Split(new[] { "," }, StringSplitOptions.None);
                        //    dress.id = share.encry_value(id_array[1]);
                        string[] id_array = dressing_ids[a].Split(new[] { "@dress" }, StringSplitOptions.None);
                        dress.id = share.encry_value(id_array[1]);
                        dress.name = share.encry_value(id_array[2]);
                        if (id_array.Length > 3)
                        {
                            dress.remark = share.encry_value(id_array[3]);
                        }
                        dressing.Add(dress);
                    }
                }

                //       eve.dressing_ids = share.encry_value(row[index++].ToString());
                //       eve.dressing_names = share.encry_value(row[index++].ToString());
                eve.dressing = dressing.ToArray();

                eve.last_ids = share.encry_value(row[index++].ToString());
                wound_last_content last = new wound_last_content();
                last.dimens = share.encry_value(row[index++].ToString());
                last.level = share.encry_value(row[index++].ToString());

                last.color = share.encry_value(row[index++].ToString());
                last.fuild_type = share.encry_value(row[index++].ToString());
                last.smell = share.encry_value(row[index++].ToString());
                last.fuild_quanity = share.encry_value(row[index++].ToString());

                index++;
                index++;
                index++;
                index++;
                index++;

                last.last_photo_id = share.encry_value(row[index].ToString());
               // sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime , ");
               // sql.Append("ifnull(doc.wound_photo_id,0)  ");
                eve.record = last;
                woundrecord.Add(eve);
            }
            if (woundrecord.Count==0)
            {
                return null;
            }
            wound_wash_parameter para = new wound_wash_parameter();
            for (int i = 1; i < ds.Tables.Count; i++)
            {
                if (ds.Tables[i].Rows.Count != 0)
                {
        
                    if (i==1)
                    {
                        para.level = set_wound_parameter(ds.Tables[i], para.level);
                    }
                    else if (i == 2)
                    {
                        para.color = set_wound_parameter(ds.Tables[i], para.color);
                    }
                    else if (i ==3)
                    {
                        para.fuild_type = set_wound_parameter(ds.Tables[i], para.fuild_type);
                    }
                    else if (i == 4)
                    {
                        para.smell = set_wound_parameter(ds.Tables[i], para.smell);
                    }
                    else if (i == 5)
                    {
                        para.fuild_quanity = set_wound_parameter(ds.Tables[i], para.fuild_quanity);
                    }
 
                }
                else
                {
                    //  brief_arr[i] = "0";
                }
            }
            para.lengthmax = share.encry_value("20");
            para.widthmax = share.encry_value("20");
            para.depthmax = share.encry_value("20");
            wound_events brief = new wound_events()
            {
            events    = woundrecord.ToArray()     
                ,wash = para

            };

            return brief;
        }

        /*  // Raymond @20210119 wound XY change para
        
        public wound_events get_wound_briefing2(string client_id)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";

            client_id = share.dencry_value(client_id);



            List<wound_event> woundrecord = new List<wound_event>();





            DataSet ds = share.GetDataSource(CWound.get_wound_brief2(string.Format("and per.client_id = {0} and wound.recovery = 'N' ", client_id)));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                wound_event eve = new wound_event();

                
                //sql.Append("select wound.care_wound_id,IFNULL(concat(per.chi_surname,per.chi_name),''),wound.client_id,");
                        //sql.Append(" concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),wound.recovery,");
                        //sql.Append(" wound.wound_position_id,pos.position_chi_name,wound.clean_frequency,");

                        //sql.Append("DATE_FORMAT(wound.next_clean_datetime, '%Y-%m-%d %H:%i'),");

                        //sql.Append("DATE_FORMAT(wound.start_datetime, '%Y-%m-%d'),");
                        //sql.Append("DATE_FORMAT(wound.end_datetime, '%Y-%m-%d'),wound.wound_report_source ,count(obs.observation_id),");
                        ///sql.Append(" group_concat(dres.care_wound_material_id order by dres.preset_id separator ';') ,");
                        /sql.Append(" group_concat(mat.care_wound_material_id,' ',dres.remark order by dres.preset_id separator ';') ,");
                        //sql.Append("substring_index(group_concat(DISTINCT obs.observation_id order by obs.observation_date separator ';'),';', 5) as records,");


                        //sql.Append("  obs1.concat( obs1.wound_length,';', obs1.wound_width,';', obs1.wound_depth),");
                        //sql.Append("  obs1.wound_color,  obs1.wound_fluid_type,  obs1.wound_smell,  obs1.fluid_quantity,  ");
                        //sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime  ");
           
                DataRow row = ds.Tables[0].Rows[i];
                int index = 0;
                eve.care_wound_id = share.encry_value(row[index++].ToString());
                index++;
                index++;
                index++;
                index++;
                //string wound_state = row[indexe++] ==? "N" ? "未復原" : "已復原";
                eve.wound_position_id = share.encry_value(row[index++].ToString());
                eve.wound_position = share.encry_value(row[index++].ToString());
                eve.frequency = share.encry_value(row[index++].ToString());
                eve.clean_days = share.encry_value(row[index++].ToString());
                eve.clean_times = share.encry_value(row[index++].ToString());
                eve.next_date = share.encry_value(row[index++].ToString());
                eve.start_date = share.encry_value(row[index++].ToString());
                index++;
                index++;
                eve.count = share.encry_value(row[index++].ToString());


                List<wound_dressing> dressing = new List<wound_dressing>();
                string dress_ids_str = row[index++].ToString();
                if (dress_ids_str.Length > 0)
                {


                    string[] dressing_ids = dress_ids_str.Split(new[] { "@dress_sep@" }, StringSplitOptions.None);

                    //    string []dressing_name = row[index++].ToString().Split(';'); ;

                    for (int a = 0; a < dressing_ids.Length; a++)
                    {
                        wound_dressing dress = new wound_dressing();
                        //    string[] id_array = dressing_ids[a].Split(new[] { "," }, StringSplitOptions.None);
                        //    dress.id = share.encry_value(id_array[1]);
                        string[] id_array = dressing_ids[a].Split(new[] { "@dress" }, StringSplitOptions.None);
                        dress.id = share.encry_value(id_array[1]);
                        dress.name = share.encry_value(id_array[2]);
                        if (id_array.Length > 3)
                        {
                            dress.remark = share.encry_value(id_array[3]);
                        }
                        dressing.Add(dress);
                    }
                }

                //       eve.dressing_ids = share.encry_value(row[index++].ToString());
                //       eve.dressing_names = share.encry_value(row[index++].ToString());
                eve.dressing = dressing.ToArray();

                eve.last_ids = share.encry_value(row[index++].ToString());
                wound_last_content last = new wound_last_content();
                last.dimens = share.encry_value(row[index++].ToString());
                last.level = share.encry_value(row[index++].ToString());

                last.color = share.encry_value(row[index++].ToString());
                last.fuild_type = share.encry_value(row[index++].ToString());
                last.smell = share.encry_value(row[index++].ToString());
                last.fuild_quanity = share.encry_value(row[index++].ToString());

                index++;
                index++;
                index++;
                index++;
                index++;

                last.last_photo_id = share.encry_value(row[index++].ToString());


                eve.wound_positionx = share.encry_value(row[index++].ToString());
                eve.wound_positiony = share.encry_value(row[index++].ToString());
                eve.wound_position_direction = share.encry_value(row[index++].ToString());
                eve.wound_position_name = share.encry_value(row[index++].ToString());

                // sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime , ");
                // sql.Append("ifnull(doc.wound_photo_id,0)  ");
                eve.record = last;
                woundrecord.Add(eve);
            }
            if (woundrecord.Count == 0)
            {
                return null;
            }
            wound_wash_parameter para = new wound_wash_parameter();
            for (int i = 1; i < ds.Tables.Count; i++)
            {
                if (ds.Tables[i].Rows.Count != 0)
                {

                    if (i == 1)
                    {
                        para.level = set_wound_parameter(ds.Tables[i], para.level);
                    }
                    else if (i == 2)
                    {
                        para.color = set_wound_parameter(ds.Tables[i], para.color);
                    }
                    else if (i == 3)
                    {
                        para.fuild_type = set_wound_parameter(ds.Tables[i], para.fuild_type);
                    }
                    else if (i == 4)
                    {
                        para.smell = set_wound_parameter(ds.Tables[i], para.smell);
                    }
                    else if (i == 5)
                    {
                        para.fuild_quanity = set_wound_parameter(ds.Tables[i], para.fuild_quanity);
                    }

                }
                else
                {
                    //  brief_arr[i] = "0";
                }
            }
            para.lengthmax = share.encry_value("20");
            para.widthmax = share.encry_value("20");
            para.depthmax = share.encry_value("20");
            wound_events brief = new wound_events()
            {
                events = woundrecord.ToArray()
                ,
                wash = para

            };

            return brief;
        }
        */

        // Raymond @20201209  coordinates 
        public wound_events_2 get_wound_briefing2(string client_id)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";

            client_id = share.dencry_value(client_id);



            List<wound_event> woundrecord = new List<wound_event>();





            DataSet ds = share.GetDataSource(CWound.get_wound_brief2(string.Format("and per.client_id = {0} and wound.recovery = 'N' ", client_id)));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                wound_event eve = new wound_event();

                /*
                sql.Append("select wound.care_wound_id,IFNULL(concat(per.chi_surname,per.chi_name),''),wound.client_id,");
                        sql.Append(" concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),wound.recovery,");
                        sql.Append(" wound.wound_position_id,pos.position_chi_name,wound.clean_frequency,");

                        sql.Append("DATE_FORMAT(wound.next_clean_datetime, '%Y-%m-%d %H:%i'),");

                        sql.Append("DATE_FORMAT(wound.start_datetime, '%Y-%m-%d'),");
                        sql.Append("DATE_FORMAT(wound.end_datetime, '%Y-%m-%d'),wound.wound_report_source ,count(obs.observation_id),");
                        sql.Append(" group_concat(dres.care_wound_material_id order by dres.preset_id separator ';') ,");
                        sql.Append(" group_concat(mat.care_wound_material_id,' ',dres.remark order by dres.preset_id separator ';') ,");
                        sql.Append("substring_index(group_concat(DISTINCT obs.observation_id order by obs.observation_date separator ';'),';', 5) as records,");


                        sql.Append("  obs1.concat( obs1.wound_length,';', obs1.wound_width,';', obs1.wound_depth),");
                        sql.Append("  obs1.wound_color,  obs1.wound_fluid_type,  obs1.wound_smell,  obs1.fluid_quantity,  ");
                        sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime  ");
           */
                DataRow row = ds.Tables[0].Rows[i];
                int index = 0;
                eve.care_wound_id = share.encry_value(row[index++].ToString());
                index++;
                index++;
                index++;
                index++;
                //string wound_state = row[indexe++] ==? "N" ? "未復原" : "已復原";
                eve.wound_position_remark = share.encry_value(row[index++].ToString());                // RAymond @20201216

                eve.wound_position_id = share.encry_value(row[index++].ToString());
                eve.wound_position = share.encry_value(row[index++].ToString());
                eve.frequency = share.encry_value(row[index++].ToString());
                eve.clean_days = share.encry_value(row[index++].ToString());
                eve.clean_times = share.encry_value(row[index++].ToString());
                eve.next_date = share.encry_value(row[index++].ToString());
                eve.start_date = share.encry_value(row[index++].ToString());
                index++;
                index++;
                eve.count = share.encry_value(row[index++].ToString());


                List<wound_dressing> dressing = new List<wound_dressing>();
                string dress_ids_str = row[index++].ToString();
                if (dress_ids_str.Length > 0)
                {


                    string[] dressing_ids = dress_ids_str.Split(new[] { "@dress_sep@" }, StringSplitOptions.None);

                    //    string []dressing_name = row[index++].ToString().Split(';'); ;

                    for (int a = 0; a < dressing_ids.Length; a++)
                    {
                        wound_dressing dress = new wound_dressing();
                        //    string[] id_array = dressing_ids[a].Split(new[] { "," }, StringSplitOptions.None);
                        //    dress.id = share.encry_value(id_array[1]);
                        string[] id_array = dressing_ids[a].Split(new[] { "@dress" }, StringSplitOptions.None);

                        dress.id = share.encry_value(id_array[1]);
                        dress.name = share.encry_value(id_array[2]);


                        if (id_array.Length > 3)
                        {
                            dress.remark = share.encry_value(id_array[3]);
                        }
                        dressing.Add(dress);
                    }
                }

                //       eve.dressing_ids = share.encry_value(row[index++].ToString());
                //       eve.dressing_names = share.encry_value(row[index++].ToString());
                eve.dressing = dressing.ToArray();

                eve.last_ids = share.encry_value(row[index++].ToString());
                wound_last_content last = new wound_last_content();
                last.dimens = share.encry_value(row[index++].ToString());
                last.level = share.encry_value(row[index++].ToString());

                last.color = share.encry_value(row[index++].ToString());
                last.fuild_type = share.encry_value(row[index++].ToString());
                last.smell = share.encry_value(row[index++].ToString());
                last.fuild_quanity = share.encry_value(row[index++].ToString());

                index++;
                index++;
                index++;
                index++;
                index++;

                last.last_photo_id = share.encry_value(row[index++].ToString());


                eve.wound_positionx = share.encry_value(row[index++].ToString());
                eve.wound_positiony = share.encry_value(row[index++].ToString());
                eve.wound_position_direction = share.encry_value(row[index++].ToString());
                eve.wound_position_name = share.encry_value(row[index++].ToString());

                // sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime , ");
                // sql.Append("ifnull(doc.wound_photo_id,0)  ");
                eve.record = last;
                woundrecord.Add(eve);
            }
            if (woundrecord.Count == 0)
            {
                return null;
            }
            wound_wash_parameter_2 para = new wound_wash_parameter_2();
            for (int i = 1; i < ds.Tables.Count; i++)
            {
                if (ds.Tables[i].Rows.Count != 0)
                {

                    if (i == 1)
                    {
                        para.level = set_wound_parameter_2(ds.Tables[i]);
                    }
                    else if (i == 2)
                    {
                        para.color = set_wound_parameter_2(ds.Tables[i]);
                    }
                    else if (i == 3)
                    {
                        para.fuild_type = set_wound_parameter_2(ds.Tables[i]);
                    }
                    else if (i == 4)
                    {
                        para.smell = set_wound_parameter_2(ds.Tables[i]);
                    }
                    else if (i == 5)
                    {
                        para.fuild_quanity = set_wound_parameter_2(ds.Tables[i]);
                    }

                }
                else
                {
                    //  brief_arr[i] = "0";
                }
            }
            para.lengthmax = share.encry_value("20");
            para.widthmax = share.encry_value("20");
            para.depthmax = share.encry_value("20");
            wound_events_2 brief = new wound_events_2()
            {
                events = woundrecord.ToArray()
                ,
                wash = para

            };


            return brief;
        }

        //Raymond @20201015 change para format
        public wound_events_2 get_wound_briefing_3(string client_id)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";

            client_id = share.dencry_value(client_id);



            List<wound_event> woundrecord = new List<wound_event>();


            DataSet ds = share.GetDataSource(CWound.get_wound_brief3(string.Format("and per.client_id = {0} and wound.recovery = 'N' ", client_id)));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                wound_event eve = new wound_event();

                /*
                sql.Append("select wound.care_wound_id,IFNULL(concat(per.chi_surname,per.chi_name),''),wound.client_id,");
                        sql.Append(" concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),wound.recovery,");
                        sql.Append(" wound.wound_position_id,pos.position_chi_name,wound.clean_frequency,");

                        sql.Append("DATE_FORMAT(wound.next_clean_datetime, '%Y-%m-%d %H:%i'),");

                        sql.Append("DATE_FORMAT(wound.start_datetime, '%Y-%m-%d'),");
                        sql.Append("DATE_FORMAT(wound.end_datetime, '%Y-%m-%d'),wound.wound_report_source ,count(obs.observation_id),");
                        sql.Append(" group_concat(dres.care_wound_material_id order by dres.preset_id separator ';') ,");
                        sql.Append(" group_concat(mat.care_wound_material_id,' ',dres.remark order by dres.preset_id separator ';') ,");
                        sql.Append("substring_index(group_concat(DISTINCT obs.observation_id order by obs.observation_date separator ';'),';', 5) as records,");


                        sql.Append("  obs1.concat( obs1.wound_length,';', obs1.wound_width,';', obs1.wound_depth),");
                        sql.Append("  obs1.wound_color,  obs1.wound_fluid_type,  obs1.wound_smell,  obs1.fluid_quantity,  ");
                        sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime  ");
           */
                DataRow row = ds.Tables[0].Rows[i];
                int index = 0;
                eve.care_wound_id = share.encry_value(row[index++].ToString());
                index++;
                index++;
                index++;
                index++;
                //string wound_state = row[indexe++] ==? "N" ? "未復原" : "已復原";
                eve.wound_position_remark = share.encry_value(row[index++].ToString());                // RAymond @20201216

                eve.wound_position_id = share.encry_value(row[index++].ToString());
                eve.wound_position = share.encry_value(row[index++].ToString());
                eve.frequency = share.encry_value(row[index++].ToString());
                eve.clean_days = share.encry_value(row[index++].ToString());
                eve.clean_times = share.encry_value(row[index++].ToString());
                eve.next_date = share.encry_value(row[index++].ToString());
                eve.start_date = share.encry_value(row[index++].ToString());
                index++;
                index++;
                eve.count = share.encry_value(row[index++].ToString());


                List<wound_dressing> dressing = new List<wound_dressing>();
                string dress_ids_str = row[index++].ToString();
                if (dress_ids_str.Length > 0)
                {


                    string[] dressing_ids = dress_ids_str.Split(new[] { "@dress_sep@" }, StringSplitOptions.None);

                    //    string []dressing_name = row[index++].ToString().Split(';'); ;

                    for (int a = 0; a < dressing_ids.Length; a++)
                    {
                        wound_dressing dress = new wound_dressing();
                        //    string[] id_array = dressing_ids[a].Split(new[] { "," }, StringSplitOptions.None);
                        //    dress.id = share.encry_value(id_array[1]);
                        string[] id_array = dressing_ids[a].Split(new[] { "@dress" }, StringSplitOptions.None);
                        dress.id = share.encry_value(id_array[1]);
                        dress.name = share.encry_value(id_array[2]);
                        if (id_array.Length > 3)
                        {
                            dress.remark = share.encry_value(id_array[3]);
                        }
                        dressing.Add(dress);
                    }
                }

                //       eve.dressing_ids = share.encry_value(row[index++].ToString());
                //       eve.dressing_names = share.encry_value(row[index++].ToString());
                eve.dressing = dressing.ToArray();

                eve.last_ids = share.encry_value(row[index++].ToString());
                wound_last_content last = new wound_last_content();
                last.dimens = share.encry_value(row[index++].ToString());
                last.level = share.encry_value(row[index++].ToString());

                last.color = share.encry_value(row[index++].ToString());
                last.fuild_type = share.encry_value(row[index++].ToString());
                last.smell = share.encry_value(row[index++].ToString());
                last.fuild_quanity = share.encry_value(row[index++].ToString());

                index++;
                index++;
                index++;
                index++;
                index++;

                last.last_photo_id = share.encry_value(row[index].ToString());
                // sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime , ");
                // sql.Append("ifnull(doc.wound_photo_id,0)  ");
                eve.record = last;
                woundrecord.Add(eve);
            }
            if (woundrecord.Count == 0)
            {
                return null;
            }
            wound_wash_parameter_2 para = new wound_wash_parameter_2();
            for (int i = 1; i < ds.Tables.Count; i++)
            {
                if (ds.Tables[i].Rows.Count != 0)
                {

                    if (i == 1)
                    {
                        para.level = set_wound_parameter_2(ds.Tables[i]);
                    }
                    else if (i == 2)
                    {
                        para.color = set_wound_parameter_2(ds.Tables[i]);
                    }
                    else if (i == 3)
                    {
                        para.fuild_type = set_wound_parameter_2(ds.Tables[i]);
                    }
                    else if (i == 4)
                    {
                        para.smell = set_wound_parameter_2(ds.Tables[i]);
                    }
                    else if (i == 5)
                    {
                        para.fuild_quanity = set_wound_parameter_2(ds.Tables[i]);
                    }

                }
                else
                {
                    //  brief_arr[i] = "0";
                }
            }
            para.lengthmax = share.encry_value("20");
            para.widthmax = share.encry_value("20");
            para.depthmax = share.encry_value("20");
            wound_events_2 brief = new wound_events_2()
            {
                events = woundrecord.ToArray()
                ,
                wash = para

            };

            return brief;
        }



        public wound_wash_event get_wound_details(string wound_id)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";

            wound_id = share.dencry_value(wound_id);
 
            List<wound_wash> woundrecord = new List<wound_wash>();

 

            DataSet ds = share.GetDataSource(CWound.get_wound_wash_detail(string.Format("and obs.care_wound_id = {0} group by observation_id order by obs.observation_date desc limit 5 ", wound_id )));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                wound_wash eve = new wound_wash();

                /*
                sql.Append("select wound.care_wound_id,IFNULL(concat(per.chi_surname,per.chi_name),''),wound.client_id,");
                        sql.Append(" concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),wound.recovery,");
                        sql.Append(" wound.wound_position_id,pos.position_chi_name,wound.clean_frequency,");

                        sql.Append("DATE_FORMAT(wound.next_clean_datetime, '%Y-%m-%d %H:%i'),");

                        sql.Append("DATE_FORMAT(wound.start_datetime, '%Y-%m-%d'),");
                        sql.Append("DATE_FORMAT(wound.end_datetime, '%Y-%m-%d'),wound.wound_report_source ,count(obs.observation_id),");
                        sql.Append(" group_concat(dres.care_wound_material_id order by dres.preset_id separator ';') ,");
                        sql.Append(" group_concat(mat.care_wound_material_id,' ',dres.remark order by dres.preset_id separator ';') ,");
                        sql.Append("substring_index(group_concat(DISTINCT obs.observation_id order by obs.observation_date separator ';'),';', 5) as records,");


                        sql.Append("  obs1.concat( obs1.wound_length,';', obs1.wound_width,';', obs1.wound_depth),");
                        sql.Append("  obs1.wound_color,  obs1.wound_fluid_type,  obs1.wound_smell,  obs1.fluid_quantity,  ");
                        sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime  ");
           */
                DataRow row = ds.Tables[0].Rows[i];
   
                int index = 0;
                eve.observation_id = share.encry_value(row[index++].ToString());
                eve.wash_date = share.encry_value(row[index++].ToString());
                eve.length = share.encry_value(row[index++].ToString());
                eve.width = share.encry_value(row[index++].ToString());
                eve.depth = share.encry_value(row[index++].ToString());
                eve.level = share.encry_value(row[index++].ToString());
                eve.color = share.encry_value(row[index++].ToString());
                eve.smell = share.encry_value(row[index++].ToString());
                eve.fuild_type = share.encry_value(row[index++].ToString());
                eve.fuild_quanity = share.encry_value(row[index++].ToString());
                index++;
                string dressingstr = row[index++].ToString();
 
                List<wound_dressing> dressing = new List<wound_dressing>();
                if (dressingstr.Length > 0)
                {


                    string[] dressing_ids = dressingstr.Split(new[] { "@dress_sep@" }, StringSplitOptions.None);

                    //    string []dressing_name = row[index++].ToString().Split(';'); ;

                    for (int a = 0; a < dressing_ids.Length; a++)
                    {
                        wound_dressing dress = new wound_dressing();
                        //    string[] id_array = dressing_ids[a].Split(new[] { "," }, StringSplitOptions.None);
                        //    dress.id = share.encry_value(id_array[1]);
                        string[] id_array = dressing_ids[a].Split(new[] { "@dress" }, StringSplitOptions.None);
                        dress.id = share.encry_value(id_array[1]);
                        dress.name = share.encry_value(id_array[2]);
                        if (id_array.Length > 3)
                        {
                            dress.remark = share.encry_value(id_array[3]);
                        }
                        dressing.Add(dress);
                    }
                    eve.dressing = dressing.ToArray();
                }

                //       eve.dressing_ids = share.encry_value(row[index++].ToString());
                //       eve.dressing_names = share.encry_value(row[index++].ToString());
                
                eve.remark = share.encry_value(row[index++].ToString());
                eve.created_by = share.encry_value(row[index++].ToString());
                eve.created_datetime = share.encry_value(row[index++].ToString());
                eve.modified_by = share.encry_value(row[index++].ToString());
                eve.modified_datetime = share.encry_value(ANICshare.Null_time_check_no_second_hyphen( row[index++].ToString()));
                eve.wound_photo_id = share.encry_value(row[index++].ToString());



 
                woundrecord.Add(eve);
            }
            if (woundrecord.Count == 0)
            {
                return null;
            }

            wound_wash_event brief = new wound_wash_event()
            {
                events = woundrecord.ToArray()
         
            };

            return brief;
        }
        public wound_wash_event get_wound_details2(string wound_id)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";

            wound_id = share.dencry_value(wound_id);

            List<wound_wash> woundrecord = new List<wound_wash>();



            DataSet ds = share.GetDataSource(CWound.get_wound_wash_detail2(string.Format("and obs.care_wound_id = {0} group by observation_id order by obs.observation_date desc limit 5 ", wound_id)));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                wound_wash eve = new wound_wash();

                /*
                sql.Append("select wound.care_wound_id,IFNULL(concat(per.chi_surname,per.chi_name),''),wound.client_id,");
                        sql.Append(" concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),wound.recovery,");
                        sql.Append(" wound.wound_position_id,pos.position_chi_name,wound.clean_frequency,");

                        sql.Append("DATE_FORMAT(wound.next_clean_datetime, '%Y-%m-%d %H:%i'),");

                        sql.Append("DATE_FORMAT(wound.start_datetime, '%Y-%m-%d'),");
                        sql.Append("DATE_FORMAT(wound.end_datetime, '%Y-%m-%d'),wound.wound_report_source ,count(obs.observation_id),");
                        sql.Append(" group_concat(dres.care_wound_material_id order by dres.preset_id separator ';') ,");
                        sql.Append(" group_concat(mat.care_wound_material_id,' ',dres.remark order by dres.preset_id separator ';') ,");
                        sql.Append("substring_index(group_concat(DISTINCT obs.observation_id order by obs.observation_date separator ';'),';', 5) as records,");


                        sql.Append("  obs1.concat( obs1.wound_length,';', obs1.wound_width,';', obs1.wound_depth),");
                        sql.Append("  obs1.wound_color,  obs1.wound_fluid_type,  obs1.wound_smell,  obs1.fluid_quantity,  ");
                        sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime  ");
           */
                DataRow row = ds.Tables[0].Rows[i];

                int index = 0;
                eve.observation_id = share.encry_value(row[index++].ToString());
                eve.wash_date = share.encry_value(row[index++].ToString());
                eve.length = share.encry_value(row[index++].ToString());
                eve.width = share.encry_value(row[index++].ToString());
                eve.depth = share.encry_value(row[index++].ToString());
                eve.level = share.encry_value(row[index++].ToString());
                eve.color = share.encry_value(row[index++].ToString());
                eve.smell = share.encry_value(row[index++].ToString());
                eve.fuild_type = share.encry_value(row[index++].ToString());
                eve.fuild_quanity = share.encry_value(row[index++].ToString());
                index++;
                string dressingstr = row[index++].ToString();

                List<wound_dressing> dressing = new List<wound_dressing>();
                if (dressingstr.Length > 0)
                {


                    string[] dressing_ids = dressingstr.Split(new[] { "@dress_sep@" }, StringSplitOptions.None);

                    //    string []dressing_name = row[index++].ToString().Split(';'); ;

                    for (int a = 0; a < dressing_ids.Length; a++)
                    {
                        wound_dressing dress = new wound_dressing();
                        //    string[] id_array = dressing_ids[a].Split(new[] { "," }, StringSplitOptions.None);
                        //    dress.id = share.encry_value(id_array[1]);
                        string[] id_array = dressing_ids[a].Split(new[] { "@dress" }, StringSplitOptions.None);
                        dress.id = share.encry_value(id_array[1]);
                        dress.name = share.encry_value(id_array[2]);
                        if (id_array.Length > 3)
                        {
                            dress.remark = share.encry_value(id_array[3]);
                        }
                        dressing.Add(dress);
                    }
                    eve.dressing = dressing.ToArray();
                }

                //       eve.dressing_ids = share.encry_value(row[index++].ToString());
                //       eve.dressing_names = share.encry_value(row[index++].ToString());

                eve.remark = share.encry_value(row[index++].ToString());
                eve.created_by = share.encry_value(row[index++].ToString());
                eve.created_datetime = share.encry_value(row[index++].ToString());
                eve.modified_by = share.encry_value(row[index++].ToString());
                eve.modified_datetime = share.encry_value(ANICshare.Null_time_check_no_second_hyphen(row[index++].ToString()));
                eve.wound_photo_id = share.encry_value(row[index++].ToString());




                woundrecord.Add(eve);
            }
            if (woundrecord.Count == 0)
            {
                return null;
            }

            wound_wash_event brief = new wound_wash_event()
            {
                events = woundrecord.ToArray()

            };

            return brief;
        }
        public wound_parameter set_wound_parameter(DataTable table ,wound_parameter  para) {
  
           // para = new wound_parameter[table.Rows.Count];
            List<string> id = new List<string>();
            List<string> name = new List<string>();
            for (int a = 0; a < table.Rows.Count; a++)
            {
                id.Add(share.encry_value(table.Rows[a][0].ToString()));
                name.Add(share.encry_value(table.Rows[a][1].ToString()));

            }
            wound_parameter wound_parameter = new wound_parameter();
            // wound_parameter wound_parameter = para[a];

            wound_parameter.id = id.ToArray(); 
            wound_parameter.name = name.ToArray();
            para = wound_parameter;
            return para;
        }

        //Raymond @20201015
        public List<wound_parameter_2> set_wound_parameter_2(DataTable table)
        {

            // para = new wound_parameter[table.Rows.Count];
            List<wound_parameter_2> wound_parameter_list = new List<wound_parameter_2>();
            for (int a = 0; a < table.Rows.Count; a++)
            {
                wound_parameter_2 item = new wound_parameter_2();
                item.id = share.encry_value(table.Rows[a][0].ToString());
                item.name = share.encry_value(table.Rows[a][1].ToString());
                if (table.Rows[a].ItemArray.Length > 2)
                {
                    item.description = share.encry_value(table.Rows[a][2].ToString());
                }
                else
                {
                    item.description = "";
                }
                wound_parameter_list.Add(item);

            }

            return wound_parameter_list;
        }

        public contact_person get_client_contact(string client_id)
        {
            client_id = share.dencry_value(client_id);
            string contact_name_str = "";
            string client_relation_str = "";
            string phone_num_str = "";
            DataSet ds = share.GetDataSource(Cclient.get_client_contact(client_id));
            if (ds.Tables[0].Rows.Count > 0)
            {

                int i = 0;
                DataRow row = ds.Tables[0].Rows[0];
                contact_name_str = row[0].ToString();
                client_relation_str = row[1].ToString();
                phone_num_str = row[2].ToString();
            }

            contact_person brief = new contact_person
              {

                    contact_name = share.encry_value(contact_name_str),
                    client_relation = share.encry_value(client_relation_str),
                    contact_num = share.encry_value(phone_num_str)
                //label_usage_code = label_usage_code_str,
                //label_usage_id = label_usage_id_str,


        };

            return brief;


        }
        /* // Raymond 20200824
        public search_client_records get_search_client_result(string keyword, string search_index)
        {
            keyword = share.dencry_value(keyword);
            search_index = share.dencry_value(search_index);

            DateTime bday = new DateTime();


            String[] search_item_arr = { "中文名", "英文名", "樓層", "區號", "床號" };

            List<string> client_id_arr = new List<string>();
            List<string> chi_name_arr = new List<string>();
            List<string> eng_lastname_arr = new List<string>();
            List<string> eng_firstname_arr = new List<string>();
            List<string> sex_arr = new List<string>();
            List<string> bedloc_arr = new List<string>();
            List<string> birth_date_arr = new List<string>();
            List<string> picture_id_arr = new List<string>();
            List<string> client_state_arr = new List<string>();


            DataSet ds = share.GetDataSource(Cclient.get_search_client(keyword,search_index));
            if (ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;
 
                    client_id_arr.Add(share.encry_value( row[i++].ToString()));
                    chi_name_arr.Add(share.encry_value(row[i++].ToString()));
                    eng_lastname_arr.Add(share.encry_value(row[i++].ToString()));
                    eng_firstname_arr.Add(share.encry_value(row[i++].ToString()));
                    sex_arr.Add(share.encry_value(row[i++].ToString()));

                    //bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);

                    birth_date_arr.Add(share.encry_value(row[i++].ToString()));
                    bedloc_arr.Add(share.encry_value(row[i++].ToString()));

                    picture_id_arr.Add(share.encry_value(row[i++].ToString()));

                    client_state_arr.Add(share.encry_value(row[i++].ToString()== "留醫中"?"H":"N"));

                }
            }

            search_client_records  result = 
                new search_client_records() { 
                client_id_list = client_id_arr,
                chi_name_list = chi_name_arr,
                eng_lastname_list = eng_lastname_arr,
                eng_firstname_list = eng_firstname_arr,
                sex_list = sex_arr,
                birth_date_list =birth_date_arr,
                bedloc_list  = bedloc_arr,
                picture_id_list = picture_id_arr,
                client_state_list = client_state_arr
                    //picturename = picturenamestr


                };

            return result;
        }
        */

        // Raymond 20200909
        /*
         
        public search_client_records get_search_client_result(string keyword, string search_index)
        {
            keyword = share.dencry_value(keyword);
            search_index = share.dencry_value(search_index);

            DateTime bday = new DateTime();


            String[] search_item_arr = { "中文名", "英文名", "樓層", "區號", "床號" };

            List<string> client_id_arr = new List<string>();
            List<string> chi_name_arr = new List<string>();
            List<string> eng_lastname_arr = new List<string>();
            List<string> eng_firstname_arr = new List<string>();
            List<string> sex_arr = new List<string>();
            List<string> bedloc_arr = new List<string>();
            List<string> birth_date_arr = new List<string>();
            List<string> picture_id_arr = new List<string>();
            List<string> client_state_arr = new List<string>();


            DataSet ds = share.GetDataSource(Cclient.get_search_client(keyword, search_index));
            if (ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;

                    client_id_arr.Add(share.encry_value(row[i++].ToString()));
                    chi_name_arr.Add(share.encry_value(row[i++].ToString()));
                    eng_lastname_arr.Add(share.encry_value(row[i++].ToString()));
                    eng_firstname_arr.Add(share.encry_value(row[i++].ToString()));
                    sex_arr.Add(share.encry_value(row[i++].ToString()));

                    //bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);

                    birth_date_arr.Add(share.encry_value(row[i++].ToString()));
                    bedloc_arr.Add(share.encry_value(row[i++].ToString()));

                    picture_id_arr.Add(share.encry_value(row[i++].ToString()));
                    //AE > Home leave > wound
                    //A: AE
                    //H: home leave
                    //W : wound
                    string icon_status = row[i++].ToString() == "留醫中" ? "A;" : "";
                    // if (string.IsNullOrWhiteSpace(icon_status)) icon_status = row[i++].ToString() == "Y" ? "H" : "N";
                    //if(string.IsNullOrWhiteSpace(icon_status)) icon_status = row[i++].ToString() == "Y" ? "W" : "N";
                    icon_status += row[i++].ToString() == "Y" ? "W;" : "";
                    client_state_arr.Add(share.encry_value(icon_status));

                }
            }

            search_client_records result =
                new search_client_records()
                {
                    client_id_list = client_id_arr,
                    chi_name_list = chi_name_arr,
                    eng_lastname_list = eng_lastname_arr,
                    eng_firstname_list = eng_firstname_arr,
                    sex_list = sex_arr,
                    birth_date_list = birth_date_arr,
                    bedloc_list = bedloc_arr,
                    picture_id_list = picture_id_arr,
                    client_state_list = client_state_arr
                    //picturename = picturenamestr


                };

            return result;
        }
         */

        public search_client_records get_search_client_result(string keyword, string search_index)
        {
            keyword = share.dencry_value(keyword);
            search_index = share.dencry_value(search_index);

            DateTime bday = new DateTime();


            String[] search_item_arr = { "中文名", "英文名", "樓層", "區號", "床號" };

            List<string> client_id_arr = new List<string>();
            List<string> chi_name_arr = new List<string>();
            List<string> eng_lastname_arr = new List<string>();
            List<string> eng_firstname_arr = new List<string>();
            List<string> sex_arr = new List<string>();
            List<string> bedloc_arr = new List<string>();
            List<string> birth_date_arr = new List<string>();
            List<string> picture_id_arr = new List<string>();
            List<string> client_state_arr = new List<string>();


            DataSet ds = share.GetDataSource(Cclient.get_search_client(keyword, search_index));
            if (ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;

                    client_id_arr.Add(share.encry_value(row["client_id"].ToString()));
                    chi_name_arr.Add(share.encry_value(row["chiname"].ToString()));
                    eng_lastname_arr.Add(share.encry_value(row["eng_name"].ToString()));
                    eng_firstname_arr.Add(share.encry_value(row["eng_surname"].ToString()));
                    sex_arr.Add(share.encry_value(row["sex"].ToString()));

                    //bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);

                    birth_date_arr.Add(share.encry_value(row["dob"].ToString()));
                    bedloc_arr.Add(share.encry_value(row["bed_name"].ToString()));

                    picture_id_arr.Add(share.encry_value(row["client_photo_id"].ToString()));
                    //AE > Home leave > wound
                    //A: AE
                    //H: home leave
                    //W : wound
                    string icon_status = row["ae_status"].ToString() == "留醫中" ? "A;" : "";
                    // if (string.IsNullOrWhiteSpace(icon_status)) icon_status = row[i++].ToString() == "Y" ? "H" : "N";
                    //if(string.IsNullOrWhiteSpace(icon_status)) icon_status = row[i++].ToString() == "Y" ? "W" : "N";
                    icon_status += row["hasWound"].ToString() == "Y" ? "W;" : "";
                    icon_status += row["hasHomeLeave"].ToString() == "Y" ? "H;" : "";
                    client_state_arr.Add(share.encry_value(icon_status));

                }
            }

            search_client_records result =
                new search_client_records()
                {
                    client_id_list = client_id_arr,
                    chi_name_list = chi_name_arr,
                    eng_lastname_list = eng_lastname_arr,
                    eng_firstname_list = eng_firstname_arr,
                    sex_list = sex_arr,
                    birth_date_list = birth_date_arr,
                    bedloc_list = bedloc_arr,
                    picture_id_list = picture_id_arr,
                    client_state_list = client_state_arr
                    //picturename = picturenamestr


                };

            return result;
        }

        public search_client_records_2 get_search_client_result2(string keyword, string search_index)
        {
            keyword = share.dencry_value(keyword);
            search_index = share.dencry_value(search_index);

            List<search_client_item> search_Client_Items = new List<search_client_item>();


            DataSet ds = share.GetDataSource(Cclient.get_search_client(keyword, search_index));
            if (ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    search_client_item item = new search_client_item();
                    item.client_id = share.encry_value(row["client_id"].ToString());
                    item.chi_name = share.encry_value(row["chiname"].ToString());
                    item.eng_lastname = share.encry_value(row["eng_name"].ToString());
                    item.eng_firstname = share.encry_value(row["eng_surname"].ToString());
                    item.sex = share.encry_value(row["sex"].ToString());
                    item.birth_date = share.encry_value(row["dob"].ToString());
                    item.bedloc = share.encry_value(row["bed_name"].ToString());
                    item.picture_id = share.encry_value(row["client_photo_id"].ToString());
                    List<String> client_state_lists = new List<string>();
                    //AE > Home leave > wound
                    //A: AE
                    //H: home leave
                    //W : wound
                    if (row["ae_status"].ToString() == "留醫中")
                    {
                        client_state_lists.Add(share.encry_value("A"));
                    }
                    if (row["hasWound"].ToString() == "Y")
                    {
                        client_state_lists.Add(share.encry_value("W"));
                    }
                    if (row["hasHomeLeave"].ToString() == "Y")
                    {
                        client_state_lists.Add(share.encry_value("H"));
                    }

                    item.client_state_lists = client_state_lists;

                    search_Client_Items.Add(item);
                }

            }

            search_client_records_2 records = new search_client_records_2
            {
                search_client_item_list = search_Client_Items
            };


            return records;
        }


        public Acc_charge_item get_acc_charge_item(string charge_item_id)
        {
            charge_item_id = share.dencry_value(charge_item_id);
    
            string package_name_str = "";
            string zero_boo = "N";
            DataSet ds = share.GetDataSource(CAccount.get_acc_charge_item(charge_item_id));
            if (ds.Tables[0].Rows.Count != 0)
            {

                float price =  float.Parse(ds.Tables[0].Rows[0][1].ToString());
                if (price == 0)
                {
                    zero_boo = "Y";
                    // package_name_str = string.Format("{0} ${1}", ds.Tables[0].Rows[0][0].ToString(), ds.Tables[0].Rows[0][1].ToString());
                    package_name_str = string.Format("{0}", ds.Tables[0].Rows[0][0].ToString());
                }
                else
                {
                    package_name_str = string.Format("{0} ${1}", ds.Tables[0].Rows[0][0].ToString(), ds.Tables[0].Rows[0][1].ToString());
                }
        

            }
  
            Acc_charge_item item = new Acc_charge_item
            {
                item_name = share.encry_value(package_name_str)
                ,is_zero = share.encry_value(zero_boo)
            };
            return item;

        }
        public Acc_upload_item get_acc_uploaded_item(string client_id)
        {
            client_id = share.dencry_value(client_id); 
            //acc_charge_consumed
            //consumed_id, account_num, charge_item_id, charge_status, charge_quantity, charge_amount, charge_datetime, created_by, created_datetime
            Acc_upload_item items = new Acc_upload_item();

            //select charge_item_name from acc_charge_item2 where charge_item_id
            string acc_num = share.Get_mysql_database_value("acc_account2", "client_id", client_id, "account_num");
            string current_time = share.get_current_time();


 
            DataSet ds = share.GetDataSource(CAccount.get_acc_uploaded_item(acc_num, current_time));
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                items.item_name.Add(share.encry_value(row[1].ToString()));
                items.item_charge_date.Add(share.encry_value(row[0].ToString()));
                items.item_quantity.Add(share.encry_value(row[2].ToString()));
            }
            return items;
        }

    
        public insert_acc_data post_charge_item(String client_id, String charge_item_id, String quantity, String timestamp, String handlingperson)
        {
            client_id = share.dencry_value(client_id);
            charge_item_id = share.dencry_value(charge_item_id);
            quantity = share.dencry_value(quantity);
            timestamp = share.dencry_value(timestamp);
            handlingperson = share.dencry_value(handlingperson);
            string current = share.get_current_time();
 
            string acc_num = share.Get_mysql_database_value("acc_account2", "client_id", client_id, "account_num");
            string unit_price_str = share.Get_mysql_database_value("acc_charge_item2", "charge_item_id", charge_item_id, "charge_item_unit_price");

            float unit_price_float = float.Parse(unit_price_str);
            float total_amount = unit_price_float * int.Parse(quantity);

            string charge_id = share.Get_mysql_database_MaxID("acc_charge_consumed", "consumed_id");
            string[] value = { charge_id, acc_num,   charge_item_id, "N", quantity, total_amount.ToString(),
            timestamp, "", handlingperson, current };
            int a = share.Ecxe_transaction_Sql(CAccount.post_charge_item(value));

            if (a > 0)
            {
                String str = "YES";
                insert_acc_data insertstate = new insert_acc_data
                {

                    result =share.encry_value( str)
                };
                return insertstate;
            }
            else
            {
                return null;
            }
        }
        public insert_acc_data post_charge_item(String client_id, String charge_item_id, String quantity, String timestamp, String handlingperson , String charge)
        {
            client_id = share.dencry_value(client_id);
            charge_item_id = share.dencry_value(charge_item_id);
            quantity = share.dencry_value(quantity);
            timestamp = share.dencry_value(timestamp);
            handlingperson = share.dencry_value(handlingperson);
            charge = share.dencry_value(charge);

            string current = share.get_current_time();

            string acc_num = share.Get_mysql_database_value("acc_account2", "client_id", client_id, "account_num");
            //string unit_price_str = share.Get_mysql_database_value("acc_charge_item2", "charge_item_id", charge_item_id, "charge_item_unit_price");

            float unit_price_float = float.Parse(charge);
            float total_amount = unit_price_float * int.Parse(quantity);

            string charge_id = share.Get_mysql_database_MaxID("acc_charge_consumed", "consumed_id");
            string[] value = { charge_id, acc_num,   charge_item_id, "N", quantity, total_amount.ToString(),
            timestamp, "", handlingperson, current };
            int a = share.Ecxe_transaction_Sql(CAccount.post_charge_item(value));

            if (a > 0)
            {
                String str = "YES";
                insert_acc_data insertstate = new insert_acc_data
                {

                    result = share.encry_value(str)
                };
                return insertstate;
            }
            else
            {
                return null;
            }
        }
        public insert_acc_data del_charge_item(string consumed_id, String handlingperson)
        {
            consumed_id = share.dencry_value(consumed_id);
            handlingperson = share.dencry_value(handlingperson);
            string[] val = { consumed_id, handlingperson };

            int a = share.Ecxe_transaction_Sql(CAccount.del_charge_item(val));
            if (a > 0)
            {
                String str = "YES";
                insert_acc_data insertstate = new insert_acc_data
                {

                    result = share.encry_value(str)
                };
                return insertstate;
            }
            else
            {
                return null;
            }
        }
        public medicine_photo get_medicine_photo(string medicine_photo_id)
        {
            medicine_photo_id = share.dencry_value(medicine_photo_id);
            byte[] imgData = null;
            if (medicine_photo_id == "0")
            {
           //     var ms = new MemoryStream();

            //    var result = new HttpResponseMessage(HttpStatusCode.OK);
                String filePath = HostingEnvironment.MapPath("~/no_image_available.png");
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                Image image = Image.FromStream(fileStream);
                MemoryStream memoryStream = new MemoryStream();
            //    image.Save(memoryStream, ImageFormat.Jpeg);
            //    result.Content = new ByteArrayContent(memoryStream.ToArray());
            //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");




            //    System.Drawing.Image image = 
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                imgData = memoryStream.ToArray();
            }


            //string pic = "";
            string result = "";

            DataSet ds = share.GetDataSource(Cmedicine.get_medicine_photo(medicine_photo_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                imgData = (byte[])row[0];
            }
 

            medicine_photo med_photo = new medicine_photo{
        
                image_data = Convert.ToBase64String(imgData)
      
        };

 
            return med_photo;

        }

 


        public medicine_bag_image get_medicine_bag_image(string medicine_bag_id)
        {
            medicine_bag_id = share.dencry_value(medicine_bag_id);
            string result = "";
            byte[] imgData = null;
            DataSet ds = share.GetDataSource(Cmedicine.get_medicine_photo(medicine_bag_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                imgData = (byte[])row[0];
            }

            medicine_bag_image med_image =
            new medicine_bag_image()
            {
                image_data = Convert.ToBase64String(imgData)
            };
            return med_image;
        }

        public medicine_box_content get_medicine_box_content(string medicine_box_id)
        {


            /*

            List<string> take_medicine_id_arr = new List<string>();
            string take_medicine_id_arr_str = "";


            DataSet ds = share.GetDataSource(Cmedicine.get_medicine_box_content(medicine_box_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                imgData = (byte[])row[0];
            }


            MySqlConnection conn = new MySqlConnection("server=" + dbHost + "; PORT= " + dbPort + " ;user id=" + dbUser + "; password=" + dbPass + "; database=" + dbName + "; CharSet=utf8");

            MySqlCommand command = conn.CreateCommand();

            conn.Open();

            //String cmdText0 = "select * from med_take_medicine Where client_id IN (@id)";
            String cmdText0 = "select * from med_medicine_box Where med_medicine_box_id IN (@id)";


            MySqlCommand cmd0 = new MySqlCommand(cmdText0, conn);
            cmd0.Parameters.AddWithValue("@id", medicine_box_id);


            MySqlDataReader dr = cmd0.ExecuteReader(); //execure the reader




            while (dr.Read())
            {
                take_medicine_id_arr.Add(dr["take_medicine_id"].ToString());

                //handlingpersonstr.Add(dr["handlingperson"].ToString());
                //timestampstr.Add(Convert.ToDateTime(dr["timestamp"]).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            dr.Close();

            take_medicine_id_arr_str = string.Join<string>(";", take_medicine_id_arr);

            conn.Close();
            medicine_box_content[] box_content = new medicine_box_content[]{
            new medicine_box_content(){
                take_medicine_id_arr = take_medicine_id_arr_str
        }

        };
        */
            return null;

        }


        public pictureinfo get_person_image(string picture_id)
        {
            picture_id = share.dencry_value(picture_id);
            byte[] imgData = null;
            if (picture_id == "0")
            {
                return null;
            }
          


            DataSet ds = share.GetDataSource(Cclient.get_client_photo(picture_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                imgData = (byte[])row[0];
            }

            if (imgData!=null)
            {
                pictureinfo image =
new pictureinfo()
{
    picturedata = Convert.ToBase64String(imgData)
};
                return image;
            }
            else
            {
                return null;
            }
 
        }

        public medicine_records  get_medicine_records(string client_id)
        {
            client_id = share.dencry_value(client_id);


            List<string> medicine_name_str = new List<string>();
            List<string> medicine_id_str = new List<string>();
            List<string> take_medicine_id_str = new List<string>();

            //String medicine_name_str = "";
            List<string> volume_amount_str = new List<string>();
            List<string> volume_unit_str = new List<string>();

            List<string> medicine_period_str = new List<string>();
            List<string> each_take_type_str = new List<string>();

            List<string> medicine_report_type_id_str = new List<string>();

            List<string> medicine_taking_method_str = new List<string>();
            List<string> medicine_taking_type_str = new List<string>();
            List<string> medicine_taking_remark_str = new List<string>();

            List<string> medicine_photo_id_str = new List<string>();
            List<string> medicine_bag_id_str = new List<string>();
            List<string> medicine_take_interval_str = new List<string>();

            List<string> handlingpersonstr = new List<string>();
            List<string> timestampstr = new List<string>();

            List<string> first_check_id_str = new List<string>();
            List<string> second_check_id_str = new List<string>();

            List<string> same_day_record = new List<string>();


            List<string> taken_state_records = new List<string>();


            String stop_flag = "N";


            DataSet ds = share.GetDataSource(Cmedicine.get_medicine_records(client_id));
            if (ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    int i = 0;
 
                    medicine_id_str.Add(row[i++].ToString());

                    medicine_name_str.Add(row[i++].ToString());

                    take_medicine_id_str.Add(row[i++].ToString());
                    //medicine_name_str = dr["medicinename"].ToString();
                    i++;
                    volume_amount_str.Add(row[i++].ToString());
                    volume_unit_str.Add(row[i++].ToString() );
                    medicine_period_str.Add(row[i++].ToString());
                    medicine_take_interval_str.Add(row[i++].ToString());

                    each_take_type_str.Add(row[i++].ToString().Replace("#N", row[i++].ToString()));
                    medicine_taking_method_str.Add(row[i++].ToString());

                    medicine_report_type_id_str.Add(row[i++].ToString());


                    medicine_taking_remark_str.Add(row[i++].ToString());

                    medicine_photo_id_str.Add(row[i++].ToString());

                    medicine_bag_id_str.Add(row[i++].ToString() );
 
                    first_check_id_str.Add(row[i++].ToString());
                    second_check_id_str.Add(row[i++].ToString());

                    List<string> taken_actual_time_list = new List<string>();
                    List<string> handlingpersonstr_list = new List<string>();
                    List<string> taken_state_list = new List<string>();
                    for (int a = 0; a < 4; a++)
                    {
                        taken_state_list.Add("0");
                        taken_actual_time_list.Add("0");
                        handlingpersonstr_list.Add("0");
                    }
                    string handlingperson_record = string.Join(";", handlingpersonstr_list.ToArray());
                    handlingpersonstr.Add(handlingperson_record);

                    string actualtime_record = string.Join(";", taken_actual_time_list.ToArray());
                    timestampstr.Add(actualtime_record);
                    string taken_state_record = string.Join(";", taken_state_list.ToArray());
                    taken_state_records.Add(taken_state_record);






                }
            }
            for (int i = 0; i < 4; i++)
            {
                taken_state_records.Add("0");

            }

            //handlingpersonstr.Add("0");





            //Debug.Print(result);

            medicine_records info =  
                new medicine_records(){

                medicine_name = share.encry_value( medicine_name_str),
                medicine_id = share.encry_value(medicine_id_str),
                take_medicine_id = share.encry_value(take_medicine_id_str),

                med_volume_amount = share.encry_value(volume_amount_str),
                med_volume_unit = share.encry_value(volume_unit_str),
                medicine_period = share.encry_value(medicine_period_str),
                each_take_type = share.encry_value(each_take_type_str),
                medicine_report_type_id = share.encry_value(medicine_report_type_id_str),


                medicine_taking_method = share.encry_value(medicine_taking_method_str),
                medicine_taking_remark = share.encry_value(medicine_taking_remark_str),

                medicine_photo_id = share.encry_value(medicine_photo_id_str),
                medicine_bag_id = share.encry_value(medicine_bag_id_str),
                medicine_take_interval = share.encry_value(medicine_take_interval_str),

                handling_person = share.encry_value(handlingpersonstr),
                ///time_stamp = same_day_record,
                time_stamp = share.encry_value(timestampstr),
                taken_state = share.encry_value(taken_state_records),


                first_check_id = share.encry_value(first_check_id_str),
                second_check_id = share.encry_value(second_check_id_str)
        };
            //Debug.Print("121212" + same_day_record[0]);
     
            return info;


        }


        public medicine_description  get_medicine_description(String take_medicine_id)
        {
            take_medicine_id = share.dencry_value(take_medicine_id);

            string each_take_type_str = "";
            string take_begin_date_str = "";

            string medicine_source_id_str = "";
            string medicine_source_str = "";

            string specialty_code_str = "";
            string medicine_PRN_str = "";
            string script_medicine_str = "";
            string need_distribute_str = "";

            string first_check_id_str = "";
            string second_check_id_str = "";


            string first_check_person_str = "";
            string first_check_time_str = "";
            string second_check_person_str = "";
            string second_check_time_str = "";



            
            DataSet ds = share.GetDataSource(Cmedicine.get_medicine_description(take_medicine_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                each_take_type_str = row["each_take_type"].ToString();
                //Convert.ToDateTime(reader0["take_begin_date"]).ToString("yyyy-MM-dd HH:mm:ss");

                take_begin_date_str =  row[3].ToString();
                medicine_source_str = row["addr_org_chi_name"].ToString();
                specialty_code_str = row["specialties_code"].ToString();
                medicine_PRN_str = row[10].ToString();
                //script_medicine_str = row["script_medicine"].ToString();
                //need_distribute_str = row["need_distribute"].ToString();

                first_check_id_str = row["first_check_id"].ToString();
                second_check_id_str = row["second_check_id"].ToString();


            }
 


            medicine_description des = 
                new medicine_description(){


                take_begin_date = share.encry_value(take_begin_date_str),
                medicine_source = share.encry_value(medicine_source_str),
                specialty_code = share.encry_value(specialty_code_str),
                medicine_PRN = share.encry_value(medicine_PRN_str),
                script_medicine = share.encry_value(script_medicine_str),
                need_distribute = share.encry_value(need_distribute_str),
                first_check_person = share.encry_value(first_check_person_str),
                first_check_time = share.encry_value(first_check_time_str),
                second_check_person = share.encry_value(second_check_person_str),
                second_check_time = share.encry_value(second_check_time_str)
        
        };


 
            return des;

        }
      
        public take_medicine_records get_take_medicines(string client_id)
        {
            client_id = share.dencry_value(client_id);
            List<take_medicine> take_list = new List<take_medicine>();
   

            DataSet ds = share.GetDataSource(Cmedicine.get_medicine_take_records(client_id));
            foreach (DataRow row in ds.Tables[0].Rows)
            {

                string[] des = new string[3];
                //來源 專科 取藥日期
                des[0] = share.encry_value(string.Format("來源:{0}", row[15].ToString()));
                des[1] = share.encry_value(string.Format("專科:{0}", row[16].ToString()));
                des[2] = share.encry_value(string.Format("取藥日期:{0}", row[17].ToString()));
                take_medicine take = new take_medicine()
                {

                    medicine_id =share.encry_value( row[0].ToString()),
                    medicine_name = share.encry_value(row[1].ToString()),
                    take_medicine_id = share.encry_value(row[2].ToString()),
                    med_volume_amount = share.encry_value(row[4].ToString()),
                    med_volume_unit = share.encry_value(row[5].ToString()),
               
                    medicine_period = share.encry_value(row[6].ToString()),
                    medicine_take_interval = share.encry_value(row[7].ToString()),

                    prn = share.encry_value(row[8].ToString()),
                    each_take_type = share.encry_value(row[9].ToString().Replace("#N", row[10].ToString())),
                    medicine_taking_method = share.encry_value(row[11].ToString()),
                    medicine_report_type_id = share.encry_value(row[12].ToString()),
                    medicine_report_type= share.encry_value(row[13].ToString()),
                    medicine_taking_remark = share.encry_value(row[14].ToString()) ,
                    medicine_source = share.encry_value(row[15].ToString()),
                     medicine_spe_code = share.encry_value(row[16].ToString()),
                    medicine_refill_date = share.encry_value(row[17].ToString()),
                    description = des
                };
                take_list.Add(take);
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                take_medicine_records record = new take_medicine_records
                {
                    takes = take_list.ToArray()
                };
                 

                return record ;
            }
            else
            {
                return null;
            }

        }

        public medical_revisit_records get_medical_revisit_records(string client_id)
        {
            client_id = share.dencry_value(client_id);
          
            List<string> reconsult_record_list = new List<string>();


            String reconsult_id_str = "";
            String reconsult_planned_datetime_str = "";
            String hospital_addr_id_str = "";
            String hospital_addr_code_str = "";
            String specialties_code_str = "";
            String reconsult_event_str = "";
            String transport_str = "";
            String reconsult_accompany_type_str = "";


            DataSet ds = share.GetDataSource(Cmedical.get_medical_revisit_records(client_id));
            if (ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i  = 0;
                    reconsult_id_str = row[i++].ToString();


                    reconsult_planned_datetime_str = row[i++].ToString();
                    reconsult_event_str = row[i++].ToString().ToString().Replace(";", ",");

                    hospital_addr_id_str = row[i++].ToString();
                    specialties_code_str = row[i++].ToString();
 
                    transport_str = row[i++].ToString();
                    reconsult_accompany_type_str = row[i++].ToString();
                    reconsult_event_str = reconsult_event_str+" "+ row[i++].ToString();
                    string addr = row[i++].ToString();
                    hospital_addr_code_str =addr == "" ? " " : addr;

                    string[] reconsult_arr = new string[] { reconsult_id_str, reconsult_planned_datetime_str, reconsult_event_str, transport_str,
                reconsult_accompany_type_str,specialties_code_str,hospital_addr_id_str,hospital_addr_code_str};

 
                    string reconsult_arr_str = string.Join(";", reconsult_arr);
                    reconsult_record_list.Add(reconsult_arr_str);
 
                }

            }
            medical_revisit_records  record = new medical_revisit_records {

                revisit_record =share.encry_value( reconsult_record_list)
 
        };
   

            return record;

        }


        public  medical_events get_medical_event(string client_id)
        {
            client_id = share.dencry_value(client_id);
            StringBuilder sql = new StringBuilder();
            sql.Append(Cmedical.get_medical_brief(client_id));
 
            DataSet ds = share.GetDataSource(sql.ToString());

            List<medical_event> med_events = new List<medical_event>();
            string table_name = "";
            string accessible_str = "";

            if (ds.Tables[0].Rows.Count > 0)
            {

                List<medical_content> client_list = new List<medical_content>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow row = ds.Tables[0].Rows[i];


                    string location = share.encry_value(row["addr_org_chi_name"].ToString());
                    string date_str = share.encry_value(row["ae_in_date"].ToString() + "\n" + row["ae_in_time"].ToString());
                    //病房 原因 備註
                    string reason = string.Format("原因:{0}", row["ae_in_reason"].ToString());
                    string bed = string.Format("病房:{0}", row["ae_in_bed_num"].ToString());
                    string remark = string.Format("備註:{0}", row["ae_in_remark"].ToString());

                    /*
                    string location = share.encry_value(row[7].ToString());
                    string date_str = share.encry_value(row[5].ToString());
                    //病房 原因 備註
                    string reason = string.Format("原因:{0}", row[4].ToString());
                    string bed = string.Format("病房:{0}", row[8].ToString());
                    string remark = string.Format("備註:{0}", row[9].ToString());
                    */
                    string[] array = new string[] { reason, bed, remark };
                    array =  share.encry_value(array);


                    client_list.Add(new medical_content()
                    {

                        title = location,
                        status = date_str,

                        content = array
                    }
                    );


                }
                med_events.Add(new medical_event()
                {

                    event_name = share.encry_value("急症"),
                    medical_content_list = client_list.ToArray(),

                }
                    );



            }

            if (ds.Tables[1].Rows.Count > 0)
            {
                List<medical_content> client_list = new List<medical_content>();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    DataRow row = ds.Tables[1].Rows[i];

                    string location = share.encry_value(row["addr_org_chi_name"].ToString());
                    string date_str = share.encry_value(row["revisit_planned_date"].ToString() + " " + row["revisit_planned_time"].ToString());
                    string description = share.encry_value(row["specialties_code"].ToString() + ANICshare.add_space(row["revisit_event"].ToString()));
                    //交通 陪診 備註
                    string transport = string.Format("交通:{0}", row["transport"].ToString());
                    string accompany = string.Format("陪診:{0}", row["revisit_accompany"].ToString());
                    string remark = string.Format("備註:{0}", row["revisit_remark"].ToString());

                    /*
                    string location = share.encry_value(row[6].ToString());
                    string date_str = share.encry_value(row[3].ToString()+" "+ row[4].ToString());
                    string description = share.encry_value(row[7].ToString() +ANICshare.add_space( row[8].ToString()));
                    //交通 陪診 備註
                    string transport = string.Format("交通:{0}", row[9].ToString());
                    string accompany = string.Format("陪診:{0}", row[10].ToString());
                    string remark = string.Format("備註:{0}", row[11].ToString());
                    */
                    string[] array = new string[] { transport, accompany, remark };
                    array = share.encry_value(array);
                    client_list.Add(new medical_content()
                    {

                        title = location,
                        status = date_str,
                        description = description,
                        content = array
                    }
                    );


                }
                med_events.Add(new medical_event()
                {

                    event_name = share.encry_value("覆診"),
                    medical_content_list = client_list.ToArray(),

                });



            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                List<medical_content> client_list = new List<medical_content>();
                table_name = "";
                for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                {
                    DataRow row = ds.Tables[2].Rows[i];

                    if (table_name != row[0].ToString())
                    {
                        if (client_list.Count > 0)
                        {
                            med_events.Add(new medical_event()
                            {

                                event_name = share.encry_value(table_name),
                                medical_content_list = client_list.ToArray(),

                            });

                        }
                        table_name = row[0].ToString();
                        client_list = new List<medical_content>();

                        string date_str = share.encry_value(row["event_planned_date"].ToString());
                        string state = share.encry_value(row["event_status"].ToString());

                        /*
                        string client_name_str = row[3].ToString();
                        string sex_str = row[2].ToString();
                        string picture_id_str = row[12].ToString();
                        //string[] timearray = row[5].ToString().Split(' ');

                     

                        string client_id_str = row[4].ToString();

                        string date = string.Format("CA;日期;{0}", row[6].ToString());

                        string time_str = string.Format("TI;時間;{0}", row[7].ToString());
                        string state = string.Format("ST;狀態;{0}", row[10].ToString());
                        */
                        //string bed = string.Format("BD;留醫病房／床號:{0}", row[8].ToString());
                        //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());


                        client_list.Add(new medical_content()
                        {
                            // title = date_str,
                            //status = state
                            title = state,
                            status = date_str
                        }
                        );
                    }
                    else
                    {

                        string date_str = share.encry_value(row["event_planned_date"].ToString());
                        string state = share.encry_value(row["event_status"].ToString());


                        client_list.Add(new medical_content()
                        {
                            // title = date_str,
                            //status = state
                            title = state,
                            status = date_str
                        }
                      );
                    }
                    if (client_list.Count > 0)
                    {
                        if (i == ds.Tables[2].Rows.Count - 1)
                        {
                            med_events.Add(new medical_event()
                            {
                                event_name = share.encry_value(table_name),
                                medical_content_list = client_list.ToArray()
                            });

                        }
                    }
                }
            }
            if (med_events.Count>0)
            {
                medical_events events_list = new medical_events()
                {
                    events = med_events.ToArray()
                };
                return events_list;
            }
            return null;

        }
        public medical_events get_medical_event2(string client_id)
        {
            client_id = share.dencry_value(client_id);
            StringBuilder sql = new StringBuilder();
            sql.Append(Cmedical.get_medical_brief(client_id));

            DataSet ds = share.GetDataSource(sql.ToString());

            List<medical_event> med_events = new List<medical_event>();
            string table_name = "";
            string accessible_str = "";

            if (ds.Tables[0].Rows.Count > 0)
            {

                List<medical_content> client_list = new List<medical_content>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow row = ds.Tables[0].Rows[i];

                    string location = share.encry_value(row["addr_org_chi_name"].ToString());
                    string date_str = share.encry_value(row["ae_in_date"].ToString() + "\n" + row["ae_in_time"].ToString());
                    //病房 原因 備註
                    string reason = string.Format("原因:{0}", row["ae_in_reason"].ToString());
                    string bed = string.Format("病房:{0}", row["ae_in_bed_num"].ToString());
                    string remark = string.Format("備註:{0}", row["ae_in_remark"].ToString());

                    /*
                    string location = share.encry_value(row[7].ToString());
                    string date_str = share.encry_value(row[5].ToString());
                    //病房 原因 備註
                    string reason = string.Format("原因:{0}", row[4].ToString());
                    string bed = string.Format("病房:{0}", row[8].ToString());
                    string remark = string.Format("備註:{0}", row[9].ToString());
                    */

                    string[] array = new string[] { reason, bed, remark };
                    array = share.encry_value(array);


                    client_list.Add(new medical_content()
                    {

                        title = location,
                        status = date_str,

                        content = array
                    }
                    );


                }
                med_events.Add(new medical_event()
                {

                    event_name = share.encry_value("急症"),
                    medical_content_list = client_list.ToArray(),

                }
                    );



            }

            if (ds.Tables[1].Rows.Count > 0)
            {
                List<medical_content> client_list = new List<medical_content>();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    DataRow row = ds.Tables[1].Rows[i];

                    string location = share.encry_value(row["addr_org_chi_name"].ToString());
                    string date_str = share.encry_value(row["revisit_planned_date"].ToString() + " " + row["revisit_planned_time"].ToString());
                    string description = share.encry_value(row["specialties_code"].ToString() + ANICshare.add_space(row["revisit_event"].ToString()));
                    //交通 陪診 備註
                    string transport = string.Format("交通:{0}", row["transport"].ToString());
                    string accompany = string.Format("陪診:{0}", row["revisit_accompany"].ToString());
                    string remark = string.Format("備註:{0}", row["revisit_remark"].ToString());

                    /*
                    string location = share.encry_value(row[6].ToString());
                    string date_str = share.encry_value(row[3].ToString() + " " + row[4].ToString());
                    string description = share.encry_value(row[7].ToString() + ANICshare.add_space(row[8].ToString()));
                    //交通 陪診 備註
                    string transport = string.Format("交通:{0}", row[9].ToString());
                    string accompany = string.Format("陪診:{0}", row[10].ToString());
                    string remark = string.Format("備註:{0}", row[11].ToString());
                    */

                    string[] array = new string[] { transport, accompany, remark };
                    array = share.encry_value(array);
                    client_list.Add(new medical_content()
                    {

                        title = location,
                        status = date_str,
                        description = description,
                        content = array
                    }
                    );


                }
                med_events.Add(new medical_event()
                {

                    event_name = share.encry_value("覆診"),
                    medical_content_list = client_list.ToArray(),

                });



            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                List<medical_content> client_list = new List<medical_content>();
                table_name = "";
                for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                {
                    DataRow row = ds.Tables[2].Rows[i];

                    if (table_name != row[0].ToString())
                    {
                        if (client_list.Count > 0)
                        {
                            med_events.Add(new medical_event()
                            {

                                event_name = share.encry_value(table_name),
                                medical_content_list = client_list.ToArray(),

                            });

                        }
                        table_name = row[0].ToString();
                        client_list = new List<medical_content>();

                        string date_str = share.encry_value(row["event_planned_date"].ToString());
                        string state = share.encry_value(row["event_status"].ToString());
                        //string date_str = share.encry_value(row[6].ToString());
                        //string state = share.encry_value(row[10].ToString());

                        /*
                        string client_name_str = row[3].ToString();
                        string sex_str = row[2].ToString();
                        string picture_id_str = row[12].ToString();
                        //string[] timearray = row[5].ToString().Split(' ');

                     

                        string client_id_str = row[4].ToString();

                        string date = string.Format("CA;日期;{0}", row[6].ToString());

                        string time_str = string.Format("TI;時間;{0}", row[7].ToString());
                        string state = string.Format("ST;狀態;{0}", row[10].ToString());
                        */
                        //string bed = string.Format("BD;留醫病房／床號:{0}", row[8].ToString());
                        //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());


                        client_list.Add(new medical_content()
                        {
                            title = state,
                            status = date_str
                        }
                        );
                    }
                    else
                    {
                        string date_str = share.encry_value(row["event_planned_date"].ToString());
                        string state = share.encry_value(row["event_status"].ToString());
                        //string date_str = share.encry_value(row[6].ToString());
                        //string state = share.encry_value(row[10].ToString());


                        client_list.Add(new medical_content()
                        {
                            title = state,
                            status = date_str
                        }
                      );
                        if (i == ds.Tables[0].Rows.Count - 1)
                        {
                            med_events.Add(new medical_event()
                            {
                                event_name = share.encry_value(table_name),
                                medical_content_list = client_list.ToArray()
                            });

                        }
                    }
                }
            }
            if (med_events.Count > 0)
            {
                medical_events events_list = new medical_events()
                {
                    events = med_events.ToArray()
                };
                return events_list;
            }
            return null;

        }


        public blood_pressure_records get_blood_pressure_records(string client_id, string time_mode)
        {
            client_id = share.dencry_value(client_id);
            time_mode = share.dencry_value(time_mode);
            List<string> systolic_str = new List<string>();
            List<string> diastolic_str = new List<string>();
            List<string> pulse_str = new List<string>();
            //String medicine_name_str = "";
            List<string> handlingperson_str = new List<string>();
            List<string> timestamp_str = new List<string>();
            DataSet ds = share.GetDataSource(Cmedical.get_blood_pressure_records(client_id,int.Parse( time_mode)));


            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;
 
                    systolic_str.Add(row[i++].ToString());
                    diastolic_str.Add(row[i++].ToString());
                    pulse_str.Add(row[i++].ToString());

                    handlingperson_str.Add(row[i++].ToString());
                    timestamp_str.Add(row[i++].ToString());

                }

            }
            blood_pressure_records temperature =
        new blood_pressure_records()
        {
            timestamp =share.encry_value(timestamp_str),
            systolic = share.encry_value(systolic_str),
            diastolic = share.encry_value(diastolic_str),
            pulse = share.encry_value(pulse_str),
            handlingperson = share.encry_value(handlingperson_str)};
            return temperature;
        }

        public blood_glucose_records get_blood_glucose_records(string client_id, string time_mode)
        {
            client_id = share.dencry_value(client_id);
            time_mode = share.dencry_value(time_mode);

            List<string> glucose_str = new List<string>();
            //String medicine_name_str = "";
            List<string> handlingperson_str = new List<string>();
            List<string> timestamp_str = new List<string>();
            DataSet ds = share.GetDataSource(Cmedical.get_blood_glucose_records(client_id,int.Parse( time_mode)));
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;

                    glucose_str.Add(row[i++].ToString());

                    handlingperson_str.Add(row[i++].ToString());
                    timestamp_str.Add(row[i++].ToString());
                }

            }

            blood_glucose_records temperature =
        new blood_glucose_records()
        {
            timestamp = share.encry_value(timestamp_str),
            blood_glucose = share.encry_value(glucose_str),
            handlingperson = share.encry_value(handlingperson_str)

        };
            return temperature;
        }

        public blood_oxygen_records get_blood_oxygen_records(string client_id, string  time_mode)
        {
            client_id = share.dencry_value(client_id);
            time_mode = share.dencry_value(time_mode);
            List<string> oxygen_str = new List<string>();
            //String medicine_name_str = "";
            List<string> handlingperson_str = new List<string>();
            List<string> timestamp_str = new List<string>();
            DataSet ds = share.GetDataSource(Cmedical.get_blood_oxygen_records(client_id,int.Parse( time_mode)));
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;

                    oxygen_str.Add(row[i++].ToString());
                 
                    handlingperson_str.Add(row[i++].ToString());
                    timestamp_str.Add(row[i++].ToString());
                }

            }

            blood_oxygen_records temperature =
        new blood_oxygen_records()
        {
            timestamp = share.encry_value(timestamp_str),
            blood_oxygen = share.encry_value(oxygen_str),
            handlingperson = share.encry_value(handlingperson_str)

        };
            return temperature;
        }
        public respiration_rate_records get_respiration_rate_records(string client_id, string time_mode)
        {
            client_id = share.dencry_value(client_id);
            time_mode = share.dencry_value(time_mode);
            List<string> respiration_str = new List<string>();
            //String medicine_name_str = "";
            List<string> handlingperson_str = new List<string>();
            List<string> timestamp_str = new List<string>();
            DataSet ds = share.GetDataSource(Cmedical.get_respiration_rate_records(client_id,int.Parse( time_mode)));
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;

                    respiration_str.Add(row[i++].ToString());
                    //medicine_name_str = dr["medicinename"].ToString();
                    handlingperson_str.Add(row[i++].ToString());
                    timestamp_str.Add(row[i++].ToString());
                }

            }

            respiration_rate_records temperature =
        new respiration_rate_records()
        {
            timestamp = share.encry_value(timestamp_str),
            respiration_rate = share.encry_value(respiration_str),
            handlingperson = share.encry_value(handlingperson_str)

        };
            return temperature;
        }
        public temperature_records get_body_temperature_records(string client_id, string time_mode)
        {
            client_id = share.dencry_value(client_id);
            time_mode = share.dencry_value(time_mode);

            List<string> temperature_str = new List<string>();
            //String medicine_name_str = "";
            List<string> handlingperson_str = new List<string>();
            List<string> timestamp_str = new List<string>();
            DataSet ds = share.GetDataSource(Cmedical.get_temperature_records(client_id,int.Parse( time_mode)));
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;

                    temperature_str.Add(share.encry_value(row[i++].ToString()));
                    //medicine_name_str = dr["medicinename"].ToString();
                    handlingperson_str.Add(share.encry_value(row[i++].ToString()));
                    timestamp_str.Add(share.encry_value(row[i++].ToString()));
                }

            }

                temperature_records  temperature = 
            new temperature_records(){
                timestamp = timestamp_str,
                body_temperature = temperature_str,
                handlingperson = handlingperson_str

        };
            return temperature;
        }
        public body_weight_records get_body_weight_records(string client_id, string time_mode)
        {
            client_id = share.dencry_value(client_id);
            time_mode = share.dencry_value(time_mode);
            List<string> weight_str = new List<string>();
            //String medicine_name_str = "";
            List<string> handlingperson_str = new List<string>();
            List<string> timestamp_str = new List<string>();

            DataSet ds = share.GetDataSource(Cmedical.get_weight_records(client_id,int.Parse(time_mode)));
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;

                    weight_str.Add(share.encry_value(row[i++].ToString()));
                    //medicine_name_str = dr["medicinename"].ToString();
                    handlingperson_str.Add(share.encry_value(row[i++].ToString()));
                    timestamp_str.Add(share.encry_value(row[i++].ToString()));
                }

            }
 
            body_weight_records weight =
        new body_weight_records()
        {
            timestamp = timestamp_str,
            body_weight = weight_str,
            handlingperson =  handlingperson_str

        };
            return weight;
        }

        public insert_vital_data  post_vital_data(String client_id, String value, String timestamp, String handlingperson,string  measure_index)
        {
            client_id = share.dencry_value(client_id);

            value = share.dencry_value(value);
            timestamp = share.dencry_value(timestamp);
            handlingperson = share.dencry_value(handlingperson);
            measure_index = share.dencry_value(measure_index);

            int index = int.Parse(measure_index);
            string time = share.get_current_time();
            if (index==0)
            {
                string result = "";
                string[] reading = value.Split(';');
                string[] values = new string[] { "",client_id, time,reading[0],reading[1],reading[2],handlingperson,time };
                values[0] = share.Get_mysql_database_MaxID(Cmedical.measuretable[index], "record_id");
                if (share.bool_Ecxe_transaction_Sql(Cmedical.insert_blood_presure(values)))
                {

                    result = "YES";

                }
                else
                {
                    result = "NO";
                }
                insert_vital_data insertstate =    new insert_vital_data(){  is_update =share.encry_value( result) };
                return insertstate;
            }
            else if(index == 6)
            {
                string result = "";
                string[] bmi_values = value.Split(';');
                decimal height = decimal.Parse(bmi_values[0]);
                string height_str = (height * 1000).ToString();

                string[] values = new string[] { "", client_id, time, height_str, bmi_values[1], handlingperson, time };
                values[0] = share.Get_mysql_database_MaxID(Cmedical.measuretable[5], "record_id");
                if (share.bool_Ecxe_transaction_Sql(Cmedical.insert_bmi_vital(values, index)))
                {
                    result = "YES";
                }
                else
                {
                    result = "NO";
                }
                insert_vital_data insertstate = new insert_vital_data() { is_update = share.encry_value(result) };
                return insertstate;
            }
            else
            {
                string result = "";

                string[] values = new string[] { "", client_id, time, value, handlingperson, time };
                values[0] = share.Get_mysql_database_MaxID(Cmedical.measuretable[index], "record_id");
                if (share.bool_Ecxe_transaction_Sql(Cmedical.insert_other_vital(values, index)))
                {
                    result = "YES";
                }
                else
                {
                    result = "NO";
                }
                insert_vital_data insertstate = new insert_vital_data() { is_update = share.encry_value(result) };
                return insertstate;
            }
        }




        public post_observation_record post_wound_observation_record(String wound_id, String length, String width, String depth, String level,  String color,
    String smell, String fluid_type, String fluid_quantity, String dressing, String clean_days, String clean_times, String examined_by, String photo_data)
        {


            wound_id = share.dencry_value(wound_id);
            length = share.dencry_value(length);
            width = share.dencry_value(width);
            depth = share.dencry_value(depth);
            level = share.dencry_value(level);
            color = share.dencry_value(color);
            smell = share.dencry_value(smell);
            fluid_type = share.dencry_value(fluid_type);
            fluid_quantity = share.dencry_value(fluid_quantity);
            clean_times = share.dencry_value(clean_times);
            clean_days = share.dencry_value(clean_days);
            dressing = share.dencry_value(dressing);
   
            examined_by = share.dencry_value(examined_by);
            DateTime now =  share.get_current_datetime();
            String datetime = now.ToString(ANICshare.sql_timeformat_nosecond);
         //   string datetime = 
            StringBuilder sql = new StringBuilder();
            string id = share.Get_mysql_database_MaxID("care_wound_observation2", "observation_id"); ;
 
            string[] vals = new string[] {id,wound_id,datetime,length,width,depth,
                level,color,smell,fluid_type,fluid_quantity,
                clean_days,clean_times,"",examined_by,datetime };

            //string dress_ids_str = dressing.ToString();
            string record_id = share.Get_mysql_database_MaxID("care_wound_observation_dressing", "dressing_id");
            string[][] dressing_idstr = new string[0][];
            if (dressing.Length > 0&&!dressing.Equals("0"))
            {
  
                string[] dressing_ids = dressing.Split(new[] { "@dress_sep" }, StringSplitOptions.None);
            
               dressing_idstr = new string[dressing_ids.Length][];
                //    string []dressing_name = row[index++].ToString().Split(';'); ;

                for (int a = 0; a < dressing_ids.Length; a++)
                {
                    // wound_dressing dress = new wound_dressing();
                    string[] dressing_record = new string [12] ;
 
                    // string[] id_array = dressing_ids[a].Split(new[] { "," }, StringSplitOptions.None);
                    //    dress.id = share.encry_value(id_array[1]);
                    string[] id_array = dressing_ids[a].Split(new[] { "@dress" }, StringSplitOptions.None);
                    dressing_record[0] = record_id;
                    dressing_record[1] = id ;
                    dressing_record[2] = id_array[0];
                    dressing_record[4] = examined_by;
                    dressing_record[5] = datetime;
                    if (id_array.Length > 1)
                    {
                        dressing_record[3] = id_array[1];
                        // dress.remark = share.encry_value(id_array[3]);
                    }
                    else
                    {
                        dressing_record[3] = "";

                    }
                    dressing_idstr[a] = dressing_record;
                  //  dressing.Add(dress);
                }
            }
 
            sql.Append(CWound.insert_wound_wash(vals, dressing_idstr, record_id));
 


            bool insert = false;
            String photo_id_str = "";
            if (!photo_data.Equals("0"))
            {
                string photo_id = share.Get_mysql_database_MaxID("care_wound_document", "wound_photo_id"); ;
                string[] values = new string[] { photo_id, id };
                sql.Append(CWound.post_wound_photo(values));
      
 
                //byte[] image_bytes = Convert.FromBase64String(photo_data);
                int a = share.ExecBoldSql(sql.ToString(), photo_data);
                if (a>0)
                {
                    insert = true;
                }
            }
            else
            {
                insert = share.bool_Ecxe_transaction_Sql(sql.ToString());
            }


 


            //    SELECT* FROM wahhei.care_wound_document;
            //    wound_photo_id, observation_id, document_photo, valid

 

 

            if (insert == false)
            {

                return null;

            }
            else
            {
          

                DataSet ds = share.GetDataSource(CWound.get_wound_brief(string.Format("and wound.care_wound_id = {0} ",wound_id)));
                DateTime nextdt = set_wound_next_date(now, decimal.Parse(ds.Tables[0].Rows[0][8].ToString()), decimal.Parse(ds.Tables[0].Rows[0][8].ToString()));
 
                string[] values = new string[] { wound_id, nextdt.ToString(ANICshare.sql_timeformat_nosecond),examined_by, datetime };
                share.bool_Ecxe_transaction_Sql(CWound.update_wound(1, values));
                String str = "YES";
                post_observation_record insertstate =
                new post_observation_record()
                {

                    is_update = share.encry_value(str),
                    nextdate = share.encry_value(nextdt.ToString(ANICshare.sql_timeformat_nosecond))

                };
                return insertstate;


            }

        }

        public post_observation_record post_wound_observation_record2(String wound_id, String length, String width, String depth, String level, String color,
String smell, String fluid_type, String fluid_quantity, String dressing, String clean_days, String clean_times, String examined_by)
        {


            wound_id = share.dencry_value(wound_id);
            length = share.dencry_value(length);
            width = share.dencry_value(width);
            depth = share.dencry_value(depth);
            level = share.dencry_value(level);
            color = share.dencry_value(color);
            smell = share.dencry_value(smell);
            fluid_type = share.dencry_value(fluid_type);
            fluid_quantity = share.dencry_value(fluid_quantity);
            clean_times = share.dencry_value(clean_times);
            clean_days = share.dencry_value(clean_days);
            dressing = share.dencry_value(dressing);

            examined_by = share.dencry_value(examined_by);
            DateTime now = share.get_current_datetime();
            String datetime = now.ToString(ANICshare.sql_timeformat_nosecond);
            //   string datetime = 
            StringBuilder sql = new StringBuilder();
            string id = share.Get_mysql_database_MaxID("care_wound_observation2", "observation_id"); ;

            string[] vals = new string[] {id,wound_id,datetime,length,width,depth,
                level,color,smell,fluid_type,fluid_quantity,
                clean_days,clean_times,"",examined_by,datetime };

            //string dress_ids_str = dressing.ToString();
            string record_id = share.Get_mysql_database_MaxID("care_wound_observation_dressing", "dressing_id");
            string[][] dressing_idstr = new string[0][];
            if (dressing.Length > 0 && !dressing.Equals("0"))
            {

                string[] dressing_ids = dressing.Split(new[] { "@dress_sep" }, StringSplitOptions.None);

                dressing_idstr = new string[dressing_ids.Length][];
                //    string []dressing_name = row[index++].ToString().Split(';'); ;

                for (int a = 0; a < dressing_ids.Length; a++)
                {
                    // wound_dressing dress = new wound_dressing();
                    string[] dressing_record = new string[12];

                    // string[] id_array = dressing_ids[a].Split(new[] { "," }, StringSplitOptions.None);
                    //    dress.id = share.encry_value(id_array[1]);
                    string[] id_array = dressing_ids[a].Split(new[] { "@dress" }, StringSplitOptions.None);
                    dressing_record[0] = record_id;
                    dressing_record[1] = id;
                    dressing_record[2] = id_array[0];
                    dressing_record[4] = examined_by;
                    dressing_record[5] = datetime;
                    if (id_array.Length > 1)
                    {
                        dressing_record[3] = id_array[1];
                        // dress.remark = share.encry_value(id_array[3]);
                    }
                    else
                    {
                        dressing_record[3] = "";

                    }
                    dressing_idstr[a] = dressing_record;
                    //  dressing.Add(dress);
                }
            }

            sql.Append(CWound.insert_wound_wash(vals, dressing_idstr, record_id));



            bool insert = false;
            String photo_id_str = "";

            insert = share.bool_Ecxe_transaction_Sql(sql.ToString());






            //    SELECT* FROM wahhei.care_wound_document;
            //    wound_photo_id, observation_id, document_photo, valid





            if (insert == false)
            {

                return null;

            }
            else
            {


                DataSet ds = share.GetDataSource(CWound.get_wound_brief(string.Format("and wound.care_wound_id = {0} ", wound_id)));
                DateTime nextdt = set_wound_next_date(now, decimal.Parse(ds.Tables[0].Rows[0][8].ToString()), decimal.Parse(ds.Tables[0].Rows[0][8].ToString()));

                string[] values = new string[] { wound_id, nextdt.ToString(ANICshare.sql_timeformat_nosecond), examined_by, datetime };
                share.bool_Ecxe_transaction_Sql(CWound.update_wound(1, values));
                String str = "YES";
                post_observation_record insertstate =
                new post_observation_record()
                {
                    observation_id = share.encry_value(id),
                    is_update = share.encry_value(str),
                    nextdate = share.encry_value(nextdt.ToString(ANICshare.sql_timeformat_nosecond))

                };
                return insertstate;


            }

        }
        public post_observation_record post_wound_observation_photo(String observation_id, String examined_by,  String photo_data )
        {


            observation_id = share.dencry_value(observation_id);
            examined_by = share.dencry_value(examined_by);

            StringBuilder sql = new  StringBuilder();

            bool insert = false;
            String photo_id_str = "";

            string photo_id = share.Get_mysql_database_MaxID("care_wound_document", "wound_photo_id"); 

            string ordering = share.Get_mysql_database_MaxID_ordering("care_wound_document", "ordering",string.Format("observation_id = {0}", observation_id));
            string[] values = new string[] { photo_id, observation_id,ordering, examined_by };
            sql.Append(CWound.post_wound_photo2(values));


            //byte[] image_bytes = Convert.FromBase64String(photo_data);
            int a = share.ExecBoldSql(sql.ToString(), photo_data);
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
        public static DateTime set_wound_next_date(  DateTime dt, Decimal day, Decimal times)
        {
            int one_day = 24;
            int add_hour = (int)((one_day * day) / times);

           return dt.AddHours(add_hour);
            //- int result = DateTime.Compare(st.Date, ed.Date);
           

        }

        public post_record post_wound_nextdate(String wound_id, String nextdate, String examined_by)
        {
            wound_id = share.dencry_value(wound_id);
            nextdate = share.dencry_value(nextdate);
 
            examined_by = share.dencry_value(examined_by);
            DateTime now = share.get_current_datetime();
            String datetime = now.ToString(ANICshare.sql_timeformat_nosecond);
            //   string datetime = 
            StringBuilder sql = new StringBuilder();
 
            string[] vals = new string[] { wound_id,nextdate, examined_by,datetime };
 
 
            sql.Append(CWound.update_wound(1,vals));
            //string dress_ids_str = dressing.ToString();
 

           bool    insert = share.bool_Ecxe_transaction_Sql(sql.ToString());

            //    SELECT* FROM wahhei.care_wound_document;
            //    wound_photo_id, observation_id, document_photo, valid





            if (insert == false)
            {

                return null;

            }
            else
            {
                 String str = "YES";
                post_record insertstate =
                new post_record()
                {
                    is_update = share.encry_value(str) ,
                };
                return insertstate;


            }

        }





        public wound_reocrds[] get_wound_records(string client_id)
        {
 



            /*
            List<string> care_wound_id_str = new List<string>();
            //String medicine_name_str = "";
            List<string> wound_position_id_str = new List<string>();
            List<string> wound_last_state_str = new List<string>();
            List<string> wound_position_direction_str = new List<string>();
            List<string> wound_position_discover_str = new List<string>();


            client_id = share.dencry_value(client_id);
  
            DataSet ds = share.GetDataSource(Cmedical.get_wound_records(client_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;

                    weight_str.Add(share.encry_value(row[i++].ToString()));
                    //medicine_name_str = dr["medicinename"].ToString();
                    handlingperson_str.Add(share.encry_value(row[i++].ToString()));
                    timestamp_str.Add(share.encry_value(row[i++].ToString()));
                }

            }








            //String medicine_name_str = "";

            MySqlConnection conn = new MySqlConnection("server=" + dbHost + "; PORT= " + dbPort + " ;user id=" + dbUser + "; password=" + dbPass + "; database=" + dbName + "; CharSet=utf8");
            //MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand command = conn.CreateCommand();

            conn.Open();

            string cmdText = "";
            string recovery_state_str = "N";
            cmdText = "SELECT * FROM care_wound2 where client_id = (@id) and recovery = (@state)";

            //String cmdText = "SELECT * FROM care_body_temperature where client_id = (@id) ORDER BY examination_datetime DESC LIMIT 1";


            //SqlCommand xp = new SqlCommand("select * from user_login Where username IN (trial)", vid);



            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            cmd.Parameters.AddWithValue("@id", client_id);
            cmd.Parameters.AddWithValue("@state", recovery_state_str);

            MySqlDataReader reader = cmd.ExecuteReader(); //execure the reader




            while (reader.Read())
            {
                //List<string> care_wound_id_str = new List<string>();
                //String medicine_name_str = "";
                //List<string> wound_position_id_str = new List<string>();

                care_wound_id_str.Add(reader["care_wound_id"].ToString());
                //medicine_name_str = dr["medicinename"].ToString();
                wound_position_id_str.Add(reader["wound_position_id"].ToString());
                //wound_position_direction_str.Add(reader["wound_direction"].ToString());

                //timestamp_str.Add(Convert.ToDateTime(reader["examination_datetime"]).ToString("yyyy-MM-dd HH:mm:ss"));



            }
            reader.Close();







            //Debug.Print(care_wound_id_str[0]);
            for (int i = 0; i < care_wound_id_str.Count(); i++)
            {



                string wound_level_str = "";
                string wound_length_str = "";
                string wound_width_str = "";
                string wound_depth_str = "";
                string color_str = "";
                string smell_str = "";
                string fluid_type_str = "";
                string fluid_quantity_str = "";
                string wound_dressing_str = "";
                string wound_photo_id = "";

                string wound_observation_date_str = "";
                string handling_person_str = "";

                string cmdText1 = "SELECT * FROM care_wound_observation_record where care_wound_id = (@id) ORDER BY wound_observation_date DESC LIMIT 1 ";

                //cmdText1 = "SELECT * FROM care_wound_observation_record where care_wound_id = (@id) ORDER BY wound_observation_date DESC LIMIT 1 ";
                MySqlCommand cmd1 = new MySqlCommand(cmdText1, conn);
                string wound_id_str = care_wound_id_str[i];
                cmd1.Parameters.AddWithValue("@id", wound_id_str);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {


                    wound_level_str = reader1["wound_condition_level"].ToString();
                    wound_length_str = reader1["wound_length"].ToString();
                    wound_width_str = reader1["wound_width"].ToString();
                    wound_depth_str = reader1["wound_depth"].ToString();
                    color_str = reader1["wound_color"].ToString();
                    smell_str = reader1["wound_smell"].ToString();
                    fluid_type_str = reader1["wound_fluid_type"].ToString();
                    fluid_quantity_str = reader1["wound_fluid_quantity"].ToString();
                    //wound_dressing_str = reader1["wound_dressing_id"].ToString();
                    wound_photo_id = reader1["wound_photo_id"].ToString();
                    wound_observation_date_str = Convert.ToDateTime(reader1["wound_observation_date"]).ToString("yyyy-MM-dd HH:mm:ss");
                    handling_person_str = reader1["examined_by"].ToString();



                }

                reader1.Close();





                string cmdText2 = "SELECT * FROM care_wound_plan where care_wound_id = (@id) ORDER BY modified_datetime DESC  LIMIT 1";

                //List<string> record_list = new List<string>();
                //cmdText1 = "SELECT * FROM care_wound_observation_record where care_wound_id = (@id) ORDER BY wound_observation_date DESC LIMIT 1 ";
                MySqlCommand cmd2 = new MySqlCommand(cmdText2, conn);
                //string wound_id_str = care_wound_id_str[i];


                cmd2.Parameters.AddWithValue("@id", care_wound_id_str[i]);
                MySqlDataReader reader2 = cmd2.ExecuteReader();
                while (reader2.Read())
                {
                    wound_dressing_str = reader2["available_dressing"].ToString();



                }

                wound_dressing_str = wound_dressing_str.Replace(";", ",");
                reader2.Close();



                string[] wound_arr = new string[] { wound_level_str, wound_length_str, wound_width_str, wound_depth_str, color_str,smell_str,fluid_type_str,fluid_quantity_str,wound_dressing_str,
                wound_photo_id,wound_observation_date_str,handling_person_str };
                string wound_arr_str = string.Join(";", wound_arr);

                wound_last_state_str.Add(wound_arr_str);

            }





            recovery_state_str = "U";
            cmdText = "SELECT * FROM care_wound2 where client_id = (@id) and recovery = (@state)";

            //String cmdText = "SELECT * FROM care_body_temperature where client_id = (@id) ORDER BY examination_datetime DESC LIMIT 1";


            //SqlCommand xp = new SqlCommand("select * from user_login Where username IN (trial)", vid);



            cmd = new MySqlCommand(cmdText, conn);
            cmd.Parameters.AddWithValue("@id", client_id);
            cmd.Parameters.AddWithValue("@state", recovery_state_str);

            reader = cmd.ExecuteReader(); //execure the reader




            while (reader.Read())
            {

                wound_position_discover_str.Add(reader["wound_position_id"].ToString());
                //wound_position_direction_str.Add(reader["wound_direction"].ToString());

                //timestamp_str.Add(Convert.ToDateTime(reader["examination_datetime"]).ToString("yyyy-MM-dd HH:mm:ss"));



            }
            reader.Close();


            wound_reocrds[] wound_record = new wound_reocrds[]{
            new wound_reocrds(){
                care_wound_id = care_wound_id_str,
                wound_position_id = wound_position_id_str,
                wound_position_direction = wound_position_direction_str,
                wound_last_state = wound_last_state_str,
                wound_position_discover =wound_position_discover_str

        }

        };
            conn.Close();

            return wound_record;

    */

            return null;

        }

        public pictureinfo get_wound_image(string picture_id)
        {
            picture_id = share.dencry_value(picture_id);
            byte[] imgData = null;
            if (picture_id == "0")
            {
                return null;
            }



            DataSet ds = share.GetDataSource(CWound.get_wound_photo(picture_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                imgData = (byte[])row[0];
            }

            if (imgData != null)
            {
                pictureinfo image =
new pictureinfo()
{
    picturedata = Convert.ToBase64String(imgData)
};
                return image;
            }
            else
            {
                return null;
            }

        }


        public care_briefing[] get_care_briefing(string client_id)
        {


            /*

            //string label_usage_id_str = "";
            //string label_usage_code_str = "";
            //restrict_details detail = getrestraintrecords(client_id);
            restrict_details[] detail = get_restrict_detail(client_id);

            string restrict_record_str = "";
            string body_turning_str = "";
            string water_control_str = "";
            string defecation_str = "";


            //string[] brief_arr = new string[] { blood_oxygen_str, blood_oxygen_str, blood_glucose_str, respiration_rate_str, body_temperature_str, body_weight_str };
            string[] table_name_arr = new string[] { "care_blood_oxygen", "care_blood_glucose",
            "care_respiration_rate", "care_body_temperature", "care_body_weight" };

            string[] column_name_arr = new string[] { "blood_oxygen", "blood_glucose",
            "respiration_rate", "temperature", "weight" };



            MySqlConnection conn = new MySqlConnection("server=" + dbHost + "; PORT= " + dbPort + " ;user id=" + dbUser + "; password=" + dbPass + "; database=" + dbName + "; CharSet=utf8");

            MySqlCommand command = conn.CreateCommand();

            conn.Open();

            String cmdText0 = "select * from care_body_restrict Where client_id = (@id)ORDER BY examination_datetime DESC LIMIT 1";


            MySqlCommand cmd1 = new MySqlCommand(cmdText0, conn);
            cmd1.Parameters.AddWithValue("@id", client_id);

            MySqlDataReader reader1 = cmd1.ExecuteReader(); //execure the reader


            while (reader1.Read())
            {

                string check_result_str = "";
                string check_type_str = "";
                string handlingperson_str = "";
                string timestamp_str = "";



                check_result_str = reader1["check_result"].ToString();
                check_type_str = reader1["check_type"].ToString();
                handlingperson_str = reader1["examined_by"].ToString();

                timestamp_str = Convert.ToDateTime(reader1["examination_datetime"]).ToString("yyyy-MM-dd HH:mm:ss");

                string[] details = { check_result_str, check_type_str };

                string[] record_arr = { string.Join<string>(",", details), handlingperson_str, timestamp_str };
                restrict_record_str = string.Join<string>(";", record_arr);


            }

            reader1.Close();



            cmdText0 = "SELECT * FROM care_body_turning where client_id = (@id) ORDER BY examination_datetime DESC LIMIT 1";



            cmd1 = new MySqlCommand(cmdText0, conn);
            cmd1.Parameters.AddWithValue("@id", client_id);

            reader1 = cmd1.ExecuteReader(); //execure the reader


            while (reader1.Read())
            {
                string body_turning = "";
                string handlingperson_str = "";
                string timestamp_str = "";

                body_turning = reader1["body_turning"].ToString();
                handlingperson_str = reader1["examined_by"].ToString();

                timestamp_str = Convert.ToDateTime(reader1["examination_datetime"]).ToString("yyyy-MM-dd HH:mm:ss");
                string[] record_arr = new string[] { body_turning, handlingperson_str, timestamp_str };

                body_turning_str = string.Join(";", record_arr);
            }


            reader1.Close();

            cmdText0 = "SELECT * FROM care_water_control where client_id = (@id) AND created_datetime >= CURDATE() ORDER BY created_datetime DESC";

            cmd1 = new MySqlCommand(cmdText0, conn);
            cmd1.Parameters.AddWithValue("@id", client_id);

            reader1 = cmd1.ExecuteReader(); //execure the reader



            int water_control_in_int = 0;
            int water_control_out_int = 0;
            int water_control_total_int = 0;

            string last_timestamp = "";


            while (reader1.Read())
            {
                string water_direction_str = "";
                string channel_name_str = "";
                string quantity_str = "";


                water_direction_str = reader1["water_direction"].ToString();
                channel_name_str = reader1["channel_name"].ToString();
                quantity_str = reader1["quantity"].ToString();

                last_timestamp = Convert.ToDateTime(reader1["created_datetime"]).ToString("yyyy-MM-dd HH:mm:ss");

                int numVal = Int32.Parse(quantity_str);

                if (water_direction_str.Equals("in"))
                {
                    water_control_in_int = water_control_in_int + numVal;
                    water_control_total_int = water_control_total_int + numVal;
                }
                else
                {
                    water_control_out_int = water_control_out_int + numVal;
                    water_control_total_int = water_control_total_int - numVal;

                }
            }
            if (last_timestamp == "")
            {
                last_timestamp = "0";
            }
            reader1.Close();
            string[] water_arr = { water_control_in_int.ToString(), water_control_out_int.ToString(), water_control_total_int.ToString(), last_timestamp.ToString() };
            water_control_str = string.Join<string>(";", water_arr);

            //water_control_str = water_control_total_int.ToString();




            cmdText0 = "SELECT * FROM care_defecation where client_id = (@id) ORDER BY created_datetime DESC LIMIT 1";

            cmd1 = new MySqlCommand(cmdText0, conn);
            cmd1.Parameters.AddWithValue("@id", client_id);

            reader1 = cmd1.ExecuteReader(); //execure the reader

            while (reader1.Read())
            {
                string quality_str = "";
                string quantity_str = "";
                string color_str = "";


                string handlingperson_str = "";
                string timestamp_str = "";


                quality_str = reader1["quality"].ToString();
                quantity_str = reader1["quantity"].ToString();
                color_str = reader1["color"].ToString();


                handlingperson_str = reader1["created_by"].ToString();
                timestamp_str = Convert.ToDateTime(reader1["created_datetime"]).ToString("yyyy-MM-dd HH:mm:ss");

                string[] details = { quality_str, color_str, quantity_str };
                string details_str = string.Join<string>(",", details);

                string[] record_arr = { details_str, handlingperson_str, timestamp_str };
                defecation_str = string.Join<string>(";", record_arr);


            }


            reader1.Close();



            conn.Close();


            care_briefing[] brief = new care_briefing[]{
                new care_briefing(){
                    resfsda = detail,
                    restrict_record = restrict_record_str,
                    body_turning= body_turning_str,
                    water_control = water_control_str,
                    defecation = defecation_str

                }
        };

            return brief;
            */
            return null;

        }

        //Raymond 20200909
       
        public client_log_setting get_client_log_setting_old(string idx )
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";
            string condition = "";
            idx = share.dencry_value(idx);



            StringBuilder sql = new StringBuilder();
            sql.Append(Cclient.get_client_log_setting(""));
            sql.Append(Cclient.get_full_client( "and  per.active_status = 'Y' "));

    
            DataSet ds = share.GetDataSource(sql.ToString());
            List<company_department> logs = new List<company_department>();
            List<client_briefing2> clients = new List<client_briefing2>();

            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow data_row = ds.Tables[0].Rows[i];
                    // string array = share.
                    string[] row = share.obj_arr_to_string_arr(data_row.ItemArray);
                    
                    company_department depart = new company_department();
           

                   // log = new client_log();
                    int index =0;

                    depart.department_id = row[index++].ToString();
                    depart.department_name = row[index++].ToString();
                    logs.Add(depart);


                }
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    DataRow data_row = ds.Tables[1].Rows[i];
                    // string array = share.
                    string[] row = share.obj_arr_to_string_arr(data_row.ItemArray);

                    client_briefing2 client = new client_briefing2();

                   // sql.Append("SELECT per.client_id, CONCAT(per.chi_surname, per.chi_name),eng_name,eng_surname,sex,");
                   // sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");
                  //  sql.Append("ifnull(doc.client_photo_id,0), ");

                   // sql.Append("ae.ae_status ");


                    // log = new client_log();
                    int index = 0;
                    client.client_id = row[index++].ToString();
                    client.name = row[index++].ToString();

                    index++;
                    index++;

                    client.sex = row[index++].ToString();
                    client.birth_date = row[index++].ToString();
                    client.bednum = row[index++].ToString();

                    client.block = row[index++].ToString();
                    client.zone = row[index++].ToString();

                    clients.Add(client);


                }
            }
            if (clients.Count==0&&logs.Count==0)
            {
                return null;
            }
            else
            {
                client_log_setting setting = new client_log_setting() { departments = logs.ToArray(),clients = clients.ToArray() };


                return setting;
            }
         


            
        }

        public client_log_setting get_client_log_setting(string idx)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";
            string condition = "";
            idx = share.dencry_value(idx);



            StringBuilder sql = new StringBuilder();
            sql.Append(Cclient.get_client_log_setting(""));
            sql.Append(Cclient.get_client_str(" where per.valid = 'Y' and per.client_id <> '' and per.active_status = 'Y' and per.client_id not in (select distinct client_id from client_book2 where book_status = '預訂中' or book_status = '已取消' )"));


            DataSet ds = share.GetDataSource(sql.ToString());
            List<company_department> logs = new List<company_department>();
            List<client_briefing2> clients = new List<client_briefing2>();
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow data_row = ds.Tables[0].Rows[i];
                        // string array = share.
                        string[] row = share.obj_arr_to_string_arr(data_row.ItemArray);

                        company_department depart = new company_department();


                        // log = new client_log();
                        int index = 0;

                        depart.department_id = row[index++].ToString();
                        depart.department_name = row[index++].ToString();
                        logs.Add(depart);


                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        DataRow data_row = ds.Tables[1].Rows[i];
                        // string array = share.
                        string[] row = share.obj_arr_to_string_arr(data_row.ItemArray);

                        client_briefing2 client = new client_briefing2();

                        // sql.Append("SELECT per.client_id, CONCAT(per.chi_surname, per.chi_name),eng_name,eng_surname,sex,");
                        // sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");
                        //  sql.Append("ifnull(doc.client_photo_id,0), ");

                        // sql.Append("ae.ae_status ");


                        // log = new client_log();
                        int index = 0;
                        client.client_id = row[index++].ToString();
                        client.name = row[index++].ToString();

                        index++;
                        index++;

                        client.sex = row[index++].ToString();
                        client.birth_date = row[index++].ToString();
                        client.bednum = row[index++].ToString();

                        client.block = row[index++].ToString();
                        client.zone = row[index++].ToString();

                        clients.Add(client);


                    }
                }
            }
            if (clients.Count == 0 && logs.Count == 0)
            {
                return null;
            }
            else
            {
                client_log_setting setting = new client_log_setting() { departments = logs.ToArray(), clients = clients.ToArray() };


                return setting;
            }

        }

        public client_log[] get_client_log(string startdate ,string enddate,string client_id,string department_id)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";
            string condition = "";
            bool reply_bool = false;
            startdate = share.dencry_value(startdate);
            enddate = share.dencry_value(enddate);
            client_id = share.dencry_value(client_id);
            department_id = share.dencry_value(department_id);

            if (!client_id.Equals("0"))
            {
                condition = string.Format("and log.client_id in ({0})", client_id.Replace(";",","));
            }
            if (!department_id.Equals("0"))
            {
                condition = condition+ string.Format(" and log.department_id in ({0}) ", department_id.Replace(";", ","));
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(Cclient.get_client_log(condition +string.Format(


              //  "and date(reply.reply_datetime) >=date('{0}') and date(date(reply.reply_datetime)) <=date('{1}') group by log_id ",
              "GROUP BY log.log_id having  DATE(updatedatetime) >= DATE('{0}')  and DATE(updatedatetime) <= DATE('{1}') order by updatedatetime desc; ",  startdate, enddate), "1"));
            //  sql.Append(Cclient.get_client_log_reply_detail(string.Format(" and reply.log_id = {0}", log_id)));
            //  sql.Append(Cclient.get_client_briefing(string.Format("and date(reply.reply_datetime) >=date('{0}') and date(date(reply.reply_datetime)) <=date('{0}')  ",
            //startdate, enddate) + condition, "1"));
            
            client_log log = new client_log();
            
            DataSet ds = share.GetDataSource(sql.ToString());
            List<client_log> logs = new List<client_log>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow data_row = ds.Tables[0].Rows[i];
                    // string array = share.
                    string[] row =share. obj_arr_to_string_arr(data_row.ItemArray);
 
                    log = new client_log();
                    int index = 1;
                    log.log_id = row[index++].ToString();
                    log.client_id = row[index++].ToString();
                    log.client_name = row[index++].ToString();
                    log.isimportant = row[index++].ToString();

                    log.department_id = row[index++].ToString();
                    log.department_name = row[index++].ToString();
                    log.title = row[index++].ToString();
                    log.event_datetime = row[index++].ToString();
                    log.content = row[index++].ToString();
                    index++;
                    if (row[index].ToString().Length>0)
                    {
                        reply_bool = true;
                        client_log_reply reply = new client_log_reply()
                        {
                            reply_id = row[index++].ToString(),
                            content = row[index++].ToString(),
                            create_by = row[index++].ToString(),
                            create_datetime = row[index++].ToString(),
                            reply_ordering = row[index++].ToString()

                        };
                        log.lastupdate_time = reply.create_by;
                        client_log_reply[]  replys= new client_log_reply[] { reply };
                        log.replys = replys;
                    }
                    else
                    {
                        index++;
                        index++;
                        index++;
                        index++;
                        index++;
                   
                    }
                    log.reply_count = row[index++].ToString();

                    index++;
                    index++;
                    index++;


                    log.create_by   = row[index++].ToString();
                    log.create_datetime =   log.lastupdate_time = row[index++].ToString();
                    if (share.dencry_value( row[index].ToString()).Length > 0)
                    {
                        log.create_by = row[index++].ToString();
                        log.create_datetime =     row[index++].ToString();

                        index++;


                    }
                    else
                    {
                        index++;
                        index++;
                        index++;
                    }
                    log.lastupdate_time = row[index++].ToString();
                    logs.Add(log);
                }
            }
       
            else
            {
                return null;
            }


            return logs.ToArray();
        }
        public client_log get_client_log_reply(string log_id)
        {
            //string label_usage_id_str = "";
            //string label_usage_code_str = "";

            log_id = share.dencry_value(log_id);


            StringBuilder sql = new StringBuilder();
            sql.Append(Cclient.get_client_log_detail(string.Format(" and log.log_id = {0} ;", log_id)));
            sql.Append(Cclient.get_client_log_reply_detail(string.Format(" and reply.log_id = {0}", log_id)));

            client_log log;
            DataSet ds = share.GetDataSource(sql.ToString());

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
     
                log = new client_log();
                log.log_id = share.encry_value( log_id);
                int index = 1;
                log.client_id = share.encry_value(row[index++].ToString());
                log.client_name = share.encry_value(row[index++].ToString());
                log.department_name = share.encry_value(row[index++].ToString());
                log.title = share.encry_value(row[index++].ToString());
                log.event_datetime = share.encry_value(row[index++].ToString());
                log.content = share.encry_value(row[index++].ToString());

                log.isimportant = share.encry_value(row[index++].ToString());
                log.create_by = share.encry_value(row[index++].ToString());
                log.create_datetime = share.encry_value(row[index++].ToString());
                if (row[index].ToString().Length > 0)
                {
                    log.create_by = share.encry_value(row[index++].ToString());
                    log.create_datetime = share.encry_value(row[index++].ToString());
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    client_log_reply[] replys = new client_log_reply[ds.Tables[1].Rows.Count];
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        int index2 = 1;
                        DataRow reply_row = ds.Tables[1].Rows[i];
                        client_log_reply reply = new client_log_reply();
                        reply.reply_id = share.encry_value(reply_row[index2++].ToString());
                        reply.reply_ordering = share.encry_value(reply_row[index2++].ToString());

                        reply.content = share.encry_value(reply_row[index2++].ToString());

                        reply.create_by = share.encry_value(reply_row[index2++].ToString());
                        reply.create_datetime = share.encry_value(reply_row[index2++].ToString());

                        if (reply_row[index2].ToString().Length > 0)
                        {
                            reply.create_by = share.encry_value(reply_row[index2++].ToString());
                            reply.create_datetime = share.encry_value(reply_row[index2++].ToString());
                        }
                        replys[i] = reply;


                    }
                    log.replys = replys;
                }

            }
            else
            {
                return null;
            }


            return log;
        }
        public client_log_update post_client_log(String client_id,string department_id,string event_datetime,string title,
            String content,string isimportant,string  boardcast_datetime, String examined_by)
        {
     
            // examined_by = share.dencry_value(examined_by);
            client_id = share.dencry_value(client_id);
            department_id = share.dencry_value(department_id);
            event_datetime = share.dencry_value(event_datetime);
            title = share.dencry_value(title);
            content = share.dencry_value(content);
            isimportant = share.dencry_value(isimportant);
            boardcast_datetime = share.dencry_value(boardcast_datetime);
            examined_by = share.dencry_value(examined_by);

            if (boardcast_datetime.Equals("0"))
            {
                boardcast_datetime = "1900-05-16";
            }
            title = title.Replace("'", "''");
            content = content.Replace("'", "''");

       
            DateTime now = share.get_current_datetime();
            String datetime = now.ToString(ANICshare.sql_timeformat_nosecond);
            //   string datetime = 

            //  sql.Append("insert client_log(log_id, client_id,department_id,title,event_datetime,
            //content,dash_board_shown,dash_board_always_shown,log_always_shown, ");
          //  sql.Append("boardcast_datetime,created_by, created_datetime )values");



            StringBuilder sql = new StringBuilder();

            // string record_id = share.Get_mysql_database_MaxID("client_log_reply", "record_id");

            string record_id = share.Get_mysql_database_MaxID("client_log", "record_id");
            string log_id = share.Get_mysql_database_MaxID("client_log", "log_id");

            //  reply_id, log_id, content, reply_datetime, reply_ordering, modified_by, modified_datetime, created_by, created_datetime, valid
          //  string order = "1";
       

            string[] vals = new string[] { record_id, log_id, client_id,department_id,title,event_datetime, content,"N","N",isimportant,boardcast_datetime,examined_by,datetime   };
            // sql.Append("insert client_log_reply(reply_id, log_id,reply_datetime,content,reply_ordering , ");
            // sql.Append("created_by, created_datetime )values");

            sql.Append(Cclient.insert_client_log(vals));
            //string dress_ids_str = dressing.ToString();


            bool insert = share.bool_Ecxe_transaction_Sql(sql.ToString());

            //    SELECT* FROM wahhei.care_wound_document;
            //    wound_photo_id, observation_id, document_photo, valid






            if (insert == false)
            {

                return null;

            }
            else
            {
                String str = "YES";
                client_log_update insertstate =
                new client_log_update()
                {
                    state = share.encry_value(str),
                };
                return insertstate;


            }

        }
        public client_log_update post_client_log_reply(String log_id, String content, String examined_by)
        {
            content = share.dencry_value(content);
            // examined_by = share.dencry_value(examined_by);
            log_id = share.dencry_value(log_id);
            examined_by = share.dencry_value(examined_by);
            DateTime now = share.get_current_datetime();
            String datetime = now.ToString(ANICshare.sql_timeformat_nosecond);
            //   string datetime = 
            StringBuilder sql = new StringBuilder();

           // string record_id = share.Get_mysql_database_MaxID("client_log_reply", "record_id");

            string reply_id = share.Get_mysql_database_MaxID("client_log_reply", "reply_id"); ;
          //  reply_id, log_id, content, reply_datetime, reply_ordering, modified_by, modified_datetime, created_by, created_datetime, valid
            string order = "1";
            StringBuilder order_sql = new StringBuilder();

            order_sql.Append("select ifnull(max(reply_ordering)+1,1) ");
            order_sql.Append("from client_log_reply reply where reply.log_id = {0} and valid = 'Y';");
            DataSet order_ds = share.GetDataSource(string.Format(order_sql.ToString(), log_id));
            if (order_ds.Tables[0].Rows.Count > 0)
            {
                //return ds.Tables[0].Rows[0][0].ToString();
                order = order_ds.Tables[0].Rows[0][0].ToString();
            }


            string[] vals = new string[] {   reply_id, log_id, datetime, content, order, examined_by, datetime };
            // sql.Append("insert client_log_reply(reply_id, log_id,reply_datetime,content,reply_ordering , ");
            // sql.Append("created_by, created_datetime )values");

            sql.Append(Cclient.insert_client_log_reply(vals));
            //string dress_ids_str = dressing.ToString();


            bool insert = share.bool_Ecxe_transaction_Sql(sql.ToString());

            //    SELECT* FROM wahhei.care_wound_document;
            //    wound_photo_id, observation_id, document_photo, valid






            if (insert == false)
            {

                return null;

            }
            else
            {
                String str = "YES";
                client_log_update insertstate =
                new client_log_update()
                {
                    state = share.encry_value(str),
                };
                return insertstate;


            }

        }
        public byte[] getdataset(String user, String pw, String sql,String is_test )
        {
            Encription eny = new Encription();
            try
            {
                user = eny.decrypt(user);
            }
            catch (Exception)
            {
                return null;
         
            }
            if (!check_login(user,pw))
            {
                return null;
            }

            try
            {

               ANICshare sh = getshare();

        
                is_test = eny.decrypt(is_test);
                sh = set_share_test(sh, is_test);

                DataSet ds = sh.GetDataSource(eny.decrypt(sql));
                byte[] bytes = ConvertDataSetToByteArray(ds);
               
                byte [] header =  Encoding.UTF8.GetBytes(user);
                byte[] sufix = Encoding.UTF8.GetBytes(pw);

                byte[] new_bytes = new byte[bytes.Length+(header.Length+ sufix.Length)];
                Buffer.BlockCopy(header, 0, new_bytes, 0, header.Length);
                Buffer.BlockCopy(bytes, 0, new_bytes, header.Length, bytes.Length);
                Buffer.BlockCopy(sufix, 0, new_bytes, header.Length+ bytes.Length, sufix.Length);

                return new_bytes;

                /*
                if (!test.Equals(share.dencry_value(dbName)))
                {
                    return null;
                }
                string state = conn.testmysql();
                ANI_server ani = new ANI_server()
                { server_state = state };
                return ani;
                */
            }
            catch (Exception)
            {

                return null;
            }

        }
        private static readonly byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

        public byte[] get_data(String user, String pw, String sql,   String is_test)
        {
            Encription eny = new Encription();
            try
            {
                user = eny.decrypt(user);
              

            }
            catch (Exception)
            {
                return null;

            }
            if (!check_login(user, pw))
            {
                return null;
            }

            try
            {

                ANICshare sh = getshare();


                is_test = eny.decrypt(is_test);
                sh = set_share_test(sh, is_test);

                DataSet ds = sh.GetDataSource(eny.decrypt(sql));

               // byte[] key = Encoding.ASCII.GetBytes(user);

                //byte[] iv = Encoding.ASCII.GetBytes(keyindex.ToString());




                byte[] bytes = ConvertDataSetToByteArray(ds);

                byte[] header = Encoding.UTF8.GetBytes(user);
                byte[] sufix = Encoding.UTF8.GetBytes(pw);

                byte[] new_bytes = Encrypt(bytes, user);
                return new_bytes;

                /*
                if (!test.Equals(share.dencry_value(dbName)))
                {
                    return null;
                }
                string state = conn.testmysql();
                ANI_server ani = new ANI_server()
                { server_state = state };
                return ani;
                */
            }
            catch (Exception)
            {

                return null;
            }

        }
        private byte[] Encrypt(byte[] message, string key)
        {
            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            Rijndael rijndael = Rijndael.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, SALT);
            rijndael.Key = pdb.GetBytes(32);
            rijndael.IV = pdb.GetBytes(16);
            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(message, 0, message.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();


        }
        private static byte[] Decrypt(byte[] message, string password)
        {
            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            Rijndael rijndael = Rijndael.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
            rijndael.Key = pdb.GetBytes(32);
            rijndael.IV = pdb.GetBytes(16);
            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(message, 0, message.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        private static byte[] EncryptBytes(
            SymmetricAlgorithm alg,
            byte[] message)
        {
            if ((message == null) || (message.Length == 0))
            {
                return message;
            }

            if (alg == null)
            {
                throw new ArgumentNullException("alg");
            }

            using (var stream = new MemoryStream())
            using (var encryptor = alg.CreateEncryptor())
            using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(message, 0, message.Length);
                encrypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }

        private static byte[] DecryptBytes(SymmetricAlgorithm alg, byte[] message)
        {
            if ((message == null) || (message.Length == 0))
            {
                return message;
            }

            if (alg == null)
            {
                throw new ArgumentNullException("alg");
            }

            using (var stream = new MemoryStream())
            using (var decryptor = alg.CreateDecryptor())
            using (var encrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(message, 0, message.Length);
                encrypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }
        public byte[] Ecxe_transaction_Sql(String user, String pw, String sql, String is_test)
        {
            Encription eny = new Encription();
            try
            {
                user = eny.decrypt(user);
            }
            catch (Exception)
            {
                return null;

            }
            if (!check_login(user, pw))
            {
                return null;
            }

            try
            {
                ANICshare sh = getshare();
    
                sh = set_share_test(sh, eny.decrypt(is_test));
                int i = sh.Ecxe_transaction_Sql(eny.decrypt(sql));


                return Encoding.ASCII.GetBytes(eny.encrypt( i.ToString()));
 
            }
            catch (Exception)
            {

                return null;
            }

        }

        public byte[] Ecxe_transaction_Sql2(String user, String pw, String sql, String sql2, String is_check, String is_test)
        {
            Encription eny = new Encription();
            try
            {
                user = eny.decrypt(user);
            }
            catch (Exception)
            {
                return null;
            }
            if (!check_login(user, pw))
            {
                return null;
            }
            try
            {
                ANICshare sh = getshare();
                sh = set_share_test(sh, eny.decrypt(is_test));
                int i = sh.Ecxe_transaction_Sql2(eny.decrypt(sql), eny.decrypt(sql2), eny.decrypt(is_check) == "Y" ? true : false);
                return Encoding.ASCII.GetBytes(eny.encrypt(i.ToString()));
            }
            catch (Exception)
            {
                return null;
            }
        }
        public byte[] ExecBoldSql(String user, String pw,  String sql, String is_test,string image)
        {
            Encription eny = new Encription();
            try
            {
                user = eny.decrypt(user);
            }
            catch (Exception)
            {
                return null;

            }
            if (!check_login(user, pw))
            {
                return null;
            }

            try
            {
                ANICshare sh = getshare();
 
                sh = set_share_test(sh, eny.decrypt(is_test));


                int i = sh.ExecBoldSql(eny.decrypt(sql), image);


                return Encoding.ASCII.GetBytes(eny.encrypt(i.ToString()));

            }
            catch (Exception)
            {

                return null;
            }

        }

        private ANICshare set_share_test(ANICshare sh, string is_test)
        {
            if (is_test == "Y")
            {
                sh.set_test = true;
            }
            else
            {
                sh.set_test = false;
            }
            return sh;
        }

        private void check_is_test(string teststr)
        {
 
            if (teststr == "Y")
            {
                set_test = true;
            }
            else
            {
                set_test = false;
            }

        }
        private byte[] ConvertDataSetToByteArray(DataSet dataSet)
        {
            byte[] binaryDataResult = null;
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter brFormatter = new BinaryFormatter();
                dataSet.RemotingFormat = SerializationFormat.Binary;
                brFormatter.Serialize(memStream, dataSet);
                binaryDataResult = memStream.ToArray();
            }
            return binaryDataResult;
        }



        private bool check_login(string name,string pw)
        {
            if (name.Equals(LoginName)&&pw.Equals(LoginPass))
            {
                return true;
                
            }
            else
            {
                return false;
            }
        }
        public alarm_update post_alarm_data(String id, String group, String port, String state, String time )
        {
            /*
            { "id",eny.encrypt(txc.id)},
   { "group", eny.encrypt(txc.mcl) },
      { "port",eny.encrypt(txc.gateway) },
                 { "state",eny.encrypt(txc.cmd) },
            { "time",eny.encrypt(txc.tx_time.ToString()) },
            */
            Encription eny = new Encription();
            try
            {
                id = eny.decrypt(id);
                group = eny.decrypt(group);
                port = eny.decrypt(port);
                state = eny.decrypt(state);
                time = eny.decrypt(time);

            }
            catch (Exception)
            {
                return null;

            }
 
            try
            {

                //ANICshare sh = getshare();
                //share
                CAlarm alarm = new CAlarm();

                string log_id = share.Get_mysql_database_MaxID("client_alarm_log", "log_id"); ;
                string[] values = new string[] { log_id,  group, id, port, state, share.get_current_time(), System.Environment.MachineName };
                //  SELECT* FROM azure.client_alarm_log;
                //  log_id, mcl, device_id, gateway, time, created_by, created_datetime, valid
                bool insert = share.bool_Ecxe_transaction_Sql(alarm.inser_alarm_state(values));
                // alarm.inser_alarm_state

                if (insert)
                {
                    alarm_update update = new alarm_update() {

                        state = eny.encrypt("yes")
                    

                    };

                    return update;

                }
                else
                {
                    return null;
                }

                /*
                if (!test.Equals(share.dencry_value(dbName)))
                {
                    return null;
                }
                string state = conn.testmysql();
                ANI_server ani = new ANI_server()
                { server_state = state };
                return ani;
                */
            }
            catch (Exception)
            {

                return null;
            }

        }
        public alarm_update post_alarm_msg(String data )
        {
            /*
            { "id",eny.encrypt(txc.id)},
   { "group", eny.encrypt(txc.mcl) },
      { "port",eny.encrypt(txc.gateway) },
                 { "state",eny.encrypt(txc.cmd) },
            { "time",eny.encrypt(txc.tx_time.ToString()) },
            */
            Encription eny = new Encription();

     
            try
            {
                data = eny.decrypt(data);
 

            }
            catch (Exception)
            {
                return null;
              //    case "Call Alarm":
             //   case "Reset":
            }
  
            try
            { 
       
                //ANICshare sh = getshare();
                //share
                CAlarm alarm = new CAlarm(); 
             ANICshare.trace_value(data);
                Debug.Print(data);
                int index = 0;
                string[] datas = data.Split(',');
 
                if (datas[5].Contains("Call Alarm") || datas[5].Contains("Reset") || datas[5].Contains("CallCord Alarm"))
                {
                    // Debug.Print("not contain");
                    // return null;
                    //            case "Call Alarm":
                    ////case "Reset":

                    //   string log_id = share.Get_mysql_database_MaxID("client_alarm_log2", "log_id"); ;

                    string[] values = new string[] {
                    datas[index++],datas[index++],datas[index++],datas[index++],datas[index++],
                     datas[index++],datas[index++],datas[index++],datas[index++],datas[index++],
                    datas[index++],datas[index++],datas[index++],datas[index++],
                   System.Environment.MachineName , share.get_current_time() };
                    //  SELECT* FROM azure.client_alarm_log;
                    //  log_id, mcl, device_id, gateway, time, created_by, created_datetime, valid

                    bool insert = share.bool_Ecxe_transaction_Sql(alarm.insert_alarm_msg(values));
                    update_alarm(values);
                    //share.bool_Ecxe_transaction_Sql(update_alarm(values));
                    // alarm.inser_alarm_state

                    if (insert)
                    {
                        alarm_update update = new alarm_update()
                        {

                            state = eny.encrypt("yes")


                        };

                        return update;

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
                    /*
                    if (!test.Equals(share.dencry_value(dbName)))
                    {
                        return null;
                    }
                    string state = conn.testmysql();
                    ANI_server ani = new ANI_server()
                    { server_state = state };
                    return ani;
                    */
                }
            catch (Exception)
            {

                return null;
            }

        }
        private void update_alarm(string [] values)
        {
            CAlarm arm = new CAlarm();
            //     SELECT* FROM azure.client_alarm_device;
            //     device_id, serial_num, bed_id, location, valid

            DataSet ds =share.GetDataSource( arm.get_alarm_device_info(string.Format("and device.serial_num = '{0}' and device.valid='Y' ", values[3])));
            //   string device_id  =     share.Get_mysql_database_value("client_alarm_device", "device_id", values[4],"serial_num","valid = 'Y' ");
            if (ds.Tables[0].Rows.Count ==  0)
            {
                Debug.Print("no device");
                return;
            }
            string device_id = ds.Tables[0].Rows[0][0].ToString();
            string bed_id = ds.Tables[0].Rows[0][2].ToString();
            if (device_id == null)
            {
                Debug.Print("no device id");
                //return null;
               return ;
            }
            bool update = false;

            DataRow row = ds.Tables[0].Rows[0];
            string status = values[5];
            switch (status)
            {
                case "Call Alarm":
                    //    '100000', 'RECV', 'ALPINE0000002', 'Event', '01_01_23_45_01_81', 
                    //  'CallPoint_01', 'Call Alarm', 'CallPoint', '1', '01',
                    //  '01', '23', '45', '00', '01',
                    //   'DESKTOP-DAK2JTE', '2018-10-05 20:42:24', 'Y

                    //  SELECT* FROM azure.client_alarm;
                    //   alarm_id, bed_id, client_id, device_id, 
                    //   activate_device, activate_id, activate_datetime,
                    //settle_action_id, alarming, repeat, settle_device, settle_by,
                    //settle_datetime, modified_by, modified_datetime, created_by, created_datetime, valid
                    int index = 2;
                    string id = share.Get_mysql_database_MaxID("client_alarm", "alarm_id" );
                     
            //        sql.Append("SELECT device.device_id, device.serial_num, device.bed_id,bed.client_id ,  ");
            //        sql.Append("device.preset_action_id, device.preset_settle_id, device.location, ");

                    string[] new_values = new string []{ id,device_id,values[1], row[index++].ToString(),
                    row[index++].ToString(),row[index++].ToString(),values[14],values[15] };
                    update = share.bool_Ecxe_transaction_Sql( arm.update_alarm_msg(new_values, 0));
                    break;
                case "CallCord Alarm" :
                    //    '100000', 'RECV', 'ALPINE0000002', 'Event', '01_01_23_45_01_81', 
                    //  'CallPoint_01', 'Call Alarm', 'CallPoint', '1', '01',
                    //  '01', '23', '45', '00', '01',
                    //   'DESKTOP-DAK2JTE', '2018-10-05 20:42:24', 'Y

                    //  SELECT* FROM azure.client_alarm;
                    //   alarm_id, bed_id, client_id, device_id, 
                    //   activate_device, activate_id, activate_datetime,
                    //settle_action_id, alarming, repeat, settle_device, settle_by,
                    //settle_datetime, modified_by, modified_datetime, created_by, created_datetime, valid
                    index = 2;
                     id = share.Get_mysql_database_MaxID("client_alarm", "alarm_id");
                  
            //        sql.Append("SELECT device.device_id, device.serial_num, device.bed_id,bed.client_id ,  ");
            //        sql.Append("device.preset_action_id, device.preset_settle_id, device.location, ");

                     new_values = new string[]{ id,device_id,values[1], row[index++].ToString(),
                    row[index++].ToString(),row[index++].ToString(),values[14],values[15] };
                    update = share.bool_Ecxe_transaction_Sql(arm.update_alarm_msg(new_values, 0));
                    break;
                case "Reset":
                    DataSet alarm_ds = share.GetDataSource(arm.get_alarm_info(string.Format("and alarm.activate_device_id = '{0}' and  alarm.alarming= 'Y' ", device_id), "order by alarm.activate_datetime desc limit 1"));
                    if (alarm_ds.Tables[0].Rows.Count==0)
                    {
                        return;
                    }
                     index = 2;
                    string alarm_id = alarm_ds.Tables[0].Rows[0][0].ToString();
                    new_values = new string[]{ alarm_id,device_id,values[1], row[index++].ToString(),
                    row[index++].ToString(),row[5].ToString(),values[14],values[15] };
                    // arm.update_alarm_msg(values, 0);
                    update = share.bool_Ecxe_transaction_Sql(arm.update_alarm_msg(new_values, 1));
      
                    break;
  
            }

            if (update)
            {
                if (!bed_id.Equals("0"))
                {
                    inform_panel(string.Format("and (bp.bed_id = '{0}'  or  panel.page_index = 2  or panel.page_index = 3) ", bed_id));
                }
                else
                {
                    inform_panel();
                }
          
                //     worker = new Thread();
                //     Worker work = new Worker();
                //     Init(new Worker());
            }

        }

        private void inform_panel_client(string client_id ,string condition = "")
        {
            CPanel panel = new CPanel();
            //  DataSet nfc_ds = share.GetDataSource( panel.get_all_panel((string.Format("and page_index  = 2  ", ""))));
          //   (Cclient.get_bed_panel_brief(string.Format("and panel.display_id = {0} ", display_id));
            StringBuilder sql = new StringBuilder();
            sql.Append(panel.get_all_alarm_panel(string.Format("and (panel.page_index = 2  or panel.page_index = 3) ", "") + condition));
            sql.Append( Cclient.get_bed_panel(string.Format("and bed.client_id = {0} ", client_id) + condition) );


            DataSet nfc_ds = share.GetDataSource(sql.ToString());
            // get_all_panel
            //   Init(Work,nfc_ds);
            worker = new Thread(() => callback_Work(nfc_ds));
            worker.Start();


        }
        private void inform_panel(string condition = "") 
        {
            CPanel panel = new CPanel();
            //  DataSet nfc_ds = share.GetDataSource( panel.get_all_panel((string.Format("and page_index  = 2  ", ""))));
            DataSet nfc_ds = share.GetDataSource(panel.get_all_alarm_panel((string.Format("and panel.valid='Y' ", ""))+condition));
            // get_all_panel
            //   Init(Work,nfc_ds);
            worker = new Thread(() => Work(nfc_ds));
            worker.Start();
 

        }


        public delegate void Worker();
        private static Thread worker;

        public static void Init(Worker work,DataSet nfc_ds)
        {
            worker = new Thread(new ThreadStart(work));
            worker.Start();
        }
        public string call_display_update(string display_id, int command = 53)
        {
            display_id = share.dencry_value(display_id);
            CPanel panel = new CPanel();
            //      DataSet nfc_ds = share.GetDataSource(panel.get_all_panel((string.Format("and display_id  = '{0}'  ",display_id))));
            //DataSet nfc_ds = share.GetDataSource(panel.get_all_panel((string.Format("and display_id  = '{0}'  ", display_id))));
            DataSet nfc_ds = share.GetDataSource(panel.get_all_panel( "and autoupdate  = 'Y'  " ));
            // get_all_panel
            //   Init(Work,nfc_ds);
            worker = new Thread(() => call_update(nfc_ds,command));
            worker.Start();
            worker.Join();

            return ""; 
        }
        public static void Work(DataSet nfc_ds)
        {
            // Debug.Print(CCallBackThread);
            //   DataSet nfc_ds =share.GetDataSource(get_panel_ds((string.Format("and display_id in ({0})", oldCode)), 0));
            /*
            try
            {
                foreach (DataRow row in nfc_ds.Tables[0].Rows)
                {
                    Cshare.trace_value(row[1].ToString());
                    Cshare.trace_value("panel_ethernet_callback_panel");
                    int ip = cthd.IpStringToInt(row[1].ToString());
                    int port = int.Parse(row[3].ToString());
                    cthd.InsertCallBack(new  CallBackInfo(ip, port, new byte[] { 0x01 }, 60));
                    //     cthd.InsertCallCommand(new CallCommand(ip, port, new byte[] { 0x01 }, new byte[] { 0x01, (byte)(51 + cmbCmd.SelectedIndex) }));
                    cthd.InsertCallCommand(new CallCommand(ip, port, new byte[] { 0x01 }, new byte[] { 0x01, (byte)(51 ) }));
                    cthd.RunCallBack();


                }

            }
            catch {

            }
            */

            Debug.Print("CCallBackThread");
            CCallBackThread cthd = new CCallBackThread();
            int client = 100025;
            if (nfc_ds.Tables[0].Rows.Count > 0)
            {
             //   Cshare.trace_value(GetHostName(nfc_ds.Tables[0].Rows[0][1].ToString()));
            }

            List<string> ip_list = GetAllLocalMachines();
            List<string> cmd_ip_list = new List<string>();
            for (int i = 0; i < nfc_ds.Tables[0].Rows.Count; i++)
            {
                DataRow row = nfc_ds.Tables[0].Rows[i];
                if (row[1].ToString().Length>0)  
                {
                    if (!cmd_ip_list.Contains(row[1].ToString()))
                    {
                        int ip = cthd.IpStringToInt(row[1].ToString());
                        int port = int.Parse(row[3].ToString());
                        cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + i).ToString(), 10));
                        cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(52) }));
                        cmd_ip_list.Add(row[1].ToString());
                    }
                }
                // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
            }
            //cthd.msgTo = msgToFormEvent;
            cthd.RunCallBack();
        }
        public static void callback_Work(DataSet nfc_ds)
        {
            // Debug.Print(CCallBackThread);
            //   DataSet nfc_ds =share.GetDataSource(get_panel_ds((string.Format("and display_id in ({0})", oldCode)), 0));
            /*
            try
            {
                foreach (DataRow row in nfc_ds.Tables[0].Rows)
                {
                    Cshare.trace_value(row[1].ToString());
                    Cshare.trace_value("panel_ethernet_callback_panel");
                    int ip = cthd.IpStringToInt(row[1].ToString());
                    int port = int.Parse(row[3].ToString());
                    cthd.InsertCallBack(new  CallBackInfo(ip, port, new byte[] { 0x01 }, 60));
                    //     cthd.InsertCallCommand(new CallCommand(ip, port, new byte[] { 0x01 }, new byte[] { 0x01, (byte)(51 + cmbCmd.SelectedIndex) }));
                    cthd.InsertCallCommand(new CallCommand(ip, port, new byte[] { 0x01 }, new byte[] { 0x01, (byte)(51 ) }));
                    cthd.RunCallBack();


                }

            }
            catch {

            }
            */

            Debug.Print("CCallBackThread");
  
            int client = 100025;
            if (nfc_ds.Tables[0].Rows.Count > 0)
            {
                //   Cshare.trace_value(GetHostName(nfc_ds.Tables[0].Rows[0][1].ToString()));
            }
            int index = 1;
           // List<string> ip_list = GetAllLocalMachines();
            List<string> cmd_ip_list = new List<string>();

            List<string> cmd_port_list = new List<string>();



            for (int a= 0; a < nfc_ds.Tables.Count; a++)
            {


                for (int i = 0; i < nfc_ds.Tables[a].Rows.Count; i++)
                {
                    DataRow row = nfc_ds.Tables[a].Rows[i];
                    if (row[1].ToString().Length > 0)
                    {
                        if (!cmd_ip_list.Contains(row[1].ToString()))
                        {
   

                            cmd_ip_list.Add(row[1].ToString());
                            cmd_port_list.Add(row[3].ToString());
                        }
                    }
                    // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                    // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
                }
            }
            if (cmd_ip_list.Count>0)
            {
                CCallBackThread cthd = new CCallBackThread();
                for (int i = 0; i < cmd_ip_list.Count; i++)
                {
                    int ip = cthd.IpStringToInt(cmd_ip_list[i]);
                    int port = int.Parse(cmd_port_list[i]);
                    cthd.InsertCallBack(new CallBackInfo(cmd_ip_list[i], port, (client + index).ToString(), 10));
                    cthd.InsertExecCommand(new ExecCommand(cmd_ip_list[i], port, Encoding.ASCII.GetBytes((client + index++).ToString()), new byte[] { 0x01, (byte)(51) }));

                }
                cthd.RunCallBack();
          //      int ip = cthd.IpStringToInt(row[1].ToString());
         //       int port = int.Parse(row[3].ToString());
          ////      cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + index).ToString(), 10));
         //       cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + index++).ToString()), new byte[] { 0x01, (byte)(52) }));

            }
            //cthd.msgTo = msgToFormEvent;
   
        }
        public bool alarm_call_update ( DataSet nfc_ds,string device_id )
        {

 
            for (int i = 0; i < nfc_ds.Tables[0].Rows.Count; i++)
            {
                DataRow row = nfc_ds.Tables[0].Rows[i];
                try
                {
         
                    CClientSocket c = new CClientSocket();
                    //    if (comboBox1.SelectedIndex < 0) comboBox1.SelectedIndex = 0;
                    //    c.ButtonTag = comboBox1.Text;
                 //  bool EXIST = c.ShowBox  ;
                    c.ButtonTag = device_id;
                    c.serverInfo = new ServerInfo(row[0].ToString(), int.Parse(row[1].ToString()), "FibroAni", "868413");
                     bool exist = c.StartConnect();
                    if (exist)
                    {
                        return exist;
                    }
             

                }
                catch (Exception)
                {
                //    return false;
                  //  throw;
                }


              

                // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
            }
            return false;
            //cthd.msgTo = msgToFormEvent;



        }
        private bool ShowMessage(Boolean bol)
        {
            return bol;
        }
        public void call_update(DataSet nfc_ds,int command=53)
        {
 

            Debug.Print("CCallBackThread");
            CCallBackThread cthd = new CCallBackThread();
            int client = 100025;
            if (nfc_ds.Tables[0].Rows.Count > 0)
            {
                //   Cshare.trace_value(GetHostName(nfc_ds.Tables[0].Rows[0][1].ToString()));
            }

            List<string> ip_list = GetAllLocalMachines();
            for (int i = 0; i < nfc_ds.Tables[0].Rows.Count; i++)
            {
                DataRow row = nfc_ds.Tables[0].Rows[i];
                if (row[1].ToString().Length>0)
                {
                    int ip = cthd.IpStringToInt(row[1].ToString());
                    int port = int.Parse(row[3].ToString());
                    cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + i).ToString(), 10));
                    cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(command) }));

                }
                // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
            }
            //cthd.msgTo = msgToFormEvent;
            cthd.RunCallBack();


        }





        public static List<string> GetAllLocalMachines()
        {
            List<string> ip_list = new List<string>();
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine("arp -a");
            p.StandardInput.WriteLine("exit");
            StreamReader reader = p.StandardOutput;
            try
            {

                string IPHead = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString().Substring(0, 3);
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    line = line.Trim();
                    Debug.Print(line);
                    if (line.StartsWith(IPHead))
                    {
                        string IP = line.Substring(0, 15).Trim();
                        string Mac = line.Substring(line.IndexOf("-") - 2, 0x11).Trim();
                        ip_list.Add(IP);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                return ip_list;


            }
            return ip_list;

        }

        public iot_ble_device_info get_iot_ble_device_info(string device_id) // Raymond @20210209 ble device
        {
            device_id = share.dencry_value(device_id);



            StringBuilder sql = new StringBuilder();
            sql.Append(Cmedical.get_iot_ble_device_info(device_id));


            DataSet ds = share.GetDataSource(sql.ToString());
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow row = ds.Tables[0].Rows[i];

                        iot_ble_device_info device = new iot_ble_device_info();


                        device.device_name = share.encry_value(row["device_name_chi"].ToString());
                        device.type_id = share.encry_value(row["type_id"].ToString());
                        device.type_name = share.encry_value(row["type_name_chi"].ToString());
                        device.device_mac_address = share.encry_value(row["device_mac_address"].ToString());
                        device.supplier_id = share.encry_value(row["supplier_id"].ToString());
                        device.supplier_name = share.encry_value(row["supplier_name_chi"].ToString());

                        return device;
                    }
                }
            }
            return null;

        }

        //Raymond @20210209 
        public class iot_ble_device_info
        {
            public string device_name;
            public string type_id;
            public string type_name;
            public string device_mac_address;
            public string supplier_id;
            public string supplier_name;
        }

        public class client_id_in_nfc
        {
            public string label_usage_code;
            public string label_usage_id;
            public string client_id;
            public string message;
        }
        public class insert_client_photo
        {
            public string client_photo_update;
        }


        public class Acc_charge_item
        {
            public string item_name;
            public string is_zero;
        }
        public class Acc_upload_item
        {
            public List<string> item_charge_date = new List<string>();
            public List<string> item_name = new List<string>();
            public List<string> item_quantity = new List<string>();
        }
        public class insert_acc_data
        {
            public string result;
        }



        public class insert_vital_data
        {
            public string is_update;
        }

        public class post_record
        {
            public string is_update;
        }
        public class post_observation_record
        {
            public string observation_id;
            public string is_update;
            public string nextdate;
        }
        public class blood_pressure_records
        {
 
            public List<string> systolic = new List<string>();
            public List<string> diastolic = new List<string>();
            public List<string> pulse = new List<string>();
            public List<string> handlingperson = new List<string>();
            public List<string> timestamp = new List<string>();
 
        }

        public class temperature_records
        {
            public List<string> body_temperature = new List<string>();
            public List<string> handlingperson = new List<string>();
            public List<string> timestamp = new List<string>();

        }

        public class blood_glucose_records
        {

            public List<string> blood_glucose = new List<string>();
            public List<string> handlingperson = new List<string>();
            public List<string> timestamp = new List<string>();

        }
        public class respiration_rate_records
        {




            public List<string> respiration_rate = new List<string>();
            public List<string> handlingperson = new List<string>();
            public List<string> timestamp = new List<string>();

        }
        public class blood_oxygen_records
        {
            public List<string> blood_oxygen = new List<string>();
            public List<string> handlingperson = new List<string>();
            public List<string> timestamp = new List<string>();

        }

        public class body_weight_records
        {

            public List<string> body_weight = new List<string>();
            public List<string> handlingperson = new List<string>();
            public List<string> timestamp = new List<string>();
 
        }

        public class medical_revisit_records
        {
            public List<string> revisit_record = new List<string>();
        }
        public class medicine_description
        {

            public string take_begin_date;
            public string medicine_source;
            public string specialty_code;
            public string medicine_PRN;
            public string script_medicine;
            public string need_distribute;

            public string first_check_person = "";
            public string first_check_time = "";
            public string second_check_person = "";
            public string second_check_time = "";


        }
        public class wound_reocrds
        {
            public List<string> care_wound_id = new List<string>();
            public List<string> wound_position_direction = new List<string>();
            public List<string> wound_position_id = new List<string>();
            public List<string> wound_position_discover = new List<string>();
            public List<string> wound_last_state = new List<string>();
        }

        public class medicine_bag_image
        {
            public string medicine_bag_id;
            public string image_data;
        }
        public class medicine_photo
        {
            public string medicine_photo_id;
            public string image_data;
        }
        public class medicine_box_content
        {
            public string take_medicine_id_arr;
        }
        public class medicine_records
        {
            public List<string> medicine_name = new List<string>();
            public List<string> medicine_id = new List<string>();
            public List<string> take_medicine_id = new List<string>();
            //public string medicinename;


            public List<string> med_volume_amount = new List<string>();
            public List<string> med_volume_unit = new List<string>();
            public List<string> medicine_period = new List<string>();
            public List<string> each_take_type = new List<string>();
            public List<string> medicine_report_type_id = new List<string>();


            public List<string> medicine_taking_method = new List<string>();
            public List<string> medicine_taking_remark = new List<string>();
            public List<string> medicine_photo_id = new List<string>();
            public List<string> medicine_bag_id = new List<string>();
            public List<string> medicine_take_interval = new List<string>();

            public List<string> handling_person = new List<string>();
            public List<string> time_stamp = new List<string>();
            public List<string> taken_state = new List<string>();

            public List<string> first_check_id = new List<string>();
            public List<string> second_check_id = new List<string>();
        }
        public class take_medicine_records
        {
            public take_medicine[] takes;
        }
        public class take_medicine
        {
            public string medicine_name;
            public string medicine_id;
            public string take_medicine_id;
            public string med_volume_amount;
            public string med_volume_unit;
            public string medicine_period;
            public string each_take_type;
            public string medicine_report_type_id;
            public string medicine_report_type;
            public string medicine_taking_method;
            public string medicine_taking_remark;
            public string medicine_photo_id;
            public string medicine_bag_id;
            public string medicine_take_interval;
            public string medicine_spe_code;

            public string[] description;
            public string medicine_source;

            public string medicine_refill_date;

            public string prn;

            public string handling_person;
            public string time_stamp;

            public string taken_state;

            public string first_check_id;
            public string second_check_id;
        }


        public class search_client_records
        {
            public List<string> client_id_list = new List<string>();
            public List<string> chi_name_list = new List<string>();
            public List<string> eng_lastname_list = new List<string>();
            public List<string> eng_firstname_list = new List<string>();
            public List<string> sex_list = new List<string>();
            public List<string> bedloc_list = new List<string>();
            public List<string> birth_date_list = new List<string>();
            public List<string> picture_id_list = new List<string>();
            public List<string> client_state_list = new List<string>();
        }

        // Raymond  20200909
        public class search_client_records_2
        {

            public List<search_client_item> search_client_item_list = new List<search_client_item>();

        }

        public class search_client_item
        {
            public string client_id = "";
            public string chi_name = "";
            public string eng_lastname = "";
            public string eng_firstname = "";
            public string sex = "";
            public string bedloc = "";
            public string birth_date = "";
            public string picture_id = "";
            public List<String> client_state_lists = new List<string>();
        }

        public class pictureinfo
        {
            public int id;
            public string pictureid;
            public string picturedata;

        }



        public class client_briefing
        {

            public string client_id;
            public string name;
            public string sex;
            public string age;
            public string birth_date;
            public string bednum;

            public string remark;
            public string picture_id;

            public string meal_type;
            public string medical_briefing = "";
            public string personal_ability = "";
            public string incontinence = "";

            public string wound_exist = "";

            //public string absence = "";
            public List<string> absence_list = new List<string>();
            public List<string> account_items = new List<string>();
            public string assessment_result;
            public string drug_ADR = "";
            public string drug_allergic = "";
            public string other_allergic = "";
            public string disease = "";

            public string absence = "";

        }
        public class client_briefing2
        {

            public string client_id;
            public string name;
            public string sex;
            public string age;
            public string birth_date;
            public string bednum;
            public string zone;
            public string block;
            public string remark;
            public string picture_id;

 

            public string absence = "";

            public  client_content[] contents;  
        }
        public class client_content
        {
            public string title = "";
            public client_detail[] details;
        }
        public class client_detail
        {
            public string title = "";
            public string description = "";
            public string status = "";
            public string code = "";
        }
        public class client_account_briefing
        {
            public List<string> account_items = new List<string>();
        }
        public class client_account_detail
        {
            public string title = "";
            public string description = "";
            public string status = "";
            public string code = "";
        }

        public class client_account_detail2
        {
            public string id = "";
            public string title = "";
            public string description = "";
            public string status = "";
            public string code = "";
            public string record_date = "";
            public string record_qty = "";
            public string created_by = "";
            public string created_datetime = "";
        }
        public class client_account_briefing2
        {

            public client_account_detail[] account_items;

        }
        public class client_account_briefing3
        {

            public client_account_detail2[] account_items;

        }
        public class contact_person
        {
            public string contact_name;
            public string client_relation;
            public string contact_num;
        }

        public class measure_briefing
        {

            public string blood_pressure;
            public string blood_oxygen;
            public string blood_glucose;
            public string respiration_rate;
            public string body_temperature;
            public string body_weight;
            public List<string> blood_pressure_list = new List<string>();
            public List<string> blood_oxygen_list = new List<string>();
            public List<string> blood_glucose_list = new List<string>();
            public List<string> respiration_rate_list = new List<string>();
            public List<string> temperature_list = new List<string>();
            public List<string> weight_list = new List<string>();



        }


        public class dash_board
        {
            public dash_board_event[] events;

        }
        public class dash_board_event
        {
            public string event_name;
            public string accessible;
            public client_info[] clients;
        }
        public class medical_events
        {
            public medical_event[] events;
        }

        public class medical_event
        {
            public string event_name;
            public medical_content[] medical_content_list;
  
        }
        public class medical_content
        {
            public string title;
            public string description;
            public string status;
            public string[] content;
        }
        public class wound_events
        {
            public wound_event[] events;
            public wound_wash_parameter wash;
        }

        //Raymond @20201015
        public class wound_events_2
        {
            public wound_event[] events;
            public wound_wash_parameter_2 wash;
        }

        public class wound_event
        {
            public string care_wound_id;
            public string wound_position_remark; // Raymond @20201216
            public string wound_position;
            public string wound_position_id;
            public string wound_positionx;
            public string wound_positiony;
            public string wound_position_direction;
            public string wound_position_name;
            public string start_date;
            public string next_date;
            public string frequency;
            public string clean_days;
            public string clean_times;
            public string count;
            public string last_ids;
   
            public wound_dressing[] dressing;
            public wound_last_content record;
        }
        public class wound_last_content
        {
            public string dimens;
            public string level;
            public string color;
            public string fuild_quanity;
            public string fuild_type;
            public string smell;

            public string last_photo_id;
        }
        public class wound_wash_parameter
        {
            public string lengthmax;
            public string widthmax;
            public string depthmax;
            public wound_parameter level;
            public wound_parameter color;
            public wound_parameter fuild_quanity;
            public wound_parameter fuild_type;
            public wound_parameter smell;
  
        }
        //Raymond @20201015
        public class wound_wash_parameter_2
        {
            public string lengthmax;
            public string widthmax;
            public string depthmax;
            public List<wound_parameter_2> level;
            public List<wound_parameter_2> color;
            public List<wound_parameter_2> fuild_quanity;
            public List<wound_parameter_2> fuild_type;
            public List<wound_parameter_2> smell;

        }

        public class wound_dressing
        {
            public string id;
            public string name;
            public string remark;
        }
        public class wound_wash_event
        {
            public wound_wash []events;
 
        }
        public class wound_wash
        {
            public string observation_id;
            public string wash_date;
            public string length;
            public string width;
            public string depth;
            public string level;
            public string color;
            public string fuild_quanity;
            public string fuild_type;
            public string smell;
            public string remark;
            public string frequency;
            public wound_dressing[] dressing;
            public string created_by;
            public string created_datetime;
            public string modified_by;
            public string modified_datetime;
            public string wound_photo_id;
     
        }
        public class wound_parameter
        {
            public string []id;
            public string []name;

        }

        //Raymond @20201015
        public class wound_parameter_2
        {
            public string id;
            public string name;
            public string description;
        }

        public class client_info
        {
            public string client_id;
            public string client_picture_id;
            public string client_name;
            public string sex;
            public string date;
            public string [] contents;


        }
        public class restrict_details
        {
            public string restrict_item;
            public string restrict_session;
            public string restrict_session_daily;
        }
        public class care_briefing
        {
            public restrict_details[] resfsda;
            public string restrict_record;
            public string body_turning;
            public string water_control;
            public string defecation;
        }

        public class alarm_update
        {
            public string state;
        }
        public class company_department
        {
            public string department_id;
            public string department_name;
        }
  
        public class client_log_setting 
        {
            public company_department[] departments;
            public client_briefing2[] clients;
        }
        public class client_log
        {
            public string log_id;

            public string client_id;
            public string client_name;
            public string department_id;
            public string department_name;

            public string title;
            public string event_datetime;
            public string content;
            public string create_by;
            public string create_datetime;
            public string isimportant;
            public string lastupdate_time;
            public string reply_count;
            public client_log_reply[] replys;
        }
        public class client_log_reply
        {
            public string reply_id;

            public string reply_ordering;
            public string content;
            public string create_by;
            public string create_datetime;


        }
        public class client_log_update
        {
            public string state;
        }
    }
}
