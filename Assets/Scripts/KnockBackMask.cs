using UnityEngine;

public class KnockBackMask : Mask
{
    protected override void OnGet(Player player)
    {
        player.KnocBackForce += 10;
    }

    protected override void OnClose(Player player)
    {
        player.KnocBackForce -= 10;
    }
}
