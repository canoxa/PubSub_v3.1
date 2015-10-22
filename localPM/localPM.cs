using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    class localPM
    {


        static void Main(string[] args)
        {
            Console.WriteLine("@localPM !!! porto -> {0}", args[0]);

            TcpChannel channel = new TcpChannel(Int32.Parse(args[0]));
            ChannelServices.RegisterChannel(channel, true);

            PMcreateProcess createProcess = new PMcreateProcess(Int32.Parse(args[0]));
            RemotingServices.Marshal(createProcess, "PuppetMasterURL", typeof(PMcreateProcess));

            Console.ReadLine();
        }
    }

    class PMcreateProcess : MarshalByRefObject, PuppetInterface
    {
        int portCounter;

        public PMcreateProcess(int pC)
        {
            portCounter = pC;
        }

        public void ping(string m) { Console.WriteLine("ping :" + m); }

        public void createProcess(TreeNode site, string role, string name, string s, string url)
        {

            string aux = "LocalPMcreateProcess @ url -> " + url + " site -> " + s;
            Console.WriteLine(aux);
            if (role.Equals("broker"))
            {
                Broker b = new Broker(url, name, s);
                site.setBroker(b);

                string port = (portCounter++).ToString();

                //Process.Start()
                ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\DAD\PubSub_v3.1\Broker\bin\Debug\Broker.exe");
                string[] args = { port, url, name, s };
                startInfo.Arguments = String.Join(";", args);

                Process p = new Process();
                p.StartInfo = startInfo;

                p.Start();
            }
            if (role.Equals("subscriber"))
            {
                Subscriber sub = new Subscriber(url, name, s);
                //site.addSubscriber(sub);

                string port = (portCounter++).ToString();

                ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\DAD\PubSub_v3.1\Subscriber\bin\Debug\Subscriber.exe");
                string[] args = { port, url, name, s };
                startInfo.Arguments = String.Join(";", args);

                Process p = new Process();
                p.StartInfo = startInfo;

                p.Start();
            }
            if (role.Equals("publisher"))
            {
                Publisher p = new Publisher(url, name, s/*, site.getBroker()*/);
                //site.addPublisher(p);

                string port = (portCounter++).ToString();

                ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\DAD\PubSub_v3.1\Publisher\bin\Debug\Publisher.exe");
                string[] args = { port, url, name, s };
                startInfo.Arguments = String.Join(";", args);

                Process pro = new Process();
                pro.StartInfo = startInfo;

                pro.Start();
            }

        }
    }
}
