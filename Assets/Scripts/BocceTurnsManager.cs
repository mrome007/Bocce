using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runs through player turns of a Bocce game.
/// </summary>
public class BocceTurnsManager : MonoBehaviour 
{
	#region Inspector Fields

	/// <summary>
	/// The number of players.
	/// </summary>
	[SerializeField]
	private int numberOfPlayers = 8;

	#endregion

	#region Player enum
	
	public enum Player
	{
		PLAYER1,
		PLAYER2
	}
	
	#endregion

	#region Private Fields

	/// <summary>
	/// The ThrowBocceBall component.
	/// </summary>
	private ThrowBocceBall throwBallComponent;

	/// <summary>
	/// The number of PLAYER ONE members.
	/// </summary>
	private int numberOfPlayerOneMembers;

	/// <summary>
	/// The number of PLAYER TWO members.
	/// </summary>
	private int numberOfPlayerTwoMembers;

	/// <summary>
	/// The current turn number.
	/// </summary>
	private int currentTurn = 0;

	/// <summary>
	/// The current player throwing.
	/// </summary>
	private Player currentPlayer;

	/// <summary>
	/// The bocce balls thrown.
	/// </summary>
	private List<BocceBallInfo> BocceBalls;

	/// <summary>
	/// The pallino.
	/// </summary>
	private BocceBallInfo Pallino;

	#endregion

	//Score
	private int PlayerOneScore = 0;
	private int PlayerTwoScore = 0;

	public Text PlayerOneScoreText;
	public Text PlayerTwoScoreText;

	private string playerTurnFormat = "PLAYER {0}'S TURN";
	public Text PlayerTurnText;

	#region MonoBehaviour methods

	/// <summary>
	/// Unity Awake method.
	/// </summary>
	private void Awake()
	{
		throwBallComponent = GetComponent<ThrowBocceBall>();
		if(throwBallComponent == null)
		{
			Debug.LogError("No Throw Bocce Ball Component");
		}

		BocceBalls = new List<BocceBallInfo>();
		Pallino = null;

		numberOfPlayerOneMembers = numberOfPlayers / 2;
		numberOfPlayerTwoMembers = numberOfPlayers / 2;

		currentTurn = 0;
		currentPlayer = Player.PLAYER1;
	}

	/// <summary>
	/// Temporary to start the game.
	/// </summary>
	private void Start()
	{
		StartGame();
	}

	#endregion

	#region Event handlers

	/// <summary>
	/// Handles the event when throwing the ball is complete.
	/// Stores the bocce balls thrown to their respective category
	/// and figures out which player will throw next.
	/// </summary>
	/// <param name="sender">Object that triggers the event.</param>
	/// <param name="e">BocceEventArgs contains info regarding ball thrown.</param>
	private void HandleThrowComplete(object sender, BocceEventArgs e)
	{
		if(currentTurn > 0)
		{
			BocceBalls.Add(e.BocceBallInfo);
		}
		else
		{
			Pallino = e.BocceBallInfo;
		}

		DetermineNextTurn();
	}

	#endregion

	#region Helpers

	/// <summary>
	/// Starts the game.
	/// </summary>
	private void StartGame()
	{
		ResetGame();
		SetCurrentPlayer();

		throwBallComponent.ThrowComplete += HandleThrowComplete;
		throwBallComponent.StartThrow(currentTurn, currentPlayer);
	}

	/// <summary>
	/// Ends the game.
	/// </summary>
	private void EndGame()
	{
		throwBallComponent.ThrowComplete -= HandleThrowComplete;
		ScoreGame();
		Debug.Log("Ending Game");
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

	/// <summary>
	/// Resets the game.
	/// </summary>
	private void ResetGame()
	{
		currentTurn = 0;
		currentPlayer = Player.PLAYER1;
		
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

	/// <summary>
	/// Figures out the Bocce Balls' distances from the Pallino.
	/// and sorts them in ascending order based on their distance
	/// from the Pallino.
	/// </summary>
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

	/// <summary>
	/// Scores the game.
	/// </summary>
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

			if(playerAhead == Player.PLAYER1)
			{
				PlayerOneScore++;
			}
			else
			{
				PlayerTwoScore++;
			}
		}
		UpdateScoreText();
	}

	private void UpdateScoreText()
	{
		PlayerOneScoreText.text = PlayerOneScore.ToString();
		PlayerTwoScoreText.text = PlayerTwoScore.ToString();
	}

	private void UpdateTurnText()
	{
		PlayerTurnText.text = string.Format(playerTurnFormat, currentPlayer == Player.PLAYER1 ? "ONE" : "TWO");
	}

	/// <summary>
	/// Gets the next player, choosing from either player one or two based on Bocce Rules.
	/// </summary>
	/// <returns><c>true</c>, if next player was gotten, <c>false</c> otherwise.</returns>
	private Player GetNextPlayer()
	{
		if(numberOfPlayerOneMembers == 0)
		{
			return Player.PLAYER2;
		}
		
		if(numberOfPlayerTwoMembers == 0)
		{
			return Player.PLAYER1;
		}
		
		SetDistancesFromPallino();
		
		//return the team which does not have their ball closest to the pallino.
		return GetOpposingPlayer(BocceBalls.FirstOrDefault().BoccePlayer);
	}

	/// <summary>
	/// Determines what happens the next turn.
	/// </summary>
	private void DetermineNextTurn()
	{
		currentTurn++;
		
		if(currentTurn >= numberOfPlayers + 1)
		{
			EndGame();
			return;
		}

		SetCurrentPlayer();
			
		if(currentPlayer == Player.PLAYER1)
		{
			numberOfPlayerOneMembers--;
		}
		else
		{
			numberOfPlayerTwoMembers--;
		}
		throwBallComponent.StartThrow(currentTurn, currentPlayer);

		UpdateTurnText();
	}

	/// <summary>
	/// Sets the current player.
	/// Turn 0 - Pick random player that will throw the Pallino.
	/// Turn 1 - Same team will throw the first Bocce ball.
	/// Turn 2 - Now the opposing team will throw the next Bocce ball.
	/// Turn n - Player is determined by what team is not closest to the Pallino.
	/// </summary>
	private void SetCurrentPlayer()
	{
		switch(currentTurn)
		{
			case 0:
				currentPlayer = GetRandomPlayer();
				break;
			case 1:
				break;
			case 2:
				currentPlayer = GetOpposingPlayer(currentPlayer);
				break;
			default:
				currentPlayer = GetNextPlayer();
				break;
		}
	}

	/// <summary>
	/// Gets the opposing player.
	/// </summary>
	/// <returns>The opposing player.</returns>
	/// <param name="player">player type</param>
	private Player GetOpposingPlayer(Player player)
	{
		return player == Player.PLAYER1 ? Player.PLAYER2 : Player.PLAYER1;
	}

	/// <summary>
	/// Gets a random player.
	/// </summary>
	/// <returns>The random player.</returns>
	private Player GetRandomPlayer()
	{
		return Random.Range(0,2) == 0 ? Player.PLAYER1 : Player.PLAYER2;
	}

	/// <summary>
	/// Takes A break.
	/// </summary>
	/// <returns>The A break.</returns>
	private IEnumerator TakeABreak()
	{
		Debug.Log("Taking a break");
		yield return new WaitForSeconds(5f);
		StartGame();
	}

	#endregion
}
