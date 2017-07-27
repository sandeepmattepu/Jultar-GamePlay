//
//  SMUIManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 27/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SandeepMattepu.UI
{
	public class SMUIManager : MonoBehaviour 
	{
		public RectTransform areYouSureExitPanel;

		void Start()
		{
			areYouSureExitPanel.gameObject.SetActive (false);
		}

		public void exitButtonPressed()
		{
			areYouSureExitPanel.gameObject.SetActive (true);
		}

		public void yesSureToExitPressed(string sceneName)
		{
			SceneManager.LoadScene (sceneName);
		}

		public void noSureToExitPressed()
		{
			areYouSureExitPanel.gameObject.SetActive (false);
		}
	}	
}
