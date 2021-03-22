using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using ANI.code;
using System.Text;
/// <summary>
/// Summary description for WebService2
/// </summary>
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
internal class Clogin  
{



    public static  string getlogin(string user, string pass)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("select a.*, b.client, b.medicine, b.medical, b.nursing, b.assessment from user_login ");
        sql.Append("as a left join user_group as b on a.group_id = b.group_id Where a.user_id ='{0}';");
        string comm = string.Format(sql.ToString(), user);

        return comm;


    }


    public static string get_version()
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT version_id, is_charge, start_date,DATE_FORMAT(expired_date, '%Y/%m/%d'),has_display_panel,has_nfc_tags,has_moblie, created_by, created_datetime FROM user_version   ");
        sql.Append(" where start_date < now() and expired_date >  now() ");

        sql.Append(" order by  start_date desc limit 1 ;");

       // string comm = string.Format(sql.ToString(), user);

        return sql.ToString();
    }
    public static string get_moblie_limit()
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT session_limit,session_minute,session_app_minute, single_login  FROM user_login_setting where valid = 'Y' ;");
        // string comm = string.Format(sql.ToString(), user);

        return sql.ToString();
    }
    public static string get_moblie_version(string condition)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT record_id, UpdatDirectory, version  FROM user_moblie   ");
        sql.Append(" where valid = 'Y' ");

        sql.Append(" ");

        if (condition.Length>0)
        {
            sql.Append(condition);
        }
        // string comm = string.Format(sql.ToString(), user);

        return sql.ToString();


    }
    public static string insert_moblie_update_log(string [] values)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("insert user_moblie_log ( log_id, device_name, old_version,  created_by, created_datetime )  ");
        sql.AppendFormat("  values('{0}','{1}','{2}','{3}','{4}'); ", values);
 
        return sql.ToString();
 

    }

    public static string insert_LoginUser(string[] values)
    {
        //SELECT* FROM ani_azure.user_access_log; user_id, login_datetime, login_ip, device_name

        StringBuilder sql = new StringBuilder();
        sql.Append("insert user_access_log(user_id, login_datetime,login_ip,device_name) ");
        sql.AppendFormat("values('{0}','{1}','{2}','{3}');", values);

        return (sql.ToString());
    }

    public static string insert_LoginUser2(string[] values)
    {
        //SELECT* FROM ani_azure.user_access_log; user_id, login_datetime, login_ip, device_name
        StringBuilder sql = new StringBuilder();

        /* @Raymond add is_mobile & lastupdatetime
        sql.Append("insert user_access_log(user_id, login_datetime,login_ip,device_name,version) ");
        sql.AppendFormat("values('{0}','{1}','{2}','{3}','{4}');", values);
        */

        sql.Append("insert user_access_log(user_id, login_datetime,login_ip,device_name,version,lastupdate_datetime,is_moblie) ");
        sql.AppendFormat("values('{0}','{1}','{2}','{3}','{4}','{1}','Y');", values);
        return (sql.ToString());
    }
    public static string insert_moblie_patrol_log(string[] values)
    {
      //  SELECT* FROM alpine.company_bed_patrol;
      //  patrol_id, device_name, tag_name, location_id, client_id, created_by, created_datetime, valid
         StringBuilder sql = new StringBuilder();
        sql.Append("insert company_bed_patrol ( device_name, tag_name, location_id, client_id,created_by, created_datetime )  ");
        sql.AppendFormat("  values('{0}','{1}','{2}','{3}','{4}','{5}'); ", values);

        return sql.ToString();


    }
    public static string display_getlogin(string user, string condition = "")
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT  display_id, ipv4, ipv6, listen_port, unique_device_id, valid, page_index, usage_id,orientation,shown_alarm,autoupdate,reverse ");
        sql.AppendFormat("from sys_display_panel where unique_device_id = '{0}' ", user);
      //  string comm = string.Format(sql.ToString(), user);
        if (condition.Length>0)
        {
            sql.Append(condition);
        }
        sql.Append(";");
        return sql.ToString();


    }
  
    public static string display_alarm(string user)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT  display_id, ipv4, ipv6, listen_port, unique_device_id, valid, page_index, usage_id,orientation,reverse ");
        sql.Append("from sys_display_panel where unique_device_id = '{0}' ;");
      //  SELECT* FROM azure.client_alarm;
      //  alarm_id, bed_id, client_id, device_id, activate_id, cancal_action_id, alarming, cancel_device, cancel_by, cancel_datetime, modified_by, modified_datetime, created_by, created_datetime, valid
        sql.Append("SELECT alarm.client_id,IFNULL(concat(per.chi_surname,per.chi_name),''), alarm.bed_id, concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value) ,   ");
        sql.Append("IFNULL(doc.client_photo_id,per.sex) ,  ");
    //    SELECT* FROM azure.client_documents2;
    //    client_photo_id, client_id, document_photo, valid
        sql.Append("DATE_FORMAT(alarm.activate_datetime, '%Y-%m-%d %H:%i'),  ");

        sql.Append("ifnull(settle.description,ifnull(active.description,'')) ");
        sql.Append("from client_alarm alarm ");
        sql.Append("left join client_alarm_action as  active on alarm.activate_id = active.action_id ");
        sql.Append("left join client_alarm_action as  settle on alarm.settle_action_id = settle.action_id ");
        sql.Append("LEFT JOIN client_personal2 AS per ON alarm.client_id = per.client_id ");
        sql.Append("LEFT JOIN client_documents2 AS doc ON doc.client_id = per.client_id  and doc.valid='Y' ");
        sql.Append("left JOIN sys_company_bed as b ON alarm.bed_id = b.bed_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append(" where alarm.alarming='Y' and alarm.valid='Y' ");
        sql.Append(" order by alarm.activate_datetime ; ");
        //   SELECT* FROM azure.client_alarm_action;
        //    action_id, description, valid
 
        string comm = string.Format(sql.ToString(), user);

        return comm;
    }
    public static string display_add_panel(string[] values)
    {

        //display_id, ipv4, ipv6, listen_port, unique_device_id, valid, created_by, created_datetime, modified_by, modified_datetime, last_update_datetime
        StringBuilder sql = new StringBuilder();

        sql.Append("update sys_display_panel set ipv4 = '' where ipv4 = '{1}' ;");
        sql.Append("Insert sys_display_panel (display_id, ipv4, ipv6, listen_port, unique_device_id,current_version, valid,created_datetime,last_update_datetime,mac_address)");
        sql.Append("values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');");
        //sql.Append("from sys_display_panel where unique_device_id = '{0}' ;");
        string comm = string.Format(sql.ToString(), values);

        return comm;


    }



    public static string display_update_time(string [] values)
    {

        //display_id, ipv4, ipv6, listen_port, unique_device_id, valid, created_by, created_datetime, modified_by, modified_datetime, last_update_datetime
        StringBuilder sql = new StringBuilder();
        sql.Append("update sys_display_panel set current_version   = '{4}' , last_update_datetime = '{5}' ");
        sql.Append("where unique_device_id = '{0}';");
        //sql.Append("from sys_display_panel where unique_device_id = '{0}' ;");
        string comm = string.Format(sql.ToString(), values);
        return comm;
    }
    public static string display_update_location(string[] values)
    {

        //display_id, ipv4, ipv6, listen_port, unique_device_id, valid, created_by, created_datetime, modified_by, modified_datetime, last_update_datetime
        StringBuilder sql = new StringBuilder();

        sql.Append("update sys_display_panel set ipv4 = '' where ipv4 = '{1}' ;");
        sql.Append("update sys_display_panel set ipv4 = '{1}', ipv6 = '{2}', listen_port   = '{3}', current_version   = '{4}' , last_update_datetime = '{5}' ");
        //sql.Append("values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');");
        sql.Append("where unique_device_id = '{0}' ;");
        //sql.Append("from sys_display_panel where unique_device_id = '{0}' ;");
        string comm = string.Format(sql.ToString(), values);

        return comm;


    }

    // Raymond @20201216
    public static string getlogindata(string user, string device_id)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT log1.user_id, date_format(log1.login_datetime, '%Y-%m-%d %H:%i:%s') As login_datetime,log1.login_ip,log1.device_name FROM user_access_log log1 ");
        sql.Append("left join user_access_log log22 ");
        sql.Append("on log1.user_id = log22.user_id ");
        sql.Append("and log1.device_name =log22.device_name ");
        sql.Append("and date(log1.logout_datetime) = date(log22.logout_datetime) ");
        sql.Append("and log1.login_datetime < log22.login_datetime ");
        sql.Append("and log1.is_moblie = log22.is_moblie ");

        sql.Append("where log1.user_id = '{0}' ");
        sql.Append("and log1.device_name = '{1}' ");
        sql.Append("and date(log1.logout_datetime) = '1900-05-16' ");
        sql.Append("and log22.user_id is null ");
        sql.Append("and log1.is_moblie = 'Y'; ");

        string comm = string.Format(sql.ToString(), user, device_id);

        return comm;


    }


    public static string update_logout_datetime(string user_id, string login_datetime, string login_ip, string device_name, string logout_datetime)
    {

        //display_id, ipv4, ipv6, listen_port, unique_device_id, valid, created_by, created_datetime, modified_by, modified_datetime, last_update_datetime
        StringBuilder sql = new StringBuilder();

        sql.Append("update user_access_log set logout_datetime = '{4}' , lastupdate_datetime = '{4}' where user_id = '{0}' and login_datetime = '{1}' and login_ip = '{2}' and is_moblie= 'Y';");

        string comm = string.Format(sql.ToString(), user_id, login_datetime, login_ip, device_name, logout_datetime);

        return comm;


    }
}

