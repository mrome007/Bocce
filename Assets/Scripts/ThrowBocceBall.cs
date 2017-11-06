using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

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

	/// <summary>
	/// The bocce ball aim transform.
	/// </summary>
	[SerializeField]
	private Transform BocceBallAimTransform;

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

	public enum ThrowMode
	{
		NOTHROW,
		AIM,
		FIRE,
		THROW
	}

	private ThrowMode currentThrowMode = ThrowMode.NOTHROW;
	private float timerThrow = 0.65f;
	private float pallinoForce = 100f;
	private float bocceBallForce = 500f;
	private float currentForce;

	public Slider ForceMeter;
	private float forceValue = 1f;
	private float forceIncrement = 1f;
	#region Throw methods

	/// <summary>
	/// Starts the throw.
	/// </summary>
	/// <param name="turn">Used to determine what type of ball to throw. Pallino or Bocce ball</param>
	/// <param name="player">Used to determine which Player's Bocce ball to throw. PLAYER ONE or PLAYER TWO.</param>
	public void StartThrow(int turn, BocceTurnsManager.Player player)
	{
		if(turn > 0)
		{
			currentBocceBall = (BocceBallInfo)Instantiate(player == BocceTurnsManager.Player.PLAYER1 ? playerOneBocceBall : playerTwoBocceBall, 
			                                              new Vector3(0f, 1.15f, -14.5f), Quaternion.identity);
			currentBocceBall.BoccePlayer = player;
			currentForce = bocceBallForce;
		}
		else
		{
			currentBocceBall = (BocceBallInfo)Instantiate(pallino, new Vector3(0, 1f,-14.5f), Quaternion.identity);
			currentForce = pallinoForce;
		}
		currentThrowMode = ThrowMode.AIM;
	}

	/// <summary>
	/// Ends the throw and triggers the Throw Complete event.
	/// </summary>
	private void EndThrow()
	{
		currentThrowMode = ThrowMode.NOTHROW;
		timerThrow = 0.65f;

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
		//This is just to wait for every ball to stop.
		yield return new WaitForSeconds(10f);
		EndThrow();
	}

	#endregion

	#region Monobehaviour methods

	private void Update()
	{
		if(currentThrowMode == ThrowMode.AIM)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				currentThrowMode = ThrowMode.FIRE;
			}
		}
		else if(currentThrowMode == ThrowMode.FIRE)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				currentThrowMode = ThrowMode.THROW;
				currentForce = (forceValue / 100f) * currentForce;
			}
		}
	}

	private void FixedUpdate()
	{
		if(currentThrowMode == ThrowMode.AIM)
		{
			ForceMeter.value = 1f;
			forceValue = 1f;
			forceIncrement = 1f;

			var position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50f));
			position.y = 0.75f;

			Debug.DrawLine(currentBocceBall.transform.position, position);
			currentBocceBall.transform.LookAt(position);
			BocceBallAimTransform.LookAt(position);
		}
		else if(currentThrowMode == ThrowMode.FIRE)
		{
			forceIncrement += 0.125f * Mathf.Sign(forceIncrement);
			forceValue += forceIncrement;
			if(forceValue >= 100f)
			{
				forceIncrement = 1f;
				forceIncrement *= -1;
			}
			else if(forceValue <= 0)
			{
				forceIncrement = -1f;
				forceIncrement *= -1;
			}

			ForceMeter.value = forceValue;
		}
		else if(currentThrowMode == ThrowMode.THROW)
		{
			if(timerThrow >= 0)
			{
				timerThrow -= Time.deltaTime;
				currentBocceBall.BocceBallRigidBody.AddForce(BocceBallAimTransform.forward * currentForce);
			}
			else
			{
				currentThrowMode = ThrowMode.NOTHROW; 
				StartCoroutine(TemporaryThrow());
			}
		}
		else
		{

		}
	}

	#endregion
}
