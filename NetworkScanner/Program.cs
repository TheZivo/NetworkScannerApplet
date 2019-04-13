using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Following are used for project specifically
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Collections.Concurrent;

namespace NetworkScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            scanner("10.0.0");
        }

        static void scanner(string subnet)
        {

            Ping pingTest;
            PingReply reply;
            IPAddress addy;
            IPHostEntry host;

            var found = new ConcurrentDictionary<string, string>();
            var notFound = new ConcurrentDictionary<string, string>();
            

            Parallel.For(0, 256, (i) =>
            {
                string itSubnet = "." + i.ToString();
                pingTest = new Ping();
                reply = pingTest.Send(subnet + itSubnet, 300);
                
                if (reply.Status == IPStatus.Success)
                {
                    try
                    {
                        addy = IPAddress.Parse(subnet + itSubnet);
                        host = Dns.GetHostEntry(addy);
                        //for form use textboxname.AppendText(subnet + itSubnet + host.hostname.tostring())
                        //Console.WriteLine(subnet + itSubnet + host.HostName.ToString());
                        found.TryAdd(subnet + itSubnet, host.HostName.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + ": " + subnet + itSubnet);
                    }
                }
                else
                {
                    //Console.WriteLine(subnet + itSubnet + " Inactive");
                    notFound.TryAdd(itSubnet, subnet + itSubnet);
                }
            });

            
            Console.WriteLine("Found the following hosts: ");
            foreach(KeyValuePair<string, string> kvp in found.OrderBy(x => x.Key))
            {
                Console.WriteLine(kvp.Key + " " + kvp.Value);
            }
            Console.ReadLine();
        }
    }
}
