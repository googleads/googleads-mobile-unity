using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{

    private float currentTime = 0f;
    private float startingTime = 5f;
    private Text[] countdownText;
    private Button[] buttons;

    public void Start()
    {
        countdownText = this.GetComponentsInChildren<Text>();
        buttons = this.GetComponentsInChildren<Button>();
        buttons[1].gameObject.SetActive(false);
        currentTime = startingTime;
    }

    // Update is called once per frame
    public void Update()
    {
        currentTime -= Time.unscaledDeltaTime;
        countdownText[1].text = Mathf.Round(currentTime).ToString() + " seconds remaining";

        if (currentTime <= 0)
        {
            currentTime = 0;
            countdownText[1].enabled = false;
            buttons[1].gameObject.SetActive(true);
        }
    }
}
