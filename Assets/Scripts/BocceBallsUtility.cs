using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Bocce balls manager.
/// </summary>
public class BocceBallsUtility : MonoBehaviour
{
	/// <summary>
	/// The pallino.
	/// </summary>
	private static BocceBallInfo Pallino = null;

	/// <summary>
	/// List of bocce balls.
	/// </summary>
	private static List<BocceBallInfo> BocceBalls = new List<BocceBallInfo>();

	/// <summary>
	/// Clears the bocce balls.
	/// </summary>
	public static void ClearBocceBalls()
	{
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
	/// Adds the bocce balls.
	/// </summary>
	/// <param name="pallino">If true add ball to pallino, else to bocce balls</param>
	/// <param name="ball">BocceBallInfo</param>
	public static void AddBocceBalls(bool pallino, BocceBallInfo ball)
	{
		if(pallino)
		{
			Pallino = ball;
		}
		else
		{
			BocceBalls.Add(ball);
		}
	}

	/// <summary>
	/// Closests the ball from pallino.
	/// </summary>
	/// <returns>The ball from pallino.</returns>
	public static BocceBallInfo ClosestBallFromPallino()
	{
		SetDistancesFromPallino();
		return BocceBalls.FirstOrDefault();
	}

	/// <summary>
	/// Numbers the of team members closest to pallino.
	/// </summary>
	/// <returns>The of team members closest to pallino.</returns>
	public static int NumberOfTeamMembersClosestToPallino()
	{
		SetDistancesFromPallino();
		
		var playerAhead = BocceBalls.FirstOrDefault().BoccePlayer;
		var closestCount = 0;
		foreach(var ball in BocceBalls)
		{
			if(playerAhead != ball.BoccePlayer)
			{
				break;
			}
			closestCount++;
		}

		return closestCount;
	}

	/// <summary>
	/// Ares the bocce balls moving.
	/// </summary>
	/// <returns><c>true</c>, if bocce balls moving <c>false</c> otherwise.</returns>
	public static bool AreBocceBallsMoving()
	{
		var moving = false;
		
		if(Pallino != null || BocceBalls.Count > 0)
		{
			if(Pallino.BocceBallRigidBody.angularVelocity.sqrMagnitude >= 0.01f)
			{
				moving = true;
			}

			foreach(var ball in BocceBalls)
			{
				if(ball.BocceBallRigidBody.angularVelocity.sqrMagnitude >= 0.01f)
				{
					moving = true;
				}
			}
		}
		return moving;
	}

	/// <summary>
	/// Pallino's location.
	/// </summary>
	/// <returns>The location.</returns>
	public static Vector3 PallinoLocation()
	{
		var position = Vector3.zero;
		if(Pallino != null)
		{
			position = Pallino.transform.position;
		}
		return position;
	}

	/// <summary>
	/// Sets the distances from pallino.
	/// </summary>
	private static void SetDistancesFromPallino()
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
}
