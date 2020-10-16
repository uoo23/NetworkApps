using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Lib
{
    public class CNetObj
    {
        Socket m_socket;
        CPacket m_recv;

        SocketAsyncEventArgs m_accpet;
        SocketAsyncEventArgs m_connect;

        // TODO
        // 상속 가능 하도록 재 설계 혹은 추가 작업
        uint m_index = 0;
        ushort m_type = 0;

        public virtual bool DoIO()
        {
            return true;
        }


        public CPacket RecvPacket
        {
            get => m_recv;
        }

        public CNetObj(ushort type)
        {
            switch (type)
            {
                case CTypeDefinition.NetObjType_Server:
                case CTypeDefinition.NetObjType_Client:
                case CTypeDefinition.NetObjType_User:
                    break;
                default:
                    throw new Exception("net obj illigal type");
            }

            m_recv = new CPacket(CTypeDefinition.BufferSize);

            m_type = type;
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_recv.UserToken = this;
        }

        void Connect(string ip, ushort port)
        {
            if (m_type != CTypeDefinition.NetObjType_Client) throw new Exception("net obj not client type");
            m_connect = new SocketAsyncEventArgs();
            m_connect.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            m_socket.ConnectAsync(m_connect);
        }
        
        void Start(ushort port)
        {
            if (m_type != CTypeDefinition.NetObjType_Server) throw new Exception("net obj not server type");
            m_socket.NoDelay = true;
            m_socket.Bind(new IPEndPoint(IPAddress.Any, port));
            m_socket.Listen(500);
            m_accpet = new SocketAsyncEventArgs();
            m_socket.AcceptAsync(m_accpet);
        }

        public void Open(Socket socket)
        {
            m_socket = socket;
        }

        public void Close()
        {
            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Dispose();
        }

        public bool AcceptPost()
        {
            m_socket.AcceptAsync(m_accpet);
            return true;
        }

        public bool SendPost(CPacket packet)
        {
            packet.UserToken = this;
            bool raise = m_socket.SendAsync(packet.AsyncAgrs);

            if (!raise)
            {
                AsyncNetwork.ProcessSend(packet.AsyncAgrs);
            }
            
            return true;
        }

        public bool RecvPost()
        {
            bool raise = m_socket.ReceiveAsync(m_recv.AsyncAgrs);

            if (!raise)
            {
                AsyncNetwork.ProcessRecv(m_recv.AsyncAgrs);
            }

            return true;
        }

    }

}
