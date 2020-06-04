using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
using Alex.Arena.IMatch;
using System.Linq;
using Photon.Pun;
using Alex.Arena.ThaidersProperties;
[RequireComponent(typeof(PhotonView))]
public class TeamRegister : MonoBehaviour,IPunObservable
{
    public static TeamRegister Singleton;
    private Hashtable Teams = new Hashtable(); // esta tabla solo debe de manejarse por rise events, debe estar siempre sincronizada
    public Action<int> ActAmmounts;
    private object LockedSIng = new object();
    public int AmmountTeams;

    private int LastAmmTeams = -1;

     void OnEnable()
    {

        List<TeamRoom> TrListAsAChild = transform.GetComponentsInChildren<TeamRoom>().ToList();

        for (int i = 0; i < TrListAsAChild.Count; i++)
        {
            TeamRoom tr = TrListAsAChild[i];
            tr.TeamID = i;
            Teams.Add(tr, Match.GetPlayersWithTeam(i));
            tr.Equipo = Match.GetPlayersWithTeam(i);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            AmmountTeams = transform.childCount;
        }
    }

    private void Awake()
    {
        lock (LockedSIng)
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(this);
            }
        }
      
    }

    public bool CheckFillTeams()
    {
        int maxFill=0;
        int actFill=0;
        foreach (TeamRoom item in Teams.Keys.Cast<TeamRoom>())
        {
            maxFill += item.maxAmmount;
            actFill += item.Equipo.Count();
        }

        if (actFill == maxFill)
        {
            return true;
        }
        return false;
    }

    public void ActAmmountTeams(int team)
    {

        TeamRoom tr = Teams.Keys.Cast<TeamRoom>().ToList().Where(x => x.TeamID == team).Single();
        for (int i = 0; i < AmmountTeams; i++)
        {
            if (tr.TeamID != team) continue;
            Teams[tr] = Match.GetPlayersWithTeam(tr.TeamID);
        } 


        ActAmmounts?.Invoke(team);

    }

    public int GetAmmounts()
    {
        return Teams.Count;
    }
    public List<TeamRoom> GetList()
    {
        return Teams.Keys.Cast<TeamRoom>().ToList();
    }

    public TeamRoom GetTR(int teamID)
    {
        List<TeamRoom> trl = Teams.Keys.Cast<TeamRoom>().ToList();
        return trl.Where(x => x.TeamID == teamID).Single();
    }

    public (TeamRoom,List<Player>) GetTeam(int TeamID)
    {
        foreach (TeamRoom item in Teams.Keys)
        {
            if (item.TeamID == TeamID)
            {
               return(item, Teams[item] as List<Player>);
            }
        }
        Debug.LogError("El equipo que buscas no existe en el contexto actual");
        return (null, null);
    }
    // rectificar y usar el metodo en el oneable
    protected void AddTeamToHas(TeamRoom TR, List<Player>PlayerTR)
    {
        Teams.Add(TR, PlayerTR);
    }

    public void removeTeam(int ID, Action MasterClientAction)
    {

        Teams.Remove(GetTR(ID));

        ActAmmountTeams(ID);

        if (!PhotonNetwork.IsMasterClient) return;

        foreach (Player item in Match.GetPlayersWithTeam(ID))
        {
            Player Target = item;
            RefProperties.ChangeValueProp(RefProperties.IsDisqualified, true, ref Target);
        }
        Match.AmmountTeams = AmmountTeams;
        MasterClientAction();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if(LastAmmTeams != AmmountTeams)
        {
            stream.Serialize(ref AmmountTeams);
            
            LastAmmTeams = AmmountTeams;
        }
        
    }

    public void AsignarEquipos()
    {

        foreach (var item in Teams.Keys.Cast<TeamRoom>())
        {
            item.Equipo = Match.GetPlayersWithTeam(item.TeamID);
            ActAmmounts?.Invoke(item.TeamID);
           
        }
    }
    public void InicializaEquipos()
    {

        foreach (var item in Teams.Keys.Cast<TeamRoom>())
        {
          
            item.OnTeamSelect();
        }
    }
}
