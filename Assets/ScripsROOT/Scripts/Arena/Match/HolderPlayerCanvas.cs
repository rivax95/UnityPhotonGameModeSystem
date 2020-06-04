using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
public class HolderPlayerCanvas : MonoBehaviour
{
    private Image CharImg;
    private Text NickName;
    private Image fondo;
    private Image IsDead;
    private Image IsDisqualdied;
    private Player Propietario;
    private void OnEnable()
    {
       CharImg= transform.GetChild(0).GetComponent<Image>();

        NickName= transform.GetChild(2).GetComponent<Text>();

        fondo = transform.GetChild(3).GetComponent<Image>();

        IsDead = transform.GetChild(5).GetComponent<Image>();

      IsDisqualdied=  transform.GetChild(6).GetComponent<Image>();
    }

    public void setPropietario(Player p)
    {
        Propietario = p;

       // setColorTeam(TeamRoom.MyTeam.ColorThisTeam);
    }

    public Player GetPropietario()
    {
        return Propietario;
    }

    public void setNick(string nick)
    {
       if (string.IsNullOrEmpty(nick)) return;

        NickName.text = nick;
    }

    public  void setColorTeam(Color32 color)
    {
        fondo.color = color;
    }

    public void SetDead(bool state)
    {
        if (IsDead.gameObject.activeSelf == state) return;
            IsDead.gameObject.SetActive(state);
    }

    public void setDisqualdied(bool state)
    {
        if (IsDisqualdied.gameObject.activeSelf == state) return;
        IsDisqualdied.gameObject.SetActive(state);
    }

    public void SetCharImage (Sprite img)
    {
        if (img == null) return;
        CharImg.sprite = img;
    }
}
