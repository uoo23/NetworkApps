using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Lib
{

    class AsyncNetwork
    {
        static Stack<CNetObj> objpool = new Stack<CNetObj>();

        static bool ProcessConnect(SocketAsyncEventArgs e)
        {
            return true;
        }
        static bool ProcessAccept(SocketAsyncEventArgs e)
        {
            // TODO
            // objpool Pop 실패 처리
            CNetObj serverobj = e.UserToken as CNetObj;
            CNetObj netobj = objpool.Pop(); 

            netobj.Open(e.AcceptSocket);

            bool raise = netobj.RecvPost();

            if (!raise)
            {
                netobj.RecvPost();
            }

            serverobj.AcceptPost();

            return true;
        }
        static public void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                CNetObj obj = e.UserToken as CNetObj;
            }
        }

        static public bool ProcessRecv(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                CNetObj obj = e.UserToken as CNetObj;

                CPacket packet = obj.RecvPacket;
                packet.Write(e.Buffer, e.BytesTransferred);
                if (packet.IsTransferComplete)
                {
                    obj.DoIO();
                    packet.Clear();
                }

                obj.RecvPost();
            }

            return true;
        }
        static bool ProcessDisconnect(SocketAsyncEventArgs e)
        {
            // TODO NetObj Pool 반납
            CNetObj obj = e.UserToken as CNetObj;

            obj.Close();

            objpool.Push(obj);

            return true;
        }
        public static void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e); break;
                case SocketAsyncOperation.Accept:
                    ProcessAccept(e); break;
                case SocketAsyncOperation.Receive:
                    ProcessRecv(e); break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e); break;
                case SocketAsyncOperation.Disconnect:
                    ProcessDisconnect(e); break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }
    }
}
