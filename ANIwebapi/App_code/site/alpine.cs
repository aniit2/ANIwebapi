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
[System.ComponentModel.ToolboxItem(false)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]

public class alpine : aniWebService
{
    string dbHost = "192.168.0.10";
    string dbPort = "3366";
    string dbUser = "alpine";//資料庫使用者帳號
    string dbPass = "nfc11573";//資料庫使用者密碼
    string dbName = "alpine";//資料庫名稱
    string db_testName = "alpine_test";//資料庫名稱

    string LoginName = "100053";//資料庫名稱
    string LoginPass = "e2b528ec109af1ae3b530d49aeb149e9";//資料庫名稱

    public alpine()
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



        panels = new ANI_Display_panel(true);
        have_panel = basadata.panel_exist= true;
        panels.dbHost = dbHost;
        panels.dbPort = dbPort;
        panels.dbUser = dbUser;
        panels.dbPass = dbPass;
        panels.dbName = dbName;
        panels.LoginName = LoginName;
        panels.LoginPass = LoginPass;
        panels.db_testName = db_testName;
        panels.refresh_connection();





 
    }

  
}

