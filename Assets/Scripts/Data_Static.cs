using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public static class Data_Static
{
    static public List<PlayerInput> playerList = new List<PlayerInput>();
    public static int alivePLayers = playerList.Count;
    public static int roundCounter = 1;
    public const int ROUND_LIMIT = 10;
}
