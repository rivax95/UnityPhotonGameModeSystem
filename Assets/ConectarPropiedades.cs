using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Alex.Arena.ThaidersProperties;
using Alex.Arena.Managers.Room;
public class ConectarPropiedades : MonoBehaviourPun
{
  public List<Text>references = new List<Text>();

     private void Update()
    {
        try
        {
            references[6].text = PhotonNetwork.IsMasterClient.ToString();
            references[4].text = photonView.ObservedComponents.Count.ToString();
            references[0].text = PhotonNetwork.LocalPlayer.CustomProperties[RefProperties.Team].ToString();
            references[3].text = PhotonNetwork.LocalPlayer.NickName;

            references[2].text = PhotonNetwork.GetPing().ToString();

            references[5].text = CanvasManager.Singleton.TimeToClock.text;//RoomServer.Singleton.MatchTimer.Min.ToString() + ":" + RoomServer.Singleton.MatchTimer.Second.ToString();

            references[1].text = PhotonNetwork.LocalPlayer.CustomProperties[RefProperties.CharacterID].ToString();
            references[7].text = PhotonNetwork.CurrentRoom.ToString();
        }
        catch { }
      
    }
}
