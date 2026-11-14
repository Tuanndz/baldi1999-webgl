using TMPro;
using UnityEngine;

public class GlobalFireEffect : MonoBehaviour
{
	[Header("Notebook Text")]
	public TMP_Text notebookTMP;

	public Color glitchColor = Color.red;

	public float glitchIntensity = 0.1f;

	public float flickerSpeed = 10f;

	[Header("Black Screen")]
	public GameObject blackScreen;

	public float blackScreenDuration = 0.4f;

	[Header("School Fire Tint")]
	public float fireRedBoost = 0.45f;

	public float fireDarken = 0.35f;

	[Header("Object Activation")]
	public GameObject parentObject;

	[Header("Fire Audio")]
	public AudioClip fireLoopClip;

	private AudioSource fireAudio;

	private GameControllerScript gc;

	private bool triggered;

	private float timer;

	private float originalFontSize;

	private Color originalColor;

	private void Start()
	{
		gc = Object.FindObjectOfType<GameControllerScript>();
		if (blackScreen != null)
		{
			blackScreen.SetActive(false);
		}
		if (notebookTMP != null)
		{
			originalFontSize = notebookTMP.fontSize;
			originalColor = notebookTMP.color;
		}
		if (parentObject != null)
		{
			parentObject.SetActive(false);
		}
		fireAudio = base.gameObject.AddComponent<AudioSource>();
		fireAudio.loop = true;
		fireAudio.playOnAwake = false;
	}

	private void Update()
	{
		if (gc == null)
		{
			return;
		}
		if (gc.notebooks == 2 && !triggered)
		{
			triggered = true;
			timer = blackScreenDuration;
			if (blackScreen != null)
			{
				blackScreen.SetActive(true);
			}
			ApplyFireTint();
			ActivateParentAndChildren();
			PlayFireAudio();
		}
		if (triggered)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f && blackScreen != null)
			{
				blackScreen.SetActive(false);
			}
			if (notebookTMP != null)
			{
				notebookTMP.text = "FIND ME";
				float num = Mathf.Sin(Time.time * flickerSpeed) * glitchIntensity;
				notebookTMP.fontSize = originalFontSize + num * 10f;
				Color color = glitchColor;
				color.a = 0.7f + num;
				notebookTMP.color = color;
			}
		}
	}

	private void ApplyFireTint()
	{
		Color ambientLight = RenderSettings.ambientLight;
		float r = Mathf.Clamp(ambientLight.r + fireRedBoost, 0f, 1f);
		float g = Mathf.Clamp(ambientLight.g * (1f - fireDarken), 0f, 1f);
		float b = Mathf.Clamp(ambientLight.b * (1f - fireDarken), 0f, 1f);
		RenderSettings.ambientLight = new Color(r, g, b);
	}

	private void ActivateParentAndChildren()
	{
		if (!(parentObject == null))
		{
			parentObject.SetActive(true);
			Transform transform = parentObject.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(true);
			}
		}
	}

	private void PlayFireAudio()
	{
		if (!(fireLoopClip == null))
		{
			fireAudio.clip = fireLoopClip;
			fireAudio.volume = 1f;
			fireAudio.Play();
		}
	}
}
