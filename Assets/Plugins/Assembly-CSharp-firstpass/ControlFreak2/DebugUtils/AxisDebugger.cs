using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2.DebugUtils
{
	public class AxisDebugger : MonoBehaviour
	{
		public bool drawGUI = true;

		public bool drawMouseStats;

		public bool drawUnityAxes;

		public bool drawKeyCodes = true;

		[Tooltip("When pressed, delta accumulators will be reset.")]
		public KeyCode deltaResetKey = KeyCode.F5;

		private Vector2 cfScroll;

		private Vector2 unScroll;

		private Vector2 cfMouseDelta;

		private Vector2 unMouseDelta;

		public GUISkin skin;

		private KeyCode[] allKeyCodes;

		public AxisDebugger()
		{
			allKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
		}

		private void Update()
		{
			if (Input.GetKeyDown(deltaResetKey))
			{
				unScroll = Vector2.zero;
				cfScroll = Vector2.zero;
				cfMouseDelta = Vector2.zero;
				unMouseDelta = Vector2.zero;
			}
			cfScroll += CF2Input.mouseScrollDelta;
			unScroll.x += Input.mouseScrollDelta.x;
			unScroll.y += Input.mouseScrollDelta.y;
			if (CF2Input.activeRig != null)
			{
				cfMouseDelta.x += CF2Input.GetAxis("Mouse X");
				cfMouseDelta.y += CF2Input.GetAxis("Mouse Y");
			}
			unMouseDelta.x += GetUnityAxis("Mouse X");
			unMouseDelta.y += GetUnityAxis("Mouse Y");
		}

		private void OnGUI()
		{
			if (CF2Input.activeRig == null)
			{
				return;
			}
			GUI.skin = skin;
			GUILayout.BeginVertical((GUI.skin != null) ? GUI.skin.box : GUIStyle.none);
			GUILayout.Box("Active RIG: " + CF2Input.activeRig.name);
			if (drawMouseStats)
			{
				GUILayout.Box(string.Concat("Scroll: cf:", cfScroll, " un:", unScroll, "\nMouse Delta: cf: ", cfMouseDelta, " un:", unMouseDelta));
			}
			if (drawKeyCodes)
			{
				InputRig activeRig = CF2Input.activeRig;
				if ((bool)activeRig && allKeyCodes != null)
				{
					string text = "";
					for (int i = 0; i < allKeyCodes.Length; i++)
					{
						if (activeRig.GetKey(allKeyCodes[i]))
						{
							text = text + (string.IsNullOrEmpty(text) ? "" : ", ") + allKeyCodes[i];
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						GUILayout.Box(text);
					}
				}
			}
			List<InputRig.AxisConfig> list = CF2Input.activeRig.axes.list;
			for (int j = 0; j < list.Count; j++)
			{
				InputRig.AxisConfig axisConfig = list[j];
				float num = 0f;
				float num2 = 0f;
				bool flag = false;
				num = axisConfig.GetAnalog();
				try
				{
					num2 = Input.GetAxis(axisConfig.name);
					flag = true;
				}
				catch (Exception)
				{
				}
				if (num != 0f || (drawUnityAxes && num2 != 0f))
				{
					GUILayout.BeginVertical();
					GUILayout.Label(axisConfig.name);
					GUI.color = ((num == 0f || num == 1f || num == -1f) ? Color.green : Color.gray);
					GUILayout.Label("CF : " + num.ToString("0.00000"));
					GUILayout.Box("", GUILayout.Width(Mathf.Abs(num) * 100f));
					if (drawUnityAxes)
					{
						GUI.color = ((num2 == 0f || num2 == 1f || num2 == -1f) ? Color.green : Color.gray);
						GUILayout.Label("UN : " + (flag ? num2.ToString("0.00000") : "UNAVAILABLE!"));
						GUILayout.Box("", GUILayout.Width(Mathf.Abs(num2) * 100f));
					}
					GUILayout.EndVertical();
					GUI.color = Color.white;
					InputRig.AxisType axisType = axisConfig.axisType;
				}
			}
			GUILayout.EndVertical();
		}

		private float GetUnityAxis(string name, float defaultVal = 0f)
		{
			float result = defaultVal;
			try
			{
				result = Input.GetAxis(name);
			}
			catch (Exception)
			{
			}
			return result;
		}
	}
}
