using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BocceTurnsManager : MonoBehaviour 
{
	private ThrowBocceBall throwBallComponent;
	private int numberOfPlayers = 8;
	private int numberOfPlayerOneMembers = 4;
	private int numberOfPlayerTwoMembers = 4;
	private int currentTurn = 0;	
	private bool currentPlayer = true;
	private List<BocceBallInfo> BocceBalls;
	private BocceBallInfo Pallino = null;

	//Score
	private int PlayerOneScore = 0;
	private int PlayerTwoScore = 0;

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
			EndGame();
		}
		else
		{
			switch(currentTurn)
			{
				case 1:	//whoever threw the pallino is who will throw in turn 1.
					break;
				case 2: //palyer from opposing team will throw.
					currentPlayer = !currentPlayer;
					break;
				default:
					currentPlayer = GetNextPlayer();
					break;
			}

			if(currentPlayer)
			{
				numberOfPlayerOneMembers--;
			}
			else
			{
				numberOfPlayerTwoMembers--;
			}
			throwBallComponent.StartThrow(currentTurn, currentPlayer);
		}
	}


	private void StartGame()
	{
		Reset();
		Debug.Log("Starting Game");
		Debug.Log(string.Format("Scores - Player1: {0}---Player2: {1}", PlayerOneScore, PlayerTwoScore));
		//First turn random person throws pallino.
		currentPlayer = Random.Range(0,2) == 0;

		throwBallComponent.ThrowComplete += HandleThrowComplete;
		throwBallComponent.StartThrow(currentTurn, currentPlayer);
	}

	private void EndGame()
	{
		Debug.Log("Ending Game");
		throwBallComponent.ThrowComplete -= HandleThrowComplete;
		ScoreGame();
		Debug.Log(string.Format("Scores - Player1: {0}---Player2: {1}", PlayerOneScore, PlayerTwoScore));

		if(PlayerOneScore >= 7 || PlayerTwoScore >= 7)
		{
			Debug.Log("GAME OVER");
		}
		else
		{
			StartCoroutine(TakeABreak());
		}
	}

	private bool GetNextPlayer()
	{
		if(numberOfPlayerOneMembers == 0)
		{
			return false;
		}

		if(numberOfPlayerTwoMembers == 0)
		{
			return true;
		}

		SetDistancesFromPallino();

		//return the team which does not have their ball closest to the pallino.
		return !BocceBalls.FirstOrDefault().BoccePlayer;
	}

	private void SetDistancesFromPallino()
	{
		if(Pallino != null && BocceBalls.Count > 0)
		{
			foreach(var ball in BocceBalls)
			{
				ball.DistanceFromPallino = Vector3.Distance(Pallino.transform.position, ball.transform.position);
			}
			BocceBalls = BocceBalls.OrderBy(ballInfo => ballInfo.DistanceFromPallino).ToList();
		}
	}

	private void ScoreGame()
	{
		SetDistancesFromPallino();

		var playerAhead = BocceBalls.FirstOrDefault ().BoccePlayer;
		foreach(var ball in BocceBalls)
		{
			if(playerAhead != ball.BoccePlayer)
			{
				break;
			}

			if(playerAhead)
			{
				PlayerOneScore++;
			}
			else
			{
				PlayerTwoScore++;
			}
		}
	}

	private IEnumerator TakeABreak()
	{
		Debug.Log("Taking a break");
		yield return new WaitForSeconds(5f);
		StartGame();
	}

	private void Reset()
	{
		currentTurn = 0;
		currentPlayer = true;

		numberOfPlayerOneMembers = numberOfPlayers / 2;
		numberOfPlayerTwoMembers = numberOfPlayers / 2;

		foreach(var ball in BocceBalls)
		{
			Destroy(ball.gameObject);
		}

		BocceBalls.Clear();

		if(Pallino != null)
		{
			Destroy(Pallino.gameObject);
			Pallino = null;
		}
	}
}
