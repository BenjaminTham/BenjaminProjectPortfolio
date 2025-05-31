using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MentalColour : MonoBehaviour
{
    public Color wantedColor;
    public Color physicalColor;
    public Button button;
    public Button physical;
    // Start is called before the first frame update
    void Start()
    {
        ColorBlock cb = button.colors;
        cb.normalColor = wantedColor;
        button.colors = cb;
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
        ColorBlock cb1 = physical.colors;
        cb1.normalColor = physicalColor;
        physical.colors = cb1;
        }
}
