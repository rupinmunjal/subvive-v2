using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    public PlayerMovement movement;

    public GameObject camera;
    
    void Start()
    {
        if (photonView.IsMine)
        {
            IsLocalPlayer();
        }
        else
        {
            IsRemotePlayer();
        }
    }
    
    public void IsLocalPlayer()
    {
        movement.enabled = true;
        camera.SetActive(true);
    }

    public void IsRemotePlayer()
    {
        movement.enabled = false;
        camera.SetActive(false);
    }
}
