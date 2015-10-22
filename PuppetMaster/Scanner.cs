using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    class Scanner
    {

        //estruturas para optimizar procura
        private Dictionary<string, string> pname_site = new Dictionary<string, string>();
        private Dictionary<string, TreeNode> site_treeNode = new Dictionary<string, TreeNode>();
        private Dictionary<TreeNode, Broker> node_broker = new Dictionary<TreeNode, Broker>();

        public Dictionary<string, string> getPname_site()
        {
            return this.pname_site;
        }
        public Dictionary<string, TreeNode> getSite_Node()
        {
            return this.site_treeNode;
        }
        public Dictionary<TreeNode, Broker> getNode_Broker()
        {
            return this.node_broker;
        }


        //actualiza site_node
        public TreeNode getRootNodeFromFile(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (string line in lines)
            {
                if (line.Contains("Parent") && line.Contains("none"))
                {
                    string[] words = line.Split(' ');
                    TreeNode root = new TreeNode(words[1]);
                    site_treeNode.Add(words[1], root);
                    return root;
                }
            }
            return null; //em principio nao chega aqui
        }


        //actualiza node_broker + site_name
        public List<MyProcess> fillProcessList(string v, TreeNode root)
        {
            PuppetInterface myremote;

            string[] lines = System.IO.File.ReadAllLines(v);
            List<MyProcess> res = new List<MyProcess>();

            foreach (string line in lines)
            {
                if (line.Contains("Is broker"))
                {
                    string[] words = line.Split(' '); //words[1]-name, words[5]-site, words[7]-url
                    TreeNode t = site_treeNode[words[5]];

                    string urlService = words[7].Substring(0, words[7].Length - 6);


                    myremote = (PuppetInterface)Activator.GetObject(typeof(PuppetInterface),urlService+"PuppetMasterURL");
                    myremote.createProcess(t, "broker", words[1], words[5], words[7]);

                    //actualizar estruturas
                    Broker aux = new Broker(words[1], words[5], words[7]);
                    t.setBroker(aux);
                    pname_site.Add(words[1], words[5]);
                    node_broker.Add(t, aux);
                    res.Add(aux);
                }
                if (line.Contains("Is publisher"))
                {
                    string[] words = line.Split(' '); //words[1]-name, words[5]-site, words[7]-url


                    TreeNode t = site_treeNode[words[5]];

                    myremote = (PuppetInterface)Activator.GetObject(typeof(PuppetInterface), "PuppetMasterURL");
                    myremote.createProcess(t,"publisher", words[1], words[5], words[7]);

                    //actualizar
                    Broker b = findBroker(words[5]);
                    Publisher aux = new Publisher(words[1], words[5], words[7], b);
                    t.addPublisher(aux);
                    pname_site.Add(words[1], words[5]);
                    res.Add(aux);
                }
                if (line.Contains("Is subscriber"))
                {
                    string[] words = line.Split(' '); //words[1]-name, words[5]-site, words[7]-url

                    TreeNode t = site_treeNode[words[5]];

                    myremote = (PuppetInterface)Activator.GetObject(typeof(PuppetInterface), "PuppetMasterURL");
                    myremote.createProcess(t,"publisher", words[1], words[5], words[7]);

                    //actualizar
                    Subscriber aux = new Subscriber(words[1], words[5], words[7]);
                    t.addSubscriber(aux);
                    pname_site.Add(words[1], words[5]);
                    res.Add(aux);
                }
            }
            return res;
        }

        //actualiza-se o site_node aqui
        public void readTreeFromFile(TreeNode root, string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (string line in lines)
            {
                if (line.Contains("Parent") && !line.Contains("none"))
                {
                    string[] words = line.Split(' '); //words[1]-filho, words[3]-pai

                    if (words[3].Equals(root.ID)) //root e o pai
                    {
                        TreeNode aux = new TreeNode(words[1]);
                        root.AddChild(aux);
                        site_treeNode.Add(words[1], aux);
                    }
                    else
                    { //temos de encontrar o pai, comecando a procura nos filhos do root
                        find(root, words[1], words[3]);
                    }
                }
            }
        }

        //actualiza site_node ( readTreeFromFile() )
        private void find(TreeNode no, string filho, string pai)
        {
            List<TreeNode> filhos = no.GetChildren();
            if (filhos != null)
            {
                foreach (var child in filhos)
                {
                    if (child.ID.Equals(pai))
                    { //child e o pai que estavamos a procura
                        TreeNode aux = new TreeNode(filho);
                        child.AddChild(aux);
                        site_treeNode.Add(filho, aux);
                    }
                }
                //pai nao esta nos filhos de "no"
                foreach (var newnode in filhos)
                { //tentar encontrar pai comecando a procura em cada filho de "no"
                    find(newnode, filho, pai);
                }
            }
        }

        private Broker findBroker(string site)
        {
            return node_broker[site_treeNode[site]];
        }
    }
}
