#region Librerias
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Alex.Arena.ThaidersProperties;
using Alex.Arena.ScoreObject;
using System.Linq;
using Alex.Arena.MTimer;
using Alex.Arena.MatchEvent;
using Alex.Arena.IMatch.Interface;
#endregion
/*Recordar
 * Los IDs de los jugadores parten desde el numero 0
 * 
 * 
 * 
 * 
 */
namespace Alex.Arena.IMatch
{
    /// <summary>
    /// Interface para complementar el sistema del Arena
    /// </summary>
    public static class Match 
    {
        public static List<int> EquiposLlenos = new List<int>();
        private static int characterID=-1;
        public static int AmmountTeams;
        public static Hashtable PlayersInArena = new Hashtable();
        public static List<GameObject> Players = new List<GameObject>();
        public  static int GetCharacterID(int photonActorID)
        { 
        return (int)PhotonNetwork.CurrentRoom.GetPlayer(photonActorID).CustomProperties[RefProperties.CharacterID];
        }


        public static string GetRutePrefabPlayer(int id)
        {
            string rute = "ArenaPrefabs/Characters/" + id;
            return rute;
        }

      public static GameObject GetPlayer(Player User)
        {
            GameObject[] Jugadores;
            Jugadores = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] Jugadores2;
            Jugadores2 = GameObject.FindGameObjectsWithTag("Player");

            GameObject[] Result = Jugadores.Union(Jugadores2).ToArray(); //Union elimina los datos repetidos

            List<GameObject> OnlyAllPlayers = new List<GameObject>();

            for (int i = 0; i < Result.Length; i++)
            {
                if (!OnlyAllPlayers.Contains(Result[i].transform.root.gameObject))
                {

                    OnlyAllPlayers.Add(Result[i].transform.root.gameObject);
                }

            }

            GameObject Getter = new GameObject();
            foreach (var item in OnlyAllPlayers)
            {
                if (!item.GetPhotonView()) continue;
                if (item.GetComponent<PhotonView>().Owner.ActorNumber == User.ActorNumber)
                {

                    Getter = item;
                    break;
                }
            }
            if (Getter != null)
            {
                throw new ArgumentNullException(paramName: nameof(User), message: "No se ha encontrado el jugador, puede que no exista una istancia de este en la arena");
            }
            return Getter;
        }

     
        /// <summary>
        /// Actualiza el Character del jugador LOCAL
        /// </summary>
        /// <param name="PhotonActorID">Se recibe atraves del Player.ActorID</param>
        public static void SetCharacterID(int ID_Character)
        {
            Player P = PhotonNetwork.LocalPlayer;
           
            RefProperties.ChangeValueProp(RefProperties.CharacterID, ID_Character, ref P);
            characterID = ID_Character;
            Debug.Log("cHANGE ID CHAR");
           
        }
        /// <summary>
        /// Selecciona al equipo al que debe pertenecer el jugador, este metodo no debe de usarse en Awake o OnEnable
        /// </summary>
        /// <returns></returns>
        public static void SetTeam(FillTeams fill)
        {
          
            Dictionary<int, int> Teams = new Dictionary<int, int>();
            for (int i = 0; i < AmmountTeams; i++)
            {
                Teams.Add(i, 0);
            }
         
            foreach (var item in PhotonNetwork.PlayerList)
            {
               if(!(bool)item.CustomProperties[RefProperties.ReadyForBattel]) continue;
                int TeamPlayer = (int)item.CustomProperties[RefProperties.Team];
                Teams[TeamPlayer]++;
            }

            int TeamSelect=-1;
            Player Me = PhotonNetwork.LocalPlayer;
            List<TeamRoom> Equipos = TeamRegister.Singleton.GetList();
    
            EquiposLlenos.Clear();

            foreach (TeamRoom item in Equipos)
            {
              
                    if(item.maxAmmount == item.Equipo.Count)
                    { 
                        EquiposLlenos.Add(item.TeamID);
                    }
                
            }
     
            Dictionary<int, int> TsDisponse = Teams;

            List<int> keys = Teams.Keys.ToList();
            for (int i = 0; i < Teams.Count; i++)
            {
                foreach (var item in EquiposLlenos)
                {
                    if (keys[i] == item)
                    {
                        TsDisponse.Remove(item);
                    }
                }

            }
            switch (fill)
            {
                case FillTeams.OneAOne:

                     TeamSelect = TsDisponse.Values.Min();
                    
                    break;

                case FillTeams.First_FillTeam:

                    TeamSelect = TsDisponse.Values.Max();

                    break;
            }
            RefProperties.ChangeValueProp(RefProperties.Team, TeamSelect, ref Me);
            RefProperties.ChangeValueProp(RefProperties.ReadyForBattel, true, ref Me);

        }

