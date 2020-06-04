using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Alex.Controller;
using Alex.Arena.IMatch;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Linq;
//todo lo que ahi en esta clase que esta comentado ahi q descomentarlo
public class MatchGenericsEvents : MonoBehaviour
{

    [Header("Referencias")]
    public Camera CameraArena;
    public GameObject cinematic;
    //public List<TeamRoom> BR;

  
    public virtual void OnPlayerDead()
    {
        CameraArena.enabled = true;
    }
    public virtual void OnPlayerFellDead()
    {
        //CanvasManager.Singleton.GunsClose();
    }
    public virtual void OnPlayerSpawn()
    {
        //CameraArena.enabled = false;
        //CanvasManager.Singleton.GunsOpen();
        Debug.Log("GenericEventSubscriber");
    }

    public virtual void OnInitMatch()
    {
        //CanvasManager.Singleton.AnnEnable();
    }

    public virtual void OnEndMatch()
    {
        StartCoroutine(LaunchCinematic());
    }

    public virtual void OnJoinArena()
    {
        if (PhotonManager.instance != null)
        {
            PhotonManager.instance.LoadingCanvas.SetActive(false);
        }

        CameraArena.enabled=(true);

        //DoorsOpened();
        //CanvasManager.Singleton.GunsClose();
    }

    public IEnumerator LaunchCinematic()
    {
        StopCoroutine(Match.Respawn(null, null, null,90,0));
        cinematic.SetActive(true);

        foreach (DictionaryEntry item in Match.PlayersInArena) // coje tambien todos los hijos
        {
            GameObject PlayerObj = item.Value as GameObject;

            PlayerObj.SetActive(false);
           
        }

        //CanvasManager.Singleton.CinematicaCanvas(true);

        yield return new WaitForSeconds(4f);
        //CanvasManager.Singleton.CinematicaCanvas(false);

        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        PhotonNetwork.LeaveRoom();

    }

    //public void DoorsOpened()
    //{
       
    //      BR.ForEach(x=>  x.SetColisionsDoors(Match.GetTeam() != x.TeamID ? false : true));
       
    //}

}
