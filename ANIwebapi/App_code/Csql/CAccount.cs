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
internal class CAccount  
{

    public static  string get_acc_charge_item(string charge_item_id)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("select charge_item_name,round(charge_item_unit_price,2) from acc_charge_item2 where charge_item_id = '{0}'");
        string comm = string.Format(sql.ToString(), charge_item_id);
        return comm;


    }
    public static string get_acc_uploaded_item(string acc_num,string current_time)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("select date_format(a.charge_datetime,'%Y/%m/%d %H:%i'), b.charge_item_name, a.charge_quantity ");
        sql.Append(" from acc_charge_consumed a ");
        sql.Append(" left join acc_charge_item2 b on a.charge_item_id = b.charge_item_id  ");
        sql.AppendFormat(" where MONTH(a.charge_datetime) = MONTH('{0}') and year(a.charge_datetime) = year('{0}') ", current_time);
        sql.AppendFormat(" and a.account_num = '{0}' and a.charge_status = 'N'  ", acc_num);
        sql.Append(" order by a.charge_datetime desc ");


        string comm = string.Format(sql.ToString());
        return comm;
    }
    public static string get_acc_uploaded_item(string client_id )
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("select date_format(a.charge_datetime,'%Y/%m/%d %H:%i'), b.charge_item_name, a.charge_quantity ");
        sql.Append(" from acc_charge_consumed a ");
        sql.Append(" left join acc_charge_item2 b on a.charge_item_id = b.charge_item_id  ");
        sql.Append(" left join client_personal2 per on a.account_num = per.client_number  ");

        sql.Append(" where  ");
        sql.AppendFormat(" per.client_id = '{0}' and a.charge_status = 'N'  ", client_id);
        sql.Append(" order by a.charge_datetime desc ;");


        string comm = string.Format(sql.ToString());
        return comm;
    }
    public static string get_acc_uploaded_item2(string client_id)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("select a.consumed_id, b.charge_item_name, a.charge_quantity,date_format(a.charge_datetime,'%Y/%m/%d') as record_date, a.created_by,date_format(a.created_datetime,'%Y/%m/%d %H:%i') as created_datetime ");
        sql.Append(" from acc_charge_consumed a ");
        sql.Append(" left join acc_charge_item2 b on a.charge_item_id = b.charge_item_id  ");
        sql.Append(" left join client_personal2 per on a.account_num = per.client_number  ");

        sql.Append(" where  ");
        sql.AppendFormat(" per.client_id = '{0}' and a.charge_status = 'N'  ", client_id);
        sql.Append(" order by a.charge_datetime desc ;");


        string comm = string.Format(sql.ToString());
        return comm;
    }
    public static string get_medical_revisit_records(string client_id)
    {

        StringBuilder sql = new StringBuilder();


        sql.Append(" select  CONCAT_WS(';',CONCAT_WS(',',systolic, diastolic, pulse_rate),if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_blood_pressure Where client_id = ('{0}') ORDER BY examination_datetime DESC LIMIT 1;");

        sql.Append(" select  CONCAT_WS(';',blood_oxygen,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_blood_oxygen Where client_id = ('{0}') ORDER BY examination_datetime DESC LIMIT 1;");


        sql.Append(" select  CONCAT_WS(';',blood_glucose, DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_blood_glucose Where client_id = ('{0}') ORDER BY examination_datetime DESC LIMIT 1;");


        sql.Append(" select  CONCAT_WS(';',respiration_rate,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_respiration_rate Where client_id = ('{0}') ORDER BY examination_datetime DESC LIMIT 1;");

        sql.Append(" select  CONCAT_WS(';',temperature,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_body_temperature Where client_id = ('{0}') ORDER BY examination_datetime DESC LIMIT 1;");

        sql.Append(" select  CONCAT_WS(';',weight,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_body_weight Where client_id = ('{0}') ORDER BY examination_datetime DESC LIMIT 1;");
        string comm = string.Format(sql.ToString(), client_id);

        return comm;


    }

    public static string post_charge_item(string[] values)
    {
        StringBuilder sql = new StringBuilder();

        sql.Append("insert acc_charge_consumed(consumed_id, account_num,  charge_item_id, charge_status, charge_quantity,");
        sql.Append("charge_amount, charge_datetime, charge_item_remark, created_by, created_datetime)");
        sql.Append("VALUES({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')");
 
        return string.Format( sql.ToString(),values);
    }

    public static string del_charge_item(string[] values)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" update acc_charge_consumed ");
        sql.Append(" set charge_status = 'C' ");
        sql.Append(" , modified_by = '{1}' ");
        sql.Append(" , modified_datetime = now() ");
        sql.Append(" where consumed_id = '{0}' ");
        sql.Append(" and charge_status = 'N' ");

        return string.Format(sql.ToString(), values);
    }

}

