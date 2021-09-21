using Godot;
using System;

public class Player : KinematicBody2D
{
    public string username = "null_name";
    public int net_id = 0;

    private Network network;

    public override void _Ready()
    {
        network = GetNode("/root/Network") as Network;
    }

    public void ProcessMovementRequest(uint tick, byte instruction)
    {
        float delta = GetPhysicsProcessDeltaTime();
        Position += PlayerMovement.Movement(instruction, delta);
        network.SendUpdatePosition(net_id, tick, Position);
    }

    public void Setup(int id, string usr, Vector2 pos)
    {
        net_id = id;
        username = usr;
        Position = pos;
    }
}
