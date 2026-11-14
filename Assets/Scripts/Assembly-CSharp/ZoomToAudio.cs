using UnityEngine;

public class ZoomToAudio : MonoBehaviour
{
	public AudioSource targetAudio;

	public float minFOV = 20f;

	public float maxFOV = 100f;

	public float zoomSpeed = 10f;

	private Camera cam;

	private void Start()
	{
		cam = GetComponent<Camera>();
	}

	private void Update()
	{
		if (targetAudio != null && targetAudio.isPlaying)
		{
			float audioLevel = GetAudioLevel();
			float b = Mathf.Lerp(minFOV, maxFOV, audioLevel);
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, b, zoomSpeed * Time.deltaTime);
		}
	}

	private float GetAudioLevel()
	{
		float[] array = new float[256];
		targetAudio.GetSpectrumData(array, 0, FFTWindow.Rectangular);
		float num = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			num += array[i];
		}
		return Mathf.Clamp01(Mathf.Sqrt(num / (float)array.Length) * 10f);
	}
}
