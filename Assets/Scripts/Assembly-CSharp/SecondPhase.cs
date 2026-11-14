using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecondPhase : MonoBehaviour
{
	public static bool secondPhaseActive;

	public GameObject firstBaldi;

	public GameObject secondBaldiPrefab;

	private GameObject secondBaldiInstance;

	public TMP_Text notebookTMP;

	public GameObject[] allNotebooks;

	public float notebookRespawnHeight = 1f;

	public PlayerScript player;

	public float walkBoost = 1.2f;

	public float runBoost = 1.2f;

	public string firstAnimation = "Intro";

	public string secondAnimation = "Chase";

	public Image whiteFlash;

	public Material secondPhaseSkybox;

	private bool triggered;

	private bool forceText;

	private void Update()
	{
		GameControllerScript gameControllerScript = Object.FindObjectOfType<GameControllerScript>();
		if (gameControllerScript == null)
		{
			return;
		}
		if (!triggered && gameControllerScript.notebooks >= 7)
		{
			triggered = true;
			forceText = true;
			StartSecondPhase();
		}
		if (forceText && notebookTMP != null)
		{
			int notebooks = gameControllerScript.notebooks;
			if (notebooks == 7)
			{
				StartCoroutine(WhiteFlashEffect());
			}
			if (notebooks >= 7 && notebooks <= 14)
			{
				notebookTMP.text = notebooks + "/14";
				notebookTMP.ForceMeshUpdate(true);
			}
		}
	}

	private void StartSecondPhase()
	{
		secondPhaseActive = true;
		forceText = true;
		ZoomToAudio zoomToAudio = Object.FindObjectOfType<ZoomToAudio>();
		if (zoomToAudio != null)
		{
			zoomToAudio.minFOV = 75f;
			zoomToAudio.maxFOV = 90f;
		}
		if (secondPhaseSkybox != null)
		{
			RenderSettings.skybox = secondPhaseSkybox;
		}
		TutorBaldiRage tutorBaldiRage = Object.FindObjectOfType<TutorBaldiRage>();
		if (tutorBaldiRage != null)
		{
			tutorBaldiRage.disableTMPForce = true;
		}
		if (notebookTMP != null)
		{
			notebookTMP.text = "7/14";
			notebookTMP.ForceMeshUpdate(true);
		}
		if (firstBaldi != null && secondBaldiPrefab != null)
		{
			Vector3 position = firstBaldi.transform.position;
			Quaternion rotation = firstBaldi.transform.rotation;
			secondBaldiInstance = Object.Instantiate(secondBaldiPrefab, position, rotation);
			secondBaldiInstance.SetActive(true);
			firstBaldi.SetActive(false);
		}
		if (secondBaldiInstance != null)
		{
			Animator component = secondBaldiInstance.GetComponent<Animator>();
			if (component != null)
			{
				StartCoroutine(PlayAnimations(component));
			}
		}
		RespawnNotebooks();
		if (player != null)
		{
			player.walkSpeed *= walkBoost;
			player.runSpeed *= runBoost;
		}
		SkyboxRotator skyboxRotator = Object.FindObjectOfType<SkyboxRotator>();
		if (skyboxRotator != null)
		{
			skyboxRotator.rotationSpeed *= 2.5f;
			skyboxRotator.rotate = true;
		}
		CameraShakeController cameraShakeController = Object.FindObjectOfType<CameraShakeController>();
		if (cameraShakeController != null)
		{
			cameraShakeController.intensity = 0.065f;
			cameraShakeController.StartShake();
		}
	}

	private IEnumerator PlayAnimations(Animator anim)
	{
		anim.Play(firstAnimation);
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
		anim.Play(secondAnimation);
	}

	private void RespawnNotebooks()
	{
		GameObject[] array = allNotebooks;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				Vector3 position = gameObject.transform.position;
				position.y = notebookRespawnHeight;
				gameObject.transform.position = position;
				gameObject.SetActive(true);
			}
		}
	}

	private IEnumerator WhiteFlashEffect()
	{
		float t = 0f;
		while (t < 0.5f)
		{
			t += Time.deltaTime;
			float a = Mathf.Lerp(0f, 1f, t / 0.75f);
			whiteFlash.color = new Color(1f, 1f, 1f, a);
			yield return null;
		}
		t = 0f;
		while (t < 0.5f)
		{
			t += Time.deltaTime;
			float a2 = Mathf.Lerp(1f, 0f, t / 0.75f);
			whiteFlash.color = new Color(1f, 1f, 1f, a2);
			yield return null;
		}
		whiteFlash.color = new Color(1f, 1f, 1f, 0f);
	}
}
