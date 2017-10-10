//
//  SMNewsFeed.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 28/09/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandeepMattepu.Web
{
	/// <summary>
	/// This class loads the server for news feed which will be displayed to player
	/// </summary>
	public class SMNewsFeed : MonoBehaviour
	{
		/// <summary>
		/// The user interface to display news feed.
		/// </summary>
		[SerializeField]
		private Text uiToDisplayNewsFeed;
		/// <summary>
		/// The web address of news feed.
		/// </summary>
		public string webAddressOfNewsFeed = "http://mattepu.com/NewsFeed.php";

		void Start () 
		{
			retriveDataFromServer ();
		}

		/// <summary>
		/// This function retrives the data from server.
		/// </summary>
		public void retriveDataFromServer()
		{
			StartCoroutine ("waitAndLoadData");
		}

		private IEnumerator waitAndLoadData()
		{
			WWW request = new WWW (webAddressOfNewsFeed);
			yield return request;
			if(uiToDisplayNewsFeed != null)
			{
				uiToDisplayNewsFeed.text = request.text;
			}
			else
			{
				Debug.Log (request.text);
			}
		}
	}
}
