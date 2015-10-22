using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PubSub
{
    static class PuppetMaster
    {
        
        private static string conf_filename = @"C:\DAD\PubSub_v3.1\example.txt";

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            TcpChannel channel = new TcpChannel(Int32.Parse(args[0]));
            ChannelServices.RegisterChannel(channel, true);

            PMcreateProcess createProcess = new PMcreateProcess(Int32.Parse(args[0]));
            RemotingServices.Marshal(createProcess, "PuppetMasterURL", typeof(PMcreateProcess));

            /*
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(PMcreateProcess),
                "PuppetMasterURL",
                WellKnownObjectMode.Singleton);*/


            //lancar slaves
            for (int i = 0; i < 2; i++) {

                ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\DAD\PubSub_v3.1\localPM\bin\Debug\localPM.exe");
                int port = 9000 + (i*100);
                string arg = port.ToString();
                startInfo.Arguments = arg;

                Process p = new Process();
                p.StartInfo = startInfo;
                
                p.Start();
               
            }

            Scanner scan = new Scanner();
            
            TreeNode root = scan.getRootNodeFromFile(conf_filename);

            //criar arvore a partir de root
            scan.readTreeFromFile(root, conf_filename);
            

            //preencher lstProcess - lista de todos os processos no config file
            List<MyProcess> lstProcess = scan.fillProcessList(conf_filename, root);
            

            //estruturas para optimizar a procura
            Dictionary<string, string> pname_site = scan.getPname_site();
            Dictionary<string, TreeNode> site_treeNode = scan.getSite_Node();
            Dictionary<TreeNode, Broker> node_broker = scan.getNode_Broker();

            MessageBox.Show("finito");
        }
    }

    class PMcreateProcess : MarshalByRefObject, PuppetInterface
    {
        int portCounter;

        public PMcreateProcess(int pC)
        {
            portCounter = pC;
        }

        public void ping(string m) { MessageBox.Show("ping :" + m); }
        public void createProcess(TreeNode site, string role, string name, string s, string url)
        {
            string aux = "MasterPMcreateProcess @ url -> " + url + " site -> " + s;
            MessageBox.Show(aux);
            if (role.Equals("broker"))
            {
                Broker b = new Broker(url, name, s);
                site.setBroker(b);

                string port = (portCounter++).ToString();

                //Process.Start()
                ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\DAD\PubSub_v3.1\Broker\bin\Debug\Broker.exe");
                string[] args = { port, url, name, s};
                startInfo.Arguments = String.Join(" ", args);

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
                startInfo.Arguments = String.Join(" ", args);

                Process p = new Process();
                p.StartInfo = startInfo;

                p.Start();
            }
            if (role.Equals("publisher"))
            {
                Publisher p = new Publisher(url, name, s/*,site.getBroker()*/);
                //site.addPublisher(p);

                string port = (portCounter++).ToString();

                ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\DAD\PubSub_v3.1\Publisher\bin\Debug\Publisher.exe");
                string[] args = { port, url, name, s };
                startInfo.Arguments = String.Join(" ", args);

                Process pro = new Process();
                pro.StartInfo = startInfo;

                pro.Start();
            }

        }
    }
}
