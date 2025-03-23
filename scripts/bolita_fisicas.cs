using Godot;
using System;

public partial class bolita_fisicas : CharacterBody3D
{
    [Export] public float Speed = 5.0f;
    [Export] public float Acceleration = 10.0f;
    [Export] public float Gravity = 9.8f;
    [Export] public Camera3D Camera; // Asigna la cámara en el Inspector

    private Vector3 _velocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta)
    {
        if (Camera == null) return; // Asegura que la cámara está asignada

        Vector3 direction = Vector3.Zero;

        // Obtener la dirección relativa a la cámara
        Vector3 forward = -Camera.GlobalTransform.Basis.Z.Normalized();
        Vector3 right = Camera.GlobalTransform.Basis.X.Normalized();

        if (Input.IsActionPressed("move_forward"))
            direction += forward;
        if (Input.IsActionPressed("move_back"))
            direction -= forward;
        if (Input.IsActionPressed("move_left"))
            direction -= right;
        if (Input.IsActionPressed("move_right"))
            direction += right;

        direction = direction.Normalized();

        // Movimiento con aceleración
        Vector3 targetVelocity = direction * Speed;
        _velocity.X = Mathf.Lerp(_velocity.X, targetVelocity.X, (float)delta * Acceleration);
        _velocity.Z = Mathf.Lerp(_velocity.Z, targetVelocity.Z, (float)delta * Acceleration);

        // Aplicar gravedad
        if (!IsOnFloor()) 
            _velocity.Y -= Gravity * (float)delta;

        // Mover la bola
        Velocity = _velocity;
        MoveAndSlide();
    }
}