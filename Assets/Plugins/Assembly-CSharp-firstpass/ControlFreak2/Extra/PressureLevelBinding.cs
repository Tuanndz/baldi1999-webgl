using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2.Extra
{
	public class PressureLevelBinding : MonoBehaviour
	{
		[Serializable]
		public class RangeConfig
		{
			[Range(-1f, 1f)]
			public float min = 0.5f;

			[Range(-1f, 1f)]
			public float max = 1f;

			[Tooltip("Target key code.")]
			public KeyCode keyTarget;

			[Tooltip("Target axis name.")]
			public string axisTarget = "";

			public bool positiveAxisSide = true;

			private int cachedAxisId;

			public void Update(InputRig rig, float val)
			{
				if (!(val < min) && !(val > max))
				{
					if (keyTarget != KeyCode.None)
					{
						rig.SetKeyCode(keyTarget);
					}
					if (!string.IsNullOrEmpty(axisTarget))
					{
						rig.SetAxisDigital(axisTarget, ref cachedAxisId, !positiveAxisSide);
					}
				}
			}
		}

		private InputRig rig;

		[Tooltip("Source axis name")]
		public string sourceAxis;

		private int sourceAxisId;

		public List<RangeConfig> rangeConfigList;

		public PressureLevelBinding()
		{
			rangeConfigList = new List<RangeConfig>(new RangeConfig[1]
			{
				new RangeConfig()
			});
		}

		private void OnEnable()
		{
			if (rig == null)
			{
				rig = GetComponentInParent<InputRig>();
			}
			if (rig != null)
			{
				rig.onAddExtraInput += UpdateRanges;
			}
		}

		private void OnDisable()
		{
			if (rig != null)
			{
				rig.onAddExtraInput -= UpdateRanges;
			}
		}

		protected void UpdateRanges()
		{
			if (!(rig == null) && !string.IsNullOrEmpty(sourceAxis))
			{
				float axisRaw = rig.GetAxisRaw(sourceAxis, ref sourceAxisId);
				for (int i = 0; i < rangeConfigList.Count; i++)
				{
					rangeConfigList[i].Update(rig, axisRaw);
				}
			}
		}
	}
}
