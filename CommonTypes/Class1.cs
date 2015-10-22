using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    public interface PubInterface {//usada pelo MPM

        void publish(string number,string topic, string secs);

    }

    public interface SubInterface {//usado pelo MPM

        void subscribe(string topic);
        void unsubscribe(string topic);

    }

    public interface PuppetInterface {//usada pelo MPM
        void createProcess(TreeNode t, string role, string n, string s, string u);
        void ping(string m);
    }

    public interface SubscriberNotify {//usada pelo Broker
        void notify(Message m);
    }

    public interface BrokerReceivePub {//usada pelo Pub
        void receivePublication(Message m);
    }

    public interface BrokerReceiveSubUnSub//usada pelo Sub
    {
        void receiveSub(string topic);
        void receiveUnsub(string topic);
    }

    public interface BrokerReceiveBroker//usada pelo Broker
    {
        void forwardFlood(Message m,Broker b);
        void forwardFilter(Message m, Broker b);
        void forwardSub(string topic,string nomeSub);
        void forwardUnsub(string topic, string nomeSub);
    }

}
