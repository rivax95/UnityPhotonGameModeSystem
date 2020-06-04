using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Alex.Arena.MatchEvent
{

    public abstract class MatchBase : MonoBehaviour
    {

        public abstract void OnJoinKillZone();

        public abstract void SpawnStartMatch();

        public  abstract void OnEndMatch();

        public abstract void OnPointScore();

        public abstract void RemoveElemnentsThisMatch();

        public abstract void ChangeEvents(MatchBase NewEvent);

    }


}