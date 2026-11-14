using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
	public float rotationSpeed = 1f;

	public bool rotate;

	private void Update()
	{
		if (rotate)
		{
			RenderSettings.skybox.SetFloat("_Rotation", RenderSettings.skybox.GetFloat("_Rotation") + rotationSpeed * Time.deltaTime);
		}
	}
}
