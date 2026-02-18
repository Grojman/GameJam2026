using System;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    public enum Status { IN, OUT }
    public Status status = Status.IN;
    public float duration = 3f;
    public float timer = 0;
    public Action OnEnd;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        timer = duration;
    }

    void Update()
    {
        // Protección anti-crasheos
        if (image == null) return;

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
        OnEnd = onEnd;
        timer = duration;
        status = Status.IN;
    }

    public void SetOut(Action onEnd)
    {
        OnEnd = onEnd;
        timer = 0;
        status = Status.OUT;
    }
}