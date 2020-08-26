// Copyright (C) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

/** This class is responsible for the countdown timer shown on a rewarded ad.
    After 5 seconds has elapsed, the close ad button will be set active and the user can then
    close the ad. **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{

    private float currentTime = 0f;
    private float startingTime = 5f;
    private Button closeButton;
    private Text countdownText;

    public void Start()
    {
        Text[] texts = this.GetComponentsInChildren<Text>();
        Button[] buttons = this.GetComponentsInChildren<Button>();

        if (texts.Length < 2 || buttons.Length < 2)
        {
            Debug.Log("Invalid Prefab");
            return;
        }

        closeButton = buttons[1];
        countdownText = texts[1];
        closeButton.gameObject.SetActive(false);
        currentTime = startingTime;
    }

    // Update is called once per frame
    public void Update()
    {
        if (countdownText == null || closeButton == null)
        {
            return;
        }

        if (currentTime <= 0)
        {
            return;
        }

        currentTime -= Time.unscaledDeltaTime;
        if (currentTime > 0)
        {
            countdownText.text = Mathf.Round(currentTime).ToString() + " second(s) remaining";
        }
        else
        {
            countdownText.enabled = false;
            closeButton.gameObject.SetActive(true);
        }
    }
}
