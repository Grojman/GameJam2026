using UnityEngine;

public class MaxJumpMask : Mask
{
    protected override void OnGet(Player player)
    {
        player.MaxJumps += 2;
    }

    protected override void OnClose(Player player)
    {
        player.MaxJumps -= 2;
    }
}
