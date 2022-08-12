using NaughtyAttributes;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class MessageSender: MonoCached
    {
        [SerializeField] private Message message;
        [SerializeField] private bool sendOnRise;
        [SerializeField] private bool bindToGameObjectActiveState;
        [SerializeField] private DataType sendData;
        [SerializeField] [ShowIf("sendFloat")] private float floatData;
        [SerializeField] [ShowIf("sendInt")]private int intData;
        [SerializeField] [ShowIf("sendString")]private string stringData;
        [SerializeField] [ShowIf("sendMono")] private MonoBehaviour monoData;
        [Inject] private Messager msg;

        private bool sendFloat => sendData == DataType.Float;
        private bool sendInt => sendData == DataType.Int;
        private bool sendString => sendData == DataType.String;
        private bool sendMono => sendData == DataType.Mono;

        public override void Rise()
        {
            if(sendOnRise)
            {
                Send();
            }
        }

        [ContextMenu("Send")]
        [Button("Send")]
        public void Send()
        {
            if(bindToGameObjectActiveState)
            {
                if(!gameObject.activeSelf)
                {
                    return;
                }
            }

            object data = null;

            switch(sendData)
            {
                case DataType.Int:
                    data = intData;
                    break;
                case DataType.Float:
                    data = floatData;
                    break;
                case DataType.String:
                    data = stringData;
                    break;
                case DataType.Mono:
                    data = monoData;
                    break;
                default:
                    data = null;
                    break;
            }
            
            Messager.Instance.Send(message, data);
        }

        private enum DataType
        {
            None,
            Int,
            Float,
            String,
            Mono,
        }
    }
    
}