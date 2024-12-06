using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Match3
{
    public class MapMenu : MonoBehaviour
    {
        public LevelList LevelList;
        public VisualTreeAsset LevelEntry;

        private UIDocument m_Document;
        private VisualElement m_Cover;
        private SettingsMenuHandler settingsMenuHandler;
        private int m_TargetLevel = -1;
        private Action m_FadeCallback;

        void Start()
        {
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

            var container = m_Document.rootVisualElement.Q<VisualElement>("LevelSelectionContainer");

            for (var i = 0; i < LevelList.SceneCount; ++i)
            {
#if UNITY_EDITOR
                if (LevelList.Scenes[i] == null)
                {
                    Debug.LogWarning("LevelList contains a null scene! Fix or remove the scene from the LevelList");
                    continue;
                }
#endif
                var newEntry = LevelEntry.Instantiate();
                var label = newEntry.Q<Label>("LevelNumber");
                label.text = (i + 1).ToString();

                container.Add(newEntry);

                var subEntry = newEntry.Q<VisualElement>("LevelEntry");
                var i1 = i;
                subEntry.AddManipulator(new Clickable(() =>
                {
                    m_TargetLevel = i1;
                    FadeOut();
                }));
            }

            m_Cover = m_Document.rootVisualElement.Q<VisualElement>("Cover");
            m_Cover.style.opacity = 1.0f;
            m_Cover.RegisterCallback<TransitionEndEvent>(evt =>
            {
                if (m_Cover.style.opacity.value > 0.9f && m_TargetLevel >= 0)
                {
                    LevelList.LoadLevel(m_TargetLevel);
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