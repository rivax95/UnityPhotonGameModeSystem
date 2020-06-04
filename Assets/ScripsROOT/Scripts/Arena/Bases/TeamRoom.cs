using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Alex.Arena.IMatch;
using Alex.Arena.ThaidersProperties;
using System;
using ExitGames.Client.Photon;

[RequireComponent(typeof(PhotonView))]
public class TeamRoom : MonoBehaviourPun,IPunObservable, IOnEventCallback
{
    public static TeamRoom MyTeam;
    public List<GameObject> Puertas;
    public GameObject SpawnFatherPoint;
    public BaseRespawn BaseResp;
    public int TeamID=-1; //ocultar al inspector
    public Color32 ColorThisTeam;
    public int maxAmmount = 2;
    public List<Player> Equipo; // Añadir el equipo // primera iteracion pero no va a funcionar ahi que seguir trabajandolo // me queda esto de la logica por añadir // parece ser que esta solucionado
    public List<Player> Descalificados; 
    public Action OnDescalificadoLocal;
    private int lasMaxAmm = -2;


    private const byte UpdateUI = 10;
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    public void OnTeamSelect()
    {
        if(Match.GetTeam() == TeamID)
        {
            MyTeam = this;
            SetColisionsDoors(false);

            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
          }
    }

    public void SetColisionsDoors(bool Enable)
    {
        Puertas.ForEach(x => x.transform.GetChild(0).GetComponent<Collider>().enabled = Enable); // esto puede fallar por el indice del get child no se si es el 1 o el 0 PROBAR
    }

    public void AgrandarEquipo(int cantidadSumada)
    {
        maxAmmount += cantidadSumada;
    }

    public void RestarEquipo(int cantidadRestada)
    {
        maxAmmount -= cantidadRestada;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
            stream.Serialize(ref maxAmmount);
      
    }

    public void IsDisqualifiedPlayer(Player p,bool IsDisqualified)
    {
        if (IsDisqualified)
        {
            if (!Descalificados.Contains(p))
            {
                Descalificados.Add(p);
            }
        }
        else
        {
            if (Descalificados.Contains(p))
            {
                Descalificados.Remove(p);
            }

        }
        Player target = p;
        RefProperties.ChangeValueProp(RefProperties.IsDisqualified, IsDisqualified, ref target);

        if (p == PhotonNetwork.LocalPlayer)
        {
            OnDescalificadoLocal();
        }
    }

    public void UpdateUI_Risevent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers= ReceiverGroup.All ,TargetActors = Match.GetPlayersWithTeam(TeamID).Select(x=>x.ActorNumber).ToArray()};
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(UpdateUI, null, raiseEventOptions, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == UpdateUI)
        {
 
            CanvasManager.Singleton.UpdatePlayerSelector();
        }
    }
}
