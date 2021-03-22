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
internal class CPanel
{
    public string get_all_panel_version(string condition)
    {

        // SELECT * FROM azure.sys_display_panel_version;
      //  version_id, version, UpdatDirectory, valid, created_by, created_datetime
                //          page_index, usage_id, valid, created_by, created_datetime,
                //         modified_by, modified_datetime, last_update_datetime, orientation, reverse  FROM azure.sys_display_panel;

                StringBuilder sql = new StringBuilder();
        sql.Append("SELECT  version, UpdatDirectory ");

        sql.Append("from sys_display_panel_version panel ");
        if (condition.Length > 0)
        {
            sql.Append("where valid ='Y'  " + condition);
        }
        //sql.Append("group by nfc_id ");
        //sql.Append("order by display_name ");
        // sql.Append(Cshare.sql_bed_order);
        return (sql.ToString());
    }

    public string get_all_panel(string condition)
    {

        //  SELECT display_id, ipv4, ipv6, listen_port, unique_device_id,
        //          page_index, usage_id, valid, created_by, created_datetime,
        //         modified_by, modified_datetime, last_update_datetime, orientation, reverse  FROM azure.sys_display_panel;

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT display_id, ipv4, ipv6, listen_port,display_name,remark, unique_device_id, " +
            "   page_index,orientation, usage_id, last_update_datetime ");

        sql.Append("from sys_display_panel panel ");



        if (condition.Length > 0)
        {
            sql.Append("where unique_device_id !=''  " + condition);
        }
        //sql.Append("group by nfc_id ");
        sql.Append("order by display_name ");
        // sql.Append(Cshare.sql_bed_order);

