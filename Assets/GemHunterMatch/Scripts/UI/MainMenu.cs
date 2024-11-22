using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Match3
{
    public class MainMenu : MonoBehaviour
    {
        public LevelList LevelList;
        public VisualTreeAsset LevelEntry;

        private UIDocument m_Document;
        private VisualElement m_Cover;

        // Settings menu elements
        private VisualElement m_SettingMenuRoot;
        private Button returnButton;
        private Button closeButton;
        private Slider m_MainVolumeSlider;
        private Slider m_MusicVolumeSlider;
        private Slider m_SFXVolumeSlider;
        private Button openSettingButton;

        private Action m_FadeCallback;

        private int m_TargetLevel = -1;
    
        void Start()
        {
            GameManager.Instance.MainMenuOpened();
        
            m_Document = GetComponent<UIDocument>();
            UIHandler.ApplySafeArea(m_Document.rootVisualElement);

            // Initialize Settings Menu
            InitializeSettingsMenu();

            var container = m_Document.rootVisualElement.Q<VisualElement>("LevelSelectionContainer");
        
            for (var i = 0; i < LevelList.SceneCount; ++i)
            {
    #if UNITY_EDITOR
                // In editor we check if the level is not null. This shouldn't happen in a build as the build script will check
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

                // The container is stretched, which would lead to be able to click UNDER the entry, so we grab the actual
                // level entry inside the container 
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
                // We're fading out
                if (m_Cover.style.opacity.value > 0.9f)
                {
                    if (m_TargetLevel >= 0)
                    {
                        LevelList.LoadLevel(m_TargetLevel);
                    }
                    else if (m_FadeCallback != null)
                    {
                        m_FadeCallback();
                        m_FadeCallback = null;
                    }
                }
            });
                
            StartCoroutine(FadeIn());
        }

        void InitializeSettingsMenu()
        {
            // Query the Settings menu root
            m_SettingMenuRoot = m_Document.rootVisualElement.Q<VisualElement>("Settings");
            m_SettingMenuRoot.style.display = DisplayStyle.None;

            // Query buttons
            returnButton = m_SettingMenuRoot.Q<Button>("ReturnButton");
            closeButton = m_SettingMenuRoot.Q<Button>("CloseButton");
            openSettingButton = m_Document.rootVisualElement.Q<Button>("ButtonMenu");

            // Register button click events
            if (returnButton != null)
            {
                returnButton.clicked += OnReturnButtonClicked;
            }
            else
            {
                Debug.LogError("ReturnButton not found in Settings menu.");
            }

            if (closeButton != null)
            {
                closeButton.clicked += OnCloseButtonClicked;
            }
            else
            {
                Debug.LogError("CloseButton not found in Settings menu.");
            }

            if (openSettingButton != null)
            {
                openSettingButton.clicked += () =>
                {
                    ToggleSettingMenu(true);
                };
            }
            else
            {
                Debug.LogError("ButtonMenu not found in MainMenu UI.");
            }

            // Query sliders
            m_MainVolumeSlider = m_SettingMenuRoot.Q<Slider>("MainVolumeSlider");
            m_MusicVolumeSlider = m_SettingMenuRoot.Q<Slider>("MusicVolumeSlider");
            m_SFXVolumeSlider = m_SettingMenuRoot.Q<Slider>("SFXVolumeSlider");

            // Initialize sliders
            var soundData = GameManager.Instance.Volumes;
            m_MainVolumeSlider.value = soundData.MainVolume;
            m_MusicVolumeSlider.value = soundData.MusicVolume;
            m_SFXVolumeSlider.value = soundData.SFXVolume;

            // Register slider value change callbacks
            m_MainVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                soundData.MainVolume = evt.newValue;
                GameManager.Instance.UpdateVolumes();
            });

            m_MusicVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                soundData.MusicVolume = evt.newValue;
                GameManager.Instance.UpdateVolumes();
            });

            m_SFXVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                soundData.SFXVolume = evt.newValue;
                GameManager.Instance.UpdateVolumes();
            });
        }

        void OnReturnButtonClicked()
        {
            ToggleSettingMenu(false);
            // Implement the desired functionality when the Return button is clicked.
            // For example, close the settings menu or navigate to a different scene.
        }

        void OnCloseButtonClicked()
        {
            ToggleSettingMenu(false);
        }

        void ToggleSettingMenu(bool display)
        {
            m_SettingMenuRoot.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;

            // If there's any input or interaction to disable/enable, handle it here.

            if (!display)
            {
                GameManager.Instance.SaveSoundData();
            }
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
            if (returnButton != null)
            {
                returnButton.clicked -= OnReturnButtonClicked;
            }
            if (closeButton != null)
            {
                closeButton.clicked -= OnCloseButtonClicked;
            }
            // Note: If you used explicit methods for openSettingButton's click event, you can unregister it here.
        }
    }
}