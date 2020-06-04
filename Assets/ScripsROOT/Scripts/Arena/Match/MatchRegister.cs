//                                          ▂ ▃ ▅ ▆ █ ARC █ ▆ ▅ ▃ ▂ 
//                                        ..........<(+_+)>...........
// RoomServer.cs (06/03/20)
//Autor: Alejandro Rivas                 alejandrotejemundos@hotmail.es
//Desc: Sistema encargado de registrar y actualizar los subssistemas cuando se cambia de modo
//Mod : 06/03/2020 Init
//Rev V0.1 
//..............................................................................................\\
#region Librerias
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using Alex.Arena.MatchEvent;
using Alex.Arena.IMatch.Interface;
#endregion

namespace Alex.Arena.IMatch.Interface
{

    public interface IEventMatch
    {
        void Subscribe(List<Action> Events);
    }

    public interface IEventSubscriptor
    {
         List<Action> GetSubscriptor();
    }
}
namespace Alex.Arena.MatchEvent
{


    public class MatchRegister : MonoBehaviour
    {
        public List<MatchBase> Matchs;


        private List<Action> ReturnedActions;
        private Action OnChangeMode;

        public MatchRegister(ref Action OnUpdateMode)
        {
            OnChangeMode = OnUpdateMode;
            ReturnedActions = new List<Action>();
            Matchs = new List<MatchBase>();
            Debug.Log("yepa");

        }
      

        /// <summary>
        /// Solo va a registra los metodos que no tengan parametros
        /// </summary>
        /// <param name="NewRegister"></param>
        /// <returns></returns>
        List<Action>Register(MatchBase type)
        {
            
            if (CheckInList(type))
            {
                return ReturnedActions;
            }
            else
            {
                List<MethodInfo> infos = type.GetType().GetMethods().Where(x => x.CustomAttributes
             .Any(y => y.AttributeType == typeof(ThaidEvent))).ToList();

                List<Action> act = new List<Action>();
                
                for (int i = 0; i < infos.Count(); i++)
                {
                    act.Add(Delegate.CreateDelegate(typeof(Action<int>), type, infos[i].Name) as Action);
                }
              
                return act;
            }

        }
       
        /// <summary>
        /// Carga un nuevo evento en el sistema
        /// </summary>
        /// <param name="NexEvent"></param>
        /// <param name="Subscriptions">Las keys han de estar vacias, es el sistema el encargado de almacenarlas</param>
        public void SetNewEvent(MatchBase NexEvent, Dictionary<IEventMatch, List<Action>> Subscriptions)
        {
            List<Action> AllEventsMatch = Register(NexEvent);

            Debug.Log(AllEventsMatch.Count + " ammou");
            foreach (var item in Subscriptions)
            {
             
                foreach (var item2 in AllEventsMatch)
                {
                    Debug.Log(item2.Method.Name);
                        item.Value.Add(item2);
                    
                }
                item.Key.Subscribe(item.Value);
            }

            if (!CheckInList(NexEvent))
            {
                if (Matchs.Count < 1) return;

                OnChangeMode();
            }


        }
        /// <summary>
        /// Check if Event in last position in list
        /// </summary>
        /// <param name="Event"></param>
        /// <returns></returns>
        bool CheckInList(MatchBase Event)
        {
            if (!Event) return false;
            if (Matchs.Count > 0)
            {
               
                if (Matchs[Matchs.Count()-1].GetType() == Event.GetType() && Matchs.Count != 0)
                {
                    ReturnedActions.Capacity = Event.GetType().GetMethods().Count();
                    return true;
                }

            }

            if (Matchs.Contains(Event))// si existe lo posicionamos el ultimo en la lista
            {
              
                Matchs.Remove(Event);
                Matchs.Add(Event);
            }
            else
            {
                Matchs.Add(Event);
            }
            
            ReturnedActions.Clear();
            ReturnedActions.Capacity = Event.GetType().GetMethods().Count();

            return false;
        }

    }
}