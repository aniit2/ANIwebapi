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
internal class CWound  
{

    public static string get_wound_brief(string condition)
    {

        StringBuilder sql = new StringBuilder();
        /*
       SELECT* FROM azure.care_wound2;
       care_wound_id, client_id, event_id, recovery, wound_type,
       clean_frequency, discover_photo_id, wound_position_id,
       wound_discover_date, wound_report_source, wound_recover_date,
       wound_remark, next_clean_datetime, start_datetime, end_datetime,
       valid, created_by, created_datetime, modified_by, modified_datetime


SELECT* FROM azure.care_wound_position;
       wound_position_id, wound_direction, position_chi_name, position_eng_name
       */
      //  SELECT* FROM tinkaping.care_wound_observation2;
     //   observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width, wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency,
    //    wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark



        sql.Append("select wound.care_wound_id,IFNULL(concat(per.chi_surname,per.chi_name),''),wound.client_id,");
        sql.Append(" concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),wound.recovery,");
        sql.Append(" wound.wound_position_id,pos.position_chi_name, concat(wound.clean_days,'天 ',wound.clean_times,'次'),");
        sql.Append(" wound.clean_days,wound.clean_times,");

        sql.Append("DATE_FORMAT(wound.next_clean_datetime, '%Y-%m-%d %H:%i'),");
     
        sql.Append("DATE_FORMAT(wound.start_datetime, '%Y-%m-%d'),");
        sql.Append("DATE_FORMAT(wound.end_datetime, '%Y-%m-%d'),wound.wound_report_source ,count(DISTINCT obs.observation_id),");
      //  sql.Append(" group_concat(DISTINCT dres.preset_id,',', dres.care_wound_material_id order by dres.preset_id separator ';') ,");
        sql.Append(" group_concat(DISTINCT dres.preset_id,'@dress', dres.care_wound_material_id,'@dress', mat.wound_dressing_eng,'@dress',dres.remark order by dres.preset_id separator '@dress_sep@') ,");
        sql.Append("substring_index(group_concat(DISTINCT obs.observation_id order by obs.observation_date separator ';'),';', 5) as records,");

         
        sql.Append("  concat( obs1.wound_length,';', obs1.wound_width,';', obs1.wound_depth),obs1.wound_level,");
        sql.Append("  obs1.wound_color,  obs1.wound_fluid_type,  obs1.wound_smell,  obs1.fluid_quantity,  ");
        sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime , ");
        sql.Append("ifnull(doc.wound_photo_id,0)  ");
        //  SELECT* FROM azure.care_wound_observation2;
        //   observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width, wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency, wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark

        sql.Append("from care_wound2 wound ");
        sql.Append("LEFT JOIN care_wound_observation2 AS obs1 ON obs1.care_wound_id = wound.care_wound_id and obs1.valid='Y' ");
        sql.Append("and obs1.observation_date = (select max(observation_date) from care_wound_observation2 where care_wound_id = wound.care_wound_id and valid = 'Y' LIMIT 1)  ");

        sql.Append("LEFT JOIN client_personal2 AS per ON wound.client_id = per.client_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");

        sql.Append("LEFT JOIN care_wound_position AS pos ON wound.wound_position_id = pos.wound_position_id ");
        sql.Append("LEFT JOIN care_wound_observation2 AS obs ON obs.care_wound_id = wound.care_wound_id and obs.valid='Y' ");
         sql.Append("left join care_wound_preset_dressing AS dres on dres.care_wound_id = wound.care_wound_id and dres.valid='Y'   ");
        sql.Append("left join care_wound_dressing_material AS mat on dres.care_wound_material_id = mat.care_wound_material_id and mat.valid='Y'   ");
        // SELECT* FROM azure.care_wound_dressing_material;
        // care_wound_material_id, wound_dressing_eng, valid, ordering
        //SELECT* FROM azure.care_wound_preset_dressing;
        // preset_id, care_wound_id, care_wound_material_id, remark, modified_by, modified_datetime, created_by, created_datetime, valid
        // sql.Append("LEFT JOIN care_wound_document AS  ON obs.care_wound_id = wound.care_wound_id and obs.valid='Y' ");

        //SELECT* FROM tinkaping.care_wound_document;
        // wound_photo_id, observation_id, document_photo, valid
       // sql.Append("left join care_wound_document doc on obs.observation_id = doc.observation_id and doc.valid = 'Y'   ");
        sql.Append("LEFT JOIN care_wound_document AS doc on doc.valid ='Y' and  doc.observation_id =  obs1.observation_id   and  doc.ordering = ( ");
        // sql.Append("LEFT JOIN care_wound_document AS doc ON doc.observation_id = obs.observation_id and doc.valid= 'Y' ) ");
        sql.Append(" SELECT  min(ordering) FROM  care_wound_document WHERE valid = 'Y' and  observation_id = obs1.observation_id ORDER BY ordering ASC LIMIT 1)");

        sql.Append("where per.client_id !='' and wound.valid = 'Y'  ");
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }

