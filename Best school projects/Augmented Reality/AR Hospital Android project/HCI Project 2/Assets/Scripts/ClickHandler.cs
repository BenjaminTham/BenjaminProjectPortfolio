using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // We need this namespace to work with Event Systems
using TMPro; // Namespace for TextMesh Pro
using UnityEngine.Events;

public class ClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent upEvent;
    public UnityEvent downEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("down");
        downEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("up");
        upEvent?.Invoke();
    }
}
