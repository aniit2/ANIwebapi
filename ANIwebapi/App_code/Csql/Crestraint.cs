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
internal class Crestraint  
{
 
    public static  string get_restraint_brief(string condition)
    {

        StringBuilder sql = new StringBuilder();
 
        sql.Append("select rev.restraint_id,IFNULL(concat(per.chi_surname,per.chi_name),''),rev.client_id,");
        sql.Append("DATE_FORMAT(rev.begin_date, '%Y-%m-%d'),DATE_FORMAT(rev.end_date, '%Y-%m-%d'),");
        sql.Append("rev.restraint_items,rev.other_restraint_items, rev.restraint_items_remarks,");


        sql.Append(" if(rev.full_day = 'Y','全日',''),");
        sql.Append(" if(rev.daytime = 'Y',concat('日間由 ',rev.daytime_begin_hour,'時至 ',rev.daytime_end_hour,'時'),''),");
        sql.Append(" if(rev.nighttime = 'Y',concat('夜間由 ',rev.nighttime_begin_hour,'時至 ',rev.nighttime_end_hour,'時'),''),");
        sql.Append(" if(rev.other_time = 'Y',concat(rev.other_time_session),''),");
        sql.Append(" rev.restraint_state,concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) , per.sex,doc.client_photo_id ");
 

        sql.Append("from nursing_restraint rev ");

        sql.Append("LEFT JOIN client_personal2 AS per ON rev.client_id = per.client_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");

        sql.Append("left join nursing_setting sett on sett.valid = 'Y'   ");

        sql.Append("left join client_documents2 doc on rev.client_id = doc.client_id and doc.valid = 'Y'   ");

        sql.Append("where per.client_id !='' and rev.valid = 'Y' ");
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }
        string comm = string.Format(sql.ToString());
        return comm;


    }
 


    public static string get_blood_pressure_records(string client_id, int time_mode)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" select systolic, diastolic, pulse_rate,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_blood_pressure Where client_id = ('{0}') and valid = 'Y' ");
        sql.Append(get_meaure_time_range_records(time_mode));
        string comm = string.Format(sql.ToString(), client_id);
        return comm;

 
    }


    public static string get_blood_oxygen_records(string client_id, int time_mode)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" select blood_oxygen,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_blood_oxygen Where client_id = ('{0}') and valid = 'Y' ");
        sql.Append(get_meaure_time_range_records(time_mode));
        string comm = string.Format(sql.ToString(), client_id);
        return comm;

    }

    public static string get_blood_glucose_records(string client_id, int time_mode)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" select blood_glucose,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_blood_glucose Where client_id = ('{0}') and valid = 'Y' ");
        sql.Append(get_meaure_time_range_records(time_mode));
        string comm = string.Format(sql.ToString(), client_id);
        return comm;

    }
    public static string get_respiration_rate_records(string client_id, int time_mode)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" select respiration_rate,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_respiration_rate Where client_id = ('{0}') and valid = 'Y' ");
        sql.Append(get_meaure_time_range_records(time_mode));
        string comm = string.Format(sql.ToString(), client_id);
        return comm;

    }



    public static string get_temperature_records(string client_id, int time_mode)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" select temperature,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_body_temperature Where client_id = ('{0}') and valid = 'Y' ");
        sql.Append(get_meaure_time_range_records(time_mode));
        string comm = string.Format(sql.ToString(), client_id);
        return comm;

    }
    public static string get_weight_records(string client_id, int time_mode)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" select weight,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i')  ");
        sql.Append(" from medicial_vital_body_weight Where client_id = ('{0}') and valid = 'Y' ");
        sql.Append(get_meaure_time_range_records(time_mode));
        string comm = string.Format(sql.ToString(), client_id);
        return comm;

    }

    public static string insert_blood_presure(string[] values)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("insert medicial_vital_blood_pressure(");
        sql.Append("record_id, client_id,examination_datetime, ");
        sql.Append("systolic, diastolic, pulse_rate,");
        sql.Append(" created_by, created_datetime , valid)");
        sql.Append("values('{0}','{1}','{2}','{3}','{4}','{5}',");
        sql.Append("'{6}','{7}','Y' );");
        return  (string.Format(sql.ToString(), values)) ;

    }
 
    public static string get_meaure_time_range_records(int time_mode)
    {

        if (time_mode == 1)
        {

            return ("ORDER BY examination_datetime DESC LIMIT 6");

        }
        else
        {
            return "";
        }

        /*
        if (time_mode == 2)
        {
            cmdText = "SELECT * FROM care_body_temperature where client_id = (@id) AND examination_datetime > NOW() - INTERVAL 1 week ORDER BY examination_datetime DESC";


        }
        else if (time_mode == 3)
        {
            cmdText = "SELECT * FROM care_body_temperature where client_id = (@id) AND examination_datetime > NOW() - INTERVAL 2 week ORDER BY examination_datetime DESC";

        }
        else if (time_mode == 4)
        {
            cmdText = "SELECT * FROM care_body_temperature where client_id = (@id) AND examination_datetime > NOW() - INTERVAL 1 month ORDER BY examination_datetime DESC";
        }
        */
    }
    public static string get_medical_revisit_records(string client_id)
    {

        StringBuilder sql = new StringBuilder();

        sql.Append("select rev.revisit_id,");
        sql.Append("DATE_FORMAT(rev.revisit_planned_datetime, '%Y-%m-%d %H:%i'),");
        sql.Append(" cast(rev.revisit_event as char), ");
        sql.Append("g.addr_org_chi_name,rev.specialties_code,rev.transport,concat( rev.revisit_accompany_type,' ',rev.revisit_accompany_name),");
        sql.Append("rev.revisit_remark,g.addr_code ");

        sql.Append("from medical_revisit rev ");

        sql.Append("LEFT JOIN client_personal2 AS per ON rev.client_id = per.client_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append("LEFT JOIN company_address_book AS g ON rev.hospital_addr_id = g.addr_id ");


        sql.Append("where per.client_id  ='{0}' and rev.valid = 'Y' and rev.revisit_status = '預約中'  ");

        string comm = string.Format(sql.ToString(), client_id);

        return comm;


    }
    public static String get_revisit_brief(string condition, bool need_photo = true)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("select rev.revisit_id,IFNULL(concat(per.chi_surname,per.chi_name),''),rev.client_id,");
        sql.Append("DATE_FORMAT(rev.revisit_planned_datetime, '%Y-%m-%d'),DATE_FORMAT(rev.revisit_planned_datetime, '%h:%i%p'),");
        sql.Append("DATE_FORMAT(rev.revisit_actual_time, '%Y-%m-%d %H:%i'), ");
        sql.Append("g.addr_org_chi_name,rev.specialties_code,cast(rev.revisit_event as char),rev.transport,concat( rev.revisit_accompany_type,' ',rev.revisit_accompany_name),");
        sql.Append("rev.revisit_remark,rev.revisit_status,per.sex " + (need_photo == true ? ",doc.document_photo " : ",doc.client_photo_id "));



        sql.Append("from medical_revisit rev ");

        sql.Append("LEFT JOIN client_personal2 AS per ON rev.client_id = per.client_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append("LEFT JOIN company_address_book AS g ON rev.hospital_addr_id = g.addr_id ");
        sql.Append("left join client_documents2 doc on rev.client_id = doc.client_id and doc.valid = 'Y'   ");

        sql.Append("where per.client_id !='' and rev.valid = 'Y' ");
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }

        sql.Append("order by rev.revisit_planned_datetime ;");
        return (sql.ToString());

    }
    public static string get_medical_brief(string client_id)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(get_ae_brief("and ae.client_id = " + client_id + " and ae.ae_status = '留醫中' "));
        sql.Append(get_revisit_brief("and rev.client_id = " + client_id + " and rev.revisit_status = '預約中' ", false));
        sql.Append(get_event_grid_table("and ( rev.client_id = " + client_id + " and rev.event_status = '預約中' )" +
            " or rev.event_id in (SELECT  eve2.event_id FROM (select * from medical_event eve3 WHERE eve3.client_id = " +
            client_id + " AND eve3.event_status = '已完成' ORDER BY eve3.event_actual_time DESC LIMIT 3) AS eve2) ",
            false, "FIELD(rev.event_status, '預約中') desc, "));

        return sql.ToString();

    }

    public static string get_ae_brief(string condition)
    {
        StringBuilder sql = new StringBuilder();

        sql.Append("select ae.ae_id,IFNULL(concat(per.chi_surname,per.chi_name),''),ae.client_id,");
        sql.Append("ae_status,ae_in_reason, ");
        sql.Append("DATE_FORMAT(ae.ae_in_datetime, '%Y/%m/%d %h:%i%p'),");
        sql.Append("hospitalized,g.addr_org_chi_name,ae_in_bed_num, ");
        sql.Append("ae_in_remark,  ");
        sql.Append("DATE_FORMAT(ae.ae_out_datetime, '%Y/%m/%d %H:%i'),");
        sql.Append("ae_out_reason,ae_out_remark,per.sex,doc.client_photo_id ");

        sql.Append("from medical_ae ae ");

        sql.Append("LEFT JOIN client_personal2 AS per ON ae.client_id = per.client_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append("LEFT JOIN company_address_book AS g ON ae.ae_addr_id = g.addr_id ");

        sql.Append("left join client_documents2 doc on ae.client_id = doc.client_id and doc.valid = 'Y'   ");


        sql.Append("where per.client_id !=''  and ae.valid = 'Y' ");

        if (condition.Length > 0)
        {
            sql.Append(condition);
        }

        sql.Append("order by ae.ae_in_datetime asc ;");
        return (sql.ToString());

    }
    public static string get_event_grid_table(string condition, bool need_photo = false, string order = "")
    {
 
        StringBuilder sql = new StringBuilder();
        sql.Append("select eve.tchi_value, rev.event_id,per.sex ,IFNULL(concat(per.chi_surname,per.chi_name),''),rev.client_id,");
        sql.Append("rev.event_item_id, ");
        sql.Append("DATE_FORMAT(rev.event_planned_datetime, '%Y/%m/%d'),DATE_FORMAT(rev.event_planned_datetime, '%H:%i'),");
        sql.Append("DATE_FORMAT(rev.event_actual_time, '%Y/%m/%d %H:%i'), ");

        sql.Append("rev.event_remark,rev.event_status,eve.event_color  " + (need_photo == true ? ",doc.document_photo " : ",doc.client_photo_id "));

        sql.Append("from medical_event rev ");

        sql.Append("LEFT JOIN client_personal2 AS per ON rev.client_id = per.client_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");

        sql.Append("left join sys_medical_event eve on rev.event_item_id = eve.event_item_id   ");


        sql.Append("left join client_documents2 doc on rev.client_id = doc.client_id and doc.valid = 'Y'   ");

        sql.AppendFormat("where per.client_id !='' and rev.valid = 'Y' ");

        if (condition.Length > 0)
        {
            sql.Append(condition);
        }

        sql.Append("group by rev.event_id order by eve.tchi_value ,"+order+" rev.event_planned_datetime desc ; ");


        return sql.ToString();

    }
    public static string get_wound_records(string client_id)
    {

        StringBuilder sql = new StringBuilder();

        //cmdText = "SELECT * FROM care_wound2 where client_id = (@id) and recovery = (@state)";

        /*
        string cmdText1 = "SELECT * FROM care_wound_observation_record where care_wound_id = (@id) ORDER BY wound_observation_date DESC LIMIT 1 ";

        string cmdText2 = "SELECT * FROM care_wound_plan where care_wound_id = (@id) ORDER BY modified_datetime DESC  LIMIT 1";

        cmdText = "SELECT * FROM care_wound2 where client_id = (@id) and recovery = (@state)";


        SELECT* FROM azure.care_wound2;
        care_wound_id, client_id, event_id, recovery, wound_type, discover_photo_id, wound_position_id, wound_discover_date, wound_report_source, wound_recover_date, wound_remark, created_by, created_datetime, modified_by, modified_datetime
SELECT* FROM azure.care_wound_plan2; care_wound_plan_id, care_wound_id, plan_start_datetime, method, frequency, plan_remark, created_by, created_datetime
SELECT* FROM azure.care_wound_observation_record2;
        care_observation_id, care_wound_id, wound_observation_date, wound_stage, wound_side_color, wound_side_tissue, wound_photo_id, wound_length, wound_width, wound_bleeding, wound_depth, wound_color, wound_smell, wound_fluid_type, wound_fluid_color, wound_fluid_quantity, wound_dressing_id, examined_by


        sql.Append("select rev.revisit_id,");
        sql.Append("DATE_FORMAT(rev.revisit_planned_datetime, '%Y-%m-%d %H:%i'),");
        sql.Append(" cast(rev.revisit_event as char), ");
        sql.Append("g.addr_org_chi_name,rev.specialties_code,rev.transport,concat( rev.revisit_accompany_type,' ',rev.revisit_accompany_name),");
        sql.Append("rev.revisit_remark,g.addr_code ");

        sql.Append("from medical_revisit rev ");

        sql.Append("LEFT JOIN client_personal2 AS per ON rev.client_id = per.client_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append("LEFT JOIN company_address_book AS g ON rev.hospital_addr_id = g.addr_id ");
        */

        sql.Append("where per.client_id  ='{0}' and rev.valid = 'Y' and rev.revisit_status = '預約中'  ");

        string comm = string.Format(sql.ToString(), client_id);

        return comm;


    }


    public static string get_nursing_panel_briefing(string display_id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報

        //    SELECT* FROM azure.sys_display_room_panel;
        //     room_id, room_name, room_remark, valid, created_by, created_datetime, modified_by, modified_datetime

        //SELECT* FROM azure.sys_display_room_bed;
        ///       room_client_id, room_id, bed_id, valid
     ///////   SELECT* FROM azure.sys_display_panel_nursing;
     //   '100000', '100000', '100000', 'Y', '', '1900-05-16 00:00:00', '', '1900-05-16 00:00:00'
      //      nursing_panel_id, display_id, color_setting_id, valid, created_by, created_datetime, modified_by, modified_datetime

        sql.Append("SELECT npanel.nursing_panel_id ");
 

        // sql.Append("CONCAT_WS(';', abi.self_care_ability, abi.movement_ability, abi.eating_ability,");
        // sql.Append("abi.cognitive_ability, abi.audiovisual_ability, abi.communicate_ability, abi.incontinence, ability_remark), ");

        //  sql.Append("GROUP_CONCAT(care_wound_id),  ");
        //  sql.Append("ae.ae_status,");
        //   sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");
        //   sql.Append("if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),");
 
        //sql.Append("left join sys_display_room_bed rb on room.room_id = rb.room_id and rb.valid='Y' ");

       // sql.Append("LEFT JOIN sys_company_bed b  on b.bed_id = rb.bed_id ");
       // sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
      //  sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
      //  sql.Append("left join client_personal2 per on per.client_id = b.client_id  ");
      //  sql.Append("left join sys_display_panel_bed bed_panel on bed_panel.bed_id = b.bed_id  ");
      //  sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");

        sql.Append(" FROM sys_display_panel_nursing npanel  where npanel.valid = 'Y' AND  npanel.display_id = '{0}' ");




        string comm = string.Format(sql.ToString(), display_id);
        return comm;
    }
}

