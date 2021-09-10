using Godot;
using System;

public class Player : KinematicBody2D
{
    public string name = "null_name";
    public int net_id = 0;

    private byte received_instruction = 0;
    private Network network;

    public override void _Ready()
    {
        network = GetNode("/root/Network") as Network;
    }

    public override void _Process(float delta)
    {
        Position += PlayerMovement.Movement(received_instruction, delta);
    }

    public void ProcessMovementRequest(int tick, byte instruction)
    {
        received_instruction = instruction;
        network.SendUpdatePosition(net_id, tick, Position);
    }
}
