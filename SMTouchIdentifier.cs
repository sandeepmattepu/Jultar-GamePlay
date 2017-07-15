//
//  TouchIdentifier.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 04/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandeepMattepu.MobileTouch
{
	/// <summary>
	/// This class is used to create unique touch objects which can be later compared.
	/// </summary>
	public class SMTouchIdentifier 
	{
		private Touch _touch;
		/// <summary>
		/// Contains the touch value
		/// </summary>
		public Touch touch
		{
			get	{ 	return _touch;	}
		}
		private int _fingerID;
		/// <summary>
		/// This is the finger id of the touch
		/// </summary>
		public int fingerID
		{
			get{ 	return _fingerID;	}
		}
		private int _layerBelongTo;
		/// <summary>
		/// This will represent which layer the touch belong to in integer form
		/// </summary>
		public int layerBelongTo
		{
			get{ 	return _layerBelongTo;	}
		}
		private TouchBelongs _touchBelongsTo;
		/// <summary>
		/// This will represent which layer the touch belong to in enum form
		/// </summary>
		public TouchBelongs touchBelongsTo
		{
			get{ 	return _touchBelongsTo;		}	
		}

		/// <summary>
		/// Creates the instance of TouchIdentifier which can be used to uniquely identify the touch
		/// </summary>
		/// <param name="touchInput">Pass the touch value</param>
		/// <param name="layerBelongsTo">Pass where touch touches the unity layer as int value</param>
		public SMTouchIdentifier(Touch touchInput, int layerBelongsTo)
		{
			this._touch = touchInput;
			this._fingerID = touch.fingerId;
			this._layerBelongTo = layerBelongsTo;
			processTouchBelongsTo (layerBelongTo);
		}

		/// <summary>
		/// This function will process where this touch belongs to based on the layer number passed to it
		/// </summary>
		/// <param name="layerNumber">Pass where touch touches the unity layer as int value</param>
		private void processTouchBelongsTo(int layerNumber)
		{
			if(layerNumber != 5)
			{
				_touchBelongsTo = TouchBelongs.GAME;
			}
			else
			{
				_touchBelongsTo = TouchBelongs.UI;
			}
		}
	}

	/// <summary>
	/// This enum describes where the touch belongs to
	/// </summary>
	public enum TouchBelongs
	{
		UI,
		GAME
	}

}