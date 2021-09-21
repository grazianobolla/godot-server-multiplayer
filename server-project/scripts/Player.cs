using Godot;
using System;

public class Player : KinematicBody2D
{
    public string username = "null_name";
    public int net_id = 0;

    private Network network;
    
    private byte last_instruction;
    private Vector2 last_pos_sent;

    public override void _Ready()
    {
        network = GetNode("/root/Network") as Network;
    }

    public override void _PhysicsProcess(float delta)
    {
        Position += PlayerMovement.Movement(last_instruction, delta);
    }

    public void ProcessMovementRequest(uint tick, byte instruction)
    {
        last_instruction = instruction;

        if (last_pos_sent == Position)
            return;

        network.SendUpdatePosition(net_id, tick, Position);
        last_pos_sent = Position;
    }

    public void Setup(int id, string usr, Vector2 pos)
    {
        net_id = id;
        username = usr;
        Position = pos;
    }
}
