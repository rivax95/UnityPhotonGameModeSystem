using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alex.Arena.Respawn;
using System;
using Photon.Pun;
using Photon.Realtime;
using Alex.Arena.ThaidersProperties;
using Alex.Arena.IMatch;
using System.Linq;
public class GenericResp : Respawn
{
    

   public float timerToRespawn;
    private float counter;
    private GameObject chara;
    private Transform LastSpawnPoint;
    public GenericResp(Dictionary<Transform, int> SpawnPoints,ref Action OnSpawn) : base(SpawnPoints)
    {
        RespawnAreas = SpawnPoints;
       OnPlayerSpawn= OnSpawn;
        Debug.Log(OnPlayerSpawn.GetInvocationList().Count() + "Its ammount Methods");
    }

    public override bool ChekSpawn(int team)
    {
        if (RespawnAreas.Count <= 0) return false;
        if (!RespawnAreas.ContainsValue(team)) return false;
        //if (CharacterInArena != null) return false;
        
        return true;
    }

    public override void SetRespawn(int Team, int actorID)
    {
    
        Respawn.Team = Team;

    }

    public override void Spawn( int ActorID, ref GameObject Actor)
    {
        if (Actor)
        {
            PhotonNetwork.Destroy(Actor);
            Debug.Log("Player Existe pero es eliminado");

        }
        
        Transform RespawnPoint = GetSpawnPoint(ActorID);
        Actor =
          PhotonNetwork.Instantiate(
              Match.GetRutePrefabPlayer(
                  Match.GetCharacterID(PhotonNetwork.LocalPlayer.ActorNumber)
              ),
    RespawnPoint.position, RespawnPoint.rotation);
        LastSpawnPoint = RespawnPoint;

        Debug.Log(OnPlayerSpawn.GetInvocationList().Count() + "Its ammount Methods");

        OnPlayerSpawn?.Invoke();
    }
    public override void Spawn(ref GameObject Actor)
    {

        Transform RespawnPoint;
        if (Actor)
        {
            RespawnPoint = Actor.transform;
            PhotonNetwork.Destroy(Actor);
            //Debug.Log("Player Existe pero es eliminado 2");
            
        }
        else
        {
            RespawnPoint = GetSpawnPoint(Actor.GetPhotonView().OwnerActorNr);
            Debug.LogError("El player deberia de existir segun el flujo de datos");
        }

        Actor =
          PhotonNetwork.Instantiate(
              Match.GetRutePrefabPlayer(
                  Match.GetCharacterID(PhotonNetwork.LocalPlayer.ActorNumber)
              ),
    RespawnPoint.position, RespawnPoint.rotation);

        OnPlayerSpawn?.Invoke();
    }
    private Transform GetSpawnPoint(int ActorID)
    {
        int team = Match.GetTeam(ActorID);
        List<Player> ListOfPlayer = new List<Player>();
        List<Transform> Respawn = new List<Transform>();
       
        foreach (var item in RespawnAreas)
        {
            if(item.Value == team)
            {
                Respawn.Add(item.Key);
            }
        }

        ListOfPlayer = PhotonNetwork.PlayerList.Where(x => (int)x.CustomProperties[RefProperties.Team] == team).OrderBy(x=> x.ActorNumber).ToList();

      
        if(ListOfPlayer.Count != 0)
        {
            for (int i = 0; i < ListOfPlayer.Count; i++)
            {
                if(ListOfPlayer[i].ActorNumber == ActorID)
                {
                    return Respawn[i];
                }
            }
        }
        else
        {
            Debug.LogError("System [GetSpawnPoint] / GenericResp.cs Failled, List Players is empty");
            Debug.Break();
        }

        return null;
    }
    public override IEnumerator InitTimeForRespawn(Action Init,Action Out,Action<string> InProgres, int Team, int ActorID)
    {
        counter = timerToRespawn;
        Init();
        while (counter>0)
        {
            counter -= Time.deltaTime;
            InProgres(counter.ToString());
            yield return null;
        }
        Out();
    }

}
