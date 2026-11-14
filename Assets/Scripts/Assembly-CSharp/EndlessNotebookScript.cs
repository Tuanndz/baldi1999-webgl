using ControlFreak2;
using UnityEngine;

public class EndlessNotebookScript : MonoBehaviour
{
	public float openingDistance;

	public GameControllerScript gc;

	public Transform player;

	public GameObject learningGame;

	private void Start()
	{
		gc = GameObject.Find("Game Controller").GetComponent<GameControllerScript>();
		player = GameObject.Find("Player").GetComponent<Transform>();
	}

	private void Update()
	{
		RaycastHit hitInfo;
		if (CF2Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out hitInfo) && ((hitInfo.transform.tag == "Notebook") & (Vector3.Distance(player.position, base.transform.position) < openingDistance)))
		{
			base.gameObject.SetActive(false);
			gc.CollectNotebook();
			learningGame.SetActive(true);
		}
	}
}
