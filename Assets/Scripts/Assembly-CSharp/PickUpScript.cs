using ControlFreak2;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
	public GameControllerScript gc;

	public Transform player;

	public AudioClip aud_pickup;

	public AudioSource audioClip;

	private void Start()
	{
	}

	private void Update()
	{
		RaycastHit hitInfo;
		if (CF2Input.GetMouseButtonDown(0) && Time.timeScale != 0f && Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f)), out hitInfo) && Vector3.Distance(player.position, hitInfo.transform.position) < 15f)
		{
			switch (hitInfo.transform.name)
			{
			case "Pickup_EnergyFlavoredZestyBar":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(1);
				audioClip.PlayOneShot(aud_pickup);
				break;
			case "Pickup_YellowDoorLock":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(2);
				audioClip.PlayOneShot(aud_pickup);
				break;
			case "Pickup_Key":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(3);
				audioClip.PlayOneShot(aud_pickup);
				break;
			case "Pickup_BSODA":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(4);
				audioClip.PlayOneShot(aud_pickup);
				break;
			case "Pickup_Quarter":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(5);
				audioClip.PlayOneShot(aud_pickup);
				break;
			case "Pickup_Tape":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(6);
				audioClip.PlayOneShot(aud_pickup);
				break;
			case "Pickup_AlarmClock":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(7);
				audioClip.PlayOneShot(aud_pickup);
				break;
			case "Pickup_WD-3D":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(8);
				audioClip.PlayOneShot(aud_pickup);
				break;
			case "Pickup_SafetyScissors":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(9);
				audioClip.PlayOneShot(aud_pickup);
				break;
			case "Pickup_BigBoots":
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(10);
				audioClip.PlayOneShot(aud_pickup);
				break;
			}
		}
	}
}
public class PickupScript : MonoBehaviour
{
	public GameControllerScript gc;

	public Transform player;

	private void Update()
	{
		RaycastHit hitInfo;
		if (Singleton<InputManager>.Instance.GetActionKeyDown(InputAction.Interact) && Time.timeScale != 0f && Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out hitInfo))
		{
			if ((hitInfo.transform.name == "Pickup_EnergyFlavoredZestyBar") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(1);
			}
			else if ((hitInfo.transform.name == "Pickup_YellowDoorLock") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(2);
			}
			else if ((hitInfo.transform.name == "Pickup_Key") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(3);
			}
			else if ((hitInfo.transform.name == "Pickup_BSODA") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(4);
			}
			else if ((hitInfo.transform.name == "Pickup_Quarter") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(5);
			}
			else if ((hitInfo.transform.name == "Pickup_Tape") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(6);
			}
			else if ((hitInfo.transform.name == "Pickup_AlarmClock") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(7);
			}
			else if ((hitInfo.transform.name == "Pickup_WD-3D") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(8);
			}
			else if ((hitInfo.transform.name == "Pickup_SafetyScissors") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(9);
			}
			else if ((hitInfo.transform.name == "Pickup_BigBoots") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(10);
			}
			else if ((hitInfo.transform.name == "Pickup_Teleporter") & (Vector3.Distance(player.position, base.transform.position) < 10f))
			{
				hitInfo.transform.gameObject.SetActive(false);
				gc.CollectItem(11);
			}
		}
	}
}
