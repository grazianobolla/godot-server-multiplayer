using Godot;
using System;

public class Player : KinematicBody2D
{
    public string name = "null_name";
    public byte received_instruction = 0;

    private Vector2 velocity = Vector2.Zero;
    private float speed = 300;

    public override void _Process(float delta)
    {
        Vector2 dir = ProcessMovement(received_instruction);
        velocity = MoveAndSlide(dir * speed, Vector2.Up);
    }

    private Vector2 ProcessMovement(byte instruction)
    {
        Vector2 direction = Vector2.Zero;

        if ((instruction & (1 << 0)) != 0)
            direction.x = 1;

        else if ((instruction & (1 << 1)) != 0)
            direction.x = -1;

        if ((instruction & (1 << 2)) != 0)
            direction.y = -1;

        else if ((instruction & (1 << 3)) != 0)
            direction.y = 1;

        direction = direction.Normalized();

        return direction;
    }
}
