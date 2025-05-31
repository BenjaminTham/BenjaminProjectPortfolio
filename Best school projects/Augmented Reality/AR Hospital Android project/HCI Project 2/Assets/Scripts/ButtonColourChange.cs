using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColourChange : MonoBehaviour
{
    public Color wantedColor;
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void ChangeButtonColour()
        {
        ColorBlock cb = button.colors;
        cb.selectedColor = wantedColor;
        button.colors = cb; 
        }
}
