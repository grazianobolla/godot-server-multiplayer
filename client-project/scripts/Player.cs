using Godot;
using System;

public class Player : Node2D
{
    private Network network;
    [Export] private float net_rate_hz = 30; //the rate in hz at which information about movement is sent to the server
    private int instruction_count = 0;
    
    //debug variables
    private Vector2 net_last_position_received;

    public override void _Ready()
    {
        network = GetNode("/root/Network") as Network;
        GetNode<Timer>("Timer").WaitTime = 1.0f / net_rate_hz;
    }

    //TODO: remove, debug: it draws the server position received by the client,
    //it helps visualize interpolation or client / side prediction.
    public override void _Draw()
    {
        DrawCircle(net_last_position_received - GlobalPosition, 8, new Color(1,0,1));
    }

    public override void _Process(float delta)
    {
        Update(); //TODO: remove
    }

    //called every `net_rate_hz`, it sends information about movement to the server
    private void OnTimerOut()
    {
        instruction_count++;
        byte instruction = PlayerMovement.ReadInput();
        network.SendClientMovementInstructions(instruction_count, instruction);
    }

    //called when the client receives a new position update from the server
    public void UpdatePosition(int tick, Vector2 position)
    {
        net_last_position_received = position;
        Position = net_last_position_received;
    }

    public void Setup(int id, string name, Vector2 initial_position)
    {
        this.Name = id.ToString();
        GetNode<Label>("UsernameLabel").Text = name;
        this.Position = initial_position;
        this.SetNetworkMaster(id);
    }
}