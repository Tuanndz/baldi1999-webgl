using UnityEngine;

public class PlatformSpecificMenu : MonoBehaviour
{
	public GameObject pC;

	public GameObject mobile;

	private void Start()
	{
		bool isMobile = Application.isMobilePlatform;
		if (pC != null)
		{
			pC.SetActive(!isMobile);
		}
		if (mobile != null)
		{
			mobile.SetActive(isMobile);
		}
	}
}
