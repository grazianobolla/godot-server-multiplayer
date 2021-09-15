using Godot;
using System;

public class Lobby : Control
{
    Network network;
    LineEdit username_input, address_input;
    Button connect_button;

    public override void _Ready()
    {
        network = GetNode<Network>("/root/Network");
        username_input = GetNode<LineEdit>("UsernameInput");
        address_input = GetNode<LineEdit>("AddressInput");
        connect_button = GetNode<Button>("ConnectButton");

        GetTree().Connect("connected_to_server", this, "OnConnection");
        GetTree().Connect("server_disconnected", this, "OnDisconnection");
    }

    private void onConnectButtonPressed()
    {
        network.ConnectClient(address_input.Text, 3074); //you can change this
    }

    private void OnConnection()
    {
        network.RequestStart(username_input.Text);
        
        this.Hide();
    }

    private void OnDisconnection()
    {
        this.Show();
    }
}
