using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonYes : MonoBehaviour
{
	public float delay = 2f;

	public string sceneName = "NextScene";

	public void OnButtonPress()
	{
		StartCoroutine(LoadSceneDelayed());
	}

	private IEnumerator LoadSceneDelayed()
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(sceneName);
	}
}
