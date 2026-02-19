using UnityEngine;

public class JumpMask : Mask
{
    protected override void OnGet(Player player)
    {
        player.JumpForce += 20;
    }

    protected override void OnClose(Player player)
    {
        player.JumpForce -= 20;
    }
}
