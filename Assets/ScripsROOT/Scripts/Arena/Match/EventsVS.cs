using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alex.Arena.MatchEvent;
using Alex.Arena.IMatch.Interface;
using System;

public class EventsVS  : MatchBase, IEventMatch
{
    public override void ChangeEvents(MatchBase NewEvent)
    {
        throw new System.NotImplementedException();
    }

    public override void OnEndMatch()
    {
        throw new System.NotImplementedException();
    }

    public override void OnJoinKillZone()
    {
        throw new System.NotImplementedException();
    }

    public override void OnPointScore()
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveElemnentsThisMatch()
    {
        throw new System.NotImplementedException();
    }

    public override void SpawnStartMatch()
    {
        throw new System.NotImplementedException();
    }

    public void Subscribe(List<Action> Events)
    {
        Debug.Log("I AM EVENT VS");
    }


}
