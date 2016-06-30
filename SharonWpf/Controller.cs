using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;

namespace Sharon
{
    enum Target : byte
    {
        None = 0,
        Left = 1,
        Right = 2,
        High = 3,
        Middle = 4,
        Low = 5,
        LeftForward = 6,
        LeftBackward = 7,
        RightForward = 8,
        RightBackward = 9
    }

    struct Instruction
    {
        public Instruction(Target target)
            : this(target, 0)
        {
        }

        public Instruction(Target target, ushort value)
        {
            this.Target = target;
            this.Value = value;
        }

        Target Target;
        ushort Value;

        public byte[] ToByteArray()
        {
            var bytes = new byte[Length];
            bytes[0] = (byte)Target;
            bytes[1] = (byte)(Value >> 8);
            bytes[2] = (byte)(Value);
            return bytes;
        }

        public static int Length
        {
            get;
        } = 3;
    }

    class Controller : IDisposable
    {
        public Controller()
        {
            client = new UdpClient();
            client.Connect("192.168.1.1", 10942);
        }

        public async Task SendValue(Instruction instruction)
        {
            try
            {
                await client.SendAsync(instruction.ToByteArray(), Instruction.Length);
            }
            catch(Exception) { }
        }

        private UdpClient client;

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    client.Close();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                client = null;

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Controller() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
