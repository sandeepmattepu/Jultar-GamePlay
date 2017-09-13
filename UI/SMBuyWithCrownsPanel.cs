//
//  SMBuyWithCrownsPanel.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 13/09/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu.UI
{
	/// <summary>
	/// This class helps to show necessary UI to player which he can decide to buy product using crowns
	/// </summary>
	public class SMBuyWithCrownsPanel : MonoBehaviour 
	{
		/// <summary>
		/// The number of crowns required UI.
		/// </summary>
		[SerializeField]
		private Text numberOfCrownsRequiredUI;
		/// <summary>
		/// The entire transaction panel.
		/// </summary>
		[SerializeField]
		private RectTransform entireTransactionPanel;
		/// <summary>
		/// This handles result
		/// </summary>
		private IHandleCrownsTransactionPanel handleResult;
		/// <summary>
		/// The product identifier of the product.
		/// </summary>
		private string Product_Identifier;

		void Start () 
		{
			entireTransactionPanel.gameObject.SetActive (false);
		}

		/// <summary>
		/// This gets called when yes button is pressed
		/// </summary>
		public void yesToBuyPressed()
		{
			if(handleResult != null)
			{
				handleResult.onHandleResult (Product_Identifier, true);
				handleResult = null;
			}
			entireTransactionPanel.gameObject.SetActive (false);
		}

		/// <summary>
		/// This gets called when no button is pressed
		/// </summary>
		public void noToBuyPressed()
		{
			if(handleResult != null)
			{
				handleResult.onHandleResult (Product_Identifier, false);
				handleResult = null;
			}
			entireTransactionPanel.gameObject.SetActive (false);
		}

		/// <summary>
		/// Starts the crowns transaction panel.
		/// </summary>
		/// <param name="crownsRequired">Crowns required for purchase.</param>
		/// <param name="productIdentifier">Product identifier.</param>
		/// <param name="handler">Handler to handle result.</param>
		public void startCrownsTransactionPanel(int crownsRequired, string productIdentifier, IHandleCrownsTransactionPanel handler)
		{
			if(entireTransactionPanel.gameObject.activeInHierarchy)
			{
				// Means already in other transaction. Fail this transaction
				handler.onHandleResult(productIdentifier, false);
				return;
			}
			else
			{
				entireTransactionPanel.gameObject.SetActive (true);
				handleResult = handler;
				numberOfCrownsRequiredUI.text = "- " + crownsRequired.ToString () + " C";
				Product_Identifier = productIdentifier;
			}
		}

		/// <summary>
		/// Handle transaction result
		/// </summary>
		public interface IHandleCrownsTransactionPanel
		{
			void onHandleResult(string productIdentifier, ObscuredBool hasPurchased);
		}
	}
}