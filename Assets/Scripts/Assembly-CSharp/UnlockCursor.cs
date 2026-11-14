using ControlFreak2;
using UnityEngine;

public class UnlockCursor : MonoBehaviour
{
	private void Start()
	{
		CFCursor.lockState = CursorLockMode.None;
		CFCursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
