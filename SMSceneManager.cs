//
//  SMSceneManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 25/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// This class will handle the scene managment like loading scenes and quiting the application
/// </summary>
public class SMSceneManager : Photon.PunBehaviour
{

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/// <summary>
	/// Call this function if the current scene is game scene and wants to return to lobby
	/// </summary>
	public void returnToLobby()
	{
		PhotonNetwork.LeaveRoom ();
	}

	/// <summary>
	/// Call this function if you want to load the scene of particular name
	/// </summary>
	/// <param name="sceneName">Exact name of the scene.</param>
	public void loadScene(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
	}
	// This function is called by Photon API once the player has successfully left the game
	public override void OnLeftRoom ()
	{
		loadScene ("Lobby");
	}
}
