// Copyright (C) 2015 Google, Inc.
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

using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;
using UnityEngine;

/// <summary>
/// Ads controller class to request and display reward based video ads.
/// </summary>
public class AdsController : MonoBehaviour
{
    /// A test ad unit for custom native ads.
    public const string AdUnitId = "/6499/example/unity-custom-native";
    public const string FormatId = "10085730";
    public Material GroundTextMaterial;
    public Material ErrorTextMaterial;
    public Font TextFont;

    private bool nativeAdLoaded;
    private CustomNativeAd nativeAd;
    private GameObject errorMessage1;
    private GameObject errorMessage2;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    public void Update()
    {
        if (this.nativeAdLoaded)
        {
            if (this.errorMessage1 != null)
            {
                MonoBehaviour.Destroy(this.errorMessage1);
            }

            if (this.errorMessage2 != null)
            {
                MonoBehaviour.Destroy(this.errorMessage1);
            }

            Texture2D billboardTexture1 = this.nativeAd.GetTexture2D("Image1");
            Texture2D billboardTexture2 = this.nativeAd.GetTexture2D("Image2");

            for (int i = 1; i <= 6; i++)
            {
                if (i % 2 == 0)
                {
                    GameObject.Find("Billboard" + i.ToString())
                            .GetComponent<Renderer>()
                            .material
                            .mainTexture = billboardTexture1;
                }
                else
                {
                    GameObject.Find("Billboard" + i.ToString())
                            .GetComponent<Renderer>()
                            .material
                            .mainTexture = billboardTexture2;
                }
            }

            GameObject textObject = new GameObject("GroundText");
            GameObject ground = GameObject.Find("Ground");
            textObject.transform.parent = ground.transform;
            textObject.transform.position = new Vector3(0, 0.1f, 0);
            textObject.AddComponent<TextMesh>();

            TextMesh textMeshComponent = textObject.GetComponent<TextMesh>();
            MeshRenderer meshRendererComponent = textObject.GetComponent<MeshRenderer>();

            string adText = this.nativeAd.GetText("MainText").Replace('-', '\n');

            textMeshComponent.text = adText;
            textMeshComponent.fontSize = 8;
            textMeshComponent.anchor = TextAnchor.MiddleCenter;
            textMeshComponent.transform.Rotate(new Vector3(90, 0, 0));
            textMeshComponent.font = this.TextFont;
            meshRendererComponent.material = this.GroundTextMaterial;

            this.nativeAdLoaded = false;
        }
    }

    /// <summary>
    /// Creates a new GameObject with a TextMesh component to display an error message.
    /// </summary>
    /// <returns>The GameObject with a TextMesh component.</returns>
    /// <param name="errorText">Error text to display.</param>
    /// <param name="offset">Local position offset from parent GameObject</param>
    /// <param name="parentObject">Parent GameObject for error text.</param>
    private GameObject CreateErrorTextOnGameObject(
            string errorText,
            GameObject parentObject,
            Vector3 offset)
    {
        GameObject errorTextObject = new GameObject("ErrorText1");

        errorTextObject.transform.parent = parentObject.transform;
        errorTextObject.transform.localPosition = offset;
        errorTextObject.AddComponent<TextMesh>();

        TextMesh textMeshComponent = errorTextObject.GetComponent<TextMesh>();
        MeshRenderer meshRendererComponent = errorTextObject.GetComponent<MeshRenderer>();

        textMeshComponent.text = errorText;
        textMeshComponent.fontSize = 8;
        textMeshComponent.anchor = TextAnchor.MiddleCenter;
        textMeshComponent.font = this.TextFont;
        meshRendererComponent.material = this.ErrorTextMaterial;

        return errorTextObject;
    }

    /// <summary>
    /// Called by MobileAds when initialization is completed.
    /// </summary>
    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        MonoBehaviour.print("Initialization complete");
        this.nativeAdLoaded = false;
        this.RequestNativeAd();
    }

    /// <summary>
    /// Requests a CustomNativeAd.
    /// </summary>
    private void RequestNativeAd()
    {
        AdLoader adLoader = new AdLoader.Builder(AdUnitId)
            .ForCustomNativeAd(FormatId)
            .Build();
        adLoader.OnCustomNativeAdLoaded += this.HandleCustomNativeAdLoaded;
        adLoader.OnAdFailedToLoad += this.HandleNativeAdFailedToLoad;
        adLoader.LoadAd(new AdRequest.Builder().Build());
    }

    /// <summary>
    /// Handles the ad event corresponding to a CustomNativeAd successfully loading.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">EventArgs wrapper for CustomNativeAd that loaded.</param>
    private void HandleCustomNativeAdLoaded(object sender, CustomNativeAdEventArgs args)
    {
        this.nativeAd = args.nativeAd;
        this.nativeAdLoaded = true;
    }

    /// <summary>
    /// Handles the native ad failing to load.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Error information.</param>
    private void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Vector3 errorTextOffset = new Vector3(0, 0, -0.25f);

        if (this.errorMessage1 == null)
        {
            this.errorMessage1 = this.CreateErrorTextOnGameObject(
                    "Ad failed to load",
                    GameObject.Find("Billboard1"),
                    errorTextOffset);
        }

        if (this.errorMessage2 == null)
        {
            this.errorMessage2 = this.CreateErrorTextOnGameObject(
                    "Ad failed to load",
                    GameObject.Find("Billboard2"),
                    errorTextOffset);
        }

        string message = args.LoadAdError.GetMessage();
        MonoBehaviour.print("Ad Loader fail event received with message: " + message);
    }
}
