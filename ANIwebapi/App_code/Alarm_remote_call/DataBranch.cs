using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmMonitorClient.code
{
    public class DataBranch
    {
        //登陆指令  (无参数指令，指令'0xE5'是校验码)
        byte[] LoginByte = new byte[] { 0x68, 0x62, 0x34, 0x46, 0x78, 0x39, 0xF0, 0xE5, 0x86 };

        //完成指令
        byte[] sqlCompleted = new byte[] { 0x68, 0xA1, 0xA1, 0xA1, 0xFF, 0x4A, 0x86 };

        //重新发送指令
        byte[] SendAgain = new byte[] { 0x68, 0xF1, 0xF1, 0xF1, 0xFF, 0x4A, 0x86 };
        //退出服务指令
        byte[] ExitServer = new byte[] { 0x68, 0xD1, 0xD1, 0xD1, 0xDF, 0xBA, 0x86 };


        //按钮单击事件
        byte[] ButtonClick = new byte[] { 0x68, 0x21, 0x01, 0x01, 0xF0, 0x7B, 0x86 };


        /// <summary>
        /// 得到发送字节
        /// </summary>
        /// <param name="cType"></param>
        /// <returns></returns>
        public byte[] GetByte(ReceCommandType cType)
        {
            switch (cType)
            {
                case ReceCommandType.Completed:
                    return sqlCompleted;

                case ReceCommandType.LoginByte:
                    return LoginByte;

                case ReceCommandType.SendAgain:
                    return SendAgain;

                case ReceCommandType.ExitServer:
                    return ExitServer;

                case ReceCommandType.ButontClick:
                        return ButtonClick;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 检测登陆信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CheckLoginCommand(byte[] data)
        {
            bool ret = false;
            if (data.Length > LoginByte.Length)
            {
                ret = true;
                for (int i = 0; i < LoginByte.Length && i < data.Length; i++)
                    if (data[i] != LoginByte[i])
                        ret = false;
            }
            return ret;
        }

        
        /// <summary>
        /// 检测指令字节
        /// </summary>
        /// <param name="data"></param>
        /// <param name="CommandByte"></param>
        /// <returns></returns>
        private bool CheckCommandByte(byte[] DataByte, byte[] CommandByte)
        {
            bool ret = true;
            for (int i = 0; i < CommandByte.Length; i++)
                if (DataByte[i] != CommandByte[i])
                    ret = false;
            return ret;
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
        /// <returns></returns>
        public bool ComparisonByte(byte[] data,byte[] destBytes)
        {
            bool ret = true;
            for (int i = 0; i < data.Length; i++)
                if (data[i] != destBytes[i])
                    ret = false;
            return ret;
        }
    }
}
