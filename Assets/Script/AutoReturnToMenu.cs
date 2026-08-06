using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class AutoReturnToMenu : MonoBehaviour
{
    public string sceneToLoad = "HomeScene";
    public float delay = 8f;

    void Start()
    {
        Invoke(nameof(ReturnToMenu), delay);
    }

    private void ReturnToMenu()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        SceneManager.LoadScene(sceneToLoad);
    }
}
