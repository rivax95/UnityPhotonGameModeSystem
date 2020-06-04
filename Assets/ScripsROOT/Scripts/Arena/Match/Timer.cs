//                                          ▂ ▃ ▅ ▆ █ ARC █ ▆ ▅ ▃ ▂ 
//                                        ..........<(+_+)>...........
// Timer.cs (31/01/20)
//Autor: Alejandro Rivas                 alejandrotejemundos@hotmail.es
//Desc: Gestiona los eventos en base a un tiempo de ejecucion
//Mod : 25/02/2020 Version completamente funcional - Testada
//Rev Logica 
//..............................................................................................\\
#region Librerias
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
#endregion
namespace Alex.Arena.MTimer
{
    public class Timer : MonoBehaviour, IPunObservable
    {
        private Action OnFinishTime;
        private Action<string> OnChangeTime = null;
        private Action OnInit;
        private float Min;
        private float Second;

        private float CalcMathInSecond = -1;

        private bool init = false;

        public Timer(int Minutos, Action OnFinish, Action<string> OnChangeValue, Action Init)
        {
            this.CalcMathInSecond = Minutos * 60;

            this.OnFinishTime = OnFinish;
            this.OnChangeTime = OnChangeValue;
            this.OnInit = Init;

        }

        public Timer(float Segundos, Action OnFinish, Action<string> OnChangeValue, Action Init)
        {
            this.CalcMathInSecond = Segundos;

            this.OnFinishTime = OnFinish;
            this.OnChangeTime = OnChangeValue;
            this.OnInit = Init;

        }

        public void Run()
        {
            GetCurrentTime();
            if (!init)
            {
                init = true;
                OnInit?.Invoke();

            }
          
            if (Min == 0 && Second == 0)
                {

                OnFinishTime?.Invoke();

                }
                CalcMathInSecond -= (Min==0 && Second==0) ? 0 : Time.deltaTime;

            if (OnChangeTime != null)
            {
                string Timer = Min + ":" + Second;
                OnChangeTime?.Invoke(Timer);
            }
           
        }

        private void GetCurrentTime()
        {
            Min = Mathf.Clamp(Min, 0, 60);
            Second = Mathf.Clamp(Second, 0, 60);

            Min = (int)CalcMathInSecond / 60;
            Second = (int)CalcMathInSecond % 60;

        }
        private void Calculate()
        {
            CalcMathInSecond = 60 * Min;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
          
            if (stream.IsWriting)
            {
                stream.SendNext(Min);
                stream.SendNext(Second);
            }
            else
            {
                Min=float.Parse( stream.ReceiveNext().ToString());
                Second=float.Parse(stream.ReceiveNext().ToString());
                if (OnChangeTime != null)
                {
                    GetCurrentTime();
                    string Timer = Min + ":" + Second;
                    OnChangeTime(Timer);
                }

            }
         
            stream.Serialize(ref CalcMathInSecond);

        }

    }
}
