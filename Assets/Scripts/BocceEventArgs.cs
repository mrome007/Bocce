using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// Class that provides BocceBallInfo component as an event argument.
/// </summary>
public class BocceEventArgs : EventArgs
{
	/// <summary>
	/// Gets the bocce ball info.
	/// </summary>
	/// <value>The bocce ball info.</value>
	public BocceBallInfo BocceBallInfo { get; private set; }

	/// <summary>
	/// Initializes a new instance of BocceEventArgs class.
	/// </summary>
	/// <param name="ballInfo">Ball info.</param>
	public BocceEventArgs(BocceBallInfo ballInfo)
	{
		BocceBallInfo = ballInfo;
	}
}
