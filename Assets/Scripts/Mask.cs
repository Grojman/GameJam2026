using TMPro;
using UnityEngine;
//Aquí me colé yo
public class Mask : MonoBehaviour
{
    bool shown = true;
    public float TimeMask = 5f;
    public float CooldownMask = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (shown)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.GetMask(this);
            }
        }
    }

    public void Hide()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        shown = false;
    }

    public void Show()
    {
        shown = true;
        GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    protected virtual void OnGet(Player player)
    {
        
    }

    protected virtual void OnClose(Player player)
    {
        
    }

    public void Get(Player player) => OnGet(player);
    public void Close(Player player) => OnClose(player);
}
