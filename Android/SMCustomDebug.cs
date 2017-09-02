//
//  SMCustomDebug.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 02/09/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu.Testing
{
	/// <summary>
	/// This class helps us to show debug messages with custom tags during testing on phone
	/// </summary>
	public class SMCustomDebug
	{
		private static string DEFAULT_TAG = "UNITY_SANDEEP";

		#if UNITY_ANDROID && !UNITY_EDITOR
		/// <summary>
		/// This holds reference to android java class which shows custom debug messages
		/// </summary>
		private static AndroidJavaClass androidCustomDebugClass = new AndroidJavaClass("com.mattepu.sandeep_jultar_plugin.DebugWithCustomTags");
		#endif

		/// <summary>
		/// Shows the debug message in android monitor or in xcode debugger.
		/// </summary>
		/// <param name="message">Message to be shown.</param>
		public static void showDebugMessage(string message)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			androidCustomDebugClass.CallStatic("showCustomLogWithTag",DEFAULT_TAG,message);
			#else
			Debug.Log(DEFAULT_TAG + " " + message);
			#endif
		}

		/// <summary>
		/// Shows the debug message in android monitor or in xcode debugger.
		/// </summary>
		/// <param name="tag">Tag to filter in debugger.</param>
		/// <param name="message">Message to be shown.</param>
		public static void showDebugMessage(string tag, string message)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			androidCustomDebugClass.CallStatic("showCustomLogWithTag",tag,message);
			#else
			Debug.Log(tag + " " + message);
			#endif
		}
	}
}

