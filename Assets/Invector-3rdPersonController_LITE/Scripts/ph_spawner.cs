using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Invector.vCharacterController;
using Alex.Arena.IMatch;
public class ph_spawner : MonoBehaviourPun
{
    vThirdPersonInput input;
    public GameObject camera;

    // Start is called before the first frame update
    void Start()
    {

        input = GetComponent<vThirdPersonInput>();
        if (!photonView.IsMine)
        {
            input.enabled = false;
            camera.SetActive(false);
        }
    }
    private void OnEnable()
    {
        Match.PlayersInArena.Add(photonView.Owner, this.gameObject);
    }
    private void OnDisable()
    {
        Match.PlayersInArena.Remove(photonView.Owner);
    }
}
