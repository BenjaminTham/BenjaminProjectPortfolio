using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhysicalColour : MonoBehaviour
{
    public Color wantedColor;
    public Color mentalColor;
    public Button button;
    public Button mental;
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
        cb.pressedColor = wantedColor;
        button.colors = cb;
        ColorBlock cb1 = mental.colors;
        cb1.normalColor = mentalColor;
        mental.colors = cb1;
        }
}
