//
//  SMLoadFromStart.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 03/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SandeepMattepu
{
	/// <summary>
	/// This class will check if player is connected to photon server in the current scene. If not it will load appropriate scene
	/// </summary>
	public class SMLoadFromStart : Photon.PunBehaviour 
	{
		/// <summary>
		/// The scene name to load if scene is not connected to photon.
		/// </summary>
		[Tooltip("The scene name to load if scene is not connected to photon.")]
		public string sceneNameToBegin;
		// Use this for initialization

		void Awake()
		{
			if(PhotonNetwork.room == null)			// Checks whether player is connected to photon
			{
				SceneManager.LoadScene (sceneNameToBegin);
			}
		}

		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void backButtonPressed()
		{
			PhotonNetwork.LeaveRoom ();
		}

		public override void OnLeftRoom ()
		{
			SceneManager.LoadScene (sceneNameToBegin);
		}
	}

}
