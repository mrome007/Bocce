using System.Collections;
using System;
using UnityEngine;

public class BocceEventArgs : EventArgs
{
	public BocceBallInfo BocceBallInfo { get; private set; }

	public BocceEventArgs(BocceBallInfo ballInfo)
	{
		BocceBallInfo = ballInfo;
	}
}
