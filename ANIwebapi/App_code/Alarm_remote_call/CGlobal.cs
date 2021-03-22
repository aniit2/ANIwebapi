using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlarmMonitorClient.code
{

    public delegate bool MsgBox(Boolean bol);

    // 客户端Socket状态信息    
    public class StateObject
    {
        //客户端socket.     
        public Socket workSocket = null;
        // 最大接收字节数
        public const int BufferSize = 1024;
        // 接收数组.     
        public byte[] buffer = new byte[BufferSize];
        //登陆状态
        public bool LoginState = false;
        //最大登陆检测次数
        public byte LoginMaxCount = 3;
    }

    public class ServerInfo
    {
        public ServerInfo(string _Ip, int _port, string username, string password) {
            IpAddress = _Ip;
            IpPort = _port;
            LoginName = username;
            LoginPass = password;
        }
        //IP地址
        public string IpAddress { set; get; }
        //Ip端口
        public int IpPort { set; get; }

        public string LoginName { set; get; }

        public string LoginPass { set; get; }

    }

    /// <summary>
    /// 命令類型
    /// </summary>
    public enum ReceCommandType
    {
        SendAgain = 0,        //重發指令                      (服務端發送)
        Completed,          //完成指令                      (服務端發送)
        ExitServer,         //退出指令                      (客戶端發送)
        ButontClick,
        LoginByte,
        ErrorCommand

    }
}
