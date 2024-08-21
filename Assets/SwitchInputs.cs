using UnityEngine;
using AC;

public class SwitchInputs : MonoBehaviour
{

	[SerializeField] private InputMethod defaultInputMethod;
	[SerializeField] private string enableJoystickInputName = "JoystickButton";

	private void Start ()
	{
		SetInputMethod (defaultInputMethod, true);
	}

	private void Update ()
	{
		if (Input.anyKeyDown)
		{
			if (Input.GetButtonDown (enableJoystickInputName))
			{
				SetInputMethod (InputMethod.KeyboardOrController);
			}
			else
			{
				SetInputMethod (InputMethod.MouseAndKeyboard);
			}
		}
	}

	private void SetInputMethod (InputMethod inputMethod, bool force = false)
	{
		if (KickStarter.settingsManager.inputMethod != inputMethod || force)
		{
			KickStarter.settingsManager.inputMethod = inputMethod;

			switch (inputMethod)
			{
				case InputMethod.MouseAndKeyboard:
					KickStarter.menuManager.keyboardControlWhenCutscene = KickStarter.menuManager.keyboardControlWhenPaused = KickStarter.menuManager.keyboardControlWhenDialogOptions = false;
                    // Set any mouse/keyboard-specific settings here
					break;

				case InputMethod.KeyboardOrController:
					KickStarter.menuManager.keyboardControlWhenCutscene = KickStarter.menuManager.keyboardControlWhenPaused = KickStarter.menuManager.keyboardControlWhenDialogOptions = true;
					KickStarter.playerMenus.FindFirstSelectedElement ();
                    // Set any keyboard/controller-specific settings here
					break;

				default:
					break;
			}
		}
	}

}