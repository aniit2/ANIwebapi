using Ani_Test_WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace ANIwebapi.Controllers
{
    public class UserImageController : ApiController
    {
        [HttpGet]
        public ClientImageInfo GetUserImageByID(string PhotoID)
        {
            CExecSql sql = new CExecSql();
            ClientImageInfo ret = new ClientImageInfo();
            ret.PhotoData = sql.GetImage(PhotoID);
            ret.PhotoID = PhotoID;
            return ret;
        }

        [HttpPost]
        public Boolean PostImageByID(String PhotoID, String PhotoBytes)
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];

            CExecSql sql = new CExecSql();
            sql.SaveImage(PhotoID, Convert.FromBase64String(PhotoBytes));
            return true;
        }

        [HttpPost]
        public String PostImageByID()
        {
            var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            using (var sr = new StreamReader(context.Request.InputStream, Encoding.UTF8, true, 1024, true))
            {

                string[] bodyValues = sr.ReadToEnd().Split(';');
                if (bodyValues[0] == "admin" && bodyValues[1] == "123456"){
                    CExecSql sql = new CExecSql();
         
                    sql.SaveImage(bodyValues[2], Convert.FromBase64String(bodyValues[3]));
                }
            }
     
            return "true";
        }
    }
}