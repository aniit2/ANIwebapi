using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ani_Test_WebApi.Models
{
    public interface IBlockCipher
    {
        // Methods
        int BlockSizeInBytes();
        void Cipher(byte[] inb, byte[] outb);
        void InitCipher(byte[] key);
        void InvCipher(byte[] inb, byte[] outb);
        int[] KeySizesInBytes();
    }

    public class StreamCipher
    {
        // Fields
        public static readonly bool ENCRYPT;
        public static readonly int KeyBits128;
        public static readonly int KeyBits192;
        public static readonly int KeyBits256;

        static StreamCipher()
        {
            ENCRYPT = true;
            DECRYPT = false;
        }

        public static void Encrypt(StreamCtx ctx, Stream instr, Stream outstr)
        {
            if (instr.Length > 0L)
            {
                byte[] inb = new byte[ctx.BlockSize()];
                byte[] iv = new byte[ctx.BlockSize()];
                bool flag = false;
                int i = 0;
                while (true)
                {
                    i = instr.Read(inb, 0, ctx.BlockSize());
                    if (i < 0)
                    {
                        i = 0;
                    }
                    if ((i > 0) && !flag)
                    {
                        flag = true;
                        if (ctx.Mode == Mode.CBC)
                        {
                            Array.Copy(ctx.IV, 0, iv, 0, iv.Length);
                        }
                    }
                    if ((i <= 0) || (i < inb.Length))
                    {
                        PrepareLastBuffer(inb, i);
                        PrepareBuffer(ctx.Mode, inb, iv);
                        ctx.Ibc.Cipher(inb, iv);
                        outstr.Write(iv, 0, iv.Length);
                        break;
                    }
                    PrepareBuffer(ctx.Mode, inb, iv);
                    ctx.Ibc.Cipher(inb, iv);
                    outstr.Write(iv, 0, iv.Length);
                }
                outstr.Flush();
            }
        }
        private static void PrepareBuffer(Mode mode, byte[] inb, byte[] iv)
        {
            if ((mode != Mode.ECB) && (mode == Mode.CBC))
            {
                for (int i = 0; i < inb.Length; i++)
                {
                    inb[i] ^= iv[i];
                }
            }
        }
        private static void PrepareLastBuffer(byte[] inb, int i)
        {
            byte num = (byte)(inb.Length - i);
            for (int j = i; j < inb.Length; j++)
            {
                inb[j] = num;
            }
        }


        public static void Decrypt(StreamCtx ctx, Stream instr, Stream outstr)
        {
            if (instr.Length > 0L)
            {
                byte[] inb = new byte[ctx.BlockSize()];
                byte[] outb = new byte[ctx.BlockSize()];
                int num = 0;
                bool flag = false;
                byte[] iv = new byte[ctx.BlockSize()];
                bool flag2 = false;
                byte[] buffer4 = new byte[ctx.BlockSize()];
                while (true)
                {
                    num = instr.Read(inb, 0, inb.Length);
                    if (num < 0)
                    {
                        num = 0;
                    }
                    if ((num > 0) && (num != inb.Length))
                    {
                        return;
                    }
                    if ((num > 0) && !flag)
                    {
                        flag = true;
                        if (ctx.Mode == Mode.CBC)
                        {
                            Array.Copy(ctx.IV, 0, iv, 0, ctx.IV.Length);
                        }
                    }
                    if (num <= 0)
                    {
                        if (!flag2)
                        {
                            return;
                        }
                        Array.Copy(buffer4, outb, outb.Length);
                        int num2 = outb[outb.Length - 1];
                        if (num2 > outb.Length)
                        {
                            return;
                        }
                        outstr.Write(outb, 0, outb.Length - num2);
                        break;
                    }
                    ctx.Ibc.InvCipher(inb, outb);
                    if (ctx.Mode == Mode.CBC)
                    {
                        PrepareBuffer(ctx.Mode, outb, iv);
                        Array.Copy(inb, iv, inb.Length);
                    }
                    if (!flag2)
                    {
                        Array.Copy(outb, buffer4, outb.Length);
                        flag2 = true;
                    }
                    else
                    {
                        outstr.Write(buffer4, 0, buffer4.Length);
                        Array.Copy(outb, buffer4, outb.Length);
                    }
                }
                outstr.Flush();
            }
        }


        public static byte[] Encode(StreamCtx ctx, byte[] data, bool encrypt)
        {
            MemoryStream instr = null;
            MemoryStream outstr = null;
            byte[] buffer = null;
            try
            {
                instr = new MemoryStream(data, false);
                outstr = new MemoryStream((data.Length - (data.Length % ctx.BlockSize())) + ctx.BlockSize());
                if (encrypt)
                {
                    Encrypt(ctx, instr, outstr);
                }
                else
                {
                    Decrypt(ctx, instr, outstr);
                }
                outstr.Seek(0L, 0);
                buffer = new byte[outstr.Length];
                outstr.Read(buffer, 0, buffer.Length);
            }
            finally
            {
                if (instr != null)
                {
                    instr.Close();
                }
                if (outstr != null)
                {
                    outstr.Close();
                }
            }
            return buffer;
        }

        public static StreamCtx MakeStreamCtx(IBlockCipher ibc, byte[] key, byte[] iv)
        {
            return MakeStreamCtx(ibc, key, iv, Mode.CBC);
        }
        public static StreamCtx MakeStreamCtx(IBlockCipher ibc, byte[] key, byte[] iv, Mode mode)
        {
            StreamCtx ctx = new StreamCtx
            {
                Ibc = ibc
            };
            ctx.Ibc.InitCipher(key);
            ctx.IV = iv;
            ctx.Mode = mode;
            ctx.MakeReadOnly();
            return ctx;
        }
        public static readonly bool DECRYPT;

        public enum Mode
        {
            ECB,
            CBC
        }
    }

    public sealed class AesFactory
    {
        // Methods
        private AesFactory() { }
        public static IBlockCipher GetAes()
        {
            return new Aes();

        }
    }

    public class Aes : IBlockCipher
    {
        // Fields
        private static readonly byte[] gfmultby02;
        private static readonly byte[] gfmultby03;
        private static readonly byte[] gfmultby09;
        private static readonly byte[] gfmultby0b;
        private static readonly byte[] gfmultby0d;
        private static readonly byte[] gfmultby0e;
        private static readonly byte[,] iSbox;
        private byte[] key;
        private int Nb;
        private int Nk;
        private int Nr;
        private static readonly byte[,] Rcon;
        private static readonly byte[,] Sbox;
        private byte[,] State;
        private byte[,] w;

        // Methods
        static Aes()
        {
            Sbox = new byte[,] { { 0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 1, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76 }, { 0xca, 130, 0xc9, 0x7d, 250, 0x59, 0x47, 240, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0 }, { 0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15 }, { 4, 0xc7, 0x23, 0xc3, 0x18, 150, 5, 0x9a, 7, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75 }, { 9, 0x83, 0x2c, 0x1a, 0x1b, 110, 90, 160, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84 }, { 0x53, 0xd1, 0, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 190, 0x39, 0x4a, 0x4c, 0x58, 0xcf }, { 0xd0, 0xef, 170, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 2, 0x7f, 80, 60, 0x9f, 0xa8 }, { 0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 210 }, { 0xcd, 12, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 100, 0x5d, 0x19, 0x73 }, { 0x60, 0x81, 0x4f, 220, 0x22, 0x2a, 0x90, 0x88, 70, 0xee, 0xb8, 20, 0xde, 0x5e, 11, 0xdb }, { 0xe0, 50, 0x3a, 10, 0x49, 6, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79 }, { 0xe7, 200, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 8 }, { 0xba, 120, 0x25, 0x2e, 0x1c, 0xa6, 180, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a }, { 0x70, 0x3e, 0xb5, 0x66, 0x48, 3, 0xf6, 14, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e }, { 0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 30, 0x87, 0xe9, 0xce, 0x55, 40, 0xdf }, { 140, 0xa1, 0x89, 13, 0xbf, 230, 0x42, 0x68, 0x41, 0x99, 0x2d, 15, 0xb0, 0x54, 0xbb, 0x16 } };
            iSbox = new byte[,] { { 0x52, 9, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb }, { 0x7c, 0xe3, 0x39, 130, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb }, { 0x54, 0x7b, 0x94, 50, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 11, 0x42, 250, 0xc3, 0x4e }, { 8, 0x2e, 0xa1, 0x66, 40, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25 }, { 0x72, 0xf8, 0xf6, 100, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92 }, { 0x6c, 0x70, 0x48, 80, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 70, 0x57, 0xa7, 0x8d, 0x9d, 0x84 }, { 0x90, 0xd8, 0xab, 0, 140, 0xbc, 0xd3, 10, 0xf7, 0xe4, 0x58, 5, 0xb8, 0xb3, 0x45, 6 }, { 0xd0, 0x2c, 30, 0x8f, 0xca, 0x3f, 15, 2, 0xc1, 0xaf, 0xbd, 3, 1, 0x13, 0x8a, 0x6b }, { 0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 220, 0xea, 0x97, 0xf2, 0xcf, 0xce, 240, 180, 230, 0x73 }, { 150, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 110 }, { 0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 14, 170, 0x18, 190, 0x1b }, { 0xfc, 0x56, 0x3e, 0x4b, 0xc6, 210, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 120, 0xcd, 90, 0xf4 }, { 0x1f, 0xdd, 0xa8, 0x33, 0x88, 7, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f }, { 0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 13, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef }, { 160, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 200, 0xeb, 0xbb, 60, 0x83, 0x53, 0x99, 0x61 }, { 0x17, 0x2b, 4, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 20, 0x63, 0x55, 0x21, 12, 0x7d } };
            Rcon = new byte[,] { { 0, 0, 0, 0 }, { 1, 0, 0, 0 }, { 2, 0, 0, 0 }, { 4, 0, 0, 0 }, { 8, 0, 0, 0 }, { 0x10, 0, 0, 0 }, { 0x20, 0, 0, 0 }, { 0x40, 0, 0, 0 }, { 0x80, 0, 0, 0 }, { 0x1b, 0, 0, 0 }, { 0x36, 0, 0, 0 } };
            gfmultby02 = new byte[] {
        0, 2, 4, 6, 8, 10, 12, 14, 0x10, 0x12, 20, 0x16, 0x18, 0x1a, 0x1c, 30,
        0x20, 0x22, 0x24, 0x26, 40, 0x2a, 0x2c, 0x2e, 0x30, 50, 0x34, 0x36, 0x38, 0x3a, 60, 0x3e,
        0x40, 0x42, 0x44, 70, 0x48, 0x4a, 0x4c, 0x4e, 80, 0x52, 0x54, 0x56, 0x58, 90, 0x5c, 0x5e,
        0x60, 0x62, 100, 0x66, 0x68, 0x6a, 0x6c, 110, 0x70, 0x72, 0x74, 0x76, 120, 0x7a, 0x7c, 0x7e,
        0x80, 130, 0x84, 0x86, 0x88, 0x8a, 140, 0x8e, 0x90, 0x92, 0x94, 150, 0x98, 0x9a, 0x9c, 0x9e,
        160, 0xa2, 0xa4, 0xa6, 0xa8, 170, 0xac, 0xae, 0xb0, 0xb2, 180, 0xb6, 0xb8, 0xba, 0xbc, 190,
        0xc0, 0xc2, 0xc4, 0xc6, 200, 0xca, 0xcc, 0xce, 0xd0, 210, 0xd4, 0xd6, 0xd8, 0xda, 220, 0xde,
        0xe0, 0xe2, 0xe4, 230, 0xe8, 0xea, 0xec, 0xee, 240, 0xf2, 0xf4, 0xf6, 0xf8, 250, 0xfc, 0xfe,
        0x1b, 0x19, 0x1f, 0x1d, 0x13, 0x11, 0x17, 0x15, 11, 9, 15, 13, 3, 1, 7, 5,
        0x3b, 0x39, 0x3f, 0x3d, 0x33, 0x31, 0x37, 0x35, 0x2b, 0x29, 0x2f, 0x2d, 0x23, 0x21, 0x27, 0x25,
        0x5b, 0x59, 0x5f, 0x5d, 0x53, 0x51, 0x57, 0x55, 0x4b, 0x49, 0x4f, 0x4d, 0x43, 0x41, 0x47, 0x45,
        0x7b, 0x79, 0x7f, 0x7d, 0x73, 0x71, 0x77, 0x75, 0x6b, 0x69, 0x6f, 0x6d, 0x63, 0x61, 0x67, 0x65,
        0x9b, 0x99, 0x9f, 0x9d, 0x93, 0x91, 0x97, 0x95, 0x8b, 0x89, 0x8f, 0x8d, 0x83, 0x81, 0x87, 0x85,
        0xbb, 0xb9, 0xbf, 0xbd, 0xb3, 0xb1, 0xb7, 0xb5, 0xab, 0xa9, 0xaf, 0xad, 0xa3, 0xa1, 0xa7, 0xa5,
        0xdb, 0xd9, 0xdf, 0xdd, 0xd3, 0xd1, 0xd7, 0xd5, 0xcb, 0xc9, 0xcf, 0xcd, 0xc3, 0xc1, 0xc7, 0xc5,
        0xfb, 0xf9, 0xff, 0xfd, 0xf3, 0xf1, 0xf7, 0xf5, 0xeb, 0xe9, 0xef, 0xed, 0xe3, 0xe1, 0xe7, 0xe5
     };
            gfmultby03 = new byte[] {
        0, 3, 6, 5, 12, 15, 10, 9, 0x18, 0x1b, 30, 0x1d, 20, 0x17, 0x12, 0x11,
        0x30, 0x33, 0x36, 0x35, 60, 0x3f, 0x3a, 0x39, 40, 0x2b, 0x2e, 0x2d, 0x24, 0x27, 0x22, 0x21,
        0x60, 0x63, 0x66, 0x65, 0x6c, 0x6f, 0x6a, 0x69, 120, 0x7b, 0x7e, 0x7d, 0x74, 0x77, 0x72, 0x71,
        80, 0x53, 0x56, 0x55, 0x5c, 0x5f, 90, 0x59, 0x48, 0x4b, 0x4e, 0x4d, 0x44, 0x47, 0x42, 0x41,
        0xc0, 0xc3, 0xc6, 0xc5, 0xcc, 0xcf, 0xca, 0xc9, 0xd8, 0xdb, 0xde, 0xdd, 0xd4, 0xd7, 210, 0xd1,
        240, 0xf3, 0xf6, 0xf5, 0xfc, 0xff, 250, 0xf9, 0xe8, 0xeb, 0xee, 0xed, 0xe4, 0xe7, 0xe2, 0xe1,
        160, 0xa3, 0xa6, 0xa5, 0xac, 0xaf, 170, 0xa9, 0xb8, 0xbb, 190, 0xbd, 180, 0xb7, 0xb2, 0xb1,
        0x90, 0x93, 150, 0x95, 0x9c, 0x9f, 0x9a, 0x99, 0x88, 0x8b, 0x8e, 0x8d, 0x84, 0x87, 130, 0x81,
        0x9b, 0x98, 0x9d, 0x9e, 0x97, 0x94, 0x91, 0x92, 0x83, 0x80, 0x85, 0x86, 0x8f, 140, 0x89, 0x8a,
        0xab, 0xa8, 0xad, 0xae, 0xa7, 0xa4, 0xa1, 0xa2, 0xb3, 0xb0, 0xb5, 0xb6, 0xbf, 0xbc, 0xb9, 0xba,
        0xfb, 0xf8, 0xfd, 0xfe, 0xf7, 0xf4, 0xf1, 0xf2, 0xe3, 0xe0, 0xe5, 230, 0xef, 0xec, 0xe9, 0xea,
        0xcb, 200, 0xcd, 0xce, 0xc7, 0xc4, 0xc1, 0xc2, 0xd3, 0xd0, 0xd5, 0xd6, 0xdf, 220, 0xd9, 0xda,
        0x5b, 0x58, 0x5d, 0x5e, 0x57, 0x54, 0x51, 0x52, 0x43, 0x40, 0x45, 70, 0x4f, 0x4c, 0x49, 0x4a,
        0x6b, 0x68, 0x6d, 110, 0x67, 100, 0x61, 0x62, 0x73, 0x70, 0x75, 0x76, 0x7f, 0x7c, 0x79, 0x7a,
        0x3b, 0x38, 0x3d, 0x3e, 0x37, 0x34, 0x31, 50, 0x23, 0x20, 0x25, 0x26, 0x2f, 0x2c, 0x29, 0x2a,
        11, 8, 13, 14, 7, 4, 1, 2, 0x13, 0x10, 0x15, 0x16, 0x1f, 0x1c, 0x19, 0x1a
     };
            gfmultby09 = new byte[] {
        0, 9, 0x12, 0x1b, 0x24, 0x2d, 0x36, 0x3f, 0x48, 0x41, 90, 0x53, 0x6c, 0x65, 0x7e, 0x77,
        0x90, 0x99, 130, 0x8b, 180, 0xbd, 0xa6, 0xaf, 0xd8, 0xd1, 0xca, 0xc3, 0xfc, 0xf5, 0xee, 0xe7,
        0x3b, 50, 0x29, 0x20, 0x1f, 0x16, 13, 4, 0x73, 0x7a, 0x61, 0x68, 0x57, 0x5e, 0x45, 0x4c,
        0xab, 0xa2, 0xb9, 0xb0, 0x8f, 0x86, 0x9d, 0x94, 0xe3, 0xea, 0xf1, 0xf8, 0xc7, 0xce, 0xd5, 220,
        0x76, 0x7f, 100, 0x6d, 0x52, 0x5b, 0x40, 0x49, 0x3e, 0x37, 0x2c, 0x25, 0x1a, 0x13, 8, 1,
        230, 0xef, 0xf4, 0xfd, 0xc2, 0xcb, 0xd0, 0xd9, 0xae, 0xa7, 0xbc, 0xb5, 0x8a, 0x83, 0x98, 0x91,
        0x4d, 0x44, 0x5f, 0x56, 0x69, 0x60, 0x7b, 0x72, 5, 12, 0x17, 30, 0x21, 40, 0x33, 0x3a,
        0xdd, 0xd4, 0xcf, 0xc6, 0xf9, 240, 0xeb, 0xe2, 0x95, 0x9c, 0x87, 0x8e, 0xb1, 0xb8, 0xa3, 170,
        0xec, 0xe5, 0xfe, 0xf7, 200, 0xc1, 0xda, 0xd3, 0xa4, 0xad, 0xb6, 0xbf, 0x80, 0x89, 0x92, 0x9b,
        0x7c, 0x75, 110, 0x67, 0x58, 0x51, 0x4a, 0x43, 0x34, 0x3d, 0x26, 0x2f, 0x10, 0x19, 2, 11,
        0xd7, 0xde, 0xc5, 0xcc, 0xf3, 250, 0xe1, 0xe8, 0x9f, 150, 0x8d, 0x84, 0xbb, 0xb2, 0xa9, 160,
        0x47, 0x4e, 0x55, 0x5c, 0x63, 0x6a, 0x71, 120, 15, 6, 0x1d, 20, 0x2b, 0x22, 0x39, 0x30,
        0x9a, 0x93, 0x88, 0x81, 190, 0xb7, 0xac, 0xa5, 210, 0xdb, 0xc0, 0xc9, 0xf6, 0xff, 0xe4, 0xed,
        10, 3, 0x18, 0x11, 0x2e, 0x27, 60, 0x35, 0x42, 0x4b, 80, 0x59, 0x66, 0x6f, 0x74, 0x7d,
        0xa1, 0xa8, 0xb3, 0xba, 0x85, 140, 0x97, 0x9e, 0xe9, 0xe0, 0xfb, 0xf2, 0xcd, 0xc4, 0xdf, 0xd6,
        0x31, 0x38, 0x23, 0x2a, 0x15, 0x1c, 7, 14, 0x79, 0x70, 0x6b, 0x62, 0x5d, 0x54, 0x4f, 70
     };
            gfmultby0b = new byte[] {
        0, 11, 0x16, 0x1d, 0x2c, 0x27, 0x3a, 0x31, 0x58, 0x53, 0x4e, 0x45, 0x74, 0x7f, 0x62, 0x69,
        0xb0, 0xbb, 0xa6, 0xad, 0x9c, 0x97, 0x8a, 0x81, 0xe8, 0xe3, 0xfe, 0xf5, 0xc4, 0xcf, 210, 0xd9,
        0x7b, 0x70, 0x6d, 0x66, 0x57, 0x5c, 0x41, 0x4a, 0x23, 40, 0x35, 0x3e, 15, 4, 0x19, 0x12,
        0xcb, 0xc0, 0xdd, 0xd6, 0xe7, 0xec, 0xf1, 250, 0x93, 0x98, 0x85, 0x8e, 0xbf, 180, 0xa9, 0xa2,
        0xf6, 0xfd, 0xe0, 0xeb, 0xda, 0xd1, 0xcc, 0xc7, 0xae, 0xa5, 0xb8, 0xb3, 130, 0x89, 0x94, 0x9f,
        70, 0x4d, 80, 0x5b, 0x6a, 0x61, 0x7c, 0x77, 30, 0x15, 8, 3, 50, 0x39, 0x24, 0x2f,
        0x8d, 0x86, 0x9b, 0x90, 0xa1, 170, 0xb7, 0xbc, 0xd5, 0xde, 0xc3, 200, 0xf9, 0xf2, 0xef, 0xe4,
        0x3d, 0x36, 0x2b, 0x20, 0x11, 0x1a, 7, 12, 0x65, 110, 0x73, 120, 0x49, 0x42, 0x5f, 0x54,
        0xf7, 0xfc, 0xe1, 0xea, 0xdb, 0xd0, 0xcd, 0xc6, 0xaf, 0xa4, 0xb9, 0xb2, 0x83, 0x88, 0x95, 0x9e,
        0x47, 0x4c, 0x51, 90, 0x6b, 0x60, 0x7d, 0x76, 0x1f, 20, 9, 2, 0x33, 0x38, 0x25, 0x2e,
        140, 0x87, 0x9a, 0x91, 160, 0xab, 0xb6, 0xbd, 0xd4, 0xdf, 0xc2, 0xc9, 0xf8, 0xf3, 0xee, 0xe5,
        60, 0x37, 0x2a, 0x21, 0x10, 0x1b, 6, 13, 100, 0x6f, 0x72, 0x79, 0x48, 0x43, 0x5e, 0x55,
        1, 10, 0x17, 0x1c, 0x2d, 0x26, 0x3b, 0x30, 0x59, 0x52, 0x4f, 0x44, 0x75, 0x7e, 0x63, 0x68,
        0xb1, 0xba, 0xa7, 0xac, 0x9d, 150, 0x8b, 0x80, 0xe9, 0xe2, 0xff, 0xf4, 0xc5, 0xce, 0xd3, 0xd8,
        0x7a, 0x71, 0x6c, 0x67, 0x56, 0x5d, 0x40, 0x4b, 0x22, 0x29, 0x34, 0x3f, 14, 5, 0x18, 0x13,
        0xca, 0xc1, 220, 0xd7, 230, 0xed, 240, 0xfb, 0x92, 0x99, 0x84, 0x8f, 190, 0xb5, 0xa8, 0xa3
     };
            gfmultby0d = new byte[] {
        0, 13, 0x1a, 0x17, 0x34, 0x39, 0x2e, 0x23, 0x68, 0x65, 0x72, 0x7f, 0x5c, 0x51, 70, 0x4b,
        0xd0, 0xdd, 0xca, 0xc7, 0xe4, 0xe9, 0xfe, 0xf3, 0xb8, 0xb5, 0xa2, 0xaf, 140, 0x81, 150, 0x9b,
        0xbb, 0xb6, 0xa1, 0xac, 0x8f, 130, 0x95, 0x98, 0xd3, 0xde, 0xc9, 0xc4, 0xe7, 0xea, 0xfd, 240,
        0x6b, 0x66, 0x71, 0x7c, 0x5f, 0x52, 0x45, 0x48, 3, 14, 0x19, 20, 0x37, 0x3a, 0x2d, 0x20,
        0x6d, 0x60, 0x77, 0x7a, 0x59, 0x54, 0x43, 0x4e, 5, 8, 0x1f, 0x12, 0x31, 60, 0x2b, 0x26,
        0xbd, 0xb0, 0xa7, 170, 0x89, 0x84, 0x93, 0x9e, 0xd5, 0xd8, 0xcf, 0xc2, 0xe1, 0xec, 0xfb, 0xf6,
        0xd6, 0xdb, 0xcc, 0xc1, 0xe2, 0xef, 0xf8, 0xf5, 190, 0xb3, 0xa4, 0xa9, 0x8a, 0x87, 0x90, 0x9d,
        6, 11, 0x1c, 0x11, 50, 0x3f, 40, 0x25, 110, 0x63, 0x74, 0x79, 90, 0x57, 0x40, 0x4d,
        0xda, 0xd7, 0xc0, 0xcd, 0xee, 0xe3, 0xf4, 0xf9, 0xb2, 0xbf, 0xa8, 0xa5, 0x86, 0x8b, 0x9c, 0x91,
        10, 7, 0x10, 0x1d, 0x3e, 0x33, 0x24, 0x29, 0x62, 0x6f, 120, 0x75, 0x56, 0x5b, 0x4c, 0x41,
        0x61, 0x6c, 0x7b, 0x76, 0x55, 0x58, 0x4f, 0x42, 9, 4, 0x13, 30, 0x3d, 0x30, 0x27, 0x2a,
        0xb1, 0xbc, 0xab, 0xa6, 0x85, 0x88, 0x9f, 0x92, 0xd9, 0xd4, 0xc3, 0xce, 0xed, 0xe0, 0xf7, 250,
        0xb7, 0xba, 0xad, 160, 0x83, 0x8e, 0x99, 0x94, 0xdf, 210, 0xc5, 200, 0xeb, 230, 0xf1, 0xfc,
        0x67, 0x6a, 0x7d, 0x70, 0x53, 0x5e, 0x49, 0x44, 15, 2, 0x15, 0x18, 0x3b, 0x36, 0x21, 0x2c,
        12, 1, 0x16, 0x1b, 0x38, 0x35, 0x22, 0x2f, 100, 0x69, 0x7e, 0x73, 80, 0x5d, 0x4a, 0x47,
        220, 0xd1, 0xc6, 0xcb, 0xe8, 0xe5, 0xf2, 0xff, 180, 0xb9, 0xae, 0xa3, 0x80, 0x8d, 0x9a, 0x97
     };
            gfmultby0e = new byte[] {
        0, 14, 0x1c, 0x12, 0x38, 0x36, 0x24, 0x2a, 0x70, 0x7e, 0x6c, 0x62, 0x48, 70, 0x54, 90,
        0xe0, 0xee, 0xfc, 0xf2, 0xd8, 0xd6, 0xc4, 0xca, 0x90, 0x9e, 140, 130, 0xa8, 0xa6, 180, 0xba,
        0xdb, 0xd5, 0xc7, 0xc9, 0xe3, 0xed, 0xff, 0xf1, 0xab, 0xa5, 0xb7, 0xb9, 0x93, 0x9d, 0x8f, 0x81,
        0x3b, 0x35, 0x27, 0x29, 3, 13, 0x1f, 0x11, 0x4b, 0x45, 0x57, 0x59, 0x73, 0x7d, 0x6f, 0x61,
        0xad, 0xa3, 0xb1, 0xbf, 0x95, 0x9b, 0x89, 0x87, 0xdd, 0xd3, 0xc1, 0xcf, 0xe5, 0xeb, 0xf9, 0xf7,
        0x4d, 0x43, 0x51, 0x5f, 0x75, 0x7b, 0x69, 0x67, 0x3d, 0x33, 0x21, 0x2f, 5, 11, 0x19, 0x17,
        0x76, 120, 0x6a, 100, 0x4e, 0x40, 0x52, 0x5c, 6, 8, 0x1a, 20, 0x3e, 0x30, 0x22, 0x2c,
        150, 0x98, 0x8a, 0x84, 0xae, 160, 0xb2, 0xbc, 230, 0xe8, 250, 0xf4, 0xde, 0xd0, 0xc2, 0xcc,
        0x41, 0x4f, 0x5d, 0x53, 0x79, 0x77, 0x65, 0x6b, 0x31, 0x3f, 0x2d, 0x23, 9, 7, 0x15, 0x1b,
        0xa1, 0xaf, 0xbd, 0xb3, 0x99, 0x97, 0x85, 0x8b, 0xd1, 0xdf, 0xcd, 0xc3, 0xe9, 0xe7, 0xf5, 0xfb,
        0x9a, 0x94, 0x86, 0x88, 0xa2, 0xac, 190, 0xb0, 0xea, 0xe4, 0xf6, 0xf8, 210, 220, 0xce, 0xc0,
        0x7a, 0x74, 0x66, 0x68, 0x42, 0x4c, 0x5e, 80, 10, 4, 0x16, 0x18, 50, 60, 0x2e, 0x20,
        0xec, 0xe2, 240, 0xfe, 0xd4, 0xda, 200, 0xc6, 0x9c, 0x92, 0x80, 0x8e, 0xa4, 170, 0xb8, 0xb6,
        12, 2, 0x10, 30, 0x34, 0x3a, 40, 0x26, 0x7c, 0x72, 0x60, 110, 0x44, 0x4a, 0x58, 0x56,
        0x37, 0x39, 0x2b, 0x25, 15, 1, 0x13, 0x1d, 0x47, 0x49, 0x5b, 0x55, 0x7f, 0x71, 0x63, 0x6d,
        0xd7, 0xd9, 0xcb, 0xc5, 0xef, 0xe1, 0xf3, 0xfd, 0xa7, 0xa9, 0xbb, 0xb5, 0x9f, 0x91, 0x83, 0x8d
     };

        }
        public Aes() { }
        private void AddRoundKey(int round)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.State[i, j] ^= this.w[(round * 4) + j, i];
                }
            }
        }
        public int BlockSizeInBytes()
        {
            return 0x10;
        }
        public void Cipher(byte[] input, byte[] output)
        {
            this.State = new byte[4, this.Nb];
            for (int i = 0; i < (4 * this.Nb); i++)
            {
                this.State[i % 4, i / 4] = input[i];
            }
            this.AddRoundKey(0);
            for (int j = 1; j <= (this.Nr - 1); j++)
            {
                this.SubBytes();
                this.ShiftRows();
                this.MixColumns();
                this.AddRoundKey(j);
            }
            this.SubBytes();
            this.ShiftRows();
            this.AddRoundKey(this.Nr);
            for (int k = 0; k < (4 * this.Nb); k++)
            {
                output[k] = this.State[k % 4, k / 4];
            }
        }
        public void InitCipher(byte[] keyBytes)
        {
            KeySize keySize = KeySize.Bits256;
            switch (keyBytes.Length)
            {
                case 0x10:
                    keySize = KeySize.Bits128;
                    break;

                case 0x18:
                    keySize = KeySize.Bits192;
                    break;
            }
            this.SetNbNkNr(keySize);
            this.key = new byte[this.Nk * 4];
            keyBytes.CopyTo(this.key, 0);
            this.KeyExpansion();
        }

        public void InvCipher(byte[] input, byte[] output)
        {
            this.State = new byte[4, this.Nb];
            for (int i = 0; i < (4 * this.Nb); i++)
            {
                this.State[i % 4, i / 4] = input[i];
            }
            this.AddRoundKey(this.Nr);
            for (int j = this.Nr - 1; j >= 1; j--)
            {
                this.InvShiftRows();
                this.InvSubBytes();
                this.AddRoundKey(j);
                this.InvMixColumns();
            }
            this.InvShiftRows();
            this.InvSubBytes();
            this.AddRoundKey(0);
            for (int k = 0; k < (4 * this.Nb); k++)
            {
                output[k] = this.State[k % 4, k / 4];
            }
        }

        private void InvMixColumns()
        {
            byte[,] buffer = new byte[4, 4];
            Array.Copy(this.State, buffer, 0x10);
            for (int i = 0; i < 4; i++)
            {
                this.State[0, i] = (byte)(((gfmultby0e[buffer[0, i]] ^ gfmultby0b[buffer[1, i]]) ^ gfmultby0d[buffer[2, i]]) ^ gfmultby09[buffer[3, i]]);
                this.State[1, i] = (byte)(((gfmultby09[buffer[0, i]] ^ gfmultby0e[buffer[1, i]]) ^ gfmultby0b[buffer[2, i]]) ^ gfmultby0d[buffer[3, i]]);
                this.State[2, i] = (byte)(((gfmultby0d[buffer[0, i]] ^ gfmultby09[buffer[1, i]]) ^ gfmultby0e[buffer[2, i]]) ^ gfmultby0b[buffer[3, i]]);
                this.State[3, i] = (byte)(((gfmultby0b[buffer[0, i]] ^ gfmultby0d[buffer[1, i]]) ^ gfmultby09[buffer[2, i]]) ^ gfmultby0e[buffer[3, i]]);
            }
        }
        private void InvShiftRows()
        {
            byte[,] buffer = new byte[4, 4];
            Array.Copy(this.State, buffer, 0x10);
            for (int i = 1; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.State[i, (j + i) % this.Nb] = buffer[i, j];
                }
            }
        }
        private void InvSubBytes()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.State[i, j] = iSbox[this.State[i, j] >> 4, this.State[i, j] & 15];
                }
            }
        }
        private void KeyExpansion()
        {
            this.w = new byte[this.Nb * (this.Nr + 1), 4];
            for (int i = 0; i < this.Nk; i++)
            {
                this.w[i, 0] = this.key[4 * i];
                this.w[i, 1] = this.key[(4 * i) + 1];
                this.w[i, 2] = this.key[(4 * i) + 2];
                this.w[i, 3] = this.key[(4 * i) + 3];
            }
            byte[] word = new byte[4];
            for (int j = this.Nk; j < (this.Nb * (this.Nr + 1)); j++)
            {
                word[0] = this.w[j - 1, 0];
                word[1] = this.w[j - 1, 1];
                word[2] = this.w[j - 1, 2];
                word[3] = this.w[j - 1, 3];
                if ((j % this.Nk) == 0)
                {
                    word = this.SubWord(this.RotWord(word));
                    word[0] ^= Rcon[j / this.Nk, 0];
                    word[1] ^= Rcon[j / this.Nk, 1];
                    word[2] ^= Rcon[j / this.Nk, 2];
                    word[3] ^= Rcon[j / this.Nk, 3];
                }
                else if ((this.Nk > 6) && ((j % this.Nk) == 4))
                {
                    word = this.SubWord(word);
                }
                this.w[j, 0] = (byte)(this.w[j - this.Nk, 0] ^ word[0]);
                this.w[j, 1] = (byte)(this.w[j - this.Nk, 1] ^ word[1]);
                this.w[j, 2] = (byte)(this.w[j - this.Nk, 2] ^ word[2]);
                this.w[j, 3] = (byte)(this.w[j - this.Nk, 3] ^ word[3]);
            }
        }

        public int[] KeySizesInBytes()
        {
            return new int[] { 0x10, 0x18, 0x20 };
        }

        private void MixColumns()
        {
            byte[,] buffer = new byte[4, 4];
            Array.Copy(this.State, buffer, 0x10);
            for (int i = 0; i < 4; i++)
            {
                this.State[0, i] = (byte)(((gfmultby02[buffer[0, i]] ^ gfmultby03[buffer[1, i]]) ^ buffer[2, i]) ^ buffer[3, i]);
                this.State[1, i] = (byte)(((buffer[0, i] ^ gfmultby02[buffer[1, i]]) ^ gfmultby03[buffer[2, i]]) ^ buffer[3, i]);
                this.State[2, i] = (byte)(((buffer[0, i] ^ buffer[1, i]) ^ gfmultby02[buffer[2, i]]) ^ gfmultby03[buffer[3, i]]);
                this.State[3, i] = (byte)(((gfmultby03[buffer[0, i]] ^ buffer[1, i]) ^ buffer[2, i]) ^ gfmultby02[buffer[3, i]]);
            }
        }
        private byte[] RotWord(byte[] word)
        {
            return new byte[] { word[1], word[2], word[3], word[0] };
        }

        private void SetNbNkNr(KeySize keySize)
        {
            this.Nb = 4;
            if (keySize == KeySize.Bits128)
            {
                this.Nk = 4;
                this.Nr = 10;
            }
            else if (keySize == KeySize.Bits192)
            {
                this.Nk = 6;
                this.Nr = 12;
            }
            else if (keySize == KeySize.Bits256)
            {
                this.Nk = 8;
                this.Nr = 14;
            }
        }
        private void ShiftRows()
        {
            byte[,] buffer = new byte[4, 4];
            Array.Copy(this.State, buffer, 0x10);
            for (int i = 1; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.State[i, j] = buffer[i, (j + i) % this.Nb];
                }
            }
        }
        private void SubBytes()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.State[i, j] = Sbox[this.State[i, j] >> 4, this.State[i, j] & 15];
                }
            }
        }
        private byte[] SubWord(byte[] word)
        {
            return new byte[] { Sbox[word[0] >> 4, word[0] & 15], Sbox[word[1] >> 4, word[1] & 15], Sbox[word[2] >> 4, word[2] & 15], Sbox[word[3] >> 4, word[3] & 15] };
        }

        // Nested Types
        public enum KeySize
        {
            Bits128,
            Bits192,
            Bits256
        }
    }

    public class StreamCtx
    {
        // Fields
        private bool fReadOnly;
        private IBlockCipher ibc;
        private byte[] iv;
        private StreamCipher.Mode mode;

        // Methods
        public StreamCtx()
        {
            this.ibc = null;
            this.iv = null;
            this.mode = StreamCipher.Mode.CBC;
            this.fReadOnly = false;
        }

        public int BlockSize()
        {
            return this.ibc.BlockSizeInBytes();
        }
        private void CheckReadOnly()
        {
            if (this.fReadOnly)
            {
                throw new Exception();
            }

        }
        public void MakeReadOnly()
        {
            this.fReadOnly = true;

        }

        // Properties
        public IBlockCipher Ibc
        {
            get
            {
                return this.ibc;
            }
            set
            {
                this.CheckReadOnly();
                this.ibc = value;
            }
        }
        public byte[] IV
        {
            get
            {
                return this.iv;
            }
            set
            {
                this.CheckReadOnly();
                this.iv = value;
            }
        }
        public StreamCipher.Mode Mode
        {

            get
            {
                return this.mode;
            }
            set
            {
                this.CheckReadOnly();
                this.mode = value;
            }
        }
    }

    public class Encription
    {
        // Fields
        private StreamCtx aesNow;
        private string CipherText;
        private const string ecmsKey = "ANIEncryptionLib";
        private string plainText;

        // Methods
        public Encription()
        {
            this.plainText = "";
            this.CipherText = "";
            int keySize = 0x20;
            IBlockCipher aes = AesFactory.GetAes();
            byte[] iv = new byte[aes.BlockSizeInBytes()];
            for (int i = 0; i < iv.Length; i++)
            {
                iv[i] = 0;
            }
            byte[] salt = new byte[8];
            int iterationCount = 0x400;
            long num4 = DateTime.Now.Ticks;
            byte[] key = KeyGen.DeriveKey("ANIEncryptionLib", keySize, salt, iterationCount);
            num4 = DateTime.Now.Ticks - num4;
            this.aesNow = StreamCipher.MakeStreamCtx(aes, key, iv);

        }
        private string convertBytes2String(byte[] bytes)
        {
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str = str + bytes[i].ToString("x2");
            }
            return str;

        }
        private byte[] convertHex2Bytes(string bytesStr)
        {
            int num = bytesStr.Length / 2;
            byte[] buffer = new byte[num];
            int index = 0;
            for (int i = 0; i < bytesStr.Length; i += 2)
            {
                int num4 = int.Parse(bytesStr.Substring(i, 2), (System.Globalization.NumberStyles)0x203);

                buffer[index] = Convert.ToByte(num4);
                index++;
            }
            return buffer;

        }
        public string decrypt(string inputStr)
        {
            string str = "";
            try
            {
                byte[] data = this.convertHex2Bytes(inputStr);
                byte[] buffer2 = StreamCipher.Encode(this.aesNow, data, StreamCipher.DECRYPT);
                str = Encoding.Unicode.GetString(buffer2);
                this.plainText = str;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error : " + exception);
            }
            return str;

        }
        public string encrypt(string inputStr)
        {
            string str = "";
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(inputStr);
                byte[] buffer2 = StreamCipher.Encode(this.aesNow, bytes, StreamCipher.ENCRYPT);
                str = this.convertBytes2String(buffer2);
                this.CipherText = str;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error : " + exception);
            }
            return str;

        }
        public bool equalsToCipherText(string inputStr)
        {
            bool flag = false;
            try
            {
                flag = this.CipherText.Equals(inputStr);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error : " + exception);
            }
            return flag;

        }
        public bool equalsToPalinText(string inputStr)
        {
            bool flag = false;
            try
            {
                flag = this.plainText.Equals(inputStr);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error : " + exception);
            }
            return flag;

        }

    }

    class KeyGen
    {
        public static byte[] DeriveKey(string password, int keySize, byte[] salt, int iterationCount)
        {
            if (keySize > 0x20)
            {
                keySize = 0x20;
            }
            byte[] message = ASCIIEncoder(password);
            if (salt != null)
            {
                byte[] buffer2 = new byte[message.Length + salt.Length];
                Array.Copy(message, 0, buffer2, 0, message.Length);
                Array.Copy(salt, 0, buffer2, message.Length, salt.Length);
                message = buffer2;
            }
            if (iterationCount <= 0)
            {
                iterationCount = 1;
            }
            for (int i = 0; i < iterationCount; i++)
            {
                message = MessageSHA256(message);
            }
            byte[] buffer3 = new byte[keySize];
            Array.Copy(message, 0, buffer3, 0, keySize);
            return buffer3;
        }
        public static byte[] MessageSHA256(byte[] message)
        {
            int num2;
            int n = message.Length >> 6;
            if ((message.Length & 0x3f) < 0x38)
            {
                n++;
                num2 = 0x38;
            }
            else
            {
                n += 2;
                num2 = 120;
            }
            long num3 = message.Length << 3;
            byte[] buffer2 = new byte[num2 - (message.Length & 0x3f)];
            buffer2[0] = 0x80;
            for (int i = 1; i < buffer2.Length; i++)
            {
                buffer2[i] = 0;
            }
            byte[] m = new byte[n * 0x40];
            Array.Copy(message, 0, m, 0, message.Length);
            Array.Copy(buffer2, 0, m, message.Length, buffer2.Length);
            byte[] bytes = BitConverter.GetBytes(num3);
            byte[] buffer3 = new byte[8];
            for (int j = 0; j < 8; j++)
            {
                buffer3[j] = bytes[7 - j];
            }
            Array.Copy(buffer3, 0, m, message.Length + buffer2.Length, 8);
            return Hash256(n, m);
        }

        private static byte[] Hash256(int n, byte[] m)
        {
            uint[] numArray = new uint[] {
        0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5, 0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
        0xe49b69c1, 0xefbe4786, 0xfc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da, 0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x6ca6351, 0x14292967,
        0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85, 0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
        0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3, 0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
     };
            uint[] numArray2 = new uint[8];
            uint[] numArray3 = new uint[0x40];
            byte[] buffer = new byte[0x20];
            byte[] buffer2 = new byte[4];
            byte[] bytes = new byte[4];
            numArray2[0] = 0x6a09e667;
            numArray2[1] = 0xbb67ae85;
            numArray2[2] = 0x3c6ef372;
            numArray2[3] = 0xa54ff53a;
            numArray2[4] = 0x510e527f;
            numArray2[5] = 0x9b05688c;
            numArray2[6] = 0x1f83d9ab;
            numArray2[7] = 0x5be0cd19;
            for (int i = 0; i < n; i++)
            {
                for (int k = 0; k < 0x40; k++)
                {
                    if (k < 0x10)
                    {
                        for (int num13 = 0; num13 < 4; num13++)
                        {
                            buffer2[num13] = m[(((i * 0x40) + (k * 4)) + 3) - num13];
                        }
                        numArray3[k] = BitConverter.ToUInt32(buffer2, 0);
                    }
                    else
                    {
                        numArray3[k] = ((Sigma(3, numArray3[k - 2]) + numArray3[k - 7]) + Sigma(2, numArray3[k - 15])) + numArray3[k - 0x10];
                    }
                }
                uint x = numArray2[0];
                uint y = numArray2[1];
                uint z = numArray2[2];
                uint num4 = numArray2[3];
                uint num5 = numArray2[4];
                uint num6 = numArray2[5];
                uint num7 = numArray2[6];
                uint num8 = numArray2[7];
                for (int num14 = 0; num14 < 0x40; num14++)
                {
                    uint num9 = (((num8 + Sigma(1, num5)) + Func(true, num5, num6, num7)) + numArray[num14]) + numArray3[num14];
                    uint num10 = Sigma(0, x) + Func(false, x, y, z);
                    num8 = num7;
                    num7 = num6;
                    num6 = num5;
                    num5 = num4 + num9;
                    num4 = z;
                    z = y;
                    y = x;
                    x = num9 + num10;
                }
                numArray2[0] = x + numArray2[0];
                numArray2[1] = y + numArray2[1];
                numArray2[2] = z + numArray2[2];
                numArray2[3] = num4 + numArray2[3];
                numArray2[4] = num5 + numArray2[4];
                numArray2[5] = num6 + numArray2[5];
                numArray2[6] = num7 + numArray2[6];
                numArray2[7] = num8 + numArray2[7];
            }
            for (int j = 0; j < 8; j++)
            {
                bytes = BitConverter.GetBytes(numArray2[j]);
                for (int num16 = 0; num16 < 4; num16++)
                {
                    buffer2[num16] = bytes[3 - num16];
                }
                Array.Copy(buffer2, 0, buffer, j * 4, 4);
            }
            return buffer;
        }
        private static uint Func(bool b, uint x, uint y, uint z)
        {
            if (b)
            {
                return ((x & y) ^ (~x & z));
            }
            return (((x & y) ^ (x & z)) ^ (y & z));
        }


        private static uint Sigma(int i, uint x)
        {
            uint num;
            switch (i)
            {
                case 0:
                    num = (x >> 2) | (x << 30);
                    num ^= (x >> 13) | (x << 0x13);
                    return (num ^ ((x >> 0x16) | (x << 10)));

                case 1:
                    num = (x >> 6) | (x << 0x1a);
                    num ^= (x >> 11) | (x << 0x15);
                    return (num ^ ((x >> 0x19) | (x << 7)));

                case 2:
                    num = (x >> 7) | (x << 0x19);
                    num ^= (x >> 0x12) | (x << 14);
                    return (num ^ (x >> 3));

                case 3:
                    num = (x >> 0x11) | (x << 15);
                    num ^= (x >> 0x13) | (x << 13);
                    return (num ^ (x >> 10));
            }
            return x;
        }




        public static byte[] ASCIIEncoder(string s)
        {
            byte[] buffer = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                buffer[i] = (byte)s[i];
            }
            return buffer;
        }



    }
}
