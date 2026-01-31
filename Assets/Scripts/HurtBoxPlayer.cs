using System.Collections.Generic;
using UnityEngine;

public class HurtBoxPlayer : MonoBehaviour
{
    public Player myPlayer;
    public List<Player> hittingPlayers = new();

    void OnTriggerEnter2D(Collider2D collider)
    {
        var p = collider.GetComponent<Player>();
        if (p != null && p != myPlayer && !hittingPlayers.Contains(p)) {
            Debug.Log("Player entered\n");
            hittingPlayers.Add(p);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        var p = collider.GetComponent<Player>();
        if (p != null && hittingPlayers.Contains(p))
        {
            hittingPlayers.Remove(p);
        } 
            
    }
}
