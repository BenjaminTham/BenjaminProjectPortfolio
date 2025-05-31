using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Include this if you're dealing with UI elements

public class UIDelayAppearance : MonoBehaviour
{
    public GameObject uiElement; // Assign your UI element here
    public GameObject uiElement1; // Assign your UI element here
    public GameObject uiElement2; // Assign your UI element here
    public GameObject uiElement3; // Assign your UI element here
    public GameObject uiElement4; // Assign your UI element here
    public GameObject uiElement5; // Assign your UI element here
    public GameObject uiElement6; // Assign your UI element here
    public float delay = 3f; // Time in seconds to wait before showing the UI

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowAfterDelay(uiElement, delay));
        StartCoroutine(ShowAfterDelay(uiElement1, delay));
        StartCoroutine(ShowAfterDelay(uiElement2, delay));
        StartCoroutine(ShowAfterDelay(uiElement3, delay));
        StartCoroutine(HideAfterDelay(uiElement4, delay));
        StartCoroutine(ShowAfterDelay(uiElement5, delay));

        StartCoroutine(HideAfterDelay(uiElement1, delay+6f));
        StartCoroutine(HideAfterDelay(uiElement2, delay+6f));
        StartCoroutine(HideAfterDelay(uiElement3, delay + 6f));
        StartCoroutine(ShowAfterDelay(uiElement4, delay+6f));
        StartCoroutine(HideAfterDelay(uiElement5, delay + 6f));

        StartCoroutine(ShowAfterDelay(uiElement6, delay + 18f));
    }

    private IEnumerator ShowAfterDelay(GameObject element, float delay)
    {
        yield return new WaitForSeconds(delay);
        element.SetActive(true);
        // If you're fading in an image or text, you would change its alpha here
    }

    private IEnumerator HideAfterDelay(GameObject element, float delay)
    {
        yield return new WaitForSeconds(delay);
        element.SetActive(false);
        // If you're fading in an image or text, you would change its alpha here
    }
}
