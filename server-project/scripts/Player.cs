using Godot;
using System;

public class Player : KinematicBody2D
{
    public string name = "null_name";
    public byte received_instruction = 0;

    private Vector2 velocity = Vector2.Zero;

    public override void _Process(float delta)
    {
        Vector2 dir = PlayerMovement.ProcessMovement(received_instruction);
        velocity = MoveAndSlide(dir * PlayerMovement.SPEED, Vector2.Up);
    }
}
