using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LaunchPad : MonoBehaviour
{
    public UnityEngine.UI.Image imageAnimator;
    public UnityEngine.UI.Slider slider;
    public TextMeshProUGUI label;


    bool startCountDown = false;
    float countDown = 0f;
    const float COUNT_DOWN = 5f;
    public int MaxPlayers;
    public int CurrentPlayers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateLabel();
    }

    // Update is called once per frame
    void Update()
    {

        if(startCountDown)
        {
            countDown += Time.deltaTime;

            slider.value = countDown / COUNT_DOWN;

            if (countDown >= COUNT_DOWN)
            {
                startCountDown = false;

                foreach (PlayerInput pl in Data_Static.playerList)
                {
                    pl.GetComponent<SpriteRenderer>().enabled = false;
                }

                SceneManager.LoadScene("Scenaries");

                //EMPEZAR PARTIDA
                imageAnimator.GetComponent<Animator>().SetTrigger("Start");
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            CurrentPlayers++;
            UpdateLabel();
            if(CurrentPlayers == MaxPlayers)
            {
                startCountDown = true;
                countDown = 0;
            } 
        }
    }

    public void Reset()
    {
        //MaxPlayers++;
        UpdateLabel();
        slider.value = 0;
        countDown = 0;
        startCountDown = false;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            CurrentPlayers--;
            UpdateLabel();
            slider.value = 0;
            countDown = 0;
            startCountDown = false;
        }
    }

    public void UpdateLabel()
    {
        label.text = $"{CurrentPlayers} / {MaxPlayers}";
    }
}
