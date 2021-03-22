using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlarmMonitorClient.code
{
    public class CClientSocket
    {
        public MsgBox ShowBox { set; get; }


        public bool EXIST = false;
        //服务器信息
        public ServerInfo serverInfo { set; get; }

        //连接状态信息对象
        StateObject state = new StateObject();

        //读取数量
        int ReadCount = 0;

        //客户端Socket
        Socket client;

        //按钮单击发送属性
        public object ButtonTag { set; get; }

        //返回按钮发送字节数组
        public byte[] GetButtonClickSendBytes(DataBranch br)
        {
            //获得按钮属性Tag 字节数组
            byte[] sb = System.Text.Encoding.UTF8.GetBytes(ButtonTag.ToString());
            //返回字节数组
            byte[] retSendBytes = new byte[sb.Length + 9];
            //获得指令头字节数组
            byte[] commBytes = br.GetByte(ReceCommandType.ButontClick);
            //复制命令头字节
            Buffer.BlockCopy(commBytes, 0, retSendBytes, 0, commBytes.Length);
            //复制按钮属性Tag
            Buffer.BlockCopy(sb, 0, retSendBytes, commBytes.Length, sb.Length);
            //添加校验码
            br.AddSumCode(retSendBytes, retSendBytes.Length);
            //返回数组
            return retSendBytes;
        }

        /// <summary>
        /// 循环发送指令，确保服务器、客户端接收到正确信息(检查校验码、重发指令)
        /// </summary>
        private void SendLoop(Socket client, byte[] sendBytes, DataBranch dr)
        {
            //设置读取字节数为零
            ReadCount = 0;
            //循环检测校验码,校验码不通过 或 接到重发指令 （重发指令）
            //ReadCount=0 确保能运行一次
            while (!dr.CheckSumCode(state.buffer, ReadCount) ||
                    dr.ComparisonByte(dr.GetByte(ReceCommandType.SendAgain), state.buffer))
            {
                //发送指令
                client.Send(sendBytes);
                //接收指令
                ReadCount = client.Receive(state.buffer);
            }
        }

        /// <summary>
        /// 得到登陸字節
        /// </summary>
        /// <param name="SqlDatatable"></param>
        /// <returns></returns>
        public byte[] GetLoginBytes(DataBranch dr)
        {
            byte[] usrname = System.Text.Encoding.UTF8.GetBytes(serverInfo.LoginName);
            byte[] password = System.Text.Encoding.UTF8.GetBytes(serverInfo.LoginPass);

            //返回字節數
            byte[] ret = new byte[13 + usrname.Length + password.Length];
            Buffer.BlockCopy(dr.GetByte(ReceCommandType.LoginByte), 0, ret, 0, 9);
            ret[9] = (byte)usrname.Length;
            Buffer.BlockCopy(usrname, 0, ret, 10, ret[9]);
            ret[10 + ret[9]] = (byte)password.Length;
            Buffer.BlockCopy(password, 0, ret, 11 + ret[9], ret[10 + ret[9]]);
            dr.AddSumCode(ret, ret.Length);
            return ret;
        }
        /// <summary>
        /// 开启客户端连接
        /// </summary>
        public bool StartConnect()
        {
            bool exist = false;
            try
            {

                //设置IP 端口
                IPAddress ipAddress = IPAddress.Parse(serverInfo.IpAddress);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, serverInfo.IpPort);// 11880
                // 建立 TCP/IP Socket     
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                DataBranch dr = new DataBranch();
                //开启连接到远程计算机     
                client.Connect(remoteEP);

                //获得客户端连接
                state.workSocket = client;

                //发送登陆字节到远程
                SendLoop(client, GetLoginBytes(dr), dr);

                //发送单击按钮事件
                SendLoop(client, GetButtonClickSendBytes(dr), dr);

                // if (ShowBox != null)
                //    ShowBox(state.buffer[2] == 1 ? true : false);
                exist = state.buffer[2] == 1 ? true : false;
                //   EXIST = state.buffer[2] == 1 ? true : false;
                //关闭连接
                SendLoop(client, dr.GetByte(ReceCommandType.ExitServer), dr);

                client.Shutdown(SocketShutdown.Both);
                client.Close();
                return exist;
            }
            catch
            {
                return exist;
            }
        }



    }
}
