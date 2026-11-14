using UnityEngine;

public class CeilingFallController : MonoBehaviour
{
	public float minDelay = 5f;

	public float maxDelay = 20f;

	public float fallSpeed = 1f;

	public float spinSpeed = 20f;

	public AudioClip fallSound;

	public float soundVolume = 1f;

	private GameObject[] ceilings;

	private float[] timers;

	private bool[] soundPlayed;

	private bool started;

	private AudioSource audioSource;

	private void Start()
	{
		ceilings = GameObject.FindGameObjectsWithTag("Ceiling");
		timers = new float[ceilings.Length];
		soundPlayed = new bool[ceilings.Length];
		audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		for (int i = 0; i < ceilings.Length; i++)
		{
			timers[i] = Random.Range(minDelay, maxDelay);
		}
	}

	private void Update()
	{
		if (!started)
		{
			return;
		}
		for (int i = 0; i < ceilings.Length; i++)
		{
			if (timers[i] > 0f)
			{
				timers[i] -= Time.deltaTime;
				continue;
			}
			if (!soundPlayed[i])
			{
				soundPlayed[i] = true;
				if (fallSound != null)
				{
					audioSource.PlayOneShot(fallSound, soundVolume);
				}
			}
			ceilings[i].transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
			ceilings[i].transform.Rotate(spinSpeed * Time.deltaTime, spinSpeed * Time.deltaTime, spinSpeed * Time.deltaTime, Space.World);
		}
	}

	public void StartCeilings()
	{
		started = true;
	}
}
