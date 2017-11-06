using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Bocce balls manager.
/// </summary>
public class BocceBallsManager : MonoBehaviour
{
	/// <summary>
	/// The pallino.
	/// </summary>
	private BocceBallInfo Pallino;

	/// <summary>
	/// List of bocce balls.
	/// </summary>
	private List<BocceBallInfo> BocceBalls;

	/// <summary>
	/// Unity Awake method.
	/// </summary>
	private void Awake()
	{
		Pallino = null;
		BocceBalls = new List<BocceBallInfo>();
	}

	/// <summary>
	/// Clears the bocce balls.
	/// </summary>
	public void ClearBocceBalls()
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
	public void AddBocceBalls(bool pallino, BocceBallInfo ball)
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
	public BocceBallInfo ClosestBallFromPallino()
	{
		SetDistancesFromPallino();
		return BocceBalls.FirstOrDefault();
	}

	/// <summary>
	/// Numbers the of team members closest to pallino.
	/// </summary>
	/// <returns>The of team members closest to pallino.</returns>
	public int NumberOfTeamMembersClosestToPallino()
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
	/// Sets the distances from pallino.
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
}