        /// <summary>
        /// Retorna el equipo en el que se encuentra el Jugador
        /// </summary>
        /// <returns></returns>
        public static int GetTeam(int ActorID)
        {
            Debug.Log(ActorID + "El actor id");
            return (int)PhotonNetwork.CurrentRoom.GetPlayer(ActorID).CustomProperties[RefProperties.Team];
        }

        // <summary>
        /// Retorna el equipo en el que se encuentra el Jugador LOCAL
        /// </summary>
        /// <returns></returns>
        public static int GetTeam()
        {
            return (int)PhotonNetwork.LocalPlayer.CustomProperties[RefProperties.Team];
        }

        /// <summary>
        /// Actualiza los Jugadores disponibles en la seleccion
        /// </summary>
       
        public static void UpdateCharactesrAvaliables(ref GameObject CharactersContainer, int team)
        {
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties[RefProperties.Team] != team) return;

            Dictionary<int, ButtonCharacterAvaliable> Updater = new Dictionary<int, ButtonCharacterAvaliable>();
            int charactersAmmount = CharactersContainer.transform.childCount;
            ButtonCharacterAvaliable [] btns = CharactersContainer.transform.GetComponentsInChildren<ButtonCharacterAvaliable>();

            for (int i = 0; i < charactersAmmount; i++)
            {
                Updater.Add(i,
                  btns.Where(user => user.CharacterID == i)
         .Select(user => user).Single()
                    );
            }
           

            List<Player> CharactersInProperties =
                PhotonNetwork.PlayerList.
                Where(x => (int)x.CustomProperties[RefProperties.Team] == team).ToList();


