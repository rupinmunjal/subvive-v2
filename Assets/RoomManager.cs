using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player1;
    public GameObject player2;

    [Space]
    public Transform spawnPoint1;

    [Space]
    public Transform spawnPoint2;

    void Start()
    {
        Debug.Log("Connecting...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("Joined Lobby");

        PhotonNetwork.JoinOrCreateRoom(
            "test",
            new RoomOptions { MaxPlayers = 2 },
            TypedLobby.Default
        );
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("Joined Room");

        GameObject selectedSprite;
        Transform selectedSpawn;

        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            selectedSprite = player1;
            selectedSpawn = spawnPoint1;
        }
        else
        {
            selectedSprite = player2;
            selectedSpawn = spawnPoint2;
        }

        PhotonNetwork.Instantiate(
            selectedSprite.name,
            selectedSpawn.position,
            Quaternion.identity
        );
    }

    public Transform GetSpawnPoint(int actorNumber)
    {
        if (actorNumber == 1)
        {
            return spawnPoint1;
        }

        return spawnPoint2;
    }
}
