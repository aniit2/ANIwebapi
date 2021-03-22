
 
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Hosting;

namespace ANI.code
{

    public class ANI_Display_panel
    {
        CPanel panel = new CPanel();
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
        public bool panel_exist = false;
        public ANI_Display_panel(bool encrytion)
        {
           
            share.encrytion = encrytion;
     
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
            panel_exist = true;
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


        public class initial_display  
        {
            public string display_id;
            public string orientation;
            public string page_index;
            public string usage_id;
            public string reverse;
            public string logname;
            public string shown_alarm;
        }

        public class Call_Alarms
        {
          public  Call_Alarm[] alarms;
           public String server_time;
        }


        public class Call_Alarm
        {
            public String client_id;
            public String client_name;
            public String client_picture_id;
            public String bed_name;
            public String bed_id;
            public String location;
            public String timestamp;
            public String description;
            public String auto_cancel;

        }
        public class ANI_server
        {
            public string server_state;

        }
        public ANI_server get_server_state(string test)
        {

            if (!dbName.Equals(share.dencry_value(test)))
            {
                return null;
            }
            string state = share.testmysql();
            ANI_server ani = new ANI_server()
            { server_state = share.encry_value(state) };
            return ani;
        }



        public initial_display get_display_info(string serial_num, string location, string location_long, string listen_port,string address,string vers)
        {
            try
            {


                serial_num = share.dencry_value(serial_num);
                location = share.dencry_value(location);
                location_long = share.dencry_value(location_long);
                listen_port = share.dencry_value(listen_port);
                address = share.dencry_value(address);
                vers = share.dencry_value(vers);
                string time = share.get_current_time();
 
                
                DataSet ds = share.GetDataSource(Clogin.display_getlogin(serial_num));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    string[] values = new string[] { serial_num, location, location_long, listen_port,vers, time };
                    if (row[5].ToString() == "N")
                    {
                        return null;
                    }

                    else if (location!= row[1].ToString())
                    {
                        share.bool_Ecxe_transaction_Sql(Clogin.display_update_location(values));
                    }
                    else
                    {
                        share.bool_Ecxe_transaction_Sql(Clogin.display_update_time(values));
                    }
                    initial_display dis = new initial_display
                    {
                        display_id = share.encry_value(row[0].ToString()),
                        page_index =share.encry_value( row[6].ToString())
                        ,usage_id = share.encry_value(row[7].ToString()),
                        orientation = share.encry_value(row[8].ToString())
                        ,
                        shown_alarm = share.encry_value(row[9].ToString())
                        ,
                        logname = share.encry_value(LoginName)
    };
                    return dis;
 
                }
                else
                {

                    String id =  share.Get_mysql_database_MaxID("sys_display_panel", "display_id");
                    string[] values = new string[] { id,location, location_long, listen_port, serial_num,vers,"Y",time,time,address };
 
                    share.bool_Ecxe_transaction_Sql(Clogin.display_add_panel(values));
                    return null;
                    
                }
                //KillSleepingConnections(2000);
        
  
            }
            catch (Exception ex)
            {
                return null;
            }
            catch
            {
                return null;
            }
            
        }
        public Call_Alarms get_alarm_info(string serial_num, string location, string location_long, string listen_port, string address)
        {
            try
            {


                serial_num = share.dencry_value(serial_num);
                location = share.dencry_value(location);
                location_long = share.dencry_value(location_long);
                listen_port = share.dencry_value(listen_port);
                address = share.dencry_value(address);

                string time = share.get_current_time();


                DataSet ds = share.GetDataSource(Clogin.display_alarm(serial_num));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    string[] values = new string[] { serial_num, location, location_long, listen_port, time };
                    if (row[5].ToString() == "N")
                    {
                        return null;
                    }
                    else if(row[5].ToString() == "Y")
                    {
                        List<Call_Alarm> alarm_list = new List<Call_Alarm>();
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
        
                            Call_Alarm alarm = new Call_Alarm();
                            DataRow alarm_row = ds.Tables[1].Rows[i];
                            int index = 0;
                            alarm.client_id = share.encry_value( alarm_row[index++].ToString());
                            alarm.client_name = share.encry_value(alarm_row[index++].ToString());
                            alarm.bed_id = share.encry_value(alarm_row[index++].ToString());
                            alarm.bed_name=alarm.location = share.encry_value(alarm_row[index++].ToString());

         

                            alarm.client_picture_id = share.encry_value(picture_id_check( alarm_row[index++].ToString(),""));


                            alarm.timestamp = share.encry_value(alarm_row[index++].ToString());
                            alarm.description = share.encry_value(alarm_row[index++].ToString());

                            alarm_list.Add(alarm);
                        }
                        if (alarm_list.Count>0)
                        {
                            Call_Alarms dis = new Call_Alarms
                            {
                                alarms = alarm_list.ToArray()
                               , server_time = share.encry_value(share.get_current_time())
                                //   display_id = share.encry_value(row[0].ToString()),
                                //   page_index = share.encry_value(row[6].ToString())
                                //   ,
                                //    usage_id = share.encry_value(row[7].ToString()),
                                //    orientation = share.encry_value(row[8].ToString())
                                //    ,
                                //     logname = share.encry_value(LoginName)
                            };
                            return dis;

                        }
                        return null;
                    }


                    return null;
                }
                else
                {

                //    String id = share.Get_mysql_database_MaxID("sys_display_panel", "display_id");
                ///    string[] values = new string[] { id, location, location_long, listen_port, serial_num, "Y", time, time, address };

                //    share.bool_Ecxe_transaction_Sql(Clogin.display_add_panel(values));
                    return null;

                }
                //KillSleepingConnections(2000);


            }
            catch
            {
                return null;
            }

        }
        #region
        //public client_panel_briefing get_client_display_info(string display_id)
        //{
        //    string milk = "";
        //    display_id = share.dencry_value(display_id);
        //    List<panel_block> blk = new List<panel_block>();

        //    List<string> icons = new List<string>();

        //    string responiblestr = "";
        //    string dinging = "";
        //    string namestr = "";
        //    string sexstr = "";

        //    string birth_datestr = "";

        //    string precautionstr = "";
        //    string liftstr = "";
        //    string toolsstr = "";

        //    string remarkstr = "";
        //    string personal_ability_str = "";
        //    string bedloc = "";
        //    string absence_str = "";
        //    string wound_exist_str = "";

        //    string meal_Type_str = "";
        //    string memo_str = "";
        //    string pictureid = "";

        //    string diaper = "";

        //    string restraint = "";

        //    List<string> account_list = new List<string>();

        //    DateTime bday = new DateTime();
        //    StringBuilder sql = new StringBuilder();
        //    sql.Append(Cclient.get_bed_panel_brief(string.Format("and panel.display_id = {0} ",display_id)));



        //    DataSet setting_ds = share.GetDataSource(sql.ToString());


        //    if (setting_ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow firstrow = setting_ds.Tables[0].Rows[0];
        //        bedloc = firstrow[2].ToString();



        //        string client_id = firstrow[3].ToString();
        //        string bed_id = firstrow[1].ToString();
        //        if (bed_id.Equals("0"))
        //        {

        //            client_panel_briefing empty_brief = new client_panel_briefing
        //            {
        //                bednum = share.encry_value("未設定"),
        //                night_mode = share.encry_value(firstrow[6].ToString()),
        //                night_start = share.encry_value(firstrow[7].ToString()),
        //                night_end = share.encry_value(firstrow[8].ToString()),
        //                bed_id = share.encry_value(firstrow[1].ToString())
        //               ,picture_id = share.encry_value("D")
        //                //picturename = picturenamestr
        //            };
        //            return empty_brief;
        //        }
        //        if (client_id.Equals("0"))
        //        {

        //            client_panel_briefing empty_brief = new client_panel_briefing
        //            {
        //                bednum = share.encry_value(bedloc),
        //                bed_id = share.encry_value(firstrow[1].ToString()),
        //                night_mode = share.encry_value(firstrow[6].ToString()),
        //                night_start = share.encry_value(firstrow[7].ToString()),
        //                night_end = share.encry_value(firstrow[8].ToString()),
        //                picture_id = share.encry_value("D")
        //                //picturename = picturenamestr


        //            };
        //            return empty_brief;
        //        }

        //        else
        //        {

        //            sql = new StringBuilder();
        //            sql.Append(Cclient.get_client_briefing2(client_id));
        //            sql.Append(Cclient.get_client_diet(client_id,string.Format("and dt.channel = 1  group by  dt.diet_type_id ")));
        //            sql.Append(Cclient.get_client_diet(client_id, string.Format("and dt.channel = 2 group by  dt.diet_type_id  ")));
        //            sql.Append(Cmedical.get_ae_brief("and ae.client_id = " + client_id + " and ae.ae_status = '留醫中' "));
        //            sql.Append(Cmedical.get_revisit_brief("and rev.client_id = " + client_id + " and rev.revisit_status = '預約中' ", false));
        //            sql.Append(Cmedical.get_event_grid_table("and ( rev.client_id = " + client_id + " and rev.event_status = '預約中' )",
        //                false, "FIELD(rev.event_status, '預約中') desc, "));

        //            DataSet ds = share.GetDataSource(sql.ToString());

        //            int i = 0;
        //            DataRow row = ds.Tables[0].Rows[0];
        //            namestr = row[i++].ToString();
        //            sexstr = row[i++].ToString().Equals("M") ? "男" : "女";
        //            bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);
        //            birth_datestr = bday.ToString("yyyy/MM/dd");
        //            string ageint = row[i++].ToString();//age
        //            bedloc = row[i++].ToString();
        //            i++;//assessment_result
        //            dinging = row[i++].ToString(); //dining_name
        //            meal_Type_str = row[i++].ToString(); //dining_name
        //            List<string> ability_list = new List<string>();
        //            toolsstr = row[i].ToString();//tool help
        //            ability_list.Add(row[i++].ToString());//tool help
        //            ability_list.Add(row[i++].ToString());//self_care_ability
        //            ability_list.Add(row[i++].ToString());//eating_ability
        //            ability_list.Add(row[i++].ToString());//cognitive_ability
        //            ability_list.Add(row[i++].ToString());//communicate_ability
        //            ability_list.Add(row[i++].ToString());//visual_ability
        //            ability_list.Add(row[i++].ToString());//audio_ability
        //            ability_list.Add(row[i++].ToString());//incontinence
        //            ability_list.Remove("");
        //            personal_ability_str = string.Join(";",  ability_list);
        //            diaper = row[i++].ToString();//添寧 (大碼) 包月
        //            //wound_exist_str = row[i++].ToString().Length > 0 ? "Y" : "N";

        //            absence_str = row[i++].ToString();//ae status
        //            i++;//teeth


        //            pictureid = row[i++].ToString();//client_photo_id
        //            if (pictureid.Length==0||pictureid=="0")
        //            {
        //                pictureid = sexstr;
        //            }
        //            liftstr = row[i++].ToString();//1人
        //            memo_str = row[i++].ToString();//沖浴椅
        //            restraint = row[i++].ToString();//restraint item
        //            i++;//turning position
        //            precautionstr = row[i++].ToString();
        //            if (precautionstr == "Y")
        //            {
        //                icons.Add("CT");
        //            }
        //            string oxgyen = row[i++].ToString();
        //            if (precautionstr == "Y")
        //            {
        //                icons.Add("OO");
        //            }

        //            //responiblestr =  row[i++].ToString();
        //            if (responiblestr.Length>0)
        //            {
        //                responiblestr = string.Format("負責護士 : {0}",responiblestr) ;
        //            }

        //            //for (int a = i; a < i+3; a++)
        //            //{
        //            //    string str = row[a].ToString();
        //            //    if (str.Length>0)
        //            //    {
        //            //        icons.Add(str);
        //            //    }

        //            //}
        //            string caution_str = "";
        //            caution_str = string.Join(";", icons);


        //            string meal_first = "";

        //            if (dinging.Length>0)
        //            {
        //                meal_first = string.Format("用餐 : {0}", dinging);
        //            }



        //            string meal_second = "";

        //            string assessment_str = "";

        //            string drug_allergic_str = "";
        //            string adr_str = "";
        //            string other_allergic_str = "";
        //            string disease_str = "";
        //            string revisit_str = "";
        //            if (ds.Tables[1].Rows.Count != 0)
        //            {
        //                drug_allergic_str = ds.Tables[1].Rows[0][7].ToString();
        //                adr_str = ds.Tables[1].Rows[0][8].ToString();
        //                other_allergic_str = ds.Tables[1].Rows[0][9].ToString() + (ds.Tables[1].Rows[0][10].ToString().Length > 0 ? " " + ds.Tables[1].Rows[0][10].ToString() : "");

        //                disease_str = ANICshare.add_comma(ds.Tables[1].Rows[0][4].ToString()) + ANICshare.add_comma(ds.Tables[1].Rows[0][5].ToString())
        //                    + ds.Tables[1].Rows[0][6].ToString();

        //                assessment_str = ds.Tables[1].Rows[0][11].ToString();
        //            }


        //            if (ds.Tables[2].Rows.Count != 0)
        //            {
        //                StringBuilder stringBuilder = new StringBuilder();
        //                if (meal_first.Length>0)
        //                {
        //                    meal_first = meal_first + "\n";
        //                }
        //                for (int a = 0; a < ds.Tables[2].Rows.Count; a++)
        //                {
        //                    stringBuilder.Append(string.Format("{0} : {1}", ds.Tables[2].Rows[a][2].ToString() , ds.Tables[2].Rows[a][3].ToString()));
        //                    if (a != ds.Tables[2].Rows.Count-1)
        //                    {
        //                        stringBuilder.Append("\n");
        //                    }
        //                    //StringBuilder stringBuilder = new StringBuilder();
        //                }
        //                meal_first = meal_first+ stringBuilder.ToString();


        //            }

        //            if (ds.Tables[3].Rows.Count != 0)
        //            {
        //                StringBuilder stringBuilder = new StringBuilder();
        //                for (int a = 0; a < ds.Tables[3].Rows.Count; a++)
        //                {
        //                    stringBuilder.Append(string.Format("{0} : {1}", ds.Tables[3].Rows[a][2].ToString(), ds.Tables[3].Rows[a][3].ToString()));
        //                    if (a != ds.Tables[3].Rows.Count - 1)
        //                    {
        //                        stringBuilder.Append("\n");
        //                    }
        //                    //StringBuilder stringBuilder = new StringBuilder();
        //                }
        //                meal_second = stringBuilder.ToString();


        //            }




        //            if (ds.Tables[4].Rows.Count != 0)
        //            {
        //                namestr = namestr + "- 急症";



        //            }
        //            if (ds.Tables[5].Rows.Count != 0)
        //            {
        //                /*
        //                List<string> revisit_event = new List<string>();
        //                foreach (DataRow item in ds.Tables[3].Rows)
        //                {
        //                    string str = item[6].ToString() + item[7] + "," + item[3].ToString();
        //                    revisit_event.Add(str);
        //                }

        //                revisit_str = string.Join(";", revisit_event.ToArray());
        //                */
        //            }
        //            string headersize = "24";
        //            /*
        //            string[] meal = new string[] { "", "", "飲食1", headersize, Color.Blue.ToArgb().ToString(), meal_first, "", Color.Black.ToArgb().ToString(), "40", "15", "Y", "N", "0", "N" };
        //            string[] restrict = new string[] { "", "", "飲食2", headersize, Color.Blue.ToArgb().ToString(), meal_second, "", Color.Black.ToArgb().ToString(), "40", "15", "Y", "N", "0", "N" };
        //            string[] remark_block = new string[] { "", "", "備忘錄", headersize, Color.Blue.ToArgb().ToString(), memo_str, "", Color.Black.ToArgb().ToString(), "40", "15", "Y", "N", "0", "Y" };
        //            string[] mike_block = new string[] { "", "", "約束物品", headersize, Color.Blue.ToArgb().ToString(), restraint, "", Color.Black.ToArgb().ToString(), "60", "15", "Y", "N", "0", "Y" };
        //            string[] assist_block = new string[] { "", "", "輔助工具", headersize, Color.Blue.ToArgb().ToString(), toolsstr, "", Color.Black.ToArgb().ToString(), "60", "15", "Y", "N", "0", "Y" };


        //            string[] revisit = new string[] { "", "", "扶抱", headersize, Color.Blue.ToArgb().ToString(), liftstr, "", Color.Black.ToArgb().ToString(), "60", "15", "N", "N", "0", "Y" };



        //            string[] medical_event = new string[] { "", "", "紙尿片", headersize, Color.Blue.ToArgb().ToString(), diaper, "", Color.Black.ToArgb().ToString(), "60", "15", "N", "N", "0", "Y" };
        //            */
        //            string[] meal = new string[] { "", "", "飲食1", headersize, Color.Blue.ToArgb().ToString(), meal_first, "", Color.Black.ToArgb().ToString(), resize_font(meal_first,false) , "15", "Y", "N", "0", "N" };
        //            string[] restrict = new string[] { "", "", "飲食2", headersize, Color.Blue.ToArgb().ToString(), meal_second, "", Color.Black.ToArgb().ToString(), resize_font(meal_second, false) , "15", "Y", "N", "0", "N" };
        //            string[] remark_block = new string[] { "", "", "備忘錄", headersize, Color.Blue.ToArgb().ToString(), memo_str, "", Color.Black.ToArgb().ToString(), resize_font(memo_str, false) , "15", "Y", "N", "0", "Y" };
        //            string[] mike_block = new string[] { "", "", "約束物品", headersize, Color.Blue.ToArgb().ToString(), restraint, "", Color.Black.ToArgb().ToString(), resize_font(restraint, false), "15", "Y", "N", "0", "Y" };
        //            string[] assist_block = new string[] { "", "", "輔助工具", headersize, Color.Blue.ToArgb().ToString(), toolsstr, "", Color.Black.ToArgb().ToString(), resize_font(toolsstr, false) , "15", "Y", "N", "0", "Y" };


        //            string[] revisit = new string[] { "", "", "扶抱", headersize, Color.Blue.ToArgb().ToString(), liftstr, "", Color.Black.ToArgb().ToString(), resize_font(liftstr, true) , "15", "N", "N", "0", "Y" };



        //            string[] medical_event = new string[] { "", "", "紙尿片", headersize, Color.Blue.ToArgb().ToString(), diaper, "", Color.Black.ToArgb().ToString(), resize_font(diaper, true), "15", "N", "N", "0", "Y" };








        //            meal = share.encry_value(meal);
        //            restrict = share.encry_value(restrict);

        //            remark_block = share.encry_value(remark_block);
        //            mike_block = share.encry_value(mike_block);

        //            assist_block = share.encry_value(assist_block);

        //            revisit = share.encry_value(revisit);
        //            medical_event = share.encry_value(medical_event);

        //            //飲食 約束衣使用 備忘錄 凝固粉 跌倒風險 紙尿片 覆診 醫療項目
        //            blk.Add(get_blocks(meal));

        //            blk.Add(get_blocks(restrict));

        //            blk.Add(get_blocks(remark_block));
        //            blk.Add(get_blocks(assist_block));
        //            blk.Add(get_blocks(mike_block));


        //            blk.Add(get_blocks(revisit));
        //            blk.Add(get_blocks(medical_event));




        //            client_panel_briefing brief = new client_panel_briefing
        //            {

        //                night_mode = share.encry_value(firstrow[6].ToString()),
        //                night_start = share.encry_value(firstrow[7].ToString()),
        //                night_end = share.encry_value(firstrow[8].ToString()),



        //                name = share.encry_value(namestr),
        //                sex = share.encry_value(sexstr),
        //                age = share.encry_value(ageint.ToString()),
        //                bednum = share.encry_value(bedloc),
        //                //relative = relative_name_str,
        //                remark = share.encry_value(remarkstr),
        //                client_id = share.encry_value(client_id),
        //                bed_id = share.encry_value(bed_id),

        //                caution = share.encry_value(caution_str),
        //                //picturename = picturenamestr
        //                picture_id = share.encry_value(pictureid),
        //                personal_ability = share.encry_value(personal_ability_str),
        //                birth_date = share.encry_value(birth_datestr),
        //                wound_exist = share.encry_value(wound_exist_str),

        //                meal_type = share.encry_value(meal_Type_str),

        //                responsible = share.encry_value(responiblestr),
        //                assessment_result = share.encry_value(assessment_str),
        //                drug_ADR = share.encry_value(adr_str),
        //                drug_allergic = share.encry_value(drug_allergic_str),
        //                other_allergic = share.encry_value(other_allergic_str),
        //                disease = share.encry_value(disease_str),
        //                absence = share.encry_value(absence_str),
        //                account_items = share.encry_value(account_list),
        //                blocks = blk.ToArray()



        //            };

        //            return brief;
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        #endregion
        public client_panel_briefing get_client_display_info_old(string display_id)
        {
            string milk = "";
            display_id = share.dencry_value(display_id);
            List<panel_block> blk = new List<panel_block>();

            List<string> icons = new List<string>();

            string responiblestr = "";
            string dinging = "";
            string namestr = "";
            string sexstr = "";
            string age = "";
            string birth_datestr = "";

            string precautionstr = "";
            string liftstr = "";
            string toolsstr = "";

            string remarkstr = "";
            string personal_ability_str = "";
            string bedloc = "";
            string absence_str = "";
            string wound_exist_str = "";

            string meal_Type_str = "";
            string memo_str = "";
            string pictureid = "";

            string diaper = "";

            string restraint = "";

            List<string> account_list = new List<string>();

            DateTime bday = new DateTime();
            StringBuilder sql = new StringBuilder();
            sql.Append(Cclient.get_bed_panel_brief(string.Format("and panel.display_id = {0} ", display_id)));



            DataSet setting_ds = share.GetDataSource(sql.ToString());


            if (setting_ds.Tables[0].Rows.Count > 0)
            {
                DataRow firstrow = setting_ds.Tables[0].Rows[0];
                bedloc = firstrow[2].ToString();



                string client_id = firstrow[3].ToString();
                string bed_id = firstrow[1].ToString();
                if (bed_id.Equals("0"))
                {

                    client_panel_briefing empty_brief = new client_panel_briefing
                    {
                        bednum = share.encry_value("未設定"),
                        night_mode = share.encry_value(firstrow[6].ToString()),
                        night_start = share.encry_value(firstrow[7].ToString()),
                        night_end = share.encry_value(firstrow[8].ToString()),
                        bed_id = share.encry_value(firstrow[1].ToString())
                       ,
                        picture_id = share.encry_value("D")
                        //picturename = picturenamestr
                    };
                    return empty_brief;
                }
                if (client_id.Equals("0"))
                {

                    client_panel_briefing empty_brief = new client_panel_briefing
                    {
                        bednum = share.encry_value(bedloc),
                        bed_id = share.encry_value(firstrow[1].ToString()),
                        night_mode = share.encry_value(firstrow[6].ToString()),
                        night_start = share.encry_value(firstrow[7].ToString()),
                        night_end = share.encry_value(firstrow[8].ToString()),
                        picture_id = share.encry_value("D")
                        //picturename = picturenamestr


                    };
                    return empty_brief;
                }

                else
                {

                    sql = new StringBuilder();
                    sql.Append(Cclient.get_client_briefing3_old(client_id));
                    sql.Append(Cclient.get_client_diet(client_id, string.Format("and dt.channel = 1  group by  dt.diet_type_id ")));
                    sql.Append(Cclient.get_client_diet(client_id, string.Format("and dt.channel = 2 group by  dt.diet_type_id  ")));
                    sql.Append(Cmedical.get_ae_brief("and ae.client_id = " + client_id + " and ae.ae_status = '留醫中' "));
                    sql.Append(Cmedical.get_revisit_brief("and rev.client_id = " + client_id + " and rev.revisit_status = '預約中' ", false));
                    sql.Append(Cmedical.get_event_grid_table("and ( rev.client_id = " + client_id + " and rev.event_status = '預約中' )",
                        false, "FIELD(rev.event_status, '預約中') desc, "));

                    DataSet ds = share.GetDataSource(sql.ToString());

                    int i = 0;
                    DataRow row = ds.Tables[0].Rows[0];
                    namestr = row[i++].ToString();
                    sexstr = row[i++].ToString().Equals("M") ? "男" : "女";
                    bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);
                    birth_datestr = bday.ToString("yyyy/MM/dd");

                    bedloc = row[i++].ToString();
                    age = row[i++].ToString();
                    personal_ability_str = row[i++].ToString();

                    wound_exist_str = row[i++].ToString();

                    if (wound_exist_str.Length > 0)
                    {
                        wound_exist_str = string.Format("傷口(" + wound_exist_str + ")");
                    }
                    absence_str = row[i++].ToString();

                    meal_Type_str = row[i++].ToString();

                    pictureid = row[i++].ToString();
                    if (pictureid.Length == 0 || pictureid == "0")
                    {
                        pictureid = sexstr;
                    }
                    //  sql.Append("doc.client_photo_id,per.contact_precaution,lift.tchi_value,tool.tchi_value ");
                    precautionstr = row[i++].ToString();
                    liftstr = row[i++].ToString();
                    toolsstr = row[i++].ToString();

                    responiblestr = row[i++].ToString();
                    if (responiblestr.Length > 0)
                    {
                        responiblestr = string.Format("負責護士 : {0}", responiblestr);
                    }
                    memo_str = row[i++].ToString();
                    diaper = row[i++].ToString();

                    restraint = row[i++].ToString();
                    for (int a = i; a < i + 3; a++)
                    {
                        string str = row[a].ToString();
                        if (str.Length > 0)
                        {
                            icons.Add(str);
                        }

                    }
                    string caution_str = "";
                    caution_str = string.Join(";", icons);
                    dinging = row[21].ToString();


                    string meal_first = "";

                    if (dinging.Length > 0)
                    {
                        meal_first = string.Format("用餐 : {0}", dinging);
                    }
                    



                    string meal_second = "";

                    string assessment_str = "";

                    string drug_allergic_str = "";
                    string adr_str = "";
                    string other_allergic_str = "";
                    string disease_str = "";
                    string revisit_str = "";
                    if (ds.Tables[1].Rows.Count != 0)
                    {
                        drug_allergic_str = ds.Tables[1].Rows[0][7].ToString();
                        adr_str = ds.Tables[1].Rows[0][8].ToString();
                        other_allergic_str = ds.Tables[1].Rows[0][9].ToString() + (ds.Tables[1].Rows[0][10].ToString().Length > 0 ? " " + ds.Tables[1].Rows[0][10].ToString() : "");

                        disease_str = ANICshare.add_comma(ds.Tables[1].Rows[0][4].ToString()) + ANICshare.add_comma(ds.Tables[1].Rows[0][5].ToString())
                            + ds.Tables[1].Rows[0][6].ToString();

                        assessment_str = ds.Tables[1].Rows[0][11].ToString();
                    }


                    if (ds.Tables[2].Rows.Count != 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        if (meal_first.Length > 0)
                        {
                            meal_first = meal_first + "\n";
                        }
                        for (int a = 0; a < ds.Tables[2].Rows.Count; a++)
                        {
                            stringBuilder.Append(string.Format("{0} : {1}", ds.Tables[2].Rows[a][2].ToString(), ds.Tables[2].Rows[a][3].ToString()));
                            if (a != ds.Tables[2].Rows.Count - 1)
                            {
                                stringBuilder.Append("\n");
                            }
                            //StringBuilder stringBuilder = new StringBuilder();
                        }
                        meal_first = meal_first + stringBuilder.ToString();


                    }

                    if (ds.Tables[3].Rows.Count != 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int a = 0; a < ds.Tables[3].Rows.Count; a++)
                        {
                            stringBuilder.Append(string.Format("{0} : {1}", ds.Tables[3].Rows[a][2].ToString(), ds.Tables[3].Rows[a][3].ToString()));
                            if (a != ds.Tables[3].Rows.Count - 1)
                            {
                                stringBuilder.Append("\n");
                            }
                            //StringBuilder stringBuilder = new StringBuilder();
                        }
                        meal_second = stringBuilder.ToString();


                    }




                    if (ds.Tables[4].Rows.Count != 0)
                    {
                        namestr = namestr + "- 急症";



                    }
                    if (ds.Tables[5].Rows.Count != 0)
                    {
                        /*
                        List<string> revisit_event = new List<string>();
                        foreach (DataRow item in ds.Tables[3].Rows)
                        {
                            string str = item[6].ToString() + item[7] + "," + item[3].ToString();
                            revisit_event.Add(str);
                        }

                        revisit_str = string.Join(";", revisit_event.ToArray());
                        */
                    }
                    //string headersize = "24";
                    string headersize = "50"; // Raymond "20200915
                    /*
                    string[] meal = new string[] { "", "", "飲食1", headersize, Color.Blue.ToArgb().ToString(), meal_first, "", Color.Black.ToArgb().ToString(), "40", "15", "Y", "N", "0", "N" };
                    string[] restrict = new string[] { "", "", "飲食2", headersize, Color.Blue.ToArgb().ToString(), meal_second, "", Color.Black.ToArgb().ToString(), "40", "15", "Y", "N", "0", "N" };
                    string[] remark_block = new string[] { "", "", "備忘錄", headersize, Color.Blue.ToArgb().ToString(), memo_str, "", Color.Black.ToArgb().ToString(), "40", "15", "Y", "N", "0", "Y" };
                    string[] mike_block = new string[] { "", "", "約束物品", headersize, Color.Blue.ToArgb().ToString(), restraint, "", Color.Black.ToArgb().ToString(), "60", "15", "Y", "N", "0", "Y" };
                    string[] assist_block = new string[] { "", "", "輔助工具", headersize, Color.Blue.ToArgb().ToString(), toolsstr, "", Color.Black.ToArgb().ToString(), "60", "15", "Y", "N", "0", "Y" };


                    string[] revisit = new string[] { "", "", "扶抱", headersize, Color.Blue.ToArgb().ToString(), liftstr, "", Color.Black.ToArgb().ToString(), "60", "15", "N", "N", "0", "Y" };
                  


                    string[] medical_event = new string[] { "", "", "紙尿片", headersize, Color.Blue.ToArgb().ToString(), diaper, "", Color.Black.ToArgb().ToString(), "60", "15", "N", "N", "0", "Y" };
                    */
                    /* //Raymond 20200915
                    string[] meal = new string[] { "", "", "飲食1", headersize, Color.Blue.ToArgb().ToString(), meal_first, "", Color.Black.ToArgb().ToString(), resize_font(meal_first, false), "15", "Y", "N", "0", "N" };
                    string[] restrict = new string[] { "", "", "飲食2", headersize, Color.Blue.ToArgb().ToString(), meal_second, "", Color.Black.ToArgb().ToString(), resize_font(meal_second, false), "15", "Y", "N", "0", "N" };
                    string[] remark_block = new string[] { "", "", "備忘錄", headersize, Color.Blue.ToArgb().ToString(), memo_str, "", Color.Black.ToArgb().ToString(), resize_font(memo_str, false), "15", "Y", "N", "0", "Y" };
                    string[] mike_block = new string[] { "", "", "約束物品", headersize, Color.Blue.ToArgb().ToString(), restraint, "", Color.Black.ToArgb().ToString(), resize_font(restraint, false), "15", "Y", "N", "0", "Y" };
                    string[] assist_block = new string[] { "", "", "輔助工具", headersize, Color.Blue.ToArgb().ToString(), toolsstr, "", Color.Black.ToArgb().ToString(), resize_font(toolsstr, false), "15", "Y", "N", "0", "Y" };


                    string[] revisit = new string[] { "", "", "扶抱", headersize, Color.Blue.ToArgb().ToString(), liftstr, "", Color.Black.ToArgb().ToString(), resize_font(liftstr, true), "15", "N", "N", "0", "Y" };



                    string[] medical_event = new string[] { "", "", "紙尿片", headersize, Color.Blue.ToArgb().ToString(), diaper, "", Color.Black.ToArgb().ToString(), resize_font(diaper, true), "15", "N", "N", "0", "Y" };
                    */


                    string[] meal = new string[] { "", "", "飲食1", headersize, Color.Blue.ToArgb().ToString(), meal_first, "", Color.Black.ToArgb().ToString(), headersize, "15", "Y", "N", "0", "N" };
                    string[] restrict = new string[] { "", "", "飲食2", headersize, Color.Blue.ToArgb().ToString(), meal_second, "", Color.Black.ToArgb().ToString(), headersize, "15", "Y", "N", "0", "N" };
                    string[] remark_block = new string[] { "", "", "備忘錄", headersize, Color.Blue.ToArgb().ToString(), memo_str, "", Color.Black.ToArgb().ToString(), headersize, "15", "Y", "N", "0", "Y" };
                    string[] mike_block = new string[] { "", "", "約束物品", headersize, Color.Blue.ToArgb().ToString(), restraint, "", Color.Black.ToArgb().ToString(), headersize, "15", "Y", "N", "0", "Y" };
                    string[] assist_block = new string[] { "", "", "輔助工具", headersize, Color.Blue.ToArgb().ToString(), toolsstr, "", Color.Black.ToArgb().ToString(), headersize, "15", "Y", "N", "0", "Y" };


                    string[] revisit = new string[] { "", "", "扶抱", headersize, Color.Blue.ToArgb().ToString(), liftstr, "", Color.Black.ToArgb().ToString(), headersize, "15", "N", "N", "0", "Y" };



                    string[] medical_event = new string[] { "", "", "紙尿片", headersize, Color.Blue.ToArgb().ToString(), diaper, "", Color.Black.ToArgb().ToString(), headersize, "15", "N", "N", "0", "Y" };




                    meal = share.encry_value(meal);
                    restrict = share.encry_value(restrict);

                    remark_block = share.encry_value(remark_block);
                    mike_block = share.encry_value(mike_block);

                    assist_block = share.encry_value(assist_block);

                    revisit = share.encry_value(revisit);
                    medical_event = share.encry_value(medical_event);

                    //飲食 約束衣使用 備忘錄 凝固粉 跌倒風險 紙尿片 覆診 醫療項目
                    blk.Add(get_blocks(meal));

                    blk.Add(get_blocks(restrict));

                    blk.Add(get_blocks(remark_block));
                    blk.Add(get_blocks(assist_block));
                    blk.Add(get_blocks(mike_block));


                    blk.Add(get_blocks(revisit));
                    blk.Add(get_blocks(medical_event));




                    client_panel_briefing brief = new client_panel_briefing
                    {

                        night_mode = share.encry_value(firstrow[6].ToString()),
                        night_start = share.encry_value(firstrow[7].ToString()),
                        night_end = share.encry_value(firstrow[8].ToString()),



                        name = share.encry_value(namestr),
                        sex = share.encry_value(sexstr),
                        age = share.encry_value(age),
                        bednum = share.encry_value(bedloc),
                        //relative = relative_name_str,
                        remark = share.encry_value(remarkstr),
                        client_id = share.encry_value(client_id),
                        bed_id = share.encry_value(bed_id),

                        caution = share.encry_value(caution_str),
                        //picturename = picturenamestr
                        picture_id = share.encry_value(pictureid),
                        personal_ability = share.encry_value(personal_ability_str),
                        birth_date = share.encry_value(birth_datestr),
                        wound_exist = share.encry_value(wound_exist_str),

                        meal_type = share.encry_value(meal_Type_str),

                        responsible = share.encry_value(responiblestr),
                        assessment_result = share.encry_value(assessment_str),
                        drug_ADR = share.encry_value(adr_str),
                        drug_allergic = share.encry_value(drug_allergic_str),
                        other_allergic = share.encry_value(other_allergic_str),
                        disease = share.encry_value(disease_str),
                        absence = share.encry_value(absence_str),
                        account_items = share.encry_value(account_list),
                        blocks = blk.ToArray()



                    };

                    return brief;
                }
            }
            else
            {
                return null;
            }
        }

        //For v153 update

        public client_panel_briefing get_client_display_info(string display_id)
        {
            string milk = "";
            display_id = share.dencry_value(display_id);
            List<panel_block> blk = new List<panel_block>();

            List<string> icons = new List<string>();

            string responiblestr = "";
            string dinging = "";
            string namestr = "";
            string sexstr = "";
            string age = "";
            string birth_datestr = "";

            string precautionstr = "";
            string liftstr = "";
            string toolsstr = "";

            string remarkstr = "";
            string personal_ability_str = "";
            string bedloc = "";
            string absence_str = "";
            string wound_exist_str = "";

            string meal_Type_str = "";
            string memo_str = "";
            string pictureid = "";

            string diaper = "";

            string restraint = "";

            List<string> account_list = new List<string>();

            DateTime bday = new DateTime();
            StringBuilder sql = new StringBuilder();
            sql.Append(Cclient.get_bed_panel_brief(string.Format("and panel.display_id = {0} ", display_id)));



            DataSet setting_ds = share.GetDataSource(sql.ToString());


            if (setting_ds.Tables[0].Rows.Count > 0)
            {
                DataRow firstrow = setting_ds.Tables[0].Rows[0];
                bedloc = firstrow[2].ToString();



                string client_id = firstrow[3].ToString();
                string bed_id = firstrow[1].ToString();
                if (bed_id.Equals("0"))
                {

                    client_panel_briefing empty_brief = new client_panel_briefing
                    {
                        bednum = share.encry_value("未設定"),
                        night_mode = share.encry_value(firstrow[6].ToString()),
                        night_start = share.encry_value(firstrow[7].ToString()),
                        night_end = share.encry_value(firstrow[8].ToString()),
                        bed_id = share.encry_value(firstrow[1].ToString())
                       ,
                        picture_id = share.encry_value("D")
                        //picturename = picturenamestr
                    };
                    return empty_brief;
                }
                if (client_id.Equals("0"))
                {

                    client_panel_briefing empty_brief = new client_panel_briefing
                    {
                        bednum = share.encry_value(bedloc),
                        bed_id = share.encry_value(firstrow[1].ToString()),
                        night_mode = share.encry_value(firstrow[6].ToString()),
                        night_start = share.encry_value(firstrow[7].ToString()),
                        night_end = share.encry_value(firstrow[8].ToString()),
                        picture_id = share.encry_value("D")
                        //picturename = picturenamestr


                    };
                    return empty_brief;
                }

                else
                {

                    sql = new StringBuilder();
                    sql.Append(Cclient.get_client_briefing3(client_id));
                    sql.Append(Cclient.get_client_diet(client_id, string.Format("and dt.channel = 1  group by  dt.diet_type_id ")));
                    sql.Append(Cclient.get_client_diet(client_id, string.Format("and dt.channel = 2 group by  dt.diet_type_id  ")));
                    sql.Append(Cmedical.get_ae_brief("and ae.client_id = " + client_id + " and ae.ae_status = '留醫中' "));
                    sql.Append(Cmedical.get_revisit_brief("and rev.client_id = " + client_id + " and rev.revisit_status = '預約中' ", false));
                    sql.Append(Cmedical.get_event_grid_table("and ( rev.client_id = " + client_id + " and rev.event_status = '預約中' )",
                        false, "FIELD(rev.event_status, '預約中') desc, "));

                    DataSet ds = share.GetDataSource(sql.ToString());

                    int i = 0;
                    DataRow row = ds.Tables[0].Rows[0];
                    namestr = row[i++].ToString();
                    sexstr = row[i++].ToString().Equals("M") ? "男" : "女";
                    bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);
                    birth_datestr = bday.ToString("yyyy/MM/dd");

                    bedloc = row[i++].ToString();
                    age = row[i++].ToString();
                    personal_ability_str = row[i++].ToString();

                    wound_exist_str = row[i++].ToString();

                    if (wound_exist_str.Length > 0)
                    {
                        wound_exist_str = string.Format("傷口(" + wound_exist_str + ")");
                    }
                    absence_str = row[i++].ToString();

                    meal_Type_str = row[i++].ToString();

                    pictureid = row[i++].ToString();
                    if (pictureid.Length == 0 || pictureid == "0")
                    {
                        pictureid = sexstr;
                    }
                    //  sql.Append("doc.client_photo_id,per.contact_precaution,lift.tchi_value,tool.tchi_value ");
                    precautionstr = row[i++].ToString();
                    liftstr = row[i++].ToString();
                    toolsstr = row[i++].ToString();

                    responiblestr = row[i++].ToString();
                    if (responiblestr.Length > 0)
                    {
                        responiblestr = string.Format("負責護士 : {0}", responiblestr);
                    }
                    memo_str = row[i++].ToString();
                    diaper = row[i++].ToString();

                    restraint = row[i++].ToString();
                    for (int a = i; a < i + 3; a++)
                    {
                        string str = row[a].ToString();
                        if (str.Length > 0)
                        {
                            icons.Add(str);
                        }

                    }
                    string caution_str = "";
                    caution_str = string.Join(";", icons);
                    dinging = row[21].ToString();


                    string meal_first = "";
                    string meal_second = "";


                    if (dinging.Length > 0)
                    {
                        meal_first = string.Format("用餐 : {0}", dinging);
                    }


                    string drug_allergic_str = "";
                    string adr_str = "";
                    string food_allergic_str = "";
                    string other_allergic_str = "";
                    string disease_str = "";
                    string icd9_str = "";
                    string assessment_str = "";

                    if (ds.Tables[1].Rows.Count != 0)
                    {



                        food_allergic_str = ds.Tables[1].Rows[0]["food_allergen"].ToString();


                        other_allergic_str = ds.Tables[1].Rows[0]["other_allergen"].ToString();

                        disease_str = ANICshare.add_comma(ds.Tables[1].Rows[0]["sickness_brief"].ToString()) + ANICshare.add_comma(ds.Tables[1].Rows[0]["diagnosis"].ToString());
                        icd9_str = ds.Tables[1].Rows[0]["icd9"].ToString();

                        assessment_str = ds.Tables[1].Rows[0]["assessment_result"].ToString();
                    }
                    if (ds.Tables[2].Rows.Count != 0)
                    {
                        drug_allergic_str = ds.Tables[2].Rows[0]["drug_allergen"].ToString();
                        adr_str = ds.Tables[2].Rows[0]["drug_adverse"].ToString();
                    }



                    //string assessment_str = "";

                    //string drug_allergic_str = "";
                    //string adr_str = "";
                    //string other_allergic_str = "";
                    //string disease_str = "";
                    //string revisit_str = "";
                    //if (ds.Tables[1].Rows.Count != 0)
                    //{
                    //    drug_allergic_str = ds.Tables[1].Rows[0][7].ToString();
                    //    adr_str = ds.Tables[1].Rows[0][8].ToString();
                    //    other_allergic_str = ds.Tables[1].Rows[0][9].ToString() + (ds.Tables[1].Rows[0][10].ToString().Length > 0 ? " " + ds.Tables[1].Rows[0][10].ToString() : "");

                    //    disease_str = ANICshare.add_comma(ds.Tables[1].Rows[0][4].ToString()) + ANICshare.add_comma(ds.Tables[1].Rows[0][5].ToString())
                    //        + ds.Tables[1].Rows[0][6].ToString();

                    //    assessment_str = ds.Tables[1].Rows[0][11].ToString();
                    //}



                    if (ds.Tables[3].Rows.Count != 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        if (meal_first.Length > 0)
                        {
                            meal_first = meal_first + "\n";
                        }
                        for (int a = 0; a < ds.Tables[3].Rows.Count; a++)
                        {
                            stringBuilder.Append(string.Format("{0} : {1}", ds.Tables[3].Rows[a][2].ToString(), ds.Tables[3].Rows[a][3].ToString()));
                            if (a != ds.Tables[3].Rows.Count - 1)
                            {
                                stringBuilder.Append("\n");
                            }
                            //StringBuilder stringBuilder = new StringBuilder();
                        }
                        meal_first = meal_first + stringBuilder.ToString();


                    }

                    if (ds.Tables[4].Rows.Count != 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int a = 0; a < ds.Tables[4].Rows.Count; a++)
                        {
                            stringBuilder.Append(string.Format("{0} : {1}", ds.Tables[4].Rows[a][2].ToString(), ds.Tables[4].Rows[a][3].ToString()));
                            if (a != ds.Tables[4].Rows.Count - 1)
                            {
                                stringBuilder.Append("\n");
                            }
                            //StringBuilder stringBuilder = new StringBuilder();
                        }
                        meal_second = stringBuilder.ToString();


                    }


                    if (!string.IsNullOrEmpty(food_allergic_str))
                    {
                        meal_second = meal_second + string.Format("\n禁食 : 免{0}", food_allergic_str.Replace(";", ",免"));
                    }



                    if (ds.Tables[5].Rows.Count != 0)
                    {
                        namestr = namestr + "- 急症";



                    }
                    if (ds.Tables[6].Rows.Count != 0)
                    {
                        /*
                        List<string> revisit_event = new List<string>();
                        foreach (DataRow item in ds.Tables[3].Rows)
                        {
                            string str = item[6].ToString() + item[7] + "," + item[3].ToString();
                            revisit_event.Add(str);
                        }

                        revisit_str = string.Join(";", revisit_event.ToArray());
                        */
                    }
                    string headersize = "24";
                    /*
                    string[] meal = new string[] { "", "", "飲食1", headersize, Color.Blue.ToArgb().ToString(), meal_first, "", Color.Black.ToArgb().ToString(), "40", "15", "Y", "N", "0", "N" };
                    string[] restrict = new string[] { "", "", "飲食2", headersize, Color.Blue.ToArgb().ToString(), meal_second, "", Color.Black.ToArgb().ToString(), "40", "15", "Y", "N", "0", "N" };
                    string[] remark_block = new string[] { "", "", "備忘錄", headersize, Color.Blue.ToArgb().ToString(), memo_str, "", Color.Black.ToArgb().ToString(), "40", "15", "Y", "N", "0", "Y" };
                    string[] mike_block = new string[] { "", "", "約束物品", headersize, Color.Blue.ToArgb().ToString(), restraint, "", Color.Black.ToArgb().ToString(), "60", "15", "Y", "N", "0", "Y" };
                    string[] assist_block = new string[] { "", "", "輔助工具", headersize, Color.Blue.ToArgb().ToString(), toolsstr, "", Color.Black.ToArgb().ToString(), "60", "15", "Y", "N", "0", "Y" };


                    string[] revisit = new string[] { "", "", "扶抱", headersize, Color.Blue.ToArgb().ToString(), liftstr, "", Color.Black.ToArgb().ToString(), "60", "15", "N", "N", "0", "Y" };
                  


                    string[] medical_event = new string[] { "", "", "紙尿片", headersize, Color.Blue.ToArgb().ToString(), diaper, "", Color.Black.ToArgb().ToString(), "60", "15", "N", "N", "0", "Y" };
                    */
                    string[] meal = new string[] { "", "", "飲食1", headersize, Color.Blue.ToArgb().ToString(), meal_first, "", Color.Black.ToArgb().ToString(), resize_font(meal_first, false), "15", "Y", "N", "0", "N" };
                    string[] restrict = new string[] { "", "", "飲食2", headersize, Color.Blue.ToArgb().ToString(), meal_second, "", Color.Black.ToArgb().ToString(), resize_font(meal_second, false), "15", "Y", "N", "0", "N" };
                    string[] remark_block = new string[] { "", "", "備忘錄", headersize, Color.Blue.ToArgb().ToString(), memo_str, "", Color.Black.ToArgb().ToString(), resize_font(memo_str, false), "15", "Y", "N", "0", "Y" };
                    string[] mike_block = new string[] { "", "", "約束物品", headersize, Color.Blue.ToArgb().ToString(), restraint, "", Color.Black.ToArgb().ToString(), resize_font(restraint, false), "15", "Y", "N", "0", "Y" };
                    string[] assist_block = new string[] { "", "", "輔助工具", headersize, Color.Blue.ToArgb().ToString(), toolsstr, "", Color.Black.ToArgb().ToString(), resize_font(toolsstr, false), "15", "Y", "N", "0", "Y" };


                    string[] revisit = new string[] { "", "", "扶抱", headersize, Color.Blue.ToArgb().ToString(), liftstr, "", Color.Black.ToArgb().ToString(), resize_font(liftstr, true), "15", "N", "N", "0", "Y" };



                    string[] medical_event = new string[] { "", "", "紙尿片", headersize, Color.Blue.ToArgb().ToString(), diaper, "", Color.Black.ToArgb().ToString(), resize_font(diaper, true), "15", "N", "N", "0", "Y" };








                    meal = share.encry_value(meal);
                    restrict = share.encry_value(restrict);

                    remark_block = share.encry_value(remark_block);
                    mike_block = share.encry_value(mike_block);

                    assist_block = share.encry_value(assist_block);

                    revisit = share.encry_value(revisit);
                    medical_event = share.encry_value(medical_event);

                    //飲食 約束衣使用 備忘錄 凝固粉 跌倒風險 紙尿片 覆診 醫療項目
                    blk.Add(get_blocks(meal));

                    blk.Add(get_blocks(restrict));

                    blk.Add(get_blocks(remark_block));
                    blk.Add(get_blocks(assist_block));
                    blk.Add(get_blocks(mike_block));


                    blk.Add(get_blocks(revisit));
                    blk.Add(get_blocks(medical_event));




                    client_panel_briefing brief = new client_panel_briefing
                    {

                        night_mode = share.encry_value(firstrow[6].ToString()),
                        night_start = share.encry_value(firstrow[7].ToString()),
                        night_end = share.encry_value(firstrow[8].ToString()),



                        name = share.encry_value(namestr),
                        sex = share.encry_value(sexstr),
                        age = share.encry_value(age),
                        bednum = share.encry_value(bedloc),
                        //relative = relative_name_str,
                        remark = share.encry_value(remarkstr),
                        client_id = share.encry_value(client_id),
                        bed_id = share.encry_value(bed_id),

                        caution = share.encry_value(caution_str),
                        //picturename = picturenamestr
                        picture_id = share.encry_value(pictureid),
                        personal_ability = share.encry_value(personal_ability_str),
                        birth_date = share.encry_value(birth_datestr),
                        wound_exist = share.encry_value(wound_exist_str),

                        meal_type = share.encry_value(meal_Type_str),

                        responsible = share.encry_value(responiblestr),
                        assessment_result = share.encry_value(assessment_str),
                        drug_ADR = share.encry_value(adr_str),
                        drug_allergic = share.encry_value(drug_allergic_str),
                        food_allergic = share.encry_value(food_allergic_str),
                        other_allergic = share.encry_value(other_allergic_str),
                        disease = share.encry_value(disease_str),
                        absence = share.encry_value(absence_str),
                        account_items = share.encry_value(account_list),
                        blocks = blk.ToArray()



                    };

                    return brief;
                }
            }
            else
            {
                return null;
            }
        }


        //  [WebMethod()]
        public string  resize_font(string serial_num,  bool is_halfblock)
        {
            int font_size = 40;
            if (is_halfblock)
            {
                if (serial_num.Length>5)
                {
                    font_size = 40;
                }
                else
                {
                    font_size = 60;
                }
            }
            else
            {
                if (serial_num.Length > 40)
                {
                    font_size = 30;
                }

                else if (serial_num.Length > 30)
                {
                    font_size = 35;
                }
 
             
                else
                {
                    font_size = 40;
                }
            }
            return font_size.ToString();
        }

            public byte[] udpate_version(string serial_num, string location, string location_long, string listen_port, string address, string vers)
        {
            try
            {
                serial_num = share.dencry_value(serial_num);
                location = share.dencry_value(location);
                location_long = share.dencry_value(location_long);
                listen_port = share.dencry_value(listen_port);
                address = share.dencry_value(address);
                vers = share.dencry_value(vers);
                string time = share.get_current_time();
                CPanel panel = new CPanel();
                string sql = Clogin.display_getlogin(serial_num,string.Format(" and autoupdate='Y' ")) + panel.get_all_panel_version(string.Format("and version> {0}", vers));
                
                DataSet ds = share.GetDataSource(sql);
                if (ds.Tables[0].Rows.Count > 0&& ds.Tables[1].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    DataRow version_row = ds.Tables[1].Rows[0];
                    String display_id = row[0].ToString();
                    string version = version_row[0].ToString();
                    string path = version_row[1].ToString();
                    //    int version = int.Parse( version_row[0].ToString());

                    if (row[5].ToString() == "N")
                    {
                        return null;
                    }
                   
                    else
                    {
                        //  SELECT* FROM azure.sys_display_panel_update_log;
                        //   record_id, display_id, update_version, created_by, created_datetime
                        //     panel.
                      //  String id = share.Get_mysql_database_MaxID("sys_display_panel_update_log", "record_id");
                        string[] values = new string[] { display_id, version, System.Environment.MachineName, time };
                        share.bool_Ecxe_transaction_Sql(panel.display_update_version(values));
                        string fileName = @path;
                        //   BinaryReader binReader = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read));
                        //   byte[] binFile = binReader.ReadBytes(Convert.ToInt32(binReader.BaseStream.Length));
                        //   binReader.Close();
                        //   return binFile;
                        /*
                        byte[] buffer = null;
                        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                        {
                            buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, (int)fs.Length);
                        }
                        return buffer;
                        */
                           System.IO.FileStream fs1 = null;
                          fs1 = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read);
                        byte[] b1 = new byte[fs1.Length];
                            fs1.Read(b1, 0, (int)fs1.Length);
                            fs1.Close();
                            return b1;
                    }
                    // null;

                }
                else
                {
 
                    return null;
                };
 
            }
            catch
            {
                return null;
            }
        }
        public room_panel_briefing get_room_display_info(string display_id)
        {

            display_id = share.dencry_value(display_id);
            List<panel_block> blk = new List<panel_block>();
            string[] str_arr = display_id.Split(';');
     



            string namestr = "";
 

            string remarkstr = "";


            string personal_ability_str = "";
            string bedloc = "";
            string absence_str = "";
            string wound_exist_str = "";

            string meal_Type_str = "";

            string pictureid = "";
            List<string> account_list = new List<string>();

            DateTime bday = new DateTime();
            StringBuilder sql = new StringBuilder();
            sql.Append(CRoom.get_room_panel_briefing(string.Format(" and room.display_id = '{0}' ",display_id)));


            DataSet ds = share.GetDataSource(sql.ToString());


            if (ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int i = 0;
                    namestr = row[i++].ToString();
                    remarkstr = row[i++].ToString();
                    string bed_id = row[i++].ToString();
                    string client_id = row[i++].ToString();
                    String client_name = row[i++].ToString();

                    String sex = row[i++].ToString();
                    String dob = row[i++].ToString();
                    String picture_id = row[i++].ToString();

                 
                    String bed_name = row[i++].ToString();
                    String alarm = row[i++].ToString();
                  

                    if (picture_id.Length == 0)
                    {
                        picture_id = sex;
                    }

                    if (bed_id.Length > 0)
                    {
                        string[] values = new string[] { bed_id, client_id, bed_name, "32", Color.Blue.ToArgb().ToString(), client_name, "", Color.Black.ToArgb().ToString(), "60", "15", "Y", alarm, picture_id, "Y" };
                        values = share.encry_value(values);
                        //飲食 約束衣使用 備忘錄 凝固粉 跌倒風險 紙尿片 覆診 醫療項目
                        blk.Add(get_blocks(values));
                    }
                }
            }
            else
            {
                return null;
            }

 
            room_panel_briefing brief = new room_panel_briefing
            {
                room_name =  share.encry_value(namestr) ,
                remark = share.encry_value(remarkstr),
                blocks = blk.ToArray()



            };

            return brief;


        }
        private panel_block get_blocks(string[] value )
        {
            int i = 0;

            panel_block block = new panel_block()
            {

                id = value[i++],
                sub_id = value[i++],
                header = value[i++],
                header_font_size = value[i++],
                header_color = value[i++],

                main_title = value[i++],
                description = value[i++],
       
                main_title_color = value[i++],
                main_title_font_size = value[i++],
                sub_title_font_size = value[i++],
                is_full = value[i++]
                ,alarm = value[i++]
                ,
                picture_id = value[i++],
                single = value[i++]

            };

            return block;

        }

        // Green 20201112 copy from alpine
        public nursing_panel_briefing get_nursing_display_info2(string display_id)
        {
            display_id = share.dencry_value(display_id);
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
            List<panel_nursing_block> dash_list = new List<panel_nursing_block>();
            Encription encr = new Encription();


            DataSet ds = share.GetDataSource(Cmedical.get_nursing_panel_briefing2(display_id));
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                DataRow row = ds.Tables[0].Rows[0];
                string scroll_speed = row[1].ToString();

                StringBuilder sql = new StringBuilder();


                sql.Append(Cmedical.get_revisit_brief("and per.active_status = 'Y' and rev.revisit_status  = '預約中' and date( rev.revisit_planned_datetime) <= CURDATE() + INTERVAL 2 DAY ", false));
                sql.Append(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                sql.Append(Cmedical.get_event_grid_table(" and per.active_status = 'Y' and rev.event_status  ='預約中'" +
" and eve.dash_shown = 'Y' " +
" and ( date(rev.event_planned_datetime) <= (date(now()) + INTERVAL (eve.reminder_day-1) DAY ))  "));
                //  sql.Append(Cclient.get_client_panel_entry_briefing("and per.active_status = 'Y' and (date( per.entry_date ) >= date(curdate()) and date( per.entry_date ) <= DATE_ADD(CURDATE(), INTERVAL 30 DAY)) group by per.client_id order by per.entry_date  "));

                sql.Append(Cbook.get_book_info(" and a.book_status = '預訂中' "));
                //icp
                sql.Append(Cmedical.get_icp_brief(""));
                sql.Append(Cmedical.get_home_leave_brief(""));
                sql.Append(Cclient.get_current_residential_analysis());
                sql.Append(";");

                // sql.Append(Crestraint.get_restraint_brief(" and ( rev.restraint_state = '有效' or rev.restraint_state = '親屬口頭同意') "));
                DataSet dm = share.GetDataSource(sql.ToString());

                //" and ( rev.restraint_state = '有效' or rev.restraint_state = '親屬口頭同意') "
                //for (int i = 0; i < dm.Tables.Count; i++)
                //{
                //    dash_list = get_nursing_block(ds, dm, i, dash_list);
                //}
                string font = ds.Tables[0].Rows[0]["font_size"].ToString();

                bool showAsBedName = ds.Tables[0].Rows[0]["show_bedname"] == null ? false : ds.Tables[0].Rows[0]["show_bedname"].ToString() == "Y" ? true : false;
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dash_list = get_nursing_block(font, ds.Tables[1].Rows[i], dm, dash_list, showAsBedName);
                }
                string theme_dark_color = "#c79f6a";
                string theme_medium_color = "#d8bc96";
                string theme_light_color = "#f1e9dc";

                nursing_panel_briefing board = new nursing_panel_briefing()
                {

                    theme_dark_color = share.encry_value(theme_dark_color),
                    theme_light_color = share.encry_value(theme_light_color),
                    theme_medium_color = share.encry_value(theme_medium_color),
                    blocks
                      = dash_list.ToArray(),
                    scroll_speed = share.encry_value(scroll_speed)

                };


                return board;
            }
            else
            {
                return null;
            }

        }

        // Raymond 20200917 copy from alpine (nursing time check list)
        public nursing_panel_briefing get_nursing_display_info(string display_id)
        {
            display_id = share.dencry_value(display_id);
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
            List<panel_nursing_block> dash_list = new List<panel_nursing_block>();
            Encription encr = new Encription();


            DataSet ds = share.GetDataSource(Cmedical.get_nursing_panel_briefing(display_id));
            if (ds.Tables[0].Rows.Count > 0)
            {

                DataRow row = ds.Tables[0].Rows[0];
                string scroll_speed = row[1].ToString();

                StringBuilder sql = new StringBuilder();


                sql.Append(Cmedical.get_revisit_brief("and per.active_status = 'Y' and rev.revisit_status  = '預約中' and date( rev.revisit_planned_datetime) <= CURDATE() + INTERVAL 2 DAY ", false));
                sql.Append(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                sql.Append(Cmedical.get_event_grid_table(" and per.active_status = 'Y' and rev.event_status  ='預約中'" +
" and eve.dash_shown = 'Y' " +
" and ( date(rev.event_planned_datetime) <= (date(now()) + INTERVAL (eve.reminder_day-1) DAY ))  "));
                //  sql.Append(Cclient.get_client_panel_entry_briefing("and per.active_status = 'Y' and (date( per.entry_date ) >= date(curdate()) and date( per.entry_date ) <= DATE_ADD(CURDATE(), INTERVAL 30 DAY)) group by per.client_id order by per.entry_date  "));

                // " and a.book_status = '預訂中' "
                sql.Append(Cbook.get_book_info(" and a.book_status = '預訂中' "));
                //icp
                #region
                sql.Append(" select main.icp_id, IFNULL(concat(per.chi_surname,per.chi_name),''),main.client_id, ");
                sql.Append(" DATE_FORMAT(main.due_date, '%Y-%m-%d') as icp_date, ");
                sql.Append(" main.icp_type_id, ctype.assess_type, ");
                sql.Append(" main.icp_status, icpstatus.status_chi, ");
                sql.Append(" per.sex ,doc.client_photo_id ");
                sql.Append(" from care_icp_main as main ");
                sql.Append(" inner join client_personal2 as per on per.client_id = main.client_id and per.active_status = 'Y' ");
                sql.Append(" inner join sys_care_icp_status icpstatus on icpstatus.status_id = main.icp_status and icpstatus.valid = 'Y' ");
                sql.Append(" left join sys_care_icp_type ctype on ctype.icp_type_id = main.icp_type_id and ctype.valid = 'Y' ");
                sql.Append(" left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
                sql.Append(" left join sys_company_zone z on b.zone_id = z.zone_id   ");
                sql.Append(" left join sys_company_block k on k.block_id = z.block_id   ");
                sql.Append(" left join nursing_setting nset on nset.valid = 'Y' ");
                sql.Append(" left join client_documents2 doc on main.client_id = doc.client_id and doc.valid = 'Y'   ");
                sql.Append(" where main.valid = 'Y' ");
                sql.Append(" and ( ");
                sql.Append(" (main.icp_status = 100001 and ( date(main.due_date) <= (date(now()) + INTERVAL (nset.icp_remind_day-1) DAY ))) ");
                sql.Append(" ) ");
                sql.Append(" and nset.icp_remind_shown = 'Y' ");
                sql.Append(" union ");
                sql.Append(" select main.icp_id, IFNULL(concat(per.chi_surname,per.chi_name),''),main.client_id, ");
                sql.Append(" DATE_FORMAT(main.review_date, '%Y-%m-%d') as icp_date, ");
                sql.Append(" main.icp_type_id, ctype.assess_type, ");
                sql.Append(" main.icp_status, icpstatus.status_chi, ");
                sql.Append(" per.sex ,doc.client_photo_id ");
                sql.Append(" from care_icp_main as main ");
                sql.Append(" inner join client_personal2 as per on per.client_id = main.client_id and per.active_status = 'Y' ");
                sql.Append(" inner join sys_care_icp_status icpstatus on icpstatus.status_id = main.icp_status and icpstatus.valid = 'Y' ");
                sql.Append(" left join sys_care_icp_type ctype on ctype.icp_type_id = main.icp_type_id and ctype.valid = 'Y' ");
                sql.Append(" left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
                sql.Append(" left join sys_company_zone z on b.zone_id = z.zone_id   ");
                sql.Append(" left join sys_company_block k on k.block_id = z.block_id   ");
                sql.Append(" left join nursing_setting nset on nset.valid = 'Y' ");
                sql.Append(" left join client_documents2 doc on main.client_id = doc.client_id and doc.valid = 'Y'   ");
                sql.Append(" where main.valid = 'Y' ");
                sql.Append(" and  ");
                sql.Append(" (main.icp_status = 100004 and ( date(main.review_date) <= (date(now()) + INTERVAL (nset.icp_remind_day-1) DAY ))) ");
                sql.Append("  ");
                sql.Append(" and nset.icp_remind_shown = 'Y' ");
                sql.Append(" order by icp_date; ");
                #endregion
                //   sql.Append(Cbook.get_book_info(" and (date( a.book_planned_entry_date ) >= date(curdate()) and date(a.book_planned_entry_date ) <= DATE_ADD(CURDATE(), INTERVAL 3 DAY))    "));

                //sql.Append(Cmedical.get_home_leave_brief(""));
                sql.Append(Cclient.get_current_residential_analysis());
                sql.Append(";");

                // sql.Append(Crestraint.get_restraint_brief(" and ( rev.restraint_state = '有效' or rev.restraint_state = '親屬口頭同意') "));
                DataSet dm = share.GetDataSource(sql.ToString());

                //" and ( rev.restraint_state = '有效' or rev.restraint_state = '親屬口頭同意') "
                for (int i = 0; i < dm.Tables.Count; i++)
                {
                    dash_list = get_nursing_block(ds, dm, i, dash_list);
                }

                string dark_color = Color.FromArgb(21, 169, 157).ToArgb().ToString();
                string light_color = Color.FromArgb(240, 253, 252).ToArgb().ToString();
                string medium_color = Color.FromArgb(58, 228, 216).ToArgb().ToString();

                nursing_panel_briefing board = new nursing_panel_briefing()
                {

                    theme_dark_color = share.encry_value(dark_color),
                    theme_light_color = share.encry_value(light_color),
                    theme_medium_color = share.encry_value(medium_color),
                    blocks
                      = dash_list.ToArray(),
                    scroll_speed = share.encry_value(scroll_speed)

                };


                return board;
            }
            else
            {
                return null;
            }

        }


        public nursing_panel_briefing get_nursing_display_info_old(string display_id)
        {
            display_id = share.dencry_value(display_id);
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
            List<panel_nursing_block> dash_list = new List<panel_nursing_block>();
            Encription encr = new Encription();

    
            DataSet ds = share.GetDataSource(Cmedical.get_nursing_panel_briefing(display_id));
            if (ds.Tables[0].Rows.Count > 0)
            {

                DataRow row = ds.Tables[0].Rows[0];
                string scroll_speed = row[1].ToString();
 
                                StringBuilder sql = new StringBuilder();

 
                sql.Append(Cmedical.get_revisit_brief("and per.active_status = 'Y' and rev.revisit_status  = '預約中' and date( rev.revisit_planned_datetime) <= CURDATE() + INTERVAL 2 DAY ", false));
                sql.Append(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                sql.Append(Cmedical.get_event_grid_table(" and per.active_status = 'Y' and rev.event_status  ='預約中'" +
" and eve.dash_shown = 'Y' " +
" and ( date(rev.event_planned_datetime) <= (date(now()) + INTERVAL (eve.reminder_day-1) DAY ))  "));
              //  sql.Append(Cclient.get_client_panel_entry_briefing("and per.active_status = 'Y' and (date( per.entry_date ) >= date(curdate()) and date( per.entry_date ) <= DATE_ADD(CURDATE(), INTERVAL 30 DAY)) group by per.client_id order by per.entry_date  "));

                // " and a.book_status = '預訂中' "
                sql.Append(Cbook.get_book_info(" and a.book_status = '預訂中' "));
                sql.Append( Cclient.get_current_residential_analysis());
                sql.Append(";");
                //icp
                #region
                sql.Append(" select main.icp_id, IFNULL(concat(per.chi_surname,per.chi_name),''),main.client_id, ");
                sql.Append(" DATE_FORMAT(main.due_date, '%Y-%m-%d') as icp_date, ");
                sql.Append(" main.icp_type_id, ctype.assess_type, ");
                sql.Append(" main.icp_status, icpstatus.status_chi, ");
                sql.Append(" per.sex ,doc.client_photo_id ");
                sql.Append(" from care_icp_main as main ");
                sql.Append(" inner join client_personal2 as per on per.client_id = main.client_id and per.active_status = 'Y' ");
                sql.Append(" inner join sys_care_icp_status icpstatus on icpstatus.status_id = main.icp_status and icpstatus.valid = 'Y' ");
                sql.Append(" left join sys_care_icp_type ctype on ctype.icp_type_id = main.icp_type_id and ctype.valid = 'Y' ");
                sql.Append(" left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
                sql.Append(" left join sys_company_zone z on b.zone_id = z.zone_id   ");
                sql.Append(" left join sys_company_block k on k.block_id = z.block_id   ");
                sql.Append(" left join nursing_setting nset on nset.valid = 'Y' ");
                sql.Append(" left join client_documents2 doc on main.client_id = doc.client_id and doc.valid = 'Y'   ");
                sql.Append(" where main.valid = 'Y' ");
                sql.Append(" and ( ");
                sql.Append(" (main.icp_status = 100001 and ( date(main.due_date) <= (date(now()) + INTERVAL (nset.icp_remind_day-1) DAY ))) ");
                sql.Append(" ) ");
                sql.Append(" and nset.icp_remind_shown = 'Y' ");
                sql.Append(" union ");
                sql.Append(" select main.icp_id, IFNULL(concat(per.chi_surname,per.chi_name),''),main.client_id, ");
                sql.Append(" DATE_FORMAT(main.review_date, '%Y-%m-%d') as icp_date, ");
                sql.Append(" main.icp_type_id, ctype.assess_type, ");
                sql.Append(" main.icp_status, icpstatus.status_chi, ");
                sql.Append(" per.sex ,doc.client_photo_id ");
                sql.Append(" from care_icp_main as main ");
                sql.Append(" inner join client_personal2 as per on per.client_id = main.client_id and per.active_status = 'Y' ");
                sql.Append(" inner join sys_care_icp_status icpstatus on icpstatus.status_id = main.icp_status and icpstatus.valid = 'Y' ");
                sql.Append(" left join sys_care_icp_type ctype on ctype.icp_type_id = main.icp_type_id and ctype.valid = 'Y' ");
                sql.Append(" left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
                sql.Append(" left join sys_company_zone z on b.zone_id = z.zone_id   ");
                sql.Append(" left join sys_company_block k on k.block_id = z.block_id   ");
                sql.Append(" left join nursing_setting nset on nset.valid = 'Y' ");
                sql.Append(" left join client_documents2 doc on main.client_id = doc.client_id and doc.valid = 'Y'   ");
                sql.Append(" where main.valid = 'Y' ");
                sql.Append(" and  ");
                sql.Append(" (main.icp_status = 100004 and ( date(main.review_date) <= (date(now()) + INTERVAL (nset.icp_remind_day-1) DAY ))) ");
                sql.Append("  ");
                sql.Append(" and nset.icp_remind_shown = 'Y' ");
                sql.Append(" order by icp_date; ");
                #endregion
                //   sql.Append(Cbook.get_book_info(" and (date( a.book_planned_entry_date ) >= date(curdate()) and date(a.book_planned_entry_date ) <= DATE_ADD(CURDATE(), INTERVAL 3 DAY))    "));


                // sql.Append(Crestraint.get_restraint_brief(" and ( rev.restraint_state = '有效' or rev.restraint_state = '親屬口頭同意') "));
                DataSet dm = share.GetDataSource(sql.ToString());

                //" and ( rev.restraint_state = '有效' or rev.restraint_state = '親屬口頭同意') "
                for (int i = 0; i < dm.Tables.Count; i++)
                {
                    dash_list = get_nursing_block(ds, dm,i, dash_list);
                }
         
                string dark_color = Color.FromArgb(21, 169, 157).ToArgb().ToString() ;
                string light_color = Color.FromArgb(240, 253, 252).ToArgb().ToString();
                string medium_color = Color.FromArgb(58, 228, 216).ToArgb().ToString();

                nursing_panel_briefing board = new nursing_panel_briefing()
                {

                    theme_dark_color = share.encry_value(dark_color),
                    theme_light_color = share.encry_value(light_color),
                    theme_medium_color = share.encry_value(medium_color),
                    blocks
                      = dash_list.ToArray(),
                    scroll_speed = share.encry_value(scroll_speed)

                };


                return board;
            }
            else
            {
                return null;
            }

        }

        private List<panel_nursing_block> get_nursing_block_old(DataSet ds,  DataSet dm , int index, List<panel_nursing_block> dash_list)
        {
 
            List<panel_nursing_client_block> client_list = new List<panel_nursing_client_block>();
            string font = ds.Tables[0].Rows[0][2].ToString();
            string table_name = "";
            string cols_str = "";
            string gravity   = "";
            string is_block_str = "";
            string time_check = "";
            ////  sql.Append("SELECT npanel.nursing_panel_id ,scroll_speed,font_size, ");
            //  sql.Append("shown_revisit, shown_medical, shown_ae, shown_restraint, ");
            //   sql.Append(" format_revisit, format_medical, format_ae, format_restraint, ");
            //   sql.Append("channel_revisit, channel_medical, channel_ae, channel_restraint, ");

            //   sql.Append("revisit_theme_id, medical_theme_id, ae_theme_id, restraint_theme_id ");

            if (index == 0)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_revisit_brief("and per.active_status = 'Y' and rev.revisit_status  = '預約中' and date( rev.revisit_planned_datetime) <= CURDATE() + INTERVAL 1 DAY ", false));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    // sql.Append("shown_revisit, shown_medical, shown_ae, shown_restraint, ");

                    if (ds.Tables[0].Rows[0][3].ToString().Equals("Y"))
                    {
                        // ds.Tables[0].Rows[0][7].ToString();
                        gravity = ds.Tables[0].Rows[0][11].ToString();
                        cols_str = "姓名;日期;地點;交通";
                        table_name = "覆診" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                        is_block_str = ds.Tables[0].Rows[0][6].ToString() == "0" ? "Y" : "N";
                        time_check = "1@yyyy/MM/dd hh:mm a;";
                        DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][15].ToString());
                        string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };


                        //    sql.Append(" select theme_id, theme_name, dark_color, medime_color, light_color, ordering ");

                        //               sql.Append(" FROM azure.sys_theme_color");

                        //    is_block_str = "N";
                        for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                        {
                            DataRow row = dm.Tables[index].Rows[i];
                            string client_name_str = row[1].ToString();
                            string date_str = row[3].ToString() + "\n" + row[4].ToString();
                            string sex_str = row[13].ToString();
                            string picture_id_str = row[14].ToString();

                            string client_id_str = row[2].ToString();
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }


                            //     string address = string.Format("AD;地點;{0}", row[6].ToString());


                            //     string spe = string.Format("SP;專科;{0}", row[7].ToString());


                            string address = string.Format(row[6].ToString() + " " + row[7].ToString());


                            string tran = string.Format(row[9].ToString() + " " + row[11].ToString());
                            //      string tran = string.Format("TP;交通;{0}", row[9].ToString());
                            // string accompany = string.Format("PP;陪診員;{0}", row[11].ToString());

                            string[] cont = new string[] { client_name_str, date_str, address, tran };
                            client_list.Add(new panel_nursing_client_block()
                            {

                                name = client_name_str,
                                //sex = sex_str,
                                id = client_id_str,
                                datetime = date_str,
                                picture_id = picture_id_str,
                                description1 = address,
                                description2 = tran
                                ,
                                contents = cont
                            }
                            );
                        }


                        dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));
                    }
                }
            }
            else if (index == 1)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][5].ToString().Equals("Y"))
                    {
                        //time_check = "1@yyyy/MM/dd hh:mm a;";
                        time_check = "";
                        is_block_str = ds.Tables[0].Rows[0][8].ToString() == "0" ? "Y" : "N";
                        gravity = gravity = ds.Tables[0].Rows[0][13].ToString();
                        cols_str = "姓名;入院日期;地點;床位";
                        table_name = "急症" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                        DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][17].ToString());
                        string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };

                        for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                        {
                            DataRow row = dm.Tables[index].Rows[i];

                            string client_name_str = row["client_name"].ToString();
                            string sex_str = row["sex"].ToString();
                            string picture_id_str = row["client_photo_id"].ToString();
                            string date_str = row["ae_in_date"].ToString() + " " + row["ae_in_time"].ToString();

                            string client_id_str = row["client_id"].ToString();


                            string address = string.Format(row["addr_org_chi_name"].ToString());
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }
                            ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                            //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                            string reason = string.Format("DC;急症原因;{0}", row["ae_in_reason"].ToString());

                            string bed = string.Format(row["ae_in_bed_num"].ToString());

                            /*
                            string client_name_str = row[1].ToString();
                            string sex_str = row[13].ToString();
                            string picture_id_str = row[14].ToString();


                            //string[] timearray = row[5].ToString().Split(' ');
                            string date_str = row[5].ToString();

                            string client_id_str = row[2].ToString();


                            string address = string.Format(row[7].ToString());
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }
                            ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                            //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                            string reason = string.Format("DC;急症原因;{0}", row[4].ToString());

                            string bed = string.Format(row[8].ToString());
                            //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());
                            */

                            string[] cont = new string[] { client_name_str, date_str, address, bed };
                            client_list.Add(new panel_nursing_client_block()
                            {
                                name = client_name_str,
                                //sex = sex_str,
                                id = client_id_str,
                                datetime = date_str,
                                picture_id = picture_id_str,
                                description1 = address,
                                description2 = bed,
                                contents = cont

                            }
                            );
                        }
                        dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));
                    }
                }
            }
            else if (index == 2)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_event_grid_table(" and per.active_status = 'Y' and rev.event_status  ='預約中'" +
                // " and eve.dash_shown = 'Y' " +
                //  " and ( date(rev.event_planned_datetime) <= (date(now()) + INTERVAL (eve.reminder_day-1) DAY ))  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][4].ToString().Equals("Y"))
                    {
                        gravity = gravity = ds.Tables[0].Rows[0][12].ToString();
                        cols_str = "姓名;日期;時間;備註";
                        table_name = "";
                        is_block_str = "N";
                        time_check = "1@yyyy/MM/dd;";
                        DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][16].ToString());
                        string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };

                        for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                        {
                            DataRow row = dm.Tables[index].Rows[i];

                            if (table_name != row[0].ToString())
                            {
                                if (client_list.Count > 0)
                                {
                                    dash_list.Add(get_nuring_event(table_name + string.Format(" - {0}項", client_list.Count.ToString()), font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));
                                }


                                table_name = row[0].ToString();
                                client_list = new List<panel_nursing_client_block>();

                                string client_name_str = row[3].ToString();
                                string sex_str = row[2].ToString();
                                string picture_id_str = row[12].ToString();
                                //string[] timearray = row[5].ToString().Split(' ');

                                string date_str = row[6].ToString();

                                string client_id_str = row[4].ToString();

                                //string date = string.Format("CA;日期;{0}", row[6].ToString());

                                string time_str = string.Format(row[7].ToString());
                                string state = string.Format(row[9].ToString());
                                if (picture_id_str.Length == 0)
                                {
                                    picture_id_str = sex_str;
                                }
                                //string bed = string.Format("BD;留醫病房／床號:{0}", row[8].ToString());
                                //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());
                                string[] cont = new string[] { client_name_str, date_str, time_str, state };

                                client_list.Add(new panel_nursing_client_block()
                                {
                                    name = client_name_str,
                                    //sex = sex_str,
                                    id = client_id_str,
                                    datetime = date_str,
                                    picture_id = picture_id_str,
                                    description1 = time_str,
                                    description2 = state
                                    ,
                                    contents = cont
                                    //             client_name = client_name_str,
                                    //             sex = sex_str,
                                    //             client_id = client_id_str,
                                    //           date = date_str,
                                    //             client_picture_id = picture_id_str,
                                    //             contents = new string[] { time_str, state }
                                }

                                );
                                if (i == dm.Tables[index].Rows.Count - 1)
                                {
                                    dash_list.Add(get_nuring_event(table_name + string.Format(" - {0}項", client_list.Count.ToString()), font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));
                                }
                            }
                            else
                            {

                                string client_id_str = row[4].ToString();
                                string client_name = row[3].ToString();
                                string sex_str = row[2].ToString();

                                string date = string.Format(row[6].ToString());
                                string time_str = string.Format(row[7].ToString());
                                string state = string.Format(row[9].ToString());
                                string picture_id_str = row[12].ToString();
                                string[] cont = new string[] { client_name, date, time_str, state };
                                if (picture_id_str.Length == 0)
                                {
                                    picture_id_str = sex_str;
                                }
                                client_list.Add(new panel_nursing_client_block()
                                {
                                    name = client_name,
                                    //sex = sex_str,
                                    id = client_id_str,
                                    datetime = date,
                                    picture_id = picture_id_str,
                                    description1 = time_str,
                                    description2 = state,
                                    contents = cont
                                    //            client_name = row[3].ToString(),
                                    //             sex = row[2].ToString(),
                                    //             client_id = row[4].ToString(),
                                    //             date = row[6].ToString(),
                                    //             client_picture_id = row[12].ToString(),
                                    //             contents = new string[] { time_str, state }
                                });
                                if (i == dm.Tables[index].Rows.Count - 1)
                                {
                                    dash_list.Add(get_nuring_event(table_name + string.Format(" - {0}項", client_list.Count.ToString()), font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));
                                }
                            }
                        }
                    }
                }
            }
            /*
            else if (index == 3)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {

                    is_block_str = ds.Tables[0].Rows[0][9].ToString() == "0" ? "Y" : "N";
                    gravity = gravity = ds.Tables[0].Rows[0][14].ToString(); ;
                    cols_str = "姓名;開始日期;結束日期;約束物品";
                    table_name = "約束物品";
                    DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][18].ToString());
                    string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };


                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = row[1].ToString();
                        string sex_str = row[14].ToString();
                        string picture_id_str = row[15].ToString();


                        //string[] timearray = row[5].ToString().Split(' ');
                        string date_st = row[3].ToString();
                        string date_ed = row[4].ToString();

                        string client_id_str = row[2].ToString();


                        string address = string.Format(row[7].ToString());

                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }
                        ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                        //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                        //string reason = string.Format("DC;急症原因;{0}", row[4].ToString());

                        string bed = string.Format(row[13].ToString());
                        string items = string.Format(row[5].ToString());
                        //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());

                        string[] cont = new string[] { client_name_str, date_st, "", items };
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_ed,
                            picture_id = picture_id_str,
                            description1 = items,
                            description2 = date_ed,
                            contents = cont

                        }
                        );
                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, client_list.ToArray()));
                }
            }
            */
            /*
            else if (index == 3)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {

                    //   is_block_str = ds.Tables[0].Rows[0][9].ToString() == "0" ? "Y" : "N";
                  is_block_str = "N";
                    gravity = gravity = ds.Tables[0].Rows[0][14].ToString(); ;
                    cols_str = "姓名;入住日期;床位;評核";
                    table_name = "新入住" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());


                  //  DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][18].ToString());
                    string[] theme_color =  get_default_theme_color()  ;
 
                   
                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = row[0].ToString()+string.Format("({0})",(row[1].ToString().Equals("M")?"男":"女"));
                       
                        string sex_str = row[1].ToString();
                        string picture_id_str = row[8].ToString();


                        //string[] timearray = row[5].ToString().Split(' ');
                        string date_st = row[20].ToString();
                    //    string date_ed = row[4].ToString();

                        string client_id_str = row[21].ToString();

                        string assess = assessment_result_to_eng( row[19].ToString());

                        string address = string.Format(row[7].ToString());
                       // Cclient.get_client_panel_entry_briefing
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }
                        ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                        //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                        //string reason = string.Format("DC;急症原因;{0}", row[4].ToString());

                        string bed = string.Format(row[3].ToString());
                        string items = string.Format(row[18].ToString());
                        //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());

                        string[] cont = new string[] { client_name_str, date_st, bed, assess };
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_st,
                            picture_id = picture_id_str,
                            description1 = bed,
                            description2 = assess,
                            contents = cont

                        }
                        );
                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, client_list.ToArray()));
                }
            }
            */
            else if (index == 3)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][19].ToString().Equals("Y"))
                    {
                        time_check = "1@yyyy/MM/dd;";

                        //   is_block_str = ds.Tables[0].Rows[0][9].ToString() == "0" ? "Y" : "N";
                        is_block_str = "N";
                        gravity = gravity = ds.Tables[0].Rows[0][14].ToString(); ;
                        cols_str = "姓名;預計入住日期;床位;評核";
                        table_name = "訂位" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());

                        //    sql.Append("SELECT a.book_id, a.book_ref_num,concat(b.chi_surname, b.chi_name),b.hkid,concat(k.tchi_value,'-',z.tchi_value,'-',d.tchi_value) ,date_format(a.book_planned_entry_date,'%Y/%m/%d'),a.book_status, ");
                        //     sql.Append(" ifnull(c.total_balance,'0.00') ,b.client_number,if( date_format(b.entry_date,'%Y/%m/%d')='1900/05/16','', date_format(b.entry_date,'%Y/%m/%d')),concat(k2.tchi_value,'-',z2.tchi_value,'-',d2.tchi_value) ");
                        //      sql.Append(", b.sex ,b.dob ,b.assessment_result ,doc.client_photo_id,b.client_id,");

                        //      sql.Append(" a.book_buy_type ,a.book_date ,");
                        //      sql.Append(" a.book_contact_name, a.book_contact_num, a.book_contact_type,a.book_remark, concat(b.eng_surname, b.eng_name)");

                        //  DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][18].ToString());
                        string[] theme_color = get_default_theme_color();


                        for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                        {
                            DataRow row = dm.Tables[index].Rows[i];
                            string client_name_str = row[2].ToString() + string.Format("({0})", (row[11].ToString().Equals("M") ? "男" : "女"));

                            string sex_str = row[11].ToString();
                            string picture_id_str = row[14].ToString();


                            //string[] timearray = row[5].ToString().Split(' ');
                            string date_st = row[5].ToString();
                            //    string date_ed = row[4].ToString();

                            string client_id_str = row[15].ToString();

                            string assess = assessment_result_to_eng(row[13].ToString());

                            string address = string.Format(row[7].ToString());
                            // Cclient.get_client_panel_entry_briefing
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }
                            ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                            //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                            //string reason = string.Format("DC;急症原因;{0}", row[4].ToString());

                            string bed = string.Format(row[4].ToString());
                            // string items = string.Format(row[18].ToString());
                            //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());

                            string[] cont = new string[] { client_name_str, date_st, bed, assess };
                            client_list.Add(new panel_nursing_client_block()
                            {
                                name = client_name_str,
                                //sex = sex_str,
                                id = client_id_str,
                                datetime = date_st,
                                picture_id = picture_id_str,
                                description1 = bed,
                                description2 = assess,
                                contents = cont

                            }
                            );
                        }
                        dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));
                    }
                }
            }
            else if (index == 4)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][20].ToString().Equals("Y"))
                    {

                        //   is_block_str = ds.Tables[0].Rows[0][9].ToString() == "0" ? "Y" : "N";
                        is_block_str = "N";
                        gravity = gravity = ds.Tables[0].Rows[0][14].ToString(); ;
                        cols_str = "入住人數;男/女;CA/NH;入住醫院";
                        table_name = "院舍統計";
                        time_check = "";
                        //    sql.Append("SELECT a.book_id, a.book_ref_num,concat(b.chi_surname, b.chi_name),b.hkid,concat(k.tchi_value,'-',z.tchi_value,'-',d.tchi_value) ,date_format(a.book_planned_entry_date,'%Y/%m/%d'),a.book_status, ");
                        //     sql.Append(" ifnull(c.total_balance,'0.00') ,b.client_number,if( date_format(b.entry_date,'%Y/%m/%d')='1900/05/16','', date_format(b.entry_date,'%Y/%m/%d')),concat(k2.tchi_value,'-',z2.tchi_value,'-',d2.tchi_value) ");
                        //      sql.Append(", b.sex ,b.dob ,b.assessment_result ,doc.client_photo_id,b.client_id,");

                        //      sql.Append(" a.book_buy_type ,a.book_date ,");
                        //      sql.Append(" a.book_contact_name, a.book_contact_num, a.book_contact_type,a.book_remark, concat(b.eng_surname, b.eng_name)");
                        int ae_client, current_client,
                          medium, high, other, male, female;
                        ae_client = current_client =
                            medium = high = other = male = female = 0;
                        foreach (DataRow row in dm.Tables[index].Rows)
                        {
                            current_client = current_client + int.Parse(row[2].ToString());
                            male = male + int.Parse(row[3].ToString());
                            female = female + int.Parse(row[4].ToString());
                            ae_client = ae_client + int.Parse(row[5].ToString());
                            medium = medium + int.Parse(row[6].ToString());
                            high = high + int.Parse(row[7].ToString());
                            other = other + int.Parse(row[8].ToString());



                            //Cshare.trace_value(row[1].ToString() + row[2].ToString());
                            //client_analysis_lab[row_index++].Text = row[1].ToString();
                            // client_analysis_lab[row_index++].Text = row[2].ToString();

                            //tableLayoutPanel20.Controls.Add(new Label { Text = row[2].ToString(), Anchor = AnchorStyles.Left, AutoSize = true }, 0, row_index);
                            //tableLayoutPanel20.Controls.Add(new Label { Text = row[1].ToString(), Anchor = AnchorStyles.Left, AutoSize = true }, 1, row_index++);

                        }

                        /*'

                        中度機能受損
                            高度機能受損
                            其他
                            男院友
                           女院友
                        int i = 0;
                        DataRow row = ds.Tables[0].Rows[0];
                        lab_current_client_num.Text = row[i++].ToString();
                        lab_current_live_client.Text = row[i++].ToString();
                        lab_sub_respite.Text = row[i++].ToString();
                        lab_non_sub_respite.Text = row[i++].ToString();
                        lab_sub_long_term.Text = row[i++].ToString();
                        lab_non_sub_long_term.Text = row[i++].ToString();


                        lab_buy_type.Text = row[i++].ToString();
                        lab_non_buy_type.Text = row[i++].ToString();



                        lab_other_long_term.Text = row[i++].ToString();
                        lab_medium_client.Text = row[i++].ToString();
                        lab_high_client.Text = row[i++].ToString();
                        lab_other_level_client.Text = row[i++].ToString();
                        lab_num_of_male_client.Text = row[i++].ToString();
                        lab_num_of_female_client.Text = row[i++].ToString();
                        */

                        //  DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][18].ToString());
                        string[] theme_color = get_default_theme_color();


                        for (int i = 0; i < 1; i++)
                        {
                            //  DataRow row = dm.Tables[index].Rows[i];
                            string client_name_str = string.Format("{0}人", current_client.ToString());


                            string picture_id_str = "D";


                            //string[] timearray = row[5].ToString().Split(' ');
                            string date_st = string.Format("{0}/{1}人", male.ToString(), female.ToString());
                            //    string date_ed = row[4].ToString();
                            string bed = string.Format("{0}/{1}人", medium.ToString(), high.ToString());
                            string assess = string.Format("{0}人", ae_client.ToString());


                            string client_id_str = "0";




                            // Cclient.get_client_panel_entry_briefing

                            ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                            //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                            //string reason = string.Format("DC;急症原因;{0}", row[4].ToString());


                            // string items = string.Format(row[18].ToString());
                            //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());

                            string[] cont = new string[] { client_name_str, date_st, bed, assess };
                            client_list.Add(new panel_nursing_client_block()
                            {
                                name = client_name_str,
                                //sex = sex_str,
                                id = client_id_str,
                                datetime = date_st,
                                picture_id = picture_id_str,
                                description1 = bed,
                                description2 = assess,
                                contents = cont

                            }
                            );
                        }
                        dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));
                    }
                }
            }
            else if (index == 5)
            {
                if (dm.Tables[index].Rows.Count > 0)
                {
                    gravity = ds.Tables[0].Rows[0][11].ToString();
                    cols_str = "姓名;日期;ICP類別;狀態";
                    table_name = "ICP" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                    is_block_str = "N";
                    time_check = "1@yyyy/MM/dd;";
                    DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][16].ToString());
                    string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };

                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                                DataRow row = dm.Tables[index].Rows[i];
                               string client_name_str = row[1].ToString();
                               string date_str = row[3].ToString();
                               string sex_str = row[8].ToString();
                                string picture_id_str = row[9].ToString();

                        string client_id_str = row[2].ToString();
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }

                        string status = row[7].ToString();
                        string icpType = row[5].ToString();
                        string[] cont = new string[] { client_name_str, date_str, icpType, status };
                        client_list.Add(new panel_nursing_client_block()
                        {

                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_str,
                            picture_id = picture_id_str,
                            description1 = icpType,
                            description2 = status
                            ,
                            contents = cont
                        }
                        );
                    }


                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));
                }
            }


            return dash_list;

        }

        // Raymond 20200917 copy from alpine (nursing time check list)

        private List<panel_nursing_block> get_nursing_block(DataSet ds, DataSet dm, int index, List<panel_nursing_block> dash_list)
        {

            List<panel_nursing_client_block> client_list = new List<panel_nursing_client_block>();
            string font = ds.Tables[0].Rows[0][2].ToString();
            string table_name = "";
            string cols_str = "";
            string gravity = "";
            string is_block_str = "";
            string time_check = "";
            ////  sql.Append("SELECT npanel.nursing_panel_id ,scroll_speed,font_size, ");
            //  sql.Append("shown_revisit, shown_medical, shown_ae, shown_restraint, ");
            //   sql.Append(" format_revisit, format_medical, format_ae, format_restraint, ");
            //   sql.Append("channel_revisit, channel_medical, channel_ae, channel_restraint, ");

            //   sql.Append("revisit_theme_id, medical_theme_id, ae_theme_id, restraint_theme_id ");

            if (index == 0)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_revisit_brief("and per.active_status = 'Y' and rev.revisit_status  = '預約中' and date( rev.revisit_planned_datetime) <= CURDATE() + INTERVAL 1 DAY ", false));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    // sql.Append("shown_revisit, shown_medical, shown_ae, shown_restraint, ");

                    if (ds.Tables[0].Rows[0][3].ToString().Equals("Y"))
                    {
                        // ds.Tables[0].Rows[0][7].ToString();
                        gravity = ds.Tables[0].Rows[0][11].ToString();
                        cols_str = "姓名;日期;地點;交通";
                        table_name = "覆診" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                        is_block_str = ds.Tables[0].Rows[0][6].ToString() == "0" ? "Y" : "N";
                        time_check = "";// "1@yyyy/MM/dd hh:mm a;";
                        List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd hh:mm a" },
                    };
                        DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][15].ToString());
                        string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };


                        //    sql.Append(" select theme_id, theme_name, dark_color, medime_color, light_color, ordering ");

                        //               sql.Append(" FROM azure.sys_theme_color");

                        //    is_block_str = "N";
                        for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                        {
                            DataRow row = dm.Tables[index].Rows[i];

                            string client_name_str = row["client_name"].ToString();
                            string date_str = row["revisit_planned_date"].ToString() + "\n" + row["revisit_planned_time"].ToString();
                            string sex_str = row["sex"].ToString();
                            string picture_id_str = row["photo_id"].ToString();

                            string client_id_str = row["client_id"].ToString();
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }
                            string address = string.Format(row["addr_org_chi_name"].ToString() + " " + row["specialties_code"].ToString());
                            string tran = string.Format(row["transport"].ToString() + " " + row["revisit_remark"].ToString());
                            string[] cont = new string[] { client_name_str, date_str, address, tran };

                            /*
                            string client_name_str = row[1].ToString();
                            string date_str = row[3].ToString() + "\n" + row[4].ToString();
                            string sex_str = row[13].ToString();
                            string picture_id_str = row[14].ToString();

                            string client_id_str = row[2].ToString();
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }


                            //     string address = string.Format("AD;地點;{0}", row[6].ToString());


                            //     string spe = string.Format("SP;專科;{0}", row[7].ToString());


                            string address = string.Format(row[6].ToString() + " " + row[7].ToString());


                            string tran = string.Format(row[9].ToString() + " " + row[11].ToString());
                            //      string tran = string.Format("TP;交通;{0}", row[9].ToString());
                            // string accompany = string.Format("PP;陪診員;{0}", row[11].ToString());

                            string[] cont = new string[] { client_name_str, date_str, address, tran };
                            */
                            client_list.Add(new panel_nursing_client_block()
                            {

                                name = client_name_str,
                                //sex = sex_str,
                                id = client_id_str,
                                datetime = date_str,
                                picture_id = picture_id_str,
                                description1 = address,
                                description2 = tran
                                ,
                                contents = cont
                            }
                            );
                        }


                        dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                    }
                }
            }
            else if (index == 1)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][5].ToString().Equals("Y"))
                    {
                        //time_check = "1@yyyy/MM/dd hh:mm a;";
                        time_check = "";
                        List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "N", date_format = "yyyy/MM/dd hh:mm a" },
                    };

                        is_block_str = ds.Tables[0].Rows[0][8].ToString() == "0" ? "Y" : "N";
                        gravity = gravity = ds.Tables[0].Rows[0][13].ToString();
                        cols_str = "姓名;入院日期;地點;床位";
                        table_name = "急症" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                        DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][17].ToString());
                        string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };

                        for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                        {
                            DataRow row = dm.Tables[index].Rows[i];

                            string client_name_str = row["client_name"].ToString();
                            string sex_str = row["sex"].ToString();
                            string picture_id_str = row["client_photo_id"].ToString();
                            string date_str = row["ae_in_date"].ToString() + " " + row["ae_in_time"].ToString();

                            string client_id_str = row["client_id"].ToString();


                            string address = string.Format(row["addr_org_chi_name"].ToString());
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }
                            ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                            //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                            string reason = string.Format("DC;急症原因;{0}", row["ae_in_reason"].ToString());

                            string bed = string.Format(row["ae_in_bed_num"].ToString());

                            /*
                            string client_name_str = row[1].ToString();
                            string sex_str = row[13].ToString();
                            string picture_id_str = row[14].ToString();


                            //string[] timearray = row[5].ToString().Split(' ');
                            string date_str = row[5].ToString();

                            string client_id_str = row[2].ToString();


                            string address = string.Format(row[7].ToString());
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }
                            ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                            //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                            string reason = string.Format("DC;急症原因;{0}", row[4].ToString());

                            string bed = string.Format(row[8].ToString());
                            //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());
                            */
                            string[] cont = new string[] { client_name_str, date_str, address, bed };
                            client_list.Add(new panel_nursing_client_block()
                            {
                                name = client_name_str,
                                //sex = sex_str,
                                id = client_id_str,
                                datetime = date_str,
                                picture_id = picture_id_str,
                                description1 = address,
                                description2 = bed,
                                contents = cont

                            }
                            );
                        }
                        dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                    }
                }
            }
            else if (index == 2)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_event_grid_table(" and per.active_status = 'Y' and rev.event_status  ='預約中'" +
                // " and eve.dash_shown = 'Y' " +
                //  " and ( date(rev.event_planned_datetime) <= (date(now()) + INTERVAL (eve.reminder_day-1) DAY ))  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][4].ToString().Equals("Y"))
                    {
                        gravity = gravity = ds.Tables[0].Rows[0][12].ToString();
                        cols_str = "姓名;日期;時間;備註";
                        table_name = "";
                        is_block_str = "N";
                        time_check = "";// "1@yyyy/MM/dd;";
                        List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd" },
                    };
                        DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][16].ToString());
                        string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };

                        for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                        {
                            DataRow row = dm.Tables[index].Rows[i];
                            if (table_name != row[0].ToString() && client_list.Count > 0)
                            {
                                dash_list.Add(get_nuring_event(table_name + string.Format(" - {0}項", client_list.Count.ToString()), font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                                client_list = new List<panel_nursing_client_block>();
                                table_name = row[0].ToString();
                            }
                            if (table_name != row[0].ToString())
                            {
                                table_name = row[0].ToString();
                            }

                            string client_id_str = row[4].ToString();
                            string client_name = row[3].ToString();
                            string sex_str = row[2].ToString();

                            string date = string.Format(row[6].ToString());
                            string time_str = string.Format(row[7].ToString());
                            string state = string.Format(row[9].ToString());
                            string picture_id_str = row[12].ToString();
                            string[] cont = new string[] { client_name, date, time_str, state };
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }
                            client_list.Add(new panel_nursing_client_block()
                            {
                                name = client_name,
                                //sex = sex_str,
                                id = client_id_str,
                                datetime = date,
                                picture_id = picture_id_str,
                                description1 = time_str,
                                description2 = state,
                                contents = cont
                                //            client_name = row[3].ToString(),
                                //             sex = row[2].ToString(),
                                //             client_id = row[4].ToString(),
                                //             date = row[6].ToString(),
                                //             client_picture_id = row[12].ToString(),
                                //             contents = new string[] { time_str, state }
                            });

                        }
                        dash_list.Add(get_nuring_event(table_name + string.Format(" - {0}項", client_list.Count.ToString()), font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));

                    }
                }
            }
            /*
            else if (index == 3)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {

                    is_block_str = ds.Tables[0].Rows[0][9].ToString() == "0" ? "Y" : "N";
                    gravity = gravity = ds.Tables[0].Rows[0][14].ToString(); ;
                    cols_str = "姓名;開始日期;結束日期;約束物品";
                    table_name = "約束物品";
                    DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][18].ToString());
                    string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };


                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = row[1].ToString();
                        string sex_str = row[14].ToString();
                        string picture_id_str = row[15].ToString();


                        //string[] timearray = row[5].ToString().Split(' ');
                        string date_st = row[3].ToString();
                        string date_ed = row[4].ToString();

                        string client_id_str = row[2].ToString();


                        string address = string.Format(row[7].ToString());

                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }
                        ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                        //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                        //string reason = string.Format("DC;急症原因;{0}", row[4].ToString());

                        string bed = string.Format(row[13].ToString());
                        string items = string.Format(row[5].ToString());
                        //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());

                        string[] cont = new string[] { client_name_str, date_st, "", items };
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_ed,
                            picture_id = picture_id_str,
                            description1 = items,
                            description2 = date_ed,
                            contents = cont

                        }
                        );
                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, client_list.ToArray()));
                }
            }
            */
            /*
            else if (index == 3)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {

                    //   is_block_str = ds.Tables[0].Rows[0][9].ToString() == "0" ? "Y" : "N";
                  is_block_str = "N";
                    gravity = gravity = ds.Tables[0].Rows[0][14].ToString(); ;
                    cols_str = "姓名;入住日期;床位;評核";
                    table_name = "新入住" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());


                  //  DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][18].ToString());
                    string[] theme_color =  get_default_theme_color()  ;
 
                   
                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = row[0].ToString()+string.Format("({0})",(row[1].ToString().Equals("M")?"男":"女"));
                       
                        string sex_str = row[1].ToString();
                        string picture_id_str = row[8].ToString();


                        //string[] timearray = row[5].ToString().Split(' ');
                        string date_st = row[20].ToString();
                    //    string date_ed = row[4].ToString();

                        string client_id_str = row[21].ToString();

                        string assess = assessment_result_to_eng( row[19].ToString());

                        string address = string.Format(row[7].ToString());
                       // Cclient.get_client_panel_entry_briefing
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }
                        ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                        //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                        //string reason = string.Format("DC;急症原因;{0}", row[4].ToString());

                        string bed = string.Format(row[3].ToString());
                        string items = string.Format(row[18].ToString());
                        //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());

                        string[] cont = new string[] { client_name_str, date_st, bed, assess };
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_st,
                            picture_id = picture_id_str,
                            description1 = bed,
                            description2 = assess,
                            contents = cont

                        }
                        );
                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, client_list.ToArray()));
                }
            }
            */
            else if (index == 3)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][19].ToString().Equals("Y"))
                    {
                        time_check = "";// "1@yyyy/MM/dd;";

                        List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd" },
                    };
                        //   is_block_str = ds.Tables[0].Rows[0][9].ToString() == "0" ? "Y" : "N";
                        is_block_str = "N";
                        gravity = gravity = ds.Tables[0].Rows[0][14].ToString(); ;
                        cols_str = "姓名;預計入住日期;床位;評核";
                        table_name = "訂位" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());

                        //    sql.Append("SELECT a.book_id, a.book_ref_num,concat(b.chi_surname, b.chi_name),b.hkid,concat(k.tchi_value,'-',z.tchi_value,'-',d.tchi_value) ,date_format(a.book_planned_entry_date,'%Y/%m/%d'),a.book_status, ");
                        //     sql.Append(" ifnull(c.total_balance,'0.00') ,b.client_number,if( date_format(b.entry_date,'%Y/%m/%d')='1900/05/16','', date_format(b.entry_date,'%Y/%m/%d')),concat(k2.tchi_value,'-',z2.tchi_value,'-',d2.tchi_value) ");
                        //      sql.Append(", b.sex ,b.dob ,b.assessment_result ,doc.client_photo_id,b.client_id,");

                        //      sql.Append(" a.book_buy_type ,a.book_date ,");
                        //      sql.Append(" a.book_contact_name, a.book_contact_num, a.book_contact_type,a.book_remark, concat(b.eng_surname, b.eng_name)");

                        //  DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][18].ToString());
                        string[] theme_color = get_default_theme_color();


                        for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                        {
                            DataRow row = dm.Tables[index].Rows[i];
                            string client_name_str = row[2].ToString() + string.Format("({0})", (row[11].ToString().Equals("M") ? "男" : "女"));

                            string sex_str = row[11].ToString();
                            string picture_id_str = row[14].ToString();


                            //string[] timearray = row[5].ToString().Split(' ');
                            string date_st = row[5].ToString();
                            //    string date_ed = row[4].ToString();

                            string client_id_str = row[15].ToString();

                            string assess = assessment_result_to_eng(row[13].ToString());

                            string address = string.Format(row[7].ToString());
                            // Cclient.get_client_panel_entry_briefing
                            if (picture_id_str.Length == 0)
                            {
                                picture_id_str = sex_str;
                            }
                            ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                            //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                            //string reason = string.Format("DC;急症原因;{0}", row[4].ToString());

                            string bed = string.Format(row[4].ToString());
                            // string items = string.Format(row[18].ToString());
                            //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());

                            string[] cont = new string[] { client_name_str, date_st, bed, assess };
                            client_list.Add(new panel_nursing_client_block()
                            {
                                name = client_name_str,
                                //sex = sex_str,
                                id = client_id_str,
                                datetime = date_st,
                                picture_id = picture_id_str,
                                description1 = bed,
                                description2 = assess,
                                contents = cont

                            }
                            );
                        }
                        dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                    }
                }
            }
            else if (index == 4)
            {
                if (dm.Tables[index].Rows.Count > 0)
                {
                    gravity = ds.Tables[0].Rows[0][11].ToString();
                    cols_str = "姓名;日期;ICP類別;狀態";
                    table_name = "ICP" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                    is_block_str = "N";
                    List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd" },
                    };
                    DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][16].ToString());
                    string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };

                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = row[1].ToString();
                        string date_str = row[3].ToString();
                        string sex_str = row[8].ToString();
                        string picture_id_str = row[9].ToString();

                        string client_id_str = row[2].ToString();
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }

                        string status = row[7].ToString();
                        string icpType = row[5].ToString();
                        string[] cont = new string[] { client_name_str, date_str, icpType, status };
                        client_list.Add(new panel_nursing_client_block()
                        {

                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_str,
                            picture_id = picture_id_str,
                            description1 = icpType,
                            description2 = status
                            ,
                            contents = cont
                        }
                        );
                    }


                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                }
            }
            else if (index == 5)
            {
                // Home leave Raymond comment
                /*
                if (dm.Tables[index].Rows.Count > 0)
                {
                    time_check = "";
                    List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "N", date_format = "yyyy/MM/dd" },
                        new panel_time_check(){ column_idx = "2", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd" },
                    };
                    is_block_str = "N";
                    gravity = gravity = ds.Tables[0].Rows[0][11].ToString();
                    cols_str = "姓名;外宿日期;預計回院;外宿原因;";
                    table_name = "外宿" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                    DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][16].ToString());
                    string[] theme_color = color_row == null ? get_default_theme_color() : new string[] { get_color(color_row[2].ToString()), get_color(color_row[3].ToString()), get_color(color_row[4].ToString()), };

                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = row[1].ToString();
                        string sex_str = row["sex"].ToString();
                        string picture_id_str = row["client_photo_id"].ToString();

                        string date_str = row["leave_out_datetime"].ToString();
                        string plan_return_str = row["leave_planned_return_datetime"].ToString();
                        string client_id_str = row["client_id"].ToString();


                        string reason = string.Format(row["reason_name"].ToString());
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }
                        string[] cont = new string[] { client_name_str, date_str, plan_return_str, reason };
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name_str,
                            id = client_id_str,
                            datetime = date_str,
                            picture_id = picture_id_str,
                            description1 = plan_return_str,
                            description2 = reason,
                            contents = cont

                        }
                        );

                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                }
                */
            }
            else if (index == 6)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][20].ToString().Equals("Y"))
                    {
                        is_block_str = "N";
                        gravity = gravity = ds.Tables[0].Rows[0][14].ToString(); ;
                        cols_str = "入住人數;男/女;CA/NH;入住醫院";
                        table_name = "院舍統計";
                        time_check = "";
                        //    sql.Append("SELECT a.book_id, a.book_ref_num,concat(b.chi_surname, b.chi_name),b.hkid,concat(k.tchi_value,'-',z.tchi_value,'-',d.tchi_value) ,date_format(a.book_planned_entry_date,'%Y/%m/%d'),a.book_status, ");
                        //     sql.Append(" ifnull(c.total_balance,'0.00') ,b.client_number,if( date_format(b.entry_date,'%Y/%m/%d')='1900/05/16','', date_format(b.entry_date,'%Y/%m/%d')),concat(k2.tchi_value,'-',z2.tchi_value,'-',d2.tchi_value) ");
                        //      sql.Append(", b.sex ,b.dob ,b.assessment_result ,doc.client_photo_id,b.client_id,");

                        //      sql.Append(" a.book_buy_type ,a.book_date ,");
                        //      sql.Append(" a.book_contact_name, a.book_contact_num, a.book_contact_type,a.book_remark, concat(b.eng_surname, b.eng_name)");
                        int ae_client, current_client,
                          medium, high, other, male, female;
                        ae_client = current_client =
                            medium = high = other = male = female = 0;
                        foreach (DataRow row in dm.Tables[index].Rows)
                        {
                            current_client = current_client + int.Parse(row[2].ToString());
                            male = male + int.Parse(row[3].ToString());
                            female = female + int.Parse(row[4].ToString());
                            ae_client = ae_client + int.Parse(row[5].ToString());
                            medium = medium + int.Parse(row[6].ToString());
                            high = high + int.Parse(row[7].ToString());
                            other = other + int.Parse(row[8].ToString());



                            //Cshare.trace_value(row[1].ToString() + row[2].ToString());
                            //client_analysis_lab[row_index++].Text = row[1].ToString();
                            // client_analysis_lab[row_index++].Text = row[2].ToString();

                            //tableLayoutPanel20.Controls.Add(new Label { Text = row[2].ToString(), Anchor = AnchorStyles.Left, AutoSize = true }, 0, row_index);
                            //tableLayoutPanel20.Controls.Add(new Label { Text = row[1].ToString(), Anchor = AnchorStyles.Left, AutoSize = true }, 1, row_index++);

                        }

                        /*'

                        中度機能受損
                            高度機能受損
                            其他
                            男院友
                           女院友
                        int i = 0;
                        DataRow row = ds.Tables[0].Rows[0];
                        lab_current_client_num.Text = row[i++].ToString();
                        lab_current_live_client.Text = row[i++].ToString();
                        lab_sub_respite.Text = row[i++].ToString();
                        lab_non_sub_respite.Text = row[i++].ToString();
                        lab_sub_long_term.Text = row[i++].ToString();
                        lab_non_sub_long_term.Text = row[i++].ToString();


                        lab_buy_type.Text = row[i++].ToString();
                        lab_non_buy_type.Text = row[i++].ToString();



                        lab_other_long_term.Text = row[i++].ToString();
                        lab_medium_client.Text = row[i++].ToString();
                        lab_high_client.Text = row[i++].ToString();
                        lab_other_level_client.Text = row[i++].ToString();
                        lab_num_of_male_client.Text = row[i++].ToString();
                        lab_num_of_female_client.Text = row[i++].ToString();
                        */

                        //  DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][18].ToString());
                        string[] theme_color = get_default_theme_color();


                        for (int i = 0; i < 1; i++)
                        {
                            //  DataRow row = dm.Tables[index].Rows[i];
                            string client_name_str = string.Format("{0}人", current_client.ToString());


                            string picture_id_str = "D";


                            //string[] timearray = row[5].ToString().Split(' ');
                            string date_st = string.Format("{0}/{1}人", male.ToString(), female.ToString());
                            //    string date_ed = row[4].ToString();
                            string bed = string.Format("{0}/{1}人", medium.ToString(), high.ToString());
                            string assess = string.Format("{0}人", ae_client.ToString());


                            string client_id_str = "0";




                            // Cclient.get_client_panel_entry_briefing

                            ///string time = string.Format("TI;時間:{0}", row[4].ToString());
                            //string date = string.Format("CA;急症日期;{0}", row[5].ToString());

                            //string reason = string.Format("DC;急症原因;{0}", row[4].ToString());


                            // string items = string.Format(row[18].ToString());
                            //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());

                            string[] cont = new string[] { client_name_str, date_st, bed, assess };
                            client_list.Add(new panel_nursing_client_block()
                            {
                                name = client_name_str,
                                //sex = sex_str,
                                id = client_id_str,
                                datetime = date_st,
                                picture_id = picture_id_str,
                                description1 = bed,
                                description2 = assess,
                                contents = cont

                            }
                            );
                        }
                        dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));
                    }
                }
            }


            return dash_list;

        }

        // green 20201112 copy from alpine
        private List<panel_nursing_block> get_nursing_block(string font, DataRow dr, DataSet dm, List<panel_nursing_block> dash_list, bool showAsBedName = false)
        {
            if (dr == null || dm == null) return dash_list;
            List<panel_nursing_client_block> client_list = new List<panel_nursing_client_block>();

            if (string.IsNullOrWhiteSpace(font)) font = "12";
            int index = Convert.ToInt32(dr["map_data_col_idx"] == null ? "0" : dr["map_data_col_idx"].ToString());
            string table_name = "";// dr["type_name"] == null ? "" : dr["type_name"].ToString(); 
            string cols_str = "";
            string gravity = dr["channel_index"] == null ? "0" : dr["channel_index"].ToString();
            string is_block_str = dr["show_grid"] == null ? "N" : dr["show_grid"].ToString();
            string time_check = "";
            string[] theme_color = !(dr["color1"] == null || dr["color2"] == null || dr["color3"] == null || string.IsNullOrWhiteSpace(dr["color1"].ToString()) || string.IsNullOrWhiteSpace(dr["color2"].ToString()) || string.IsNullOrWhiteSpace(dr["color3"].ToString()))
                ? new string[] { dr["color1"].ToString(), dr["color2"].ToString(), dr["color3"].ToString() }
                : get_default_theme_color_hec();
            if (index == 0)
            {
                if (dm.Tables[index].Rows.Count > 0)
                {
                    cols_str = "姓名;日期;地點;交通";
                    table_name = "覆診" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                    time_check = "";// "1@yyyy/MM/dd hh:mm a;";
                    List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                        {
                            new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd hh:mm a" },
                        };
                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = showAsBedName ? row["bedName"].ToString() : row["client_name"].ToString();
                        string date_str = row["revisit_planned_date"].ToString() + "\n" + row["revisit_planned_time"].ToString();
                        string sex_str = row["sex"].ToString();
                        string picture_id_str = row["photo_id"].ToString();

                        string client_id_str = row["client_id"].ToString();
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }
                        string address = string.Format(row["addr_org_chi_name"].ToString() + " " + row["specialties_code"].ToString());
                        string tran = string.Format(row["transport"].ToString() + " " + row["revisit_remark"].ToString());
                        string[] cont = new string[] { client_name_str, date_str, address, tran };
                        client_list.Add(new panel_nursing_client_block()
                        {

                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_str,
                            picture_id = picture_id_str,
                            description1 = address,
                            description2 = tran
                            ,
                            contents = cont
                        }
                        );
                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                }
            }
            else if (index == 1)
            {
                if (dm.Tables[index].Rows.Count > 0)
                {
                    //time_check = "1@yyyy/MM/dd hh:mm a;";
                    time_check = "";
                    List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                        {
                            new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "N", date_format = "yyyy/MM/dd hh:mm a" },
                        };
                    cols_str = "姓名;入院日期;地點;床位";
                    table_name = "急症" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = showAsBedName ? row["bedName"].ToString() : row["client_name"].ToString();
                        string sex_str = row["sex"].ToString();
                        string picture_id_str = row["client_photo_id"].ToString();
                        string date_str = row["ae_in_date"].ToString() + "\n" + row["ae_in_time"].ToString();

                        string client_id_str = row["client_id"].ToString();


                        string address = string.Format(row["addr_org_chi_name"].ToString());
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }
                        string reason = string.Format("DC;急症原因;{0}", row["ae_in_reason"].ToString());
                        string bed = string.Format(row["ae_in_bed_num"].ToString());
                        //string accompany = string.Format("PP;陪診員:{0}", row[11].ToString());

                        string[] cont = new string[] { client_name_str, date_str, address, bed };
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_str,
                            picture_id = picture_id_str,
                            description1 = address,
                            description2 = bed,
                            contents = cont

                        }
                        );
                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));

                }
            }
            else if (index == 2)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_event_grid_table(" and per.active_status = 'Y' and rev.event_status  ='預約中'" +
                // " and eve.dash_shown = 'Y' " +
                //  " and ( date(rev.event_planned_datetime) <= (date(now()) + INTERVAL (eve.reminder_day-1) DAY ))  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    cols_str = "姓名;日期;時間;備註";
                    table_name = "";
                    time_check = "";// "1@yyyy/MM/dd;";
                    List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd" },
                    };
                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        if (table_name != row[0].ToString() && client_list.Count > 0)
                        {
                            dash_list.Add(get_nuring_event(table_name + string.Format(" - {0}項", client_list.Count.ToString()), font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                            client_list = new List<panel_nursing_client_block>();
                            table_name = row[0].ToString();
                        }
                        if (table_name != row[0].ToString())
                        {
                            table_name = row[0].ToString();
                        }

                        string client_id_str = row[4].ToString();
                        string client_name = showAsBedName ? row["bedName"].ToString() : row["client_name"].ToString();
                        string sex_str = row["sex"].ToString();

                        string date = string.Format(row["event_planned_date"].ToString());
                        string time_str = string.Format(row["event_planned_time"].ToString());
                        string state = string.Format(row["event_status"].ToString());
                        string picture_id_str = row["photo_id"].ToString();
                        string[] cont = new string[] { client_name, date, time_str, state };
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date,
                            picture_id = picture_id_str,
                            description1 = time_str,
                            description2 = state,
                            contents = cont
                            //            client_name = row[3].ToString(),
                            //             sex = row[2].ToString(),
                            //             client_id = row[4].ToString(),
                            //             date = row[6].ToString(),
                            //             client_picture_id = row[12].ToString(),
                            //             contents = new string[] { time_str, state }
                        });

                    }
                    dash_list.Add(get_nuring_event(table_name + string.Format(" - {0}項", client_list.Count.ToString()), font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));

                }

            }
            else if (index == 3)
            {
                if (dm.Tables[index].Rows.Count > 0)
                {

                    time_check = "";// "1@yyyy/MM/dd;";

                    List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd" },
                    };
                    //   is_block_str = ds.Tables[0].Rows[0][9].ToString() == "0" ? "Y" : "N";
                    cols_str = "姓名;預計入住日期;床位;評核";
                    table_name = "訂位" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());

                    //    sql.Append("SELECT a.book_id, a.book_ref_num,concat(b.chi_surname, b.chi_name),b.hkid,concat(k.tchi_value,'-',z.tchi_value,'-',d.tchi_value) ,date_format(a.book_planned_entry_date,'%Y/%m/%d'),a.book_status, ");
                    //     sql.Append(" ifnull(c.total_balance,'0.00') ,b.client_number,if( date_format(b.entry_date,'%Y/%m/%d')='1900/05/16','', date_format(b.entry_date,'%Y/%m/%d')),concat(k2.tchi_value,'-',z2.tchi_value,'-',d2.tchi_value) ");
                    //      sql.Append(", b.sex ,b.dob ,b.assessment_result ,doc.client_photo_id,b.client_id,");

                    //      sql.Append(" a.book_buy_type ,a.book_date ,");
                    //      sql.Append(" a.book_contact_name, a.book_contact_num, a.book_contact_type,a.book_remark, concat(b.eng_surname, b.eng_name)");

                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = row[2].ToString() + string.Format("({0})", (row[11].ToString().Equals("M") ? "男" : "女"));

                        string sex_str = row[11].ToString();
                        string picture_id_str = row[14].ToString();
                        string date_st = row[5].ToString();
                        string client_id_str = row[15].ToString();
                        string assess = assessment_result_to_eng(row[13].ToString());
                        string address = string.Format(row[7].ToString());
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }

                        string bed = string.Format(row[4].ToString());

                        string[] cont = new string[] { client_name_str, date_st, bed, assess };
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_st,
                            picture_id = picture_id_str,
                            description1 = bed,
                            description2 = assess,
                            contents = cont

                        }
                        );
                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));

                }
            }
            else if (index == 4)
            {
                if (dm.Tables[index].Rows.Count > 0)
                {
                    cols_str = "姓名;日期;ICP類別;狀態";
                    table_name = "ICP" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                    List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd" },
                    };
                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = showAsBedName ? row["bedName"].ToString() : row[1].ToString();
                        string date_str = row["icp_date"].ToString();
                        string sex_str = row["sex"].ToString();
                        string picture_id_str = row["client_photo_id"].ToString();

                        string client_id_str = row[2].ToString();
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }

                        string status = row["status_chi"].ToString();
                        string icpType = row["assess_type"].ToString();
                        string[] cont = new string[] { client_name_str, date_str, icpType, status };
                        client_list.Add(new panel_nursing_client_block()
                        {

                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_str,
                            picture_id = picture_id_str,
                            description1 = icpType,
                            description2 = status
                            ,
                            contents = cont
                        }
                        );
                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                }
            }
            else if (index == 5)
            {
                if (dm.Tables[index].Rows.Count > 0)
                {
                    time_check = "";
                    List<panel_time_check> panel_time_check_list = new List<panel_time_check>()
                    {
                        new panel_time_check(){ column_idx = "1", isCurrent = "Y", isExpried = "N", date_format = "yyyy/MM/dd" },
                        new panel_time_check(){ column_idx = "2", isCurrent = "Y", isExpried = "Y", date_format = "yyyy/MM/dd" },
                    };
                    cols_str = "姓名;外宿日期;預計回院;外宿原因;";
                    table_name = "外宿" + string.Format(" - {0}項", dm.Tables[index].Rows.Count.ToString());
                    for (int i = 0; i < dm.Tables[index].Rows.Count; i++)
                    {
                        DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = showAsBedName ? row["bedName"].ToString() : row[1].ToString();
                        string sex_str = row["sex"].ToString();
                        string picture_id_str = row["client_photo_id"].ToString();

                        string date_str = row["leave_out_datetime"].ToString();
                        string plan_return_str = row["leave_planned_return_datetime"].ToString();
                        string client_id_str = row["client_id"].ToString();


                        string reason = string.Format(row["reason_name"].ToString());
                        if (picture_id_str.Length == 0)
                        {
                            picture_id_str = sex_str;
                        }
                        string[] cont = new string[] { client_name_str, date_str, plan_return_str, reason };
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name_str,
                            id = client_id_str,
                            datetime = date_str,
                            picture_id = picture_id_str,
                            description1 = plan_return_str,
                            description2 = reason,
                            contents = cont

                        }
                        );

                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, panel_time_check_list.ToArray(), client_list.ToArray()));
                }
            }
            else if (index == 6)
            {
                // DataSet ds = share.GetDataSource(Cmedical.get_ae_brief("and per.active_status = 'Y' and ae.ae_status  = '留醫中'  "));
                if (dm.Tables[index].Rows.Count > 0)
                {
                    cols_str = "入住人數;男/女;CA/NH;入住醫院";
                    table_name = "院舍統計";
                    time_check = "";
                    //    sql.Append("SELECT a.book_id, a.book_ref_num,concat(b.chi_surname, b.chi_name),b.hkid,concat(k.tchi_value,'-',z.tchi_value,'-',d.tchi_value) ,date_format(a.book_planned_entry_date,'%Y/%m/%d'),a.book_status, ");
                    //     sql.Append(" ifnull(c.total_balance,'0.00') ,b.client_number,if( date_format(b.entry_date,'%Y/%m/%d')='1900/05/16','', date_format(b.entry_date,'%Y/%m/%d')),concat(k2.tchi_value,'-',z2.tchi_value,'-',d2.tchi_value) ");
                    //      sql.Append(", b.sex ,b.dob ,b.assessment_result ,doc.client_photo_id,b.client_id,");

                    //      sql.Append(" a.book_buy_type ,a.book_date ,");
                    //      sql.Append(" a.book_contact_name, a.book_contact_num, a.book_contact_type,a.book_remark, concat(b.eng_surname, b.eng_name)");
                    int ae_client, current_client,
                      medium, high, other, male, female;
                    ae_client = current_client =
                        medium = high = other = male = female = 0;
                    foreach (DataRow row in dm.Tables[index].Rows)
                    {
                        current_client = current_client + int.Parse(row[2].ToString());
                        male = male + int.Parse(row[3].ToString());
                        female = female + int.Parse(row[4].ToString());
                        ae_client = ae_client + int.Parse(row[5].ToString());
                        medium = medium + int.Parse(row[6].ToString());
                        high = high + int.Parse(row[7].ToString());
                        other = other + int.Parse(row[8].ToString());



                        //Cshare.trace_value(row[1].ToString() + row[2].ToString());
                        //client_analysis_lab[row_index++].Text = row[1].ToString();
                        // client_analysis_lab[row_index++].Text = row[2].ToString();

                        //tableLayoutPanel20.Controls.Add(new Label { Text = row[2].ToString(), Anchor = AnchorStyles.Left, AutoSize = true }, 0, row_index);
                        //tableLayoutPanel20.Controls.Add(new Label { Text = row[1].ToString(), Anchor = AnchorStyles.Left, AutoSize = true }, 1, row_index++);

                    }

                    /*'

                    中度機能受損
                        高度機能受損
                        其他
                        男院友
                       女院友
                    int i = 0;
                    DataRow row = ds.Tables[0].Rows[0];
                    lab_current_client_num.Text = row[i++].ToString();
                    lab_current_live_client.Text = row[i++].ToString();
                    lab_sub_respite.Text = row[i++].ToString();
                    lab_non_sub_respite.Text = row[i++].ToString();
                    lab_sub_long_term.Text = row[i++].ToString();
                    lab_non_sub_long_term.Text = row[i++].ToString();


                    lab_buy_type.Text = row[i++].ToString();
                    lab_non_buy_type.Text = row[i++].ToString();



                    lab_other_long_term.Text = row[i++].ToString();
                    lab_medium_client.Text = row[i++].ToString();
                    lab_high_client.Text = row[i++].ToString();
                    lab_other_level_client.Text = row[i++].ToString();
                    lab_num_of_male_client.Text = row[i++].ToString();
                    lab_num_of_female_client.Text = row[i++].ToString();
                    */

                    //  DataRow color_row = ANICshare.Get_dataset_row_by_id(ds.Tables[1], 0, ds.Tables[0].Rows[0][18].ToString());
                    for (int i = 0; i < 1; i++)
                    {
                        //  DataRow row = dm.Tables[index].Rows[i];
                        string client_name_str = string.Format("{0}人", current_client.ToString());


                        string picture_id_str = "D";


                        //string[] timearray = row[5].ToString().Split(' ');
                        string date_st = string.Format("{0}/{1}人", male.ToString(), female.ToString());
                        //    string date_ed = row[4].ToString();
                        string bed = string.Format("{0}/{1}人", medium.ToString(), high.ToString());
                        string assess = string.Format("{0}人", ae_client.ToString());


                        string client_id_str = "0";


                        string[] cont = new string[] { client_name_str, date_st, bed, assess };
                        client_list.Add(new panel_nursing_client_block()
                        {
                            name = client_name_str,
                            //sex = sex_str,
                            id = client_id_str,
                            datetime = date_st,
                            picture_id = picture_id_str,
                            description1 = bed,
                            description2 = assess,
                            contents = cont

                        }
                        );
                    }
                    dash_list.Add(get_nuring_event(table_name, font, theme_color, gravity, cols_str, is_block_str, time_check, client_list.ToArray()));

                }
            }


            return dash_list;

        }

        private string assessment_result_to_eng (string str)
        {
            if (str.Equals("高度機能受損"))
            {
                return "NH";
            }
            else if (str.Equals("中度機能受損"))
            {
                return "C&A";
            }
            return str;

        }
        private panel_nursing_block get_nuring_event(string table_name, string font ,string []theme,  string gravity_str, string cols,  string is_block_str, string time_check_str, panel_nursing_client_block[] client_list)
        {
            for (int i = 0; i < client_list.Length; i++)
            {
 
                client_list[i].id = share.encry_value(client_list[i].id);
                client_list[i].picture_id = share.encry_value(client_list[i].picture_id);
                client_list[i].name = share.encry_value(client_list[i].name);
                client_list[i].description1 = share.encry_value(client_list[i].description1);
                client_list[i].description2 = share.encry_value(client_list[i].description2);
                client_list[i].datetime = share.encry_value(client_list[i].datetime);
                client_list[i].contents = share.encry_value(client_list[i].contents);

            }
            table_name = share.encry_value(table_name  );
            cols = share.encry_value(cols);
            gravity_str = share.encry_value(gravity_str);
            is_block_str = share.encry_value(is_block_str);
            string dark_color = share.encry_value( theme[0]);
            string light_color = share.encry_value(theme[2]);
            string medium_color = share.encry_value(theme[1]);
            time_check_str = share.encry_value(time_check_str);
            string headerfont  = share.encry_value( (int.Parse(font)+2).ToString());
            string headercolor = share.encry_value(Color.White.ToArgb().ToString());

            panel_nursing_block board = new panel_nursing_block()
            {
                name = table_name,
                col_titles = cols,
                client_blocks = client_list ,
                gravity = gravity_str
                ,is_block= is_block_str,
                block_dark_color = dark_color,
                block_light_color = light_color,
                block_medium_color = medium_color

                ,content_font_size = headerfont
                ,
                header_font_size =headerfont,

                header_color = headercolor,
                time_check = time_check_str
                //  clients = client_list,
                //    accessible = accessible_str
            };
            return board;

        }

        private panel_nursing_block get_nuring_event(string table_name, string font, string[] theme, string gravity_str, string cols, string is_block_str, panel_time_check[] time_check_list, panel_nursing_client_block[] client_list)
        {
            for (int i = 0; i < client_list.Length; i++)
            {

                client_list[i].id = share.encry_value(client_list[i].id);
                client_list[i].picture_id = share.encry_value(client_list[i].picture_id);
                client_list[i].name = share.encry_value(client_list[i].name);
                client_list[i].description1 = share.encry_value(client_list[i].description1);
                client_list[i].description2 = share.encry_value(client_list[i].description2);
                client_list[i].datetime = share.encry_value(client_list[i].datetime);
                client_list[i].contents = share.encry_value(client_list[i].contents);

            }
            panel_time_check[] _time_check_list = new panel_time_check[time_check_list.Length];
            for (int i = 0; i < time_check_list.Length; i++)
            {
                _time_check_list[i] = new panel_time_check();
                _time_check_list[i].column_idx = share.encry_value(time_check_list[i].column_idx);
                _time_check_list[i].date_format = share.encry_value(time_check_list[i].date_format);
                _time_check_list[i].isCurrent = share.encry_value(time_check_list[i].isCurrent);
                _time_check_list[i].isExpried = share.encry_value(time_check_list[i].isExpried);
            }
            table_name = share.encry_value(table_name);
            cols = share.encry_value(cols);
            gravity_str = share.encry_value(gravity_str);
            is_block_str = share.encry_value(is_block_str);
            string dark_color = share.encry_value(theme[0]);
            string light_color = share.encry_value(theme[2]);
            string medium_color = share.encry_value(theme[1]);
            string headerfont = share.encry_value((int.Parse(font) + 2).ToString());
            string headercolor = share.encry_value(Color.White.ToArgb().ToString());

            panel_nursing_block board = new panel_nursing_block()
            {
                name = table_name,
                col_titles = cols,
                client_blocks = client_list,
                gravity = gravity_str
                ,
                is_block = is_block_str,
                block_dark_color = dark_color,
                block_light_color = light_color,
                block_medium_color = medium_color

                ,
                content_font_size = headerfont
                ,
                header_font_size = headerfont,

                header_color = headercolor,
                panel_time_check_list = _time_check_list
                //  clients = client_list,
                //    accessible = accessible_str
            };
            return board;

        }


        public public_panel_briefing get_public_display_info(string display_id)
        {
            display_id = share.dencry_value(display_id);
            //password = share.dencry_value(password);
 
    
            Encription encr = new Encription();

   
            DataSet ds = share.GetDataSource(panel.get_public_info_panel(string.Format("and panel.display_id = {0} and panel.valid = 'Y' ", display_id)));
            if (ds.Tables[0].Rows.Count > 0)
            {
 
                return get_public_panel_briefing(ds);
            }
            else
            {
                return null;
            }

        }
        private public_panel_briefing get_public_panel_briefing(DataSet ds)
        {


            List<panel_public_block> play_list = new List<panel_public_block>();

            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
            {


                DataRow row = ds.Tables[0].Rows[j];
                if (!row[8].ToString().Equals("0"))
                {

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        int index = 11;
                        DataRow record_row = ds.Tables[0].Rows[i];
                        //      object[] values = new object[] { record_row[index++].ToString(), record_row[index++].ToString(), record_row[index++].ToString(),
                        //           record_row[index++].ToString(), record_row[index++].ToString(),record_row[index++].ToString(),
                        //     record_row[index++].ToString(),record_row[index++].ToString(),
                        //            record_row[index++].ToString(),record_row[index++].ToString(),record_row[index++].ToString()};



                        //    sql.Append(" public_panel_id ,pub.content_id, ");
                        //     sql.Append(" content.content_record_id, ");
                        //     sql.Append(" type.tchi_value,content.content_type_id,  ");
                        //     sql.Append(" content.last_period,ifnull(food.menu_name,content.content),content.usage_id,content.header, content.footer,content.ordering  , ");
                        //     sql.Append(" content.landscape_image_id, content.portrait_image_id ");
                        //              public String content_type_id;
                        // public String content;
                        //  public String period;
                        //   public String shown_header;
                        //   public String shown_footer;
                        //    public food_menu[] menu;
                        //    public String block_dark_color;
                        //    public String block_light_color;
                        //    public String block_medium_color;
     
                        panel_public_block block = new panel_public_block();
                        block.content_type_id = record_row[index++].ToString();
                        block.period = share.encry_value( record_row[index++].ToString());
                        block.content = share.encry_value(record_row[index++].ToString());
                        string usage_id = record_row[index++].ToString();
                        block.shown_header = share.encry_value(record_row[index++].ToString());
                        block.shown_footer = share.encry_value(record_row[index++].ToString());
                        index++;
                        string land_image_id = record_row[index++].ToString();
                        string por_image_id = record_row[index++].ToString();
            
                        //   block.image = record_row[index++].ToString();
                        //   block.content = record_row[index++].ToString();
                        switch (int.Parse(block.content_type_id))
                        {
                            case 0:
                                if (!usage_id.Equals("0"))
                                {
                                   // DataSet food_ds = share.GetDataSource(panel.get_food_menu_content_brief(
                                 //       string.Format("and curdate() between   date(menu.start_datetime) and  date(end_datetime)  group by menu.menu_id  having  count(ct.content_id)>1 and  menu.menu_id = {0} ", usage_id)));
                                    DataSet food_ds = share.GetDataSource(panel.get_food_menu_content_panel_brief(
                            string.Format("and menu.menu_id = {0} and DATE_FORMAT(DATE_ADD(menu.start_datetime, INTERVAL cont.ordering DAY),  '%Y/%m/%d') = curdate()  order by ordering  ", usage_id),
                            string.Format("and menu.menu_id = {0} and DATE_FORMAT(DATE_ADD(menu.start_datetime, INTERVAL cont.ordering DAY),  '%Y/%m/%d') = DATE_ADD(CURDATE(), INTERVAL 1 DAY)  order by ordering  ", usage_id)));

                                    //   grid_ds1 = setting.get_food_menu_grid_ds(grid_ds1, string.Format(" menu.menu_id = {0} and DATE_ADD(menu.start_datetime, INTERVAL cont.ordering DAY) = curdate() order by ordering ", ""));

                                    block.menu = reformat_food_menu_ds(food_ds);
                                }
                                break;
                            case 1:
                                block.content = share.encry_value(string.Format("{0};{1}",land_image_id,por_image_id))  ;
                                break;
                        }
                        block.content_type_id = share.encry_value(block.content_type_id);
                        play_list.Add(block);
                        //  grd_take_time.Rows.Add(values);
                    }
                }
            }


            string company_name_str = share.Get_mysql_database_value("company_contact_info", "company_id", "100000", "company_chi_name");
            if (company_name_str==null)
            {
                company_name_str = "";
            }

           // SELECT* FROM azure.company_contact_info;
           // company_id, company_chi_name, company_eng_name, company_region, company_chi_address, company_eng_address, company_phone, company_fax, company_email, company_url
            
                string dark_color = Color.FromArgb(21, 169, 157).ToArgb().ToString();
            string light_color = Color.FromArgb(240, 253, 252).ToArgb().ToString();
            string medium_color = Color.FromArgb(58, 228, 216).ToArgb().ToString();

            public_panel_briefing board = new public_panel_briefing()
            {
                company_name = share.encry_value(company_name_str),
                theme_dark_color = share.encry_value(dark_color),
                theme_light_color = share.encry_value(light_color),
                theme_medium_color = share.encry_value(medium_color),
                blocks
                  = play_list.ToArray(),
               // scroll_speed = share.encry_value(scroll_speed)

            };

            return board;

        }
        public food_menu[] reformat_food_menu_ds(  DataSet ds)
        {
            CPanel panel = new CPanel();

        
            string[] display_strings = new string[] { "早餐", "午餐", "湯", "水果", "下午茶", "晚餐", "宵夜", "明天早餐" };
            string[] food_array = new string[] { "", "", "", "", "", "", "", "" };
            string id = "";
            string table_name = "";

            if (ds.Tables[0].Rows.Count>0)
            {
                int a = 0;
                DataRow foodrow = ds.Tables[0].Rows[0];
                id = foodrow[0].ToString();
                table_name = foodrow[10].ToString();

                food_array[a++] = foodrow[3].ToString();
                food_array[a++] = foodrow[4].ToString();
                food_array[a++] = foodrow[5].ToString();
                food_array[a++] = foodrow[6].ToString();
                food_array[a++] = foodrow[7].ToString();
                food_array[a++] = foodrow[8].ToString();
                food_array[a++] = foodrow[9].ToString();
   
            }

            if (ds.Tables[1].Rows.Count > 0)
            {
                int a = 0;
                DataRow foodrow = ds.Tables[1].Rows[0];
                food_array[7] = foodrow[3].ToString();
 
            }
            if (id.Length > 0)
            {
                return reformat_food_menu_datatable(id, table_name, display_strings, food_array);
            }

            else
            {

                return reformat_food_menu_datatable(id, table_name, display_strings, food_array);
               // return null;
            }
            /*
            return reformat_food_menu_datatable(row[0].ToString(), row[1].ToString(), display_strings, food_array);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {


                DataRow row = ds.Tables[0].Rows[i];

                if (now.ToString(ANICshare.calandar_dateformat)== row[2].ToString())
                {

                }

                string jump_index = row[3].ToString();

                DataSet foodds = share.GetDataSource(panel.get_food_menu_content_brief(string.Format(" and menu_id = {0}  ", row[0].ToString())));

                if (jump_index.Length > 0)
                {
                     
                 
                    for (int a = 0; a < foodds.Tables[0].Rows.Count; a++)
                    {
                        DataRow foodrow = foodds.Tables[0].Rows[a];
                     
                        if (jump_index.Equals(foodrow[2].ToString()))
                        {
                            int next_index = a;
                            next_index = next_index + 1> foodds.Tables[0].Rows.Count?0: next_index + 1;
                  


                            DataRow nextfoodrow = foodds.Tables[0].Rows[next_index];
                            string[] food_array = new string[] {
                                 foodrow[3].ToString(),
                            foodrow[4].ToString(),foodrow[5].ToString(),
                            foodrow[6].ToString(),foodrow[7].ToString(),
                            foodrow[8].ToString(),foodrow[9].ToString(),nextfoodrow[3].ToString()

                            };

                            return reformat_food_menu_datatable(row[0].ToString(), row[1].ToString(), display_strings, food_array);



                        }
                    }
   

                }
                else
                {
                    int next_index = 0;
                    next_index = next_index + 1 > foodds.Tables[0].Rows.Count ? 0 : next_index + 1;
              
                    DataRow nextfoodrow = foodds.Tables[0].Rows[next_index];
                    DataRow foodrow = foodds.Tables[0].Rows[0];


                    string[] food_array = new string[] {
                                 foodrow[3].ToString(),
                            foodrow[4].ToString(),foodrow[5].ToString(),
                            foodrow[6].ToString(),foodrow[7].ToString(),
                            foodrow[8].ToString(),foodrow[9].ToString(),nextfoodrow[3].ToString()

                            };

                    return (reformat_food_menu_datatable(row[0].ToString(), row[1].ToString(), display_strings, food_array));
                }
            }

            /*
            DataTable tab = new DataTable();
            string tablename = "豁免紙到期日";
            tab.TableName = tablename;
           // tab.Columns.Add("Image", typeof(Image));
            tab.Columns.Add("院友");
            tab.Columns.Add("到期日");


            tab.Columns.Add("key");
            tab.Columns.Add("field");
            tab.Columns.Add("BackColor");
            //fibroGrid1.BackColorColumn = "BackColor";

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                //sql.Append("select rev.revisit_id,IFNULL(concat(per.chi_surname,per.chi_name),''),rev.client_id,");
                //sql.Append("DATE_FORMAT(rev.revisit_planned_datetime, '%Y-%m-%d'),DATE_FORMAT(rev.revisit_planned_datetime, '%H:%i'),");
                // sql.Append("DATE_FORMAT(rev.revisit_actual_time, '%Y-%m-%d %H:%i'), ");
                // sql.Append("g.addr_org_chi_name,rev.specialties_code,cast(rev.revisit_event as char),rev.transport,concat( rev.revisit_accompany_type,' ',rev.revisit_accompany_name),");
                //  sql.Append("rev.revisit_remark,rev.revisit_status,per.sex,doc.document_photo   ");

                //地點 陪診員
                DataRow row = ds.Tables[0].Rows[i];
                tab.Rows.Add( 
                    row[1].ToString(), row[3].ToString(), row[2].ToString(),
                    "EXM", Cshare.get_grid_color_states(DateTime.ParseExact(row[3].ToString(), Cshare.sql_dateformat, null))
                    );
                //CYuanList.get_client_photo
                if (i == ds.Tables[0].Rows.Count - 1)
                {
                    new_ds.Tables.Add(tab);
                }

            }
            */
            return null;
            /*
            obj[0] = obj[0].ToString();
            obj[1] = obj[1].ToString();
            obj[2] = obj[2].ToString();
            obj[3] = obj[3].ToString().Replace(";", "\n");
            obj[4] = obj[4].ToString().Replace(";", "\n");
            obj[5] = obj[5].ToString();
            obj[6] = obj[6].ToString().Replace(";", "\n");
            obj[7] = obj[9].ToString() == "已完成" ? obj[7].ToString() : "";
            //obj[7] = obj[7].ToString();
            obj[8] = obj[8].ToString().Replace(";", "\n");
            obj[9] = obj[9].ToString();
            //obj[10] = obj[10].ToString();
            //obj[11] = obj[11].ToString();
            */
            // return obj;
        }
        public food_menu[] reformat_food_menu_datatable(string id, string tablename, string[] first_row, string[] second_row)
        {



            food_menu[] menus = new food_menu[first_row.Length];
 
            for (int i = 0; i < menus.Length; i++)
            {
                food_menu menu = new food_menu();
                menu.id = share.encry_value("0");
                menu.title = share.encry_value(first_row[i]);
                menu.content = share.encry_value(second_row[i]);
                menu.image_id = share.encry_value("NO");
                menu.content_font_size =  share.encry_value(check_food_content_size(second_row[i]));
                menus[i] = menu; ;
                // tab.Rows.Add(ResANIImage.task_icon_7, first_row[i], second_row[i], id, "MNU", Color.FromArgb(240, 253, 252).ToArgb());
            }

            //fibroGrid1.BackColorColumn = "BackColor";


            return menus;


        }

        public string check_food_content_size(string str)
        {
            if (str.Length>10)
            {
                return "26";
            }
            else
            {
                return "36";
            }




        }
        public string get_color(string values)
        {
            string[] str = values.Split(',');
            string  color = (Color.FromArgb(int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2])).ToArgb().ToString());
 
            return   color   ;
        }
        public string [] get_default_theme_color()
        {
            string dark_color = (Color.FromArgb(21, 169, 157).ToArgb().ToString());
            string light_color =Color.FromArgb(240, 253, 252).ToArgb().ToString();
            string medium_color = (Color.FromArgb(58, 228, 216).ToArgb().ToString());
            return new string[] { dark_color, medium_color, light_color };
        }
        public string[] get_default_theme_color_hec()
        {
            string dark_color = "#c79f6a";
            string medium_color = "#d8bc96";
            string light_color = "#f1e9dc";
            return new string[] { dark_color, medium_color, light_color };
        }
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
                if (right[i] == "Y")
                {
                    if (i == 0)
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
        private List<dash_board_event> get_dash_event(int index, List<dash_board_event> dash_list)
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
                DataSet ds = share.GetDataSource(Cmedical.get_revisit_brief("and per.active_status = 'Y' and rev.revisit_status  = '預約中' and date( rev.revisit_planned_datetime) <= CURDATE() + INTERVAL 1 DAY ", false));
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
                        string accompany = string.Format("PP;陪診員;{0}", row["revisit_remark"].ToString());


                        /*
                        string client_name_str = row[1].ToString();
                        string date_str = row[3].ToString() + " " + row[4].ToString();
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
                            contents = new string[] { address, spe, tran, accompany }
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
                        string date_str = row["ae_in_date"].ToString() + "\n" + row["ae_in_time"].ToString();

                        string client_id_str = row["client_id"].ToString();


                        string address = string.Format("AD;地點;{0}", row["addr_org_chi_name"].ToString());

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
                            contents = new string[] { address, reason, bed }
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
                                contents = new string[] { time_str, state }
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
                if (client_list[i].client_picture_id.Length == 0)
                {
                    client_list[i].client_picture_id = "0";
                }

                client_list[i].client_id = share.encry_value(client_list[i].client_id);
                client_list[i].client_picture_id = share.encry_value(client_list[i].client_picture_id);
                client_list[i].client_name = share.encry_value(client_list[i].client_name);
                client_list[i].sex = share.encry_value(client_list[i].sex);
                client_list[i].date = share.encry_value(client_list[i].date);

                for (int a = 0; a < client_list[i].contents.Length; a++)
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
        public client_id_in_nfc get_client_id_in_nfc(string nfc_id)
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
                client_id_str = share.Get_mysql_database_value("company_bed", "bed_id", label_usage_id_str, "client_id");
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
                        empty_bed = da.Tables[0].Rows[0].ToString();
                    }


                    client_briefing empty_brief = new client_briefing
                    {



                        //label_usage_code = label_usage_code_str,
                        //label_usage_id = label_usage_id_str,
                        name = "吉床位",
                        sex = "",
                        age = "",
                        bednum = empty_bed,
                        //relative = relative_name_str,
                        remark = "",
                        client_id = "0",
                        //picturename = picturenamestr
                        personal_ability = "",
                        birth_date = "",
                        wound_exist = "",
                        absence_list = null,
                        meal_type = "",
                        picture_id = "",
                        assessment_result = "",
                        drug_ADR = "",
                        drug_allergic = "",
                        other_allergic = "",
                        disease = ""


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
            if (ds.Tables[0].Rows.Count > 0)
            {

                int i = 0;
                DataRow row = ds.Tables[0].Rows[0];
                namestr = row[i++].ToString();
                sexstr = row[i++].ToString().Equals("M") ? "男" : "女";
                bday = DateTime.ParseExact(row[i++].ToString(), "yyyy/MM/dd", null);
                birth_datestr = bday.ToString("yyyy/MM/dd");

                bedloc = row[i++].ToString();
                personal_ability_str = row[i++].ToString();

                wound_exist_str = row[i++].ToString().Length > 0 ? "Y" : "N";

                absence_str = row[i++].ToString();

                meal_Type_str = row[i++].ToString();
                pictureid = row[i++].ToString();
                if (pictureid.Length == 0)
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
                other_allergic_str = ds.Tables[1].Rows[0][9].ToString() + (ds.Tables[1].Rows[0][10].ToString().Length > 0 ? " " + ds.Tables[1].Rows[0][10].ToString() : "");

                disease_str = ANICshare.add_comma(ds.Tables[1].Rows[0][4].ToString()) + ANICshare.add_comma(ds.Tables[1].Rows[0][5].ToString())
                    + ds.Tables[1].Rows[0][6].ToString();

                assessment_str = ds.Tables[1].Rows[0][11].ToString();



            }


            client_briefing brief = new client_briefing
            {
                name = share.encry_value(namestr),
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
        public string get_client_picture_id(string client_id)
        {
            client_id = share.dencry_value(client_id);
            string id = "0";
            DataSet dp = share.GetDataSource(Cclient.get_client_picture_id(client_id));
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
            if (current_id == null)
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
                    brief_arr[i] = ds.Tables[i].Rows[0][0].ToString();
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

            measure_briefing brief = new measure_briefing()
            {
                blood_pressure = share.encry_value(brief_arr[0]),
                blood_oxygen = share.encry_value(brief_arr[1]),
                blood_glucose = share.encry_value(brief_arr[2]),
                respiration_rate = share.encry_value(brief_arr[3]),
                body_temperature = share.encry_value(brief_arr[4]),
                body_weight = share.encry_value(brief_arr[5]),
                blood_pressure_list = share.encry_value(blood_pressure_record),
                blood_oxygen_list = share.encry_value(blood_oxygen_record),
                blood_glucose_list = share.encry_value(blood_glucose_record),
                respiration_rate_list = share.encry_value(respiration_rate_record),
                temperature_list = share.encry_value(temperature_record),
                weight_list = share.encry_value(weight_record)

            };

            return brief;
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

                    client_state_arr.Add(share.encry_value(row[i++].ToString() == "留醫中" ? "H" : "N"));

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





        public Acc_charge_item get_acc_charge_item(string charge_item_id)
        {
            charge_item_id = share.dencry_value(charge_item_id);

            string package_name_str = "";
            string zero_boo = "N";
            DataSet ds = share.GetDataSource(CAccount.get_acc_charge_item(charge_item_id));
            if (ds.Tables[0].Rows.Count != 0)
            {

                float price = float.Parse(ds.Tables[0].Rows[0][1].ToString());
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
                ,
                is_zero = share.encry_value(zero_boo)
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

                    result = share.encry_value(str)
                };
                return insertstate;
            }
            else
            {
                return null;
            }
        }
        public insert_acc_data post_charge_item(String client_id, String charge_item_id, String quantity, String timestamp, String handlingperson, String charge)
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
        public medicine_photo get_medicine_photo(string medicine_photo_id)
        {
            medicine_photo_id = share.dencry_value(medicine_photo_id);
            byte[] imgData = null;
            if (medicine_photo_id == "0")
            {
                var ms = new MemoryStream();

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


            medicine_photo med_photo = new medicine_photo
            {

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


        public panel_pictureinfo get_person_image(string picture_id)
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

            if (imgData != null)
            {
                panel_pictureinfo image =
new panel_pictureinfo()
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
        public panel_pictureinfo get_company_image(string picture_id)
        {
            picture_id = share.dencry_value(picture_id);
            byte[] imgData = null;
            if (picture_id == "0")
            {
                return null;
            }



            DataSet ds = share.GetDataSource(CRoom.get_company_photo(picture_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                imgData = (byte[])row[0];
            }

            if (imgData != null)
            {
                panel_pictureinfo image =
new panel_pictureinfo()
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
        public panel_pictureinfo get_poster_image(string poster_id)
        {
            poster_id = share.dencry_value(poster_id);
            byte[] imgData = null;
            if (poster_id == "0")
            {
                return null;
            }



            DataSet ds = share.GetDataSource(CPanel.get_poster_photo(poster_id));
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                imgData = (byte[])row[0];
            }

            if (imgData != null)
            {
                panel_pictureinfo image =
new panel_pictureinfo()
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
        public medicine_records get_medicine_records(string client_id)
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
                    volume_unit_str.Add(row[i++].ToString());
                    medicine_period_str.Add(row[i++].ToString());
                    medicine_take_interval_str.Add(row[i++].ToString());

                    each_take_type_str.Add(row[i++].ToString().Replace("#N", row[i++].ToString()));
                    medicine_taking_method_str.Add(row[i++].ToString());

                    medicine_report_type_id_str.Add(row[i++].ToString());


                    medicine_taking_remark_str.Add(row[i++].ToString());

                    medicine_photo_id_str.Add(row[i++].ToString());

                    medicine_bag_id_str.Add(row[i++].ToString());

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
                new medicine_records()
                {

                    medicine_name = share.encry_value(medicine_name_str),
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


        public medicine_description get_medicine_description(String take_medicine_id)
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

                take_begin_date_str = row[3].ToString();
                medicine_source_str = row["addr_org_chi_name"].ToString();
                specialty_code_str = row["specialties_code"].ToString();
                medicine_PRN_str = row[10].ToString();
                //script_medicine_str = row["script_medicine"].ToString();
                //need_distribute_str = row["need_distribute"].ToString();

                first_check_id_str = row["first_check_id"].ToString();
                second_check_id_str = row["second_check_id"].ToString();


            }



            medicine_description des =
                new medicine_description()
                {


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

                    medicine_id = share.encry_value(row[0].ToString()),
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
                    medicine_report_type = share.encry_value(row[13].ToString()),
                    medicine_taking_remark = share.encry_value(row[14].ToString()),
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


                return record;
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
                    int i = 0;
                    reconsult_id_str = row[i++].ToString();


                    reconsult_planned_datetime_str = row[i++].ToString();
                    reconsult_event_str = row[i++].ToString().ToString().Replace(";", ",");

                    hospital_addr_id_str = row[i++].ToString();
                    specialties_code_str = row[i++].ToString();

                    transport_str = row[i++].ToString();
                    reconsult_accompany_type_str = row[i++].ToString();
                    reconsult_event_str = reconsult_event_str + " " + row[i++].ToString();
                    string addr = row[i++].ToString();
                    hospital_addr_code_str = addr == "" ? " " : addr;

                    string[] reconsult_arr = new string[] { reconsult_id_str, reconsult_planned_datetime_str, reconsult_event_str, transport_str,
                reconsult_accompany_type_str,specialties_code_str,hospital_addr_id_str,hospital_addr_code_str};


                    string reconsult_arr_str = string.Join(";", reconsult_arr);
                    reconsult_record_list.Add(reconsult_arr_str);

                }

            }
            medical_revisit_records record = new medical_revisit_records
            {

                revisit_record = share.encry_value(reconsult_record_list)

            };


            return record;

        }


        public medical_events get_medical_event(string client_id)
        {
            client_id = share.dencry_value(client_id);
            StringBuilder sql = new StringBuilder();
            sql.Append(Cmedical.get_ae_brief("and ae.client_id = " + client_id + " and ae.ae_status = '留醫中' "));
            sql.Append(Cmedical.get_revisit_brief("and rev.client_id = " + client_id + " and rev.revisit_status = '預約中' ", false));
            sql.Append(Cmedical.get_event_grid_table("and ( rev.client_id = " + client_id + " and rev.event_status = '預約中' )" +
                " or rev.event_id in (SELECT  eve2.event_id FROM (select * from medical_event eve3 WHERE eve3.client_id = " +
                client_id + " AND eve3.event_status = '已完成' ORDER BY eve3.event_actual_time DESC LIMIT 3) AS eve2) ",
                false, "FIELD(rev.event_status, '預約中') desc, "));

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



                    string location = share.encry_value(row[6].ToString());
                    string date_str = share.encry_value(row[3].ToString() + " " + row[4].ToString());
                    string description = share.encry_value(row[7].ToString() + ANICshare.add_space(row[8].ToString()));
                    //交通 陪診 備註
                    string transport = string.Format("交通:{0}", row[9].ToString());
                    string accompany = string.Format("陪診:{0}", row[10].ToString());
                    string remark = string.Format("備註:{0}", row[11].ToString());

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

                        string date_str = share.encry_value(row[6].ToString());
                        string state = share.encry_value(row[10].ToString());

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
                            title = date_str,
                            status = state
                        }
                        );
                    }
                    else
                    {

                        string date_str = share.encry_value(row[6].ToString());
                        string state = share.encry_value(row[10].ToString());


                        client_list.Add(new medical_content()
                        {
                            title = date_str,
                            status = state
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
            DataSet ds = share.GetDataSource(Cmedical.get_blood_pressure_records(client_id, int.Parse(time_mode)));


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
            timestamp = share.encry_value(timestamp_str),
            systolic = share.encry_value(systolic_str),
            diastolic = share.encry_value(diastolic_str),
            pulse = share.encry_value(pulse_str),
            handlingperson = share.encry_value(handlingperson_str)
        };
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
            DataSet ds = share.GetDataSource(Cmedical.get_blood_glucose_records(client_id, int.Parse(time_mode)));
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

        public blood_oxygen_records get_blood_oxygen_records(string client_id, string time_mode)
        {
            client_id = share.dencry_value(client_id);
            time_mode = share.dencry_value(time_mode);
            List<string> oxygen_str = new List<string>();
            //String medicine_name_str = "";
            List<string> handlingperson_str = new List<string>();
            List<string> timestamp_str = new List<string>();
            DataSet ds = share.GetDataSource(Cmedical.get_blood_oxygen_records(client_id, int.Parse(time_mode)));
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
            DataSet ds = share.GetDataSource(Cmedical.get_respiration_rate_records(client_id, int.Parse(time_mode)));
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
            DataSet ds = share.GetDataSource(Cmedical.get_temperature_records(client_id, int.Parse(time_mode)));
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

            temperature_records temperature =
        new temperature_records()
        {
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

            DataSet ds = share.GetDataSource(Cmedical.get_weight_records(client_id, int.Parse(time_mode)));
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
            handlingperson = handlingperson_str

        };
            return weight;
        }

        public insert_vital_data post_vital_data(String client_id, String value, String timestamp, String handlingperson, string measure_index)
        {
            client_id = share.dencry_value(client_id);

            value = share.dencry_value(value);
            timestamp = share.dencry_value(timestamp);
            handlingperson = share.dencry_value(handlingperson);
            measure_index = share.dencry_value(measure_index);

            int index = int.Parse(measure_index);
            string time = share.get_current_time();
            if (index == 0)
            {
                string result = "";
                string[] reading = value.Split(';');
                string[] values = new string[] { "", client_id, time, reading[0], reading[1], reading[2], handlingperson, time };
                values[0] = share.Get_mysql_database_MaxID(Cmedical.measuretable[index], "record_id");
                if (share.bool_Ecxe_transaction_Sql(Cmedical.insert_blood_presure(values)))
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
        /*
        public bool panel_ethernet_callback_panel_client(CCallBackThread cthd, string client_id, bool nursing_panel, int command = 51)
        {
            string bed_id = Cshare.Get_mysql_database_value("sys_company_bed", "client_id", client_id, "bed_id");
            if (!Cshare.panel_exist || bed_id.Length == 0 || Cshare.practice_mode)
            {
                return false;
            }


            return panel_ethernet_callback_panel_bed(cthd, bed_id, nursing_panel);

        }

        public bool panel_ethernet_callback_panel_bed(CCallBackThread cthd, string bed_id, bool nursing_panel, int command = 51)
        {
            //bed_id, zone_id, tchi_value, eng_value, book_id, client_id, valid
            //nfc_id, display_name, client_id, usage_code, usage_id, valid

            // sql.Append(Cclient.get_bed_panel_brief(string.Format("and panel.display_id = {0} ", display_id)));
            StringBuilder sql = new StringBuilder();

            CPanel panel = new CPanel();
            Cclient.get_bed_panel_brief(string.Format("and panel.display_id = {0} ", display_id);

            DataSet nfc_ds = share.GetDataSource (Cclient.get_bed_panel_brief(string.Format("and( bp.bed_id in ({0}))", bed_id)));

           // DataSet nfc_ds = panel.GET((string.Format("and( bp.bed_id in ({0}))", bed_id)));

           // DataSet room_ds = panel.roo(Cclient.GetClientRoom(string.Format("and( rb.bed_id in ({0}))", bed_id)));



            DataSet room_ds = share.GetDataSource(CRoom.get_room_panel_briefing(string.Format("and( rb.bed_id in ({0}))", bed_id)));
            int client = 100025;

            List<string> ip_list = new List<string>();

            List<string> port_list = new List<string>();

            for (int i = 0; i < nfc_ds.Tables[0].Rows.Count; i++)
            {
                DataRow row = nfc_ds.Tables[0].Rows[i];
                if (row[1].ToString().Length > 0)
                {

          
                    //   if (ip_list.Contains(row[1].ToString()))
                    //   {
                    ip_list.Add(row[1].ToString());
                    port_list.Add(row[3].ToString());
                    /*
                    int ip = cthd.IpStringToInt(row[1].ToString());
                    int port = int.Parse(row[3].ToString());
                    cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + i).ToString(), 10));
                    cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(command) }));
           
                }
                //   }
                // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
            }
            for (int i = 0; i < room_ds.Tables[0].Rows.Count; i++)
            {
                DataRow row = room_ds.Tables[0].Rows[i];
                if (row[1].ToString().Length > 0)
                {

                    Cshare.trace_value(row[1].ToString());
                    //   if (ip_list.Contains(row[1].ToString()))
                    //   {
                    ip_list.Add(row[1].ToString());
                    port_list.Add(row[3].ToString());
                    /*
                    int ip = cthd.IpStringToInt(row[1].ToString());
                    int port = int.Parse(row[3].ToString());
                    cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + i).ToString(), 10));
                    cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(command) }));
             
                }
                //   }
                // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
            }
            if (nursing_panel)
            {
                nfc_ds = panel.get_all_panel((string.Format(" and panel.page_index  = 2 and valid = 'Y' ")));
                for (int i = 0; i < nfc_ds.Tables[0].Rows.Count; i++)
                {
                    DataRow row = nfc_ds.Tables[0].Rows[i];
                    if (row[1].ToString().Length > 0)
                    {


                        //   if (ip_list.Contains(row[1].ToString()))
                        //   {
                        int ip = cthd.IpStringToInt(row[1].ToString());
                        int port = int.Parse(row[3].ToString());
                        cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + i).ToString(), 10));
                        cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(command) }));
                    }
                    //   }
                    // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                    // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
                }
            }
            if (ip_list.Count > 0)
            {
                for (int i = 0; i < ip_list.Count; i++)
                {
                    int ip = cthd.IpStringToInt(ip_list[i]);
                    int port = int.Parse(port_list[i]);
                    cthd.InsertCallBack(new CallBackInfo(ip_list[i], port, (client + i).ToString(), 10));
                    cthd.InsertExecCommand(new ExecCommand(ip_list[i], port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(command) }));

                }
                cthd.msgTo = panel.msgToFormEvent;
                cthd.RunCallBack();
            }


            return false;
        }
        static public bool panel_ethernet_callback_panel_nursing(CCallBackThread cthd, int command = 51)
        {
            //bed_id, zone_id, tchi_value, eng_value, book_id, client_id, valid
            //nfc_id, display_name, client_id, usage_code, usage_id, valid
            if (!Cshare.panel_exist || Cshare.practice_mode)
            {
                return false;
            }

            CDisplay_panel panel = new CDisplay_panel();
            DataSet nfc_ds = panel.get_all_panel((string.Format(" and panel.page_index  = 2 and valid = 'Y' ")));


            int client = 100025;


            for (int i = 0; i < nfc_ds.Tables[0].Rows.Count; i++)
            {
                DataRow row = nfc_ds.Tables[0].Rows[i];
                if (row[1].ToString().Length > 0)
                {


                    //   if (ip_list.Contains(row[1].ToString()))
                    //   {
                    int ip = cthd.IpStringToInt(row[1].ToString());
                    int port = int.Parse(row[3].ToString());
                    cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + i).ToString(), 10));
                    cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(command) }));
                }
                //   }
                // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
            }

            cthd.msgTo = panel.msgToFormEvent;
            cthd.RunCallBack();


            return false;
        }
        public bool panel_ethernet_callback_panel(CCallBackThread cthd, string oldCode, int command = 51)
        {
            //bed_id, zone_id, tchi_value, eng_value, book_id, client_id, valid
            //nfc_id, display_name, client_id, usage_code, usage_id, valid
            if (Cshare.practice_mode)
            {
                return false;
            }
            DataSet nfc_ds = get_panel_ds((string.Format("and display_id in ({0})", oldCode)), 0);
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
          
            int client = 100025;
            if (nfc_ds.Tables[0].Rows.Count > 0)
            {
                //   Cshare.trace_value(GetHostName(nfc_ds.Tables[0].Rows[0][1].ToString()));
            }

            //  List<string> ip_list = GetAllLocalMachines();

            if (command == 53)
            {

                for (int i = 0; i < nfc_ds.Tables[0].Rows.Count; i++)
                {


                    DataRow row = nfc_ds.Tables[0].Rows[i];
                    if (row[1].ToString().Length > 0)
                    {
                        //  cthd = new CCallBackThread();

                        //   if (ip_list.Contains(row[1].ToString()))
                        //   {
                        try
                        {
                            /*
                            int ip = cthd.IpStringToInt(row[1].ToString());
                            int port = int.Parse(row[3].ToString());
                            cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + i).ToString(), 10));
                            cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(command) }));
                            cthd.msgTo = msgToFormEvent;
                            cthd.RunCallBack();
                            System.Threading.Thread.Sleep(10000);
                         

                            int ip = cthd.IpStringToInt(row[1].ToString());
                            int port = int.Parse(row[3].ToString());
                            cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + i).ToString(), 10));
                            cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(command) }));

                        }
                        catch (Exception)
                        {
                            continue;
                        }


                    }
                    cthd.msgTo = msgToFormEvent;
                    cthd.RunCallBack();
                    //   }
                    // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                    // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
                }

            }
            else
            {
                for (int i = 0; i < nfc_ds.Tables[0].Rows.Count; i++)
                {
                    DataRow row = nfc_ds.Tables[0].Rows[i];
                    if (row[1].ToString().Length > 0)
                    {


                        //   if (ip_list.Contains(row[1].ToString()))
                        //   {
                        int ip = cthd.IpStringToInt(row[1].ToString());
                        int port = int.Parse(row[3].ToString());
                        cthd.InsertCallBack(new CallBackInfo(row[1].ToString(), port, (client + i).ToString(), 10));
                        cthd.InsertExecCommand(new ExecCommand(row[1].ToString(), port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, (byte)(command) }));
                    }
                    //   }
                    // cthd.InsertCallBack(new CallBackInfo(ip, lst[i].Port, (client + i).ToString(), 10));
                    // cthd.InsertExecCommand(new ExecCommand(lst[i].IP, lst[i].Port, Encoding.ASCII.GetBytes((client + i).ToString()), new byte[] { 0x01, 0x33 }));
                }
                cthd.msgTo = msgToFormEvent;
                cthd.RunCallBack();
            }

            return false;
        }
        */

        private string picture_id_check(string sex , string picrture_id)
        {
            if (picrture_id.Length > 0)
            {
                return picrture_id;
            }
            else if (picrture_id.Length == 0 && sex.Length > 0)
            {
                return sex;
            }
            else
            {
                return "D";
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



        public byte[] getdataset(String user, String pw, String sql, String is_test)
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
                byte[] bytes = ConvertDataSetToByteArray(ds);

                byte[] header = Encoding.UTF8.GetBytes(user);
                byte[] sufix = Encoding.UTF8.GetBytes(pw);

                byte[] new_bytes = new byte[bytes.Length + (header.Length + sufix.Length)];
                Buffer.BlockCopy(header, 0, new_bytes, 0, header.Length);
                Buffer.BlockCopy(bytes, 0, new_bytes, header.Length, bytes.Length);
                Buffer.BlockCopy(sufix, 0, new_bytes, header.Length + bytes.Length, sufix.Length);

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


                return Encoding.ASCII.GetBytes(eny.encrypt(i.ToString()));

            }
            catch (Exception)
            {

                return null;
            }

        }
        public byte[] ExecBoldSql(String user, String pw, String sql, String is_test, string image)
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



        private bool check_login(string name, string pw)
        {
            if (name.Equals(LoginName) && pw.Equals(LoginPass))
            {
                return true;

            }
            else
            {
                return false;
            }
        }




        public class client_id_in_nfc
        {
            public string label_usage_code;
            public string label_usage_id;
            public string client_id;

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
        public class panel_pictureinfo
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

        public class client_panel_briefing
        {

            public string night_mode;
            public string night_start;
            public string night_end;

            public string bed_id;
            public string client_id;

            public string responsible;

            public string care_logo;

            public string alarm;
            public string name;
            public string sex;
            public string age;
            public string birth_date;
            public string bednum;


            public string caution;


            public string remark;
            public string picture_id;

            public string meal_type;
            public string medical_briefing = "";
            public string personal_ability = "";
            public string incontinence = "";

           
            public string precaution = "";
            public string wound_exist = "";

            //public string absence = "";
            public List<string> absence_list = new List<string>();
            public List<string> account_items = new List<string>();
            public string assessment_result;
            public string drug_ADR = "";
            public string drug_allergic = "";
            public string food_allergic = "";
            public string other_allergic = "";
            public string disease = "";

            public string absence = "";
            public panel_block [] blocks;
        }
        public class room_panel_briefing
        {
            public string room_name;
            public string remark;
            public string company_picture_id;
            public string theme_dark_color;
            public string theme_medium_color;
            public string theme_light_color;
            public panel_block[] blocks;
        }
        public class nursing_panel_briefing
        {
            public string theme_dark_color;
            public string theme_medium_color;
            public string theme_light_color;

            public string scroll_speed;
       

            public string room_name;
            public string remark;
            public string company_picture_id;


            public string speed;

            public panel_nursing_block[] blocks;
        }
        public class panel_nursing_block
        {
            public string name;
            public string col_titles;
            public string header;
            public string header_color;
            public string header_font_size;
            public string col_titles_font_size;

            public string content_font_size;
            public string time_check;
            public string is_block;
            public string gravity;
            public panel_nursing_client_block[] client_blocks;
            public panel_time_check[] panel_time_check_list;

            public string block_dark_color;
            public string block_medium_color;
            public string block_light_color;
      

        }
        public class panel_nursing_client_block
        {
            public string id;
            public string sub_id;

            public string [] contents;

            public string name;
            public string datetime;
            public string description1;
            public string description2;

            public string title_font_size;

            public string picture_id;

        }

        // Raymond 20200917
        public class panel_time_check
        {
            public string column_idx;
            public string date_format;
            public string isCurrent;
            public string isExpried;
        }

        public class public_panel_briefing
        {
            public String theme_dark_color;
            public String theme_medium_color;
            public String theme_light_color;
            public String scroll_speed;
            public string company_name = "";
            public panel_public_block[] blocks;
        }
        public class panel_public_block
        {
            public String content_type_id;
            public String content;
            public String period;
            public String shown_header;
            public String shown_footer;
            public food_menu[] menu;
            public String block_dark_color;
            public String block_light_color;
            public String block_medium_color;

        }
        public class food_menu
        {
            public String id;
            public String title;
            public String content;
            public string content_font_size;
            public String image_id;
        }
        public class panel_block
        {
            public string id;
            public string sub_id;
            public string header;
            public string header_font_size;
            public string header_color;
            public string main_title;
            public string description;
 
            public string main_title_color;
            public string main_title_font_size;
            public string sub_title_font_size;
            public string is_full;

            public string picture_id;
            public string alarm;

            public string single;

        }
            public class client_account_briefing
        {
            public List<string> account_items = new List<string>();
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

        public class client_info
        {
            public string client_id;
            public string client_picture_id;
            public string client_name;
            public string sex;
            public string date;
            public string[] contents;


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



    }
}
