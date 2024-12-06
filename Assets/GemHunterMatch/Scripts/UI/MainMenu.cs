using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Match3
{
    public class MainMenu : MonoBehaviour
    {
        private UIDocument m_Document;
        private VisualElement m_Cover;

        private SettingsMenuHandler settingsMenuHandler;

        private Action m_FadeCallback;
    
        void Start()
        {
            GameManager.Instance.MainMenuOpened();

            m_Document = GetComponent<UIDocument>();
            UIHandler.ApplySafeArea(m_Document.rootVisualElement);

            // Query the open settings button
            var openSettingButton = m_Document.rootVisualElement.Q<Button>("ButtonMenu");

            // Initialize Settings Menu Handler
            settingsMenuHandler = new SettingsMenuHandler(
                m_Document.rootVisualElement,
                openSettingButton,
                OnReturnButtonClicked
            );

            // Query the map button
            var mapMenuButton = m_Document.rootVisualElement.Q<Button>("MapMenu");

            // Set up click event handler for the map button
            mapMenuButton.clicked += OnMapButtonClicked;

            m_Cover = m_Document.rootVisualElement.Q<VisualElement>("Cover");
            m_Cover.style.opacity = 1.0f;
            m_Cover.RegisterCallback<TransitionEndEvent>(evt =>
            {
                if (m_FadeCallback != null)
                {
                    m_FadeCallback();
                    m_FadeCallback = null;
                }
            });
                
            StartCoroutine(FadeIn());
        }

        private void OnReturnButtonClicked()
        {
            // Navigate back to the main scene or perform desired action
            FadeOut(() =>
            {
                SceneManager.LoadScene(1, LoadSceneMode.Single);
            });
        }

        private void OnMapButtonClicked()
        {
            // Navigate to the map scene when the map button is clicked
            FadeOut(() =>
            {
                SceneManager.LoadScene(5, LoadSceneMode.Single); // Replace '5' with the actual build index of the map scene
            });
        }

        void FadeOut(Action onFadeFinished = null)
        {
            m_FadeCallback = onFadeFinished;
            m_Cover.style.opacity = 1.0f;
        }

        IEnumerator FadeIn()
        {
            yield return null;
            yield return null;

            m_Cover.style.opacity = 0.0f;
        }

        void OnDestroy()
        {
            settingsMenuHandler?.Cleanup();
        }
    }
}