//
//  SMScoreBoardUIManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 30/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandeepMattepu.UI
{
	/// <summary>
	/// This class will update the score board UI in real time
	/// </summary>
	public class SMScoreBoardUIManager : MonoBehaviour 
	{
		/// <summary>
		/// The players UI text to show their scores in free for all.
		/// </summary>
		[SerializeField]
		private Text[] freeForAllPlayers;
		/// <summary>
		/// The players UI text to show team 1 score in team death match.
		/// </summary>
		[SerializeField]
		private Text[] team1;
		/// <summary>
		/// The players UI text to show team 2 score in team death match.
		/// </summary>
		[SerializeField]
		private Text[] team2;
		// Use this for initialization
		void Start () 
		{
		}

		// Update is called once per frame
		void Update () {

		}
	}	
}
