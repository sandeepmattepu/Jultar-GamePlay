//
//  SMPlayerEquipper.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 18/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu.Multiplayer;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu
{
	/// <summary>
	/// This class will analyze SMProductEquipper and assigns player with his abilities like helmets, perks etc
	/// </summary>
	public class SMPlayerEquipper : MonoBehaviour 
	{
		[SerializeField]
		private GameObject pilotarHelmet;
		[SerializeField]
		private GameObject operativeHelmet;
		[SerializeField]
		private GameObject brethorHelmet;
		[SerializeField]
		private GameObject tsTacticalHelmet;
		private SMPlayerHealth playerHealth;
		private Animator animator;
		[SerializeField]
		private ObscuredFloat breathorControllerSpeed = 1.05f;
		[SerializeField]
		private ObscuredFloat operativeControllerSpeed = 1.15f;
		[SerializeField]
		private ObscuredFloat pilotarControllerSpeed = 1.0f;
		[SerializeField]
		private ObscuredFloat tsTacticalControllerSpeed = 1.05f;

		void Start () 
		{
			playerHealth = GetComponent<SMPlayerHealth> ();
			animator = GetComponent<Animator> ();
			assignHelmet ();
		}

		private void assignHelmet()
		{
			Helmet_Type helmetType = ((Helmet_Type)((int)SMProductEquipper.INSTANCE.CurrentHelmet));
			switch(helmetType)
			{
			case Helmet_Type.BREATHOR:
				brethorHelmet.SetActive (true);
				playerHealth.addPointsToMaxHealthBy (25.0f);
				animator.speed = breathorControllerSpeed;
				break;
			case Helmet_Type.OPERATIVE:
				operativeHelmet.SetActive (true);
				playerHealth.addPointsToMaxHealthBy (10.0f);
				animator.speed = operativeControllerSpeed;
				break;
			case Helmet_Type.PILOTAR:
				pilotarHelmet.SetActive (true);
				playerHealth.addPointsToMaxHealthBy (20.0f);
				animator.speed = pilotarControllerSpeed;
				break;
			case Helmet_Type.TS_TACTICAL:
				tsTacticalHelmet.SetActive (true);
				playerHealth.addPointsToMaxHealthBy (10.0f);
				animator.speed = tsTacticalControllerSpeed;
				break;
			}
		}
	}
}
