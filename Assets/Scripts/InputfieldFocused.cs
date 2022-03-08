using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputfieldFocused : MonoBehaviour
{
    private InputfieldSlideScreen slideScreen;
    private InputField inputField;
    public InputField next;

    private void Start()
    {
        slideScreen = GameObject.Find("MainObject").GetComponent<InputfieldSlideScreen>();
        inputField = transform.GetComponent<InputField>();
        inputField.shouldHideMobileInput = true;
    }

    private void Update()
    {
        if (inputField != null && inputField.isFocused)
        {
            slideScreen.InputFieldActive = true;
            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputField.DeactivateInputField();
                if (next != null)
                {
                    EventSystem.current.SetSelectedGameObject(next.gameObject, null);
                }
            }
        }
    }
}
