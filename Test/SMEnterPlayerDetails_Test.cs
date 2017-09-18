using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.Android;

public class SMEnterPlayerDetails_Test : MonoBehaviour
{
	private bool canSave = false;
	private int levelValue = 1;
	private int additionalXp = 0;
	// Use this for initialization
	void Start () 
	{
		SMPlayerDataManager.loadData ();
		StartCoroutine ("setCanSave");
	}

	private IEnumerator setCanSave()
	{
		yield return new WaitUntil (() => SMPlayerDataManager.PlayerData != null);
		canSave = true;
	}

	public void saveButtonPressed()
	{
		if(canSave)
		{
			SMPlayerDataManager.PlayerData.playerLevel = levelValue;
			SMPlayerDataManager.PlayerData.additionalXp = additionalXp;
			SMPlayerDataManager.saveData ();
		}
	}

	public void currentLevelValueTextField(string value)
	{
		int tempValue = levelValue;
		if(int.TryParse(value, out tempValue))
		{
			levelValue = tempValue;
		}
	}

	public void currentXpValueTextField(string value)
	{
		int tempValue = additionalXp;
		if(int.TryParse(value, out tempValue))
		{
			additionalXp = tempValue;
		}
	}
}
