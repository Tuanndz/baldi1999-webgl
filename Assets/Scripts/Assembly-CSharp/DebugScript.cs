using System;
using System.Collections;
using ControlFreak2;
using TMPro;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
	public bool limitFramerate;

	public int framerate;

	public TextMeshProUGUI fpsText;

	public float deltaTime;

	[SerializeField]
	private TextMeshProUGUI keyText;

	[SerializeField]
	private TMP_Text errorText;

	[SerializeField]
	private AudioSource errorSource;

	private Coroutine logDissappearCoroutine;

	private void OnEnable()
	{
		Application.logMessageReceived += LogError;
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= LogError;
	}

	private void LogError(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception)
		{
			errorText.text = logString + "\n" + stackTrace + "\n";
			if (!errorSource.isPlaying)
			{
				errorSource.Play();
			}
			if (logDissappearCoroutine != null)
			{
				StopCoroutine(logDissappearCoroutine);
			}
			logDissappearCoroutine = StartCoroutine(LogDissappear());
		}
	}

	private IEnumerator LogDissappear()
	{
		yield return new WaitForSeconds(10f);
		float elapsed = 0f;
		Color color = errorText.color;
		color.a = 1f;
		errorText.color = color;
		while (elapsed < 1f)
		{
			elapsed += Time.deltaTime;
			color.a = Mathf.Lerp(1f, 0f, elapsed / 1f);
			errorText.color = color;
			yield return null;
		}
		color.a = 0f;
		errorText.color = color;
		logDissappearCoroutine = null;
	}

	private void Start()
	{
		if (limitFramerate)
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = framerate;
		}
	}

	public void Update()
	{
		if (Time.deltaTime != 0f)
		{
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			float f = 1f / deltaTime;
			fpsText.text = Mathf.Ceil(f) + " :FPS";
		}
		foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
		{
			if (CF2Input.GetKeyDown(value))
			{
				keyText.text = value.ToString() + " :Input";
				break;
			}
		}
	}
}
