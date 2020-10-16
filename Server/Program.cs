using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(4, 4);
            ThreadPool.SetMaxThreads(4, 4);

            AutoResetEvent autoEvent = new AutoResetEvent(false);
            
                       
            autoEvent.WaitOne();
            Console.ReadKey();
        }

    }


}
