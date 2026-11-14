using UnityEngine;

public class CameraScript : MonoBehaviour
{
	public GameObject player;

	public PlayerScript ps;

	public Transform baldi;

	public Transform MonsterBaldi;

	public Vector3 BaldiOffset;

	public float MonsterBaldiOffsetY = 2.5f;

	public float initVelocity;

	public float velocity;

	public float gravity;

	private int lookBehind;

	public Vector3 offset;

	public float jumpHeight;

	public Vector3 jumpHeightV3;

	private void Start()
	{
		offset = base.transform.position - player.transform.position;
	}

	private void Update()
	{
		if (ps.jumpRope)
		{
			velocity -= gravity * Time.deltaTime;
			jumpHeight += velocity * Time.deltaTime;
			if (jumpHeight <= 0f)
			{
				jumpHeight = 0f;
				if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Jump))
				{
					velocity = initVelocity;
				}
			}
			jumpHeightV3 = new Vector3(0f, jumpHeight, 0f);
		}
		else if (Singleton<InputManager>.Instance.GetActionKey(InputAction.LookBehind))
		{
			lookBehind = 180;
		}
		else
		{
			lookBehind = 0;
		}
	}

	private void LateUpdate()
	{
		base.transform.position = player.transform.position + offset;
		if (!ps.gameOver & !ps.jumpRope)
		{
			base.transform.position = player.transform.position + offset;
			base.transform.rotation = player.transform.rotation * Quaternion.Euler(0f, lookBehind, 0f);
		}
		else if (ps.gameOver)
		{
			Transform monsterBaldi = baldi;
			float num = BaldiOffset.y;
			if (MonsterBaldi != null && MonsterBaldi.gameObject.activeInHierarchy)
			{
				monsterBaldi = MonsterBaldi;
				num = MonsterBaldiOffsetY;
			}
			base.transform.position = monsterBaldi.position + monsterBaldi.forward * BaldiOffset.z + new Vector3(0f, num, 0f);
			float num2 = 1.5f;
			base.transform.LookAt(new Vector3(monsterBaldi.position.x, monsterBaldi.position.y + num + num2, monsterBaldi.position.z));
		}
		else if (ps.jumpRope)
		{
			base.transform.position = player.transform.position + offset + jumpHeightV3;
			base.transform.rotation = player.transform.rotation;
		}
	}
}
