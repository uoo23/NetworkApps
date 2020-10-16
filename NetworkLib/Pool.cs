using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace Lib
{
    // TODO 
    // trypop 실패 처리

    class CPacketPool
    {
        ConcurrentStack<CPacket> m_recv_pool;
        ConcurrentStack<CPacket> m_send_pool;
        public CPacketPool(int recv_size, int send_size, int buffer_size)
        {
            m_recv_pool = new ConcurrentStack<CPacket>();
            m_send_pool = new ConcurrentStack<CPacket>();

            for (int i = 0; i < recv_size; i++)
            {
                var tmp = new CPacket(buffer_size);
                m_recv_pool.Push(tmp);
            }

            for (int i = 0; i < send_size; i++)
            {
                var tmp = new CPacket(buffer_size);
                m_send_pool.Push(tmp);
            }
        }

        CPacket PopRecv()
        {
            CPacket value;
            m_recv_pool.TryPop(out value);
            return value;
        }
        CPacket PopSend()
        {
            CPacket value;
            m_send_pool.TryPop(out value);
            return value;
        }
        void PushRecv(CPacket packet)
        {
            m_recv_pool.Push(packet);
        }
        void PushSend(CPacket packet)
        {
            m_recv_pool.Push(packet);
        }
    }

    class CNetObjPool
    {
        ConcurrentStack<CNetObj> m_pool;
        public CNetObjPool(int size, ushort type)
        {
            m_pool = new ConcurrentStack<CNetObj>();

            for (int i = 0; i < size; i++)
            {
                var tmp = new CNetObj(type);
                m_pool.Push(tmp);
            }
       }

        CNetObj PopNetObj()
        {
            CNetObj value;
            m_pool.TryPop(out value);
            return value;
        }

        void PushNetObj(CNetObj netobj)
        {
            m_pool.Push(netobj);
        }
    }
}
