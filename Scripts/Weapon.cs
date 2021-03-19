using Godot;
using System;

public class Weapon : Node
{
    
    [Export] protected float fireRate {get; set;} = (float)0.5;
    [Export] protected int clipSize {get; set;} = 5;
    [Export] protected float reloadRate {get; set;} = (float)1.0;

    int currentAmmo;
    bool reloading = false;
    protected RayCast rayCast;
    protected Label ammoLabel;

     bool canFire = true;
    public override void _Ready()
    {
        ammoLabel = GetNode<Label>("/root/World/UI/Label");
        currentAmmo = clipSize;
        rayCast = GetNode<RayCast>("../Head/Camera/RayCast");
    }
 public override void _Process(float delta)
 {
     if(reloading)
        ammoLabel.Text = "Reloading...";
     else
        ammoLabel.Text = currentAmmo.ToString() + " / " + clipSize.ToString();
     if(Input.IsActionJustPressed("primaryFire") && canFire)
     {
        if (currentAmmo > 0 && !reloading)
            Shoot();
        else if(!reloading)
            Reload();
     }
    if(Input.IsActionJustPressed("Reload") && !reloading)
        Reload();
 }

 private async void Shoot()
 {
        GD.Print("Fired Weapon");
        canFire = false;
        currentAmmo -= 1;
        CheckCollision();
        await ToSignal(GetTree().CreateTimer(fireRate), "timeout");
        canFire = true;
 }

 private async void Reload()
 {
        reloading = true;
        GD.Print("Reloading!");
        await ToSignal(GetTree().CreateTimer(reloadRate), "timeout");
        currentAmmo = clipSize;
        reloading = false;
        GD.Print("Reload Complete.");
 }
 public void CheckCollision()
 {
     if(rayCast.IsColliding())
     {
         GD.Print("colliding");
         Node collider = (Node)rayCast.GetCollider();
         if(collider.IsInGroup("Enemies"))
         {
             collider.QueueFree();
             GD.Print($"Killed ", collider.Name);
         }
     }
 }
}
