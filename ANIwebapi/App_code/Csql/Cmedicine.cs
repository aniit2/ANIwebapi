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
internal class Cmedicine  
{

    public static string get_medicine_photo(string medicine_photo_id)
    {

        StringBuilder sql = new StringBuilder();

        sql.Append("select document_photo from med_medicine_photo Where medicine_photo_id ='{0}' and valid = 'Y';");
        string comm = string.Format(sql.ToString(), medicine_photo_id);

        return comm;


    }

    public static string get_medicine_bag_photo(string medicine_photo_bag_id)
    {

        StringBuilder sql = new StringBuilder();

        sql.Append("select document_photo from med_medicine_bag_record Where medicine_bag_id ='{0}' and valid = 'Y';");

        string comm = string.Format(sql.ToString(), medicine_photo_bag_id);

        return comm;


    }
    public static string get_medicine_box_content(string medicine_box_id)
    {
        StringBuilder sql = new StringBuilder();
        /*
        String cmdText0 = "select * from med_medicine_box Where med_medicine_box_id IN (@id)";


    
        take_medicine_id
        sql.Append("CONCAT_WS(';', abi.self_care_ability, abi.movement_ability, abi.eating_ability,");
        sql.Append("select * from med_medicine_box Where med_medicine_box_id ='{0}' and valid = 'Y';");
        */
        string comm = string.Format(sql.ToString(), medicine_box_id);

        return comm;


    }

