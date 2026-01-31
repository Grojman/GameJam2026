using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public Player player;


    public void OnTriggerEnter2D(Collider2D c)
    {
        if(c.CompareTag("Ground"))
        {
            Debug.Log("Pared");
            player.animator.SetBool("Walling", true);
            player.WallSlide = true;
            player.rg.gravityScale *=  0.1f;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            Debug.Log("FueraPared");
            player.WallSlide = false;
            player.animator.SetBool("Walling", false);
            player.rg.gravityScale = player.originalGravity;
        }
    }

}
