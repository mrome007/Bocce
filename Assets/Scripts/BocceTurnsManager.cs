using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BocceTurnsManager : MonoBehaviour 
{
	private ThrowBocceBall throwBallComponent;
	private int numberOfPlayers = 6;
	private int numberOfPlayerOneMembers = 3;
	private int numberOfPlayerTwoMembers = 3;
	private int currentTurn = 0;	
	private bool currentPlayer = true;
	private List<BocceBallInfo> BocceBalls;
	private BocceBallInfo Pallino;

	private void Awake()
	{
		throwBallComponent = GetComponent<ThrowBocceBall>();
		if(throwBallComponent == null)
		{
			Debug.LogError("No Throw Bocce Ball Component");
		}

		BocceBalls = new List<BocceBallInfo>();

		numberOfPlayerOneMembers = numberOfPlayers / 2;
		numberOfPlayerTwoMembers = numberOfPlayers / 2;

		currentTurn = 0;
		currentPlayer = true;
	}

	private void Start()
	{
		StartGame();
	}

	private void HandleThrowComplete (object sender, BocceEventArgs e)
	{
		if(currentTurn > 0)
		{
			BocceBalls.Add(e.BocceBallInfo);
		}
		else
		{
			Pallino = e.BocceBallInfo;
		}
		currentTurn++;
		if(currentTurn >= numberOfPlayers + 1)
		{
			throwBallComponent.ThrowComplete -= HandleThrowComplete;
		}
		else
		{
			throwBallComponent.StartThrow(currentTurn, currentPlayer);
		}
	}


	private void StartGame()
	{
		throwBallComponent.ThrowComplete += HandleThrowComplete;
		throwBallComponent.StartThrow(currentTurn, currentPlayer);
	}
}
