using System;
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

	/// <summary>
	/// The pallino camera.
	/// </summary>
	[SerializeField]
	private Camera pallinoCamera;

	#endregion

	#region Player enum
	
	public enum Player
	{
		PLAYER1,
		PLAYER2
	}
	
	#endregion

	#region Events

	/// <summary>
	/// Occurs when game started.
	/// </summary>
	public event EventHandler GameStarted;

	/// <summary>
	/// Occurs when game ended.
	/// </summary>
	public event EventHandler GameEnded;

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
	/// The take A break coroutine.
	/// </summary>
	private Coroutine takeABreakCoroutine;

	#endregion

	#region UI elements for turns manager

	/// <summary>
	/// The player turn string format.
	/// </summary>
	private string playerTurnFormat = "PLAYER {0}'S TURN";

	/// <summary>
	/// The player turn text.
	/// </summary>
	[SerializeField]
	private Text PlayerTurnText;

	#endregion

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

		if(numberOfPlayers % 2 != 0)
		{
			numberOfPlayers++;
		}

		numberOfPlayerOneMembers = numberOfPlayers / 2;
		numberOfPlayerTwoMembers = numberOfPlayers / 2;

		currentTurn = 0;
		currentPlayer = Player.PLAYER1;
		takeABreakCoroutine = null;
	}

	/// <summary>
	/// Temporary to start the game.
	/// </summary>
	private void Start()
	{
		InitializeBocceGame();
	}

	/// <summary>
	/// Unity OnDestroy method.
	/// </summary>
	private void OnDestroy()
	{
		throwBallComponent.ThrowComplete -= HandleThrowComplete;
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
		DetermineNextTurn();
	}

	#endregion

	#region Helpers

	/// <summary>
	/// Initializes the bocce game.
	/// </summary>
	public void InitializeBocceGame()
	{
		pallinoCamera.enabled = false;

		if(takeABreakCoroutine != null)
		{
			StopCoroutine(takeABreakCoroutine);
		}

		StartGame();

		var handler = GameStarted;
		if(handler != null)
		{
			handler(this, null);
		}
	}

	/// <summary>
	/// Starts the game.
	/// </summary>
	private void StartGame()
	{
		pallinoCamera.enabled = false;

		ResetGame();
		SetCurrentPlayer();
		UpdateTurnText();

		throwBallComponent.ThrowComplete += HandleThrowComplete;
		throwBallComponent.StartThrow(currentTurn, currentPlayer);
	}

	/// <summary>
	/// Ends the game.
	/// </summary>
	private void EndGame()
	{
		throwBallComponent.ThrowComplete -= HandleThrowComplete;

		//Just an added bonus to see who the closest balls are to the pallino at the end of a round.
		var pallinoCamPosition = BocceBallsUtility.PallinoLocation();
		pallinoCamPosition.y = pallinoCamera.transform.position.y;
		pallinoCamera.transform.position = pallinoCamPosition;
		pallinoCamera.enabled = true;

		takeABreakCoroutine = StartCoroutine(TakeABreak());

		var handler = GameEnded;
		if(handler != null)
		{
			handler(this, null);
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
		
		BocceBallsUtility.ClearBocceBalls();
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
		
		var closestBall = BocceBallsUtility.ClosestBallFromPallino();
		
		//return the team which does not have their ball closest to the pallino.
		return GetOpposingPlayer(closestBall.BoccePlayer);
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
		return UnityEngine.Random.Range(0,2) == 0 ? Player.PLAYER1 : Player.PLAYER2;
	}

	/// <summary>
	/// Takes A break.
	/// </summary>
	/// <returns>The A break.</returns>
	private IEnumerator TakeABreak()
	{
		yield return new WaitForSeconds(5f);
		StartGame();
	}

	#endregion
}
