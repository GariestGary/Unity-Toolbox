using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class MessageBase
    {
        public MonoBehaviour sender { get; private set; }
        public Message id { get; private set; }
        public object data { get; private set; }

        public MessageBase(MonoBehaviour sender, Message id, object data = null)
        {
            this.sender = sender;
            this.id = id;
            this.data = data;
        }
    }
}