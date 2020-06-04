using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alex.Arena.MatchEvent;
using Alex.Arena.IMatch.Interface;
using System;
using System.Reflection;
using System.Linq;
namespace Alex.Arena.MatchEvent
{
    /*
     var method = typeof(EventsGoal).GetRuntimeMethod(nameof(EventsGoal), new Type[] { });
    method.GetCustomAttribute<ThaidEvent>().setDestino(ref a);
    */
    public class DataSubscriptor
    {
       public Type tipo;
       public List<Action> Subscriptores;
       public PropertyInfo[] Pinfo;
    }
   
    public class EventsGoal : MatchBase, IEventMatch
    {
        Dictionary<IEventSubscriptor, DataSubscriptor> Punteros = new Dictionary<IEventSubscriptor, DataSubscriptor>(); 
       
        public void Awake()
        {
            foreach (var item in Punteros)
            {
              item.Value.Subscriptores = item.Key.GetSubscriptor();//.ForEach(x=> .Add(x));
              item.Value.tipo=  item.Key.GetType();    
            }
        }

        public void OnDisable()
        {
            
        }

     

        public override void ChangeEvents(MatchBase NewEvent)
        {
            Debug.Log("ChangeEvents");
          

        }
        [ThaidEvent("Alex", version = 1.0)]
        public override void OnEndMatch()
        {
            Debug.Log("OnEndMatch");
            Debug.Log(transform.name);
        }
        [ThaidEvent("Alex", version = 1.0)]
        public override void OnJoinKillZone()
        {
            Debug.Log("OnJoinKillZone");
           
        }
        [ThaidEvent("Alex", version = 1.0)]
        public override void OnPointScore()
        {
            Debug.Log("OnPointScore");
        }
        [ThaidEvent("Alex", version = 1.0)]
        public override void RemoveElemnentsThisMatch()
        {
            Debug.Log("RemoveElemnentsThisMatch");
        }
        [ThaidEvent("Alex", version = 1.0)]
        public override void SpawnStartMatch()
        {
            Debug.Log("SpawnStartMatch");
        }
    
        public void Subscribe(List<Action> Events)
        {
            Debug.Log("I AM EVENT GOAL");

        }
    }
}

