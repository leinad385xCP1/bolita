using Godot;
using System;

public partial class camaraBolita : Camera3D
{
    [Export] public Node3D Target; // Asigna la bolita aquí en el editor
    [Export] public float SmoothSpeed = 5.0f;
    [Export] public Vector3 Offset = new Vector3(3, 5, -4);

    public override void _Process(double delta)
    {
        if (Target == null) return;

        // Posición deseada detrás de la bolita
        Vector3 desiredPosition = Target.GlobalTransform.Origin + Target.GlobalTransform.Basis * Offset;

        // Suaviza la transición de la cámara
        GlobalTransform = new Transform3D(
            GlobalTransform.Basis, 
            GlobalTransform.Origin.Lerp(desiredPosition, (float)delta * SmoothSpeed)
        );

        // La cámara siempre mira a la bolita
        LookAt(Target.GlobalTransform.Origin, Vector3.Up);
    }
}

