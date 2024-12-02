using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Match3
{
    public class SettingsMenuHandler
    {
        private VisualElement m_SettingMenuRoot;
        private Slider m_MainVolumeSlider;
        private Slider m_MusicVolumeSlider;
        private Slider m_SFXVolumeSlider;
        private Button m_ReturnButton;
        private Button m_CloseButton;
        private Button m_OpenSettingButton;
        private Action<bool> m_OnToggleSettingsMenu;
        private Action m_OnReturnButtonClicked;

        public SettingsMenuHandler(VisualElement rootElement, Button openSettingButton, Action onReturnButtonClicked = null, Action<bool> onToggleSettingsMenu = null)
        {
            m_OnToggleSettingsMenu = onToggleSettingsMenu;
            m_OnReturnButtonClicked = onReturnButtonClicked;

            // Query the Settings menu root
            m_SettingMenuRoot = rootElement.Q<VisualElement>("Settings");
            m_SettingMenuRoot.style.display = DisplayStyle.None;

            // Query buttons
            m_ReturnButton = m_SettingMenuRoot.Q<Button>("ReturnButton");
            m_CloseButton = m_SettingMenuRoot.Q<Button>("CloseButton");
            m_OpenSettingButton = openSettingButton;

            // Register button click events
            if (m_ReturnButton != null)
            {
                m_ReturnButton.clicked += OnReturnButtonClicked;
            }
            else
            {
                Debug.LogError("ReturnButton not found in Settings menu.");
            }

            if (m_CloseButton != null)
            {
                m_CloseButton.clicked += OnCloseButtonClicked;
            }
            else
            {
                Debug.LogError("CloseButton not found in Settings menu.");
            }

            if (m_OpenSettingButton != null)
            {
                m_OpenSettingButton.clicked += () =>
                {
                    ToggleSettingMenu(true);
                };
            }
            else
            {
                Debug.LogError("OpenSettingsButton not provided.");
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

        private void OnReturnButtonClicked()
        {
            ToggleSettingMenu(false);
            m_OnReturnButtonClicked?.Invoke();
        }

        private void OnCloseButtonClicked()
        {
            ToggleSettingMenu(false);
        }

        public void ToggleSettingMenu(bool display)
        {
            m_SettingMenuRoot.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
            m_OnToggleSettingsMenu?.Invoke(display);

            if (!display)
            {
                GameManager.Instance.SaveSoundData();
            }
        }

        public void Cleanup()
        {
            if (m_ReturnButton != null)
            {
                m_ReturnButton.clicked -= OnReturnButtonClicked;
            }
            if (m_CloseButton != null)
            {
                m_CloseButton.clicked -= OnCloseButtonClicked;
            }
            if (m_OpenSettingButton != null)
            {
                m_OpenSettingButton.clicked -= () => { ToggleSettingMenu(true); };
            }
        }
    }
}