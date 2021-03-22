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


public class altru1 : aniWebService
{
    string dbHost = "192.168.1.17";
    string dbPort = "3366";
    string dbUser = "altru1";//資料庫使用者帳號
    string dbPass = "nfc11545";//資料庫使用者密碼
    string dbName = "altru1";//資料庫名稱
    string db_testName = "altru1_test";//資料庫名稱

    string LoginName = "100023";//資料庫名稱
    string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱

    public altru1()
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
        Debug.Print("refred");
    }


}

