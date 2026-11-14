using UnityEngine;

public class NearExitTriggerScript : MonoBehaviour
{
	public GameControllerScript gc;

	public EntranceScript es;

	private void OnTriggerEnter(Collider other)
	{
		if ((gc.exitsReached < 3) & gc.finaleMode & (other.tag == "Player"))
		{
			gc.ExitReached();
			es.Lower();
			if (gc.baldiScrpt.isActiveAndEnabled)
			{
				gc.baldiScrpt.Hear(base.transform.position, 8f);
			}
		}
	}
}
