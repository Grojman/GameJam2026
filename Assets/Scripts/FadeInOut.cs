using System;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    static int id = 0;
    static int GetId() => id++;
    int myId;
    public enum Status { IN, OUT }
    public Status status = Status.IN;
    public float duration = 3f;
    public float timer = 0;
    public Action OnEnd;
    private Image image;

    void Start()
    {
        myId = GetId();
        image = GetComponent<Image>();
        timer = duration;
    }

    void Update()
    {
        // Protecci�n anti-crasheos
        if (image == null) {
            Debug.Log("I am not updating because of this");
            return;
        }

        if (status == Status.IN && timer > 0)
        {
            timer -= Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, timer / duration);
            if (timer <= 0)
            {
                timer = 0;
                OnEnd?.Invoke();
                OnEnd = null;
            }
        }
        else if (status == Status.OUT && timer < duration)
        {
            timer += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, timer / duration);
            if (timer >= duration)
            {
                timer = duration;
                OnEnd?.Invoke();
                OnEnd = null;
            }
        }
    }

    public void SetIn(Action onEnd)
    {
        Debug.Log($"Soy {myId} SetIn");
        OnEnd = onEnd;
        timer = duration;
        status = Status.IN;
    }

    public void SetOut(Action onEnd)
    {
        Debug.Log($"Soy {myId} SetOut");
        OnEnd = onEnd;
        timer = 0;
        status = Status.OUT;
    }
}