using Godot;
using System;

public class Player : KinematicBody
{
    #region properties
    [Export] int speed = 10;
    [Export] int acceleration = 5;
    [Export] double gravity = 40;
    [Export] float jump_power = 30;
    [Export] float mouse_sensitivity = (float)0.3;
    
    Vector3 velocity;
    private Spatial head;
    private Camera camera;
    private double cameraXRotation = 0;
    #endregion
    public override void _Ready()
    {
        velocity = new Vector3();
        head = GetNode<Spatial>("Head");
        camera = GetNode<Camera>("Head/Camera");
        Input.SetMouseMode(Input.MouseMode.Captured);
    }
    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion eventMouseMotion)
        {
            head.RotateY(deg2rad(-eventMouseMotion.Relative.x) * mouse_sensitivity);
            var x_delta = eventMouseMotion.Relative.y * mouse_sensitivity;
            if(cameraXRotation + x_delta > -90 && cameraXRotation + x_delta < 90)
            {
                camera.RotateX(deg2rad(-x_delta));
                cameraXRotation += x_delta;
            }
        }
        base._Input(@event);
    }

    public override void _PhysicsProcess(float delta)
    {
        var headBasis = head.GlobalTransform.basis;

        base._PhysicsProcess(delta);
        var direction = new Vector3();
        if(Input.IsActionPressed("Move_Forward"))
            direction -= headBasis.z;
        else if(Input.IsActionPressed("Move_Backward"))
            direction += headBasis.z;

        if(Input.IsActionPressed("Move_Left"))
            direction -= headBasis.x;
        else if(Input.IsActionPressed("Move_Right"))
            direction += headBasis.x;

        direction = direction.Normalized();
        velocity = velocity.LinearInterpolate(direction * speed, acceleration * delta);

        velocity.y -= (float)gravity * delta;

        if(Input.IsActionJustPressed("Jump") && IsOnFloor())
            velocity.y += jump_power;

        velocity = MoveAndSlide(velocity, Vector3.Up);
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
            Input.SetMouseMode(Input.MouseMode.Visible);
        base._Process(delta);
    }

    private float deg2rad(double angle)
    {
        return (float)((Math.PI / 180) * angle);
    }
}
