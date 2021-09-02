using Godot;
using System;

public class Lobby : Control
{
    Network network;
    LineEdit username_input;
    Button connect_button;

    public override void _Ready()
    {
        network = GetNode<Network>("/root/Network");
        username_input = GetNode<LineEdit>("UsernameInput");
        connect_button = GetNode<Button>("ConnectButton");

        GetTree().Connect("connected_to_server", this, "OnConnection");
    }

    private void onConnectButtonPressed()
    {
        network.ConnectClient("localhost", 3074); //you can change this
    }

    private void OnConnection()
    {
        network.RequestStart(username_input.Text);
        
        connect_button.Hide();
        username_input.Hide();
    }
}
