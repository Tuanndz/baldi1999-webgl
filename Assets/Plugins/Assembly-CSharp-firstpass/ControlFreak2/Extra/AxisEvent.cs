using UnityEngine;
using UnityEngine.Events;

namespace ControlFreak2.Extra
{
	public class AxisEvent : MonoBehaviour
	{
		public enum EventType
		{
			OnPress = 0,
			OnRelease = 1,
			WhenPressed = 2
		}

		public string axisName = "Horizontal";

		[Range(0.1f, 0.9f)]
		public float axisDeadzone = 0.5f;

		private int axisId;

		private int prevAxisDir;

		public EventType eventType;

		public UnityEvent positiveEvent;

		public UnityEvent negativeEvent;

		public AxisEvent()
		{
			positiveEvent = new UnityEvent();
			negativeEvent = new UnityEvent();
		}

		private void OnEnable()
		{
			prevAxisDir = 0;
		}

		private void Update()
		{
			float axisRaw = CF2Input.GetAxisRaw(axisName, ref axisId);
			int num = ((Mathf.Abs(axisRaw) > axisDeadzone) ? ((axisRaw > 0f) ? 1 : (-1)) : 0);
			switch (eventType)
			{
			case EventType.OnPress:
				if (num != 0 && prevAxisDir != num)
				{
					((num > 0) ? positiveEvent : negativeEvent).Invoke();
				}
				break;
			case EventType.OnRelease:
				if (prevAxisDir != 0 && num != prevAxisDir)
				{
					((prevAxisDir > 0) ? positiveEvent : negativeEvent).Invoke();
				}
				break;
			case EventType.WhenPressed:
				if (num != 0)
				{
					((num > 0) ? positiveEvent : negativeEvent).Invoke();
				}
				break;
			}
			prevAxisDir = num;
		}
	}
}
