using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchScript : MonoBehaviour
{
    public GameObject ContentHolder;
    public GameObject[] Element;
    public TMP_InputField SearchBar;
    public int totalElements;

    // Start is called before the first frame update
    void Start()
    {
        totalElements = ContentHolder.transform.childCount;
        Element = new GameObject[totalElements];

        for (int i = 0; i < totalElements; i++)
        {
            Element[i] = ContentHolder.transform.GetChild(i).gameObject;
        }
    }

    public void Search()
    {
        string searchText = SearchBar.text.ToLower();

        foreach (GameObject ele in Element)
        {
            Transform childOfContentHolder = ele.transform.GetChild(0); // Access the child of ContentHolder
            TextMeshProUGUI buttonText = childOfContentHolder.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                string buttonLabel = buttonText.text.ToLower();

                // Check if the search input exists at the beginning of the button's text
                bool showButton = buttonLabel.StartsWith(searchText);
                ele.SetActive(showButton);
            }
            else
            {
                // If the TextMeshProUGUI component is not found, you might want to handle this case
                Debug.LogWarning("TextMeshProUGUI component not found in child.");
                ele.SetActive(false);
            }
        }
    }
}
