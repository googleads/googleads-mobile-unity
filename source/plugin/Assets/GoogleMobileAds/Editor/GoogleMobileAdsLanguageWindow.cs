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
    private string m_UserLanguage;

    // Open the window from the menu item defined below.
    // TODO: b/364890345 - We decided to add static Menu Items instead with each one dedicated to
    // setting a language.
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
    void AddMenuItemForLanguage(GenericMenu menu, string menuPath, string language)
    {
      // The menu item is marked as selected if it matches the current value of m_UserLanguage.
      menu.AddItem(new GUIContent(menuPath), m_UserLanguage.Equals(language), OnLanguageSelected,
                   language);
    }

    // The GenericMenu.MenuFunction2 event handler for when a menu item is selected.
    void OnLanguageSelected(object language)
    {
      // Persist to GMA settings.
      GoogleMobileAdsSettings.LoadInstance().UserLanguage = (string)language;
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
        m_UserLanguage = GoogleMobileAdsSettings.LoadInstance().UserLanguage;
        if (string.IsNullOrEmpty(m_UserLanguage))
        {
          m_UserLanguage = localization.GetDefaultLanguage();
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
