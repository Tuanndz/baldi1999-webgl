using System;
using UnityEngine;

namespace ControlFreak2
{
	public static class CF2Input
	{
		public enum MobileMode
		{
			Auto = 0,
			Enabled = 1,
			Disabled = 2
		}

		private const bool simulateMouseWithTouchesWhenRigIsActive = false;

		private static MobileMode mMobileMode = MobileMode.Enabled;

		private static bool mSimulateMouseWithTouches;

		private static InputRig mRig;

		public static InputRig activeRig
		{
			get
			{
				return mRig;
			}
			set
			{
				if (!(value == mRig))
				{
					if (mRig != null)
					{
						mRig.OnDisactivateRig();
					}
					else
					{
						mSimulateMouseWithTouches = Input.simulateMouseWithTouches;
					}
					mRig = value;
					if (mRig != null)
					{
						mRig.OnActivateRig();
						Input.simulateMouseWithTouches = false;
					}
					else
					{
						Input.simulateMouseWithTouches = mSimulateMouseWithTouches;
					}
					if (CF2Input.onActiveRigChange != null)
					{
						CF2Input.onActiveRigChange();
					}
					CFCursor.InternalRefresh();
				}
			}
		}

		public static bool anyKey
		{
			get
			{
				if (!(mRig != null))
				{
					return Input.anyKey;
				}
				return mRig.AnyKey();
			}
		}

		public static bool anyKeyDown
		{
			get
			{
				if (!(mRig != null))
				{
					return Input.anyKeyDown;
				}
				return mRig.AnyKeyDown();
			}
		}

		public static int touchCount
		{
			get
			{
				if (!(mRig != null))
				{
					return Input.touchCount;
				}
				return mRig.GetEmuTouchCount();
			}
		}

		public static InputRig.Touch[] touches
		{
			get
			{
				if (!(mRig != null))
				{
					return InputRig.Touch.TranslateUnityTouches(Input.touches);
				}
				return mRig.GetEmuTouchArray();
			}
		}

		public static Vector3 mousePosition
		{
			get
			{
				if (!(mRig == null))
				{
					return mRig.mouseConfig.GetPosition();
				}
				return Input.mousePosition;
			}
		}

		public static Vector2 mouseScrollDelta
		{
			get
			{
				if (!(mRig == null))
				{
					return mRig.scrollWheel.GetDelta();
				}
				return new Vector2(Input.mouseScrollDelta.x, Input.mouseScrollDelta.y);
			}
		}

		public static bool simulateMouseWithTouches
		{
			get
			{
				if (!(mRig == null))
				{
					return mSimulateMouseWithTouches;
				}
				return Input.simulateMouseWithTouches;
			}
			set
			{
				mSimulateMouseWithTouches = value;
				if (mRig == null)
				{
					Input.simulateMouseWithTouches = mSimulateMouseWithTouches;
				}
				else
				{
					Input.simulateMouseWithTouches = false;
				}
			}
		}

		public static event Action onMobileModeChange;

		public static event Action onActiveRigChange;

		public static MobileMode GetMobileMode()
		{
			return mMobileMode;
		}

		public static void SetMobileMode(MobileMode mode)
		{
			mMobileMode = mode;
			if (CF2Input.onMobileModeChange != null)
			{
				CF2Input.onMobileModeChange();
			}
			CFCursor.InternalRefresh();
		}

		public static bool IsInMobileMode()
		{
			if (mMobileMode != MobileMode.Enabled)
			{
				if (mMobileMode == MobileMode.Auto)
				{
					return IsMobilePlatform();
				}
				return false;
			}
			return true;
		}

		public static bool IsMobilePlatform()
		{
			return Application.isMobilePlatform;
		}

		[Obsolete("Use .IsInMobileMode()")]
		public static bool ControllerActive()
		{
			return IsInMobileMode();
		}

		public static void ResetInputAxes()
		{
			if (mRig != null)
			{
				mRig.ResetInputAxes();
			}
			Input.ResetInputAxes();
		}

		public static float GetAxis(string axisName)
		{
			if (!(mRig != null))
			{
				return Input.GetAxis(axisName);
			}
			return mRig.GetAxis(axisName);
		}

		public static float GetAxis(string axisName, ref int cachedId)
		{
			if (!(mRig != null))
			{
				return Input.GetAxis(axisName);
			}
			return mRig.GetAxis(axisName, ref cachedId);
		}

