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

		void Start () 
		{
			assignHelmet ();
		}

		private void assignHelmet()
		{
			Helmet_Type helmetType = ((Helmet_Type)((int)SMProductEquipper.INSTANCE.CurrentHelmet));
			switch(helmetType)
			{
			case Helmet_Type.BREATHOR:
				brethorHelmet.SetActive (true);
				break;
			case Helmet_Type.OPERATIVE:
				operativeHelmet.SetActive (true);
				break;
			case Helmet_Type.PILOTAR:
				pilotarHelmet.SetActive (true);
				break;
			case Helmet_Type.TS_TACTICAL:
				tsTacticalHelmet.SetActive (true);
				break;
			}
		}
	}
}
