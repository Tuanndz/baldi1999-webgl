using UnityEngine;
using UnityEngine.UI;

namespace ControlFreak2
{
	public class NotificationElementGUI : MonoBehaviour
	{
		public Text targetText;

		public Image targetIcon;

		private float duration;

		private float elapsed;

		public void End()
		{
			base.gameObject.SetActive(false);
		}

		public void Show(string msg, Sprite icon, RectTransform parent, float duration)
		{
			RectTransform obj = (RectTransform)base.transform;
			obj.SetParent(parent, false);
			obj.SetSiblingIndex(0);
			base.gameObject.SetActive(true);
			this.duration = duration;
			elapsed = 0f;
			targetText.text = msg;
			if (targetIcon != null && (targetIcon.enabled = icon != null))
			{
				targetIcon.sprite = icon;
			}
		}

		public void UpdateNotification(float dt)
		{
			if (IsActive())
			{
				elapsed += dt;
				if (elapsed > duration)
				{
					base.gameObject.SetActive(false);
				}
				OnUpdate();
			}
		}

		public float GetTimeElapsed()
		{
			return elapsed;
		}

		protected virtual void OnUpdate()
		{
			float num = elapsed / duration;
			base.transform.localScale = Vector3.one * Mathf.Clamp01((1f - num) * 8f);
		}

		public bool IsActive()
		{
			return base.gameObject.activeSelf;
		}

		public static int Compare(NotificationElementGUI a, NotificationElementGUI b)
		{
			if (a == null || b == null)
			{
				return 0;
			}
			if (a.IsActive() != b.IsActive())
			{
				if (!a.IsActive())
				{
					return 1;
				}
				return -1;
			}
			if (!a.IsActive())
			{
				return 0;
			}
			if (a.elapsed != b.elapsed)
			{
				if (!(a.elapsed < b.elapsed))
				{
					return 1;
				}
				return -1;
			}
			return 0;
		}
	}
}
