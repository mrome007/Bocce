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

	/// <summary>
	/// Gets the bocce ball rigid body.
	/// </summary>
	/// <value>The bocce ball rigid body.</value>
	public Rigidbody BocceBallRigidBody{ get; private set; }

	/// <summary>
	/// Unity Awake method.
	/// </summary>
	private void Awake()
	{
		BocceBallRigidBody = GetComponent<Rigidbody>();
	}
}
