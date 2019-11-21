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

    public static KeyCode Default_ToggleIce = KeyCode.UpArrow;
    public static KeyCode Default_ToggleFire = KeyCode.RightArrow;
    public static KeyCode Default_ToggleWind = KeyCode.DownArrow;
    public static KeyCode Default_ToggleEarth = KeyCode.LeftArrow;

    public static KeyCode Default_shift = KeyCode.RightShift;
    public static KeyCode Default_pause = KeyCode.Escape;

    public static string moveLeft = "Player_LeftKey";
    public static string moveRight = "Player_RightKey";
    public static string moveUp = "Player_UpKey";
    public static string moveDown = "Player_DownKey";
    public static string jump = "Player_Jump";

    public static string toggleIce = "Player_ToggleIce";
    public static string toggleFire = "Player_ToggleFire";
    public static string toggleWind = "Player_ToggleWind";
    public static string toggleEarth = "Player_ToggleEarth";
    public static string shift = "Player_Shift";

    public static string pause = "Player_Pause";

    public PlayerInputProfile()
    {
        //Movement, jumping.
        keyLoadList.Add(new InputCommand(moveLeft, Default_moveLeft));
        keyLoadList.Add(new InputCommand(moveRight, Default_moveRight));
        keyLoadList.Add(new InputCommand(moveUp , Default_moveUp));
        keyLoadList.Add(new InputCommand(moveDown , Default_moveDown));
        keyLoadList.Add(new InputCommand(jump, Default_jump));

        //Ability triggers.
        keyLoadList.Add(new InputCommand(toggleIce, Default_ToggleIce));
        keyLoadList.Add(new InputCommand(toggleFire, Default_ToggleFire));
        keyLoadList.Add(new InputCommand(toggleWind, Default_ToggleWind));
        keyLoadList.Add(new InputCommand(toggleEarth, Default_ToggleEarth));
        keyLoadList.Add(new InputCommand(shift, Default_shift));

        //Pause Menu
        keyLoadList.Add(new InputCommand(pause, Default_pause));

        assignKeys(keyLoadList);
    }
}
