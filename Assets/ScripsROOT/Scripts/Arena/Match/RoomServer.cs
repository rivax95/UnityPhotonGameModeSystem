//                                          ▂ ▃ ▅ ▆ █ ARC █ ▆ ▅ ▃ ▂ 
//                                        ..........<(+_+)>...........
// RoomServer.cs (31/01/20)
//Autor: Alejandro Rivas                 alejandrotejemundos@hotmail.es
//Desc: Gestiona la partida. Practicamente actua de servidor para gestionar eventos que llegan y sincronizar la partida con el resto de jugadores
//Mod : 18/03/2020 Version No testada - Correcion de Errores / Limpieza y organizacion del codigo / TeamRegister
//Rev V2.0 
//..............................................................................................\\
//------------------------Anotaciones---------------------
//Todos los registros del TeamRegister.singleton.asignarequipos debe de estar actualizado cada vez que sale o entra un jugador y en la primer metodo de OnJoinRoom

#region Librerias
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;
using Alex.Arena.ScoreObject;
using Alex.Arena.MTimer;
using Alex.Arena.IMatch;
using Alex.Arena.ThaidersProperties;
using ExitGames.Client.Photon;
using UnityEngine.Events;
using Alex.Arena.MatchEvent;
using Alex.Arena.IMatch.Interface;
#endregion

#region Enums
public enum FillTeams
{
    First_FillTeam,
    OneAOne
}

public enum TypeMatch
{
    Goal
}
#endregion
namespace Alex.Arena.Managers.Room
{

    [RequireComponent(typeof(PhotonView))]
    [System.Serializable]
    public class RoomServer : MonoBehaviourPun, IPunObservable, IOnEventCallback, IInRoomCallbacks
    {
        #region Variables
        [Header("Referencias")]

        private GameObject Actor;
        public GameObject FatherCharacterSelection;
        public GameObject ContainerEvents;
        public static RoomServer Singleton;
        public MatchGenericsEvents MGV;
        [Space(10)]
        [Header("Variables de control")]
        public FillTeams ModeFill=FillTeams.OneAOne;
       

        private MatchRegister matchRegister;
        public bool OfLineMode;

        
        [HideInInspector]
        public GenericResp resp;

        private GenericResp Resp {
            get
            {
                if (resp == null)
                {
                    
                    Dictionary<Transform, int> DictOfRespawns = new Dictionary<Transform, int>();
                    foreach (TeamRoom item in TeamRegister.Singleton.GetList())
                    {
                        for (int i = 0; i < item.SpawnFatherPoint.transform.childCount; i++)
                        {

                            DictOfRespawns.Add(item.SpawnFatherPoint.transform.GetChild(i).transform, item.TeamID);
                       
                        }
                    }
                    GenericResp retorno = new GenericResp(DictOfRespawns,ref OnPlayerSpawning);

                    resp = retorno;

                }
                resp.OnPlayerSpawn = OnPlayerSpawning;
                return resp;
            }

        }
        [SerializeField]
        public TypeMatch ModeMatch { get => modeMatch; set {
                modeMatch = value;
                OnChangeMatchEvent(modeMatch);
            }  }
        [SerializeField]
        private TypeMatch modeMatch = TypeMatch.Goal;

        private Nullable< TypeMatch >LastMode=null;

        private const byte CharSelect_Event = 0;
        public int maxTeams;

         private int MaxJugadoresPerTeam;
        public int AmmountCharacters = 8;

        private const byte InSelect = 2;
        private const byte EndSelect = 3;

        private const byte InMatch = 4;
        private const byte EndMatch = 5;

        private const byte UpdateUI = 6;
        private const byte PlayerDeath=7;

        private Score Marcadores; 

        private Timer MatchTimer;

        private Timer SelectorTimer;

        private static Action<int,int> OnPoint = delegate { };

        private static Action OnPlayerSpawning = delegate { };

        private static Action<Player,Player> OnPlayerDead = delegate { };


        private static Action OnInitMatch = delegate { };

        private static Action OnFinishTimeMatch = delegate { };

