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

	#region ThrowMode enum
	
	public enum ThrowMode
	{
		NOTHROW,
		AIM,
		FIRE,
		THROW
	}

	#endregion

	#region Private fields

	/// <summary>
	/// The current bocce ball being thrown.
	/// </summary>
	private BocceBallInfo currentBocceBall = null;

	/// <summary>
	/// The current throw mode.
	/// </summary>
	private ThrowMode currentThrowMode = ThrowMode.NOTHROW;

	#endregion

	#region Force variables

	/// <summary>
	/// The timer for throwing ball.
	/// </summary>
	private float timerThrow = 0.65f;

	/// <summary>
	/// Maximum force put on pallino.
	/// </summary>
	private float pallinoForce = 100f;

	/// <summary>
	/// Maximum force put on Bocce ball.
	/// </summary>
	private float bocceBallForce = 500f;

	/// <summary>
	/// Current force used.
	/// </summary>
	private float currentForce;

	/// <summary>
	/// The value used by Slider.
	/// </summary>
	private float forceSliderValue = 1f;

	/// <summary>
	/// The increment value for Slider to animate up/down.
	/// </summary>
	private float forceIncrement = 1f;

	#endregion

	#region UI element for aiming

	/// <summary>
	/// Slider to determine force used.
	/// </summary>
	[SerializeField]
	private Slider forceMeter;

	/// <summary>
	/// Line for aiming bocce balls.
	/// </summary>
	[SerializeField]
	private LineRenderer aimLine;

	/// <summary>
	/// The fire mode text.
	/// </summary>
	[SerializeField]
	private Text FireModeText;

	/// <summary>
	/// The throw mode text.
	/// </summary>
	[SerializeField]
	private Text ThrowModeText;

	#endregion

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
			currentBocceBall = (BocceBallInfo)Instantiate(pallino, new Vector3(0, 0.75f,-14.5f), Quaternion.identity);
			currentForce = pallinoForce;
		}
		currentThrowMode = ThrowMode.AIM;
	}

	/// <summary>
	/// Ends the throw and triggers the Throw Complete event.
	/// </summary>
	private void EndThrow()
	{
		aimLine.enabled = false;
		forceMeter.gameObject.SetActive(false);

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

	/// <summary>
	/// Unity Update method.
	/// </summary>
	private void Update()
	{
		ThrowBallInput();
	}

	/// <summary>
	/// Unity FixedUpdate method.
	/// </summary>
	private void FixedUpdate()
	{
		if(currentThrowMode == ThrowMode.AIM)
		{
			Aim();
		}
		else if(currentThrowMode == ThrowMode.FIRE)
		{
			Fire();
		}
		else if(currentThrowMode == ThrowMode.THROW)
		{
			Throw();
		}
	}

	#endregion

	#region Helpers

	/// <summary>
	/// Input detection for throwing bocce balls.
	/// </summary>
	private void ThrowBallInput()
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
				currentForce *= (forceSliderValue / 100f);
			}
		}
	}

	/// <summary>
	/// Method for aiming bocce ball.
	/// </summary>
	private void Aim()
	{
		DisableThrowTexts();
		FireModeText.gameObject.SetActive(true);

		forceMeter.value = 1f;
		forceSliderValue = 1f;
		forceIncrement = 1f;
		
		var position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50f));
		position.y = currentBocceBall.transform.position.y;
		
		currentBocceBall.transform.LookAt(position);
		BocceBallAimTransform.LookAt(position);
		
		aimLine.enabled = true;
		aimLine.SetPosition(0, new Vector3(position.x, 1f, position.z));
	}

	/// <summary>
	/// Method for firing the bocce ball.
	/// </summary>
	private void Fire()
	{
		DisableThrowTexts();
		ThrowModeText.gameObject.SetActive(true);

		forceMeter.gameObject.SetActive(true);

		forceIncrement += 0.125f * Mathf.Sign(forceIncrement);
		forceSliderValue += forceIncrement;
		if(forceSliderValue >= 100f)
		{
			forceIncrement = 1f;
			forceIncrement *= -1;
		}
		else if(forceSliderValue <= 0)
		{
			forceIncrement = -1f;
			forceIncrement *= -1;
		}
		
		forceMeter.value = forceSliderValue;
	}

	/// <summary>
	/// Method when bocce ball is thrown.
	/// </summary>
	private void Throw()
	{
		DisableThrowTexts();

		if(timerThrow >= 0)
		{
			timerThrow -= Time.deltaTime;
			currentBocceBall.BocceBallRigidBody.AddForce(BocceBallAimTransform.forward * currentForce);
		}
		else
		{
			aimLine.enabled = false;
			currentThrowMode = ThrowMode.NOTHROW; 
			StartCoroutine(TemporaryThrow());
		}
	}

	/// <summary>
	/// Disables the throw texts.
	/// </summary>
	private void DisableThrowTexts()
	{
		FireModeText.gameObject.SetActive(false);
		ThrowModeText.gameObject.SetActive(false);
	}

	#endregion
}
