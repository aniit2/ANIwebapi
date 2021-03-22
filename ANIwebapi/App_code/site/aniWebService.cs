using ANI.code;
using System;
using System.Timers;
using System.Web.Services;
using System.Security.Cryptography;
using System.Diagnostics;

using System.Web;
using System.IO;
/// <summary>
/// Summary description for WebService2
/// </summary>

[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]


public class aniWebService : System.Web.Services.WebService
{

    #region ANImoblie 
    protected  bool have_panel =false;
    protected ANImoblie basadata = new ANImoblie(true);
    protected ANI_Display_panel panels = new ANI_Display_panel(true);
    protected Encription eny = new Encription();
    public aniWebService()
    {
        /*
        basadata.dbHost = dbHost;
        basadata.dbPort = dbPort;
        basadata.dbUser = dbUser;
        basadata.dbPass = dbPass;
        basadata.dbName = dbName;
        basadata.LoginName = LoginName;
        basadata.LoginPass = LoginPass;
        basadata.db_testName = db_testName;

        basadata.refresh_connection();

        Debug.Print("refred");
        */
    }
    [WebMethod]
    public ANImoblie.logining getlogin(string username, string password)
    {
        return basadata.getlogin(username, password);
    }

    [WebMethod]
    public ANImoblie.logining get_app_login(string username, string password, string device_id, string location)
    {
        return basadata.get_app_login(username, password,device_id,location);
    }
    [WebMethod]
    public ANImoblie.logining get_app_login2(string username, string password, string device_id, string location, string version)
    {
        return basadata.get_app_login2(username, password, device_id, location, version);
    }

    [WebMethod]
    public ANImoblie.logout get_app_logout(string username, string device_id)
    {
        return basadata.get_app_logout(username, device_id);
    }

    Timer acc_timer = new Timer();
    [WebMethod]
    public ANImoblie.ANI_server get_server_state(string test)
    {
        return basadata.get_server_state(test);
    }
    [WebMethod]
    public ANImoblie.dash_board get_dash_board(string username)
    {
        return basadata.get_dash_board(username);
    }

    [WebMethod]
    public ANImoblie.iot_ble_device_info get_iot_ble_device_info(string device_id)
    {
        return basadata.get_iot_ble_device_info(device_id);
    }

    [WebMethod]
    public ANImoblie.client_id_in_nfc  get_client_id_in_nfc(string nfc_id)
    {
        return basadata.get_client_id_in_nfc(nfc_id);
    }
    [WebMethod]
    public ANImoblie.client_id_in_nfc get_patrol_client_id(string nfc_id, string device, string user)
    {
        return basadata.get_patrol_client_id(nfc_id, device, user);
    }
    [WebMethod]
    public ANImoblie.client_id_in_nfc get_usage_in_nfc(string nfc_id, string device, string user)
    {
        return basadata.get_usage_in_nfc(nfc_id,device,user);
    }


    [WebMethod]
    public ANImoblie.client_briefing get_client_briefing(string client_id)
    {
      //  basadata.get_client_briefing(client_id);

        return basadata.get_client_briefing(client_id) ;

    }
    [WebMethod]
    public ANImoblie.client_briefing2 get_client_briefing2(string client_id)
    {
        //  basadata.get_client_briefing(client_id);

        return basadata.get_client_briefing2(client_id);

    }

    /*
    [WebMethod]
    public ANImoblie.client_briefing2 get_client_briefing2_v153(string client_id)
    {
        //  basadata.get_client_briefing(client_id);

        return basadata.get_client_briefing2_v153(client_id);

    }*/
    [WebMethod]
 
