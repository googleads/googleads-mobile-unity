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
    /// Button for loading a scene.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/Utility/SceneMenuButton")]
    public class SceneMenuButton : Button
    {
        [Tooltip("Name of the scene to load.")]
        public string SceneToLoadName = "GoogleMobileAdsScene";

        [Tooltip("Text element for the label.")]
        public Text Label;

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
    [CustomEditor(typeof(SceneMenuButton))]
    public class SceneMenuButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            SceneMenuButton targetMyButton = target as SceneMenuButton;

            var sceneToLoadName = EditorGUILayout.TextField(
                "Scene To Load",
                targetMyButton.SceneToLoadName);

            // Equality check to prevent marking GameObject as dirty when it is not.
            if (sceneToLoadName != targetMyButton.SceneToLoadName)
            {
                targetMyButton.SceneToLoadName = sceneToLoadName;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Label"),
                                            new GUIContent("Label"));
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
#endif
}