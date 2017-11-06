using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles scoring the game.
/// </summary>
public class BocceScore : MonoBehaviour 
{
	/// <summary>
	/// The player one score text.
	/// </summary>
	[SerializeField]
	private Text PlayerOneScoreText;
	
	/// <summary>
	/// The player two score text.
	/// </summary>
	[SerializeField]
	private Text PlayerTwoScoreText;
	
	/// <summary>
	/// The game over text.
	/// </summary>
	[SerializeField]
	private Text GameOverText;

	/// <summary>
	/// The balls manager.
	/// </summary>
	private BocceBallsManager ballsManager;

	/// <summary>
	/// The turns manager.
	/// </summary>
	private BocceTurnsManager turnsManager;

	/// <summary>
	/// The player one score.
	/// </summary>
	private int PlayerOneScore = 0;

	/// <summary>
	/// The player two score.
	/// </summary>
	private int PlayerTwoScore = 0;

	/// <summary>
	/// The game over text format.
	/// </summary>
	private string GameOverTextFormat = "GAME OVER! {0} WINS!!!!";

	/// <summary>
	/// Unity Awake method.
	/// </summary>
	private void Awake()
	{
		ballsManager = GetComponent<BocceBallsManager>();
		if(ballsManager == null)
		{
			Debug.LogError("No BocceBallsManager");
		}

		turnsManager = GetComponent<BocceTurnsManager>();
		if(turnsManager == null)
		{
			Debug.LogError("No Turns Manager");
		}

		turnsManager.GameStarted += HandleGameStarted;
		turnsManager.GameEnded += HandleGameEnded;
	}

	/// <summary>
	/// Unity OnDestroy method.
	/// </summary>
	private void OnDestroy()
	{
		turnsManager.GameStarted -= HandleGameStarted;
		turnsManager.GameEnded -= HandleGameEnded;
	}
	
	/// <summary>
	/// Handles the game started.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void HandleGameStarted(object sender, EventArgs e)
	{
		ResetGame();
	}

	/// <summary>
	/// Handles the game ended.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void HandleGameEnded(object sender, EventArgs e)
	{
		ScoreGame();
		CheckEndGame();
	}

	/// <summary>
	/// Scores the game.
	/// </summary>
	private void ScoreGame()
	{
		var closest = ballsManager.ClosestBallFromPallino().BoccePlayer.ToString().Contains("1");
		var closestTeamCount = ballsManager.NumberOfTeamMembersClosestToPallino();
		if(closest)
		{
			PlayerOneScore += closestTeamCount;
		}
		else
		{
			PlayerTwoScore += closestTeamCount;
		}
		UpdateScoreText();
	}

	/// <summary>
	/// Resets the game.
	/// </summary>
	private void ResetGame()
	{
		GameOverText.gameObject.SetActive(false);

		PlayerOneScore = 0;
		PlayerTwoScore = 0;

		UpdateScoreText();
	}

	/// <summary>
	/// Checks the end game.
	/// </summary>
	private void CheckEndGame()
	{
		if(PlayerOneScore >= 2 || PlayerTwoScore >= 2)
		{
			GameOverText.gameObject.SetActive(true);
			GameOverText.text = string.Format(GameOverTextFormat, PlayerOneScore > PlayerTwoScore ? "PLAYER ONE" : "PLAYER TWO");
			StartCoroutine(HoldScoreForGameOver());
		}
	}

	/// <summary>
	/// Updates the score text.
	/// </summary>
	private void UpdateScoreText()
	{
		PlayerOneScoreText.text = PlayerOneScore.ToString();
		PlayerTwoScoreText.text = PlayerTwoScore.ToString();
	}

	private IEnumerator HoldScoreForGameOver()
	{
		yield return new WaitForSeconds(4f);
		ResetGame();
		turnsManager.InitializeBocceGame();
	}
}
