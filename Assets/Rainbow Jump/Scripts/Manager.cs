using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using System.IO;
using SouthPointe.Serialization.MessagePack;
using Unity.VisualScripting;
using MessagePack;


namespace RainbowJump.Scripts
{
    public class Manager : MonoBehaviour
    {
        public string androidUrl;
        public string iOSUrl;

        public Player playerMovement;
        public TrailRenderer playerTrail;
        public Spawner spawner;
        public GameObject mainCamera;

        public Transform playerTransform;
        public Text scoreText;
        public Text highScoreText; // reference to the UI text for displaying the high score
        public Text obstaclePassedText; // reference to the UI text for displaying the high score

        public float score = 0f;
        public float obstacleScore = 0;
        private float highScore = 0f; // variable to hold the high score

        public bool gameOver = false;

        public SpriteRenderer playerSprite;

        public GameObject playerScore;
        public GameObject deathParticle;
        public GameObject gameOverUI;
        public GameObject tapToPlayUI;
        public GameObject tapToStartBtn;
        public GameObject settingsButton;
        public GameObject settingsButtons;
        public SettingsButton settingsButtonScript;

        public AudioClip tapSound;
        public AudioClip deathSound;
        public AudioClip buttonSound;
        private AudioSource audioSource;
        public MessagePackFormatter formatter;

        [SerializeField] private List<Vector3> obstaclesPos;

        [SerializeField] private LocalizedString localStringScore;
        [SerializeField] private LocalizedString localStringObstacleScore;

        private void OnEnable()
        {
            localStringScore.Arguments = new object[] { highScore };
            localStringObstacleScore.Arguments = new object[] { obstacleScore };
            localStringScore.StringChanged += UpdateText;
            localStringObstacleScore.StringChanged += UpdateObstacleText;
        }

        private void UpdateObstacleText(string value)
        {
            obstaclePassedText.text = value;
        }

        private void UpdateText(string value)
        {
            highScoreText.text = value;  
        }

        private void OnDisable()
        {
            localStringScore.StringChanged -= UpdateText;
            localStringObstacleScore.StringChanged -= UpdateObstacleText;
        }


        // Start is called before the first frame update
        void Start()
        {
            formatter = new MessagePackFormatter();
            Application.targetFrameRate = 144;
            playerMovement.enabled = false;
            playerMovement.rb.simulated = false;

            audioSource = GetComponent<AudioSource>();

            // Load the high score from PlayerPrefs and display it in the UI
            //highScore = PlayerPrefs.GetFloat("HighScore", 0f);

            //highScore = 0;

            string json = File.ReadAllText(Application.dataPath + "/Data/DataFile.json");
            DataModel data = JSONSerializeable<DataModel>.CreateFromJSON(json);
            highScore = data.score;

            localStringScore.Arguments[0] = highScore.ToString("0");
            localStringScore.RefreshString();
            localStringObstacleScore.Arguments[0] = 0;
            localStringObstacleScore.RefreshString();

           
        }

        // Update is called once per frame
        void Update()
        {
            obstaclesPos = spawner.GetObstacleSpawned();
            if (gameOver == true)
            {
                playerMovement.enabled = false;
                playerMovement.rb.simulated = false;
                playerScore.SetActive(false);
                playerSprite.enabled = false;
                tapToStartBtn.SetActive(false);
                gameOverUI.SetActive(true);
                deathParticle.SetActive(true);
                gameOver = false;

                // Save the high score if it's greater than the current high score
                if (score > highScore)
                {
                    DataModel data = new DataModel();
                    data.score = score;
                    data.day = DateTime.Now.Day;
                    data.month = DateTime.Now.Month;
                    data.year = DateTime.Now.Year;
                    data.obstclepassed = obstacleScore;

                    string json = data.CreateToJSON();
                    File.WriteAllText(Application.dataPath + "/Data/DataFile.json", json);
                    Debug.Log(json);

                    byte[] bytes = new MessagePackFormatter().Serialize(data);
                    string jsonMSGPACK = formatter.AsJson(bytes);
                    File.WriteAllText(Application.dataPath + "/Data/DataFileMsgPack.json", jsonMSGPACK);
                    Debug.Log(jsonMSGPACK);

                    highScore = score;
                    PlayerPrefs.SetFloat("HighScore", highScore);
                    localStringScore.Arguments[0] = highScore.ToString("0");
                    localStringScore.RefreshString();
                }

                localStringObstacleScore.Arguments[0] = obstacleScore;
                localStringObstacleScore.RefreshString();
            }

            // Update the score to be the highest player's y position in the current game session
            if (playerTransform.position.y > score)
            {
                score = playerTransform.position.y;
            }

            for (int i = 0; i < obstaclesPos.Count; i++)
            {
                Vector3 obstacle = obstaclesPos[i];
                bool isAboveofDot = obstacle.y < playerTransform.position.y;
                if (isAboveofDot)
                {
                    obstaclesPos.RemoveAt(i);
                    Debug.Log("yay");
                    obstacleScore++;
                }
              
            }

            // Display the score as the highest player's y position minus 3
            scoreText.text = (score).ToString("0");
        }

        

        public void TapToStart()
        {
            settingsButtonScript.timesClicked = 0;
            PlayTapSound();
            playerMovement.rb.simulated = true;
            playerMovement.rb.velocity = Vector2.up * playerMovement.jumpForce;
            playerScore.SetActive(true);
            tapToPlayUI.SetActive(false);
            settingsButton.SetActive(false);
            settingsButtons.SetActive(false);
            tapToStartBtn.SetActive(false);
            playerMovement.enabled = true;
        }

        public void RestartGame()
        {
            obstaclesPos.Clear();

            settingsButton.SetActive(true);
            tapToPlayUI.SetActive(true);
            tapToStartBtn.SetActive(true);
            playerSprite.enabled = true;
            deathParticle.SetActive(false);
            gameOverUI.SetActive(false);
            playerMovement.rb.simulated = false;
            playerMovement.transform.position = new Vector3(0f, -3f, 0f);
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
            spawner.DestroyAllObstacles();
            spawner.InitializeObstacles();
            obstaclesPos = spawner.GetObstacleSpawned();
            score = 0f;
            obstacleScore = 0f;

            playerTrail.Clear();
        }

        public void PlayTapSound()
        {
            audioSource.PlayOneShot(tapSound);
        }

        public void PlayDeathSound()
        {
            audioSource.PlayOneShot(deathSound);
        }

        public void PlayButtonSound()
        {
            audioSource.PlayOneShot(buttonSound);
        }

        public void OpenURL()
        {
#if UNITY_ANDROID
        Application.OpenURL(androidUrl);
#elif UNITY_IOS
        Application.OpenURL(iOSUrl);
#endif
        }
    }
}