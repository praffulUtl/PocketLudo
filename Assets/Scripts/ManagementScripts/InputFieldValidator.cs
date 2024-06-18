using UnityEngine;
using TMPro;

public class InputFieldValidator : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        // Ensure the input field only accepts numbers and has a character limit of 4
        inputField.characterLimit = 4;
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;

        // Add listener for value change to validate input
        inputField.onValueChanged.AddListener(delegate { ValidateInput(); });
    }

    void ValidateInput()
    {
        // Remove any non-numeric characters
        string validText = "";
        foreach (char c in inputField.text)
        {
            if (char.IsDigit(c))
            {
                validText += c;
            }
        }

        // Update the input field text with the valid characters only
        if (inputField.text != validText)
        {
            inputField.text = validText;
        }
    }
}
