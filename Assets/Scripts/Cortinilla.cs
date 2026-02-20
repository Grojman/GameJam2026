using UnityEngine;

public class Cortinilla : MonoBehaviour
{
    private static Cortinilla instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null)
        {
            FindObjectOfType<FadeInOut>().SetIn(() => {});
            Destroy(gameObject);
            return;
        }

        instance = this;
        FindObjectOfType<FadeInOut>().SetIn(() => {});
        DontDestroyOnLoad(gameObject);

    }

}
