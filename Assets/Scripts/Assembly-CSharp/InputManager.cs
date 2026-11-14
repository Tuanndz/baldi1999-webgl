using System.Collections.Generic;
using ControlFreak2;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
	public Dictionary<InputAction, InputBinding> KeyboardMapping = new Dictionary<InputAction, InputBinding>();

	private Dictionary<KeyCode, SimulatedKeyState> simulatedKeys = new Dictionary<KeyCode, SimulatedKeyState>();

	private bool[] currentKeyStates = new bool[13];

	private bool[] previousKeyStates = new bool[13];

	private const string SavePrefix = "InputBinding_";

	public Dictionary<InputAction, InputBinding> Mappings
	{
		get
		{
			if (KeyboardMapping == null || KeyboardMapping.Count == 0)
			{
				Load();
			}
			return KeyboardMapping;
		}
	}

	protected override void Awake()
	{
		if (Singleton<InputManager>.Instance != null && Singleton<InputManager>.Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		base.Awake();
		Object.DontDestroyOnLoad(base.gameObject);
		Load();
	}

	private void Update()
	{
		bool[] array = previousKeyStates;
		previousKeyStates = currentKeyStates;
		currentKeyStates = array;
		UpdateSimulatedKeys();
		for (int i = 0; i < 13; i++)
		{
			InputBinding value;
			if (KeyboardMapping.TryGetValue((InputAction)i, out value))
			{
				bool flag = (value.primaryKey != KeyCode.None && GetKeyState(value.primaryKey)) || (value.secondaryKey != KeyCode.None && GetKeyState(value.secondaryKey));
				currentKeyStates[i] = flag;
			}
		}
	}

	private void UpdateSimulatedKeys()
	{
		if (simulatedKeys.Count == 0)
		{
			return;
		}
		foreach (KeyCode item in new List<KeyCode>(simulatedKeys.Keys))
		{
			SimulatedKeyState value = simulatedKeys[item];
			value.previousFrame = value.currentFrame;
			value.currentFrame = false;
			simulatedKeys[item] = value;
			if (!value.currentFrame && !value.previousFrame)
			{
				simulatedKeys.Remove(item);
			}
		}
	}

	public void SimulateKey(KeyCode key)
	{
		SimulatedKeyState value;
		if (!simulatedKeys.TryGetValue(key, out value))
		{
			value = default(SimulatedKeyState);
		}
		value.currentFrame = true;
		simulatedKeys[key] = value;
	}

	private bool GetKeyState(KeyCode key)
	{
		SimulatedKeyState value;
		if (simulatedKeys.TryGetValue(key, out value) && value.currentFrame)
		{
			return true;
		}
		return CF2Input.GetKey(key);
	}

	public bool GetActionKey(InputAction action)
	{
		return currentKeyStates[(int)action];
	}

	public bool GetActionKeyDown(InputAction action)
	{
		if (currentKeyStates[(int)action])
		{
			return !previousKeyStates[(int)action];
		}
		return false;
	}

	public bool GetActionKeyUp(InputAction action)
	{
		if (!currentKeyStates[(int)action])
		{
			return previousKeyStates[(int)action];
		}
		return false;
	}

	public void ClearPrimaryKey(InputAction action)
	{
		InputBinding value;
		if (KeyboardMapping.TryGetValue(action, out value))
		{
			value.primaryKey = KeyCode.None;
			KeyboardMapping[action] = value;
		}
	}

	public void ClearSecondaryKey(InputAction action)
	{
		InputBinding value;
		if (KeyboardMapping.TryGetValue(action, out value))
		{
			value.secondaryKey = KeyCode.None;
			KeyboardMapping[action] = value;
		}
	}

	public string ConvertInputActionToString(InputAction action)
	{
		InputBinding value;
		if (!KeyboardMapping.TryGetValue(action, out value))
		{
			return action.ToString();
		}
		if (value.primaryKey != KeyCode.None)
		{
			return KeyCodeToDisplayString(value.primaryKey);
		}
		if (value.secondaryKey != KeyCode.None)
		{
			return KeyCodeToDisplayString(value.secondaryKey);
		}
		return action.ToString();
	}

	public void Modify(InputAction action, KeyCode newKey, KeyCode secondaryKey = KeyCode.None)
	{
		InputBinding value;
		if (!KeyboardMapping.TryGetValue(action, out value))
		{
			value = default(InputBinding);
		}
		if (newKey != KeyCode.None)
		{
			value.primaryKey = newKey;
		}
		if (secondaryKey != KeyCode.None)
		{
			value.secondaryKey = secondaryKey;
		}
		KeyboardMapping[action] = value;
	}

	public void Save()
	{
		foreach (KeyValuePair<InputAction, InputBinding> item in KeyboardMapping)
		{
			string key = "InputBinding_" + item.Key;
			string value = string.Format("{0},{1}", (int)item.Value.primaryKey, (int)item.Value.secondaryKey);
			PlayerPrefs.SetString(key, value);
		}
		PlayerPrefs.Save();
	}

	public void Load()
	{
		KeyboardMapping.Clear();
		for (int i = 0; i < 13; i++)
		{
			InputAction inputAction = (InputAction)i;
			string key = "InputBinding_" + inputAction;
			if (PlayerPrefs.HasKey(key))
			{
				string[] array = PlayerPrefs.GetString(key).Split(',');
				int result;
				int result2;
				if (array.Length == 2 && int.TryParse(array[0], out result) && int.TryParse(array[1], out result2))
				{
					KeyboardMapping[inputAction] = new InputBinding((KeyCode)result, (KeyCode)result2);
					continue;
				}
			}
			KeyboardMapping[inputAction] = GetDefaultBinding(inputAction);
		}
	}

	public void SetDefaults()
	{
		KeyboardMapping.Clear();
		for (int i = 0; i < 13; i++)
		{
			InputAction inputAction = (InputAction)i;
			KeyboardMapping[inputAction] = GetDefaultBinding(inputAction);
		}
		Save();
	}

	public string KeyCodeToDisplayString(KeyCode key)
	{
		switch (key)
		{
		case KeyCode.Escape:
			return "ESC";
		case KeyCode.Mouse0:
			return "Left Mouse Button";
		case KeyCode.Mouse1:
			return "Right Mouse Button";
		case KeyCode.Mouse2:
			return "Middle Mouse Button";
		case KeyCode.Alpha0:
			return "0";
		case KeyCode.Alpha1:
			return "1";
		case KeyCode.Alpha2:
			return "2";
		case KeyCode.Alpha3:
			return "3";
		case KeyCode.Alpha4:
			return "4";
		case KeyCode.Alpha5:
			return "5";
		case KeyCode.Alpha6:
			return "6";
		case KeyCode.Alpha7:
			return "7";
		case KeyCode.Alpha8:
			return "8";
		case KeyCode.Alpha9:
			return "9";
		case KeyCode.None:
			return "";
		default:
			return key.ToString();
		}
	}

	private InputBinding GetDefaultBinding(InputAction action)
	{
		switch (action)
		{
		case InputAction.MoveLeft:
			return new InputBinding(KeyCode.A);
		case InputAction.MoveRight:
			return new InputBinding(KeyCode.D);
		case InputAction.MoveForward:
			return new InputBinding(KeyCode.W);
		case InputAction.MoveBackward:
			return new InputBinding(KeyCode.S);
		case InputAction.Interact:
			return new InputBinding(KeyCode.None, KeyCode.Mouse0);
		case InputAction.UseItem:
			return new InputBinding(KeyCode.None, KeyCode.Mouse1);
		case InputAction.Slot0:
			return new InputBinding(KeyCode.Alpha1);
		case InputAction.Slot1:
			return new InputBinding(KeyCode.Alpha2);
		case InputAction.Slot2:
			return new InputBinding(KeyCode.Alpha3);
		case InputAction.Run:
			return new InputBinding(KeyCode.LeftShift);
		case InputAction.LookBehind:
			return new InputBinding(KeyCode.Space);
		case InputAction.Jump:
			return new InputBinding(KeyCode.Space);
		case InputAction.PauseOrCancel:
			return new InputBinding(KeyCode.Escape);
		default:
			return new InputBinding(KeyCode.None);
		}
	}
}
