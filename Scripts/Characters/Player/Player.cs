using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Signal] public delegate void HitEventHandler();
    [ExportGroup("Physical Variables")] [Export]
    public float moveSpeed = 14f;

    [Export] public float jumpSpeed = 10f;
    [Export] public float gravity = 35f;

    [Export] public float jumpImpulse = 16f;
    // physical variables
    private Vector3 inputVector;
    private Vector3 velocity;
    public override void _PhysicsProcess(double delta)
    {
        inputVector = Vector3.Zero;
        if (Input.IsActionPressed(GlobalConstants.MOVELEFT))
        {
            inputVector.X -= 1;
        }

        if (Input.IsActionPressed(GlobalConstants.MOVERIGHT))
        {
            inputVector.X += 1;
        }

        if (Input.IsActionPressed(GlobalConstants.MOVEFORWARD))
        {
            inputVector.Z -= 1;
        }

        if (Input.IsActionPressed(GlobalConstants.MOVEBACKWARD))
        {
            inputVector.Z += 1;
        }

        if (inputVector != Vector3.Zero)
        {
            inputVector = inputVector.Normalized();

            GetNode<Node3D>("PlayerPivot").Basis = Basis.LookingAt(inputVector);

            GetNode<AnimationPlayer>("PlayerAnimPlayer").SpeedScale = 4;

        }

        else
        {
            GetNode<AnimationPlayer>("PlayerAnimPlayer").SpeedScale = 1;
        }

        // checking for jump and fall
        if (IsOnFloor() && Input.IsActionJustPressed(GlobalConstants.JUMP))
        {
            velocity.Y = jumpSpeed;
        }

        if (!IsOnFloor())
        {
            velocity.Y -= gravity * (float)delta;
        }

        velocity.X = inputVector.X * moveSpeed;
        velocity.Z = inputVector.Z * moveSpeed;

        Velocity = new Vector3(velocity.X, velocity.Y, velocity.Z);

        MoveAndSlide();
        
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D collision = GetSlideCollision(i);
            if (collision.GetCollider() is Mob mob)
            {
                if (Vector3.Up.Dot(collision.GetNormal()) > 0.1)
                {
                    mob.Squash();
                    velocity.Y = jumpImpulse;
                    break;
                }
            }
        }

        var pivot = GetNode<Node3D>("PlayerPivot");
        GetNode<Node3D>("PlayerPivot").Rotation = new Vector3(Mathf.Pi/6 * Velocity.Y/jumpImpulse, pivot.Rotation.Y, pivot.Rotation.Z);

    }

    public void OnMobDetectorBodyEntered(Node3D body)
    {
        Die();
        GD.Print("Player Died");
    }

    public void Die()
    {
        QueueFree();
        EmitSignal(SignalName.Hit);
    }
}
