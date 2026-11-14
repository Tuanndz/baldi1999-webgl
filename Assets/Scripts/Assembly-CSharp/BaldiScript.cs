using UnityEngine;
using UnityEngine.AI;

public class BaldiScript : MonoBehaviour
{
	public float baseTime;

	public float speed;

	public float baldiAnger;

	public float baldiTempAnger;

	public float baldiWait;

	public float baldiSpeedScale;

	public float currentPriority;

	public bool antiHearing;

	public float antiHearingTime;

	public float vibrationDistance;

	public float angerRate;

	public float angerRateRate;

	public float angerFrequency;

	public float timeToAnger;

	public bool endless;

	public Transform player;

	public Transform wanderTarget;

	public AILocationSelectorScript wanderer;

	public AudioClip slap;

	public Animator baldiAnimator;

	private AudioSource baldiAudio;

	public NavMeshAgent agent;

	private float timeToMove;

	public float coolDown;

	private Vector3 previous;

	private bool rumble;

	public float turnSpeed = 120f;

	public float normSpeed = 12f;

	public float runSpeed = 20f;

	public float currentSpeed;

	private float angleDiff;

	public bool db;

	private float startDelay = 7f;

	private void Start()
	{
		baldiAudio = GetComponent<AudioSource>();
		agent = GetComponent<NavMeshAgent>();
		timeToMove = baseTime;
		Wander();
		currentSpeed = normSpeed;
		if (PlayerPrefs.GetInt("Rumble") == 1)
		{
			rumble = true;
		}
	}

	private void Update()
	{
		if (startDelay > 0f)
		{
			startDelay -= Time.deltaTime;
			agent.speed = 0f;
			return;
		}
		if (timeToMove > 0f)
		{
			timeToMove -= Time.deltaTime;
		}
		else
		{
			MoveLikeFirstPrize();
		}
		if (coolDown > 0f)
		{
			coolDown -= Time.deltaTime;
		}
		if (baldiTempAnger > 0f)
		{
			baldiTempAnger -= 0.02f * Time.deltaTime;
		}
		else
		{
			baldiTempAnger = 0f;
		}
		if (antiHearingTime > 0f)
		{
			antiHearingTime -= Time.deltaTime;
		}
		else
		{
			antiHearing = false;
		}
		if (endless)
		{
			if (timeToAnger > 0f)
			{
				timeToAnger -= Time.deltaTime;
			}
			else
			{
				timeToAnger = angerFrequency;
				GetAngry(angerRate);
				angerRate += angerRateRate;
			}
		}
		angleDiff = Mathf.DeltaAngle(base.transform.eulerAngles.y, Mathf.Atan2(agent.steeringTarget.x - base.transform.position.x, agent.steeringTarget.z - base.transform.position.z) * 57.29578f);
		if (Mathf.Abs(angleDiff) < 5f)
		{
			base.transform.LookAt(new Vector3(agent.steeringTarget.x, base.transform.position.y, agent.steeringTarget.z));
			agent.speed = currentSpeed;
		}
		else
		{
			base.transform.Rotate(new Vector3(0f, turnSpeed * Mathf.Sign(angleDiff) * Time.deltaTime, 0f));
			agent.speed = 0f;
		}
	}

	private void FixedUpdate()
	{
		Vector3 direction = player.position - base.transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out hitInfo, float.PositiveInfinity, 769, QueryTriggerInteraction.Ignore) && hitInfo.transform.CompareTag("Player"))
		{
			db = true;
			TargetPlayer();
			currentSpeed = runSpeed;
			return;
		}
		db = false;
		currentSpeed = normSpeed;
		if (coolDown <= 0f && agent.velocity.magnitude <= 1f && (base.transform.position - agent.destination).magnitude < 5f)
		{
			Wander();
		}
	}

	private void Wander()
	{
		wanderer.GetNewTarget();
		agent.SetDestination(wanderTarget.position);
		coolDown = 1f;
		currentPriority = 0f;
	}

	public void TargetPlayer()
	{
		agent.SetDestination(player.position);
		coolDown = 1f;
		currentPriority = 0f;
	}

	private void MoveLikeFirstPrize()
	{
		timeToMove = 0f;
		previous = base.transform.position;
		baldiAnimator.speed = 1f;
		currentSpeed = Mathf.Lerp(normSpeed, runSpeed, baldiAnger / 10f);
	}

	public void GetAngry(float value)
	{
		baldiAnger += value;
		if (baldiAnger < 0.5f)
		{
			baldiAnger = 0.5f;
		}
	}

	public void GetTempAngry(float value)
	{
		baldiTempAnger += value;
	}

	public void Hear(Vector3 soundLocation, float priority)
	{
		if (!antiHearing && priority >= currentPriority)
		{
			agent.SetDestination(soundLocation);
			currentPriority = priority;
		}
	}

	public void ActivateAntiHearing(float t)
	{
		Wander();
		antiHearing = true;
		antiHearingTime = t;
	}
}
