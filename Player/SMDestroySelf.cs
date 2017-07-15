//
//  SMDestroySelf.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 31/05/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu
{
	/// <summary>
	/// This class will destroy the game object that attached to it
	/// </summary>
	public class SMDestroySelf : MonoBehaviour 
	{
		/// <summary>
		/// This value will determine the lifespan of the gameobject in seconds
		/// </summary>
		public float lifeSpan;
		// Use this for initialization
		void Start () 
		{
			Destroy (this.gameObject, lifeSpan);
		}
	}	
}
