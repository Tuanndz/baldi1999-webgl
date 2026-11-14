using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonFade : MonoBehaviour
{
	public AudioSource music;

	public AudioSource clickSound;

	public Image fadeImage;

	public string nextScene;

	public float delayBeforeFade = 1f;

	public float fadeDuration = 2f;

	private bool clicked;

	public void Trigger()
	{
		if (!clicked)
		{
			clicked = true;
			if (music != null)
			{
				music.Stop();
			}
			if (clickSound != null)
			{
				clickSound.Play();
			}
			StartCoroutine(FadeAndLoad());
		}
	}

	private IEnumerator FadeAndLoad()
	{
		yield return new WaitForSeconds(delayBeforeFade);
		float t = 0f;
		Color c = fadeImage.color;
		while (t < fadeDuration)
		{
			t += Time.deltaTime;
			c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
			fadeImage.color = c;
			yield return null;
		}
		SceneManager.LoadScene(nextScene);
	}
}
