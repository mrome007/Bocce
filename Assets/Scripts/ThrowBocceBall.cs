using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// Class that handles throwing the Bocce Ball.
/// </summary>
public class ThrowBocceBall : MonoBehaviour 
{
	#region Inpector Fields

	/// <summary>
	/// The pallino prefab.
	/// </summary>
	[SerializeField]
	private BocceBallInfo pallino;

	/// <summary>
	/// The PLAYER ONE bocce ball prefab.
	/// </summary>
	[SerializeField]
	private BocceBallInfo playerOneBocceBall;

	/// <summary>
	/// The PLAYER TWO bocce ball prefab.
	/// </summary>
	[SerializeField]
	private BocceBallInfo playerTwoBocceBall;

	#endregion

	#region Events

	/// <summary>
	/// Occurs when throw is complete.
	/// </summary>
	public event EventHandler<BocceEventArgs> ThrowComplete;

	#endregion

	#region Private fields

	/// <summary>
	/// The current bocce ball being thrown.
	/// </summary>
	private BocceBallInfo currentBocceBall = null;

	#endregion

	#region Throw methods

	/// <summary>
	/// Starts the throw.
	/// </summary>
	/// <param name="turn">Used to determine what type of ball to throw. Pallino or Bocce ball</param>
	/// <param name="player">Used to determine which Player's Bocce ball to throw. PLAYER ONE or PLAYER TWO.</param>
	public void StartThrow(int turn, BocceTurnsManager.Player player)
	{
		//Temporary for now. Just using set position.
		var randomX = UnityEngine.Random.Range(-6f, 6f);
		var randomZ = UnityEngine.Random.Range(-10f, 40f);

		if(turn > 0)
		{
			currentBocceBall = (BocceBallInfo)Instantiate(player == BocceTurnsManager.Player.PLAYER1 ? playerOneBocceBall : playerTwoBocceBall, 
			                                              new Vector3(randomX, 1.15f, randomZ), Quaternion.identity);
			currentBocceBall.BoccePlayer = player;
		}
		else
		{
			currentBocceBall = (BocceBallInfo)Instantiate(pallino, new Vector3(randomX, 0.75f,randomZ), Quaternion.identity);
		}

		StartCoroutine(TemporaryThrow());
	}

	/// <summary>
	/// Ends the throw and triggers the Throw Complete event.
	/// </summary>
	private void EndThrow()
	{
		var handler = ThrowComplete;
		if(handler != null)
		{
			handler(this, new BocceEventArgs(currentBocceBall));
		}
	}

	/// <summary>
	/// Temporaries the throw.
	/// </summary>
	/// <returns>The throw.</returns>
	private IEnumerator TemporaryThrow()
	{
		yield return new WaitForSeconds(2f);
		EndThrow();
	}

	#endregion
}
