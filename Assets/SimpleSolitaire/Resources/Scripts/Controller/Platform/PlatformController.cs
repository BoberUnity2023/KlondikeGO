using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BloomLines
{
    public class PlatformController : MonoBehaviour
    {
        protected SaveController _saveController;
        protected int _loadedScene;
        public string Json { get; protected set; }
        public bool IsSaveLoaded { get; protected set; }

        private void Awake()
        {
#if !VK && !OK
    Destroy(gameObject);
#endif

#if VK || OK
            Debug.Log("VK/OK Awake()");
            if (IsSingle)
                DontDestroyOnLoad(gameObject);
            else
                Destroy(gameObject);
#endif
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            _loadedScene = scene.buildIndex;
            if (scene.name == "2_KlondikeGO")
            {
                _saveController = FindFirstObjectByType<SaveController>();

                if (_saveController == null)
                {
                    Debug.LogError("VK. SaveController == null");
                    return;
                }

                _saveController.OnGetStorageVK(Json);
            }
        }

        private bool IsSingle
        {
            get
            {
                PlatformController[] platformControllers = FindObjectsByType<PlatformController>(FindObjectsSortMode.None);

                if (platformControllers.Length > 1)
                    return false;
                else
                    return true;
            }
        }
    }
}
