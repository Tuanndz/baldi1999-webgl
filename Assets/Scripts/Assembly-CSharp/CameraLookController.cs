using UnityEngine;

public class CameraLookController : MonoBehaviour
{
	public Camera cam;

	public Transform target;

	public float speed = 900f;

	public bool active;

	private void LateUpdate()
	{
		if (active)
		{
			Quaternion to = Quaternion.LookRotation(target.position + Vector3.up * 0.35f - cam.transform.position);
			cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, to, speed * Time.deltaTime);
		}
	}

	public void StartLook()
	{
		active = true;
	}

	public void StopLook()
	{
		active = false;
	}
}
