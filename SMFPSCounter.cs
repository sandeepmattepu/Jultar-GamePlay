//
//  SMFPSCounter.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 27/04/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandeepMattepu
{
    /// <summary>
    /// This class counts the number of frames happened in one second and displayes it on the screen
    /// </summary>
	public class SMFPSCounter : MonoBehaviour
	{
        /// <summary>
        /// This timer variable ticks every frame and returns to zero after every one second
        /// </summary>
        private float timer = 0.0f;
        /// <summary>
        /// This value indicates whether to update FPS value on screen or not
        /// </summary>
        private bool canUpdateFPS = false;
        /// <summary>
        /// This variable counts the number of framers that happened in one second
        /// </summary>
        private int fpsCounter = 0;
        /// <summary>
        /// This variable stores number of frames that happened in last second
        /// </summary>
        private int lastSecondFPSValue = 0;
        /// <summary>
        /// The GUI text component.
        /// </summary>
        private GUIText guiTextComponent;
		// Use this for initialization
		void Start()
		{
            guiTextComponent = GetComponent<GUIText>();
		}

		// Update is called once per frame
		void Update()
		{
            fpsCounter++;
            timer += Time.deltaTime;
			if (timer >= 1.0f)
			{
                timer -= 1.0f;
                canUpdateFPS = true;

                if(guiTextComponent != null)
                {
                    guiTextComponent.text = "FPS : " + fpsCounter.ToString();
                    canUpdateFPS = false;
                    lastSecondFPSValue = fpsCounter;
                    fpsCounter = 0;
                }
			}
            else
            {
                if(guiTextComponent != null)
                {
                    guiTextComponent.text = "FPS : " + lastSecondFPSValue.ToString();
                }
            }
		}

        void OnGUI()
        {
            if(guiTextComponent == null)
            {
				if (canUpdateFPS)
				{
					canUpdateFPS = false;
					GUI.Label(new Rect(20, 20, 200, 40), fpsCounter.ToString());
					lastSecondFPSValue = fpsCounter;
					fpsCounter = 0;
				}
				else
				{
					GUI.Label(new Rect(20, 20, 200, 40), lastSecondFPSValue.ToString());
				}
            }
        }
	}
}
