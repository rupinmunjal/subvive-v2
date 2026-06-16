using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
   public string sceneToLoad;



    public void ChangeSceneNow()
    {
        SceneManager.LoadScene(sceneToLoad);
    }


    public void QuitGame()
    {
        Debug.Log("Game Closed");

        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }


}
