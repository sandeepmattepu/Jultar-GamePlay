//
//  SMSpecialWeaponManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 22/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class helps to maintain special weapons and abilities of the player
	/// </summary>
	public class SMSpecialWeaponManager : MonoBehaviour 
	{
		/// <summary>
		/// The rules for multiplayer game
		/// </summary>
		private SMMultiplayerGame multiplayerGame;
		// Use this for initialization
		void Start () 
		{
			// Hide all UI and special abilities at beginning
		}

		// Update is called once per frame
		void Update () 
		{

		}

		/// <summary>
		/// This function gets called whenever there is kill streak change
		/// </summary>
		/// <param name="killStreak">Current kill streak.</param>
		private void onKillStreakHandler(int killStreak)
		{
			if(killStreak == 0)
			{
				Debug.Log ("All kill streak reset");
			}
			else if(killStreak >= 6)
			{
				Debug.Log ("Boost health unlocked");
			}
			else if(killStreak == 4)
			{
				Debug.Log ("Rocket unlocked");
			}
			else
			{
				Debug.Log ("Kill streak counting");
			}
		}

		/// <summary>
		/// Registers the multiplayer game.
		/// </summary>
		/// <param name="gameType">Multiplayer game type.</param>
		public void registerMultiplayerGame(SMMultiplayerGame gameType)
		{
			multiplayerGame = gameType;
			multiplayerGame.OnKillStreakChange += onKillStreakHandler;
		}
	}	
}
