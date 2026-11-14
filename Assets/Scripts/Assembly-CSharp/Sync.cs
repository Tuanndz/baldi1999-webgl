using UnityEngine;
using UnityEngine.UI;

public class Sync : MonoBehaviour
{
	public Image fadeImage;

	public AudioSource music;

	public float duration = 2f;

	private float t;

	private bool fading;

	public void StartFade()
	{
		if (!(fadeImage == null) && !(music == null))
		{
			t = 0f;
			fading = true;
			fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
			music.volume = 0f;
			music.Play();
		}
	}

	private void Update()
	{
		if (fading)
		{
			t += Time.deltaTime;
			float num = t / duration;
			float a = Mathf.Lerp(1f, 0f, num);
			fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
			music.volume = Mathf.Lerp(0f, 1f, num);
			if (t >= duration)
			{
				fading = false;
			}
		}
	}
}
