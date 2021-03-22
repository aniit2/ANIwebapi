using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidServerClient
{
    class CCallBackThread
    {
        //定义CallBack信息集合
        List<CallBackInfo> calls = new List<CallBackInfo>();
        //发送完成命令
        static byte[] cmdCompleted = new byte[] { 0x68, 0xA1, 0xA1, 0xA1, 0xFF, 0x4A, 0x86 };

        static byte[] LoginByte = new byte[] { 0x68, 0x62, 0x34, 0x46, 0x78, 0x39, 0xF0, 0xE5, 0x86 };
        //定义互斥鎖
        Mutex WriteMutex = new Mutex();
        Mutex CallMutex = new Mutex();

        //定义线程信号量.     
        ManualResetEvent callDone = new ManualResetEvent(false);

        //定义CallBack线程
        Thread callBackThread;

        //执行命令列表
        List<ExecCommand> execCmds = new List<ExecCommand>();

        //自动检测执行计时器
        Timer AutoRun;

        msgToForm msg;

        public msgToForm msgTo { set { msg = value; } get { return msg; } }

        //停止线程标记
        bool isStopThread = false;

        public CCallBackThread()
        {
            //设置线程池最小工作程序线程数
            ThreadPool.SetMinThreads(1, 1);
            //设置线程池中辅助线程的最大数目。
            ThreadPool.SetMaxThreads(200, 200);
            //执行线程
            callBackThread = new Thread(new ThreadStart(ThreadMethod));
            callBackThread.IsBackground = true;
            callBackThread.Start();

            //AutoRun = new Timer(new TimerCallback(checkAutoRun), null, 2000, 2000);
        }
        public int IpStringToInt(String ipAddress)
        {
            int ipInt = 0;
            //将IP地址转换数组
            String[] IPArr = ipAddress.Split('.');

            ipInt += (int.Parse(IPArr[0]) & 0xFF) << 24;
            ipInt += (int.Parse(IPArr[1]) & 0xFF) << 16;

            ipInt += (int.Parse(IPArr[2]) & 0xFF) << 8;

            ipInt += int.Parse(IPArr[3]) & 0xFF;

            return ipInt;
        }

        /// <summary>
        /// 自动检测是否浏览时间
        /// </summary>
        /// <param name="a"></param>
        public void checkAutoRun(object a)
        {
            //  申請互斥鎖
            WriteMutex.WaitOne();
            foreach (CallBackInfo c in calls)
            {
                if (c.browerTime != 0)
                {
                    c.runTime = (c.runTime + 1) % c.browerTime;
                    if (c.runTime == c.browerTime - 1 && c.isConnection)
                    {
                        //添加  视图右移 指令
                        InsertExecCommand(new ExecCommand(c.callIP, c.callPort, c.clientID, new byte[] { 0x01, 0x33 }));
                    }
                }
            }
            // 释放互斥鎖
            WriteMutex.ReleaseMutex();
            //开始执行
            if (execCmds.Count > 0) RunCallBack();
        }

        /// <summary>
        /// 激活信息量,运行CallBack线程
        /// </summary>
        public void RunCallBack()
        {
            callDone.Set();
        }

        /// <summary>
        /// 停止CallBack线程
        /// </summary>
        public void StopThread()
        {
            isStopThread = true;
            callDone.Set();
        }
        /// <summary>
        /// 线程函数
        /// </summary>
        /// <param name="paramer"></param>
        public void ThreadMethod()
        {
            while (!isStopThread)
            {
                // 设置为非终止状态,阻止线程运行
                callDone.Reset();
                //阻止当前线程，直到接收信号    
                callDone.WaitOne();
                // 申請互斥鎖
                WriteMutex.WaitOne();
                CallMutex.WaitOne();
                for (int i = 0; i < calls.Count; i++)
                {
                    for (int j = execCmds.Count - 1; j > -1; j--)
                    {
                        //检测客户ID是否相同
                        if (ComparisonByte(execCmds[j].clientID, calls[i].clientID))
                        {
                            if (calls[i].isConnection && !calls[i].isExec)
                            {
                                //赋值CallBack IP地址 Ip端口
                                execCmds[j].callIP = calls[i].callIP;
                                execCmds[j].callPort = calls[i].callPort;
                                execCmds[j].index = i;
                                calls[i].isExec = true;
                                //执行线程池线程
                                new Thread(new ParameterizedThreadStart(CallBackConnection)).Start(execCmds[j]);
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(CallBackConnection), execCmds[j]);
                            }
                            execCmds.RemoveAt(j);
                        }
                    }
                }
                // 释放互斥鎖
                CallMutex.ReleaseMutex();
                WriteMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// CallBack连接
        /// </summary>
        /// <param name="obj"></param>
        public void CallBackConnection(object obj)
        {
            bool isSend = false;
            ExecCommand c = (ExecCommand)obj;
            byte[] buffer = new byte[1024];
            //设置连接服务器IP地址，Ip端口
            IPAddress ipAddress = IPAddress.Parse(c.callIP);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, c.callPort);
            //定义连接Socket
            Socket workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                workSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);

                //开始连接到远程计算机     
                workSocket.Connect(remoteEP);

                //发送登陆字节+命令指令
                LoopSend(workSocket, buffer, getSendBytes(c.command));

                //禁用Socket接收发送
                workSocket.Shutdown(SocketShutdown.Both);

                //关闭客户端Socket
                workSocket.Close();
            }
            catch (SocketException e)
            {
                if (workSocket != null && workSocket.Connected)
                {
                    //禁用Socket接收发送
                    workSocket.Shutdown(SocketShutdown.Both);
                }
                //关闭客户端Socket
                workSocket.Close();
                calls[c.index].isConnection = false;
                if (msgTo != null) msgTo(new execIpInfo(calls[c.index].callIP, calls[c.index].callPort, false, false));
                isSend = true;
            }
            if (!isSend && msgTo != null) msgTo(new execIpInfo(calls[c.index].callIP, calls[c.index].callPort, true, true));
            calls[c.index].isExec = false;
        }

        private byte[] getSendBytes(byte[] commandbytes)
        {
            byte[] ret = new byte[11 + commandbytes.Length];
            Buffer.BlockCopy(LoginByte, 0, ret, 0, 9);
            Buffer.BlockCopy(commandbytes, 0, ret, 9, commandbytes.Length);
            AddSumCode(ret, ret.Length);
            return ret;
        }
        /// <summary>
        /// 循环发送指令，确保服务器、客户端接收到正确信息(检查校验码、重发指令)
        /// </summary>
        /// <param name="sendBytes">發送的字節數組</param>
        /// <param name="ReceCommandBytes">接收的命令字節</param>
        /// <returns></returns>
        private void LoopSend(Socket workSocket, byte[] buffer, byte[] sendBytes)
        {
            int SendSum = 0;
            //最多循環5次發送數據到服務器。
            while (SendSum < 5)
            {
                //发送指令
                workSocket.Send(sendBytes);

                //接收返回信息
                int ReadCount = workSocket.Receive(buffer);

                //檢測接收校驗碼
                if (CheckSumCode(buffer, ReadCount))
                {
                    //完成指令發送
                    if (ComparisonByte(buffer, cmdCompleted))
                        return;
                }
                SendSum++;
            }
        }
        /// <summary>
        /// 检测和校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CheckSumCode(byte[] data, int dataLen)
        {
            int val = 0;
            for (int i = 0; i < dataLen - 2; i++)
                val = (val + data[i]) & 0xff;
            return dataLen > 2 ? val == data[dataLen - 2] : false;
        }
        /// <summary>
        /// 添加和校验
        /// </summary>
        /// <param name="data"></param>
        public void AddSumCode(byte[] data, int len)
        {
            int val = 0;
            for (int i = 0; i < len - 2; i++)
                val = (val + data[i]) & 0xff;
            data[len - 2] = (byte)val;
            data[data.Length - 1] = 0x86;
        }

        /// <summary>
        /// 比较字节
        /// </summary>
        /// <param name="srcdata">源字节</param>
        /// <param name="decData">目标字节</param>
        /// <returns></returns>
        public bool ComparisonByte(byte[] srcdata, byte[] decData)
        {
            bool ret = true;
            //如果目標字節長度小于比較字節長度，直接返回False;
            if (decData.Length > srcdata.Length) return false;
            //循環比較目標字節
            for (int i = 0; i < decData.Length; i++)
                if (srcdata[i] != decData[i])
                    ret = false;
            return ret;
        }


        /// <summary>
        /// 插入Call Back信息
        /// </summary>
        public void InsertCallBack(CallBackInfo backInfo)
        {
            // 申請互斥鎖
            WriteMutex.WaitOne();
            bool isExists = false;
            //删除已经存在的客户ID
            for (int i = calls.Count - 1; i >= 0; i--)
            {
                if (ComparisonByte(calls[i].clientID, backInfo.clientID))
                {
                    isExists = true;
                    calls[i].callIP = backInfo.callIP;
                    calls[i].runTime = backInfo.runTime;
                    calls[i].callPort = backInfo.callPort;
                    calls[i].isExec = false;
                    calls[i].isConnection = true;
                }
            }
            //添加Call Back
            if (!isExists) calls.Add(backInfo);
            // 释放互斥鎖
            WriteMutex.ReleaseMutex();
        }

        /// <summary>
        /// 移除Call Back信息
        /// </summary>
        public void RemoveCallBack(CallBackInfo backInfo)
        {
            // 申請互斥鎖
            WriteMutex.WaitOne();

            //移除Call Back
            calls.Remove(backInfo);

            // 释放互斥鎖
            WriteMutex.ReleaseMutex();
        }
        /// <summary>
        /// 得到所有Call Back客户ID
        /// </summary>
        /// <returns></returns>
        public byte[][] GetCallClientIDs()
        {
            // 申請互斥鎖
            WriteMutex.WaitOne();
            byte[][] ret = new byte[calls.Count][];

            //循环遍历所有CallBack信息
            for (int i = 0; i < calls.Count; i++)
                ret[i] = calls[i].clientID;

            // 释放互斥鎖
            WriteMutex.ReleaseMutex();
            return ret;
        }

        /// <summary>
        /// 插入执行命令
        /// </summary>
        public void InsertExecCommand(ExecCommand execInfo)
        {
            // 申請互斥鎖
            CallMutex.WaitOne();
            //添加Call Back
            execCmds.Add(execInfo);
            // 释放互斥鎖
            CallMutex.ReleaseMutex();
        }



        /// <summary>
        /// 移除执行命令
        /// </summary>
        public void RemoveExecCommand(ExecCommand execInfo)
        {
            // 申請互斥鎖
            CallMutex.WaitOne();
            //移除Call Back
            execCmds.Remove(execInfo);
            // 释放互斥鎖
            CallMutex.ReleaseMutex();
        }

    }
}
