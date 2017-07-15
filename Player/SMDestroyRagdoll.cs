//
//  SMDestroyRagdoll.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 14/04/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using UnityEngine;

namespace SandeepMattepu
{
	/// <summary>
	/// This class will destroy the ragdoll after few seconds
	/// </summary>
	public class SMDestroyRagdoll : MonoBehaviour
	{
		public bool isMultiplayer = false;
		// Use this for initialization
		void Start()
		{
			if(isMultiplayer)
			{
				StartCoroutine("destroyRagdoll");
			}
		}

		IEnumerator destroyRagdoll()
		{
			yield return new WaitForSeconds(5.0f);
			Destroy(this.gameObject);
		}
	}	
}