    public static string get_medicine_records(string client_id)
    {
        StringBuilder sql = new StringBuilder();


        sql.Append("SELECT  c.medicine_id,c.medicine_name_eng,take_medicine_id, ");
        sql.Append("");
        sql.Append("  concat(a.dose_amount, ' ', a.dose_unit), ");
        sql.Append(" a.volume_amount,   a.volume_unit , ");
        sql.Append(" peri.medicine_period_code  , ");
        //sql.Append("ifnull(CONCAT(a.medicine_period_id, '(', cast(tt.bb as char), ')'), a.medicine_period_id), ");
        sql.Append("concat(GROUP_CONCAT(DISTINCT DATE_FORMAT(inter.take_interval_time, '%H:%i') ");
        sql.Append("ORDER BY inter.ordering ASC SEPARATOR ';' )), ");

        sql.Append("concat(if (a.prn = 'Y','PRN ',''),a.each_take_type),a.every_n_days,a.take_method_code,f.medicine_report_type, ");
        sql.Append("concat(a.take_remark,' ',a.remark),");

        // sql.Append("DATE_FORMAT(a.take_begin_date, '%Y-%m-%d'),IF(a.take_prescription_date > '1990-01-01', ");
        //. sql.Append("DATE_FORMAT(a.take_prescription_date, '%Y-%m-%d'), ''), ");
        //  sql.Append("IF(a.take_end_date > '1990-01-01', DATE_FORMAT(a.take_end_date, '%Y-%m-%d'), ''), ");
        //   sql.Append("g.addr_org_chi_name,a.specialties_code,a.prn, ");
        //   sql.Append("a.script_medicine, ");
        //  sql.Append("if((a.take_end_date > '1910-05-16' and date(now()) < a.take_end_date)or(date(a.take_end_date) = '1900-05-16' ),'Y','N'), ");
        //  sql.Append("a.client_id,h.active_status,a.medicine_id,DATE_FORMAT(a.bring_date, '%Y-%m-%d') ,  ");
        sql.Append(" c.medicine_photo_id,  a.medicine_bag_id, a.first_check_id ,a.second_check_id ");

        sql.Append("FROM med_take_medicine2 AS a LEFT JOIN med_medicine2 AS c ON a.medicine_id = c.medicine_id ");

        sql.Append("LEFT JOIN med_medicine_report_type AS f ON a.medicine_report_type_id = f.medicine_report_type_id ");
        sql.Append("LEFT JOIN company_address_book AS g ON a.medicine_source = g.addr_id ");
        sql.Append("LEFT JOIN client_personal2 AS h ON a.client_id = h.client_id ");

        sql.Append("LEFT JOIN med_take_period peri ON peri.medicine_period_id = a.medicine_period_id and peri.active_status = 'Y' ");
        sql.Append("LEFT JOIN med_take_interval2 inter ON peri.medicine_period_id = inter.medicine_period_id and inter.valid = 'Y' ");



        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = h.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id ");

        sql.Append("where a.valid  = 'Y' and ((a.take_end_date > '1910-05-16' and date(now()) <= a.take_end_date) or date(a.take_end_date) = '1900-05-16' ) ");
        sql.Append(" and a.client_id = {0} " );


        sql.Append("  group by a.take_medicine_id ");
 

        return string.Format(sql.ToString(), client_id);
    }
    public static string get_medicine_description(string take_medicine_id)
    {
        StringBuilder sql = new StringBuilder();

        sql.Append("SELECT a.medicine_id,c.medicine_name_eng,take_medicine_id, ");
        sql.Append("DATE_FORMAT(a.take_begin_date, '%Y-%m-%d'),IF(a.take_prescription_date > '1990-01-01', ");
        sql.Append("DATE_FORMAT(a.take_prescription_date, '%Y-%m-%d'), ''), ");
        sql.Append("  concat(a.dose_amount, ' ', a.dose_unit), ");
        sql.Append(" a.volume_amount,   a.volume_unit , ");
        sql.Append(" peri.medicine_period_code  , ");
        //sql.Append("ifnull(CONCAT(a.medicine_period_id, '(', cast(tt.bb as char), ')'), a.medicine_period_id), ");
        sql.Append("concat(GROUP_CONCAT(DISTINCT DATE_FORMAT(inter.take_interval_time, '%H:%i') ");
        sql.Append("ORDER BY inter.ordering ASC SEPARATOR ';' )), ");
        sql.Append(" if (a.prn = 'Y','PRN ',''),a.each_take_type,a.take_method_code,f.medicine_report_type, ");
        sql.Append("concat(a.take_remark,' ',a.remark),");
 

        sql.Append("IF(a.take_end_date > '1990-01-01', DATE_FORMAT(a.take_end_date, '%Y-%m-%d'), ''), ");
        sql.Append("g.addr_org_chi_name,a.specialties_code,a.prn, ");

        sql.Append("a.script_medicine, ");
        sql.Append("if((a.take_end_date > '1910-05-16' and date(now()) < a.take_end_date)or(date(a.take_end_date) = '1900-05-16' ),'Y','N'), ");
        sql.Append("a.every_n_days,a.client_id,h.active_status,a.medicine_id,DATE_FORMAT(a.bring_date, '%Y-%m-%d'),  ");
        sql.Append(" c.medicine_photo_id,  a.medicine_bag_id, a.first_check_id  ,a.second_check_id  ");


        sql.Append("FROM med_take_medicine2 AS a LEFT JOIN med_medicine2 AS c ON a.medicine_id = c.medicine_id ");

        sql.Append("LEFT JOIN med_medicine_report_type AS f ON a.medicine_report_type_id = f.medicine_report_type_id ");
        sql.Append("LEFT JOIN company_address_book AS g ON a.medicine_source = g.addr_id ");
        sql.Append("LEFT JOIN client_personal2 AS h ON a.client_id = h.client_id ");

        sql.Append("LEFT JOIN med_take_period peri ON peri.medicine_period_id = a.medicine_period_id and peri.active_status = 'Y' ");
        sql.Append("LEFT JOIN med_take_interval2 inter ON peri.medicine_period_id = inter.medicine_period_id and inter.valid = 'Y' ");



        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = h.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id ");

        sql.AppendFormat("where a.take_medicine_id = '{0}' ",take_medicine_id);


        sql.Append("  group by a.take_medicine_id ");
        sql.Append(" order by client_id, h.active_status asc, CONVERT(k.tchi_value, UNSIGNED) asc ,k.tchi_value asc, ");
        sql.Append(" CONVERT(z.tchi_value, UNSIGNED) asc, z.tchi_value asc, ");
        sql.Append(" CONVERT(b.tchi_value, UNSIGNED) asc, b.tchi_value asc, a.prn DESC; ");

 

        return string.Format(sql.ToString(), take_medicine_id);
    }

