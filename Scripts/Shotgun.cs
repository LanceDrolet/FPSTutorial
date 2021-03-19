using Godot;
using System;

public class Shotgun : Weapon
{
    [Export] public float fireRange { get; set; } = (float)10;
    public override void _Ready()
    {
        base._Ready();
        rayCast.CastTo = new Vector3(0, 0, -fireRange);
    }

}
