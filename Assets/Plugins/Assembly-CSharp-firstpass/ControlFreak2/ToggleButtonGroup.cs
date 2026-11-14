using UnityEngine;

namespace ControlFreak2
{
	public class ToggleButtonGroup : MonoBehaviour
	{
		public TouchButton[] groupButtons = new TouchButton[0];

		public void ActivateButton(TouchButton activeButton)
		{
			for (int i = 0; i < groupButtons.Length; i++)
			{
				TouchButton touchButton = groupButtons[i];
				if ((bool)touchButton)
				{
					bool flag = touchButton == activeButton;
					if (touchButton.Toggled() != flag)
					{
						touchButton.ChangeToggleState(flag);
					}
				}
			}
		}

		private void OnEnable()
		{
			ActivateButton(null);
		}

		private void Update()
		{
			for (int i = 0; i < groupButtons.Length; i++)
			{
				TouchButton touchButton = groupButtons[i];
				if ((bool)touchButton && touchButton.JustToggled())
				{
					ActivateButton(touchButton);
				}
			}
		}
	}
}
