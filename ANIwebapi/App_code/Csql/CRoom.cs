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
internal class CRoom  
{

    public static  string get_client_id_in_nfc(string   id)
    {

        StringBuilder sql = new StringBuilder();
        // sql.Append("insert acc_charge_consumed(charge_id, account_num, invoice_id, charge_item_id, charge_status, charge_quantity,");
        // sql.Append("charge_amount, charge_datetime, charge_item_remark, created_by, created_datetime)");
        // sql.Append("VALUES({0},'{1}',{2},{3},'{4}',{5},{6},'{7}','{8}','{9}','{10}');");
 
        sql.Append("select client_id, usage_code, usage_id from client_nfc_id Where nfc_id = '{0}' and valid = 'Y' ;");
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

 
        sql.Append("from  ");
        sql.Append("left join sys_company_bed b  on b.client_id = per.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
 
        sql.Append("where k.bed_id = '{0}'; ");

 

        string comm = string.Format(sql.ToString(), id);
        return comm;
    }

    public static string get_client_briefing(string id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報
        sql.Append("SELECT CONCAT(per.chi_surname, per.chi_name),sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),  ");

        sql.Append("CONCAT_WS(';', abi.self_care_ability, abi.movement_ability, abi.eating_ability,");
        sql.Append("abi.cognitive_ability, abi.audiovisual_ability, abi.communicate_ability, abi.incontinence, ability_remark), ");

        sql.Append("GROUP_CONCAT(care_wound_id),  ");
        sql.Append("ae.ae_status,");
        sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");
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

 

        string comm = string.Format(sql.ToString(), id);
        return comm;
    }
    public static string get_company_photo(string picture_id)
    {

        StringBuilder sql = new StringBuilder();
 

        // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
        sql.Append(" select document_logo from company_documents where company_id = '{0}';");
        string comm = string.Format(sql.ToString(), picture_id);

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
        sql.Append("order by concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value)  ");

        return sql.ToString();
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



    public static string get_room_panel_briefing(string room_id)
    {
        StringBuilder sql = new StringBuilder();
        //查詢客戶簡報

    //    SELECT* FROM azure.sys_display_room_panel;
   //     room_id, room_name, room_remark, valid, created_by, created_datetime, modified_by, modified_datetime

//SELECT* FROM azure.sys_display_room_bed;
 ///       room_client_id, room_id, bed_id, valid


        sql.Append("SELECT room.room_name,room_remark,rb.bed_id,per.client_id,  CONCAT(per.chi_surname, per.chi_name), per.sex,");
        sql.Append("DATE_FORMAT(dob, '%Y/%m/%d'),doc.client_photo_id ,concat(k.tchi_value,'/',z.tchi_value,'/',b.tchi_value),b.alarm   ");

       // sql.Append("CONCAT_WS(';', abi.self_care_ability, abi.movement_ability, abi.eating_ability,");
       // sql.Append("abi.cognitive_ability, abi.audiovisual_ability, abi.communicate_ability, abi.incontinence, ability_remark), ");

      //  sql.Append("GROUP_CONCAT(care_wound_id),  ");
      //  sql.Append("ae.ae_status,");
     //   sql.Append("concat(if(diet.prefered_rice='','',concat(diet.prefered_rice)),if(diet.prefered_groceries='','',concat(' ',prefered_groceries)),if(diet.diet_preference='','',concat(' ',diet.diet_preference))),");
     //   sql.Append("if(diet.prefered_thickener = '', '',concat(' 凝固粉 ',diet.prefered_thickener)),");
 
        sql.Append(" FROM sys_display_panel_room room ");
 
        // SELECT* FROM greenery.client_diet; client_diet_id, client_id,
        //  prefered_rice, prefered_groceries, prefered_thickener, diet_preference, created_by, created_datetime, valid
        sql.Append("left join sys_display_room_bed rb on room.room_id = rb.room_id and rb.valid='Y' ");

        sql.Append("LEFT JOIN sys_company_bed b  on b.bed_id = rb.bed_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id   ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id  ");
        sql.Append("left join client_personal2 per on per.client_id = b.client_id  ");
        sql.Append("left join sys_display_panel_bed bed_panel on bed_panel.display_id = room.display_id and bed_panel.valid = 'Y'  ");
        sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");


        // sql.Append("left join care_wound3 wou on wou.client_id = per.client_id and recovery ='N'   ");
        //  sql.Append("left join medical_ae ae on ae.client_id = per.client_id and ae_status = '留醫中' and   ae.valid  = 'Y'  ");
        //  sql.Append("left join client_diet diet on diet.client_id = per.client_id and  diet.valid  = 'Y'  ");
        //  sql.Append("left join client_documents2 doc on doc.client_id = per.client_id and  doc.valid  = 'Y'  ");
        sql.Append("where  room.valid='Y'    ");
        if (room_id.Length>0)
        {
            sql.AppendFormat("{0} ",room_id);
        }


        sql.Append(" order by rb.record_id  ;  ");

        sql.Append("select company_id from company_documents where company_id = 100000;");
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




        string comm = string.Format(sql.ToString(), room_id);
        return comm;
    }
}

