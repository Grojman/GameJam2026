using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public Player player;


    public void OnCollisionEnter2D(Collision2D c)
    {
        if(c.gameObject.tag == "Ground")
        {
            Debug.Log("Pared");
            player.animator.SetBool("Walling", true);
            player.WallSlide = true;
            player.rg.gravityScale *=  0.1f;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Debug.Log("FueraPared");
            player.WallSlide = false;
            player.animator.SetBool("Walling", false);
            player.rg.gravityScale = player.originalGravity;
        }
    }

}
