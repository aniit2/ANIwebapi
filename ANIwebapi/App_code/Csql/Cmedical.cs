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
internal class Cmedical  
{
    public static string[] measuretable = new string[] {
                "medicial_vital_blood_pressure",
            "medicial_vital_blood_oxygen",
            "medicial_vital_blood_glucose",
               "medicial_vital_respiration_rate",
                "medicial_vital_body_temperature",
                "medicial_vital_body_weight"};
    public static string[] measure_value_col = new string[] {
            "",
            "blood_oxygen",
                "blood_glucose",
                "respiration_rate",
                "temperature",
            "weight"};
    public static  string get_measure_brief(string  client_id)
    {

        StringBuilder sql = new StringBuilder();


        sql.Append(" select  CONCAT_WS(';',CONCAT_WS(',',systolic, diastolic, pulse_rate),if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i'))  ");
        sql.Append(" from medicial_vital_blood_pressure Where client_id =  '{0}'  ORDER BY examination_datetime DESC LIMIT 6;");

        sql.Append(" select  CONCAT_WS(';',blood_oxygen,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i'))  ");
        sql.Append(" from medicial_vital_blood_oxygen Where client_id =  '{0}'  ORDER BY examination_datetime DESC LIMIT 6;");


        sql.Append(" select  CONCAT_WS(';',blood_glucose,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i'))  ");
        sql.Append(" from medicial_vital_blood_glucose Where client_id =  '{0}'  ORDER BY examination_datetime DESC LIMIT 6;");


        sql.Append(" select  CONCAT_WS(';',respiration_rate,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i'))  ");
        sql.Append(" from medicial_vital_respiration_rate Where client_id =  '{0}'  ORDER BY examination_datetime DESC LIMIT 6;");

        sql.Append(" select  CONCAT_WS(';',temperature,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i'))  ");
        sql.Append(" from medicial_vital_body_temperature Where client_id = '{0}'  ORDER BY examination_datetime DESC LIMIT 6;");

        sql.Append(" select  CONCAT_WS(';',weight,if(modified_by='',created_by,modified_by), DATE_FORMAT(examination_datetime,'%Y/%m/%d %H:%i'))  ");
        sql.Append(" from medicial_vital_body_weight Where client_id =  '{0}'   ORDER BY examination_datetime DESC LIMIT 6;");
        string comm = string.Format(sql.ToString(), client_id);

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
    public static string insert_other_vital(string[] values, int index)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendFormat("insert {0}(", measuretable[index]);
        sql.Append("record_id, client_id,examination_datetime, ");
        sql.AppendFormat("{0},", measure_value_col[index]);
        sql.Append(" created_by, created_datetime , valid)");
        sql.Append("values('{0}','{1}','{2}','{3}','{4}','{5}','Y');");
        return  string.Format(sql.ToString(), values) ;
    }
    public static string insert_bmi_vital(string[] values, int index)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendFormat("insert {0}(", measuretable[5]);
        sql.Append("record_id, client_id,examination_datetime, ");
        //     sql.AppendFormat("{0},", measure_value_col[index]);
        sql.Append(" height, weight ,");
        sql.Append(" created_by, created_datetime , valid)");
        sql.Append("values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','Y');");
        return string.Format(sql.ToString(), values);
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
        //sql.Append("select rev.revisit_id,IFNULL(concat(per.chi_surname,per.chi_name),'') as client_name,rev.client_id,");
        //sql.Append("DATE_FORMAT(rev.revisit_planned_datetime, '%Y-%m-%d') as revisit_planned_date,DATE_FORMAT(rev.revisit_planned_datetime, '%h:%i%p') as revisit_planned_time,");
        //sql.Append("DATE_FORMAT(rev.revisit_actual_time, '%Y-%m-%d %H:%i'), ");
        //sql.Append("g.addr_org_chi_name,rev.specialties_code,cast(rev.revisit_event as char),rev.transport,concat( rev.revisit_accompany_type,' ',rev.revisit_accompany_name),");
        //sql.Append("rev.revisit_remark,rev.revisit_status,per.sex " + (need_photo == true ? ",doc.document_photo " : ",doc.client_photo_id "));
        sql.Append("select rev.revisit_id, IFNULL(concat(per.chi_surname,per.chi_name),'') as client_name, rev.client_id,");
        sql.Append(" CONCAT(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value) as bedName, ");
        sql.Append("DATE_FORMAT(rev.revisit_planned_datetime, '%Y-%m-%d') as revisit_planned_date, DATE_FORMAT(rev.revisit_planned_datetime, '%h:%i%p') as revisit_planned_time,");
        sql.Append("DATE_FORMAT(rev.revisit_actual_time, '%Y-%m-%d %H:%i') as revisit_actual_time, ");
        sql.Append("g.addr_org_chi_name, rev.specialties_code,cast(rev.revisit_event as char)as revisit_event, rev.transport, concat( rev.revisit_accompany_type,' ',rev.revisit_accompany_name) as revisit_accompany,");
        sql.Append("rev.revisit_remark,rev.revisit_status, per.sex " + (need_photo == true ? ", doc.document_photo " : ", doc.client_photo_id ") + " as photo_id ");


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

