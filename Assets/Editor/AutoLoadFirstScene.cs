using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace BloomLines.Editor
{
    // Скрипт для запуска нужной первой сцены из едитора юнити
    [InitializeOnLoad]
    public class AutoLoadFirstScene : EditorWindow
    {
        private const string StartSceneKey = "PlayModeStartScenePath";
        private static SceneAsset startScene;

        static AutoLoadFirstScene()
        {
            ApplySavedStartScene();
        }

        private void OnGUI()
        {
            startScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"), startScene, typeof(SceneAsset), false);

            if (GUI.changed)
            {
                if (startScene != null)
                {
                    string scenePath = AssetDatabase.GetAssetPath(startScene);
                    EditorPrefs.SetString(StartSceneKey, scenePath);
                }
                else
                {
                    EditorPrefs.DeleteKey(StartSceneKey);
                }
            }

            if (startScene != null)
            {
                EditorSceneManager.playModeStartScene = startScene;
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }

        [MenuItem("Tools/PlayModeStartScene")]
        private static void Open()
        {
            GetWindow<AutoLoadFirstScene>();
        }

        private static void ApplySavedStartScene()
        {
            string savedScenePath = EditorPrefs.GetString(StartSceneKey, string.Empty);
            if (!string.IsNullOrEmpty(savedScenePath))
            {
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(savedScenePath);
                if (sceneAsset != null)
                {
                    startScene = sceneAsset;
                    EditorSceneManager.playModeStartScene = sceneAsset;
                }
                else
                {
                    Debug.LogWarning($"Сцена по пути {savedScenePath} не найдена. Возможно, файл был удалён или перемещён.");
                }
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }
    }
}