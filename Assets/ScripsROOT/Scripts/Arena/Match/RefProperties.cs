using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Realtime;
namespace Alex.Arena.ThaidersProperties
{

    public static class RefProperties
    {
        /// <summary>
        /// El trato es de int
        /// </summary>
        public const string Team = "Team";
        /// <summary>
        /// El trato es de int
        /// </summary>
        public const string Death = "Death";
        /// <summary>
        /// El trato es de int
        /// </summary>
        public const string Kills = "Kills";
        /// <summary>
        /// El trato es de bool
        /// </summary>
        public const string ReadyForBattel = "ReadyForBattel";
        /// <summary>
        /// El trato es de int 
        /// </summary>
        public const string CharacterID = "CharacterID";
        /// <summary>
        /// El trato es de int
        /// </summary>
        public const string Ping = "Ping";
        /// <summary>
        /// El trato es de bool
        /// </summary>
        public const string IsDeath = "IsDeath";
        /// <summary>
        /// El trato es de bool
        /// </summary>
        public const string IsDisqualified = "IsDisqualified";
        #region Methods
        /// <summary>
        /// Cambia el valor de la propiedad
        /// </summary>
        /// <param name="Prop">Es combeniente pasarle la cadena de texto con una de las constantes declaradas en RefProperties</param>
        /// <param name="value">Valor del cambio Int</param>
        /// <param name="player">Jugador destinatario</param>
        public static void ChangeValueProp(string Prop, int value, ref Player player)
        {
            player.SetCustomProperties(
                        new ExitGames.Client.Photon.Hashtable()
                        {
                            {
                                       Prop, value
                              }
                          }
                    );
        }
        /// <summary>
        /// Cambia el valor de la propiedad
        /// </summary>
        /// <param name="Prop">Es combeniente pasarle la cadena de texto con una de las constantes declaradas en RefProperties</param>
        /// <param name="value">Valor del cambio bool</param>
        /// <param name="player">Jugador destinatario</param>
        public static void ChangeValueProp(string Prop, bool value, ref Player player)
        {
            player.SetCustomProperties(
                       new ExitGames.Client.Photon.Hashtable()
                       {
                            {
                                       Prop, value
                              }
                         }
                   );

        }
    }

    #endregion

}