        return  (sql.ToString());
    }
    public string get_all_alarm_panel(string condition)
    {

        //  SELECT display_id, ipv4, ipv6, listen_port, unique_device_id,
        //          page_index, usage_id, valid, created_by, created_datetime,
        //         modified_by, modified_datetime, last_update_datetime, orientation, reverse  FROM azure.sys_display_panel;

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT panel.display_id, panel.ipv4, panel.ipv6, panel.listen_port,panel.display_name,panel.remark, panel.unique_device_id, " +
            "   panel.page_index,panel.orientation, panel.usage_id, panel.last_update_datetime,bp.bed_id ");

        sql.Append("from sys_display_panel panel ");
   

        sql.Append("left join sys_display_panel_bed as bp on panel.display_id = bp.display_id and bp.valid = 'Y' ");
 

        if (condition.Length > 0)
        {
            sql.Append("where panel.unique_device_id !=''  " + condition);
        }
        //sql.Append("group by nfc_id ");
        sql.Append("order by panel.display_name ");
        sql.Append(" ;");
        // sql.Append(Cshare.sql_bed_order);

        return (sql.ToString());
    }


 


    public string get_public_info_panel_brief(string condition)
    {

        //  SELECT display_id, ipv4, ipv6, listen_port, unique_device_id,
        //          page_index, usage_id, valid, created_by, created_datetime,
        //         modified_by, modified_datetime, last_update_datetime, orientation, reverse  FROM azure.sys_display_panel;

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT panel.display_id, panel.ipv4, panel.ipv6, panel.listen_port,panel.display_name, panel.unique_device_id,last_update_datetime,");


        sql.Append(" public_panel_id , ");
        sql.Append(" GROUP_CONCAT(DISTINCT concat(type.tchi_value, ' ', content.last_period   ) order by content.ordering separator ';') , ");
        sql.Append(" shown_header, shown_footer ");

        sql.Append("from sys_display_panel panel ");
        //public_panel_id, display_id, content_id, valid, shown_header, shown_footer,
        //created_by, created_datetime, modified_by, modified_datetime
        sql.Append("left join sys_display_panel_public as pub on panel.display_id = pub.display_id and pub.valid = 'Y' ");
        sql.Append("left join sys_display_public_content as content on pub.content_id = content.content_id and content.valid = 'Y' ");
        sql.Append("left join sys_display_public_content_type as type on content.content_type_id = type.content_type_id  ");
        //SELECT* FROM azure.sys_display_public_content;
        //content_record_id, content_id, content_type_id, content, landscape_image_id, portrait _image_id, last_period, valid, created_by, created_datetime

        //SELECT* FROM azure.sys_display_public_content_type;
        //content_type_id, tchi_value, eng_value

        if (condition.Length > 0)
        {
            sql.Append("where unique_device_id !='' and panel.page_index = 1  " + condition);
        }
        //sql.Append("group by nfc_id ");
        sql.Append(" group by content.content_id  order by display_name ");
        // sql.Append(Cshare.sql_bed_order);

        return sql.ToString();
    }
    public string get_public_info_panel(string condition)
    {

        //  SELECT display_id, ipv4, ipv6, listen_port, unique_device_id,
        //          page_index, usage_id, valid, created_by, created_datetime,
        //         modified_by, modified_datetime, last_update_datetime, orientation, reverse  FROM azure.sys_display_panel;

        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT panel.display_id, panel.ipv4, panel.ipv6, panel.listen_port,panel.display_name, panel.unique_device_id,last_update_datetime,");



        sql.Append(" public_panel_id ,pub.content_id, ");
        sql.Append(" content.content_record_id, ");
        sql.Append(" type.tchi_value,type.content_type_index,");
        sql.Append(" content.last_period,ifnull(food.menu_name,content.content),content.usage_id,content.header, content.footer,content.ordering  , ");
        sql.Append(" content.landscape_image_id, content.portrait_image_id ");

        //            content_record_id, content_id, content_type_id, content, landscape_image_id, portrait _image_id, last_period, ordering, header, footer, valid, created_by, created_datetime
        sql.Append("from sys_display_panel panel ");
        //public_panel_id, display_id, content_id, valid, shown_header, shown_footer,
        //created_by, created_datetime, modified_by, modified_datetime


        sql.Append("left join sys_display_panel_public as pub on panel.display_id = pub.display_id and pub.valid = 'Y' ");
        sql.Append("left join sys_display_public_content as content on pub.content_id = content.content_id and content.valid = 'Y' ");
        sql.Append("left join sys_display_public_content_type as type on content.content_type_id = type.content_type_id  ");
        sql.Append("left join sys_client_food_menu as food on type.content_type_index = 0 and content.usage_id = food.menu_id ");
        //           SELECT* FROM azure.sys_client_food_menu;
        //           menu_id, menu_name, start_datetime, end_datetime, created_by, created_datetime, modified_by, modified_datetime, valid
        //SELECT* FROM azure.sys_display_public_content;
        //content_record_id, content_id, content_type_id, content, landscape_image_id, portrait _image_id, last_period, valid, created_by, created_datetime

        //SELECT* FROM azure.sys_display_public_content_type;
        //content_type_id, tchi_value, eng_value

        if (condition.Length > 0)
        {
            sql.Append("where unique_device_id !='' and panel.page_index = 1  " + condition);
        }
        //sql.Append("group by nfc_id ");
        sql.Append("  order by content.ordering  ");
        // sql.Append(Cshare.sql_bed_order);

        return sql.ToString();
    }
    public string get_public_info_panel_picture(string condition)
    {

        //  SELECT display_id, ipv4, ipv6, listen_port, unique_device_id,
        //          page_index, usage_id, valid, created_by, created_datetime,
        //         modified_by, modified_datetime, last_update_datetime, orientation, reverse  FROM azure.sys_display_panel;

        StringBuilder sql = new StringBuilder();

        //    image_id, image_type_id, content, valid, created_by, created_datetime

        sql.Append("SELECT image_id,   content ");


        sql.Append("from sys_display_public_picture  ");
        //public_panel_id, display_id, content_id, valid, shown_header, shown_footer,
        //created_by, created_datetime, modified_by, modified_datetime

        //SELECT* FROM azure.sys_display_public_content;
        //content_record_id, content_id, content_type_id, content, landscape_image_id, portrait _image_id, last_period, valid, created_by, created_datetime

        //SELECT* FROM azure.sys_display_public_content_type;
        //content_type_id, tchi_value, eng_value

        if (condition.Length > 0)
        {
            sql.Append("where image_id !='' " + condition);
        }
        //sql.Append("group by nfc_id ");
        //sql.Append(" group by content.content_id  order by display_name ");
        // sql.Append(Cshare.sql_bed_order);

        return sql.ToString();
    }
    public string get_public_info_content_type(string condition)
    {

        //  SELECT* FROM azure.sys_display_public_content;
        //  content_record_id, content_id, content_type_id, content, landscape_image_id, portrait _image_id, last_period, ordering, valid, created_by, created_datetime

        StringBuilder sql = new StringBuilder();
        //SELECT* FROM azure.sys_display_public_content_type;

        //content_type_id, tchi_value, eng_value, valid



        sql.Append("SELECT content_type_id,tchi_value ");


        sql.Append("from sys_display_public_content_type  ");

        if (condition.Length > 0)
        {
            sql.Append("where content_type_id !='' and valid='Y' " + condition);
        }

        return sql.ToString();
    }
    public static string get_poster_photo(string poster_id)
    {

        StringBuilder sql = new StringBuilder();

       // SELECT* FROM futai.sys_display_public_picture;
      //  image_id, image_type_id, content, valid, created_by, created_datetime, modified_by, modified_datetime
        // String cmdText = "select * from client_documents2 Where client_photo_id IN (@id) and valid = 'Y'";
        sql.Append(" select content from sys_display_public_picture where image_id = '{0}' and valid='Y';");
        string comm = string.Format(sql.ToString(), poster_id);

        return comm;


    }
    public string get_food_menu_content_brief(string condition)
    {
        //SELECT* FROM azure.sys_client_food_menu_content;
        // content_id, menu_id, breakfast, lunch, soup, fruit, afternoon_tea, dinner, midnight_snacks, ordering, created_by, created_datetime, modified_by, modified_datetime, valid

        StringBuilder sql = new StringBuilder();
        sql.Append("select cont.content_id, cont.menu_id ,DATE_FORMAT(DATE_ADD(menu.start_datetime, INTERVAL cont.ordering DAY),  '%Y/%m/%d'),");
        sql.Append(" cont.breakfast, cont.lunch, cont.soup, cont.fruit, cont.afternoon_tea, cont.dinner, cont.midnight_snacks,menu.menu_name  ");
        sql.Append("from sys_client_food_menu_content cont  ");
        sql.Append(" left join sys_client_food_menu as menu on cont.menu_id = menu.menu_id  and menu.valid = 'Y'   ");


        //sql.Append(" left join sys_client_food_menu_content as cont  ");
        sql.Append("  where cont.valid = 'Y'  ");
        if (condition.Length > 0)
        {
            sql.AppendFormat(" {0}  ", condition);
        }

        return (sql.ToString());
    }
    public string get_food_menu_content_panel_brief(string first_condition,string second_condition)
    {
        //SELECT* FROM azure.sys_client_food_menu_content;
        // content_id, menu_id, breakfast, lunch, soup, fruit, afternoon_tea, dinner, midnight_snacks, ordering, created_by, created_datetime, modified_by, modified_datetime, valid

        StringBuilder sql = new StringBuilder();
        sql.Append("select cont.content_id, cont.menu_id ,DATE_FORMAT(DATE_ADD(menu.start_datetime, INTERVAL cont.ordering DAY),  '%Y/%m/%d'),");
        sql.Append(" cont.breakfast, cont.lunch, cont.soup, cont.fruit, cont.afternoon_tea, cont.dinner, cont.midnight_snacks,menu.menu_name  ");
        sql.Append("from sys_client_food_menu_content cont  ");
        sql.Append(" left join sys_client_food_menu as menu on cont.menu_id = menu.menu_id  and menu.valid = 'Y'   ");


        //sql.Append(" left join sys_client_food_menu_content as cont  ");
        sql.Append("  where cont.valid = 'Y'  ");
        if (first_condition.Length > 0)
        {
            sql.AppendFormat(" {0}  ;", first_condition);
        }
        sql.Append("select cont.content_id, cont.menu_id ,DATE_FORMAT(DATE_ADD(menu.start_datetime, INTERVAL cont.ordering DAY),  '%Y/%m/%d'),");
        sql.Append(" cont.breakfast, cont.lunch, cont.soup, cont.fruit, cont.afternoon_tea, cont.dinner, cont.midnight_snacks,menu.menu_name  ");
        sql.Append("from sys_client_food_menu_content cont  ");
        sql.Append(" left join sys_client_food_menu as menu on cont.menu_id = menu.menu_id  and menu.valid = 'Y'   ");


        //sql.Append(" left join sys_client_food_menu_content as cont  ");
        sql.Append("  where cont.valid = 'Y'  ");
        if (second_condition.Length > 0)
        {
            sql.AppendFormat(" {0}  ;", second_condition);
        }
        return (sql.ToString());
    }




    public string get_food_menu_ds(string condition )
    {
        //  SELECT* FROM azure_test.sys_client_food_menu;
        //     menu_id, menu_name, start_datetime, end_datetime, created_by, created_datetime, modified_by, modified_datetime, valid

        //SELECT* FROM azure_test.sys_client_food_menu_content;
        //            content_id, menu_id, breakfast, lunch, soup, fruit, afternoon_tea, dinner, midnight_snacks, ordering, created_by, created_datetime, modified_by, modified_datetime, valid
        //            jump_id, menu_id, menu_index, jump_date, valid, created_by, created_datetime
        //sys_client_food_menu_jump



        StringBuilder sql = new StringBuilder();
        sql.Append("select menu.menu_id,menu.menu_name, ");
        sql.Append("(DATEDIFF(date(curdate()), date(menu.start_datetime)) mod count(ct.content_id))+1,");

        sql.Append(" ((DATEDIFF(date(menu.end_datetime), date(b.jump_date) + ((count(ct.content_id) - b.menu_index)))  mod count(ct.content_id)))+1 as tt ");

        //sql.Append((need_photo == true ? ",doc.document_photo " : ""));

        sql.Append(" ");
        sql.Append(" ");


        sql.Append(" from sys_client_food_menu menu ");

        sql.Append(" LEFT JOIN sys_client_food_menu_content AS ct ON menu.menu_id = ct.menu_id and menu.valid = 'Y'  and ct.valid = 'Y' ");

        sql.Append("  Left join sys_client_food_menu_jump as b on menu.menu_id = b.menu_id and b.valid = 'Y' ");
        sql.Append(" left join sys_client_food_menu_jump as b2 on b.menu_id = b2.menu_id AND b2.valid = 'Y'  and b.jump_id < b2.jump_id  ");



        sql.Append("where  ct.valid = 'Y'   and b2.jump_id is null ");
        if (condition.Length > 0)
        {
            sql.Append(condition);
        }

        sql.Append(" order by ct.ordering ");




        //  sql.Append("select menu.menu_id, menu_name,DATE_FORMAT(start_datetime,  '%Y/%m/%d'),DATE_FORMAT(end_datetime,  '%Y/%m/%d'),count(cont.content_id),DATE_FORMAT(b.jump_date,  '%Y/%m/%d'), b.menu_index   ");
        //   sql.Append("from sys_client_food_menu menu ");
        //   sql.Append(" left join sys_client_food_menu_content as cont on cont.menu_id = menu.menu_id  and cont.valid = 'Y'   ");
        //   sql.Append(" Left join sys_client_food_menu_jump as b on menu.menu_id = b.menu_id and b.valid = 'Y'   ");
        //  sql.Append(" left join sys_client_food_menu_jump as b2 on b.menu_id = b2.menu_id AND b2.valid = 'Y'  and b.jump_id < b2.jump_id ");

        // sql.Append("  where menu.valid = 'Y'  and b2.jump_id is null ");


        return  (sql.ToString());
    }
 /*
    public string get_food_menu_content_brief(string condition)
    {
        //SELECT* FROM azure.sys_client_food_menu_content;
        // content_id, menu_id, breakfast, lunch, soup, fruit, afternoon_tea, dinner, midnight_snacks, ordering, created_by, created_datetime, modified_by, modified_datetime, valid

        StringBuilder sql = new StringBuilder();
        sql.Append("select content_id, menu_id ,ordering , ");
        sql.Append(" breakfast, lunch, soup, fruit, afternoon_tea, dinner, midnight_snacks ");
        sql.Append("from sys_client_food_menu_content  ");
        //sql.Append(" left join sys_client_food_menu_content as cont  ");
        sql.Append("  where valid = 'Y'  ");
        if (condition.Length > 0)
        {
            sql.AppendFormat(" {0}  ", condition);
        }
        sql.Append(" order by ordering  ");
        return  (sql.ToString());
    }
    */
    public DataTable reformat_food_menu_datatable(string id, string tablename, string[] first_row, string[] second_row)
    {
        DataTable tab = new DataTable();
        //  tablename = tablename;
        tab.TableName = tablename;
      //  tab.Columns.Add("Image", typeof(Image));

        tab.Columns.Add("類別");
        tab.Columns.Add("名稱");


        tab.Columns.Add("key");
        tab.Columns.Add("field");
        tab.Columns.Add("BackColor");
        for (int i = 0; i < first_row.Length; i++)
        {
         //   tab.Rows.Add(ResANIImage.task_icon_7, first_row[i], second_row[i], id, "MNU", Color.FromArgb(240, 253, 252).ToArgb());
        }

        //fibroGrid1.BackColorColumn = "BackColor";


        return tab;


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


        sql.Append("SELECT room.room_name,room_remark,bed_panel.bed_id,per.client_id,  CONCAT(per.chi_surname, per.chi_name), per.sex,");
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
        sql.Append("where room.display_id = '{0}' and room.valid='Y' order by rb.record_id  ;  ");
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




    public string display_update_version(string[] values)
    {
        //  SELECT* FROM azure.sys_display_panel_update_log;
        //   record_id, display_id, update_version, created_by, created_datetime
        //     panel.
         StringBuilder sql = new StringBuilder();
        sql.Append("insert sys_display_panel_update_log(display_id, update_version, created_by, created_datetime) ");
        sql.AppendFormat("values('{0}','{1}','{2}','{3}');", values);


 
        //sql.Append("from sys_display_panel where unique_device_id = '{0}' ;");
        string comm = string.Format(sql.ToString(), values);
        return comm;
    }










}

