using Godot;
using System;

//handles networking for the player, sends information about movement to the server
public class PlayerNetwork : Node
{
    private Network network;
    
    //the rate in hz at which information about movement is sent to the server
    [Export] private float net_rate_hz = 30;
    private float last_input_data = -1;

    public override void _Ready()
    {
        network = GetNode("/root/Network") as Network;
        GetNode<Timer>("Timer").WaitTime = 1.0f / net_rate_hz;
    }

    private void OnTimerOut()
    {
        if (IsNetworkMaster() == true)
            SendInputData();
    }

    private void SendInputData()
    {
        int input = ReadInput();

        if(input != last_input_data)
            network.SendClientMovementInstructions(input);

        last_input_data = input;
    }

    private int ReadInput()
    {
        /*
        For a proper game, you should probably send just 1 byte,
        and encode the movement there, since it required a more
        complex code on the server side, I left it as this.
        */
        
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
