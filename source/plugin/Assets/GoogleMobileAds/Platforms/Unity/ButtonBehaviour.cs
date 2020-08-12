using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonBehaviour : MonoBehaviour
{
    public event EventHandler<EventArgs> OnAdOpening;
    public event EventHandler<EventArgs> OnLeavingApplication;
   
    public void OpenURL()
    {
        Debug.Log("Opened URL");
        Application.OpenURL("http://google.com");
        if (OnAdOpening != null)
        {
            OnAdOpening.Invoke(this, new EventArgs());
        }
        if (OnLeavingApplication != null)
        {
            OnLeavingApplication.Invoke(this, new EventArgs());
        }
    }
}
