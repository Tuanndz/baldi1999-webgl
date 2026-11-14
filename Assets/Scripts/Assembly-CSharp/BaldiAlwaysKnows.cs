using UnityEngine;

public class BaldiAlwaysKnows : MonoBehaviour
{
	public BaldiScript baldi;

	public Transform player;

	public float updateRate = 0.2f;

	private float timer;

	private void Start()
	{
		if (baldi == null)
		{
			baldi = GetComponent<BaldiScript>();
		}
	}

	private void Update()
	{
		if (!(player == null) && !(baldi == null))
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				timer = updateRate;
				baldi.agent.SetDestination(player.position);
				baldi.currentSpeed = baldi.runSpeed;
				baldi.coolDown = 1f;
				baldi.currentPriority = 999f;
			}
		}
	}
}
