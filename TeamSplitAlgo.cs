using System.Collections.Generic;
using UnityEngine;

public class TeamSplitAlgo : MonoBehaviour 
{
	public int noOfTeams = 2;
	public int[] randNumber;
	private Dictionary<int, int> randNumAndTeam = new Dictionary<int, int>();
	private List<int> players = new List<int> ();
	// Use this for initialization
	void Start ()
	{
		splitIntoTeam();
		printTeamNumbers();
		players = sortPlayersInOrder (new List<int> (randNumber));
		printOrder ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void splitIntoTeam()
	{
		List<int> members = new List<int>(randNumber);
		int teamIndex = 1;
		int tempTeam = noOfTeams;

		while(members.Count > 0)
		{
			int countInTeam = (members.Count / tempTeam);
			while(countInTeam > 0)
			{
				randNumAndTeam.Add(members[0],teamIndex);
				members.RemoveAt(0);
				countInTeam--;
			}
			teamIndex++;
			tempTeam--;
		}
	}

	private void printTeamNumbers()
	{
		foreach(KeyValuePair<int,int> answer in randNumAndTeam)
		{
			Debug.Log("Index with " + answer.Key + " has team as " + answer.Value);
		}
	}

	private List<int> sortPlayersInOrder(List<int> players)
	{
		List<int> orderedPlayers = new List<int>();
		foreach(int player in players)
		{
			if(orderedPlayers.Count != 0)
			{
				for(int i = 0; i < orderedPlayers.Count; i++)
				{
					if(player < orderedPlayers[i])
					{
						orderedPlayers.Insert (i, player);
						break;
					}
					else if(i == (orderedPlayers.Count - 1))
					{
						orderedPlayers.Add (player);
						break;
					}
				}
			}
			else
			{
				orderedPlayers.Add (player);
			}
		}
		return orderedPlayers;
	}

	private void printOrder()
	{
		foreach (int value in players) 
		{
			Debug.Log ("My value is " + value);
		}
	}
}
