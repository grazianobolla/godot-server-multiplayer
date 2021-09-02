using Godot;
using System;

//handles movement both for the local client and the dummies
public class PlayerMovement : Node
{
    //the last position received from the server
    public Vector2 received_net_pos;

    [Export] private bool enable_interpolation = true;

    //speed of the movement interpolation
    [Export] private float net_interpolation_speed = 50;

    //the rate in hz at which information about movement is sent to the server
    [Export] private float net_rate_hz = 60;

    private float net_cooldown = 0;
    private float last_input_data = -1;
    private Network network;

    //logic
    private Node2D player_node;

    public override void _Ready()
    {
        network = GetNode("/root/Network") as Network;
        player_node = GetParent() as Node2D;
    }

    public override void _Process(float delta)
    {
        //interpolates to the last server received position for a smoother movement
        if (enable_interpolation)
            player_node.Position = player_node.Position.LinearInterpolate(received_net_pos, delta * net_interpolation_speed);
        else
            player_node.Position = received_net_pos;
            
        if (ReadyToSend(delta) == true && IsNetworkMaster() == true)
            SendInputData(delta);
    }

    public void ChangeName(string name)
    {
        GetNode<Label>("Sprite/UsernameLabel").Text = name;
    }

    private bool ReadyToSend(float delta)
    {
        if(net_cooldown < 1.0f / net_rate_hz)
        {
            net_cooldown += delta;
            return false;
        }
        else net_cooldown = 0;

        return true;
    }

    private void SendInputData(float delta)
    {
        int input = ReadInput();

        if(input != last_input_data)
            network.SendClientMovementInstructions(input);

        last_input_data = input;
    }

    private int ReadInput()
    {
        //for a proper game, you should probably send just 1 byte and encode the movement there,
        //since it required a more complex code on the server side, I left it as this.
        
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