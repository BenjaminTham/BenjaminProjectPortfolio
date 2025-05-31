using UnityEngine;
using UnityEngine.UI; // Required when dealing with UI

public class HideUIElements : MonoBehaviour
{
    public GameObject pic;

    public void Trigger()
    {
        if (pic.activeInHierarchy == false)
        {
            pic.SetActive(true);

        }
        else
        {
            pic.SetActive(false);
        }
    }
}
