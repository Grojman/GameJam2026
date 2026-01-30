using UnityEngine;

public class Mask : MonoBehaviour
{
    public float TimeMask = 5f;
    public float CooldownMask = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.GetMask(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
