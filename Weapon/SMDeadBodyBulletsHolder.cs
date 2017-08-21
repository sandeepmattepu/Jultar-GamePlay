//
//  SMDeadBodyBulletsHolder.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 21/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu.Weapon
{
	/// <summary>
	/// This class acts as a pocket of dead body which tells whether the dead body has bullets.
	/// </summary>
	public class SMDeadBodyBulletsHolder : MonoBehaviour 
	{
		/// <summary>
		/// Is the dead body has spare bullets
		/// </summary>
		private ObscuredBool hasSpareBullets = true;
		/// <summary>
		/// Gets a value indicating whether dead body has spare bullets.
		/// </summary>
		public ObscuredBool HasSpareBullets {
			get {
				return hasSpareBullets;
			}
		}

		/// <summary>
		/// Loots the spare bullets from body.
		/// </summary>
		/// <returns><c>true</c>, if spare bullets from body was looted successfully <c>false</c> Not successfully looted.</returns>
		public bool lootSpareBulletsFromBody()
		{
			if(hasSpareBullets)
			{
				hasSpareBullets = false;
				return true;
			}
			return false;
		}
	}
}
