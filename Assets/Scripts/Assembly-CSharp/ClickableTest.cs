using ControlFreak2;
using UnityEngine;

public class ClickableTest : MonoBehaviour
{
	private Transform player;

	public AudioSource test;

	public AudioClip testHit;

	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
		if (gameObject != null)
		{
			player = gameObject.transform;
		}
	}

	private void Update()
	{
		RaycastHit hitInfo;
		if (CF2Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(CF2Input.mousePosition), out hitInfo) && ((hitInfo.transform.tag == "Notebook") & (Vector3.Distance(player.position, base.transform.position) < 10f)))
		{
			test.PlayOneShot(testHit);
			base.gameObject.SetActive(false);
		}
	}
}
