using UnityEngine;
using System.Collections;

/// <summary>
/// Relevant Information regarding the Bocce Ball.
/// </summary>
public class BocceBallInfo : MonoBehaviour 
{
	/// <summary>
	/// Player throwing the Bocce Ball.
	/// </summary>
	public BocceTurnsManager.Player BoccePlayer { get; set; }

	/// <summary>
	/// Gets or sets the distance from pallino.
	/// </summary>
	public float DistanceFromPallino { get; set; }
}
