using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    public Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            player.OnTouchGround();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            player.OnLeaveGround();
        }
    }
}
