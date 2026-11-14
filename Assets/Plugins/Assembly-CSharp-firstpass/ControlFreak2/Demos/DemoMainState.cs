using UnityEngine;

namespace ControlFreak2.Demos
{
	public class DemoMainState : GameState
	{
		public MultiDemoManager multiDemoManager;

		public HelpBoxState helpBox;

		public KeyCode helpKey = KeyCode.Escape;

		protected override void OnStartState(GameState parentState)
		{
			base.OnStartState(parentState);
			base.gameObject.SetActive(true);
		}

		protected override void OnExitState()
		{
			base.OnExitState();
			base.gameObject.SetActive(false);
		}

		protected override void OnUpdateState()
		{
			if (helpBox != null && !helpBox.IsRunning() && helpKey != KeyCode.None && CF2Input.GetKeyDown(helpKey))
			{
				ShowHelpBox();
			}
			base.OnUpdateState();
		}

		public void ExitToMainMenu()
		{
			if (multiDemoManager != null)
			{
				multiDemoManager.EnterMainMenu();
			}
			else
			{
				Application.Quit();
			}
		}

		public void ShowHelpBox()
		{
			if (helpBox != null)
			{
				helpBox.ShowHelpBox(this);
			}
		}
	}
}
