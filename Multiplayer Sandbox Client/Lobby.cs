using Godot;
using System;

public class Lobby : Control
{
    Server server;
    LineEdit username_input;

    public override void _Ready()
    {
        server = GetNode<Server>("/root/Server");
        username_input = GetNode<LineEdit>("UsernameInput");

        GetTree().Connect("connected_to_server", this, "onConnection");
    }

    private void onConnectButtonPressed()
    {
        server.ConnectClient("localhost", 3074); //you can change this
    }

    private void onConnection()
    {
        server.RequestStart(username_input.Text);
        Hide();
    }
}
