using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRespawnOnce : MonoBehaviour
{
	public List<GameObject> itemsToRespawn;

	public float respawnDelay = 15f;

	private Dictionary<GameObject, bool> respawned = new Dictionary<GameObject, bool>();

	private Dictionary<GameObject, Vector3> originalPos = new Dictionary<GameObject, Vector3>();

	private Dictionary<GameObject, Quaternion> originalRot = new Dictionary<GameObject, Quaternion>();

	private bool phaseStarted;

	private void Start()
	{
		foreach (GameObject item in itemsToRespawn)
		{
			respawned[item] = false;
			originalPos[item] = item.transform.position;
			originalRot[item] = item.transform.rotation;
		}
	}

	private void Update()
	{
		if (!phaseStarted && SecondPhase.secondPhaseActive)
		{
			phaseStarted = true;
			StartCoroutine(RespawnAllPickedItems());
		}
	}

	private IEnumerator RespawnAllPickedItems()
	{
		yield return new WaitForSeconds(respawnDelay);
		foreach (GameObject item in itemsToRespawn)
		{
			if (!respawned[item] && !item.activeSelf)
			{
				respawned[item] = true;
				RespawnItem(item);
			}
		}
	}

	private void RespawnItem(GameObject item)
	{
		item.transform.position = originalPos[item];
		item.transform.rotation = originalRot[item];
		item.SetActive(true);
	}
}
