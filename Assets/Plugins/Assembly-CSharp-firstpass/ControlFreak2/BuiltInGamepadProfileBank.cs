using UnityEngine;

namespace ControlFreak2
{
	public abstract class BuiltInGamepadProfileBank
	{
		private static BuiltInGamepadProfileBank mInst;

		protected GamepadProfile[] profiles;

		protected GamepadProfile genericProfile;

		public static GamepadProfile GetProfile(string deviceName)
		{
			BuiltInGamepadProfileBank builtInGamepadProfileBank = Inst();
			if (builtInGamepadProfileBank != null)
			{
				return builtInGamepadProfileBank.FindProfile(deviceName);
			}
			return null;
		}

		public static GamepadProfile GetGenericProfile()
		{
			BuiltInGamepadProfileBank builtInGamepadProfileBank = Inst();
			if (builtInGamepadProfileBank != null)
			{
				return builtInGamepadProfileBank.GetInternalGenericProfile();
			}
			return null;
		}

		protected virtual GamepadProfile GetInternalGenericProfile()
		{
			if (genericProfile == null)
			{
				genericProfile = new GamepadProfile.GenericProfile();
			}
			return genericProfile;
		}

		protected virtual GamepadProfile FindProfile(string deviceName)
		{
			return FindInternalProfile(deviceName);
		}

		protected GamepadProfile FindInternalProfile(string deviceName)
		{
			if (profiles == null || profiles.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < profiles.Length; i++)
			{
				if (profiles[i] != null && profiles[i].IsCompatible(deviceName))
				{
					return profiles[i];
				}
			}
			return null;
		}

		private static BuiltInGamepadProfileBank Inst()
		{
			if (mInst != null)
			{
				return mInst;
			}
			string text = Application.platform.ToString().ToLower();
			if (text == "webglplayer")
			{
				string text2 = SystemInfo.operatingSystem.ToLower();
				if (text2.Contains("windows"))
				{
					text = "webgl_win";
				}
				else if (text2.Contains("mac os x"))
				{
					text = "webgl_mac";
				}
				else if (text2.Contains("linux"))
				{
					text = "webgl_linux";
				}
				else if (text2.Contains("android"))
				{
					text = "webgl_android";
				}
				else if (text2.Contains("iphone"))
				{
					text = "webgl_ios";
				}
			}
			switch (text)
			{
			case "android":
			case "webgl_android":
				mInst = new BuiltInGamepadProfileBankAndroid();
				break;
			case "osxeditor":
			case "osxplayer":
			case "osxdashboardplayer":
			case "osxwebplayer":
			case "webgl_mac":
				mInst = new BuiltInGamepadProfileBankOSX();
				break;
			case "windowseditor":
			case "windowsplayer":
			case "metroplayerarm":
			case "metroplayerx64":
			case "metroplayerx86":
			case "wsaplayerarm":
			case "wsaplayerx64":
			case "wsaplayerx86":
			case "windowswebplayer":
			case "webgl_win":
				mInst = new BuiltInGamepadProfileBankWin();
				break;
			case "tvos":
			case "iphoneplayer":
			case "webgl_ios":
				mInst = new BuiltInGamepadProfileBankIOS();
				break;
			case "linuxeditor":
			case "linuxplayer":
			case "webgl_linux":
				mInst = new BuiltInGamepadProfileBankLinux();
				break;
			case "stadia":
				mInst = new BuiltInGamepadProfileBankStadia();
				break;
			case "xboxone":
				mInst = new BuiltInGamepadProfileBankXboxOne();
				break;
			case "ps4":
				mInst = new BuiltInGamepadProfileBankPS4();
				break;
			}
			return mInst;
		}
	}
}
