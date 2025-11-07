#if VK
using System;
using UnityEngine;

namespace BloomLines
{
    public class VKHandle : MonoBehaviour
    {
        public Action<string> onGetStorage;

        public void StorageGetResult(string result)
        {
            onGetStorage?.Invoke(result);
        }

        public void SendResult(string text)
        {
            //Ответ от ВК на вызов CustomSend (BannerShow)
            //Инициализируется в index.html
            Debug.Log("Vk.Banner result: " + text);
        }
    }
}
#endif