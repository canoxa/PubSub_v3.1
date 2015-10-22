using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    public class Publisher : MyProcess
    {
        private Broker broker;//broker do seu site

        public Publisher(string u, string n, string s, Broker b) : base(u, n, s)
        {
            broker = b;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("@publisher !!! porto -> {0}", args[0]);


            Console.ReadLine();
        }

        public void publishEvent(string topic, int number, int interval)
        {
            for (int i = 1; i <= number; i++)
            {
                Message aux = new Message(topic, i.ToString());
                broker.publish(aux);
            }
        }
    }
}
