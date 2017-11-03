using UnityEngine;
using System.Collections;

public class BocceBallInfo : MonoBehaviour 
{
	public enum Player
	{
		Player1,
		Player2
	}

	public Player CurrentPlayer { get; private set; }

	public void SetCurrentPlayer(bool playerOne)
	{
		CurrentPlayer = playerOne ? Player.Player1 : Player.Player2;
	}
}
