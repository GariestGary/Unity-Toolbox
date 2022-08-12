using System;

namespace VolumeBox.Toolbox
{
    public class Subscriber
    {
        private Message msg;
        private Action<object> next;

        public Message id {get{return msg;} private set{}}
        public Action<object> react {get{return next;} private set{}}

        public Subscriber(Message msg, Action<object> next)
        {
            this.msg = msg;
            this.next = next;
        }
    }
}