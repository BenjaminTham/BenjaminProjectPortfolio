using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    public GameObject[] Background;
    public GameObject PrevArrow;
    public GameObject NextArrow;
    public GameObject NextScene;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (index >= Background.Length-1)
            index = Background.Length-1;

        if (index < 0)
            index = 0;

        if (index == 0)
        {
            Background[0].gameObject.SetActive(true);
            PrevArrow.gameObject.SetActive(false);
            NextScene.gameObject.SetActive(false);
        }
        else if (index == Background.Length - 1)
        {
            NextArrow.gameObject.SetActive(false) ;
            NextScene.gameObject.SetActive(true);
        }
        else
        {
            PrevArrow.gameObject.SetActive(true);
            NextArrow.gameObject.SetActive(true);
        }
    }
    public void Next()
    {
        index += 1;

        for (int i = 0; i < Background.Length; i++)
        {
            Background[i].gameObject.SetActive(false);
            Background[index].gameObject.SetActive(true);
        }
        Debug.Log(index);
    }

    public void Previous()
    {
        index -= 1;

        for (int i = 0; i < Background.Length; i++)
        {
            Background[i].gameObject.SetActive(false);
            Background[index].gameObject.SetActive(true);
        }
        Debug.Log(index);
    }

    public void NextStep()
    {
        if (index < 3)
        {
            NextScene.gameObject.SetActive(false);
        }
        Debug.Log(index);
    }



}
