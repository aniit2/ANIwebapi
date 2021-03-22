using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidServerClient
{

    public class CallBackInfo
    {
        //回调IP
        public string callIP { set; get; }
        //回调端口
        public int callPort { set; get; }
        //客户ID
        public byte[] clientID { set; get; }
        //更新数据间隔时间
        public int browerTime { set; get; }
        //运行时间
        public int runTime { set; get; }

        public bool isConnection { set; get; }

        public bool isExec { set; get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_callIP"></param>
        /// <param name="_callPort"></param>
        /// <param name="_clientID"></param>
        /// <param name="_browerTime"></param>
        public CallBackInfo(string _callIP, int _callPort, string _clientID, int _browerTime)
        {
            //赋值IP地址
            callIP = _callIP;
            //赋值IP端口

            callPort = _callPort;
            //赋值客户ID
            clientID = System.Text.Encoding.ASCII.GetBytes(_clientID);
            //赋值浏览时间
            browerTime = _browerTime;
            //运行时间（计时用）
            runTime = 0;

            isConnection = true;
        }
    }



    public class ExecCommand
    {
        //客户ID
        public byte[] clientID { set; get; }
        //执行命令
        public byte[] command { set; get; }
        //回调IP
        public string callIP { set; get; }
        //回调端口
        public string callIP6 { set; get; }
        //回调端口
        public int callPort { set; get; }

        public int index { set; get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_clientID"></param>
        /// <param name="_command"></param>
        public ExecCommand(string ip, int port, byte[] _clientID, byte[] _command)
        {
            clientID = _clientID;
            callIP = ip;
            callPort = port;
            command = _command;
        }
    }

    public class execIpInfo
    {
        public string IP { set; get; }
        //回调端口
        public int Port { set; get; }

        public bool isConnection { set; get; }

        public bool isEnable { set; get; }

        public execIpInfo(string _ip, int _port, bool _Connection, bool _enable)
        {
            IP = _ip;
            Port = _port;
            isConnection = _Connection;
            isEnable = _enable;
        }
    }

    public delegate void msgToForm(execIpInfo execip);
}
