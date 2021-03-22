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
internal class Cclient  
{

    public static  string get_client_id_in_nfc(string   id)
    {

        StringBuilder sql = new StringBuilder();
        // sql.Append("insert acc_charge_consumed(charge_id, account_num, invoice_id, charge_item_id, charge_status, charge_quantity,");
        // sql.Append("charge_amount, charge_datetime, charge_item_remark, created_by, created_datetime)");
        // sql.Append("VALUES({0},'{1}',{2},{3},'{4}',{5},{6},'{7}','{8}','{9}','{10}');");
        sql.Append("select client_id, usage_code, usage_id,display_name from client_nfc_id Where nfc_id = '{0}' and valid = 'Y' ;");
        string comm = string.Format(sql.ToString(), id);

        return comm;


    }

        public static  string get_empty_bed(string   id)
    {

        StringBuilder sql = new StringBuilder();
        sql.Append("select * from company_bed Where bed_id =  '{0}';");
        string comm = string.Format(sql.ToString(), id);

        return comm;


    }
    public static string get_client_picture_id(string client_id)
    {

        StringBuilder sql = new StringBuilder();
        sql.AppendFormat("SELECT client_photo_id from client_documents2 Where client_id IN ({0}) and valid = 'Y'", client_id);
        return sql.ToString();


    }
    public static string get_empty_bed_info(string id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT  ");
        sql.Append(" concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value)  ");

 
        sql.Append("from  sys_company_bed b ");
     //   sql.Append("left join sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
 
        sql.Append("where b.bed_id = '{0}'; ");

 

        string comm = string.Format(sql.ToString(), id);
        return comm;
    }
    public static string get_client_briefing(string id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT CONCAT(per.chi_surname, per.chi_name),sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");

        sql.Append("CONCAT_WS(';', ifnull(abi.self_care_ability,' ') ,ifnull(tool.tchi_value,' '),ifnull(abi.eating_ability,' '),");
        sql.Append("ifnull(abi.cognitive_ability,' '), ifnull(abi.audiovisual_ability,' '),ifnull(abi.communicate_ability,' '),ifnull(abi.incontinence,' '), ability_remark), ");

        sql.Append("GROUP_CONCAT(care_wound_id),  ");
        sql.Append("ae.ae_status,");
        // sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");
  
        sql.Append(" group_concat(DISTINCT dt.type_name,' ', diet.tchi_value separator '\n') ,");

        sql.Append("doc.client_photo_id,per.contact_precaution,lift.tchi_value,tool.tchi_value ,sysres.tchi_value,");
        sql.Append("group_concat(DISTINCT eve.tchi_value,'', memo.remark separator ' '),sysdia.tchi_value,    ");
        sql.Append("group_concat(DISTINCT res.restraint_items,'', res.other_restraint_items,' ',res.restraint_items_remarks separator ' '),  ");
        sql.Append(" if(per.contact_precaution='Y','CT',''),if(abi.oxygen='Y','OO',''),  (SELECT icon_code FROM sys_display_panel_bed_icon WHERE tchi_value=abi.teeth and type_id = 1) AS teeth ,per.assessment_result  , din.dining_name  ");
        sql.Append(" FROM client_personal2 per ");
        //  sql.Append(" client_personal Where client_id=@id");
        //   SELECT* FROM greenery.medical_ae; ae_id, client_id, ae_status, ae_in_reason, ae_in_datetime, ae_in_remark, hospitalized, 
        //   ae_in_bed_num, ae_out_reason, ae_addr_id, ae_out_datetime, ae_out_remark, created_by, created_datetime, modified_by, modified_datetime, valid

 
        //     IF(a.addressid IS NULL, 0, 1)

        // SELECT* FROM greenery.client_diet; client_diet_id, client_id,
        //  prefered_rice, prefered_groceries, prefered_thickener, diet_preference, created_by, created_datetime, valid

        sql.Append("left join client_ability abi on abi.client_id = per.client_id and abi.valid='Y' ");
        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
        sql.Append("left join care_wound3 wou on wou.client_id = per.client_id and recovery ='N'   ");
        sql.Append("left join medical_ae ae on ae.client_id = per.client_id and ae_status = '留醫中' and   ae.valid  = 'Y'  ");
       // sql.Append("left join client_diet diet on diet.client_id = per.client_id and  diet.valid  = 'Y'  ");

 
        sql.Append(" LEFT JOIN client_diet_preference AS pre ON per.client_id = pre.client_id and pre.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_preference AS diet ON diet.diet_id = pre.diet_id and diet.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_type AS dt ON dt.diet_type_id = diet.diet_type_id   ");

          
        sql.Append("left join client_responsible_record respon on per.client_id = respon.client_id and respon.valid ='Y'  ");
        sql.Append("left join sys_client_responsible sysres on sysres.responsible_id = respon.responsible_id and sysres.valid ='Y'   ");

        sql.Append(" LEFT JOIN sys_client_help_lifting AS lift ON abi.help_lifting = lift.lifting_id and lift.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_assisting_tools AS tool ON abi.assisting_tools = tool.tools_id and tool.valid = 'Y'  ");

        sql.Append(" LEFT JOIN client_memo_event AS memo ON memo.client_id = per.client_id and memo.valid = 'Y'  ");
        sql.Append(" and ( date( memo.end_datetime)>= curdate() or date( memo.end_datetime) = date('1900-05-16')) ");
    
        sql.Append("LEFT JOIN sys_client_memo_event eve  on eve.memo_event_id = memo.memo_event_id ");

        sql.Append("LEFT JOIN client_diaper_event dia  on dia.client_id = per.client_id and dia.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diaper AS sysdia ON sysdia.diaper_id = dia.diaper_id and sysdia.valid = 'Y' ");

 
        sql.Append("LEFT JOIN ");
        sql.Append(" sys_client_dining din  ON abi.dining_id = din.dining_id and din.valid = 'Y' ");


        sql.Append("LEFT JOIN nursing_restraint res  on res.client_id = per.client_id and res.valid = 'Y'   ");
        sql.Append("and (res.restraint_state ='親屬口頭同意' or res.restraint_state = '有效' ) ");
       

    //    restraint_state
//親屬口頭同意
//有效
        //   SELECT* FROM azure.nursing_restraint;
        //    restraint_items


        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("where per.client_id = '{0}' ;  ");
        
 
        sql.Append("SELECT CONCAT(per.chi_surname,per.chi_name),CONCAT(per.dob),per.sex,concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value), ");
        sql.Append("concat( cast(m.sickness_brief as char) ),");
        sql.Append("GROUP_CONCAT(DISTINCT g.eng_value SEPARATOR ';'),CAST(m.diagnosis1 as char),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name_eng separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name_eng separator ';'),m.other_allergen,");
        sql.Append("m.allergen_remark,per.assessment_result ,CONCAT(per.eng_surname,' ',per.eng_name),per.hkid,per.client_number   FROM client_personal2 AS per ");

        sql.Append("LEFT JOIN client_documents2 AS c ON per.client_id = c.client_id and c.valid = 'Y' ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = per.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine2 h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine2 d on m.drug_allergen REGEXP d.medicine_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append(" where per.client_id = '{0}'  ");
        sql.Append(" ;");

 

        string comm = string.Format(sql.ToString(), id);
        return comm;
    }
    public static string get_client_briefing2_old(string id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT CONCAT(per.chi_surname, per.chi_name),sex,DATE_FORMAT(dob, '%Y/%m/%d'),");
        sql.Append("YEAR(CURRENT_TIMESTAMP) - YEAR(per.dob) - (RIGHT(CURRENT_TIMESTAMP, 5) < RIGHT(per.dob, 5)) as age,  ");
        sql.Append("concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),per.assessment_result , din.dining_name , ");
        sql.Append(" group_concat(DISTINCT dt.type_name,' ', diet.tchi_value separator '\n') ,");
        sql.Append(" ifnull(tool.tchi_value,' '),");

        sql.Append("ifnull(abi.self_care_ability,' ')   ,ifnull(abi.eating_ability,' '),");
        sql.Append("ifnull(abi.cognitive_ability,' '),ifnull(abi.communicate_ability,' '), ifnull(abi.visual_ability,' '),ifnull(abi.audio_ability,' '), ifnull(abi.incontinence,' '),");
        sql.Append("group_concat(DISTINCT sysdia.tchi_value separator '\n'),     ");
        // client_ability_id, client_id, self_care_ability, movement_ability, eating_ability,
        // cognitive_ability, audiovisual_ability, communicate_ability, fall_risk, incontinence, ability_remark,
        // created_by, created_datetime, valid, assisting_tools, help_lifting, audio_ability, visual_ability, oxygen, teeth, dining_id

 
        sql.Append("ae.ae_status,");
        sql.Append("abi.teeth,");
        // sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");



        sql.Append("doc.client_photo_id,lift.tchi_value,  ");
        sql.Append("group_concat(DISTINCT eve.tchi_value,'', memo.remark separator '\n'),    ");
        sql.Append("group_concat(DISTINCT  sysrt.tchi_value, '' separator ';'),   ");
        //sql.Append("group_concat(DISTINCT  sysrt.tchi_value , '',res.restraint_items,'', res.other_restraint_items,' ',res.restraint_items_remarks separator ' '),  ");
        sql.Append("GROUP_CONCAT(DISTINCT pos.position_chi_name separator ','),  ");
        sql.Append("per.contact_precaution,abi.oxygen   ");

        sql.Append(" FROM client_personal2 per ");
        //  sql.Append(" client_personal Where client_id=@id");
        //   SELECT* FROM greenery.medical_ae; ae_id, client_id, ae_status, ae_in_reason, ae_in_datetime, ae_in_remark, hospitalized, 
        //   ae_in_bed_num, ae_out_reason, ae_addr_id, ae_out_datetime, ae_out_remark, created_by, created_datetime, modified_by, modified_datetime, valid


        //     IF(a.addressid IS NULL, 0, 1)

        // SELECT* FROM greenery.client_diet; client_diet_id, client_id,
        //  prefered_rice, prefered_groceries, prefered_thickener, diet_preference, created_by, created_datetime, valid

        sql.Append("left join client_ability abi on abi.client_id = per.client_id and abi.valid='Y' ");
        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
        sql.Append("left join care_wound2 wou on wou.client_id = per.client_id and recovery ='N'   ");
        sql.Append("left join medical_ae ae on ae.client_id = per.client_id and ae_status = '留醫中' and   ae.valid  = 'Y'  ");
        // sql.Append("left join client_diet diet on diet.client_id = per.client_id and  diet.valid  = 'Y'  ");
        sql.Append(" LEFT JOIN care_wound_position pos ON wou.wound_position_id = pos.wound_position_id   ");
 
        sql.Append(" LEFT JOIN client_diet_preference AS pre ON per.client_id = pre.client_id and pre.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_preference AS diet ON diet.diet_id = pre.diet_id and diet.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_type AS dt ON dt.diet_type_id = diet.diet_type_id   ");


        sql.Append("left join client_responsible_record respon on per.client_id = respon.client_id and respon.valid ='Y'  ");
        sql.Append("left join sys_client_responsible sysres on sysres.responsible_id = respon.responsible_id and sysres.valid ='Y'   ");

        sql.Append(" LEFT JOIN sys_client_help_lifting AS lift ON abi.help_lifting = lift.lifting_id and lift.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_assisting_tools AS tool ON abi.assisting_tools = tool.tools_id and tool.valid = 'Y'  ");

        sql.Append(" LEFT JOIN client_memo_event AS memo ON memo.client_id = per.client_id and memo.valid = 'Y'  ");
        sql.Append(" and ( date( memo.end_datetime)>= curdate() or date( memo.end_datetime) = date('1900-05-16')) ");

        sql.Append("LEFT JOIN sys_client_memo_event eve  on eve.memo_event_id = memo.memo_event_id ");

        sql.Append("LEFT JOIN client_diaper_event dia  on dia.client_id = per.client_id and dia.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diaper AS sysdia ON sysdia.diaper_id = dia.diaper_id and sysdia.valid = 'Y' ");


        sql.Append("LEFT JOIN ");
        sql.Append(" sys_client_dining din  ON abi.dining_id = din.dining_id and din.valid = 'Y' ");


        sql.Append("LEFT JOIN nursing_restraint res  on res.client_id = per.client_id and res.valid = 'Y'   ");
        sql.Append("and (res.restraint_state ='親屬口頭同意' or res.restraint_state = '有效' ) ");
        sql.Append(" left join nursing_restraint_item_record res_item on res_item.restraint_id = res.restraint_id and res_item.valid = 'Y' ");
        sql.Append(" left join sys_restraint_item sysrt on sysrt.restraint_item_id = res_item.restraint_item_id and sysrt.valid = 'Y' ");

        //    restraint_state
        //親屬口頭同意
        //有效
        //   SELECT* FROM azure.nursing_restraint;
        //    restraint_items


        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("where per.client_id = '{0}' ;  ");
        

        sql.Append("SELECT CONCAT(per.chi_surname,per.chi_name),CONCAT(per.dob),per.sex,concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value), ");
        sql.Append("concat( cast(m.sickness_brief as char) ),");
        sql.Append("GROUP_CONCAT(DISTINCT g.eng_value SEPARATOR ';'),CAST(m.diagnosis1 as char),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name_eng separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name_eng separator ';'),m.other_allergen,");
        sql.Append("m.allergen_remark,per.assessment_result ,CONCAT(per.eng_surname,' ',per.eng_name),per.hkid,per.client_number   FROM client_personal2 AS per ");

        sql.Append("LEFT JOIN client_documents2 AS c ON per.client_id = c.client_id and c.valid = 'Y' ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = per.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine2 h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine2 d on m.drug_allergen REGEXP d.medicine_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append(" where per.client_id = '{0}'  ");
        sql.Append(" ;");



        string comm = string.Format(sql.ToString(), id);
        return comm;
    }
    public static string get_client_briefing3_old(string id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT CONCAT(per.chi_surname, per.chi_name),sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");
        sql.Append("YEAR(CURRENT_TIMESTAMP) - YEAR(per.dob) - (RIGHT(CURRENT_TIMESTAMP, 5) < RIGHT(per.dob, 5)) as age,  ");

        sql.Append("CONCAT_WS(';', ifnull(abi.self_care_ability,' ') ,ifnull(tool.tchi_value,' '),ifnull(abi.eating_ability,' '),");
        sql.Append("ifnull(abi.cognitive_ability,' '), ifnull(abi.audiovisual_ability,' '),ifnull(abi.communicate_ability,' '),ifnull(abi.incontinence,' '), ability_remark), ");

        sql.Append(" GROUP_CONCAT(DISTINCT pos.position_chi_name separator ','),  ");
        sql.Append("ae.ae_status,");
        // sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");

        sql.Append(" group_concat(DISTINCT dt.type_name,' ', diet.tchi_value separator '\n') ,");

        sql.Append("doc.client_photo_id,per.contact_precaution,lift.tchi_value,tool.tchi_value ,sysres.tchi_value,");
        sql.Append("group_concat(DISTINCT eve.tchi_value,'', memo.remark separator ' '),sysdia.tchi_value,    ");
        sql.Append("group_concat(DISTINCT  sysrt.tchi_value, '' separator ';'),   ");
        sql.Append(" if(per.contact_precaution='Y','CT',''),if(abi.oxygen='Y','OO',''),  (SELECT icon_code FROM sys_display_panel_bed_icon WHERE tchi_value=abi.teeth and type_id = 1) AS teeth ,per.assessment_result  , din.dining_name  ");
        sql.Append(" FROM client_personal2 per ");
        //  sql.Append(" client_personal Where client_id=@id");
        //   SELECT* FROM greenery.medical_ae; ae_id, client_id, ae_status, ae_in_reason, ae_in_datetime, ae_in_remark, hospitalized, 
        //   ae_in_bed_num, ae_out_reason, ae_addr_id, ae_out_datetime, ae_out_remark, created_by, created_datetime, modified_by, modified_datetime, valid


        //     IF(a.addressid IS NULL, 0, 1)

        // SELECT* FROM greenery.client_diet; client_diet_id, client_id,
        //  prefered_rice, prefered_groceries, prefered_thickener, diet_preference, created_by, created_datetime, valid

        sql.Append("left join client_ability abi on abi.client_id = per.client_id and abi.valid='Y' ");
        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
        sql.Append("left join care_wound2 wou on wou.client_id = per.client_id and recovery ='N'   ");
        sql.Append("left join medical_ae ae on ae.client_id = per.client_id and ae_status = '留醫中' and   ae.valid  = 'Y'  ");
         sql.Append(" LEFT JOIN care_wound_position pos ON wou.wound_position_id = pos.wound_position_id    ");


        sql.Append(" LEFT JOIN client_diet_preference AS pre ON per.client_id = pre.client_id and pre.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_preference AS diet ON diet.diet_id = pre.diet_id and diet.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_type AS dt ON dt.diet_type_id = diet.diet_type_id   ");


        sql.Append("left join client_responsible_record respon on per.client_id = respon.client_id and respon.valid ='Y'  ");
        sql.Append("left join sys_client_responsible sysres on sysres.responsible_id = respon.responsible_id and sysres.valid ='Y'   ");

        sql.Append(" LEFT JOIN sys_client_help_lifting AS lift ON abi.help_lifting = lift.lifting_id and lift.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_assisting_tools AS tool ON abi.assisting_tools = tool.tools_id and tool.valid = 'Y'  ");

        sql.Append(" LEFT JOIN client_memo_event AS memo ON memo.client_id = per.client_id and memo.valid = 'Y'  ");
        sql.Append(" and ( date( memo.end_datetime)>= curdate() or date( memo.end_datetime) = date('1900-05-16')) ");

        sql.Append("LEFT JOIN sys_client_memo_event eve  on eve.memo_event_id = memo.memo_event_id ");

        sql.Append("LEFT JOIN client_diaper_event dia  on dia.client_id = per.client_id and dia.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diaper AS sysdia ON sysdia.diaper_id = dia.diaper_id and sysdia.valid = 'Y' ");


        sql.Append("LEFT JOIN ");
        sql.Append(" sys_client_dining din  ON abi.dining_id = din.dining_id and din.valid = 'Y' ");


        sql.Append("LEFT JOIN nursing_restraint res  on res.client_id = per.client_id and res.valid = 'Y'   ");
        sql.Append("and (res.restraint_state ='親屬口頭同意' or res.restraint_state = '有效' ) ");
        sql.Append(" left join nursing_restraint_item_record res_item on res_item.restraint_id = res.restraint_id and res_item.valid = 'Y' ");
        sql.Append(" left join sys_restraint_item sysrt on sysrt.restraint_item_id = res_item.restraint_item_id and sysrt.valid = 'Y' ");


        //    restraint_state
        //親屬口頭同意
        //有效
        //   SELECT* FROM azure.nursing_restraint;
        //    restraint_items


        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("where per.client_id = '{0}' ;  ");

        /*

        sql.Append("SELECT CONCAT(a.chi_name, a.chi_name_last),CONCAT(a.dob),a.sex,CONCAT(b.block, '-', b.zone, '-', b.bed_num),c.document_photo ,");
        sql.Append("GROUP_CONCAT(DISTINCT g.tchi_value SEPARATOR ';'),CONCAT_WS(' ', cast(m.diagnosis1 as char), CAST(m.diagnosis2 as char)),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name separator ';'),m.other_allergen,");
        sql.Append("GROUP_CONCAT(DISTINCT d.medicine_common_name separator ';'),CONCAT(a.eng_lastname,' ', a.eng_firstname),a.hkid,a.assessment_result  FROM client_personal2 AS a ");
        sql.Append("LEFT JOIN company_bed AS b ON a.client_id = b.client_id ");
        sql.Append("LEFT JOIN client_documents AS c ON a.client_id = c.client_id ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = a.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine d on m.drug_allergen REGEXP d.medicine_id where a.client_id = '{0}' GROUP BY a.client_id; ");
 


    */

        sql.Append("SELECT CONCAT(per.chi_surname,per.chi_name),CONCAT(per.dob),per.sex,concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value), ");
        sql.Append("concat( cast(m.sickness_brief as char) ),");
        sql.Append("GROUP_CONCAT(DISTINCT g.eng_value SEPARATOR ';'),CAST(m.diagnosis1 as char),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name_eng separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name_eng separator ';'),m.other_allergen,");
        sql.Append("m.allergen_remark,per.assessment_result ,CONCAT(per.eng_surname,' ',per.eng_name),per.hkid,per.client_number   FROM client_personal2 AS per ");

        sql.Append("LEFT JOIN client_documents2 AS c ON per.client_id = c.client_id and c.valid = 'Y' ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = per.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine2 h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine2 d on m.drug_allergen REGEXP d.medicine_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append(" where per.client_id = '{0}'  ");
        sql.Append(" ;");



        string comm = string.Format(sql.ToString(), id);
        return comm;
    }

    //For v153 update
    public static string get_client_briefing2(string id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT CONCAT(per.chi_surname, per.chi_name),sex,DATE_FORMAT(dob, '%Y/%m/%d'),");
        sql.Append("YEAR(CURRENT_TIMESTAMP) - YEAR(per.dob) - (RIGHT(CURRENT_TIMESTAMP, 5) < RIGHT(per.dob, 5)) as age,  ");
        sql.Append("concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),per.assessment_result , din.dining_name , ");
        sql.Append(" group_concat(DISTINCT dt.type_name,' ', diet.tchi_value separator '\n') ,");
        sql.Append(" ifnull(tool.tchi_value,' '),");

        sql.Append("ifnull(abi.self_care_ability,' ')   ,ifnull(abi.eating_ability,' '),");
        sql.Append("ifnull(abi.cognitive_ability,' '),ifnull(abi.communicate_ability,' '), ifnull(abi.visual_ability,' '),ifnull(abi.audio_ability,' '), ifnull(abi.incontinence,' '),");
        sql.Append("group_concat(DISTINCT sysdia.tchi_value separator '\n'),     ");
        // client_ability_id, client_id, self_care_ability, movement_ability, eating_ability,
        // cognitive_ability, audiovisual_ability, communicate_ability, fall_risk, incontinence, ability_remark,
        // created_by, created_datetime, valid, assisting_tools, help_lifting, audio_ability, visual_ability, oxygen, teeth, dining_id


        sql.Append("ae.ae_status,");
        sql.Append("abi.teeth,");
        // sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");



        sql.Append("doc.client_photo_id,lift.tchi_value,  ");
        sql.Append("group_concat(DISTINCT eve.tchi_value,'', memo.remark separator '\n'),    ");
        sql.Append("group_concat(DISTINCT  sysrt.tchi_value, '' separator ';'),   ");
        //sql.Append("group_concat(DISTINCT  sysrt.tchi_value , '',res.restraint_items,'', res.other_restraint_items,' ',res.restraint_items_remarks separator ' '),  ");
        sql.Append("GROUP_CONCAT(DISTINCT pos.position_chi_name separator ','),  ");
        sql.Append("per.contact_precaution,abi.oxygen   ");

        sql.Append(" FROM client_personal2 per ");
        //  sql.Append(" client_personal Where client_id=@id");
        //   SELECT* FROM greenery.medical_ae; ae_id, client_id, ae_status, ae_in_reason, ae_in_datetime, ae_in_remark, hospitalized, 
        //   ae_in_bed_num, ae_out_reason, ae_addr_id, ae_out_datetime, ae_out_remark, created_by, created_datetime, modified_by, modified_datetime, valid


        //     IF(a.addressid IS NULL, 0, 1)

        // SELECT* FROM greenery.client_diet; client_diet_id, client_id,
        //  prefered_rice, prefered_groceries, prefered_thickener, diet_preference, created_by, created_datetime, valid

        sql.Append("left join client_ability abi on abi.client_id = per.client_id and abi.valid='Y' ");
        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
        sql.Append("left join care_wound2 wou on wou.client_id = per.client_id and recovery ='N' and wou.valid='Y'  ");
        sql.Append("left join medical_ae ae on ae.client_id = per.client_id and ae_status = '留醫中' and   ae.valid  = 'Y'  ");
        // sql.Append("left join client_diet diet on diet.client_id = per.client_id and  diet.valid  = 'Y'  ");
        sql.Append(" LEFT JOIN care_wound_position pos ON wou.wound_position_id = pos.wound_position_id   ");

        sql.Append(" LEFT JOIN client_diet_preference AS pre ON per.client_id = pre.client_id and pre.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_preference AS diet ON diet.diet_id = pre.diet_id and diet.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_type AS dt ON dt.diet_type_id = diet.diet_type_id   ");


        sql.Append("left join client_responsible_record respon on per.client_id = respon.client_id and respon.valid ='Y'  ");
        sql.Append("left join sys_client_responsible sysres on sysres.responsible_id = respon.responsible_id and sysres.valid ='Y'   ");

        sql.Append(" LEFT JOIN sys_client_help_lifting AS lift ON abi.help_lifting = lift.lifting_id and lift.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_assisting_tools AS tool ON abi.assisting_tools = tool.tools_id and tool.valid = 'Y'  ");

        sql.Append(" LEFT JOIN client_memo_event AS memo ON memo.client_id = per.client_id and memo.valid = 'Y'  ");
        sql.Append(" and ( date( memo.end_datetime)>= curdate() or date( memo.end_datetime) = date('1900-05-16')) ");

        sql.Append("LEFT JOIN sys_client_memo_event eve  on eve.memo_event_id = memo.memo_event_id ");

        sql.Append("LEFT JOIN client_diaper_event dia  on dia.client_id = per.client_id and dia.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diaper AS sysdia ON sysdia.diaper_id = dia.diaper_id and sysdia.valid = 'Y' ");


        sql.Append("LEFT JOIN ");
        sql.Append(" sys_client_dining din  ON abi.dining_id = din.dining_id and din.valid = 'Y' ");

        /*
        sql.Append("LEFT JOIN nursing_restraint res  on res.client_id = per.client_id and res.valid = 'Y'   ");
        sql.Append("and (res.restraint_state ='親屬口頭同意' or res.restraint_state = '有效' ) ");
        sql.Append(" left join nursing_restraint_item_record res_item on res_item.restraint_id = res.restraint_id and res_item.valid = 'Y' ");
        */
        sql.Append(" LEFT JOIN nursing_restraint_tmp res ON res.client_id = per.client_id  AND res.valid = 'Y'  ");
        sql.Append("  AND res.restraint_state in ('100003', '100004', '100002') ");
        sql.Append("  AND date(now()) between date(res.begin_date) and date(res.end_date) ");
        sql.Append("  LEFT JOIN nursing_restraint_item res_item ON res_item.restraint_id = res.restraint_id  AND res_item.valid = 'Y' ");
        sql.Append(" left join sys_restraint_item sysrt on sysrt.restraint_item_id = res_item.def_restraint_item_id and sysrt.valid = 'Y' ");

        //    restraint_state
        //親屬口頭同意
        //有效
        //   SELECT* FROM azure.nursing_restraint;
        //    restraint_items


        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("where per.client_id = '{0}' ;  ");

        /*

        sql.Append("SELECT CONCAT(a.chi_name, a.chi_name_last),CONCAT(a.dob),a.sex,CONCAT(b.block, '-', b.zone, '-', b.bed_num),c.document_photo ,");
        sql.Append("GROUP_CONCAT(DISTINCT g.tchi_value SEPARATOR ';'),CONCAT_WS(' ', cast(m.diagnosis1 as char), CAST(m.diagnosis2 as char)),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name separator ';'),m.other_allergen,");
        sql.Append("GROUP_CONCAT(DISTINCT d.medicine_common_name separator ';'),CONCAT(a.eng_lastname,' ', a.eng_firstname),a.hkid,a.assessment_result  FROM client_personal2 AS a ");
        sql.Append("LEFT JOIN company_bed AS b ON a.client_id = b.client_id ");
        sql.Append("LEFT JOIN client_documents AS c ON a.client_id = c.client_id ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = a.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine d on m.drug_allergen REGEXP d.medicine_id where a.client_id = '{0}' GROUP BY a.client_id; ");
 


    */

        // medical history updated
        sql.Append("SELECT CONCAT(per.chi_surname,per.chi_name),CONCAT(per.dob),per.sex,concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value), ");
        sql.Append("GROUP_CONCAT(DISTINCT sms.tchi_value ORDER BY sms.medical_sickness_id SEPARATOR ';') as sickness_brief,");
        sql.Append("GROUP_CONCAT(DISTINCT g.tchi_value SEPARATOR ';') as icd9, mhd.diagnosis as diagnosis ,");
        //sql.Append("GROUP_CONCAT(DISTINCT d.medicine_plain_name_eng separator ';') as drug_allergen, GROUP_CONCAT(DISTINCT h.medicine_plain_name_eng separator ';') as drug_adverse , ");
        sql.Append(" '' as drug_allergen , '' as drug_adverse, ");
        sql.Append("GROUP_CONCAT(DISTINCT sfa.tchi_value SEPARATOR ';') as food_allergen, GROUP_CONCAT(DISTINCT soa.tchi_value SEPARATOR ';') as other_allergen, ");
        sql.Append("a.allergen_remark,per.assessment_result ,CONCAT(per.eng_surname,' ',per.eng_name),per.hkid,per.client_number ,per.client_id  FROM client_personal2 AS per ");

        //sql.Append("LEFT JOIN client_documents2 AS c ON per.client_id = c.client_id and c.valid = 'Y' ");
        sql.Append(" LEFT JOIN medical_history2 AS a ON a.client_id = per.client_id and a.valid = 'Y' ");
        sql.Append(" LEFT JOIN medical_history_detail AS mhd ON mhd.client_id = per.client_id and mhd.valid = 'Y' ");
        sql.Append(" and mhd.record_datetime = (SELECT max(record_datetime) from medical_history_detail mhd2 where mhd.client_id = mhd2.client_id) ");
        //sql.Append(" LEFT JOIN medical_history_detail AS mhd2 ON mhd.client_id = mhd2.client_id and mhd.record_datetime < mhd2.record_datetime AND mhd2.valid = 'Y' ");
        sql.Append("LEFT JOIN sys_medical_sickness sms on FIND_IN_SET( sms.medical_sickness_id, REPLACE(a.sickness_brief,';',',')) AND sms.valid = 'Y'");
        sql.Append("LEFT JOIN sys_medical_allergen_food sfa ON find_in_set(sfa.allergen_id, replace(a.food_allergen,';',',')) ");
        sql.Append("LEFT JOIN sys_medical_allergen_other soa ON find_in_set(soa.allergen_id, replace(a.other_allergen,';',',')) ");
        sql.Append("LEFT JOIN medical_icd9 g ON FIND_IN_SET(g.uid, REPLACE(a.icd_9, ';', ',')) ");
        //sql.Append("LEFT JOIN medical_icd9 g ON a.icd_9 REGEXP g.uid ");
        //sql.Append("LEFT JOIN med_medicine2 h ON a.drug_adverse REGEXP h.medicine_id ");
        //sql.Append("LEFT JOIN med_medicine2 d on a.drug_allergen REGEXP d.medicine_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append(" where per.client_id = '{0}' ");
        sql.Append("GROUP BY per.client_id; ");

        sql.Append(" SELECT mh.client_id , ");
        sql.Append(" GROUP_CONCAT(DISTINCT h.medicine_plain_name_eng SEPARATOR ';') AS drug_adverse , ");
        sql.Append(" GROUP_CONCAT(DISTINCT d.medicine_plain_name_eng SEPARATOR ';') AS drug_allergen ");
        sql.Append(" from medical_history2 mh ");
        sql.Append(" LEFT JOIN med_medicine2 h ON FIND_IN_SET(h.medicine_id, REPLACE(mh.drug_adverse, ';', ',')) AND h.valid = 'Y' ");
        sql.Append(" LEFT JOIN med_medicine2 d ON FIND_IN_SET(d.medicine_id, REPLACE(mh.drug_allergen, ';', ',')) AND d.valid = 'Y' ");
        sql.Append(" WHERE mh.client_id != '' AND mh.client_id IN ({0}) ");
        sql.Append(" GROUP BY mh.client_id ; ");

        /*
        sql.Append("SELECT CONCAT(per.chi_surname,per.chi_name),CONCAT(per.dob),per.sex,concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value), ");
        sql.Append("concat( cast(m.sickness_brief as char) ),");
        sql.Append("GROUP_CONCAT(DISTINCT g.eng_value SEPARATOR ';'),CAST(m.diagnosis1 as char),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name_eng separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name_eng separator ';'),m.other_allergen,");
        sql.Append("m.allergen_remark,per.assessment_result ,CONCAT(per.eng_surname,' ',per.eng_name),per.hkid,per.client_number   FROM client_personal2 AS per ");

        sql.Append("LEFT JOIN client_documents2 AS c ON per.client_id = c.client_id and c.valid = 'Y' ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = per.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine2 h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine2 d on m.drug_allergen REGEXP d.medicine_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append(" where per.client_id = '{0}'  ");
        sql.Append(" ;");
        */


        string comm = string.Format(sql.ToString(), id);
        return comm;
    }
    public static string get_client_briefing3(string id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT CONCAT(per.chi_surname, per.chi_name),sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");
        sql.Append("YEAR(CURRENT_TIMESTAMP) - YEAR(per.dob) - (RIGHT(CURRENT_TIMESTAMP, 5) < RIGHT(per.dob, 5)) as age,  ");

        sql.Append("CONCAT_WS(';', ifnull(abi.self_care_ability,' ') ,ifnull(tool.tchi_value,' '),ifnull(abi.eating_ability,' '),");
        sql.Append("ifnull(abi.cognitive_ability,' '), ifnull(abi.audiovisual_ability,' '),ifnull(abi.communicate_ability,' '),ifnull(abi.incontinence,' '), ability_remark), ");

        sql.Append(" GROUP_CONCAT(DISTINCT pos.position_chi_name separator ','),  ");
        sql.Append("ae.ae_status,");
        // sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");

        sql.Append(" group_concat(DISTINCT dt.type_name,' ', diet.tchi_value separator '\n') ,");

        sql.Append("doc.client_photo_id,per.contact_precaution,lift.tchi_value,tool.tchi_value ,sysres.tchi_value,");
        sql.Append("group_concat(DISTINCT eve.tchi_value,'', memo.remark separator ' '),sysdia.tchi_value,    ");
        //sql.Append("group_concat(DISTINCT  concat(sysrt.tchi_value,res_item.restraint_items_remark,' ',res_item.client_state), '' separator ';'),   ");
        sql.Append("GROUP_CONCAT(DISTINCT CONCAT(resJoin.restraint_name,resJoin.item_remark,' ',resJoin.restraint_session), ' ' SEPARATOR ';'),");
        sql.Append(" if(per.contact_precaution='Y','CT',''),if(abi.oxygen='Y','OO',''),  (SELECT icon_code FROM sys_display_panel_bed_icon WHERE tchi_value=abi.teeth and type_id = 1) AS teeth ,per.assessment_result  , din.dining_name  ");
        sql.Append(" FROM client_personal2 per ");
        //  sql.Append(" client_personal Where client_id=@id");
        //   SELECT* FROM greenery.medical_ae; ae_id, client_id, ae_status, ae_in_reason, ae_in_datetime, ae_in_remark, hospitalized, 
        //   ae_in_bed_num, ae_out_reason, ae_addr_id, ae_out_datetime, ae_out_remark, created_by, created_datetime, modified_by, modified_datetime, valid


        //     IF(a.addressid IS NULL, 0, 1)

        // SELECT* FROM greenery.client_diet; client_diet_id, client_id,
        //  prefered_rice, prefered_groceries, prefered_thickener, diet_preference, created_by, created_datetime, valid

        sql.Append("left join client_ability abi on abi.client_id = per.client_id and abi.valid='Y' ");
        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
        sql.Append("left join care_wound2 wou on wou.client_id = per.client_id and recovery ='N'   ");
        sql.Append("left join medical_ae ae on ae.client_id = per.client_id and ae_status = '留醫中' and   ae.valid  = 'Y'  ");
        sql.Append(" LEFT JOIN care_wound_position pos ON wou.wound_position_id = pos.wound_position_id    ");


        sql.Append(" LEFT JOIN client_diet_preference AS pre ON per.client_id = pre.client_id and pre.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_preference AS diet ON diet.diet_id = pre.diet_id and diet.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_type AS dt ON dt.diet_type_id = diet.diet_type_id   ");


        sql.Append("left join client_responsible_record respon on per.client_id = respon.client_id and respon.valid ='Y'  ");
        sql.Append("left join sys_client_responsible sysres on sysres.responsible_id = respon.responsible_id and sysres.valid ='Y'   ");

        sql.Append(" LEFT JOIN sys_client_help_lifting AS lift ON abi.help_lifting = lift.lifting_id and lift.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_assisting_tools AS tool ON abi.assisting_tools = tool.tools_id and tool.valid = 'Y'  ");

        sql.Append(" LEFT JOIN client_memo_event AS memo ON memo.client_id = per.client_id and memo.valid = 'Y'  ");
        sql.Append(" and ( date( memo.end_datetime)>= curdate() or date( memo.end_datetime) = date('1900-05-16')) ");

        sql.Append("LEFT JOIN sys_client_memo_event eve  on eve.memo_event_id = memo.memo_event_id ");

        sql.Append("LEFT JOIN client_diaper_event dia  on dia.client_id = per.client_id and dia.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diaper AS sysdia ON sysdia.diaper_id = dia.diaper_id and sysdia.valid = 'Y' ");


        sql.Append("LEFT JOIN ");
        sql.Append(" sys_client_dining din  ON abi.dining_id = din.dining_id and din.valid = 'Y' ");

        /*
        sql.Append("LEFT JOIN nursing_restraint res  on res.client_id = per.client_id and res.valid = 'Y'   ");
        sql.Append("and (res.restraint_state ='親屬口頭同意' or res.restraint_state = '有效' ) ");
        sql.Append(" left join nursing_restraint_item_record res_item on res_item.restraint_id = res.restraint_id and res_item.valid = 'Y' ");
        sql.Append(" left join sys_restraint_item sysrt on sysrt.restraint_item_id = res_item.restraint_item_id and sysrt.valid = 'Y' ");
        */
        sql.Append("LEFT JOIN( ");
        sql.Append("         select per.client_id, ");
        sql.Append(" sysrt.tchi_value as restraint_name, ");
        sql.Append(" res_item.item_remark, ");
        sql.Append(" GROUP_CONCAT(DISTINCT CONCAT(sysrs.session_name)  order by sysrs.ordering SEPARATOR ' ') as restraint_session ");
        sql.Append(" from client_personal2 per ");
        sql.Append("  LEFT JOIN nursing_restraint_tmp res ON res.client_id = per.client_id ");
        sql.Append(" AND res.valid = 'Y' AND res.restraint_state in ('100003', '100004', '100002') ");
        sql.Append(" AND date(now()) between date(res.begin_date) and date(res.end_date) ");
        sql.Append(" LEFT JOIN nursing_restraint_item res_item ON res_item.restraint_id = res.restraint_id AND res_item.valid = 'Y' ");
        sql.Append("  LEFT JOIN sys_restraint_item sysrt ON sysrt.restraint_item_id = res_item.def_restraint_item_id  AND sysrt.valid = 'Y' ");
        sql.Append("  LEFT JOIN nursing_restraint_item_session_tmp item_session ON item_session.item_id = res_item.item_id and item_session.valid = 'Y' ");
        sql.Append("  LEFT JOIN sys_restraint_session sysrs on sysrs.session_id = item_session.def_session_id and sysrs.valid = 'Y' ");
        sql.Append(" WHERE res.restraint_id IS NOT NULL group by per.client_id, res.restraint_id, res_item.def_restraint_item_id ) as resJoin on resJoin.client_id = per.client_id ");


        //    restraint_state
        //親屬口頭同意
        //有效
        //   SELECT* FROM azure.nursing_restraint;
        //    restraint_items


        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("where per.client_id = '{0}' ;  ");

        /*

        sql.Append("SELECT CONCAT(a.chi_name, a.chi_name_last),CONCAT(a.dob),a.sex,CONCAT(b.block, '-', b.zone, '-', b.bed_num),c.document_photo ,");
        sql.Append("GROUP_CONCAT(DISTINCT g.tchi_value SEPARATOR ';'),CONCAT_WS(' ', cast(m.diagnosis1 as char), CAST(m.diagnosis2 as char)),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name separator ';'),m.other_allergen,");
        sql.Append("GROUP_CONCAT(DISTINCT d.medicine_common_name separator ';'),CONCAT(a.eng_lastname,' ', a.eng_firstname),a.hkid,a.assessment_result  FROM client_personal2 AS a ");
        sql.Append("LEFT JOIN company_bed AS b ON a.client_id = b.client_id ");
        sql.Append("LEFT JOIN client_documents AS c ON a.client_id = c.client_id ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = a.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine d on m.drug_allergen REGEXP d.medicine_id where a.client_id = '{0}' GROUP BY a.client_id; ");
 


    */

        // medical history updated
        sql.Append("SELECT CONCAT(per.chi_surname,per.chi_name),CONCAT(per.dob),per.sex,concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value), ");
        sql.Append("GROUP_CONCAT(DISTINCT sms.tchi_value ORDER BY sms.medical_sickness_id SEPARATOR ';') as sickness_brief,");
        sql.Append("GROUP_CONCAT(DISTINCT g.tchi_value SEPARATOR ';') as icd9, mhd.diagnosis as diagnosis ,");
        //sql.Append("GROUP_CONCAT(DISTINCT d.medicine_plain_name_eng separator ';') as drug_allergen, GROUP_CONCAT(DISTINCT h.medicine_plain_name_eng separator ';') as drug_adverse , ");
        sql.Append(" '' as drug_allergen , '' as drug_adverse, ");
        sql.Append("GROUP_CONCAT(DISTINCT sfa.tchi_value SEPARATOR ';') as food_allergen, GROUP_CONCAT(DISTINCT soa.tchi_value SEPARATOR ';') as other_allergen, ");
        sql.Append("a.allergen_remark,per.assessment_result ,CONCAT(per.eng_surname,' ',per.eng_name),per.hkid,per.client_number ,per.client_id  FROM client_personal2 AS per ");

        //sql.Append("LEFT JOIN client_documents2 AS c ON per.client_id = c.client_id and c.valid = 'Y' ");
        sql.Append(" LEFT JOIN medical_history2 AS a ON a.client_id = per.client_id and a.valid = 'Y' ");
        sql.Append(" LEFT JOIN medical_history_detail AS mhd ON mhd.client_id = per.client_id and mhd.valid = 'Y' ");
        sql.Append(" and mhd.record_datetime = (SELECT max(record_datetime) from medical_history_detail mhd2 where mhd.client_id = mhd2.client_id) ");
        //sql.Append(" LEFT JOIN medical_history_detail AS mhd2 ON mhd.client_id = mhd2.client_id and mhd.record_datetime < mhd2.record_datetime AND mhd2.valid = 'Y' ");
        sql.Append("LEFT JOIN sys_medical_sickness sms on FIND_IN_SET( sms.medical_sickness_id, REPLACE(a.sickness_brief,';',',')) AND sms.valid = 'Y'");
        sql.Append("LEFT JOIN sys_medical_allergen_food sfa ON find_in_set(sfa.allergen_id, replace(a.food_allergen,';',',')) ");
        sql.Append("LEFT JOIN sys_medical_allergen_other soa ON find_in_set(soa.allergen_id, replace(a.other_allergen,';',',')) ");
        sql.Append("LEFT JOIN medical_icd9 g ON FIND_IN_SET(g.uid, REPLACE(a.icd_9, ';', ',')) ");
        //sql.Append("LEFT JOIN medical_icd9 g ON a.icd_9 REGEXP g.uid ");
        //sql.Append("LEFT JOIN med_medicine2 h ON a.drug_adverse REGEXP h.medicine_id ");
        //sql.Append("LEFT JOIN med_medicine2 d on a.drug_allergen REGEXP d.medicine_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append(" where per.client_id = '{0}' ");
        sql.Append("GROUP BY per.client_id; ");

        sql.Append(" SELECT mh.client_id , ");
        sql.Append(" GROUP_CONCAT(DISTINCT h.medicine_plain_name_eng SEPARATOR ';') AS drug_adverse , ");
        sql.Append(" GROUP_CONCAT(DISTINCT d.medicine_plain_name_eng SEPARATOR ';') AS drug_allergen ");
        sql.Append(" from medical_history2 mh ");
        sql.Append(" LEFT JOIN med_medicine2 h ON FIND_IN_SET(h.medicine_id, REPLACE(mh.drug_adverse, ';', ',')) AND h.valid = 'Y' ");
        sql.Append(" LEFT JOIN med_medicine2 d ON FIND_IN_SET(d.medicine_id, REPLACE(mh.drug_allergen, ';', ',')) AND d.valid = 'Y' ");
        sql.Append(" WHERE mh.client_id != '' AND mh.client_id IN ({0}) ");
        sql.Append(" GROUP BY mh.client_id ; ");

        string comm = string.Format(sql.ToString(), id);
        return comm;
    }

    public static string get_current_residential_analysis()
    {
        StringBuilder sql = new StringBuilder();
        string first_table = "company_bed_charge2  cha " +
            "left join company_bed_history as his on cha.room_charge_id = his.bed_type and DATE(his.leave_date) = DATE('1900-05-16') and date(his.entry_date) <= curdate() ";
        string first_select = "his.bed_type = cha.room_charge_id";
        string first_condition = "group by room_charge_id,his.bed_type ";

        string second_table = "company_bed_history his " + "left join company_bed_charge2 as cha on cha.room_charge_id = his.bed_type ";
        string second_select = "his.bed_type = 0";
        string second_condition = "where bed_type = '0'  and DATE(his.leave_date) = DATE('1900-05-16') and date(his.entry_date) <= curdate()  " +
            "group by his.bed_type ";
        sql = get_current_residential_analysis_prefix(sql, first_select, first_table, first_condition);

        sql.Append("union ");
        sql = get_current_residential_analysis_prefix(sql, second_select, second_table, second_condition);

        string comm = string.Format(sql.ToString() );
        return comm;
    }
    public static StringBuilder get_current_residential_analysis_prefix(StringBuilder sql, string select, string table, string condition)
    {

        sql.Append("SELECT ");
        sql.Append("ifnull(room_charge_id, 0),ifnull(cha.charge_chi_type,'其他') ,  ");
        sql.Append("IFNULL(SUM({0}), 0),  IFNULL(SUM(per.sex = 'M'), 0), ");
        sql.Append("IFNULL(SUM(per.sex = 'F'), 0),IFNULL(SUM(ae.ae_status = '留醫中'), 0), ");
        sql.Append("IFNULL(SUM(his.assessment_result = '中度機能受損'), 0), ");
        sql.Append("IFNULL(SUM(his.assessment_result = '高度機能受損'), 0), ");
        sql.Append("IFNULL(SUM(his.assessment_result = ''), 0) ");
        sql.Append("FROM  {1} ");


        sql.Append("LEFT JOIN ");
        sql.Append("client_personal2 AS per ON his.client_id = per.client_id and per.active_status = 'Y' ");
        sql.Append("left join medical_ae as ae on per.client_id = ae.client_id and ae.ae_status = '留醫中' and ae.valid = 'Y' ");
        sql.Append("{2} ");
        return new StringBuilder(string.Format(sql.ToString(), select, table, condition));
    }

    public static string get_client_diet(string id,string condition = "")
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT CONCAT(per.chi_surname, per.chi_name),sex,dt.type_name, ");


        sql.Append("group_concat(DISTINCT  diet.tchi_value separator ',') ");
 
       // sql.Append(" group_concat(DISTINCT dt.type_name,' ', diet.tchi_value separator '\n')  ");
 
        sql.Append(" FROM client_personal2 per ");
 
        sql.Append(" LEFT JOIN client_diet_preference AS pre ON per.client_id = pre.client_id and pre.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_preference AS diet ON diet.diet_id = pre.diet_id and diet.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_type AS dt ON dt.diet_type_id = diet.diet_type_id   ");

 
        sql.Append("where per.client_id = '{0}'   ");
        if (condition.Length>0)
        {
            sql.Append(condition);
        }
        sql.Append(" ; ");
        /*

        sql.Append("SELECT CONCAT(a.chi_name, a.chi_name_last),CONCAT(a.dob),a.sex,CONCAT(b.block, '-', b.zone, '-', b.bed_num),c.document_photo ,");
        sql.Append("GROUP_CONCAT(DISTINCT g.tchi_value SEPARATOR ';'),CONCAT_WS(' ', cast(m.diagnosis1 as char), CAST(m.diagnosis2 as char)),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name separator ';'),m.other_allergen,");
        sql.Append("GROUP_CONCAT(DISTINCT d.medicine_common_name separator ';'),CONCAT(a.eng_lastname,' ', a.eng_firstname),a.hkid,a.assessment_result  FROM client_personal2 AS a ");
        sql.Append("LEFT JOIN company_bed AS b ON a.client_id = b.client_id ");
        sql.Append("LEFT JOIN client_documents AS c ON a.client_id = c.client_id ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = a.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine d on m.drug_allergen REGEXP d.medicine_id where a.client_id = '{0}' GROUP BY a.client_id; ");
 


    */
 

        string comm = string.Format(sql.ToString(), id);
        return comm;
    }
    public static string get_client_contact(string id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報

        sql.Append("select relative_name,relative_type,if(relative_phone1='',relative_phone2,relative_phone1) from client_relatives ");
        sql.Append(" Where client_id IN ('{0}') and  relative_is_emergency = 'Y' and valid='Y' order by relative_ordering  ");
        string comm = string.Format(sql.ToString(), id);
        return comm;
    }
    public static string GetClientRoom(string FindVal)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append("select block_id, tchi_value from sys_company_block ");
        sql.Append("where valid = 'Y' order by CONVERT(tchi_value, UNSIGNED) asc , tchi_value asc;");

        sql.Append("select concat(k.tchi_value,'/',z.tchi_value), z.zone_id, z.block_id, z.tchi_value, z.eng_value ");
        sql.Append(" from sys_company_zone z ");
        sql.Append("left join sys_company_block k on z.block_id = k.block_id ");
        sql.Append("where z.valid = 'Y' and k.valid = 'Y'   ");
        sql.Append("order by CONVERT(k.tchi_value, UNSIGNED) asc ,k.tchi_value asc, ");
        sql.Append(" CONVERT(z.tchi_value, UNSIGNED) asc ,z.tchi_value asc; ");


        return (sql.ToString());
    }
    public static string get_full_client(string condition)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報

     //   int search_int = int.Parse(search_index);

        sql.Append("SELECT per.client_id, CONCAT(per.chi_surname, per.chi_name),eng_name,eng_surname,sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),k.tchi_value,z.tchi_value,  ");
        sql.Append("ifnull(doc.client_photo_id,0), ");

        sql.Append("ae.ae_status ");


        sql.Append(" from client_personal2 per ");

        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");

        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("Left join medical_ae as ae on per.client_id = ae.client_id and ae.valid = 'Y' and ae.ae_status = '留醫中'   ");


        // client_id, client_number, client_card_id, chi_surname, chi_name, eng_surname, eng_name,
        //  sex, hkid, passport, dob, dob_month, nation, spoken_language, marital_status, child_boy_num, child_girl_num, education_level, LDS_num, religion, belongings, belongings2, property, entry_date, assessment_result, assessment_datetime, buy_type, remark, entry_status, living_status, agree_free_out, relationship_status,
        //  social_activities, active_status, created_by, created_datetime, modified_by, modified_datetime

        sql.Append("where per.client_id !='' ");
        if (condition.Length>0)
        {
            sql.Append(condition);
        }
 

        sql.Append("order by CONVERT(k.tchi_value, UNSIGNED) asc ,k.tchi_value asc, CONVERT(z.tchi_value, UNSIGNED) asc, z.tchi_value asc, CONVERT(b.tchi_value, UNSIGNED) asc, b.tchi_value asc");
        return sql.ToString();
    }
    /* // Raymond 20200824
    public static string  get_search_client(string keyword, string search_index)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報

        int search_int = int.Parse(search_index);

        sql.Append("SELECT per.client_id, CONCAT(per.chi_surname, per.chi_name),eng_name,eng_surname,sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");
        sql.Append("ifnull(doc.client_photo_id,0), ");

        sql.Append("ae.ae_status ");


        sql.Append(" from client_personal2 per ");

        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");

        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("Left join medical_ae as ae on per.client_id = ae.client_id and ae.valid = 'Y' and ae.ae_status = '留醫中'   ");
 

        // client_id, client_number, client_card_id, chi_surname, chi_name, eng_surname, eng_name,
        //  sex, hkid, passport, dob, dob_month, nation, spoken_language, marital_status, child_boy_num, child_girl_num, education_level, LDS_num, religion, belongings, belongings2, property, entry_date, assessment_result, assessment_datetime, buy_type, remark, entry_status, living_status, agree_free_out, relationship_status,
        //  social_activities, active_status, created_by, created_datetime, modified_by, modified_datetime
        if (search_int == 0)
        {
            //search_field = "chi_name";

            //cmdText0 = "select * from client_personal Where  ( chi_name LIKE '%" + keyword + "%' or  chi_name_last LIKE '%" + keyword + "%' or concat(chi_name,chi_name_last) LIKE '%" + keyword + "%' ) and ( active_status = 'Y' )";
            sql.Append("where  ( per.chi_surname LIKE '%" + keyword + "%' or  per.chi_name LIKE '%" + keyword + "%' or concat(per.chi_surname, per.chi_name) LIKE '%" + keyword + "%' ) and ( per.active_status = 'Y' ) ");

        }

        else if (search_int == 1)
        {
         //   cmdText0 = "select * from client_personal Where  ( eng_firstname LIKE '%" + keyword + "%' or eng_lastname LIKE '%" + keyword + "%' ) and ( active_status = 'Y' )";
            sql.Append(" Where  (per.eng_name LIKE '%" + keyword + "%' or per.eng_surname LIKE '%" + keyword + "%' ) and ( per.active_status = 'Y' )");

        }
        else if (search_int == 2)
        {
            sql.Append(" Where (concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' ) and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 3)
        {
            sql.Append(" Where ae.ae_status = '留醫中'   and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if(search_int == 4)
        {
            sql.Append(" Where month(per.dob) = month(curdate())  and ( per.active_status = 'Y' ) ");
        }
        else if (search_int == 5)
        {
            sql.Append(" Where (concat(k.tchi_value) = '" + keyword + "' ) and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 6)
        {
            sql.Append(" Where (concat(k.tchi_value,'/',z.tchi_value) = '" + keyword + "' ) and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 7)
        {
            sql.Append(" Where (concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) = '" + keyword + "' ) and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }

        sql.Append("order by CONVERT(k.tchi_value, UNSIGNED) asc ,k.tchi_value asc, CONVERT(z.tchi_value, UNSIGNED) asc, z.tchi_value asc, CONVERT(b.tchi_value, UNSIGNED) asc, b.tchi_value asc");
        return sql.ToString();
    }
    */
    // Raymond 20200909
    /*
    public static string get_search_client(string keyword, string search_index)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報

        int search_int = int.Parse(search_index);

        sql.Append("SELECT distinct per.client_id, CONCAT(per.chi_surname, per.chi_name),eng_name,eng_surname,sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");
        sql.Append("ifnull(doc.client_photo_id,0), ");

        sql.Append("ae.ae_status, ");
        sql.Append(" case when wound.client_id is not null then 'Y' else 'N' end as hasWound ");

        sql.Append(" from client_personal2 per ");

        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");

        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("Left join medical_ae as ae on per.client_id = ae.client_id and ae.valid = 'Y' and ae.ae_status = '留醫中'   ");
        sql.Append(" LEFT JOIN  care_wound2 wound ON wound.client_id = per.client_id and now() >= wound.start_datetime and wound.end_datetime = '1900-05-16' and wound.recovery = 'N'and wound.valid = 'Y' ");

        // client_id, client_number, client_card_id, chi_surname, chi_name, eng_surname, eng_name,
        //  sex, hkid, passport, dob, dob_month, nation, spoken_language, marital_status, child_boy_num, child_girl_num, education_level, LDS_num, religion, belongings, belongings2, property, entry_date, assessment_result, assessment_datetime, buy_type, remark, entry_status, living_status, agree_free_out, relationship_status,
        //  social_activities, active_status, created_by, created_datetime, modified_by, modified_datetime
        if (search_int == 0)
        {
            //search_field = "chi_name";

            //cmdText0 = "select * from client_personal Where  ( chi_name LIKE '%" + keyword + "%' or  chi_name_last LIKE '%" + keyword + "%' or concat(chi_name,chi_name_last) LIKE '%" + keyword + "%' ) and ( active_status = 'Y' )";
            sql.Append("where  ( per.chi_surname LIKE '%" + keyword + "%' or  per.chi_name LIKE '%" + keyword + "%' or concat(per.chi_surname, per.chi_name) LIKE '%" + keyword + "%' ) and ( per.active_status = 'Y' ) ");

        }

        else if (search_int == 1)
        {
            //   cmdText0 = "select * from client_personal Where  ( eng_firstname LIKE '%" + keyword + "%' or eng_lastname LIKE '%" + keyword + "%' ) and ( active_status = 'Y' )";
            sql.Append(" Where  (per.eng_name LIKE '%" + keyword + "%' or per.eng_surname LIKE '%" + keyword + "%' ) and ( per.active_status = 'Y' )");

        }
        else if (search_int == 2)
        {
            sql.Append(" Where (concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' ) and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 3)
        {
            sql.Append(" Where ae.ae_status = '留醫中'   and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 4)
        {
            sql.Append(" Where month(per.dob) = month(curdate())  and ( per.active_status = 'Y' ) ");
        }
        else if (search_int == 5)
        {
            sql.Append(" Where (concat(k.tchi_value) = '" + keyword + "' ) and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 6)
        {
            sql.Append(" Where (concat(k.tchi_value,'/',z.tchi_value) = '" + keyword + "' ) and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 7)
        {
            sql.Append(" Where (concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) = '" + keyword + "' ) and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 8)
        {
            sql.Append(" Where wound.client_id is not null   and ( per.active_status = 'Y' ) ");

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }

        sql.Append("order by CONVERT(k.tchi_value, UNSIGNED) asc ,k.tchi_value asc, CONVERT(z.tchi_value, UNSIGNED) asc, z.tchi_value asc, CONVERT(b.tchi_value, UNSIGNED) asc, b.tchi_value asc");
        return sql.ToString();
    }
    */
    // Raymond 20200909 copy from alpine
    public static string get_client_str(string condition)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT distinct per.client_id, CONCAT(per.chi_surname, per.chi_name) as chiname ,eng_name,eng_surname,sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d') as dob,concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) as bed_name, k.tchi_value as blk_name,z.tchi_value as zone_name,  ");
        sql.Append("ifnull(doc.client_photo_id,0) as client_photo_id, ");

