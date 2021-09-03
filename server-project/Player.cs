using Godot;
using System;

public class Player : KinematicBody2D
{
    public string name = "null_name";
    public int movement_instructions = -1;

    private Vector2 velocity = Vector2.Zero;
    private float speed = 300;

    public override void _Process(float delta)
    {
        var dir = ProcessMovement(movement_instructions);
        velocity = MoveAndSlide(dir * speed, Vector2.Up);
    }

    private Vector2 ProcessMovement(int instruction)
    {
        Vector2 direction = Vector2.Zero;

        if (instruction == 0)
            direction.x = 1;
        else if (instruction == 1)
            direction.x = -1;

        if (instruction == 2)
            direction.y = -1;
        else if (instruction == 3)
            direction.y = 1;

        direction = direction.Normalized();

        return direction;
    }
}
