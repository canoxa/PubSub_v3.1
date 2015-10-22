using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    public class Publisher : MyProcess
    {
        //private Broker broker;//broker do seu site

        public Publisher(string u, string n, string s/*, Broker b*/) : base(u, n, s)
        {
            //broker = b;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("@publisher !!! porto -> {0}", args[0]);

            string[] arguments = args[0].Split(';');//arguments[0]->port; arguments[1]->url; arguments[2]->nome; arguments[3]->site;

            TcpChannel channel = new TcpChannel(Int32.Parse(arguments[0]));
            ChannelServices.RegisterChannel(channel, true);

            MPMPubImplementation MPMpublish = new MPMPubImplementation();
            RemotingServices.Marshal(MPMpublish, "PuppetMasterURL", typeof(MPMPubImplementation));
            
            Console.ReadLine();
        }

    }
    class MPMPubImplementation : MarshalByRefObject, PubInterface
    {
        public void publish(string number, string topic, string secs)
        {
            Console.WriteLine("@MPMPubImplementation - publishing {0} events, on topic {1}",number, topic);
        }
    }
}