        sql.Append("select ae.ae_id,IFNULL(concat(per.chi_surname,per.chi_name),'') as client_name,ae.client_id,");
        sql.Append(" CONCAT(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value) as bedName, ");
        sql.Append("ae_status,ae_in_reason, ");
        sql.Append("DATE_FORMAT(ae.ae_in_datetime, '%Y/%m/%d') as ae_in_date, DATE_FORMAT(ae.ae_in_datetime, '%h:%i%p') as ae_in_time,");
        //sql.Append("DATE_FORMAT(rev.revisit_planned_datetime, '%Y-%m-%d') as revisit_planned_date,DATE_FORMAT(rev.revisit_planned_datetime, '%h:%i%p') as revisit_planned_time,");
        sql.Append("hospitalized,g.addr_org_chi_name,ae_in_bed_num, ");
        sql.Append("ae_in_remark,  ");
        sql.Append("DATE_FORMAT(ae.ae_out_datetime, '%Y/%m/%d %H:%i') as ae_out_datetime,");
        sql.Append("ae_out_reason, ae_out_remark, per.sex, doc.client_photo_id ");

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
        //sql.Append("select eve.tchi_value, rev.event_id,per.sex ,IFNULL(concat(per.chi_surname,per.chi_name),'') as client_name,rev.client_id,");
        //sql.Append(" CONCAT(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value) as bedName, ");
        //sql.Append("rev.event_item_id, ");
        //sql.Append("DATE_FORMAT(rev.event_planned_datetime, '%Y/%m/%d') as event_planned_date,DATE_FORMAT(rev.event_planned_datetime, '%H:%i')as event_planned_time,");
        //sql.Append("DATE_FORMAT(rev.event_actual_time, '%Y/%m/%d %H:%i') as event_actual_time, ");

        //sql.Append("rev.event_remark,rev.event_status,eve.event_color  " + (need_photo == true ? ",doc.document_photo " : ",doc.client_photo_id "));

        sql.Append("select eve.tchi_value, rev.event_id,per.sex ,IFNULL(concat(per.chi_surname,per.chi_name),'')as client_name,rev.client_id,");
        sql.Append(" CONCAT(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value) as bedName, ");
        sql.Append("rev.event_item_id, ");
        sql.Append("DATE_FORMAT(rev.event_planned_datetime, '%Y/%m/%d') as event_planned_date,DATE_FORMAT(rev.event_planned_datetime, '%H:%i') as event_planned_time,");
        sql.Append("DATE_FORMAT(rev.event_actual_time, '%Y/%m/%d %H:%i') as event_actual_time, ");

        sql.Append("rev.event_remark,rev.event_status,eve.event_color  " + (need_photo == true ? ",doc.document_photo " : ",doc.client_photo_id ") + "as photo_id ");

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

        sql.Append("group by rev.event_id order by eve.tchi_value ,"+order+" rev.event_planned_datetime asc ; ");


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
 
        sql.Append("SELECT npanel.nursing_panel_id ,scroll_speed,font_size, ");
        sql.Append("shown_revisit, shown_medical, shown_ae, shown_restraint, ");
        sql.Append(" format_revisit, format_medical, format_ae, format_restraint, ");
        sql.Append("channel_revisit, channel_medical, channel_ae, channel_restraint, ");

        sql.Append("revisit_theme_id, medical_theme_id, ae_theme_id, restraint_theme_id, ");
        sql.Append("shown_book, shown_analysis ");
        sql.Append("  ");

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

        sql.Append(" FROM sys_display_panel_nursing npanel  where npanel.valid = 'Y' AND  npanel.display_id = '{0}'; ");


        sql.Append(" select theme_id, theme_name, dark_color, medime_color, light_color, ordering ");

        sql.Append(" FROM  sys_theme_color");

        sql.AppendFormat(" order by ordering ");

