//
//  SMProductEquipper.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 18/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.SceneManagement;

namespace SandeepMattepu
{
	/// <summary>
	/// This class will be used to persist information about which product is equipped and which product is not
	/// </summary>
	public class SMProductEquipper : MonoBehaviour 
	{
		/// <summary>
		/// Access this instance to get to know about what products are equipped to player
		/// </summary>
		public static SMProductEquipper INSTANCE = null;

		private ObscuredInt currentExpBoostMultiplier = 1;
		/// <summary>
		/// Gets the current exp boost active.
		/// </summary>
		/// <value>The current exp boost active.</value>
		public ObscuredInt CurrentExpBoostMultiplier {
			get {
				return currentExpBoostMultiplier;
			}
		}

		private ObscuredInt currentEquippedGunType = 3;
		/// <summary>
		/// Gets the current equipped gun.
		/// </summary>
		/// <value>The type of the current equipped gun.</value>
		public ObscuredInt CurrentEquippedGunType {
			get {
				return currentEquippedGunType;
			}
		}

		private ObscuredInt currentPerk = 0;
		/// <summary>
		/// Gets the current perk.
		/// </summary>
		/// <value>The current perk.</value>
		public ObscuredInt CurrentPerk {
			get {
				return currentPerk;
			}
		}

		private ObscuredInt currentHelmet = 0;
		/// <summary>
		/// Gets the current helmet.
		/// </summary>
		/// <value>The current helmet.</value>
		public ObscuredInt CurrentHelmet {
			get {
				return currentHelmet;
			}
		}

		private ObscuredInt currentLaserType = 0;
		/// <summary>
		/// Gets the type of the current laser.
		/// </summary>
		/// <value>The type of the current laser.</value>
		public ObscuredInt CurrentLaserType {
			get {
				return currentLaserType;
			}
		}

		// Use this for initialization
		void Start () 
		{
			if(INSTANCE == null)
			{
				INSTANCE = this;
				loadPlayerPrefs ();
				DontDestroyOnLoad (this.gameObject);		// Remove this after testing
			}
			else
			{
				Destroy (this.gameObject);
			}
		}

		// Update is called once per frame
		void Update () 
		{
			cheatCodes ();
		}

		private void cheatCodes()
		{
			if(SceneManager.GetActiveScene().name == "Lobby")
			{
				if(Input.GetKeyDown(KeyCode.Q))
				{
					setCurrentExpBoost (5);
				}
				else if(Input.GetKeyDown(KeyCode.W))
				{
					setCurrentExpBoost (8);
				}
				else if(Input.GetKeyDown(KeyCode.E))
				{
					equipGunWith (GUN_TYPE.Blonde);
				}
				else if(Input.GetKeyDown(KeyCode.R))
				{
					equipGunWith (GUN_TYPE.GreenEye);
				}
				else if(Input.GetKeyDown(KeyCode.T))
				{
					equipGunWith (GUN_TYPE.July11);
				}
				else if(Input.GetKeyDown(KeyCode.Y))
				{
					equipGunWith (GUN_TYPE.LeoBlackDog);
				}
				else if(Input.GetKeyDown(KeyCode.U))
				{
					equipGunWith (GUN_TYPE.Raini);
				}
				else if(Input.GetKeyDown(KeyCode.I))
				{
					equipGunWith (GUN_TYPE.Smilere);
				}
				else if(Input.GetKeyDown(KeyCode.O))
				{
					equipGunWith (GUN_TYPE.Sniper);
				}
				else if(Input.GetKeyDown(KeyCode.P))
				{
					setPerk (Perks_Type.EGO);
				}
				else if(Input.GetKeyDown(KeyCode.A))
				{
					setPerk (Perks_Type.RUSHER);
				}
				else if(Input.GetKeyDown(KeyCode.S))
				{
					setPerk (Perks_Type.STRATEGY);
				}
				else if(Input.GetKeyDown(KeyCode.D))
				{
					setPerk (Perks_Type.THINKER);
				}
				else if(Input.GetKeyDown(KeyCode.F))
				{
					setPerk (Perks_Type.TRY_HARD);
				}
				else if(Input.GetKeyDown(KeyCode.G))
				{
					setHelmet (Helmet_Type.BREATHOR);
				}
				else if(Input.GetKeyDown(KeyCode.H))
				{
					setHelmet (Helmet_Type.OPERATIVE);
				}
				else if(Input.GetKeyDown(KeyCode.J))
				{
					setHelmet (Helmet_Type.PILOTAR);
				}
				else if(Input.GetKeyDown(KeyCode.K))
				{
					setLaser (Laser_Type.BLUE_LASER);
				}
				else if(Input.GetKeyDown(KeyCode.L))
				{
					setLaser (Laser_Type.GREEN_LASER);
				}
				else if(Input.GetKeyDown(KeyCode.Semicolon))
				{
					setLaser (Laser_Type.RED_LASER);
				}
				else if(Input.GetKeyDown(KeyCode.Z))
				{
					setHelmet (Helmet_Type.TS_TACTICAL);
				}
				else if(Input.GetKeyDown(KeyCode.X))
				{
					saveToPrefs ();
				}
				else if(Input.GetKeyDown(KeyCode.C))
				{
					if(ObscuredPrefs.HasKey("DateAndTime"))
					{
						ObscuredPrefs.SetString ("DateAndTime", "13000");
					}
				}
			}
		}

