using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextControl : MonoBehaviour
{
    public TextMeshProUGUI obj_text; // TextMeshPro Text
    public TMP_InputField display; // TextMeshPro InputField
    public TextMeshProUGUI errorText; // TextMeshPro Text for error messages
    public GameObject confirmButton; // Reference to the confirm button
    public GameObject editButton; // Reference to the edit button

    private string originalText;
    private bool isEmailValid = false;

    void Start()
    {
        // Get the original text from the obj_text component
        originalText = obj_text.text;

        // Attach the onEndEdit event handler
        display.onEndEdit.AddListener(OnEndEdit);

        // Attach the confirm button's click event to the custom method
        confirmButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ConfirmInput);

        // Attach the edit button's click event to the custom method
        editButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(EditInput);

        
    }

    public void Create()
    {
        ConfirmInput();
    }

    public void ConfirmInput()
    {
        string newText = display.text;

        // Check if the text is a valid email
        if (!IsValidEmail(newText))
        {
            DisplayError("Invalid email address");
            isEmailValid = false;
            return;
        }

        // Check if the text is less than 5 characters
        if (newText.Length < 5)
        {
            DisplayError("Text must be at least 5 characters long");
            return;
        }

        // Email is valid, hide the input field
        display.gameObject.SetActive(false);

        // Unhide the obj_text
        obj_text.gameObject.SetActive(true);

        // Hide the confirm button
        confirmButton.SetActive(false);

        // Show the edit button
        editButton.SetActive(true);

        obj_text.text = newText;
        errorText.text = string.Empty; // Clear error message
    }

    public void EditInput()
    {
        // Show the input field for editing
        display.gameObject.SetActive(true);

        // Hide the obj_text
        obj_text.gameObject.SetActive(false);

        // Show the confirm button
        confirmButton.SetActive(true);

        // Hide the edit button
        editButton.SetActive(false);
    }

    private void DisplayError(string errorMessage)
    {
        errorText.text = errorMessage;
    }

    bool IsValidEmail(string email)
    {
        // You can implement a more robust email validation logic here
        bool valid = email.Contains("@") && email.Contains(".");
        if (valid) {
            isEmailValid = true;
        }
        return valid;
    }

    void OnEndEdit(string text)
    {
        // Check for errors when the user finishes editing
        if (!string.IsNullOrEmpty(errorText.text))
        {
            // Display error message while keeping the input field active
        } else if (isEmailValid) {
            // Hide the input field
            display.gameObject.SetActive(false);

            // Unhide the obj_text
            obj_text.gameObject.SetActive(true);

            // Hide the confirm button
            confirmButton.SetActive(false);

            // Show the edit button
            editButton.SetActive(true);
        }
    }

    void Update()
    {
        // Your Update code here
    }
}