            for (int i = 0; i < Updater.Count; i++)
            {
                for (int e = 0; e < CharactersInProperties.Count; e++)
                {
                    if(Updater[i].CharacterID == (int)CharactersInProperties[e].CustomProperties[RefProperties.CharacterID])
                    {
                        Updater[i].Btn.interactable = false;
                        break;
                    }
                    else
                    {
                        Updater[i].Btn.interactable = true;
                    }
                }
            }

        }

        public static List<int> CharAvaliable(int CharacterAmmount)
        {

            List<int> Avaliables = new List<int>();
            List<int> Disponse = new List<int>();
            for (int i = 0; i <= CharacterAmmount; i++)
            {
                Avaliables.Add(i);
            }
            Dictionary<Player, int> MyTeam = GetPlayersMyTeam();

          Disponse=  Avaliables.Where(x => !MyTeam.ContainsValue(x)).ToList();//.Select(a => Avaliables.Remove(a));
            for (int i = 0; i < Disponse.Count; i++)
            {
                Debug.LogError("Avaliable - " + Disponse[i]);
            }
           
            return Disponse;
        }
        /// <summary>
        /// Retorna el jugador y el character asociado a este
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Player,int> GetPlayersMyTeam()
        {
            int team = (int)PhotonNetwork.LocalPlayer.CustomProperties[RefProperties.Team];
            Dictionary<Player, int> MyTeam = new Dictionary<Player, int>();
            foreach (var item in PhotonNetwork.PlayerList)
            {
                if ((int)item.CustomProperties[RefProperties.Team] == team)
                {
                    //Debug.Log(item.CustomProperties[RefProperties.CharacterID] + "el que saca");

                    MyTeam.Add(item, (int)item.CustomProperties[RefProperties.CharacterID]);
                }
            }
            return MyTeam;
        }
        /// <summary>
        /// Retorna los jugadores que pertenecen a un equipo
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static List<Player> GetPlayersWithTeam(int team)
        {
            return PhotonNetwork.PlayerList.Where(x => (int)x.CustomProperties[RefProperties.Team] == team).ToList();
        }

        /// <summary>
        /// Comrpueba si el jugador esta vivo
        /// </summary>
        /// <param name="ActorID"></param>
        /// <returns></returns>
        public static bool CheckAlivePlayer(int ActorID)
        { 
            return (bool)PhotonNetwork.CurrentRoom.GetPlayer(ActorID).CustomProperties[RefProperties.IsDeath];
        }
        /// <summary>
        /// Retorna el GameObject Jugador asociado al jugador
        /// </summary>
        /// <param name="ActorID"></param>
        /// <returns></returns>
        public static GameObject GetObjectPlayer(PhotonView ActorID)
        {
          
           foreach(DictionaryEntry Item in PlayersInArena)
            { 

                if((PhotonView)Item.Key == ActorID)
                {
                    return (GameObject)Item.Key;
                }

            }

            //.Select(g => g.transform)
            GameObject[] Jugadores;
            Jugadores = GameObject.FindGameObjectsWithTag("Enemy").Select(g => g.transform.root.gameObject).ToArray();
            GameObject[] Jugadores2;
            Jugadores2 = GameObject.FindGameObjectsWithTag("Player").Select(g => g.transform.root.gameObject).ToArray();

            GameObject[] Result = Jugadores.Union(Jugadores2).ToArray(); //Union elimina los datos repetidos

            List<GameObject> OnlyAllPlayers = new List<GameObject>();

            for (int i = 0; i < Result.Length; i++)
            {
                if (!OnlyAllPlayers.Contains(Result[i].transform.root.gameObject))
                {

                    OnlyAllPlayers.Add(Result[i].transform.root.gameObject);
                }

            }

            GameObject Getter = null;
            foreach (var item in OnlyAllPlayers)
            {
                if (!item.GetPhotonView()) continue;
                if (item.GetComponent<PhotonView>().Owner.ActorNumber == ActorID.OwnerActorNr)
                {

                    Getter = item;
                    break;
                }
            }
            if (Getter == null)
            {
                Debug.LogError("El GameObject Solicitado no existe o no contiene los tags Enemy o Player");
            }
            return Getter;
        }

    public static IEnumerator Respawn (Action OnDead, Action OnChangePlayer, Action OnRevive,float timeToGoDied,float timeToRespawn)
        {
            Player Me = PhotonNetwork.LocalPlayer;
            bool FirstDone=false;
            bool TwoDone=false;

            Timer First= new Timer(timeToGoDied, ()=> { FirstDone = true; },null,null);
            Timer Two= new Timer(timeToRespawn, () => { TwoDone = true; }, null,null);
            if ((bool)Me.CustomProperties[RefProperties.IsDeath]) yield return null;
                while ((bool)Me.CustomProperties[RefProperties.IsDisqualified] == false)
            {


                if ((bool)Me.CustomProperties[RefProperties.ReadyForBattel])
                {
                    RefProperties.ChangeValueProp(RefProperties.IsDeath, true, ref Me);

                    OnDead();

                  
                    while (!FirstDone)
                    {
                     
                        First.Run();
                        yield return null;
                    }
                  
                    OnChangePlayer();

                    while (!TwoDone)
                    {
                   
                        Two.Run();
                        yield return null;
                    }

                    RefProperties.ChangeValueProp(RefProperties.IsDeath, false, ref Me);
                    OnRevive();
                 
                }
                break;
            }
        }
        /// <summary>
        /// Limpia las propiedades Kill y Death
        /// </summary>
        public static void ClearProperties()
        {
            foreach (var item in PhotonNetwork.PlayerList)
            {
                item.CustomProperties["Kills"] = 0;
                item.CustomProperties["Death"] = 0;
            }
        }

        public static Dictionary<MatchBase, IEventMatch> GetEventMatch(ref GameObject EventsContainer)
        {

            GameObject child = EventsContainer;
            Dictionary<MatchBase, IEventMatch> instances = new Dictionary<MatchBase, IEventMatch>();
            foreach (Transform item in child.transform)
            {
                MatchBase MB = item.GetComponent<MatchBase>();
                if (MB != null)
                {

                    instances.Add(MB, MB.GetComponent<IEventMatch>());
                }
            }
            return instances;
        }
    }
}

