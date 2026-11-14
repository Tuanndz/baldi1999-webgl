using System;
using System.Text.RegularExpressions;

namespace ControlFreak2
{
	[Serializable]
	public class GamepadProfile
	{
		public enum DeviceType
		{
			Unknown = 0,
			PS3 = 1,
			PS4 = 2,
			Xbox360 = 3,
			XboxOne = 4,
			MOGA = 5,
			OUYA = 6
		}

		public enum ProfileMode
		{
			Normal = 0,
			Regex = 1
		}

		[Serializable]
		public class JoystickSource
		{
			public KeySource keyU;

			public KeySource keyD;

			public KeySource keyR;

			public KeySource keyL;

			public JoystickSource()
			{
				keyD = new KeySource();
				keyU = new KeySource();
				keyL = new KeySource();
				keyR = new KeySource();
			}

			public static JoystickSource Dpad(int keyU, int keyR, int keyD, int keyL)
			{
				JoystickSource joystickSource = new JoystickSource();
				joystickSource.keyD.SetKey(keyD);
				joystickSource.keyU.SetKey(keyU);
				joystickSource.keyL.SetKey(keyL);
				joystickSource.keyR.SetKey(keyR);
				return joystickSource;
			}

			public static JoystickSource Axes(int horzAxisId, bool horzPositiveRight, int vertAxisId, bool vertPositiveUp)
			{
				JoystickSource joystickSource = new JoystickSource();
				joystickSource.keyR.SetAxis(horzAxisId, horzPositiveRight);
				joystickSource.keyL.SetAxis(horzAxisId, horzPositiveRight);
				joystickSource.keyU.SetAxis(vertAxisId, vertPositiveUp);
				joystickSource.keyD.SetAxis(vertAxisId, vertPositiveUp);
				return joystickSource;
			}

			public bool IsDuplicateOf(JoystickSource a)
			{
				if (keyD.IsDuplicateOf(a.keyD) && keyU.IsDuplicateOf(a.keyU) && keyL.IsDuplicateOf(a.keyL))
				{
					return keyR.IsDuplicateOf(a.keyR);
				}
				return false;
			}

			public static JoystickSource Empty()
			{
				return new JoystickSource();
			}
		}

		[Serializable]
		public class KeySource
		{
			public int keyId;

			public int axisId;

			public bool axisSign;

			public KeySource()
			{
				keyId = -1;
				axisId = -1;
				axisSign = true;
			}

			public bool IsEmpty()
			{
				if (keyId < 0)
				{
					return axisId < 0;
				}
				return false;
			}

			public bool IsDuplicateOf(KeySource a)
			{
				if (keyId == a.keyId && axisId == a.axisId)
				{
					return axisSign == a.axisSign;
				}
				return false;
			}

			public void Clear()
			{
				axisId = -1;
				keyId = -1;
				axisSign = true;
			}

			public void SetKey(int keyId)
			{
				this.keyId = keyId;
				axisId = -1;
				axisSign = true;
			}

			public void SetAxis(int axisId, bool axisSign)
			{
				keyId = -1;
				this.axisId = axisId;
				this.axisSign = axisSign;
			}

			private KeySource(int keyId, int axisId, bool axisSign)
			{
				this.axisId = axisId;
				this.keyId = keyId;
				this.axisSign = axisSign;
			}

			public static KeySource Key(int keyId)
			{
				return new KeySource(keyId, -1, true);
			}

			public static KeySource PlusAxis(int axisId)
			{
				return new KeySource(-1, axisId, true);
			}

			public static KeySource MinusAxis(int axisId)
			{
				return new KeySource(-1, axisId, false);
			}

			public static KeySource KeyAndPlusAxis(int keyId, int axisId)
			{
				return new KeySource(keyId, axisId, true);
			}

			public static KeySource KeyAndMinusAxis(int keyId, int axisId)
			{
				return new KeySource(keyId, axisId, false);
			}

			public static KeySource Empty()
			{
				return new KeySource(-1, -1, true);
			}
		}

		public class GenericProfile : GamepadProfile
		{
			public GenericProfile(JoystickSource leftStick = null, JoystickSource rightStick = null, JoystickSource dpad = null, KeySource faceBottom = null, KeySource faceRight = null, KeySource faceLeft = null, KeySource faceTop = null, KeySource select = null, KeySource start = null, KeySource L1 = null, KeySource R1 = null, KeySource L2 = null, KeySource R2 = null, KeySource L3 = null, KeySource R3 = null)
				: base("Generic Gamepad", "", ProfileMode.Normal, null, null, leftStick, rightStick, dpad, faceBottom, faceRight, faceLeft, faceTop, select, start, L1, R1, L2, R2, L3, R3)
			{
			}
		}

		public const int CAP_DPAD = 1;

		public const int CAP_LEFT_STICK = 2;

		public const int CAP_RIGHT_STICK = 4;

		public const int CAP_START = 8;

		public const int CAP_SELECT = 16;

		public const int CAP_SHOULDER_KEYS = 32;

		public const int CAP_ANALOG_TRIGGERS = 64;

		public const int CAP_PRESSABLE_STICKS = 128;

		public string name;

		public string joystickIdentifier;

		public ProfileMode profileMode;

		public string unityVerFrom;

		public string unityVerTo;

		public JoystickSource leftStick;

		public JoystickSource rightStick;

		public JoystickSource dpad;

		public KeySource keyFaceU;

		public KeySource keyFaceR;

