using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void PressLoadScene(int index)
    {
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }
}

