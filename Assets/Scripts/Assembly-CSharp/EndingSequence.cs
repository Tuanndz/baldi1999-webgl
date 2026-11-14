using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingSequence : MonoBehaviour
{
	public TMP_Text textUI;

	public AudioSource audioSource;

	public AudioClip typeSound;

	public float startDelay = 4f;

	public float letterDelay = 0.05f;

	public float betweenTextsDelay = 3f;

	public string[] texts;

	public Sprite[] sprites;

	public Image targetImage;

	public Animator finalAnimator;

	public string finalTrigger = "Play";

	public AudioSource finalAudioSource;

	public AudioClip finalSound;

	public RawImage fadeImage;

	private void Start()
	{
		StartCoroutine(SequenceRoutine());
	}

	private IEnumerator SequenceRoutine()
	{
		yield return new WaitForSeconds(startDelay);
		for (int i = 0; i < texts.Length; i++)
		{
			if (sprites[i] != null)
			{
				targetImage.sprite = sprites[i];
			}
			yield return StartCoroutine(TypeText(texts[i]));
			yield return new WaitForSeconds(betweenTextsDelay);
		}
		if (finalAnimator != null)
		{
			finalAnimator.enabled = true;
			finalAnimator.SetTrigger(finalTrigger);
		}
		if (finalAudioSource != null && finalSound != null)
		{
			finalAudioSource.PlayOneShot(finalSound);
		}
		yield return new WaitForSeconds(1.5f);
		yield return StartCoroutine(FadeOutImage());
		Application.Quit();
	}

	private IEnumerator TypeText(string t)
	{
		textUI.text = "";
		for (int i = 0; i < t.Length; i++)
		{
			textUI.text += t[i];
			if (audioSource != null && typeSound != null)
			{
				audioSource.PlayOneShot(typeSound);
			}
			yield return new WaitForSeconds(letterDelay);
		}
	}

	private IEnumerator FadeOutImage()
	{
		float duration = 2f;
		float timer = 0f;
		Color c = fadeImage.color;
		while (timer < duration)
		{
			timer += Time.deltaTime;
			float t = timer / duration;
			c.a = Mathf.Lerp(0f, 1f, t);
			fadeImage.color = c;
			yield return null;
		}
	}
}
