using ControlFreak2;
using UnityEngine;

public class CursorControllerScript : MonoBehaviour
{
	public void LockCursor()
	{
		CFCursor.lockState = CursorLockMode.Locked;
		CFCursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void UnlockCursor()
	{
		CFCursor.lockState = CursorLockMode.None;
		CFCursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
		{
			LockCursor();
		}
	}
}