    public static string get_medicine_take_records(string client_id)
    {
        StringBuilder sql = new StringBuilder();


        sql.Append("SELECT  c.medicine_id,c.medicine_name_eng,a.take_medicine_id, ");
        sql.Append("");
        sql.Append("  concat(a.dose_amount, ' ', a.dose_unit), ");
        sql.Append(" a.volume_amount,   a.volume_unit , ");
        sql.Append(" peri.medicine_period_code  , ");
        //sql.Append("ifnull(CONCAT(a.medicine_period_id, '(', cast(tt.bb as char), ')'), a.medicine_period_id), ");
        sql.Append("concat(GROUP_CONCAT(DISTINCT DATE_FORMAT(inter.take_interval_time, '%H:%i') ");
        sql.Append("ORDER BY inter.ordering ASC SEPARATOR ';' )), ");

        sql.Append("if (a.prn = 'Y','PRN ',''),concat(a.each_take_type),a.every_n_days,a.take_method_code,a.medicine_report_type_id,f.medicine_report_type, ");
        sql.Append("concat(a.take_remark,' ',a.remark),");

        // sql.Append("DATE_FORMAT(a.take_begin_date, '%Y-%m-%d'),IF(a.take_prescription_date > '1990-01-01', ");
        //. sql.Append("DATE_FORMAT(a.take_prescription_date, '%Y-%m-%d'), ''), ");
        //  sql.Append("IF(a.take_end_date > '1990-01-01', DATE_FORMAT(a.take_end_date, '%Y-%m-%d'), ''), ");
         sql.Append("g.addr_org_chi_name,a.specialties_code,DATE_FORMAT(refill.refill_date, '%Y-%m-%d'), ");
        //   sql.Append("a.script_medicine, ");
        //  sql.Append("if((a.take_end_date > '1910-05-16' and date(now()) < a.take_end_date)or(date(a.take_end_date) = '1900-05-16' ),'Y','N'), ");
        //  sql.Append("a.client_id,h.active_status,a.medicine_id,DATE_FORMAT(a.bring_date, '%Y-%m-%d') ,  ");
        sql.Append(" c.medicine_photo_id,  a.medicine_bag_id, a.first_check_id ,a.second_check_id ");

        sql.Append("FROM med_take_medicine2 AS a LEFT JOIN med_medicine2 AS c ON a.medicine_id = c.medicine_id ");

        sql.Append("LEFT JOIN med_medicine_report_type AS f ON a.medicine_report_type_id = f.medicine_report_type_id ");
        sql.Append("LEFT JOIN company_address_book AS g ON a.medicine_source = g.addr_id ");
        sql.Append("LEFT JOIN client_personal2 AS h ON a.client_id = h.client_id ");

        sql.Append("LEFT JOIN med_take_period peri ON peri.medicine_period_id = a.medicine_period_id and peri.active_status = 'Y' ");
        sql.Append("LEFT JOIN med_take_interval2 inter ON peri.medicine_period_id = inter.medicine_period_id and inter.valid = 'Y' ");

        sql.Append("LEFT JOIN med_take_medicine_refill refill ON refill.take_medicine_id = a.take_medicine_id and refill.valid = 'Y' ");
        sql.Append("LEFT JOIN med_take_medicine_refill refill2 ON refill2.take_medicine_id = refill.take_medicine_id and refill2.valid = 'Y' and refill2.take_medicine_refill_id > refill.take_medicine_refill_id ");

        sql.Append("LEFT JOIN sys_company_bed b  on b.client_id = h.client_id ");
        sql.Append("left join sys_company_zone z on b.zone_id = z.zone_id ");
        sql.Append("left join sys_company_block k on k.block_id = z.block_id ");

        sql.Append("where a.valid  = 'Y' and ((a.take_end_date > '1910-05-16' and date(now()) <= a.take_end_date) or date(a.take_end_date) = '1900-05-16' ) ");
        sql.Append(" and a.client_id = {0}  and refill2.take_medicine_refill_id is null ");


        sql.Append("  group by a.take_medicine_id ;");
        sql.Append(" ");

        return string.Format(sql.ToString(), client_id);
    }
 
}

