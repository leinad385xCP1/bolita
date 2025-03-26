using Godot;
using System;

public partial class bolita_fisicas : CharacterBody3D
{
    [Export] public float Speed = 50.0f;
    [Export] public float Acceleration = 10.0f;
    [Export] public float Gravity = 9.8f;
    [Export] public float SlopeFriction = 0.1f; // Fricción en superficies inclinadas
    [Export] public float MaxSlopeAngle = 45.0f; // Ángulo máximo para moverse normalmente
    [Export] public float SpeedIncreaseRate = 1.0f; // Aumento de velocidad en cada intervalo
    [Export] public float MaxSpeed = 50.0f; // Límite de velocidad al deslizarse
    [Export] public Camera3D Camera; // Asigna la cámara en el Inspector

    private Vector3 _velocity = Vector3.Zero;
    private float timeOnSlope = 0.0f;

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

        // Detectar si estamos en una pendiente
        bool onSlope = false;
        Vector3 slopeNormal = Vector3.Up;

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D collision = GetSlideCollision(i);
            float angle = Mathf.RadToDeg(collision.GetNormal().AngleTo(Vector3.Up));

            if (angle > 0.1f && angle < MaxSlopeAngle)
            {
                onSlope = true;
                slopeNormal = collision.GetNormal();
                break;
            }
        }

        // Movimiento con aceleración en pendientes
        Vector3 targetVelocity = direction * Speed;

        if (onSlope)
        {
            Vector3 slopeDirection = (Vector3.Down - slopeNormal * Vector3.Down.Dot(slopeNormal)).Normalized();
            targetVelocity += slopeDirection * Gravity;
            targetVelocity *= 1.0f - SlopeFriction; // Reduce la velocidad en la pendiente

            // Aumentar velocidad cada 2 segundos
            timeOnSlope += (float)delta;
            if (timeOnSlope >= 2.0f)
            {
                Speed = Mathf.Min(Speed + SpeedIncreaseRate, MaxSpeed);
                timeOnSlope = 0.0f; // Reiniciar el temporizador
            }
        }
        else
        {
            timeOnSlope = 0.0f; // Reiniciar si no está en una pendiente
        }

        _velocity.X = Mathf.Lerp(_velocity.X, targetVelocity.X, (float)delta * Acceleration);
        _velocity.Z = Mathf.Lerp(_velocity.Z, targetVelocity.Z, (float)delta * Acceleration);

        // Aplicar gravedad
        if (!IsOnFloor() && !onSlope) 
            _velocity.Y -= Gravity * (float)delta;

        // Mover la bola
        Velocity = _velocity;
        MoveAndSlide();
    }
}
