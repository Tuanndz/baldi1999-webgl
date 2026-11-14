using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
	public Camera cam;

	public float intensity = 0.03f;

	private bool active;

	private Vector3 basePos;

	private void Start()
	{
		basePos = cam.transform.localPosition;
	}

	private void LateUpdate()
	{
		if (!active)
		{
			cam.transform.localPosition = basePos;
		}
		else
		{
			cam.transform.localPosition = basePos + new Vector3(Random.Range(0f - intensity, intensity), Random.Range(0f - intensity, intensity), 0f);
		}
	}

	public void StartShake()
	{
		basePos = cam.transform.localPosition;
		active = true;
	}

	public void StopShake()
	{
		active = false;
		cam.transform.localPosition = basePos;
	}
}
