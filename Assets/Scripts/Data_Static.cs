using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public static class Data_Static
{
    static public List<PlayerInput> playerList = new List<PlayerInput>();
    public static int alivePLayers = playerList.Count;
}