		public KeySource keyFaceD;

		public KeySource keyFaceL;

		public KeySource keyStart;

		public KeySource keySelect;

		public KeySource keyL1;

		public KeySource keyR1;

		public KeySource keyL2;

		public KeySource keyR2;

		public KeySource keyL3;

		public KeySource keyR3;

		public GamepadProfile()
		{
			name = "New Profile";
			joystickIdentifier = "Device Identifier";
			profileMode = ProfileMode.Normal;
			unityVerFrom = "4.7";
			unityVerTo = "9.9";
			dpad = new JoystickSource();
			leftStick = new JoystickSource();
			rightStick = new JoystickSource();
			keyFaceU = KeySource.Empty();
			keyFaceR = KeySource.Empty();
			keyFaceD = KeySource.Empty();
			keyFaceL = KeySource.Empty();
			keyStart = KeySource.Empty();
			keySelect = KeySource.Empty();
			keyL1 = KeySource.Empty();
			keyR1 = KeySource.Empty();
			keyL2 = KeySource.Empty();
			keyR2 = KeySource.Empty();
			keyL3 = KeySource.Empty();
			keyR3 = KeySource.Empty();
		}

		public bool IsCompatible(string deviceName)
		{
			if (profileMode == ProfileMode.Normal)
			{
				if (deviceName.IndexOf(joystickIdentifier, StringComparison.OrdinalIgnoreCase) < 0)
				{
					return false;
				}
			}
			else if (profileMode == ProfileMode.Regex && !Regex.IsMatch(deviceName, joystickIdentifier, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
			{
				return false;
			}
			return true;
		}

		public void AddSupportedVersion(string unityVer)
		{
			if (!string.IsNullOrEmpty(unityVer) && unityVerTo.CompareTo(unityVer) < 0)
			{
				unityVerTo = unityVer;
			}
		}

		public JoystickSource GetJoystickSource(int id)
		{
			JoystickSource result = null;
			switch (id)
			{
			case 0:
				result = leftStick;
				break;
			case 1:
				result = rightStick;
				break;
			case 2:
				result = dpad;
				break;
			}
			return result;
		}

		public KeySource GetKeySource(int id)
		{
			KeySource result = null;
			switch (id)
			{
			case 0:
				result = keyFaceD;
				break;
			case 1:
				result = keyFaceR;
				break;
			case 3:
				result = keyFaceL;
				break;
			case 2:
				result = keyFaceU;
				break;
			case 4:
				result = keyStart;
				break;
			case 5:
				result = keySelect;
				break;
			case 6:
				result = keyL1;
				break;
			case 7:
				result = keyR1;
				break;
			case 8:
				result = keyL2;
				break;
			case 9:
				result = keyR2;
				break;
			case 10:
				result = keyL3;
				break;
			case 11:
				result = keyR3;
				break;
			}
			return result;
		}

		public bool IsDuplicateOf(GamepadProfile profile)
		{
			if (!joystickIdentifier.Equals(profile.joystickIdentifier, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			for (int i = 0; i < 12; i++)
			{
				if (!GetKeySource(i).IsDuplicateOf(profile.GetKeySource(i)))
				{
					return false;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				if (!GetJoystickSource(j).IsDuplicateOf(profile.GetJoystickSource(j)))
				{
					return false;
				}
			}
			return true;
		}

		public GamepadProfile(string name, string deviceIdentifier, ProfileMode profileMode = ProfileMode.Normal, string unityVerFrom = null, string unityVerTo = null, JoystickSource leftStick = null, JoystickSource rightStick = null, JoystickSource dpad = null, KeySource faceBottom = null, KeySource faceRight = null, KeySource faceLeft = null, KeySource faceTop = null, KeySource select = null, KeySource start = null, KeySource L1 = null, KeySource R1 = null, KeySource L2 = null, KeySource R2 = null, KeySource L3 = null, KeySource R3 = null)
		{
			this.name = name;
			joystickIdentifier = deviceIdentifier;
			this.profileMode = profileMode;
			this.unityVerFrom = (string.IsNullOrEmpty(unityVerFrom) ? "4.3" : unityVerFrom);
			this.unityVerTo = (string.IsNullOrEmpty(unityVerTo) ? "9.9" : unityVerTo);
			this.leftStick = ((leftStick != null) ? leftStick : JoystickSource.Empty());
			this.rightStick = ((rightStick != null) ? rightStick : JoystickSource.Empty());
			this.dpad = ((dpad != null) ? dpad : JoystickSource.Empty());
			keyFaceU = ((faceTop != null) ? faceTop : KeySource.Empty());
			keyFaceR = ((faceRight != null) ? faceRight : KeySource.Empty());
			keyFaceD = ((faceBottom != null) ? faceBottom : KeySource.Empty());
			keyFaceL = ((faceLeft != null) ? faceLeft : KeySource.Empty());
			keyStart = ((start != null) ? start : KeySource.Empty());
			keySelect = ((select != null) ? select : KeySource.Empty());
			keyL1 = ((L1 != null) ? L1 : KeySource.Empty());
			keyR1 = ((R1 != null) ? R1 : KeySource.Empty());
			keyL2 = ((L2 != null) ? L2 : KeySource.Empty());
			keyR2 = ((R2 != null) ? R2 : KeySource.Empty());
			keyL3 = ((L3 != null) ? L3 : KeySource.Empty());
			keyR3 = ((R3 != null) ? R3 : KeySource.Empty());
		}
	}
}
