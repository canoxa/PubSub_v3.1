using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    public interface PubInterface {

        void publish(string message);

    }

    public interface SubInterface {

        void subscribe(string topico);
        void unsubscribe(string topico);

    }

    public interface PuppetInterface {
        void createProcess(TreeNode t, string role, string n, string s, string u);
    }
}
