using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonNo : MonoBehaviour
{
	public AudioSource mainAudio;

	public AudioSource screamAudio;

	public Animator animator;

	public TextMeshProUGUI warningText;

	public string sceneName = "NextScene";

	public void OnButtonPress()
	{
		StartCoroutine(ProcessNo());
	}

	private IEnumerator ProcessNo()
	{
		if (mainAudio != null)
		{
			mainAudio.Play();
		}
		if (warningText != null)
		{
			warningText.text = "It's Too Late To Give Up";
		}
		yield return new WaitForSeconds(4f);
		animator.enabled = true;
		animator.Play(0);
		yield return new WaitForSeconds(0.2f);
		screamAudio.Play();
		yield return new WaitForSeconds(0.3f);
		SceneManager.LoadScene(sceneName);
	}
}
