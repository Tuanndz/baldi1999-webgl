using System;
using UnityEngine;

[Serializable]
public struct InputBinding
{
	public KeyCode primaryKey;

	public KeyCode secondaryKey;

	public InputBinding(KeyCode primary, KeyCode secondary = KeyCode.None)
	{
		primaryKey = primary;
		secondaryKey = secondary;
	}
}
