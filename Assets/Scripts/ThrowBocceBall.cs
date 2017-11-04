using System.Collections;
using System;
using UnityEngine;

public class ThrowBocceBall : MonoBehaviour 
{
	public BocceBallInfo Pallino;
	public BocceBallInfo BocceBallPlayerOne;
	public BocceBallInfo BocceBallPlayerTwo;

	public event EventHandler<BocceEventArgs> ThrowComplete;

	private BocceBallInfo currentBocceBall = null;

	public void StartThrow(int turn, bool player)
	{
		var randomX = UnityEngine.Random.Range(-6f, 6f);
		var randomZ = UnityEngine.Random.Range(-10f, 40f);
		if(turn > 0)
		{
			currentBocceBall = (BocceBallInfo)Instantiate(player ? BocceBallPlayerOne : BocceBallPlayerTwo, new Vector3(randomX, 1.15f, randomZ), Quaternion.identity);
			currentBocceBall.BoccePlayer = player;
		}
		else
		{
			currentBocceBall = (BocceBallInfo)Instantiate(Pallino, new Vector3(randomX, 0.75f,randomZ), Quaternion.identity);
		}
		StartCoroutine(TemporaryThrow());
	}

	private void EndThrow()
	{
		var handler = ThrowComplete;
		if(handler != null)
		{
			handler(this, new BocceEventArgs(currentBocceBall));
		}
	}

	private IEnumerator TemporaryThrow()
	{
		yield return new WaitForSeconds(2f);
		EndThrow();
	}
}
