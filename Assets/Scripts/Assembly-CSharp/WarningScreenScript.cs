using ControlFreak2;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarningScreenScript : MonoBehaviour
{
	public string LoadScene;

	private void Update()
	{
		if (CF2Input.anyKeyDown)
		{
			SceneManager.LoadScene(LoadScene);
		}
	}
}
