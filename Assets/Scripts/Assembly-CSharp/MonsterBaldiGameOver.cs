using UnityEngine;

public class MonsterBaldiGameOver : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		PlayerScript component = collision.transform.GetComponent<PlayerScript>();
		if (component != null && !component.gc.debugMode)
		{
			component.gameOver = true;
			RenderSettings.skybox = component.blackSky;
			component.StartCoroutine(component.KeepTheHudOff());
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerScript component = other.GetComponent<PlayerScript>();
		if (component != null && !component.gc.debugMode)
		{
			component.gameOver = true;
			RenderSettings.skybox = component.blackSky;
			component.StartCoroutine(component.KeepTheHudOff());
		}
	}
}