        sql.Append("ae.ae_status, ");
        sql.Append(" case when wound.client_id is not null then 'Y' else 'N' end as hasWound, ");
        //sql.Append(" case when cleave.client_id is not null then 'Y' else 'N' end as hasHomeLeave ");
        sql.Append("'N' as hasHomeLeave ");

        sql.Append(" from client_personal2 per ");

        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");

        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("Left join medical_ae as ae on per.client_id = ae.client_id and ae.valid = 'Y' and ae.ae_status = '留醫中'   ");
        //sql.Append(" Left join client_leave cleave on cleave.client_id = per.client_id and cleave.leave_state = 1 and now()>=cleave.leave_out_datetime and cleave.valid = 'Y' ");
        sql.Append(" LEFT JOIN  care_wound2 wound ON wound.client_id = per.client_id and now() >= wound.start_datetime and wound.end_datetime = '1900-05-16' and wound.recovery = 'N'and wound.valid = 'Y' ");


        if (!string.IsNullOrWhiteSpace(condition))
        {
            sql.Append(condition);
        }
        sql.Append("order by CONVERT(k.tchi_value, UNSIGNED) asc ,k.tchi_value asc, CONVERT(z.tchi_value, UNSIGNED) asc, z.tchi_value asc, CONVERT(b.tchi_value, UNSIGNED) asc, b.tchi_value asc");
        return sql.ToString();

    }

    public static string get_search_client(string keyword, string search_index)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報

        int search_int = int.Parse(search_index);
        string condition = "";

        // client_id, client_number, client_card_id, chi_surname, chi_name, eng_surname, eng_name,
        //  sex, hkid, passport, dob, dob_month, nation, spoken_language, marital_status, child_boy_num, child_girl_num, education_level, LDS_num, religion, belongings, belongings2, property, entry_date, assessment_result, assessment_datetime, buy_type, remark, entry_status, living_status, agree_free_out, relationship_status,
        //  social_activities, active_status, created_by, created_datetime, modified_by, modified_datetime
        if (search_int == 0)
        {
            //search_field = "chi_name";

            //cmdText0 = "select * from client_personal Where  ( chi_name LIKE '%" + keyword + "%' or  chi_name_last LIKE '%" + keyword + "%' or concat(chi_name,chi_name_last) LIKE '%" + keyword + "%' ) and ( active_status = 'Y' )";
            condition = " where  ( per.chi_surname LIKE '%" + keyword + "%' or  per.chi_name LIKE '%" + keyword + "%' or concat(per.chi_surname, per.chi_name) LIKE '%" + keyword + "%' ) and ( per.active_status = 'Y' ) ";

        }
        else if (search_int == 1)
        {
            //   cmdText0 = "select * from client_personal Where  ( eng_firstname LIKE '%" + keyword + "%' or eng_lastname LIKE '%" + keyword + "%' ) and ( active_status = 'Y' )";
            condition = " Where  (per.eng_name LIKE '%" + keyword + "%' or per.eng_surname LIKE '%" + keyword + "%' ) and ( per.active_status = 'Y' )";

        }
        else if (search_int == 2)
        {
            condition = " Where (concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' ) and ( per.active_status = 'Y' ) ";

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 3)
        {
            condition = " Where ae.ae_status = '留醫中'   and ( per.active_status = 'Y' ) ";

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 4)
        {
            condition = " Where month(per.dob) = month(curdate())  and ( per.active_status = 'Y' ) ";
        }
        else if (search_int == 5)
        {
            condition = " Where (concat(k.tchi_value) = '" + keyword + "' ) and ( per.active_status = 'Y' ) ";

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 6)
        {
            condition = " Where (concat(k.tchi_value,'/',z.tchi_value) = '" + keyword + "' ) and ( per.active_status = 'Y' ) ";

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 7)
        {
            condition = " Where (concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) = '" + keyword + "' ) and ( per.active_status = 'Y' ) ";

            //cmdText0 = "select * from company_bed Where (  concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value) LIKE '%" + keyword + "%' )";

        }
        else if (search_int == 8)
        {
            condition = " Where wound.client_id is not null   and ( per.active_status = 'Y' ) ";
        }
        /*
        else if (search_int == 9)
        {
            condition = " where cleave.client_id is not null and per.active_status = 'Y' ";
        }*/
        return get_client_str(condition);
    }

    public static string get_client_photo(string client_id)
    {

        StringBuilder sql = new StringBuilder();


       // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
        sql.Append("select document_photo  from client_documents2 Where client_photo_id = '{0}' and valid = 'Y';");
        string comm = string.Format(sql.ToString(), client_id);

        return comm;


    }
    public static string post_client_photo(string [] values)
    {

        StringBuilder sql = new StringBuilder();
        sql.AppendFormat("update client_documents2  set valid= 'N' where client_id= {0};", values[1]);
 
 
       sql.Append("Insert into client_documents2(client_photo_id,client_id, document_photo) Values({0},{1},?parval);");
        // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
      // sql.Append("select document_photo  from client_documents2 Where client_photo_id = '{0}' and valid = 'Y';");
        string comm = string.Format(sql.ToString(), values);

        return comm;


    }

    //String cmdText0 = "Insert into client_documents2(client_photo_id,client_id, document_photo) Values(@client_photo_id,@client_id,@photo_data);";

    public static string get_bed_panel_brief(string condition)
    {

        //  SELECT display_id, ipv4, ipv6, listen_port, unique_device_id,
        //          page_index, usage_id, valid, created_by, created_datetime,
        //         modified_by, modified_datetime, last_update_datetime, orientation, reverse  FROM azure.sys_display_panel;

        StringBuilder sql = new StringBuilder();

 

        sql.Append("select   ");

        //  SELECT* FROM azure.sys_display_panel_bed;
        //   bed_panel_id, display_id, bed_id, alarm, night_mode,
        //  night_start, night_end, shown_revisit, shown_medical, shown_restraint, valid, created_by, created_datetime, modified_by, modified_datetime

        sql.Append(" bp.bed_panel_id ,bp.bed_id ,");
        sql.Append(" concat(block.tchi_value, '/', zone.tchi_value, '/', bed.tchi_value)  , bed.client_id ,");
        sql.Append(" concat(per.chi_surname, chi_name ),");
        sql.Append(" bp.alarm,");
        sql.Append(" bp.night_mode ,bp.night_start+12,bp.night_end ,");
        sql.Append(" bp.shown_revisit,bp.shown_medical,bp.shown_restraint, ");
        sql.Append(" bp.remind_revisit, bp.remind_medical ");
          
        sql.Append("from sys_display_panel panel ");

        sql.Append("left join sys_display_panel_bed as bp on panel.display_id = bp.display_id and bp.valid = 'Y' ");
        sql.Append("left JOIN sys_company_bed as bed ON bp.bed_id = bed.bed_id ");
        sql.Append("left join sys_company_zone zone on bed.zone_id = zone.zone_id   ");
        sql.Append("left join sys_company_block block on block.block_id = zone.block_id   ");
        sql.Append("left join client_personal2 as per on bed.client_id = per.client_id and bed.valid = 'Y' ");


        if (condition.Length > 0)
        {
            sql.Append("where unique_device_id !='' and panel.page_index = 4 and bp.valid = 'Y'  " + condition);
        }

        sql.Append(" order by display_name ");
        // sql.Append(Cshare.sql_bed_order);

        string comm = string.Format(sql.ToString());
        return comm;
    }
    public static string get_bed_panel(string condition)
    {

        //  SELECT display_id, ipv4, ipv6, listen_port, unique_device_id,
        //          page_index, usage_id, valid, created_by, created_datetime,
        //         modified_by, modified_datetime, last_update_datetime, orientation, reverse  FROM azure.sys_display_panel;

        StringBuilder sql = new StringBuilder();

 
        sql.Append("select   ");
        sql.Append("  panel.display_id, panel.ipv4, panel.ipv6, panel.listen_port,panel.display_name,panel.remark, panel.unique_device_id,  ");
        //  SELECT* FROM azure.sys_display_panel_bed;
        //   bed_panel_id, display_id, bed_id, alarm, night_mode,
        //  night_start, night_end, shown_revisit, shown_medical, shown_restraint, valid, created_by, created_datetime, modified_by, modified_datetime

        sql.Append(" bp.bed_panel_id ,bp.bed_id ,");
        sql.Append(" concat(block.tchi_value, '/', zone.tchi_value, '/', bed.tchi_value)  , bed.client_id ,");
        sql.Append(" concat(per.chi_surname, chi_name ),");
        sql.Append(" bp.alarm,");
        sql.Append(" bp.night_mode ,bp.night_start+12,bp.night_end ,");
        sql.Append(" bp.shown_revisit,bp.shown_medical,bp.shown_restraint, ");
        sql.Append(" bp.remind_revisit, bp.remind_medical ");

        sql.Append("from sys_display_panel panel ");

        sql.Append("left join sys_display_panel_bed as bp on panel.display_id = bp.display_id and bp.valid = 'Y' ");
        sql.Append("left JOIN sys_company_bed as bed ON bp.bed_id = bed.bed_id ");
        sql.Append("left join sys_company_zone zone on bed.zone_id = zone.zone_id   ");
        sql.Append("left join sys_company_block block on block.block_id = zone.block_id   ");
        sql.Append("left join client_personal2 as per on bed.client_id = per.client_id and bed.valid = 'Y' ");


        if (condition.Length > 0)
        {
            sql.Append("where panel.unique_device_id !='' and panel.page_index = 4 and bp.valid = 'Y'  " + condition);
        }

        sql.Append(" order by panel.display_name ");
        // sql.Append(Cshare.sql_bed_order);

        string comm = string.Format(sql.ToString());
        return comm;
    }
    public static string get_client_panel_briefing(string client_id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT CONCAT(per.chi_surname, per.chi_name),sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");

        sql.Append("CONCAT_WS(';', abi.self_care_ability, abi.movement_ability, abi.eating_ability,");
        sql.Append("abi.cognitive_ability, abi.audiovisual_ability, abi.communicate_ability, abi.incontinence, ability_remark), ");

        sql.Append("GROUP_CONCAT(care_wound_id),  ");
        sql.Append("ae.ae_status,");
        sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");
        sql.Append("if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),");
        sql.Append("doc.client_photo_id ");
        sql.Append(" FROM client_personal2 per ");
        //  sql.Append(" client_personal Where client_id=@id");
        //   SELECT* FROM greenery.medical_ae; ae_id, client_id, ae_status, ae_in_reason, ae_in_datetime, ae_in_remark, hospitalized, 
        //   ae_in_bed_num, ae_out_reason, ae_addr_id, ae_out_datetime, ae_out_remark, created_by, created_datetime, modified_by, modified_datetime, valid


        //     IF(a.addressid IS NULL, 0, 1)

        // SELECT* FROM greenery.client_diet; client_diet_id, client_id,
        //  prefered_rice, prefered_groceries, prefered_thickener, diet_preference, created_by, created_datetime, valid

        sql.Append("left join client_ability abi on abi.client_id = per.client_id and abi.valid='Y' ");
        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
        sql.Append("left join care_wound3 wou on wou.client_id = per.client_id and recovery ='N'   ");
        sql.Append("left join medical_ae ae on ae.client_id = per.client_id and ae_status = '留醫中' and   ae.valid  = 'Y'  ");
        sql.Append("left join client_diet diet on diet.client_id = per.client_id and  diet.valid  = 'Y'  ");
        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("where per.client_id = '{0}';  ");

        /*

        sql.Append("SELECT CONCAT(a.chi_name, a.chi_name_last),CONCAT(a.dob),a.sex,CONCAT(b.block, '-', b.zone, '-', b.bed_num),c.document_photo ,");
        sql.Append("GROUP_CONCAT(DISTINCT g.tchi_value SEPARATOR ';'),CONCAT_WS(' ', cast(m.diagnosis1 as char), CAST(m.diagnosis2 as char)),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name separator ';'),m.other_allergen,");
        sql.Append("GROUP_CONCAT(DISTINCT d.medicine_common_name separator ';'),CONCAT(a.eng_lastname,' ', a.eng_firstname),a.hkid,a.assessment_result  FROM client_personal2 AS a ");
        sql.Append("LEFT JOIN company_bed AS b ON a.client_id = b.client_id ");
        sql.Append("LEFT JOIN client_documents AS c ON a.client_id = c.client_id ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = a.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine d on m.drug_allergen REGEXP d.medicine_id where a.client_id = '{0}' GROUP BY a.client_id; ");
 


    */

        sql.Append("SELECT CONCAT(per.chi_surname,per.chi_name),CONCAT(per.dob),per.sex,concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value), ");
        sql.Append("concat( cast(m.sickness_brief as char) ),");
        sql.Append("GROUP_CONCAT(DISTINCT g.eng_value SEPARATOR ';'),CAST(m.diagnosis1 as char),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name_eng separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name_eng separator ';'),m.other_allergen,");
        sql.Append("m.allergen_remark,per.assessment_result ,CONCAT(per.eng_surname,' ',per.eng_name),per.hkid,per.client_number   FROM client_personal2 AS per ");

        sql.Append("LEFT JOIN client_documents2 AS c ON per.client_id = c.client_id and c.valid = 'Y' ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = per.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine2 h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine2 d on m.drug_allergen REGEXP d.medicine_id ");
        sql.Append("left JOIN sys_company_bed as b ON per.client_id = b.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");
        sql.Append(" where per.client_id = '{0}'  ");
        sql.Append(" ;");



        string comm = string.Format(sql.ToString(), client_id);
        return comm;
    }
    public static string get_client_panel_entry_briefing(string condition)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT CONCAT(per.chi_surname, per.chi_name),sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");

        sql.Append("CONCAT_WS(';', abi.self_care_ability, abi.movement_ability, abi.eating_ability,");
        sql.Append("abi.cognitive_ability, abi.audiovisual_ability, abi.communicate_ability, abi.incontinence, ability_remark), ");

        sql.Append("GROUP_CONCAT(care_wound_id),  ");
        sql.Append("ae.ae_status,");
        // sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");

        sql.Append(" group_concat(DISTINCT dt.type_name,' ', diet.tchi_value separator '\n') ,");

        sql.Append("doc.client_photo_id,per.contact_precaution,lift.tchi_value,tool.tchi_value ,sysres.tchi_value,");
        sql.Append("group_concat(DISTINCT eve.tchi_value,'', memo.remark separator ' '),sysdia.tchi_value,    ");
        sql.Append("group_concat(DISTINCT res.restraint_items,'', res.other_restraint_items,' ',res.restraint_items_remarks separator ' '),  ");
        sql.Append(" if(per.contact_precaution='Y','CT',''),if(abi.oxygen='Y','OO',''),  (SELECT icon_code FROM sys_display_panel_bed_icon WHERE tchi_value=abi.teeth and type_id = 1) AS teeth ,per.assessment_result,DATE_FORMAT(per.entry_date, '%Y/%m/%d'),per.client_id    ");
        sql.Append(" FROM client_personal2 per ");
        //  sql.Append(" client_personal Where client_id=@id");
        //   SELECT* FROM greenery.medical_ae; ae_id, client_id, ae_status, ae_in_reason, ae_in_datetime, ae_in_remark, hospitalized, 
        //   ae_in_bed_num, ae_out_reason, ae_addr_id, ae_out_datetime, ae_out_remark, created_by, created_datetime, modified_by, modified_datetime, valid


        //     IF(a.addressid IS NULL, 0, 1)

        // SELECT* FROM greenery.client_diet; client_diet_id, client_id,
        //  prefered_rice, prefered_groceries, prefered_thickener, diet_preference, created_by, created_datetime, valid

        sql.Append("left join client_ability abi on abi.client_id = per.client_id and abi.valid='Y' ");
        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
        sql.Append("left join care_wound3 wou on wou.client_id = per.client_id and recovery ='N'   ");
        sql.Append("left join medical_ae ae on ae.client_id = per.client_id and ae_status = '留醫中' and   ae.valid  = 'Y'  ");
        // sql.Append("left join client_diet diet on diet.client_id = per.client_id and  diet.valid  = 'Y'  ");


        sql.Append(" LEFT JOIN client_diet_preference AS pre ON per.client_id = pre.client_id and pre.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_preference AS diet ON diet.diet_id = pre.diet_id and diet.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diet_type AS dt ON dt.diet_type_id = diet.diet_type_id   ");


        sql.Append("left join client_responsible_record respon on per.client_id = respon.client_id and respon.valid ='Y'  ");
        sql.Append("left join sys_client_responsible sysres on sysres.responsible_id = respon.responsible_id and sysres.valid ='Y'   ");

        sql.Append(" LEFT JOIN sys_client_help_lifting AS lift ON abi.help_lifting = lift.lifting_id and lift.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_assisting_tools AS tool ON abi.assisting_tools = tool.tools_id and tool.valid = 'Y'  ");

        sql.Append(" LEFT JOIN client_memo_event AS memo ON memo.client_id = per.client_id and memo.valid = 'Y'  ");
        sql.Append(" and ( date( memo.end_datetime)>= curdate() or date( memo.end_datetime) = date('1900-05-16')) ");

        sql.Append("LEFT JOIN sys_client_memo_event eve  on eve.memo_event_id = memo.memo_event_id ");

        sql.Append("LEFT JOIN client_diaper_event dia  on dia.client_id = per.client_id and dia.valid = 'Y' ");
        sql.Append(" LEFT JOIN sys_client_diaper AS sysdia ON sysdia.diaper_id = dia.diaper_id and sysdia.valid = 'Y' ");


        sql.Append("LEFT JOIN nursing_restraint res  on res.client_id = per.client_id and res.valid = 'Y'   ");
        sql.Append("and (res.restraint_state ='親屬口頭同意' or res.restraint_state = '有效' ) ");
        //    restraint_state
        //親屬口頭同意
        //有效
        //   SELECT* FROM azure.nursing_restraint;
        //    restraint_items


        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("where per.client_id   !=''   ");
        if (condition.Length>0)
        {
            sql.Append(condition);
        }
        sql.Append(";");
        /*

        sql.Append("SELECT CONCAT(a.chi_name, a.chi_name_last),CONCAT(a.dob),a.sex,CONCAT(b.block, '-', b.zone, '-', b.bed_num),c.document_photo ,");
        sql.Append("GROUP_CONCAT(DISTINCT g.tchi_value SEPARATOR ';'),CONCAT_WS(' ', cast(m.diagnosis1 as char), CAST(m.diagnosis2 as char)),");
        sql.Append("GROUP_CONCAT(DISTINCT h.medicine_plain_name separator ';'),GROUP_CONCAT(DISTINCT d.medicine_plain_name separator ';'),m.other_allergen,");
        sql.Append("GROUP_CONCAT(DISTINCT d.medicine_common_name separator ';'),CONCAT(a.eng_lastname,' ', a.eng_firstname),a.hkid,a.assessment_result  FROM client_personal2 AS a ");
        sql.Append("LEFT JOIN company_bed AS b ON a.client_id = b.client_id ");
        sql.Append("LEFT JOIN client_documents AS c ON a.client_id = c.client_id ");
        sql.Append("LEFT JOIN medical_history AS m ON m.client_id = a.client_id ");
        sql.Append("LEFT JOIN medical_icd9 g ON m.icd_9 REGEXP g.uid ");
        sql.Append("LEFT JOIN med_medicine h ON m.drug_adverse REGEXP h.medicine_id ");
        sql.Append("LEFT JOIN med_medicine d on m.drug_allergen REGEXP d.medicine_id where a.client_id = '{0}' GROUP BY a.client_id; ");
 


    */

 


      string comm = string.Format(sql.ToString() );
        return comm;
    }

 

    public static string insert_client_log(string[] vals)
    {
        StringBuilder sql = new StringBuilder();
        //SELECT* FROM azure.sys_client_food_menu;
        // menu_id, menu_name, start_datetime, end_datetime, created_by, created_datetime, modified_by, modified_datetime, valid
        // SELECT* FROM alpine_test.client_log_reply;
        // client_log_reply reply_id, log_id, content, reply_datetime, reply_ordering, modified_by, modified_datetime, created_by, created_datetime, valid

        sql.Append("insert client_log(record_id,log_id, client_id,department_id,title,event_datetime, content,dash_board_shown,dash_board_always_shown,log_always_shown, ");
        sql.Append("boardcast_datetime,created_by, created_datetime )values");

        //  string id = Cshare.Get_mysql_database_MaxID("client_log", "log_id");
        //   string[] client_id = vals[1].Split(',');

        //   vals[0] = id;
        //  vals[1] = client_id[i].ToString();
        sql.AppendFormat("({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", vals);
 
        //  id = (int.Parse(id) + 1).ToString();




        return string.Format(sql.ToString(), vals);
    }
    public static string insert_client_log_reply(string[] vals)
    {
        StringBuilder sql = new StringBuilder();
        //SELECT* FROM azure.sys_client_food_menu;
        // menu_id, menu_name, start_datetime, end_datetime, created_by, created_datetime, modified_by, modified_datetime, valid
        // SELECT* FROM alpine_test.client_log_reply;
        // client_log_reply reply_id, log_id, content, reply_datetime, reply_ordering, modified_by, modified_datetime, created_by, created_datetime, valid

        sql.Append("insert client_log_reply(  reply_id, log_id,reply_datetime,content,reply_ordering , ");
        sql.Append("created_by, created_datetime )values");

        //  string id = Cshare.Get_mysql_database_MaxID("client_log", "log_id");
        //   string[] client_id = vals[1].Split(',');

        //   vals[0] = id;
        //  vals[1] = client_id[i].ToString();
        sql.AppendFormat("({0},'{1}','{2}','{3}','{4}','{5}','{6}')", vals);
        //  id = (int.Parse(id) + 1).ToString();



        return string.Format(sql.ToString(), vals);
    }

    public static string get_client_log_setting(string condition)
    {
        //SELECT* FROM gracenursing.company_department;
        // log_id, client_id, log_department_id, start_datetime, end_datetime, remark, modified_by, modified_datetime, created_by, created_datetime, valid
        StringBuilder sql = new StringBuilder();
       // SELECT* FROM anidemo2.sys_company_department;
      //  department_id, department_name, modified_by, modified_datetime, created_by, created_datetime, valid
        sql.Append("select  department_id, department_name ");
        sql.Append(" from sys_company_department where valid = 'Y' ");
        //  SELECT* FROM gracenursing.sys_company_department;
        //   log_department_id, tchi_value, modified_by, modified_datetime, created_by, created_datetime, valid
        //   SELECT* FROM alpine.client_log_reply;
        //   reply_id, log_id, content, reply_datetime, reply_ordering, modified_by, modified_datetime, created_by, created_datetime, valid


       // sql.Append("  where  log.valid = 'Y' and reply2.log_id is null  ");

        if (condition.Length > 0)
        {
            sql.AppendFormat(" {0}  ", condition);
        }

        sql.Append("  ; ");

        return (sql.ToString());
    }
    public static string    get_client_log(string condition, string last_record, bool need_photo = false)
    {
        //SELECT* FROM gracenursing.company_department;
        // log_id, client_id, log_department_id, start_datetime, end_datetime, remark, modified_by, modified_datetime, created_by, created_datetime, valid
        StringBuilder sql = new StringBuilder();

        sql.Append("select record_id, log.log_id, per.client_id ,");
        sql.Append(" IFNULL(concat(per.chi_surname,per.chi_name),''),log.log_always_shown ,");
        sql.Append("log.department_id,eve.department_name,log.title ,  ");
        sql.Append("DATE_FORMAT(log.event_datetime, '%Y-%m-%d %H:%i'),log.content,");


        //    sql.Append("concat(log.content,if(GROUP_CONCAT(reply.content) IS NULL,'','\n\n--最新回覆--\n'), ifnull(substring_index(GROUP_CONCAT(DISTINCT concat('#', reply_ordering,'.',' - ',reply.created_by,' ',DATE_FORMAT(reply.reply_datetime, '%Y-%m-%d %H:%i'),' -', '\n', reply.content , '\n'  )   ");
        //     sql.AppendFormat("ORDER BY reply.reply_ordering asc SEPARATOR 'log_seperator' ),'log_seperator',{0}),'')) as result , ", string.Format("-{0}",last_record));
        sql.Append("concat(log.content,if(reply.content IS NULL,'', concat('\n\n----------最新回覆----------\n','#', reply.reply_ordering,'.',' - ',reply.created_by,' ',DATE_FORMAT(reply.reply_datetime, '%Y-%m-%d %H:%i'),' -', '\n', reply.content , '\n') ");
        sql.Append(")) as result , ");
        sql.Append("reply.reply_id, reply.content ,reply.created_by,DATE_FORMAT(reply.reply_datetime, '%Y-%m-%d %H:%i'),reply.reply_ordering, ");
        sql.Append("count(reply3.content), dash_board_shown,dash_board_always_shown,log_always_shown, ");

        /*
        sql.Append("SELECT per.client_id, acc.account_num,IFNULL(concat(per.chi_surname,per.chi_name), ''),concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value),");
        sql.Append("ifnull(a.total_balance, '0.00') ,ifnull(c.total_balance, '0.00'),(ifnull(c.total_balance, '0.00')-ifnull(a.total_balance, '0.00')) ");

        sql.Append("  FROM acc_account2 AS acc ");
        //DATE_FORMAT(a.body_check_expiry_date, '%Y-%m-%d %H:%i'),
        //concat(k.tchi_value, '/', z.tchi_value, '/', b.tchi_value)
        sql.Append("left JOIN client_personal2 as per ON per.client_number = acc.account_num ");

        sql.Append("left join acc_deposit as a on a.account_num = acc.account_num and a.valid = 'Y' " + string.Format("and  a.created_datetime <= {0} ", dep_st_condition));
        sql.Append("left join acc_deposit as a2 on a.account_num = a2.account_num and a.acc_deposit_id < a2.acc_deposit_id and a2.valid = 'Y' " + string.Format("and  a2.created_datetime <= {0} ", dep_st_condition));
        */


        sql.Append(get_sql_log_col("log"));
        sql.Append(",DATE_FORMAT(log.boardcast_datetime, '%Y-%m-%d'), ");
        sql.Append(" if(DATE_FORMAT(reply.reply_datetime, '%Y/%m/%d %H:%i')is null,DATE_FORMAT(log.created_datetime, '%Y/%m/%d %H:%i'),  DATE_FORMAT(reply.reply_datetime,'%Y/%m/%d %H:%i')) as updatedatetime ");
        sql.Append(need_photo == true ? ",doc.document_photo " : "");
        sql.Append("from client_log log ");
        //  SELECT* FROM gracenursing.sys_company_department;
        //   log_department_id, tchi_value, modified_by, modified_datetime, created_by, created_datetime, valid

        sql.Append("LEFT JOIN client_log_reply reply  on reply.log_id = log.log_id and reply.valid ='Y' ");
        sql.Append("LEFT JOIN client_log_reply reply2  on reply2.log_id = log.log_id and reply.reply_ordering < reply2.reply_ordering and reply2.valid = 'Y' ");
        sql.Append("LEFT JOIN client_log_reply reply3  on reply3.log_id = log.log_id and reply3.valid ='Y' ");


        sql.Append("LEFT JOIN sys_company_department eve  on eve.department_id = log.department_id ");
        sql.Append("LEFT JOIN client_personal2 per  on log.client_id = per.client_id ");
        sql.Append("LEFT JOIN client_documents2 doc  on doc.client_id = per.client_id  and doc.valid='Y' ");
        sql.Append("LEFT JOIN sys_company_bed bed  on per.client_id = bed.client_id ");
        sql.Append("left join sys_company_zone z on bed.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");

        //   SELECT* FROM alpine.client_log_reply;
        //   reply_id, log_id, content, reply_datetime, reply_ordering, modified_by, modified_datetime, created_by, created_datetime, valid


        sql.Append("  where  log.valid = 'Y' and reply2.log_id is null  ");

        if (condition.Length > 0)
        {
            sql.AppendFormat(" {0}  ", condition);
        }
       
        return (sql.ToString());
    }

    public static string get_client_log_detail(string condition, bool need_photo = false)
    {
        //SELECT* FROM gracenursing.company_department;
        // log_id, client_id, log_department_id, start_datetime, end_datetime, remark, modified_by, modified_datetime, created_by, created_datetime, valid
        StringBuilder sql = new StringBuilder();

        sql.Append("select log.log_id, per.client_id ,");
        sql.Append(" IFNULL(concat(per.chi_surname,per.chi_name),''),");
        sql.Append("eve.department_name,log.title ,  ");
        sql.Append("DATE_FORMAT(log.event_datetime, '%Y-%m-%d %H:%i'),");


        sql.Append(" log.content ,log_always_shown,");


        //  sql.Append("dash_board_shown,dash_board_always_shown,DATE_FORMAT(log.boardcast_datetime, '%Y-%m-%d %H:%i'), ");


        sql.Append(get_sql_log_col("log"));
        sql.Append(need_photo == true ? ",doc.document_photo " : "");
        sql.Append("from client_log log ");
        //  SELECT* FROM gracenursing.sys_company_department;
        //   log_department_id, tchi_value, modified_by, modified_datetime, created_by, created_datetime, valid



        sql.Append("LEFT JOIN sys_company_department eve  on eve.department_id = log.department_id ");
        sql.Append("LEFT JOIN client_personal2 per  on log.client_id = per.client_id ");
        sql.Append("LEFT JOIN client_documents2 doc  on doc.client_id = per.client_id  and doc.valid='Y' ");
        sql.Append("LEFT JOIN sys_company_bed bed  on per.client_id = bed.client_id ");
        sql.Append("left join sys_company_zone z on bed.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id   ");

        //   SELECT* FROM alpine.client_log_reply;
        //   reply_id, log_id, content, reply_datetime, reply_ordering, modified_by, modified_datetime, created_by, created_datetime, valid


        sql.Append("  where  log.valid = 'Y'  ");
        if (condition.Length > 0)
        {
            sql.AppendFormat(" {0}  ", condition);
        }

        return (sql.ToString());
    }
    public static string get_client_log_reply_detail(string condition, bool need_photo = false)
    {
        //SELECT* FROM gracenursing.company_department;
        // log_id, client_id, log_department_id, start_datetime, end_datetime, remark, modified_by, modified_datetime, created_by, created_datetime, valid
        StringBuilder sql = new StringBuilder();

        sql.Append("select reply.reply_id,reply.reply_id,reply_ordering,  ");

        //SELECT* FROM alpine.client_log_reply;
        //reply_id, log_id, content, reply_datetime, reply_ordering, modified_by, modified_datetime, created_by, created_datetime, valid


        sql.Append(" reply.content ,");

        //   sql.Append("reply.created_by,DATE_FORMAT(reply.created_datetime, '%Y-%m-%d %H:%i'),");
        //   sql.Append("reply.modified_by,DATE_FORMAT(reply.modified_datetime, '%Y-%m-%d %H:%i'),");

        sql.Append(get_sql_log_col("reply"));
        sql.Append(need_photo == true ? ",doc.document_photo " : "");
        sql.Append("from client_log_reply reply ");
        //  SELECT* FROM gracenursing.sys_company_department;
        //   log_department_id, tchi_value, modified_by, modified_datetime, created_by, created_datetime, valid

        //   SELECT* FROM alpine.client_log_reply;
        //   reply_id, log_id, content, reply_datetime, reply_ordering, modified_by, modified_datetime, created_by, created_datetime, valid


        sql.Append("  where  reply.valid = 'Y'  ");
        if (condition.Length > 0)
        {
            sql.AppendFormat(" {0}  ", condition);
        }

        return (sql.ToString());
    }
    public static string get_sql_log_col(string prefix)
    {
        if (prefix.Length > 0)
        {
            string log_sql = string.Format("{0}.created_by," +
                "DATE_FORMAT({0}.created_datetime, '%Y/%m/%d %H:%i')," +
                " {0}.modified_by," +
                " DATE_FORMAT({0}.modified_datetime, '%Y/%m/%d %H:%i') ", prefix);
            return log_sql;
        }
        else
        {
            string log_sql = string.Format(" created_by," +
"DATE_FORMAT( created_datetime, '%Y/%m/%d %H:%i')," +
"  modified_by," +
" DATE_FORMAT( modified_datetime, '%Y/%m/%d %H:%i') ");
            return log_sql;
        }
    }
}

