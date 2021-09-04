using Godot;
using System;

//handles networking for the player, sends information about movement to the server
public class PlayerNetwork : Node
{
    private Network network;
    
    //the rate in hz at which information about movement is sent to the server
    [Export] private float net_rate_hz = 30;
    private byte last_instruction = 0;

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
        byte instruction = ReadInput();

        if(instruction != last_instruction)
            network.SendClientMovementInstructions(instruction);

        last_instruction = instruction;
    }

    private byte ReadInput()
    {
        /*
            1 byte = 8 bits
            0 0 0 0 0 0 0 0
            we have 8 places to encode our input,
            we will use the first 4 bits
        */
        
        byte input = 0;

        if (Input.IsActionPressed("ui_right"))
        {
            input ^= (1 << 0);
        }

        else if (Input.IsActionPressed("ui_left"))
        {
            input ^= (1 << 1);
        }

        if (Input.IsActionPressed("ui_up"))
        {
            input ^= (1 << 2);
        }

        else if (Input.IsActionPressed("ui_down"))
        {
            input ^= (1 << 3);
        }

        return input;
    }
}
