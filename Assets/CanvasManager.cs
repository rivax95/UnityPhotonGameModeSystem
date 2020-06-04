using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Alex.Arena.IMatch;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using Alex.Arena.ThaidersProperties;

[RequireComponent(typeof(PhotonView))]
public class CanvasManager : MonoBehaviourPun
{
    public Text TimeToClock;
    public static CanvasManager Singleton;
    public GameObject PlayerSelector;
    public Text TimeToSelector;
    public GameObject FatherScore;
    public List<Text> Scores;
    public List<ButtonCharacterAvaliable> ButtonsChangePlayer = new List<ButtonCharacterAvaliable>();
    public GameObject FatherButtonCharAvaliable;
    public GameObject FatherPlayersSelect;
    private List<HolderPlayerCanvas> PlayersYaSeleccionado = new List<HolderPlayerCanvas>();
    private Sprite Alpha0;
    private void Awake()
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

    public void Init()
    {
        foreach (Transform item in FatherButtonCharAvaliable.transform)
        {
            if (item.GetComponent<ButtonCharacterAvaliable>())
            {
                ButtonsChangePlayer.Add( item.GetComponent<ButtonCharacterAvaliable>());
            }
            //Debug.Log("ppp");
        }
      
        for (int i = 0; i < TeamRoom.MyTeam.maxAmmount; i++)
        {
            GameObject Load = Instantiate( Resources.Load<GameObject>("ArenaPrefabs/GUI/Holder") as GameObject);
            PlayersYaSeleccionado.Add(Load.GetComponent<HolderPlayerCanvas>());
            Load.transform.parent = FatherPlayersSelect.transform;
            Load.transform.localScale = Vector3.one;
         
        }

        List<TeamRoom> trl = TeamRegister.Singleton.GetList();
        foreach (var item in trl)
        {
            GameObject Load = Instantiate(Resources.Load<GameObject>("ArenaPrefabs/GUI/TeamScore") as GameObject);
            Load.transform.parent = FatherScore.transform;
            Load.transform.localScale = Vector3.one;
        }
        for (int i = 0; i < trl.Count; i++)
        {
            FatherScore.transform.GetChild(i).transform.Find("Image").GetComponent<Image>().color = trl[i].ColorThisTeam;
        }

        foreach (Transform obj in FatherScore.transform)
        {
            Scores.Add(obj.Find("Text").GetComponent<Text>());
        }

        Alpha0 = (Sprite)Resources.Load<Sprite>("ArenaPrefabs/GUI/Alpha0");
    }

    public void SetTime_UI(string text)
    {
        string[] MinAndSec = text.Split(':');
        int Min = int.Parse(MinAndSec[0]);
        int Sec = int.Parse(MinAndSec[1]);

        if (TimeToClock)
        {
            TimeToClock.text = text;
        } 
    }

    public void SetTime_Selector(string text)
    {
        TimeToSelector.text = text;
    }

    public void HidePlayerSelector()
    {
        PlayerSelector.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    public void ShowPlayerSelector()
    {
        PlayerSelector.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void SetScore(int team,int Ammount)
    {
        //Debug.Log(team + " score: " + Ammount);
        Scores[team].text = Ammount.ToString(); ;
    }

    public void ChangeListenersButtons(UnityAction<int> NewListener)
    {
        foreach (var item in ButtonsChangePlayer)
        {

            item.ChangeListener(NewListener);
        }
    }

    private string getRuteImageChar(int idChar)
    {
        string rute = "ArenaPrefabs/CharactersImages/" + idChar;
        return rute;
    }
    /// <summary>
    /// Actaliza los iconos de la seleccion de personajes con el nick y el character de los jugadores
    /// </summary>
    public void UpdatePlayersCharacters()
    {
       
        Dictionary<Player, int> MyTeam = Match.GetPlayersMyTeam();
        for (int i = 0; i < PlayersYaSeleccionado.Count; i++) 
        {
            PlayersYaSeleccionado[i].SetCharImage(Alpha0);
            PlayersYaSeleccionado[i].setNick("Empty");
            PlayersYaSeleccionado[i].setColorTeam(TeamRoom.MyTeam.ColorThisTeam);
        }

        for (int i = 0; i < MyTeam.Count; i++)
        {
            if (PlayersYaSeleccionado[i] == null) { Debug.LogError("Indice fuera de rango"); break; }
            List<Player> Parse = MyTeam.Keys.ToList();
            int charS = (int)Parse[i].CustomProperties[RefProperties.CharacterID];
            Sprite Load = Resources.Load<Sprite>(getRuteImageChar(charS));
            string nick = Parse[i].NickName;
            PlayersYaSeleccionado[i].SetCharImage(Load);
            PlayersYaSeleccionado[i].setPropietario(Parse[i]);
            PlayersYaSeleccionado[i].setNick(nick);
           

        }
    }

    private enum PlayerState
    {
        Ambos,soloKiller,SoloDeath,AmbosNull
    }
   
   
    public void UpdatePlayerSelector()
    {
        Debug.LogError("Up");
            foreach (var item in PlayersYaSeleccionado)
            {
                bool Isdead = (bool)item.GetPropietario().CustomProperties[RefProperties.IsDeath];
                bool IsDisqualified = (bool)item.GetPropietario().CustomProperties[RefProperties.IsDisqualified];
            Debug.Log(item.GetPropietario().NickName + " " + Isdead);
                item.SetDead(Isdead);
                item.setDisqualdied(IsDisqualified);
            }
    }

    public void UpdatePlayersDead(Player Killer, Player Death)
    {
        //asignacion de los casos
        PlayerState State = PlayerState.AmbosNull;

       if(Killer != null && Death != null)
        {
            State = PlayerState.Ambos;
        }
        else
        {
            if (Killer != null)
            {
                State = PlayerState.soloKiller;
            }

            if (Death != null)
            {
                State = PlayerState.SoloDeath;
            }
        }

        //Operaciones que ha de hacer si o si
     




        //Operaciones que ha de hacer cuando se den los siguientes casos

        switch (State)
        {
            case PlayerState.Ambos: //Significa que la muerte se ha producido por un enfrentamiento

                break;
            case PlayerState.soloKiller: //Significa que la muerte se ha producido por un suicidio a ti mismo

                break;
            case PlayerState.SoloDeath: //Significa que la muerte se ha producido por un suicidio debido a causas externas a las del jugador

                break;
            case PlayerState.AmbosNull: //Signidica que solo vamos a hacer las operaciones de si o si

                return;
                
            default:
                break;
        }

    }

}
