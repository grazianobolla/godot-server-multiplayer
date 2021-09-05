using Godot;
using System;

public class Player : Node2D
{
    private Network network;
    private byte last_instruction_sent = 0;
    private Vector2 net_last_position_received = Vector2.Zero;
    
    //the rate in hz at which information about movement is sent to the server
    [Export] private float net_rate_hz = 30;

    public override void _Ready()
    {
        network = GetNode("/root/Network") as Network;
        GetNode<Timer>("Timer").WaitTime = 1.0f / net_rate_hz;
    }

    public override void _Process(float delta)
    {
        Update(); //TODO: remove
    }

    //called every `net_rate_hz`, it sends information about movement to the server
    private void OnTimerOut()
    {
        byte instruction = PlayerMovement.ReadInput();

        if(instruction != last_instruction_sent)
            network.SendClientMovementInstructions(instruction);

        last_instruction_sent = instruction;
    }

    //called when the client receives a new position update from the server
    public void UpdatePosition(Vector2 new_pos)
    {
        net_last_position_received = new_pos;
        Position = net_last_position_received;
    }

    public void Setup(int id, string name, Vector2 initial_position)
    {
        this.Name = id.ToString();
        SetLabelText(name);
        this.Position = initial_position;
        this.SetNetworkMaster(id);
    }

    public void SetLabelText(string text)
    {
        GetNode<Label>("UsernameLabel").Text = text;
    }

    //TODO: remove, debug: it draws the server position received by the client,
    //it helps visualize interpolation or client / side prediction.
    public override void _Draw()
    {
        DrawCircle(net_last_position_received - GlobalPosition, 8, new Color(1,0,1));
    }
}