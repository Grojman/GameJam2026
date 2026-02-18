using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    public Player player;
    private int groundContacts = 0;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundContacts++;
            player.OnTouchGround();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundContacts--;

            if (groundContacts <= 0)
            {
                groundContacts = 0;
                player.OnLeaveGround();
            }
        }
    }
}