        private static Action OnStarMatch = delegate { };

        private static Action<string> OnChangeTimeMatch = delegate { };

        private static Action OnInitSelector = delegate { };

        private static Action<string> OnChangeTimeSelector = delegate { };

        private static Action OnFinishTimeSelector = delegate { };

        private static Action OnTeamSelect = delegate { };

        private static Action OnChangeMode = delegate { };

        private static Action OnJoiningRoom = delegate { };

        private UnityAction<int> ChangeSpawn;

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

        private Coroutine RespCorutine;

        private Dictionary<MatchBase, IEventMatch> IEvents = new Dictionary<MatchBase, IEventMatch>();


        [Header("Tiempos")]
        public int TiempoMatch;
        public int TimeToRespawn = 8;
        public int TimeToGoRespawn = 4;
        public int TimeToSelectPlayerOnInitMatch = 30;
        //public bool InitMathc;
        private bool MathcInPlay = false;
        private bool FullPlayers = false;


        #endregion

        #region Ininicializadores
       
        void Awake()
        {
            
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
            PhotonNetwork.OfflineMode = OfLineMode;

            properties.Add(RefProperties.Team, -1);
            properties.Add(RefProperties.Kills, 0);
            properties.Add(RefProperties.Death, 0);
            properties.Add(RefProperties.IsDeath, false);
            properties.Add(RefProperties.ReadyForBattel, false);
            properties.Add(RefProperties.Ping, PhotonNetwork.GetPing());
            properties.Add(RefProperties.CharacterID, -1);
            properties.Add(RefProperties.IsDisqualified, false);

            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

            maxTeams = TeamRegister.Singleton.AmmountTeams;
            Match.AmmountTeams = maxTeams;
        }

        void Start()
        {
          
            OnJoinRoom();

            CanvasManager.Singleton.ChangeListenersButtons(StartRoom_Listener);
            MatchTimer = new Timer(TiempoMatch == 0 ? 10 : TiempoMatch, EndMatch_RiseEvent, OnChangeTimeMatch, InMatch_RiseEvent);

            SelectorTimer = new Timer(TimeToSelectPlayerOnInitMatch, EndSelect_RiseEvent, OnChangeTimeSelector, InSelect_RiseEvent);
            Marcadores = new Score(maxTeams, OnPoint);

            photonView.ObservedComponents.Add(MatchTimer);
            photonView.ObservedComponents.Add(Marcadores);
            photonView.ObservedComponents.Add(SelectorTimer);
          
        }

        #endregion

        void Update()
        {
          
            if (Input.GetKeyDown(KeyCode.M))
            {
                // OnSpawnEvent();
                //CharacterSelection_RiseEvent();
                // int T = Match.GetTeam();
                //  Marcadores.SetScore(T, Marcadores.GetScore(T) + 1);
                RespawnDied(null);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                int T = Match.GetTeam();
                Marcadores.SetScore(T, Marcadores.GetScore(T) + 1);
            }

            if (!PhotonNetwork.IsMasterClient) return;
            if (TeamRegister.Singleton.CheckFillTeams()) FullPlayers = true;

            if (FullPlayers){
                if (MathcInPlay)
                {
                    MatchTimer.Run();
                }
                else
                {
                    SelectorTimer.Run();

                }
            }
           
        }

        #region Delegados

