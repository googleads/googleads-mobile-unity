using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GoogleMobileAds.Editor
{
    public class GoogleMobileAdsLanguageWindow : EditorWindow
    {
        private const string LANGUAGE_GUI_CONTENT = "Language";
        private const string SELECT_LANGUAGE_BUTTON_LABEL = "Select Plugin Language";

        // Serialize field on window so its value will be saved when Unity recompiles.
        [SerializeField]
        private string m_UserLocale;

        // Open the window from the menu item defined below.
        [MenuItem("Assets/Google Mobile Ads/Plugin Language")]
        static void Init()
        {
            EditorWindow window = GetWindow<GoogleMobileAdsLanguageWindow>();
            window.position = new Rect(50f, 50f, 200f, 24f);
            window.Show();
        }

        void OnEnable()
        {
            titleContent = new GUIContent(LANGUAGE_GUI_CONTENT);
        }

        // A method to simplify adding menu items.
        void AddMenuItemForLanguage(GenericMenu menu, string menuPath, string locale)
        {
            // The menu item is marked as selected if it matches the current value of m_UserLocale.
            menu.AddItem(new GUIContent(menuPath), m_UserLocale.Equals(locale), OnLanguageSelected, locale);
        }

        // The GenericMenu.MenuFunction2 event handler for when a menu item is selected.
        void OnLanguageSelected(object locale)
        {
            // Persist to GMA settings.
            GoogleMobileAdsSettings.LoadInstance().UserLocale = (string)locale;
        }

        void OnGUI()
        {
            // Display the GenericMenu when pressing the button.
            if (GUILayout.Button(SELECT_LANGUAGE_BUTTON_LABEL))
            {
                // Create the menu.
                GenericMenu menu = new();

                // Populate the menu.
                EditorLocalization localization = new();
                m_UserLocale = GoogleMobileAdsSettings.LoadInstance().UserLocale;
                if (string.IsNullOrEmpty(m_UserLocale))
                {
                    m_UserLocale = localization.GetDefaultLanguage();
                }
                foreach (KeyValuePair<string, string> kvp in localization.GetLanguages())
                {
                    AddMenuItemForLanguage(menu, kvp.Key, kvp.Value);
                }

                // Display the menu.
                menu.ShowAsContext();
            }
        }
    }
}
