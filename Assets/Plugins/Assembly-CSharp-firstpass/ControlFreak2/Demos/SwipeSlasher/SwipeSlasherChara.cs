using UnityEngine;
using UnityEngine.UI;

namespace ControlFreak2.Demos.SwipeSlasher
{
	public class SwipeSlasherChara : MonoBehaviour
	{
		public enum ActionType
		{
			LEFT_STAB = 0,
			LEFT_SLASH_U = 1,
			LEFT_SLASH_R = 2,
			LEFT_SLASH_D = 3,
			LEFT_SLASH_L = 4,
			RIGHT_STAB = 5,
			RIGHT_SLASH_U = 6,
			RIGHT_SLASH_R = 7,
			RIGHT_SLASH_D = 8,
			RIGHT_SLASH_L = 9,
			DODGE_LEFT = 10,
			DODGE_RIGHT = 11
		}

		public AudioClip soundDodge;

		public AudioClip soundSlash;

		public AudioClip soundStab;

		public Graphic graphicLeftStab;

		public Graphic graphicLeftSlashU;

		public Graphic graphicLeftSlashR;

		public Graphic graphicLeftSlashD;

		public Graphic graphicLeftSlashL;

		public Graphic graphicRightStab;

		public Graphic graphicRightSlashU;

		public Graphic graphicRightSlashR;

		public Graphic graphicRightSlashD;

		public Graphic graphicRightSlashL;

		public Graphic graphicDodgeLeft;

		public Graphic graphicDodgeRight;

		public float fadeDur = 0.5f;

		private Animator animator;

		public string animLeftStab = "Left-Stab";

		public string animLeftSlashU = "Left-Slash-U";

		public string animLeftSlashR = "Left-Slash-R";

		public string animLeftSlashD = "Left-Slash-D";

		public string animLeftSlashL = "Left-Slash-L";

		public string animRightStab = "Right-Stab";

		public string animRightSlashU = "Right-Slash-U";

		public string animRightSlashR = "Right-Slash-R";

		public string animRightSlashD = "Right-Slash-D";

		public string animRightSlashL = "Right-Slash-L";

		public string animDodgeLeft = "Dodge-Left";

		public string animDodgeRight = "Dodge-Right";

		private void OnEnable()
		{
			animator = GetComponent<Animator>();
		}

		private void Start()
		{
			for (ActionType actionType = ActionType.LEFT_STAB; actionType <= ActionType.DODGE_RIGHT; actionType++)
			{
				Graphic actionGraphic = GetActionGraphic(actionType);
				if (actionGraphic != null)
				{
					Color color = actionGraphic.color;
					color.a = 0f;
					actionGraphic.color = color;
				}
			}
		}

		private Graphic GetActionGraphic(ActionType a)
		{
			switch (a)
			{
			case ActionType.DODGE_LEFT:
				return graphicDodgeLeft;
			case ActionType.DODGE_RIGHT:
				return graphicDodgeRight;
			case ActionType.LEFT_STAB:
				return graphicLeftStab;
			case ActionType.LEFT_SLASH_U:
				return graphicLeftSlashU;
			case ActionType.LEFT_SLASH_D:
				return graphicLeftSlashD;
			case ActionType.LEFT_SLASH_R:
				return graphicLeftSlashR;
			case ActionType.LEFT_SLASH_L:
				return graphicLeftSlashL;
			case ActionType.RIGHT_STAB:
				return graphicRightStab;
			case ActionType.RIGHT_SLASH_U:
				return graphicRightSlashU;
			case ActionType.RIGHT_SLASH_D:
				return graphicRightSlashD;
			case ActionType.RIGHT_SLASH_R:
				return graphicRightSlashR;
			case ActionType.RIGHT_SLASH_L:
				return graphicRightSlashL;
			default:
				return null;
			}
		}

		private void SetActionTriggerState(ActionType action)
		{
			if (!(animator == null))
			{
				string text = null;
				switch (action)
				{
				case ActionType.LEFT_STAB:
					text = animLeftStab;
					break;
				case ActionType.LEFT_SLASH_U:
					text = animLeftSlashU;
					break;
				case ActionType.LEFT_SLASH_R:
					text = animLeftSlashR;
					break;
				case ActionType.LEFT_SLASH_D:
					text = animLeftSlashD;
					break;
				case ActionType.LEFT_SLASH_L:
					text = animLeftSlashL;
					break;
				case ActionType.RIGHT_STAB:
					text = animRightStab;
					break;
				case ActionType.RIGHT_SLASH_U:
					text = animRightSlashU;
					break;
				case ActionType.RIGHT_SLASH_R:
					text = animRightSlashR;
					break;
				case ActionType.RIGHT_SLASH_D:
					text = animRightSlashD;
					break;
				case ActionType.RIGHT_SLASH_L:
					text = animRightSlashL;
					break;
				case ActionType.DODGE_LEFT:
					text = animDodgeLeft;
					break;
				case ActionType.DODGE_RIGHT:
					text = animDodgeRight;
					break;
				}
				if (!string.IsNullOrEmpty(text))
				{
					animator.SetTrigger(text);
				}
			}
		}

		public void ExecuteAction(ActionType action)
		{
			Graphic actionGraphic = GetActionGraphic(action);
			switch (action)
			{
			case ActionType.LEFT_STAB:
			case ActionType.RIGHT_STAB:
				PlaySound(soundStab);
				break;
			case ActionType.DODGE_LEFT:
			case ActionType.DODGE_RIGHT:
				PlaySound(soundDodge);
				break;
			default:
				PlaySound(soundSlash);
				break;
			}
			if (animator != null)
			{
				SetActionTriggerState(action);
			}
			if (actionGraphic != null)
			{
				Color color = actionGraphic.color;
				color.a = 1f;
				actionGraphic.color = color;
				actionGraphic.CrossFadeAlpha(1f, 0f, true);
				actionGraphic.CrossFadeAlpha(0f, fadeDur, true);
			}
		}

		private void PlaySound(AudioClip clip)
		{
			if (clip != null)
			{
				AudioSource.PlayClipAtPoint(clip, Vector3.zero);
			}
		}
	}
}
