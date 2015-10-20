using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PubSub
{
    static class PuppetMaster
    {
        
        private static string conf_filename = @"C:\Users\Lucília\Downloads\PubSub_v3-master\PubSub_v3-master\example.txt";

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(PMcreateProcess),
                "PuppetMasterURL",
                WellKnownObjectMode.Singleton);


            //lancar slaves
            for (int i = 0; i < 6; i++) {

                ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Users\Tiny\Documents\Visual Studio 2015\Projects\PubSub_v3.1\localPM\bin\Debug\localPM.exe");
                string arg = "tcp://1.1.1." + i.ToString() + ":8000/PuppetMasterURL";
                startInfo.Arguments = arg;

                Process p = new Process();
                p.StartInfo = startInfo;
                
                p.Start();
            }

            /*

            Scanner scan = new Scanner();
            
            TreeNode root = scan.getRootNodeFromFile(conf_filename);

            //criar arvore a partir de root
            scan.readTreeFromFile(root, conf_filename);
            MessageBox.Show("depois de fazer a tree");
            //preencher lstProcess - lista de todos os processos no config file
            List<MyProcess> lstProcess = scan.fillProcessList(conf_filename, root);
            MessageBox.Show("depois de fazer a lista");
            //estruturas para optimizar a procura
            Dictionary<string, string> pname_site = scan.getPname_site();
            Dictionary<string, TreeNode> site_treeNode = scan.getSite_Node();
            Dictionary<TreeNode, Broker> node_broker = scan.getNode_Broker();
            */
            MessageBox.Show("finito");
        }
    }

    class PMcreateProcess : MarshalByRefObject, PuppetInterface
    {
        public void createProcess(TreeNode site, string role, string name, string s, string url)
        {
            string aux = "PMcreateProcess @ " + site.ID + " role " + role;
            MessageBox.Show(aux);
            if (role.Equals("broker")) {
                Broker b = new Broker(url, name, s);
                site.setBroker(b);
            }
            if (role.Equals("subscriber"))
            {
                Subscriber sub = new Subscriber(url, name, s);
                site.addSubscriber(sub);
            }
            if (role.Equals("publisher"))
            {
                Publisher p = new Publisher(url, name, s,site.getBroker());
                site.addPublisher(p);
            }

        }
    }
}
