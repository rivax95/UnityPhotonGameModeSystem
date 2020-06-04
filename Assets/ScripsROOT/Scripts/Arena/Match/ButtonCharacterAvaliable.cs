using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
[RequireComponent(typeof(Button))]
public class ButtonCharacterAvaliable : MonoBehaviour
{
  
    public int CharacterID;
    [HideInInspector]
    public Button Btn;

    private void Awake()
    {
        if (Btn == null)
        {
            Btn = GetComponent<Button>();
            if (Btn == null)
            {
                Btn = gameObject.AddComponent<Button>();
            }
        }
       
    }
    public void ChangeListener(UnityAction<int> newListener)
    {
        Btn.onClick.RemoveAllListeners();
        Btn.onClick.AddListener(() => newListener(CharacterID));

    }
}
