using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
namespace Alex.Arena.Respawn
{
    /// <summary>
    /// Objeto para el control de respawn
    /// </summary>
    public abstract class Respawn : MonoBehaviour
    {
        public Action OnPlayerSpawn;

        protected GameObject CharacterInArena { get; set; }
        /// <summary>
        /// Equipo al que perteneces
        /// </summary>
        protected static int Team;
        /// <summary>
        /// Value team, Key Transform del objeto donde debe respawnear
        /// </summary>
        protected Dictionary<Transform, int> RespawnAreas;
        /// <summary>
        /// Establece donde debe respawnear el Jugador
        /// </summary>
        /// <param name="Team">Equipo al que pertecenece</param>
        public abstract void SetRespawn(int Team, int actorID);
        /// <summary>
        /// Spawnear al jugador, SOLO DEBE USARSE PARA SPAWNEAR AL JUGADOR LOCAL
        /// </summary>

        /// <param name="ActorID">ID del jugador </param>
        public abstract void Spawn(int ActorID, ref GameObject Actor);
        /// <summary>
        /// Determina si el jugador puede o no spawnear
        /// </summary>
        /// <param name="team">Numero del equipo al que pertenece</param>
        /// <returns></returns>
        public abstract bool ChekSpawn(int team);

        /// <summary>
        /// Constructor del objeto
        /// </summary>
        /// <param name="SpawnPoints"></param>
        public Respawn(Dictionary<Transform, int> SpawnPoints)
            => SpawnPoints = RespawnAreas;

        /// <summary>
        /// Temporizador de respawn
        /// </summary>
        /// <param name="Init">Cosas que deben ejecutarse cuando el jugador muere</param>
        /// <param name="Out">Cosas que deben ejecutarse cuando el jugador respawnea</param>
        /// <param name="InProgres">Cosas que deben ejecutarse mientras el respawn esta en progreso</param>
        /// <returns></returns>
        public abstract IEnumerator InitTimeForRespawn(Action Init, Action Out, Action<string> InProgres, int Team, int ActorID);

        public abstract void Spawn(ref GameObject Actor);

    }
}
