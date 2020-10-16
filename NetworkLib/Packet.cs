using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Lib
{
    static public class CTypeDefinition
    {
        public const int HeaderSize = 6;
        public const int BufferSize = 8192;
        public const int PacketSize = 8192;
        public const uint PacketDataSize = 8192 - HeaderSize;
        public const ushort NetObjType_Server = 0;
        public const ushort NetObjType_Client = 1;
        public const ushort NetObjType_User = 3;
    }

    public class CPacket
    {
        SocketAsyncEventArgs m_args;
        byte[] m_buffer;

        ushort m_size = 0;
        uint m_type = 0;
        ushort m_seekr = 0;
        ushort m_seekw = 0;
        ushort m_bytestransferred = 0;

        public CPacket(int size)
        {
            m_buffer = new byte[size];
            m_args = new SocketAsyncEventArgs();
            m_args.SetBuffer(m_buffer, 0, size);
            m_args.Completed += AsyncNetwork.IO_Completed;
            m_args.Completed += AsyncNetwork.IO_Completed;
        }
        
        public ushort Size
        {
            get => m_size;
            set => m_size = value;
        }

        public uint Type
        {
            get => m_type;
            set => m_type = value;
        }

        public SocketAsyncEventArgs AsyncAgrs
        {
            get => m_args;
        }

        public byte[] Data
        {
            get => m_buffer;
        }

        public object UserToken
        {
            get => m_args.UserToken;
            set => m_args.UserToken = value;
        }

        public bool IsTransferComplete
        {
            get => (m_bytestransferred == m_size);
        }

        public void Write(int value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, m_buffer, m_seekw, sizeof(int));
            m_seekw += sizeof(int);
        }

        public void Write(byte[] value, int size)
        {
            Buffer.BlockCopy(value, 0, m_buffer, m_seekw, size);
            m_seekw += (ushort)size;
        }

        public void Read(ref int value)
        {
            value = BitConverter.ToInt32(m_buffer, m_seekr);
            m_seekr += (ushort)sizeof(int);
        }

        public void Read(byte[] value, ushort size)
        {
            Buffer.BlockCopy(m_buffer, m_seekr, value, 0, size);
            m_seekr += (ushort)size;
        }

        public void Clear()
        {
            m_buffer.Initialize();
            m_size = 0;
            m_type = 0;
            m_bytestransferred = 0;
        }
    }
}
