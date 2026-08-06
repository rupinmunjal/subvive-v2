using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class LoadingScreenController : MonoBehaviour
{
    public string sceneToLoad = "Submarine";
    public VideoPlayer videoPlayer;
    public float minimumDisplayTime = 2f;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        float startTime = Time.time;

        if (videoPlayer != null)
            videoPlayer.Play();

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncOp.allowSceneActivation = false;

        while (asyncOp.progress < 0.9f)
            yield return null;

        float elapsed = Time.time - startTime;
        if (elapsed < minimumDisplayTime)
            yield return new WaitForSeconds(minimumDisplayTime - elapsed);

        asyncOp.allowSceneActivation = true;
    }
}
