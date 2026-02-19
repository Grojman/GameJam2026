using UnityEngine;

public class ChangingRoom : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        var player = coll.GetComponent<Player>();
        if(player != null)
        {
            player.canChange = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        var player = coll.GetComponent<Player>();
        if(player != null)
        {
            player.canChange = false;
        }
    }
}
