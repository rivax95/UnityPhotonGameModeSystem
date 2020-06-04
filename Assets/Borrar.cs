using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Borrar : MonoBehaviour
{
    public Button btn;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Locuraaaa");
    }

    // Update is called once per frame
    void Update()
    {
        btn.interactable = PhotonNetwork.IsConnectedAndReady;
    }
}
