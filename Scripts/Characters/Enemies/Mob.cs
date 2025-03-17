using Godot;
using System;

public partial class Mob : CharacterBody3D
{
    [Signal]
    public delegate void MobSquashedEventHandler();
    

    // exported variables
    [Export] public float minSpeed { get; private set; } = 10f;
    [Export] public float maxSpeed { get; private set; } = 18f;
    [Export] public float minAngle { get; private set; } = -Mathf.Pi / 4;
    [Export] public float maxAngle { get; private set; } = Mathf.Pi / 4;
    
    //declared variables
    private Vector3 velocity = Vector3.Zero;


    public override void _PhysicsProcess(double delta)
    {
        
        
        MoveAndSlide();
    }

    public void OnVisibleOnScreenNotifier3DScreenExited()
    {
        QueueFree();
    }

    public void InitializeMob(Vector3 mobStartPosition, Vector3 playerStartPosition)
    {
        
        // the mob has rotated 
        LookAtFromPosition(mobStartPosition, playerStartPosition, Vector3.Up);
        RotateY((float)GD.RandRange(minAngle, maxAngle));
        // getting the speed in its forward direction
        Velocity = Vector3.Forward * (float)GD.RandRange(minSpeed, maxSpeed);
        // turning velocity to match the mob direction
        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);

        GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = (float)GD.RandRange(minSpeed, maxSpeed) % minSpeed;
    }

    public void Squash()
    {
        QueueFree();
        EmitSignal(SignalName.MobSquashed);
    }
}