		public static float GetAxisRaw(string axisName)
		{
			if (!(mRig != null))
			{
				return Input.GetAxisRaw(axisName);
			}
			return mRig.GetAxisRaw(axisName);
		}

		public static float GetAxisRaw(string axisName, ref int cachedId)
		{
			if (!(mRig != null))
			{
				return Input.GetAxisRaw(axisName);
			}
			return mRig.GetAxisRaw(axisName, ref cachedId);
		}

		public static bool GetButton(string axisName)
		{
			if (!(mRig != null))
			{
				return Input.GetButton(axisName);
			}
			return mRig.GetButton(axisName);
		}

		public static bool GetButton(string axisName, ref int cachedId)
		{
			if (!(mRig != null))
			{
				return Input.GetButton(axisName);
			}
			return mRig.GetButton(axisName, ref cachedId);
		}

		public static bool GetButtonDown(string axisName)
		{
			if (!(mRig != null))
			{
				return Input.GetButtonDown(axisName);
			}
			return mRig.GetButtonDown(axisName);
		}

		public static bool GetButtonDown(string axisName, ref int cachedId)
		{
			if (!(mRig != null))
			{
				return Input.GetButtonDown(axisName);
			}
			return mRig.GetButtonDown(axisName, ref cachedId);
		}

		public static bool GetButtonUp(string axisName)
		{
			if (!(mRig != null))
			{
				return Input.GetButtonUp(axisName);
			}
			return mRig.GetButtonUp(axisName);
		}

		public static bool GetButtonUp(string axisName, ref int cachedId)
		{
			if (!(mRig != null))
			{
				return Input.GetButtonUp(axisName);
			}
			return mRig.GetButtonUp(axisName, ref cachedId);
		}

		public static bool GetKey(KeyCode keyCode)
		{
			if (!(mRig != null))
			{
				return Input.GetKey(keyCode);
			}
			return mRig.GetKey(keyCode);
		}

		public static bool GetKeyDown(KeyCode keyCode)
		{
			if (!(mRig != null))
			{
				return Input.GetKeyDown(keyCode);
			}
			return mRig.GetKeyDown(keyCode);
		}

		public static bool GetKeyUp(KeyCode keyCode)
		{
			if (!(mRig != null))
			{
				return Input.GetKeyUp(keyCode);
			}
			return mRig.GetKeyUp(keyCode);
		}

		[Obsolete("Please, use GetKey(KeyCode) version instead!")]
		public static bool GetKey(string keyName)
		{
			if (!(mRig != null))
			{
				return Input.GetKey(keyName);
			}
			return mRig.GetKey(keyName);
		}

		[Obsolete("Please, use GetKeyDown(KeyCode) version instead!")]
		public static bool GetKeyDown(string keyName)
		{
			if (!(mRig != null))
			{
				return Input.GetKeyDown(keyName);
			}
			return mRig.GetKeyDown(keyName);
		}

		[Obsolete("Please, use GetKeyUp(KeyCode) version instead!")]
		public static bool GetKeyUp(string keyName)
		{
			if (!(mRig != null))
			{
				return Input.GetKeyUp(keyName);
			}
			return mRig.GetKeyUp(keyName);
		}

		public static InputRig.Touch GetTouch(int i)
		{
			if (!(mRig != null))
			{
				return new InputRig.Touch(Input.GetTouch(i));
			}
			return mRig.GetEmuTouch(i);
		}

		public static bool GetMouseButton(int mouseButton)
		{
			if (!(mRig != null))
			{
				return Input.GetMouseButton(mouseButton);
			}
			return mRig.GetMouseButton(mouseButton);
		}

		public static bool GetMouseButtonDown(int mouseButton)
		{
			if (!(mRig != null))
			{
				return Input.GetMouseButtonDown(mouseButton);
			}
			return mRig.GetMouseButtonDown(mouseButton);
		}

		public static bool GetMouseButtonUp(int mouseButton)
		{
			if (!(mRig != null))
			{
				return Input.GetMouseButtonUp(mouseButton);
			}
			return mRig.GetMouseButtonUp(mouseButton);
		}

		public static void CalibrateTilt()
		{
			if (activeRig != null)
			{
				activeRig.CalibrateTilt();
			}
		}
	}
}
