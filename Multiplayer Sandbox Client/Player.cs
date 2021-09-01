using Godot;
using System;

//handles movement both for the local client and the dummies
public class Player : Node2D
{
    //network stuff
    public Vector2 received_net_pos; //last position received from the server

    private Network network;

    [Export] private float net_interpolation_speed = 50;
    [Export] private float net_rate_hz = 60;

    private float net_cooldown = 0;
    private float last_input_data = -1;

    public override void _Ready()
    {
        network = (Network)GetNode("/root/Network");
    }

    public override void _Process(float delta)
    {
        //interpolates to the last server received position for a smoother movement
        Position = Position.LinearInterpolate(received_net_pos, delta * net_interpolation_speed);

        if (IsNetworkMaster() == false)
            return;

        if(net_cooldown < 1.0f / net_rate_hz)
        {
            net_cooldown += delta;
            return;
        }
        else net_cooldown = 0;

        int input = ReadInput();

        if(input != last_input_data)
            network.SendClientMovementInstructions(input);

        last_input_data = input;
    }

    public void ChangeName(string name)
    {
        GetNode<Label>("Sprite/UsernameLabel").Text = name;
    }

    private int ReadInput()
    {
        int input_data = -1;

        if (Input.IsActionPressed("ui_right"))
            input_data = 0;

        else if (Input.IsActionPressed("ui_left"))
            input_data = 1;

        if (Input.IsActionPressed("ui_up"))
            input_data = 2;

        else if (Input.IsActionPressed("ui_down"))
            input_data = 3;

        return input_data;
    }
}