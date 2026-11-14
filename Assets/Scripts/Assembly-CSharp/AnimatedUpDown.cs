using UnityEngine;

public class AnimatedUpDown : MonoBehaviour
{
	private RectTransform itemPosition;

	private void Start()
	{
		itemPosition = GetComponent<RectTransform>();
	}

	private void Update()
	{
		itemPosition.localPosition = new Vector3(0f, Mathf.Sin((float)Time.frameCount * 0.035f) * 40f, 0f);
	}
}
