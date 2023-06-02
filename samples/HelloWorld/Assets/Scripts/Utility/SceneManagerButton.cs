using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace GoogleMobileAds.Samples.Utility
{
    /// <summary>
    /// Button with scene loading functionality.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/Utility/SceneManagerButton")]
    public class SceneManagerButton : Button
    {
        [Tooltip("Name of the scene to load.")]
        public string SceneToLoadName = "GoogleMobileAdsScene";

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (enabled)
            {
                SceneManager.LoadScene(SceneToLoadName);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SceneManagerButton))]
    public class SceneManagerButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            SceneManagerButton targetMyButton = target as SceneManagerButton;

            var sceneToLoadName = EditorGUILayout.TextField(
                "Scene To Load",
                targetMyButton.SceneToLoadName);

            // Equality check to prevent marking GameObject as dirty when it is not.
            if (sceneToLoadName != targetMyButton.SceneToLoadName)
            {
                targetMyButton.SceneToLoadName = sceneToLoadName;
            }

            base.OnInspectorGUI();
        }
    }
#endif
}