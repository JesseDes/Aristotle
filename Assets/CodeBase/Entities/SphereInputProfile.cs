using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SphereInputProfile : InputProfile
{
    public static KeyCode Default_moveLeft = KeyCode.A;
    public static KeyCode Default_moveRight = KeyCode.D;
    public static KeyCode Default_moveUp = KeyCode.W;
    public static KeyCode Default_moveDown = KeyCode.S;

    public static string moveLeft = "Sphere_LeftKey";
    public static string moveRight = "Sphere_RightKey";
    public static string moveUp = "Sphere_UpKey";
    public static string moveDown = "Sphere_DownKey";

    public SphereInputProfile()
    {
        keyLoadList.Add(new InputCommand(moveLeft, Default_moveLeft));
        keyLoadList.Add(new InputCommand(moveRight, Default_moveRight));
        keyLoadList.Add(new InputCommand(moveUp , Default_moveUp));
        keyLoadList.Add(new InputCommand(moveDown , Default_moveDown));

        assignKeys(keyLoadList);
    }
}
