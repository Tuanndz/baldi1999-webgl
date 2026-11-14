using UnityEngine;

public class HoverHandScript : MonoBehaviour
{
	public GameObject MouseCursor;

	public Transform playerTransform;

	private void Start()
	{
	}

	private void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo) && ((hitInfo.collider.tag == "Door") & (Vector3.Distance(playerTransform.position, hitInfo.transform.position) <= 15f)))
		{
			MouseCursor.SetActive(true);
		}
		else if (Physics.Raycast(ray, out hitInfo) && ((hitInfo.collider.tag == "Item") & (Vector3.Distance(playerTransform.position, hitInfo.transform.position) <= 10f)))
		{
			MouseCursor.SetActive(true);
		}
		else if (Physics.Raycast(ray, out hitInfo) && ((hitInfo.collider.tag == "Notebook") & (Vector3.Distance(playerTransform.position, hitInfo.transform.position) <= 10f)))
		{
			MouseCursor.SetActive(true);
		}
		else
		{
			MouseCursor.SetActive(false);
		}
	}
}
