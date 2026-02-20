using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public Player player;


    public void OnCollisionEnter2D(Collision2D c)
    {
        if(c.gameObject.tag == "Ground")
        {
            player.animator.SetBool("Walling", true);
            player.WallSlide = true;
            player.rg.gravityScale *=  0.1f;
            //player.jumpCounter = player.MaxJumps;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            player.WallSlide = false;
            player.animator.SetBool("Walling", false);
            player.rg.gravityScale = player.originalGravity;
        }
    }

}
