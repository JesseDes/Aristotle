using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInputProfile : InputProfile
{
    public static KeyCode Default_moveLeft = KeyCode.A;
    public static KeyCode Default_moveRight = KeyCode.D;
    public static KeyCode Default_moveUp = KeyCode.W;
    public static KeyCode Default_moveDown = KeyCode.S;
    public static KeyCode Default_jump = KeyCode.Space;

    public static string moveLeft = "Player_LeftKey";
    public static string moveRight = "Player_RightKey";
    public static string moveUp = "Player_UpKey";
    public static string moveDown = "Player_DownKey";
    public static string jump = "Player_Jump";

    public PlayerInputProfile()
    {
        keyLoadList.Add(new InputCommand(moveLeft, Default_moveLeft));
        keyLoadList.Add(new InputCommand(moveRight, Default_moveRight));
        keyLoadList.Add(new InputCommand(moveUp , Default_moveUp));
        keyLoadList.Add(new InputCommand(moveDown , Default_moveDown));
        keyLoadList.Add(new InputCommand(jump, Default_jump));

        assignKeys(keyLoadList);
    }
}
