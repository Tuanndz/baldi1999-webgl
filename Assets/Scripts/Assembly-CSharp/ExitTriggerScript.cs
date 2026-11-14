using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitTriggerScript : MonoBehaviour
{
	public GameControllerScript gc;

	public string ResultsScene;

	public string SecretScene;

	public PlayerScript player;

	public CameraScript cam;

	public AudioSource audioSrc;

	public AudioClip chaseMusic;

	public Image glitchImage;

	private bool triggered;

	private void OnTriggerEnter(Collider other)
	{
		if (!triggered && gc.notebooks >= 14 && other.tag == "Player")
		{
			triggered = true;
			StartCoroutine(ExitSequence());
		}
	}

	private IEnumerator ExitSequence()
	{
		player.enabled = false;
		cam.enabled = false;
		for (float t = 0f; t < 3f; t += 0.1f)
		{
			audioSrc.pitch = Random.Range(0.6f, 1.4f);
			audioSrc.volume = Random.Range(0.5f, 1f);
			audioSrc.PlayOneShot(chaseMusic);
			yield return new WaitForSeconds(0.1f);
		}
		float g = 0f;
		float fade = 0f;
		if (glitchImage != null)
		{
			glitchImage.color = new Color(0f, 0f, 0f, 0f);
		}
		while (g < 5f)
		{
			cam.transform.localPosition += new Vector3(Random.Range(-0.03f, 0.03f), Random.Range(-0.03f, 0.03f), Random.Range(-0.03f, 0.03f));
			cam.transform.localRotation = Quaternion.Euler(cam.transform.localRotation.eulerAngles.x + Random.Range(-1f, 1f), cam.transform.localRotation.eulerAngles.y + Random.Range(-1f, 1f), cam.transform.localRotation.eulerAngles.z + Random.Range(-2f, 2f));
			fade += Time.deltaTime * 0.25f;
			if (glitchImage != null)
			{
				glitchImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(fade));
			}
			audioSrc.volume = Mathf.Lerp(1f, 0f, g / 5f);
			g += Time.deltaTime;
			yield return null;
		}
		if (gc.failedNotebooks >= 7)
		{
			SceneManager.LoadScene(SecretScene);
		}
		else
		{
			SceneManager.LoadScene(ResultsScene);
		}
	}
}