		private void loadPlayerPrefs()
		{
			if(ObscuredPrefs.HasKey("DateAndTime"))
			{
				string value = ObscuredPrefs.GetString ("DateAndTime");
				char[] valueArray = value.ToCharArray ();

				currentExpBoostMultiplier = int.Parse (char.ToString(valueArray [0]));
				currentEquippedGunType = int.Parse (char.ToString (valueArray [1]));
				currentPerk = int.Parse (char.ToString (valueArray [2]));
				currentHelmet = int.Parse (char.ToString (valueArray [3]));
				currentLaserType = int.Parse (char.ToString (valueArray [4]));
			}
			else
			{
				string value = "13000";
				ObscuredPrefs.SetString ("DateAndTime", value);		// Key is to trick the hackers
			}
		}

		/// <summary>
		/// Sets the exp boost value which can be used to award player.
		/// Note :- The value that entered here will be multiplied to xp boost. For example if you entered 2 then 2 x (Normal exp for player)
		/// </summary>
		/// <returns>INSTANCE.</returns>
		/// <param name="multiplier">Exp boost to be multiplied</param>
		public SMProductEquipper setCurrentExpBoost(int multiplier)
		{
			currentExpBoostMultiplier = multiplier;
			return INSTANCE;
		}

		/// <summary>
		/// This function will make the player to equip the gun in entire game
		/// </summary>
		/// <returns>INSTANCE</returns>
		/// <param name="gunToEquip">Gun to equip.</param>
		public SMProductEquipper equipGunWith(GUN_TYPE gunToEquip)
		{
			currentEquippedGunType = (int)gunToEquip;
			return INSTANCE;
		}

		/// <summary>
		/// Sets the perk which will be followed by the player in his entire game.
		/// </summary>
		/// <returns>INSTANCE.</returns>
		/// <param name="perkType">Perk type to follow in game.</param>
		public SMProductEquipper setPerk(Perks_Type perkType)
		{
			currentPerk = (int)perkType;
			return INSTANCE;
		}

		/// <summary>
		/// Sets the helmet to the player. He will wear this in entire game
		/// </summary>
		/// <returns>INSTANCE</returns>
		/// <param name="helmetType">Helmet type.</param>
		public SMProductEquipper setHelmet(Helmet_Type helmetType)
		{
			currentHelmet = (int)helmetType;
			return INSTANCE;
		}

		/// <summary>
		/// Sets the laser. So that player gun will always have lasers attached in entire game
		/// </summary>
		/// <returns>INSTANCE.</returns>
		/// <param name="laserType">Laser type.</param>
		public SMProductEquipper setLaser(Laser_Type laserType)
		{
			currentLaserType = (int)laserType;
			return INSTANCE;
		}

		/// <summary>
		/// Saves all the player customization configuration to prefs
		/// </summary>
		public void saveToPrefs()
		{
			string toSave;
			toSave = ((int)currentExpBoostMultiplier).ToString();
			toSave += ((int)currentEquippedGunType).ToString ();
			toSave += ((int)currentPerk).ToString ();
			toSave += ((int)currentHelmet).ToString ();
			toSave += ((int)currentLaserType).ToString ();

			ObscuredPrefs.SetString ("DateAndTime", toSave);
		}
	}

	public enum Perks_Type
	{
		NONE = 0,
		RUSHER,
		STRATEGY,
		EGO,
		THINKER,
		TRY_HARD
	}

	public enum Helmet_Type
	{
		NONE = 0,
		PILOTAR,
		OPERATIVE,
		BREATHOR,
		TS_TACTICAL,
		MASKERA
	}

	public enum Laser_Type
	{
		NONE = 0,
		RED_LASER,
		BLUE_LASER,
		GREEN_LASER
	}
}
