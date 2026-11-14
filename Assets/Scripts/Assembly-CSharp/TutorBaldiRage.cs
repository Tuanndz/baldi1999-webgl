using TMPro;
using UnityEngine;

public class TutorBaldiRage : MonoBehaviour
{
	public enum LookMode
	{
		LookAtTarget = 0,
		LookAtTutor = 1,
		LookAtPlayer = 2
	}

	public Transform player;

	public Camera playerCamera;

	public MonoBehaviour movementScript;

	public float rageDistance = 5f;

	public float lookDelay = 1f;

	public Transform lookTarget;

	public LookMode lookMode;

	public Animator tutorAnimator;

	public string animationTrigger = "Rage";

	public AudioClip loopSound;

	public int loopCount = 3;

	public float loopInterval = 2f;

	public GameObject uiScreen1;

	public AudioClip screen1Audio;

	public GameObject uiScreen2;

	public AudioClip screen2Audio;

	public GameObject uiScreen3;

	public GameObject tutorObjectToDisable;

	public GameObject extraObject;

	public GameObject[] extraObjectsToEnable;

	public AudioClip chaseMusic;

	public float walkBoost = 2f;

	public float runBoost = 3f;

	private AudioSource audioSource;

	private bool triggered;

	private bool animationStarted;

	private bool animationFinished;

	private bool screen1Done;

	private bool screen2Done;

	private bool cleanupStarted;

	private float timer;

	private int playedLoops;

	private bool flashing;

	private float flashTimer = 0.75f;

	private int flashCount;

	private CanvasGroup screen2Alpha;

	public CameraLookController lookCtrl;

	public CameraShakeController shakeCtrl;

	public TMP_Text notebookTMP;

	private bool findMeActive;

	private bool runForced;

	public CeilingFallController ceilingController;

	public SkyboxRotator skyboxRotator;

	public bool disableTMPForce;

	private bool lockCameraY;

	private float fixedYRotation;

	private void Start()
	{
		audioSource = base.gameObject.AddComponent<AudioSource>();
		tutorAnimator.enabled = false;
		if (uiScreen1 != null)
		{
			uiScreen1.SetActive(false);
		}
		if (uiScreen2 != null)
		{
			screen2Alpha = uiScreen2.GetComponent<CanvasGroup>();
			if (screen2Alpha == null)
			{
				screen2Alpha = uiScreen2.AddComponent<CanvasGroup>();
			}
			screen2Alpha.alpha = 0f;
			uiScreen2.SetActive(false);
		}
		if (uiScreen3 != null)
		{
			uiScreen3.SetActive(false);
		}
		if (extraObject != null)
		{
			extraObject.SetActive(false);
		}
	}

	private void Update()
	{
		GameControllerScript gameControllerScript = Object.FindObjectOfType<GameControllerScript>();
		if (gameControllerScript != null && notebookTMP != null && !disableTMPForce)
		{
			if (runForced)
			{
				if (gameControllerScript.notebooks == 2)
				{
					notebookTMP.text = "RUN";
				}
				else
				{
					notebookTMP.text = gameControllerScript.notebooks + "/7";
					runForced = false;
				}
			}
			else
			{
				if (gameControllerScript.notebooks == 2 && !findMeActive)
				{
					notebookTMP.text = "RUN";
					findMeActive = true;
				}
				else if (findMeActive && gameControllerScript.notebooks > 2)
				{
					notebookTMP.text = "2/7";
					findMeActive = false;
				}
				if (!findMeActive && gameControllerScript.notebooks > 2)
				{
					notebookTMP.text = gameControllerScript.notebooks + "/7";
				}
			}
		}
		if (!triggered && Vector3.Distance(base.transform.position, player.position) <= rageDistance)
		{
			triggered = true;
			timer = lookDelay;
		}
		if (triggered && !animationStarted)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				movementScript.enabled = false;
				lookCtrl.StartLook();
				tutorAnimator.enabled = true;
				tutorAnimator.SetTrigger(animationTrigger);
				animationStarted = true;
				timer = loopInterval;
				PlayerScript component = player.GetComponent<PlayerScript>();
				component.staminaBar.value = component.stamina / component.maxStamina * 100f;
				fixedYRotation = playerCamera.transform.localEulerAngles.x;
				lockCameraY = true;
			}
		}
		if (animationStarted && !animationFinished)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				if (playedLoops < loopCount)
				{
					audioSource.PlayOneShot(loopSound);
					playedLoops++;
					timer = loopInterval;
				}
				else
				{
					animationFinished = true;
					timer = 2f;
				}
			}
		}
		if (animationFinished && !screen1Done)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				uiScreen1.SetActive(true);
				audioSource.PlayOneShot(screen1Audio);
				screen1Done = true;
				timer = 2f;
			}
		}
		else if (screen1Done && !screen2Done)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				uiScreen1.SetActive(false);
				if (tutorObjectToDisable != null)
				{
					tutorObjectToDisable.SetActive(false);
				}
				if (extraObject != null)
				{
					extraObject.SetActive(true);
				}
				uiScreen2.SetActive(true);
				screen2Alpha.alpha = 0f;
				audioSource.PlayOneShot(screen2Audio);
				lookCtrl.StopLook();
				shakeCtrl.StartShake();
				screen2Done = true;
				flashing = true;
				runForced = true;
				if (uiScreen3 != null)
				{
					uiScreen3.SetActive(true);
				}
				if (skyboxRotator != null)
				{
					skyboxRotator.rotate = true;
				}
			}
		}
		else if (flashing && !cleanupStarted)
		{
			flashTimer -= Time.deltaTime;
			if (flashTimer <= 0f)
			{
				screen2Alpha.alpha = ((screen2Alpha.alpha == 0f) ? 1f : 0f);
				flashTimer = 0.75f;
				flashCount++;
				if (flashCount >= 10)
				{
					flashing = false;
					StopFlashingAndStartChase();
				}
			}
		}
		if (lockCameraY)
		{
			Vector3 localEulerAngles = playerCamera.transform.localEulerAngles;
			localEulerAngles.x = fixedYRotation;
			playerCamera.transform.localEulerAngles = localEulerAngles;
		}
	}

	public void StopFlashingAndStartChase()
	{
		screen2Alpha.alpha = 0f;
		uiScreen2.SetActive(false);
		audioSource.loop = true;
		audioSource.clip = chaseMusic;
		audioSource.Play();
		Object.FindObjectOfType<ZoomToAudio>().targetAudio = audioSource;
		PlayerScript component = player.GetComponent<PlayerScript>();
		component.walkSpeed *= walkBoost;
		component.runSpeed *= runBoost;
		cleanupStarted = true;
		lockCameraY = false;
		lookCtrl.StopLook();
		movementScript.enabled = true;
		if (extraObjectsToEnable != null)
		{
			for (int i = 0; i < extraObjectsToEnable.Length; i++)
			{
				if (extraObjectsToEnable[i] != null)
				{
					extraObjectsToEnable[i].SetActive(true);
				}
			}
		}
		runForced = true;
		if (ceilingController != null)
		{
			ceilingController.StartCeilings();
		}
	}
}
