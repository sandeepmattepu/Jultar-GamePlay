//
//  SMButtonTextBehaviour.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 19/09/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SandeepMattepu.UI
{
	public class SMButtonTextBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField]
		private Text buttonText;
		[SerializeField]
		private Color pressedColor = Color.white;
		private Color tempColor;

		#region IPointerDownHandler implementation

		public void OnPointerDown (PointerEventData eventData)
		{
			if(buttonText != null)
			{
				tempColor = buttonText.color;
				buttonText.color = pressedColor;
			}
		}

		#endregion

		#region IPointerUpHandler implementation

		public void OnPointerUp (PointerEventData eventData)
		{
			if(buttonText != null)
			{
				buttonText.color = tempColor;
			}
		}

		#endregion
	}
}
