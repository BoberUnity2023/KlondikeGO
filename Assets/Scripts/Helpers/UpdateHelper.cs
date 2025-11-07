using System;
using UnityEngine;

namespace BloomLines.Helpers
{
    public class UpdateHelper : MonoBehaviour
    {
        public Action OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private static UpdateHelper _updateHelper;
        public static UpdateHelper Get()
        {
            if (_updateHelper == null)
                _updateHelper = FindAnyObjectByType<UpdateHelper>();

            if (_updateHelper == null)
            {
                var obj = new GameObject("UpdateHelper");
                _updateHelper = obj.AddComponent<UpdateHelper>();

                DontDestroyOnLoad(obj);
            }

            return _updateHelper;
        }
    }
}