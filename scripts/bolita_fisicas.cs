using Godot;
using System;

public partial class bolita_fisicas : CharacterBody3D
{
    [Export] public float Speed = 5.0f;
    [Export] public float Acceleration = 10.0f;
    [Export] public float Gravity = 9.8f;

    private Vector3 _velocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta)
    {
        Vector3 direction = Vector3.Zero;

        if (Input.IsActionPressed("move_forward"))
            direction -= Transform.Basis.Z; // Forward
        if (Input.IsActionPressed("move_back"))
            direction += Transform.Basis.Z; // Backward
        if (Input.IsActionPressed("move_left"))
            direction -= Transform.Basis.X; // Left
        if (Input.IsActionPressed("move_right"))
            direction += Transform.Basis.X; // Right

        direction = direction.Normalized();

        // Apply acceleration for smooth movement
        Vector3 targetVelocity = direction * Speed;
        _velocity.X = Mathf.Lerp(_velocity.X, targetVelocity.X, (float)delta * Acceleration);
        _velocity.Z = Mathf.Lerp(_velocity.Z, targetVelocity.Z, (float)delta * Acceleration);

        // Apply gravity
        if (!IsOnFloor()) 
            _velocity.Y -= Gravity * (float)delta;

        // Move the ball
        Velocity = _velocity;
        MoveAndSlide();
    }
}