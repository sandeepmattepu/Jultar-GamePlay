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
		/// The players UI text to show their scores.
		/// </summary>
		private Text[] players;
		// Use this for initialization
		void Start () 
		{
			players = new Text[PhotonNetwork.playerList.Length];
		}

		// Update is called once per frame
		void Update () {

		}
	}	
}
