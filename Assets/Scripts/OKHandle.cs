#if OK
using System;
using UnityEngine;

namespace BloomLines
{
    public class OKHandle : MonoBehaviour
    {
        public Action<string> onGetStorage;
        public Action<string> onGetTargetPlatform;
        public Action<string> onGetCallback;

        public void OnGetStorage(string value)
        {
            onGetStorage?.Invoke(value);
        }

        public void OnGetTargetPlatform(string arg1)
        {
            onGetTargetPlatform?.Invoke(arg1);
        }

        public void OnGetCallback(string arg1)
        {
            onGetCallback?.Invoke(arg1);
        }
    }
}
#endif