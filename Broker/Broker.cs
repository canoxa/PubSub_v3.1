using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    [Serializable]
    public class Broker : MyProcess
    {
        private List<Broker> lstVizinhos;
        private Dictionary<string, string> lstSubsTopic; //quem subscreveu neste no, a quE
        private Dictionary<Broker, string> routingTable; //vizinho,subscrições atingiveis atraves desse vizinho

        public Broker(string u, string n, string s) : base(u, n, s)
        {
            lstVizinhos = new List<Broker>();
            lstSubsTopic = new Dictionary<string, string>();
            routingTable = new Dictionary<Broker, string>();

        }

        static void Main(string[] args)
        {
            Console.WriteLine("@broker !!! url -> {0}", args[0]);
           

            Console.ReadLine();
        }

        public void publish(Message aux)//chamada pelo Publisher
        {

            foreach (KeyValuePair<string, string> t in lstSubsTopic)
            {
                if (aux.Topic.Equals(t.Value))
                {
                    notify(t.Key, aux);
                }
            }

            //if metodo de routing == flooding
            foreach (var viz in lstVizinhos)
            {
                floodMsg(aux, this);
            }
        }

        private void floodMsg(Message aux, Broker broker)//chamada pelos brokers
        {

            foreach (KeyValuePair<string, string> t in lstSubsTopic)
            {
                if (aux.Topic.Equals(t.Value))
                {
                    notify(t.Key, aux);
                }
            }

            List<Broker> lst = lstVizinhos;
            lst.Remove(broker);
            //propagar para os outros todos
            foreach (var viz in lst)
            {
                floodMsg(aux, this);
            }
        }

        private void notify(string s, Message m)
        {
            //s.entregaEvento(m);
        }

        public List<Broker> getVizinhos()
        {
            return lstVizinhos;
        }
        public Dictionary<string, string> getSubsTopic()
        {
            return lstSubsTopic;
        }
        public Dictionary<Broker, string> getRouting()
        {
            return routingTable;
        }


    }
}
