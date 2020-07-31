using UnityEngine;
using System;

public class ButtonBehaviour : MonoBehaviour
{
    public event EventHandler<EventArgs> OnAdOpening;
    public event EventHandler<EventArgs> OnLeavingApplication;

    public void OpenURL()
    {
        Debug.Log("Opened URL");
        Application.OpenURL("http://google.com");
       
        OnAdOpening?.Invoke(this, new EventArgs());
        OnLeavingApplication?.Invoke(this, new EventArgs());
    }
}
