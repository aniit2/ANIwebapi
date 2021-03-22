using ANI.code;
using System;
using System.Timers;
using System.Web.Services;
using System.Security.Cryptography;
using System.Diagnostics;
/// <summary>
/// Summary description for WebService2
/// </summary>

[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]


public class siusin : aniWebService
{
    string dbHost = "database-server-01.cyplnzjfey2i.ap-east-1.rds.amazonaws.com";
    string dbPort = "3306";
    string dbUser = "siusin";//資料庫使用者帳號
    string dbPass = "nfc11541";//資料庫使用者密碼
    string dbName = "siusin";//資料庫名稱
    string db_testName = "siusin_test";//資料庫名稱

    string LoginName = "100017";//資料庫名稱
    string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱

    public siusin()
    {

        basadata = new ANImoblie(true);
        basadata.dbHost = dbHost;
        basadata.dbPort = dbPort;
        basadata.dbUser = dbUser;
        basadata.dbPass = dbPass;
        basadata.dbName = dbName;
        basadata.LoginName = LoginName;
        basadata.LoginPass = LoginPass;
        basadata.db_testName = db_testName;
        basadata.refresh_connection();


    }
}

