using System;
using System.Collections;
using ControlFreak2;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField]
	private Button[] allButtons;

	[SerializeField]
	private TMP_Text[] buttonTexts;

	[SerializeField]
	private TMP_Text[] secondaryButtonTexts;

	[SerializeField]
	private TMP_Text[] labelTexts;

	[Header("UI Panels")]
	[SerializeField]
	private GameObject panel;

	[SerializeField]
	private GameObject confirmationPanel;

	[SerializeField]
	private GameObject resetConfirmationPanel;

	[Header("Counter Text")]
	[SerializeField]
	private TMP_Text counterText;

	[SerializeField]
	private TMP_Text confirmText;

	private int currentActionIndex;

	private Coroutine rebindCoroutine;

	private KeyCode[] allKeyCodes;

	private bool isBindingMouseInput;

	private InputManager inputMan;

	private void Awake()
	{
		allKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
	}

	private void Start()
	{
		inputMan = Singleton<InputManager>.Instance;
		if (inputMan != null)
		{
			UpdateAllButtonTexts();
		}
	}

	private void OnEnable()
	{
		if (inputMan == null)
		{
			inputMan = Singleton<InputManager>.Instance;
		}
		if (inputMan == null)
		{
			Debug.LogError("ControlsMenu: InputManager not found! Is it in the scene?");
			return;
		}
		Debug.Log(string.Format("ControlsMenu: InputManager found. Mappings count: {0}", inputMan.KeyboardMapping.Count));
		UpdateAllButtonTexts();
	}

	private void OnDisable()
	{
		inputMan.Save();
	}

	public void UpdateAllButtonTexts()
	{
		for (int i = 0; i < labelTexts.Length; i++)
		{
			if (i < Enum.GetValues(typeof(InputAction)).Length)
			{
				InputAction key = (InputAction)i;
				InputBinding value;
				if (inputMan.KeyboardMapping.TryGetValue(key, out value))
				{
					buttonTexts[i].text = (IsMouseKey(value.primaryKey) ? FormatMouseText(value.primaryKey) : FormatKeyText(value.primaryKey));
					secondaryButtonTexts[i].text = (IsMouseKey(value.secondaryKey) ? FormatMouseText(value.secondaryKey) : FormatKeyText(value.secondaryKey));
				}
			}
		}
	}

	private string FormatKeyText(KeyCode key)
	{
		switch (key)
		{
		case KeyCode.Escape:
			return "ESC";
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

	private string FormatMouseText(KeyCode mouseKey)
	{
		switch (mouseKey)
		{
		case KeyCode.Mouse0:
			return "Left Mouse Button";
		case KeyCode.Mouse1:
			return "Right Mouse Button";
		case KeyCode.Mouse2:
			return "Middle Mouse Button";
		default:
			return mouseKey.ToString();
		}
	}

	public void StartRebindKeyboard(int actionIndex)
	{
		if (!panel.activeSelf && !confirmationPanel.activeSelf && actionIndex < Enum.GetValues(typeof(InputAction)).Length)
		{
			currentActionIndex = actionIndex;
			isBindingMouseInput = false;
			confirmationPanel.SetActive(true);
			SetButtonsInteractable(false);
			confirmText.text = labelTexts[currentActionIndex].text + "\n\n" + buttonTexts[currentActionIndex].text;
		}
	}

	public void StartRebindMouse(int actionIndex)
	{
		if (!panel.activeSelf && !confirmationPanel.activeSelf && actionIndex < Enum.GetValues(typeof(InputAction)).Length)
		{
			currentActionIndex = actionIndex;
			isBindingMouseInput = true;
			confirmationPanel.SetActive(true);
			SetButtonsInteractable(false);
			confirmText.text = labelTexts[currentActionIndex].text + "\n\n" + secondaryButtonTexts[currentActionIndex].text;
		}
	}

	public void ShowResetConfirmation()
	{
		resetConfirmationPanel.SetActive(true);
		SetButtonsInteractable(false);
	}

	public void ResetKey()
	{
		if (!isBindingMouseInput)
		{
			inputMan.ClearPrimaryKey((InputAction)currentActionIndex);
		}
		else
		{
			inputMan.ClearSecondaryKey((InputAction)currentActionIndex);
		}
		inputMan.Save();
		UpdateAllButtonTexts();
		CancelRebind();
	}

	public void ConfirmRebind()
	{
		confirmationPanel.SetActive(false);
		panel.SetActive(true);
		if (rebindCoroutine != null)
		{
			StopCoroutine(rebindCoroutine);
		}
		rebindCoroutine = StartCoroutine(RebindCoroutine(5f));
	}

	public void ConfirmReset()
	{
		resetConfirmationPanel.SetActive(false);
		ResetToDefaults();
		SetButtonsInteractable(true);
	}

	public void CancelRebind()
	{
		confirmationPanel.SetActive(false);
		if (rebindCoroutine != null)
		{
			StopCoroutine(rebindCoroutine);
		}
		SetButtonsInteractable(true);
	}

	public void CancelReset()
	{
		resetConfirmationPanel.SetActive(false);
		SetButtonsInteractable(true);
	}

	private IEnumerator RebindCoroutine(float duration)
	{
		string actionName = labelTexts[currentActionIndex].text;
		float remainingTime = duration;
		counterText.text = string.Format("<size=16>{0}<size=21>\nPress a key to assign it to {1}\n\n\n\n\n\n{2}", actionName, actionName, Mathf.CeilToInt(remainingTime));
		while (CF2Input.anyKey)
		{
			yield return null;
		}
		yield return null;
		while (remainingTime > 0f)
		{
			counterText.text = string.Format("<size=16>{0}<size=21>\nPress a key to assign it to {1}\n\n\n\n\n\n{2}", actionName, actionName, Mathf.CeilToInt(remainingTime));
			KeyCode pressedKey;
			if (GetPressedInput(out pressedKey))
			{
				if (isBindingMouseInput && IsMouseKey(pressedKey))
				{
					inputMan.Modify((InputAction)currentActionIndex, KeyCode.None, pressedKey);
				}
				else if (!isBindingMouseInput && !IsMouseKey(pressedKey))
				{
					inputMan.Modify((InputAction)currentActionIndex, pressedKey);
				}
				remainingTime = 0.01f;
			}
			remainingTime -= Time.unscaledDeltaTime;
			yield return null;
		}
		panel.SetActive(false);
		rebindCoroutine = null;
		isBindingMouseInput = false;
		SetButtonsInteractable(true);
		inputMan.Save();
		UpdateAllButtonTexts();
	}

	private bool GetPressedInput(out KeyCode pressedKey)
	{
		pressedKey = KeyCode.None;
		if (isBindingMouseInput)
		{
			for (int i = 0; i < 7; i++)
			{
				if (CF2Input.GetMouseButtonDown(i))
				{
					pressedKey = (KeyCode)(323 + i);
					return true;
				}
			}
		}
		else if (CF2Input.anyKeyDown)
		{
			KeyCode[] array = allKeyCodes;
			foreach (KeyCode keyCode in array)
			{
				if (CF2Input.GetKeyDown(keyCode) && !IsMouseKey(keyCode))
				{
					pressedKey = keyCode;
					return true;
				}
			}
		}
		return false;
	}

	private bool IsMouseKey(KeyCode key)
	{
		if (key != KeyCode.Mouse0 && key != KeyCode.Mouse1 && key != KeyCode.Mouse2 && key != KeyCode.Mouse3)
		{
			return key == KeyCode.Mouse4;
		}
		return true;
	}

	private void SetButtonsInteractable(bool state)
	{
		Button[] array = allButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].interactable = state;
		}
	}

	public void ResetToDefaults()
	{
		SetButtonsInteractable(true);
		inputMan.SetDefaults();
		UpdateAllButtonTexts();
	}
}
