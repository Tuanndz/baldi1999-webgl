namespace ControlFreak2
{
	public class BuiltInGamepadProfileBankStadia : BuiltInGamepadProfileBank
	{
		public BuiltInGamepadProfileBankStadia()
		{
			profiles = new GamepadProfile[1]
			{
				new GamepadProfile("Stadia", "Stadia", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, true, 1, false), GamepadProfile.JoystickSource.Axes(3, true, 4, false), GamepadProfile.JoystickSource.Axes(5, true, 6, true), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(2), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.PlusAxis(8), GamepadProfile.KeySource.PlusAxis(9), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(9))
			};
			genericProfile = new GamepadProfile.GenericProfile(GamepadProfile.JoystickSource.Axes(0, true, 1, false), GamepadProfile.JoystickSource.Axes(3, true, 4, false), GamepadProfile.JoystickSource.Axes(5, true, 6, true), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(2), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.PlusAxis(8), GamepadProfile.KeySource.PlusAxis(9), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(9));
		}
	}
}