        sql.Append("  group by wound.care_wound_id ");
        sql.Append("order by wound.next_clean_datetime , wound.wound_position_id ;");
        // return Cshare.GetDataSource(sql.ToString());

 

        sql.Append(get_wound_wash_parameter());



        string comm = string.Format(sql.ToString());
        return comm;


    }
    public static string get_wound_brief2(string condition)
    {

        StringBuilder sql = new StringBuilder();
        /*
       SELECT* FROM azure.care_wound2;
       care_wound_id, client_id, event_id, recovery, wound_type,
       clean_frequency, discover_photo_id, wound_position_id,
       wound_discover_date, wound_report_source, wound_recover_date,
       wound_remark, next_clean_datetime, start_datetime, end_datetime,
       valid, created_by, created_datetime, modified_by, modified_datetime


SELECT* FROM azure.care_wound_position;
       wound_position_id, wound_direction, position_chi_name, position_eng_name
       */
        //  SELECT* FROM tinkaping.care_wound_observation2;
        //   observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width, wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency,
        //    wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark



        sql.Append("select wound.care_wound_id,IFNULL(concat(per.chi_surname,per.chi_name),''),wound.client_id,");
        sql.Append(" concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),wound.recovery,");
        sql.Append("wound.wound_position_remark, ");
        sql.Append(" wound.wound_position_id,pos.position_chi_name, concat(wound.clean_days,'天 ',wound.clean_times,'次'),");
        sql.Append(" wound.clean_days,wound.clean_times,");

        sql.Append("DATE_FORMAT(wound.next_clean_datetime, '%Y-%m-%d %H:%i'),");

        sql.Append("DATE_FORMAT(wound.start_datetime, '%Y-%m-%d'),");
        sql.Append("DATE_FORMAT(wound.end_datetime, '%Y-%m-%d'),wound.wound_report_source ,count(DISTINCT obs.observation_id),");
        //  sql.Append(" group_concat(DISTINCT dres.preset_id,',', dres.care_wound_material_id order by dres.preset_id separator ';') ,");
        sql.Append(" group_concat(DISTINCT dres.preset_id,'@dress', dres.care_wound_material_id,'@dress', mat.wound_dressing_eng,'@dress',dres.remark order by dres.preset_id separator '@dress_sep@') ,");
        sql.Append("substring_index(group_concat(DISTINCT obs.observation_id order by obs.observation_date separator ';'),';', 5) as records,");


        sql.Append("  concat( obs1.wound_length,';', obs1.wound_width,';', obs1.wound_depth),obs1.wound_level,");
        sql.Append("  obs1.wound_color,  obs1.wound_fluid_type,  obs1.wound_smell,  obs1.fluid_quantity,  ");
        sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime , ");
        sql.Append("ifnull(doc.wound_photo_id,0) , ");
        sql.Append("pos.applocx, pos.applocy,pos.wound_direction,pos.position_chi_name ");

        //    record_id, wound_position_id, wound_direction, position_chi_name, position_eng_name, winlocx, winlocy, 
        ///applocx, applocy, valid, modified_by, modified_datetime, created_by, created_datetime

        //  SELECT* FROM azure.care_wound_observation2;
        //   observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width, wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency, wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark

        sql.Append("from care_wound2 wound ");
        sql.Append("LEFT JOIN care_wound_observation2 AS obs1 ON obs1.care_wound_id = wound.care_wound_id and obs1.valid='Y' ");
        sql.Append("and obs1.observation_date = (select max(observation_date) from care_wound_observation2 where care_wound_id = wound.care_wound_id and valid = 'Y' LIMIT 1)  ");

        sql.Append("LEFT JOIN client_personal2 AS per ON wound.client_id = per.client_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");

        sql.Append("LEFT JOIN care_wound_position AS pos ON wound.wound_position_id = pos.wound_position_id and pos.valid = 'Y' ");
        sql.Append("LEFT JOIN care_wound_observation2 AS obs ON obs.care_wound_id = wound.care_wound_id and obs.valid='Y' ");
        sql.Append("left join care_wound_preset_dressing AS dres on dres.care_wound_id = wound.care_wound_id and dres.valid='Y'   ");
        sql.Append("left join care_wound_dressing_material AS mat on dres.care_wound_material_id = mat.care_wound_material_id and mat.valid='Y'   ");
        // SELECT* FROM azure.care_wound_dressing_material;
        // care_wound_material_id, wound_dressing_eng, valid, ordering
        //SELECT* FROM azure.care_wound_preset_dressing;
        // preset_id, care_wound_id, care_wound_material_id, remark, modified_by, modified_datetime, created_by, created_datetime, valid
        // sql.Append("LEFT JOIN care_wound_document AS  ON obs.care_wound_id = wound.care_wound_id and obs.valid='Y' ");

        //SELECT* FROM tinkaping.care_wound_document;
        // wound_photo_id, observation_id, document_photo, valid
        // sql.Append("left join care_wound_document doc on obs.observation_id = doc.observation_id and doc.valid = 'Y'   ");
        sql.Append("LEFT JOIN care_wound_document AS doc on doc.valid ='Y' and  doc.observation_id =  obs1.observation_id   and  doc.ordering = ( ");
        // sql.Append("LEFT JOIN care_wound_document AS doc ON doc.observation_id = obs.observation_id and doc.valid= 'Y' ) ");
        sql.Append(" SELECT  min(ordering) FROM  care_wound_document WHERE valid = 'Y' and  observation_id = obs1.observation_id ORDER BY ordering ASC LIMIT 1)");

        sql.Append("where per.client_id !='' and wound.valid = 'Y'  ");
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }

        sql.Append("  group by wound.care_wound_id ");
        sql.Append("order by wound.next_clean_datetime , wound.wound_position_id ;");
        // return Cshare.GetDataSource(sql.ToString());



        sql.Append(get_wound_wash_parameter());



        string comm = string.Format(sql.ToString());
        return comm;


    }
    //Raymond @20201015
    public static string get_wound_brief3(string condition)
    {

        StringBuilder sql = new StringBuilder();
        /*
       SELECT* FROM azure.care_wound2;
       care_wound_id, client_id, event_id, recovery, wound_type,
       clean_frequency, discover_photo_id, wound_position_id,
       wound_discover_date, wound_report_source, wound_recover_date,
       wound_remark, next_clean_datetime, start_datetime, end_datetime,
       valid, created_by, created_datetime, modified_by, modified_datetime


SELECT* FROM azure.care_wound_position;
       wound_position_id, wound_direction, position_chi_name, position_eng_name
       */
        //  SELECT* FROM tinkaping.care_wound_observation2;
        //   observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width, wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency,
        //    wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark



        sql.Append("select wound.care_wound_id,IFNULL(concat(per.chi_surname,per.chi_name),''),wound.client_id,");
        sql.Append(" concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),wound.recovery,");
        sql.Append("wound.wound_position_remark, ");
        sql.Append(" wound.wound_position_id,pos.position_chi_name, concat(wound.clean_days,'天 ',wound.clean_times,'次'),");
        sql.Append(" wound.clean_days,wound.clean_times,");

        sql.Append("DATE_FORMAT(wound.next_clean_datetime, '%Y-%m-%d %H:%i'),");

        sql.Append("DATE_FORMAT(wound.start_datetime, '%Y-%m-%d'),");
        sql.Append("DATE_FORMAT(wound.end_datetime, '%Y-%m-%d'),wound.wound_report_source ,count(DISTINCT obs.observation_id),");
        //  sql.Append(" group_concat(DISTINCT dres.preset_id,',', dres.care_wound_material_id order by dres.preset_id separator ';') ,");
        sql.Append(" group_concat(DISTINCT dres.preset_id,'@dress', dres.care_wound_material_id,'@dress', mat.wound_dressing_eng,'@dress',dres.remark order by dres.preset_id separator '@dress_sep@') ,");
        sql.Append("substring_index(group_concat(DISTINCT obs.observation_id order by obs.observation_date separator ';'),';', 5) as records,");


        sql.Append("  concat( obs1.wound_length,';', obs1.wound_width,';', obs1.wound_depth),obs1.wound_level,");
        sql.Append("  obs1.wound_color,  obs1.wound_fluid_type,  obs1.wound_smell,  obs1.fluid_quantity,  ");
        sql.Append("per.sex , wound.created_by, wound.created_datetime, wound.modified_by, wound.modified_datetime , ");
        sql.Append("ifnull(doc.wound_photo_id,0)  ");
        //  SELECT* FROM azure.care_wound_observation2;
        //   observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width, wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency, wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark

        sql.Append("from care_wound2 wound ");
        sql.Append("LEFT JOIN care_wound_observation2 AS obs1 ON obs1.care_wound_id = wound.care_wound_id and obs1.valid='Y' ");
        sql.Append("and obs1.observation_date = (select max(observation_date) from care_wound_observation2 where care_wound_id = wound.care_wound_id and valid = 'Y' LIMIT 1)  ");

        sql.Append("LEFT JOIN client_personal2 AS per ON wound.client_id = per.client_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");

        sql.Append("LEFT JOIN care_wound_position AS pos ON wound.wound_position_id = pos.wound_position_id ");
        sql.Append("LEFT JOIN care_wound_observation2 AS obs ON obs.care_wound_id = wound.care_wound_id and obs.valid='Y' ");
        sql.Append("left join care_wound_preset_dressing AS dres on dres.care_wound_id = wound.care_wound_id and dres.valid='Y'   ");
        sql.Append("left join care_wound_dressing_material AS mat on dres.care_wound_material_id = mat.care_wound_material_id and mat.valid='Y'   ");
        // SELECT* FROM azure.care_wound_dressing_material;
        // care_wound_material_id, wound_dressing_eng, valid, ordering
        //SELECT* FROM azure.care_wound_preset_dressing;
        // preset_id, care_wound_id, care_wound_material_id, remark, modified_by, modified_datetime, created_by, created_datetime, valid
        // sql.Append("LEFT JOIN care_wound_document AS  ON obs.care_wound_id = wound.care_wound_id and obs.valid='Y' ");

        //SELECT* FROM tinkaping.care_wound_document;
        // wound_photo_id, observation_id, document_photo, valid
        // sql.Append("left join care_wound_document doc on obs.observation_id = doc.observation_id and doc.valid = 'Y'   ");
        sql.Append("LEFT JOIN care_wound_document AS doc on doc.valid ='Y' and  doc.observation_id =  obs1.observation_id   and  doc.ordering = ( ");
        // sql.Append("LEFT JOIN care_wound_document AS doc ON doc.observation_id = obs.observation_id and doc.valid= 'Y' ) ");
        sql.Append(" SELECT  min(ordering) FROM  care_wound_document WHERE valid = 'Y' and  observation_id = obs1.observation_id ORDER BY ordering ASC LIMIT 1)");

        sql.Append("where per.client_id !='' and wound.valid = 'Y'  ");
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }

        sql.Append("  group by wound.care_wound_id ");
        sql.Append("order by wound.next_clean_datetime , wound.wound_position_id ;");
        // return Cshare.GetDataSource(sql.ToString());



        sql.Append(get_wound_wash_parameter_2());



        string comm = string.Format(sql.ToString());
        return comm;


    }

    public static string get_wound_wash_detail(string condition, bool need_photo = true)
    {

        /*
        SELECT* FROM azure.care_wound_observation2;
        observation_id, care_wound_id, observation_date, wound_level,
        wound_length, wound_width, wound_depth, wound_color, wound_fluid_type,
        wound_smell, fluid_quantity, frequency, wound_photo_id,
        modified_by, modified_datetime, created_by, created_datetime, valid, remark

            SELECT* FROM azure.care_wound_stage;
        care_wound_stage_id, wound_num_of_stage, wound_stage_chi, valid
            SELECT* FROM azure.care_wound_color;
        care_wound_color_id, wound_color_chi, valid, ordering
            SELECT* FROM azure.care_wound_smell;
        care_wound_smell_id, wound_smell_chi, valid, ordering


            SELECT* FROM azure.care_wound_fluid_type;
        care_wound_fluid_type_id, wound_fluid_type_chi, valid, ordering
            SELECT* FROM azure.care_wound_fluid_quantity;
        care_wound_fluid_quantity_id, wound_fluid_quantity_chi, valid, ordering
                                                                    //            care_wound_color
                                                                    //care_wound_fluid_type
                                                                    //care_wound_smell
                                                                    //care_wound_fluid_quantity
                                                                    */
        StringBuilder sql = new StringBuilder();
        sql.Append("select obs.observation_id , ");
        sql.Append(" DATE_FORMAT(obs.observation_date, '%Y-%m-%d %H:%i'),");

        sql.Append(" obs.wound_length , obs.wound_width , obs.wound_depth ,  ");

        sql.Append(" concat(stage_num_chi,' - ', stage.wound_stage_chi),color.wound_color_chi,smell.wound_smell_chi, ");

        sql.Append(" fluty.wound_fluid_type_chi,");
        sql.Append(" quan.wound_fluid_quantity_chi, ");
        sql.Append(" concat(obs.clean_days,'天 ',obs.clean_times,'次'), ");
        sql.Append(" group_concat(DISTINCT dress.dressing_id,'@dress', dress.care_wound_material_id,'@dress', mat.wound_dressing_eng,'@dress',dress.remark order by dress.dressing_id separator '@dress_sep@') ,");
        //   sql.Append("GROUP_CONCAT(DISTINCT concat(mat.wound_dressing_eng,' ',dress.remark) separator '@dress_sep@'), ");
     //   SELECT* FROM wahhei.care_wound_observation_dressing;

     //   dressing_id, observation_id, care_wound_material_id, remark, modified_by, modified_datetime, created_by, created_datetime, valid


        sql.Append("  obs.remark, ");
        sql.Append("  obs.created_by,");
        sql.Append(" DATE_FORMAT(obs.created_datetime, '%Y-%m-%d %H:%i'), ");
        sql.Append("  obs.modified_by,");
        sql.Append(" DATE_FORMAT(obs.modified_datetime, '%Y-%m-%d %H:%i'),");

        sql.Append(" doc.wound_photo_id    ");
        if (need_photo)
        {
            sql.Append("  , doc.document_photo   ");
        }
        //  SELECT* FROM azure.care_wound_observation2;
        //   observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width, wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency, wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark

        sql.Append("from care_wound_observation2 obs ");

        sql.Append("LEFT JOIN care_wound_stage AS stage ON stage.care_wound_stage_id = obs.wound_level ");
        sql.Append("LEFT JOIN care_wound_color AS color ON color.care_wound_color_id = obs.wound_color ");
        sql.Append("LEFT JOIN care_wound_smell AS smell ON smell.care_wound_smell_id = obs.wound_smell ");
        sql.Append("LEFT JOIN care_wound_fluid_type AS fluty ON fluty.care_wound_fluid_type_id = obs.wound_fluid_type ");
        sql.Append("LEFT JOIN care_wound_fluid_quantity AS quan ON quan.care_wound_fluid_quantity_id = obs.fluid_quantity ");
        sql.Append("LEFT JOIN care_wound_document AS doc ON doc.observation_id = obs.observation_id and doc.valid= 'Y'  ");

        sql.Append("LEFT JOIN care_wound_observation_dressing AS dress ON obs.observation_id = dress.observation_id and dress.valid = 'Y' ");

        sql.Append("LEFT JOIN care_wound_dressing_material AS mat ON mat.care_wound_material_id = dress.care_wound_material_id ");




        sql.Append("where obs.observation_id !='' and obs.valid = 'Y' ");
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }

        /// sql.Append("  group by wound.care_wound_id ");
        //  sql.Append("order by wound.next_clean_datetime  ;");
        string comm = string.Format(sql.ToString());
        return comm;


    }
    public static string get_wound_wash_detail2(string condition, bool need_photo = false)
    {

        /*
        SELECT* FROM azure.care_wound_observation2;
        observation_id, care_wound_id, observation_date, wound_level,
        wound_length, wound_width, wound_depth, wound_color, wound_fluid_type,
        wound_smell, fluid_quantity, frequency, wound_photo_id,
        modified_by, modified_datetime, created_by, created_datetime, valid, remark

            SELECT* FROM azure.care_wound_stage;
        care_wound_stage_id, wound_num_of_stage, wound_stage_chi, valid
            SELECT* FROM azure.care_wound_color;
        care_wound_color_id, wound_color_chi, valid, ordering
            SELECT* FROM azure.care_wound_smell;
        care_wound_smell_id, wound_smell_chi, valid, ordering


            SELECT* FROM azure.care_wound_fluid_type;
        care_wound_fluid_type_id, wound_fluid_type_chi, valid, ordering
            SELECT* FROM azure.care_wound_fluid_quantity;
        care_wound_fluid_quantity_id, wound_fluid_quantity_chi, valid, ordering
                                                                    //            care_wound_color
                                                                    //care_wound_fluid_type
                                                                    //care_wound_smell
                                                                    //care_wound_fluid_quantity
                                                                    */
        StringBuilder sql = new StringBuilder();
        sql.Append("select obs.observation_id , ");
        sql.Append(" DATE_FORMAT(obs.observation_date, '%Y-%m-%d %H:%i'),");

        sql.Append(" obs.wound_length , obs.wound_width , obs.wound_depth ,  ");

        sql.Append(" concat(stage_num_chi,' - ', stage.wound_stage_chi),color.wound_color_chi,smell.wound_smell_chi, ");

        sql.Append(" fluty.wound_fluid_type_chi,");
        sql.Append(" quan.wound_fluid_quantity_chi, ");
        sql.Append(" concat(obs.clean_days,'天 ',obs.clean_times,'次'), ");
        sql.Append(" group_concat(DISTINCT dress.dressing_id,'@dress', dress.care_wound_material_id,'@dress', mat.wound_dressing_eng,'@dress',dress.remark order by dress.dressing_id separator '@dress_sep@') ,");
        //   sql.Append("GROUP_CONCAT(DISTINCT concat(mat.wound_dressing_eng,' ',dress.remark) separator '@dress_sep@'), ");
        //   SELECT* FROM wahhei.care_wound_observation_dressing;

        //   dressing_id, observation_id, care_wound_material_id, remark, modified_by, modified_datetime, created_by, created_datetime, valid


        sql.Append("  obs.remark, ");
        sql.Append("  obs.created_by,");
        sql.Append(" DATE_FORMAT(obs.created_datetime, '%Y-%m-%d %H:%i'), ");
        sql.Append("  obs.modified_by,");
        sql.Append(" DATE_FORMAT(obs.modified_datetime, '%Y-%m-%d %H:%i'),");

      //  sql.Append(" doc.wound_photo_id    ");

        sql.Append("GROUP_CONCAT(DISTINCT doc.wound_photo_id ");
        sql.Append("ORDER BY doc.ordering ASC SEPARATOR ';' )  ");


        if (need_photo)
        {
            sql.Append("  , doc.document_photo   ");
        }
        //  SELECT* FROM azure.care_wound_observation2;
        //   observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width, wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency, wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark

        sql.Append("from care_wound_observation2 obs ");

        sql.Append("LEFT JOIN care_wound_stage AS stage ON stage.care_wound_stage_id = obs.wound_level ");
        sql.Append("LEFT JOIN care_wound_color AS color ON color.care_wound_color_id = obs.wound_color ");
        sql.Append("LEFT JOIN care_wound_smell AS smell ON smell.care_wound_smell_id = obs.wound_smell ");
        sql.Append("LEFT JOIN care_wound_fluid_type AS fluty ON fluty.care_wound_fluid_type_id = obs.wound_fluid_type ");
        sql.Append("LEFT JOIN care_wound_fluid_quantity AS quan ON quan.care_wound_fluid_quantity_id = obs.fluid_quantity ");
        sql.Append("LEFT JOIN care_wound_document AS doc ON doc.observation_id = obs.observation_id and doc.valid= 'Y'  ");

        sql.Append("LEFT JOIN care_wound_observation_dressing AS dress ON obs.observation_id = dress.observation_id and dress.valid = 'Y' ");

        sql.Append("LEFT JOIN care_wound_dressing_material AS mat ON mat.care_wound_material_id = dress.care_wound_material_id ");




        sql.Append("where obs.observation_id !='' and obs.valid = 'Y' ");
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }
        sql.Append(" ");
        /// sql.Append("  group by wound.care_wound_id ");
        //  sql.Append("order by wound.next_clean_datetime  ;");
        string comm = string.Format(sql.ToString());
        return comm;


    }
    public static string get_wound_photo(string wound_id)
    {

        StringBuilder sql = new StringBuilder();

      //  SELECT* FROM azure.care_wound_document;
      //  wound_photo_id, observation_id, document_photo, valid
        // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
        sql.Append("select document_photo  from care_wound_document Where wound_photo_id = '{0}' and valid = 'Y';");
        string comm = string.Format(sql.ToString(), wound_id);

        return comm;


    }
    public static string get_wound_wash_parameter(string condition = "")
    {


        StringBuilder sql = new StringBuilder();
        sql.Append("select ");
        sql.Append(" care_wound_stage_id, concat(stage_num_chi,' - ', wound_stage_chi) ");
        sql.Append("from care_wound_stage ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by wound_num_of_stage ;");




        sql.Append("select ");
        sql.Append(" care_wound_color_id, wound_color_chi ");
        sql.Append("from care_wound_color ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by ordering ;");


        sql.Append("select ");
        sql.Append(" care_wound_fluid_type_id, wound_fluid_type_chi ");
        sql.Append("from care_wound_fluid_type ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by ordering ;");

        sql.Append("select ");
        sql.Append("care_wound_smell_id, wound_smell_chi ");
        sql.Append("from care_wound_smell ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by ordering ;");

        sql.Append("select ");
        sql.Append("care_wound_fluid_quantity_id, wound_fluid_quantity_chi ");
        sql.Append("from care_wound_fluid_quantity ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by ordering ;");
        // wound_position_id, wound_direction, position_chi_name, position_eng_name


        if (condition.Length > 0)
        {
            sql.Append(condition);
        }
        string comm = string.Format(sql.ToString());
        return comm;

    }

    // Raymond @20201015
    public static string get_wound_wash_parameter_2(string condition = "")
    {


        StringBuilder sql = new StringBuilder();
        sql.Append("select ");
        sql.Append(" care_wound_stage_id, stage_num_chi, wound_stage_chi ");
        sql.Append("from care_wound_stage ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by wound_num_of_stage ;");




        sql.Append("select ");
        sql.Append(" care_wound_color_id, wound_color_chi ");
        sql.Append("from care_wound_color ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by ordering ;");


        sql.Append("select ");
        sql.Append(" care_wound_fluid_type_id, wound_fluid_type_chi ");
        sql.Append("from care_wound_fluid_type ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by ordering ;");

        sql.Append("select ");
        sql.Append("care_wound_smell_id, wound_smell_chi ");
        sql.Append("from care_wound_smell ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by ordering ;");

        sql.Append("select ");
        sql.Append("care_wound_fluid_quantity_id, wound_fluid_quantity_chi ");
        sql.Append("from care_wound_fluid_quantity ");
        sql.Append(" where valid='Y' ");
        sql.Append("order by ordering ;");
        // wound_position_id, wound_direction, position_chi_name, position_eng_name


        if (condition.Length > 0)
        {
            sql.Append(condition);
        }
        string comm = string.Format(sql.ToString());
        return comm;

    }

    public static string insert_wound_wash(string[] vals, string[][] dressing ,string dress_id)
    {
        StringBuilder sql = new StringBuilder();

        ///         SELECT* FROM azure.care_wound_observation2;
        ///         observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width,
        //wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency, 
        //wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark

        sql.Append("insert care_wound_observation2( observation_id, care_wound_id, ");
        sql.Append("observation_date, wound_length, wound_width, ");
        sql.Append(" wound_depth,wound_level, wound_color,wound_smell, wound_fluid_type,  ");
        sql.Append("fluid_quantity, clean_days,clean_times ,remark,  ");
        sql.Append("created_by, created_datetime) ");


        sql.AppendFormat("values({0},'{1}','{2}','{3}','{4}','{5}', ", vals);
        sql.AppendFormat("'{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}');", vals);

        sql.Append(insert_wound_observation_dressing_sql(vals, dressing ,dress_id));
 

        return  string.Format(sql.ToString(), vals);
    }
    public static string update_wound(int index, string[] vals )
    {
        // SELECT* FROM azure.care_wound_preset_dressing;
        // preset_id, care_wound_id, care_wound_material_id, remark, modified_by, modified_datetime, created_by, created_datetime, valid



        ///         SELECT* FROM azure.care_wound_observation2;
        ///         observation_id, care_wound_id, observation_date, wound_level, wound_length, wound_width,
        //wound_depth, wound_color, wound_fluid_type, wound_smell, fluid_quantity, frequency, 
        //wound_photo_id, modified_by, modified_datetime, created_by, created_datetime, valid, remark

        string[] column = new string[] { "recovery", "next_clean_datetime", "clean_frequency" };
        StringBuilder sql = new StringBuilder();
        //menu_id, menu_name, start_datetime, end_datetime, created_by, created_datetime, modified_by, modified_datetime, valid
        sql.Append("update care_wound2 set " + column[index] + " = '{1}',");
        sql.Append("modified_by = '{2}', modified_datetime  = '{3}' ");
        sql.Append("where care_wound_id = {0} ;  ");
        //sql.AppendFormat("values({0},'{1}','{2}','{3}','{4}','{5}','{6}',", vals);
        //sql.AppendFormat("'{7}','{8}','{9}','{10}','{11}','{13}','{14}');", vals);
        sql = new StringBuilder(string.Format(sql.ToString(), vals));
        return string.Format(sql.ToString(), vals);
    }
    public static string insert_wound_observation_dressing_sql(string[] vals, string[][] list = null ,string dress_id =null)
    {
        //    SELECT* FROM azure.care_wound_observation_dressing;
        //     dressing_id, observation_id, care_wound_material_id, modified_by, modified_datetime, created_by, created_datetime, valid
        StringBuilder sql = new StringBuilder();
        sql.Append("update care_wound_observation_dressing set valid = 'N' ,");
        sql.AppendFormat("modified_by  = '{0}',modified_datetime = '{1}' ", vals[14], vals[15]);
        sql.AppendFormat(" where observation_id in ({0}) and valid = 'Y' ;", vals[0]);
 
        if (list.Length > 0)
        {
          //  string record_id = Cshare.Get_mysql_database_MaxID("care_wound_observation_dressing", "dressing_id");
            sql.Append("insert care_wound_observation_dressing (dressing_id ,observation_id,care_wound_material_id,remark,created_by,created_datetime)values");

            for (int i = 0; i < list.Length; i++)
            {
                string[] array = list[i];
                string[] insert_values = new string[] { dress_id, vals[0], array[2], array[3], vals[14], vals[15] };
                sql.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}')", insert_values);
                if (i == list.Length - 1)
                {
                    sql.Append(";");
                }
                else
                {
                    sql.Append(",");
                }
                dress_id = (int.Parse(dress_id) + 1).ToString();
            }

        }
        return sql.ToString();
    }

    public static string post_wound_photo(string[] values)
    {
    //    SELECT* FROM wahhei.care_wound_document;
    //    wound_photo_id, observation_id, document_photo, valid
                StringBuilder sql = new StringBuilder();
        sql.AppendFormat("update care_wound_document  set valid= 'N' where observation_id= {0};", values[1]);


        sql.Append("Insert into care_wound_document(wound_photo_id,observation_id, document_photo) Values({0},{1},?parval);");
        // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
        // sql.Append("select document_photo  from client_documents2 Where client_photo_id = '{0}' and valid = 'Y';");
        string comm = string.Format(sql.ToString(), values);

        return comm;


    }
    public static string post_wound_photo2(string[] values)
    {
        //    SELECT* FROM wahhei.care_wound_document;
        //    wound_photo_id, observation_id, document_photo, valid
        StringBuilder sql = new StringBuilder();
        //sql.AppendFormat("update care_wound_document  set valid= 'N' where observation_id= {0};", values[1]);


        sql.Append("Insert into care_wound_document(wound_photo_id,observation_id,ordering,created_by,created_datetime,document_photo) Values({0},{1},{2},'{3}',now(),?parval);");
        // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
        // sql.Append("select document_photo  from client_documents2 Where client_photo_id = '{0}' and valid = 'Y';");
        string comm = string.Format(sql.ToString(), values);

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

