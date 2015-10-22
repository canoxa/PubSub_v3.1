using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("@subscriber !!! porto -> {0}", args[0]);


            Console.ReadLine();
        }

        public void entregaEvento(Message m)
        {
            Console.WriteLine("O subscriber {0} recebeu uma msg no topico {1}, com conteudo {2}", this.Name, m.Topic, m.Content);
        }
    }
}
