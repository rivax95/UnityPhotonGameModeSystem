using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
namespace Alex.Arena.ScoreObject
{
    [RequireComponent(typeof(PhotonView))]
    public class Score:MonoBehaviour, IPunObservable
    {
        
        /// <summary>
        /// Key equipo, Value valor del marcador
        /// </summary>
        protected Dictionary<int, int> Scores = new Dictionary<int, int>();
        private Action<int,int> ScoreModificado;


        public Score(int AmmountTeams, Action<int,int> WhenScoreModificate)
        {
            ScoreModificado = WhenScoreModificate;
            for (int i = 0; i < AmmountTeams; i++)
            {
                Scores.Add(i , 0);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
          
            if (stream.IsWriting)
            {
                foreach (var item in Scores)
                {
                    stream.SendNext(item.Value);
                    
                }
            }
            else
            {
                for (int i = 0; i < Scores.Count; i++)
                {
                    Scores[i] = (int)stream.ReceiveNext();
                   // ScoreModificado(i, Scores[i]);
                }

            }
           
        }

        public virtual void SetScore(int Team, int Ammount)
        {
            ScoreModificado(Team, Ammount);

            if (!PhotonNetwork.IsMasterClient) return;
            if (Scores.ContainsKey(Team))
            {
                Scores[Team] = Ammount;
            }
            else
            {
                Debug.LogError("No existe el equipo al que desea sumar la puntuacion");
                Debug.DebugBreak();
            }
        }

        public virtual int GetScore(int Team)
        {
            if (!CheckTeam(Team))
            {
                Debug.LogError("No Existe el equipo que se solicita "+ Team);
            }

            return Scores[Team];
        }
        private bool CheckTeam(int Team)
        {
            if (Scores.ContainsKey(Team))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<int> GetTeamWinner()
        {
            List<int> Teams = new List<int>();

            int Puntuacion = -1;
            foreach (var item in Scores)
            {
                if(item.Value > Puntuacion)
                {
                    Puntuacion = item.Value;
                }
            }

            foreach (var item in Scores)
            {
                if(item.Value == Puntuacion)
                {
                    Teams.Add(item.Key);
                }
            }

            return Teams;
        }

    }

}
