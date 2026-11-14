using ControlFreak2;
using UnityEngine;

public class CursorControllerScript : MonoBehaviour
{
	public void LockCursor()
	{
		CFCursor.lockState = CursorLockMode.Locked;
		CFCursor.visible = false;
	}

	public void UnlockCursor()
	{
		CFCursor.lockState = CursorLockMode.None;
		CFCursor.visible = true;
	}
}
