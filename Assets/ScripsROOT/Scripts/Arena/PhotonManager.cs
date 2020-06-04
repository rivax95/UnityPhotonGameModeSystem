using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Photon.Chat;
using UnityEngine.UI;
using Alex.Arena.Managers.Room;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    string gameVersion = "1";
    public string NickPlayer;
    public int totalPlayers;
    public TypedLobby MyLobby;
    public static PhotonManager instance;

    RoomOptions MyOptions;

    public string NombreEscena;


    public GameObject LoadingCanvas;
    ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
    [Header("LoadingReferences")]
    public Image LoadingBar;
    public int totalRooms;
    public Text LoadingText;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
        MyOptions = new RoomOptions();
        PhotonNetwork.ConnectUsingSettings();
    }
    void Start()
    {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;

        PhotonNetwork.ConnectToBestCloudServer(); //Conéctese a la región de Photon Cloud con el ping más bajo (en plataformas que admiten Ping de Unity).

        SetPlayerName("User" + Random.Range(0, 100));

        properties.Add("Team", -1);
        properties.Add("Kills", 0);
        properties.Add("Death", 0);
        properties.Add("IsDeath", false);
        properties.Add("IsPlaying", false);
        properties.Add("Character", 0);
        properties.Add("Ping", PhotonNetwork.GetPing());
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);


    }
    public void StarMathc()
    {
       EstablecerRoomOptions(10);
       MyLoadRoom();
    }

    void Update()
    {
        totalPlayers = PhotonNetwork.CountOfPlayers;
        totalRooms = PhotonNetwork.CountOfRooms;
    }
    public void EstablecerRoomOptions(byte players)
    {
        MyOptions.MaxPlayers = players;
        MyOptions.IsVisible = true;
        MyOptions.IsOpen = true;
        MyOptions.PublishUserId = true;

    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        //Debug.Log("this is lobby");
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
        {
           
        }
    }
    public void MyLoadRoom()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        //load rule
        crearSala();

    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        if (MyLobby == null)
        {
            MyLobby = new TypedLobby("Default", LobbyType.Default);
        }
        //if(PhotonNetwork.LocalPlayer.UserId.)
        MyLobby.Type = LobbyType.SqlLobby;


        Debug.Log("Conectado al master Server");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconect" + cause.ToString());
        if (cause == DisconnectCause.DisconnectByServerReasonUnknown)
        {
            SceneManager.LoadScene("Login", LoadSceneMode.Single);
        }

    }
    public void crearSala()
    {
        //Debug.Log("Call");
        MyOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

        MyOptions.PlayerTtl = 30;
        MyOptions.EmptyRoomTtl = 15;
        PhotonNetwork.JoinOrCreateRoom(NombreEscena, MyOptions, MyLobby);
        ;
    }
    public void SetPlayerName(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;
        NickPlayer = value;

        PlayerPrefs.SetString(NickPlayer, value);
    }
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        base.OnFriendListUpdate(friendList);
     
        foreach (var item in friendList)
        {

        }
        Debug.Log("UpdateFrinds");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        //Debug.Log("Entro a sala :" + NombreEscena);
        if (NombreEscena == "" || NombreEscena == null)
        {
            NombreEscena = "ArenaPG";
        }
        StartCoroutine(AsynchronousLoad(NombreEscena));
        PhotonNetwork.IsMessageQueueRunning = false;
  
    }
    void OnLevelWasLoaded(int level) // cuando el nivel ha sido cargado
    {
        PhotonNetwork.IsMessageQueueRunning = true;

    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // cuando un jugador entra a la sala
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
      
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
        Destroy(this.gameObject);
    }

    IEnumerator AsynchronousLoad(string scene)
    {
        LoadingCanvas.SetActive(true);
        yield return null;
        Debug.LogError("Cargando");
        PhotonNetwork.LoadLevel(scene);
      

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {

            float progress = Mathf.Clamp01(PhotonNetwork.LevelLoadingProgress / 0.91f);
            LoadingBar.fillAmount = progress;
            LoadingText.text = (progress * 100).ToString("0") + " %";

            yield return null;
        }
    }

}