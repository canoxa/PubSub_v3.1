using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    public class Message
    {
        private string topic;
        private string content;

        public string Topic
        {
            get { return topic; }
            set { topic = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public Message(string t, string c)
        {
            topic = t;
            content = c;
        }

    }
}
