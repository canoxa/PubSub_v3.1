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
    public class Subscriber : MyProcess
    {
        //private Broker broker;

        public Subscriber(string u, string n, string s/* Broker b*/) : base(u, n, s)
        {
            //broker = b;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("@Subscriber !!! args -> {0}", args[0]);

            string[] arguments = args[0].Split(';');//arguments[0]->port; arguments[1]->url; arguments[2]->nome; arguments[3]->site;
            
            TcpChannel channel = new TcpChannel(Int32.Parse(arguments[0]));
            ChannelServices.RegisterChannel(channel, true);

            MPMSubImplementation subUnsub = new MPMSubImplementation();
            RemotingServices.Marshal(subUnsub, "MPMSubUnsub", typeof(MPMSubImplementation));
            
            Console.ReadLine();
        }

        public void entregaEvento(Message m)
        {
            Console.WriteLine("O subscriber {0} recebeu uma msg no topico {1}, com conteudo {2}", this.Name, m.Topic, m.Content);
        }

    }

    class MPMSubImplementation : MarshalByRefObject, SubInterface
    {
        public void subscribe(string topic)
        {
            Console.WriteLine("@MPMSubImplementatio - subscribing on topic {0}", topic);
        }

        public void unsubscribe(string topic)
        {
            Console.WriteLine("@MPMSubImplementatio - subscribing on topic {0}", topic);
        }
    }
}

