using System;
using System.Net.Sockets;
using System.Threading;

namespace ClassifiedAds.Infrastructure.HealthChecks
{
    public class NetworkPortCheck
    {
        public static bool IsReady(string address)
        {
            var parts = address.Split(':');
            string host = parts[0];
            int port = int.Parse(parts[1]);

            try
            {
                TcpClient client = new TcpClient(host, port);
                return client.Connected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to {address} :" + ex.Message);
                return false;
            }
        }

        public static void Wait(string address, int retries = 0, int timeOut = 30000)
        {
            int count = 0;
            do
            {
                var isReady = IsReady(address);
                if (isReady)
                {
                    return;
                }

                count++;
                Thread.Sleep(timeOut);
            }
            while (count <= retries);
        }
    }
}
