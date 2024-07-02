using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace RainbowJump.Scripts
{

    public class SettingsButton : MonoBehaviour
    {
        public bool muted;
        public GameObject buttons;
        public GameObject muteButton;
        public GameObject languageButton;
        public GameObject unMuteButton;
        public GameObject leaderboardPanel;
        public GameObject profilePanel;
        public Animator buttonsAnimator;
        public AnimationClip newAnimationClip;
        public int timesClicked = 0;

        public GameObject loginPanel;
        public GameObject registerPanel;

        public Text nameText;

        [SerializeField]
        public FirebaseAuthManager firebaseAuthManager;

        public void backButton()
        {
            registerPanel.SetActive(false);
            loginPanel.SetActive(false);
        }

        public void loginButton()
        {
            if(firebaseAuthManager.auth != null && firebaseAuthManager.user != null)
            {
                profilePanel.SetActive(true);
                nameText.text = firebaseAuthManager.user.DisplayName;
            }
            else
            {
                loginPanel.SetActive(true);
                registerPanel.SetActive(false);
            }
        }

        public void logOutButton()
        {
            profilePanel.SetActive(false);
            loginPanel.SetActive(true);
        }

        public void registerButton()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
        }

        public void SettingsButtonClick()
        {
            timesClicked++;
            if (timesClicked % 2 == 0)
            {
                buttonsAnimator.Play(newAnimationClip.name);
                Invoke("DisableButtons", 0.35f);
            }
            else
            {
                buttons.SetActive(true);
            }
        }

        private void DisableButtons()
        {
            buttons.SetActive(false);
        }

        public void UnMuteButtonClick()
        {
            muted = true;
            unMuteButton.SetActive(false);
            muteButton.SetActive(true);
            PlayerPrefs.SetInt("muted", muted ? 1 : 0);
            AudioListener.volume = 0f;
        }

        public void MuteButtonClick()
        {
            muted = false;
            unMuteButton.SetActive(true);
            muteButton.SetActive(false);
            PlayerPrefs.SetInt("muted", muted ? 1 : 0);
            AudioListener.volume = 1f;
        }

        public void BackButtonClick()
        {
            leaderboardPanel.SetActive(false);
            timesClicked++;
        }

        public void LeaderboardButtonClick()
        {
            leaderboardPanel.SetActive(true);
            buttonsAnimator.Play(newAnimationClip.name);
            Invoke("DisableButtons", 0.35f);

        }

        public void BackProfile()
        {
            profilePanel.SetActive(false);
        }

        void Start()
        {
            buttons.SetActive(false);
            muted = PlayerPrefs.GetInt("muted", 0) == 1;
            if (muted)
            {
                UnMuteButtonClick();
            }
            else
            {
                MuteButtonClick();
            }
        }

    }
}
