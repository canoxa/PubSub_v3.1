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
        private Dictionary<Subscriber, string> lstSubsTopic; //quem subscreveu neste no, a quE
        private Dictionary<Broker, string> routingTable; //vizinho,subscrições atingiveis atraves desse vizinho

        public Broker(string u, string n, string s) : base(u, n, s)
        {
            lstVizinhos = new List<Broker>();
            lstSubsTopic = new Dictionary<Subscriber, string>();
            routingTable = new Dictionary<Broker, string>();

        }

        public void publish(Message aux)//chamada pelo Publisher
        {

            foreach (KeyValuePair<Subscriber, string> t in lstSubsTopic)
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

            foreach (KeyValuePair<Subscriber, string> t in lstSubsTopic)
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

        private void notify(Subscriber s, Message m)
        {
            s.entregaEvento(m);
        }

        public List<Broker> getVizinhos()
        {
            return lstVizinhos;
        }
        public Dictionary<Subscriber, string> getSubsTopic()
        {
            return lstSubsTopic;
        }
        public Dictionary<Broker, string> getRouting()
        {
            return routingTable;
        }


    }
}