        private void OnEnable()
        {
            
            PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
            
            OnInitSelector += () => {
                if (Actor)
                {
                    PhotonNetwork.Destroy(Actor);
                }
                CanvasManager.Singleton.ChangeListenersButtons(CharacterSelect);
                CanvasManager.Singleton.ShowPlayerSelector();

                if (RespCorutine!=null)
                {
                    StopCoroutine(RespCorutine);
                }

            };
            
            CanvasManager.Singleton.SetTime_Selector("0:00");
            OnTeamSelect += CharacterSelection_RiseEvent;

            OnChangeTimeSelector += CanvasManager.Singleton.SetTime_Selector;
            OnFinishTimeSelector += StartMatch;
            OnFinishTimeSelector += CanvasManager.Singleton.HidePlayerSelector;

            OnPoint += CanvasManager.Singleton.SetScore;
            OnChangeTimeMatch += CanvasManager.Singleton.SetTime_UI;

            OnFinishTimeMatch += MGV.OnEndMatch;
            OnFinishTimeMatch += CanvasManager.Singleton.HidePlayerSelector;

            OnJoiningRoom += MGV.OnJoinArena;
            OnJoiningRoom += CanvasManager.Singleton.UpdatePlayerSelector;

            OnPlayerSpawning += MGV.OnPlayerSpawn;

            OnJoiningRoom += CanvasManager.Singleton.Init;

            OnPlayerDead += CanvasManager.Singleton.UpdatePlayersDead;
        }

        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
            PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this);
            OnInitSelector -= () => {
                if (Actor)
                {
                    PhotonNetwork.Destroy(Actor);
                }
                CanvasManager.Singleton.ChangeListenersButtons(CharacterSelect);
                CanvasManager.Singleton.ShowPlayerSelector();

                if (RespCorutine != null)
                {
                    StopCoroutine(RespCorutine);
                }

            };

            OnChangeTimeMatch -= CanvasManager.Singleton.SetTime_UI;
            OnTeamSelect -= CharacterSelection_RiseEvent;

            OnChangeTimeSelector -= CanvasManager.Singleton.SetTime_Selector;
            OnFinishTimeSelector -= StartMatch;
            OnFinishTimeSelector -= CanvasManager.Singleton.HidePlayerSelector;

            OnPoint -= CanvasManager.Singleton.SetScore;


            OnFinishTimeMatch -= MGV.OnEndMatch;
            OnFinishTimeMatch -= CanvasManager.Singleton.HidePlayerSelector;

            OnJoiningRoom -= MGV.OnJoinArena;
            OnJoiningRoom -= CanvasManager.Singleton.UpdatePlayerSelector;

            OnPlayerSpawning -= MGV.OnPlayerSpawn;

            OnJoiningRoom -= CanvasManager.Singleton.Init;