        string comm = string.Format(sql.ToString(), display_id);
        return comm;
    }

    public static string get_nursing_panel_briefing2(string display_id)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" SELECT npanel.nursing_panel_id, npanel.scroll_speed, npanel.font_size, npanel.show_bed_name as show_bedname ");
        sql.Append(" FROM sys_display_panel_nursing2 npanel ");
        sql.Append(" where npanel.valid = 'Y' ");
        sql.Append(" and npanel.display_id = '{0}' ; ");

        sql.Append(" SELECT IFNULL(ndetail.type_id,0) as type_id, npanel_type.type_name, npanel_type.show_subitem, npanel_type.map_data_col_idx  ");
        sql.Append(" , ndetail.show_grid ");
        sql.Append(" , ndetail.channel_id, ifnull(npanel_channel.channel_index, 0) as channel_index ");
        sql.Append(" , ndetail.theme_id, npanel_theme.color1, npanel_theme.color2, npanel_theme.color3 ");
        sql.Append(" FROM sys_display_panel_nursing2 npanel ");
        sql.Append(" left join sys_display_panel_nursing_detail ndetail on ndetail.nursing_panel_id = npanel.nursing_panel_id and ndetail.valid = 'Y' ");
        sql.Append(" left join sys_display_panel_nursing_type npanel_type on npanel_type.type_id = ndetail.type_id and npanel_type.valid = 'Y' ");
        sql.Append(" left join sys_display_panel_theme_color npanel_theme on npanel_theme.theme_id = ndetail.theme_id and npanel_theme.valid = 'Y' ");
        sql.Append(" left join sys_display_panel_nursing_channel npanel_channel on npanel_channel.channel_id = ndetail.channel_id and npanel_channel.valid = 'Y' ");
        sql.Append(" where npanel.valid = 'Y' ");
        sql.Append(" and npanel.display_id = '{0}' ");
        sql.Append(" and ndetail.detail_id is not null ");
        sql.Append(" order by npanel_type.ordering ; ");
        sql.Append("  ");
        sql.Append("  ");

        string comm = string.Format(sql.ToString(), display_id);
        return comm;
    }

    public static string get_home_leave_brief(string condition)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" SELECT distinct ");
        sql.Append(" cleave.leave_id,IFNULL(concat(per.chi_surname,per.chi_name),'') as client_name ,cleave.client_id, ");
        sql.Append(" cleave_state.state_id, cleave_state.state_name ,cleave.leave_out_reason, cleave_reason.reason_name, ");
        sql.Append(" DATE_FORMAT(cleave.leave_out_datetime, '%Y/%m/%d') as leave_out_datetime,   ");
        sql.Append(" case when date(cleave.leave_planned_return_datetime) = '1900-05-16' then '' else DATE_FORMAT(cleave.leave_planned_return_datetime, '%Y/%m/%d') end as leave_planned_return_datetime,   ");
        sql.Append(" per.sex,doc.client_photo_id  ");
        sql.Append(" FROM client_leave cleave ");
        sql.Append(" left join sys_client_leave_state cleave_state on cleave_state.state_id = cleave.leave_state and cleave_state.valid = 'Y' ");
        sql.Append(" left join sys_client_leave_reason cleave_reason on cleave_reason.reason_id = cleave.leave_out_reason and cleave_reason.valid = 'Y' ");
        sql.Append(" LEFT JOIN client_personal2 per on cleave.client_id = per.client_id and per.valid = 'Y' ");
        sql.Append(" left JOIN sys_company_bed as bed ON per.client_id = bed.client_id and bed.valid = 'Y' ");
        sql.Append(" left join sys_company_zone zone on bed.zone_id = zone.zone_id  and zone.valid = 'Y' ");
        sql.Append(" left join sys_company_block blk on blk.block_id = zone.block_id and blk.valid = 'Y' ");
        sql.Append(" left join client_documents2 doc on cleave.client_id = doc.client_id and doc.valid = 'Y'   ");
        sql.Append(" where per.client_id !='' and cleave.valid = 'Y' ");
        sql.Append(" and date(now()) >= (date(cleave.leave_out_datetime) + INTERVAL -7 DAY )");
        sql.Append(" and cleave.leave_state = 1 ");
        sql.Append("  ");
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }

        sql.Append(" ; ");

        return sql.ToString();
    }
    public static string get_icp_brief(string condition)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" select main.icp_id, IFNULL(concat(per.chi_surname,per.chi_name),''),main.client_id, ");
        sql.Append(" DATE_FORMAT(main.due_date, '%Y-%m-%d') as icp_date, ");
        sql.Append(" main.icp_type_id, ctype.assess_type, ");
        sql.Append(" main.icp_status, icpstatus.status_chi, ");
        sql.Append(" per.sex ,doc.client_photo_id");
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
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }
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
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }
        sql.Append(" order by icp_date ; ");

        return sql.ToString();
    }

    public static string get_iot_ble_device_info(string device_id) // Raymond @20210209 ble device
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(" SELECT iot_map.device_name_chi, iot_map.type_id, iot_type.type_name_chi, iot_map.device_mac_address, iot_map.supplier_id, iot_suppier.supplier_name_chi ");
        sql.Append(" FROM sys_iot_ble_device_mapping iot_map ");
        sql.Append(" left join sys_iot_ble_device_type iot_type on iot_map.type_id = iot_type.type_id and iot_type.valid = 'Y' ");
        sql.Append(" left join sys_iot_ble_device_supplier iot_suppier on iot_map.supplier_id = iot_suppier.supplier_id and iot_suppier.valid = 'Y' ");
        sql.Append(" WHERE iot_map.device_id = '{0}' ");

        string comm = string.Format(sql.ToString(), device_id);
        return comm;
    }
}

