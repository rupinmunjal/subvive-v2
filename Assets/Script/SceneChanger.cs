using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Optional scene selected in Inspector")]
    public string sceneToLoad;

    [Header("Transition delay")]
    public float delay = 0.15f;

    public void ChangeSceneNow()
    {
        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError("No scene has been assigned to Scene To Load.");
            return;
        }

        StartCoroutine(LoadAfterDelay(sceneToLoad));
    }

    private IEnumerator LoadAfterDelay(string targetScene)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(targetScene);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Tutorial1");
    }

    public void OpenTutorial1()
    {
        SceneManager.LoadScene("Tutorial1");
    }

    public void OpenTutorial2()
    {
        SceneManager.LoadScene("Tutorial2");
    }

    public void OpenTutorial3()
    {
        SceneManager.LoadScene("Tutorial3");
    }

    public void SkipTutorial()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void QuitGame()
    {
        StartCoroutine(QuitAfterDelay());
    }

    private IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Game Closed");
        Application.Quit();
    }
}