            OnPlayerDead -= CanvasManager.Singleton.UpdatePlayersDead;
        }

      

        #endregion

        //Enable and Disable (subscriptores de acciones)
        #region Photon Events && Photon Serialize (Streaming)

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            stream.Serialize(ref maxTeams);
            stream.Serialize(ref MathcInPlay);
            stream.Serialize(ref FullPlayers);
        }
        /// <summary>
        /// Envia el Evento que actualiza los jugadores que ahi disponibles
        /// </summary>
        public void CharacterSelection_RiseEvent()
        {
            int team = (int)PhotonNetwork.LocalPlayer.CustomProperties[RefProperties.Team];

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
            SendOptions sendOptions = new SendOptions { Reliability = true };

            object[] data = new object[] { team };
            PhotonNetwork.RaiseEvent(CharSelect_Event, data, raiseEventOptions, sendOptions);
    
        }

      

        public void InSelect_RiseEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };

          
            PhotonNetwork.RaiseEvent(InSelect, null, raiseEventOptions, sendOptions);
        }

        public void EndSelect_RiseEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };

           
            PhotonNetwork.RaiseEvent(EndSelect, null, raiseEventOptions, sendOptions);
        }

        public void InMatch_RiseEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            
            PhotonNetwork.RaiseEvent(InMatch, null, raiseEventOptions, sendOptions);
        }

        public void EndMatch_RiseEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };

         
            PhotonNetwork.RaiseEvent(EndMatch, null, raiseEventOptions, sendOptions);

        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == CharSelect_Event)
            {

                object[] Datas = (object[])photonEvent.CustomData;
                int team = (int)Datas[0];
                if (team != Match.GetTeam()) return;

                Match.UpdateCharactesrAvaliables(ref FatherCharacterSelection, team);

                CanvasManager.Singleton.UpdatePlayersCharacters();
               
            }

            if(photonEvent.Code == InSelect)
            {
                OnInitSelector();
            }

            if (photonEvent.Code == EndSelect)
            {
                OnFinishTimeSelector();
            }

            if (photonEvent.Code == InMatch)
            {
                OnInitMatch();
            }

            if (photonEvent.Code == EndMatch)
            {
                OnFinishTimeMatch();
            }

            if(photonEvent.Code == UpdateUI)
            {
                CanvasManager.Singleton.UpdatePlayersCharacters();
            }

        }

        #endregion

        //Metodos para construir la logica de la room
        #region Metodos locales del Manager
        /// <summary>
        /// Evento para inicializar el match una vez que acaba el tiempo de inicio
        /// </summary>
        private void StartMatch()
        {

            MathcInPlay = true;

            Match.ClearProperties();

            OnSpawnEvent();

            OnStarMatch();

        }

        /// <summary>
        /// Marca el nuevo id Del Character
        /// </summary>
        /// <param name="CharacterId"></param>
        public void CharacterSelect(int CharacterId)
        {
            Match.SetCharacterID(CharacterId);
            OnTeamSelect();
        }

        /// <summary>
        /// Eventos de Spawn, encapsula la logica del spawn inicial
        /// </summary>
        private void OnSpawnEvent()
        {

            int actorid = PhotonNetwork.LocalPlayer.ActorNumber;
            int Char = Match.GetCharacterID(actorid);
            if (Char == -1)
            {
                List<int> Disponibles = Match.CharAvaliable(AmmountCharacters);

                List<int> Actores = PhotonNetwork.PlayerList.Where(x =>
                x.CustomProperties[RefProperties.Team] == PhotonNetwork.LocalPlayer.CustomProperties[RefProperties.Team]).
                OrderBy(p=>p.ActorNumber).
                Select(p => p.ActorNumber).ToList();

                int Index = Actores.Where(p => p == PhotonNetwork.LocalPlayer.ActorNumber).Single();

                Debug.LogError("El Actor local ha seleccionado el Character : " + Index);

                Char = Disponibles[Index];
                CharacterSelect(Char);

            }

            Resp.SetRespawn(Match.GetTeam(PhotonNetwork.LocalPlayer.ActorNumber), PhotonNetwork.LocalPlayer.ActorNumber);

            Resp.Spawn(actorid, ref Actor);
        }

        /// <summary>
        /// Una cofiguracion inicial para el spawn antes de arrancar el match
        /// </summary>
        /// <param name="CharacterId"></param>
        private void StartRoom_Listener(int CharacterId)
        {
            CharacterSelect(CharacterId);
            CanvasManager.Singleton.HidePlayerSelector();
            Resp.Spawn(PhotonNetwork.LocalPlayer.ActorNumber, ref Actor);
        }

        /// <summary>
        /// Se limita a hacer el cambio de id y spawnear al actor
        /// </summary>
        /// <param name="CharacterId"></param>
        private void OnRespawnEvent(int CharacterId)
        {
            CharacterSelect(CharacterId);
            Resp.Spawn(ref Actor);
        }

        /// <summary>
        /// Evento que salta al iniciar la room
        /// </summary>
        private void OnJoinRoom()
        {

            if (PhotonManager.instance != null)
            {
                PhotonManager.instance.LoadingCanvas.SetActive(false);
            }

            
            TeamRegister.Singleton.AsignarEquipos();

            Match.SetTeam(ModeFill);
           
            matchRegister = new MatchRegister(ref OnChangeMode);

            TeamRegister.Singleton.InicializaEquipos();

            OnJoiningRoom();
            CharacterSelection_RiseEvent();

          
        }
       
        private void OnChangeMatchEvent(TypeMatch mode)
        {
            Debug.Log("Salta evento local en room 01");
          
            if (LastMode == mode) return;
            string SelectionMod = mode.ToString(); //Debil
            MatchBase NE = null;
            //foreach (var item in IEvents)
            //{
            //    Debug.Log(item.GetType().ToString());
            //    if (item.Key.GetType().ToString().Substring(7).Contains(SelectionMod))
            //    {
            //        NE = item.Key;
            //    }
            //}
            
            NE = IEvents.Where(x => x.Key.GetType().ToString().Substring(7).Contains(SelectionMod)).Select(p => p.Key).SingleOrDefault();
          
            if (NE == null)
            {
                Debug.LogError("El Evento no esta cargado");
            }
            Dictionary<IEventMatch, List<Action>> Loaded = new Dictionary<IEventMatch, List<Action>>();

            foreach (var item in IEvents)
            {
                Loaded.Add(item.Value, new List<Action>());
            }
            Debug.Log(Loaded.Count());
            if(matchRegister == null) { Debug.Log("p"); }
            matchRegister.SetNewEvent(NE, Loaded);
            LastMode = mode;
            Debug.Log("Salta evento local en room 02");

        }

       

        /// <summary>
        /// Evento de respawn cuando muere el jugador
        /// </summary>
        /// <param name="onDeadLocal"></param>
        public void RespawnDied(Action onDeadLocal)
        {
            Action OnDead = delegate { }; 
            Action OnChangePlayer = delegate { }; 
            Action OnRevive = delegate { };
            OnPlayerSpawn( true );

            OnDead += onDeadLocal;
            OnDead += MGV.OnPlayerFellDead;
            OnDead += TeamRoom.MyTeam.UpdateUI_Risevent;
            OnChangePlayer += MGV.OnPlayerDead;
            OnDead += CanvasManager.Singleton.HidePlayerSelector;

            OnChangePlayer += CanvasManager.Singleton.ShowPlayerSelector;

            OnRevive += CanvasManager.Singleton.HidePlayerSelector;
            OnRevive += OnSpawnEvent;
            OnRevive += TeamRoom.MyTeam.UpdateUI_Risevent;

            RespCorutine= StartCoroutine(Match.Respawn(OnDead, OnChangePlayer, OnRevive,TimeToGoRespawn,TimeToRespawn));
        }

        /// <summary>
        /// Elimina al jugador local
        /// </summary>
        public void RemovePlayer()
        {
            if (!Actor)
            {
                Debug.LogError("No Existe el Actor local");
                return;
            }

            PhotonNetwork.Destroy(Actor);
        }

    
        /// <summary>
        /// Cambia los Listeners de los botones en funcion del estado
        /// </summary>
        /// <param name="death">Player local Muerto</param>
        public void OnPlayerSpawn(bool death)
        {
            ChangeSpawn -= OnRespawnEvent;
            ChangeSpawn -= CharacterSelect;

            if (death)
            {
                ChangeSpawn += CharacterSelect ;
            }
            else
            {
                ChangeSpawn += OnRespawnEvent;
            }

            CanvasManager.Singleton.ChangeListenersButtons(ChangeSpawn);
        }

        // TODOO Se ha de enviar por RiseEvent
        public void RemoveTeam(int team)
        {

            TeamRegister.Singleton.removeTeam(team,null);

            maxTeams = TeamRegister.Singleton.GetAmmounts();
        }

        #endregion

        //Las operaciones se ejecutan en todos los clientes, por lo que solo el MC debe de mandar los eventos de red
        #region MathcMaking Callbacks Photon
        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            // todos excepto el que entra
            TeamRegister.Singleton.AsignarEquipos();

            //solo el local // si es el que acaba de entrar esta llamada no salta
            if (newPlayer== PhotonNetwork.LocalPlayer)
            {

            }

            //solo el cliente maestro
            if (!PhotonNetwork.IsMasterClient) return;
          
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            TeamRegister.Singleton.AsignarEquipos();
            CanvasManager.Singleton.UpdatePlayerSelector();
            if (PhotonNetwork.LocalPlayer == otherPlayer)
            {
                RemovePlayer();
            }

            if (!PhotonNetwork.IsMasterClient) return;
            Debug.LogError("Se va - " + otherPlayer.NickName);
            CharacterSelection_RiseEvent();
          
        }

        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            //throw new NotImplementedException();
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            //throw new NotImplementedException();
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            //throw new NotImplementedException();
        }
        #endregion



    }
}
