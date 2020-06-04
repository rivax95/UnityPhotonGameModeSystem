using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Alex.Arena.Managers.Room;
using Alex.Arena.ThaidersProperties;
public class BaseRespawn : MonoBehaviourPun
{
    //public int team;
    //// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.GetComponent<PhotonView>())
        {
          
            if (other.GetComponent<PhotonView>().IsMine)
            {
                Player Me = other.GetComponent<PhotonView>().Owner;
                bool IsDeath = (bool)Me.CustomProperties[RefProperties.IsDeath];
            
                if (IsDeath) return;
               CanvasManager.Singleton.ShowPlayerSelector();
                RoomServer.Singleton.OnPlayerSpawn(false);

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.GetComponent<PhotonView>())
        {

            if (other.GetComponent<PhotonView>().IsMine)
            {
                Player Me = other.GetComponent<PhotonView>().Owner;
                bool IsDeath = (bool)Me.CustomProperties[RefProperties.IsDeath];
                if (IsDeath) return;
                CanvasManager.Singleton.HidePlayerSelector();
                

            }
        }
    }

}
