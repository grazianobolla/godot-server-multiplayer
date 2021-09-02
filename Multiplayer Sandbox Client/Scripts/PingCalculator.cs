using Godot;
using System;

public class PingCalculator : Label
{
    private Network network;
    private Label ping_label;
    private uint time = 0;

    public override void _Ready()
    {
        network = GetNode("/root/Network") as Network;
        network.Connect("ReceivedPing", this, "OnReceivedPing");
    }

    private void OnTimerTimeout()
    {
        if (network.isConnected == false)
            return;
            
        network.CalculateLatency();
        time = OS.GetTicksMsec();
    }

    private void OnReceivedPing()
    {
        uint ping = OS.GetTicksMsec() - time;
        Text = $"Ping: {ping} ms";
    }
}
