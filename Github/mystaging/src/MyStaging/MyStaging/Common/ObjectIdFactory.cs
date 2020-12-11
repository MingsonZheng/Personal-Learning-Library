using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MyStaging.Common
{
    public class ObjectIdFactory
    {
        private int increment;
        private readonly byte[] pidHex;
        private readonly byte[] machineHash;
        private readonly UTF8Encoding utf8 = new UTF8Encoding(false);
        private readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public ObjectIdFactory()
        {
            MD5 md5 = MD5.Create();
            machineHash = md5.ComputeHash(utf8.GetBytes(Dns.GetHostName()));
            pidHex = BitConverter.GetBytes(Process.GetCurrentProcess().Id);
            Array.Reverse(pidHex);
        }

        /// <summary>
        ///  产生一个新的 24 位唯一编号
        /// </summary>
        /// <returns></returns>
        public ObjectId NewId()
        {
            int copyIdx = 0;
            byte[] hex = new byte[12];

            byte[] time = BitConverter.GetBytes(GetTimestamp());
            Array.Reverse(time);
            Array.Copy(time, 0, hex, copyIdx, 4);
            copyIdx += 4;

            Array.Copy(machineHash, 0, hex, copyIdx, 3);
            copyIdx += 3;

            Array.Copy(pidHex, 2, hex, copyIdx, 2);
            copyIdx += 2;

            byte[] inc = BitConverter.GetBytes(GetIncrement());
            Array.Reverse(inc);
            Array.Copy(inc, 1, hex, copyIdx, 3);

            return new ObjectId(hex);
        }

        private int GetIncrement() => System.Threading.Interlocked.Increment(ref increment);

        private int GetTimestamp() => Convert.ToInt32(Math.Floor((DateTime.UtcNow - unixEpoch).TotalSeconds));
    }
}