    public ANImoblie.ANI_mobile check_moblie_update(string version)
    {
        //FileInfo myInfo = new FileInfo(@"C:\Users\anialpine01\Desktop\update\apk");
        return basadata.check_moblie_update(version);

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

    [WebMethod]
    public void get_moblie_update(string device, string version)
    {
        string class_name = basadata.dbName;
        System.IO.FileInfo fileInfo = basadata.get_moblie_update(device, version);
        //new System.IO.FileInfo(filepath);
        if (fileInfo != null)
        {
            try
            {
                var Response = Context.Response;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + fileInfo.Name);
                Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                Response.Flush();
                Response.WriteFile(fileInfo.FullName);


                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (System.Threading.ThreadAbortException e)
            {

                // throw;
            }

        }
        return;
        /*
        //FileInfo myInfo = new FileInfo(@"C:\Users\anialpine01\Desktop\update\apk");
        FileInfo fileInfo = basadata.get_moblie_update(device, version);
   // C: \Users\anialpine01\Desktop\update\apk
        if (fileInfo!=null)
        {
            var Response = Context.Response;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + fileInfo.Name);
            Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            Response.Flush();
            Response.WriteFile(fileInfo.FullName);
            Response.End();
        }
        return;
        */
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

    [WebMethod]
    public ANImoblie.client_account_briefing get_client_account_briefing(string client_id)
    {
        //  basadata.get_client_briefing(client_id);

        return basadata.get_client_account_briefing(client_id);

    }

    [WebMethod]
    public ANImoblie.client_account_briefing2 get_client_account_briefing2(string client_id)
    {
        //  basadata.get_client_briefing(client_id);

        return basadata.get_client_account_briefing2(client_id);
    }


    [WebMethod]
    public ANImoblie.client_account_briefing3 get_client_account_briefing3(string client_id)
    {
        //  basadata.get_client_briefing(client_id);

        return basadata.get_client_account_briefing3(client_id);
    }


    [WebMethod]
    public string get_client_picture_id(string client_id)
    {
 
        return basadata.get_client_picture_id(client_id);
    }
    [WebMethod]
    public ANImoblie.measure_briefing  get_measure_briefing(string client_id)
    {
        //string label_usage_id_str = "";
        //string label_usage_code_str = "";
        return basadata.get_measure_briefing(client_id);
    }
    [WebMethod]
    public ANImoblie.insert_client_photo post_client_photo_data(string client_id, String photo_data)
    {
        return basadata.post_client_photo_data(client_id, photo_data);
    }


    [WebMethod]
    public ANImoblie.contact_person  get_client_contact(string client_id)
    {
        return basadata.get_client_contact (client_id);
    }
    [WebMethod]
    public ANImoblie.medical_revisit_records get_medical_revisit_records(string client_id)
    {
      
        return basadata.get_medical_revisit_records(client_id);

    }

    [WebMethod]
    public ANImoblie.medical_events get_medical_event(string client_id)
    {
        return basadata.get_medical_event(client_id);

    }

    [WebMethod]
    public ANImoblie.medical_events get_medical_event2(string client_id)
    {//trial
        return basadata.get_medical_event2(client_id);

    }



    [WebMethod]
    public ANImoblie.search_client_records  get_search_client_result(string keyword, string search_index)
    {
  
        return basadata.get_search_client_result( keyword,search_index);
    }

    // Raymond @20200909
    [WebMethod]
    public ANImoblie.search_client_records_2 get_search_client_result_2(string keyword, string search_index)
    {

        return basadata.get_search_client_result2(keyword, search_index);
    }


    [WebMethod]
    public ANImoblie.medicine_photo  get_medicine_photo(string medicine_photo_id)
    {

        return basadata.get_medicine_photo(medicine_photo_id);
    }


    [WebMethod]
    public ANImoblie.medicine_bag_image get_medicine_bag_image(string medicine_bag_id)
    {

        return basadata.get_medicine_bag_image(medicine_bag_id);
    }



    [WebMethod]
    public ANImoblie.pictureinfo  get_person_image(string picture_id)
    {
        return basadata.get_person_image(picture_id);
    }

    [WebMethod]
    public ANImoblie.medicine_box_content  get_medicine_box_content(string medicine_box_id)
    {
        return basadata.get_medicine_box_content(medicine_box_id);
    }



    [WebMethod]
    public ANImoblie.medicine_records  get_medicine_records(string client_id)
    {

        return basadata.get_medicine_records(client_id);
    }
    [WebMethod]
    public ANImoblie.medicine_description  get_medicine_description(String take_medicine_id)
    {
        return basadata.get_medicine_description(take_medicine_id);
    }

    [WebMethod]
    public ANImoblie.take_medicine_records get_take_medicines(String client_id)
    {
        return basadata.get_take_medicines(client_id);
    }


    [WebMethod]
    public ANImoblie.temperature_records  get_body_temperature_records(string client_id, string time_mode)
    {
        return basadata.get_body_temperature_records(client_id, time_mode);
    }

    [WebMethod]
    public ANImoblie.body_weight_records  get_body_weight_records(string client_id, string time_mode)
    {

        return basadata.get_body_weight_records(client_id, time_mode);
    }

    [WebMethod]
    public ANImoblie.blood_glucose_records  get_blood_glucose_records(string client_id, string time_mode)
    {
        return basadata.get_blood_glucose_records(client_id, time_mode);
    }

    [WebMethod]
    public ANImoblie.blood_oxygen_records  get_blood_oxygen_records(string client_id, string time_mode)
    {
        return basadata.get_blood_oxygen_records(client_id, time_mode);
    }

    [WebMethod]
    public ANImoblie.respiration_rate_records get_respiration_rate_records(string client_id, string time_mode)
    {
        return basadata.get_respiration_rate_records(client_id, time_mode);
    }
    [WebMethod]
    public ANImoblie.blood_pressure_records  get_blood_pressure_records(string client_id, string time_mode)
    {
        return basadata.get_blood_pressure_records(client_id,time_mode);
    }
    [WebMethod]
    public ANImoblie.insert_vital_data post_vital_data(String client_id, String value, String timestamp, String handlingperson, string measure_index)
    {
        return basadata.post_vital_data( client_id,  value,  timestamp,  handlingperson,  measure_index);
    }



    [WebMethod]
    public ANImoblie.wound_reocrds[] get_wound_records(string client_id)
    {
        return basadata.get_wound_records(client_id );
    }
    [WebMethod]
    public ANImoblie.pictureinfo get_wound_image(string picture_id)
    {
        return basadata.get_wound_image(picture_id);
    }
    [WebMethod]
    public ANImoblie.wound_events get_wound_briefing(string client_id)
    {
        return basadata.get_wound_briefing(client_id);
    }
    [WebMethod]
    public ANImoblie.wound_events_2 get_wound_briefing2(string client_id)
    {
        return basadata.get_wound_briefing2(client_id);
    }

    [WebMethod]
    public ANImoblie.wound_events_2 get_wound_briefing_3(string client_id)
    {
        return basadata.get_wound_briefing_3(client_id);
    }

    [WebMethod]
    public ANImoblie.wound_wash_event get_wound_details(string wound_id)
    {
        return basadata.get_wound_details(wound_id);
    }
    [WebMethod]
    public ANImoblie.wound_wash_event get_wound_details2(string wound_id)
    {
        return basadata.get_wound_details2(wound_id);
    }

    [WebMethod]
    public ANImoblie.client_log_setting get_client_log_setting(string idx)
    {
        return basadata.get_client_log_setting(idx);
    }
    [WebMethod]
    public ANImoblie.client_log[] get_client_log(string startdate, string enddate, string client_id, string department_id)
    {
        return basadata.get_client_log( startdate,  enddate,  client_id, department_id);
    }
    [WebMethod]
    public ANImoblie.client_log get_client_log_reply(string log_id)
    {
        return basadata.get_client_log_reply(log_id);


    }


    [WebMethod]
    public ANImoblie.Acc_charge_item get_acc_charge_item(string charge_item_id)
    {
        return basadata.get_acc_charge_item(charge_item_id);
    }
    [WebMethod]
    public ANImoblie.Acc_upload_item get_acc_uploaded_item(string client_id)
    {
        return basadata.get_acc_uploaded_item(client_id);
    }
    [WebMethod]
    public ANImoblie.insert_acc_data post_charge_item(String client_id, String charge_item_id, String quantity, String timestamp, String handlingperson)
    {
        return basadata.post_charge_item(client_id, charge_item_id, quantity, timestamp, handlingperson);
    }
    [WebMethod]
    public ANImoblie.insert_acc_data post_charge_item_zero(String client_id, String charge_item_id, String quantity, String timestamp, String handlingperson, String charge)
    {
        return basadata.post_charge_item(client_id, charge_item_id, quantity, timestamp, handlingperson, charge);
    }

    [WebMethod]
    public ANImoblie.insert_acc_data del_client_account_item(String consumed_id, String handlingperson)
    {
        return basadata.del_charge_item(consumed_id, handlingperson);
    }

    [WebMethod]
    public ANImoblie.post_observation_record post_wound_observation_record(String wound_id, String length, String width, String depth, String level, String color,
    String smell, String fluid_type, String fluid_quantity, String dressing, String clean_days, String clean_times, String examined_by, String photo_data)
    {
        return basadata.post_wound_observation_record(wound_id, length, width, depth, level, color,
    smell, fluid_type, fluid_quantity, dressing, clean_days, clean_times, examined_by, photo_data);
    }
    [WebMethod]
    public ANImoblie.post_observation_record post_wound_observation_record2(String wound_id, String length, String width, String depth, String level, String color,
String smell, String fluid_type, String fluid_quantity, String dressing, String clean_days, String clean_times, String examined_by)
    {
        return basadata.post_wound_observation_record2(wound_id, length, width, depth, level, color,
    smell, fluid_type, fluid_quantity, dressing, clean_days, clean_times, examined_by);
    }

    [WebMethod]
    public ANImoblie.post_observation_record post_wound_observation_photo(String observation_id,  String examined_by, String photo_data)
    {
        return basadata.post_wound_observation_photo(observation_id,    examined_by, photo_data);
    }

    [WebMethod]
    public ANImoblie.post_record post_wound_nextdate(String wound_id, String nextdate, String examined_by)
    {
        return basadata.post_wound_nextdate(wound_id, nextdate, examined_by);
    }
    [WebMethod]
    public ANImoblie.client_log_update post_client_log(String client_id, string department_id, string event_datetime, string title, String content, string isimportant, string boardcast_datetime, String examined_by)
    {
        return basadata.post_client_log(client_id, department_id, event_datetime, title, content, isimportant, boardcast_datetime, examined_by);
 
    }
    [WebMethod]
    public ANImoblie.client_log_update post_client_log_reply(String log_id, String content, String examined_by)
    {
        return basadata.post_client_log_reply(log_id,  content,  examined_by);
    }

    [WebMethod]
    public byte[] getdataset(String user , String pw, String sql , string istest)
    {
        return basadata.getdataset(user,pw, sql,istest );
    }
    [WebMethod]
    public byte[] get_data(String user, String pw, String sql, string istest)
    {
        return basadata.get_data(user, pw, sql, istest);
    }
    [WebMethod]
    public byte[] Ecxe_transaction_Sql(String user, String pw,  String sql, string istest)
    {
        return basadata.Ecxe_transaction_Sql(user, pw, sql, istest);
    }
    [WebMethod]
    public byte[] Ecxe_transaction_Sql2(String user, String pw, String sql, String sql2, string isCheck, string istest)
    {
        return basadata.Ecxe_transaction_Sql2(user, pw, sql, sql2, isCheck, istest);
    }
    [WebMethod]
    public byte[] ExecBoldSql (String user, String pw, String sql, string istest, string image)
    {
        return basadata.ExecBoldSql(user, pw, sql, istest,image);
    }

    #endregion

    #region ANIalarm 
    [WebMethod]
    public ANImoblie.alarm_update post_alarm_data(String id, String group, String port, String state, String time)
    {
        return basadata.post_alarm_data( id,  group,  port,  state,  time);
    }

    [WebMethod]
    public ANImoblie.alarm_update post_alarm_msg(String data)
    {
        return basadata.post_alarm_msg(data);
    }

    #endregion
    [WebMethod]
    public ANI_Display_panel.initial_display get_display_info(string serial_num, string location, string location_long, string listen_port,string address,string vers)
    {
        return panels.get_display_info(serial_num, location, location_long, listen_port,address,vers);
    }

    [WebMethod]
    public ANI_Display_panel.Call_Alarms get_alarm_info(string serial_num, string location, string location_long, string listen_port, string address)
    {
        return panels.get_alarm_info(serial_num, location, location_long, listen_port, address);
    }

    [WebMethod]
    public ANI_Display_panel.client_panel_briefing   get_client_display_info(string display_id)
    {

        return panels.get_client_display_info(display_id);

    }
    /*
    [WebMethod]
    public ANI_Display_panel.client_panel_briefing get_client_display_info_v153(string display_id)
    {

        return panels.get_client_display_info_v153(display_id);

    }*/
    [WebMethod]
    public ANI_Display_panel.room_panel_briefing get_room_display_info(string display_id)
    {

        return panels.get_room_display_info(display_id);

    }
    [WebMethod]
    public ANI_Display_panel.nursing_panel_briefing get_nursing_display_info(string display_id)
    {

        return panels.get_nursing_display_info(display_id);

    }
    [WebMethod]
    public ANI_Display_panel.nursing_panel_briefing get_nursing_display_info2(string display_id)
    {

        return panels.get_nursing_display_info2(display_id);

    }



    [WebMethod]
    public ANI_Display_panel.public_panel_briefing get_public_display_info(string display_id)
    {
        return panels.get_public_display_info(display_id);
    }
 
    [WebMethod]
    public ANI_Display_panel.panel_pictureinfo get_company_image(string company_picture_id)
    {

        return panels.get_company_image(company_picture_id);

    }
 
    [WebMethod]
    public ANI_Display_panel.panel_pictureinfo get_poster_image(string poster_id)
    {

        return panels.get_poster_image(poster_id);

    }
    [WebMethod]
    public ANI_Display_panel.panel_pictureinfo get_panel_person_image(string picture_id)
    {

        return panels.get_person_image(picture_id);

    }


    //  
    [WebMethod()]
    public byte[] udpate_version(string serial_num, string location, string location_long, string listen_port, string address, string vers)
    {
        if (!have_panel)
        {
            return null;
        }

        return panels.udpate_version( serial_num,  location,  location_long,  listen_port,  address,  vers);
 
    }
    [WebMethod()]
    public string call_display_update(string display_id)
    {
        if (!have_panel)
        {
            return null;
        }

        return basadata.call_display_update(display_id);

    }


    [WebMethod]
    public void get_panel_update(string display_id, string version)
    {
        //FileInfo myInfo = new FileInfo(@"C:\Users\anialpine01\Desktop\update\apk");
        // FileInfo fileInfo = basadata.get_moblie_update(device, version);

        //FileInfo fileInfo = new FileInfo(@"C:\Users\humansaadmin\Desktop\update\anitv_apk\app.apk");
        string class_name = basadata.dbName;
        //string filepath = string.Format(@"C:\Users\anialpine01\Desktop\update\apk\{0}\app.apk", class_name);
        string filepath = string.Format(@"C:\Users\ANI\Desktop\update\apk\{0}\app.apk", class_name); // Raymond 20200904
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(filepath);
        // C: \Users\anialpine01\Desktop\update\apk
        //C:\Users\anialpine01\Desktop\update\apk
        if (fileInfo != null)
        {
            try
            {
                var Response = Context.Response;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + fileInfo.Name);
                Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                Response.Flush();
                Response.WriteFile(fileInfo.FullName);


                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (System.Threading.ThreadAbortException e)
            {

               // throw;
            }
       
        }
        return;

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

}