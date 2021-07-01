using Godot;
using System;

//handles movement both for the local client and the dummies
public class Player : KinematicBody2D
{
    Server server; //server singleton

    [Export] float speed = 250;
    [Export] float net_interpolation_speed = 20;

    public Vector2 received_net_pos; //last position received from the server

    public override void _Ready()
    {
        server = (Server)GetNode("/root/Server");
    }

    public override void _PhysicsProcess(float delta)
    {
        //if its a dummy
        if (IsNetworkMaster() == false)
        {
            //interpolates the positions to get a smoother gameplay
            Position = Position.LinearInterpolate(received_net_pos, delta * net_interpolation_speed);
            return;
        }

        //very simple input, you should do movement calculations on the server, not here!
        Vector2 direction = Vector2.Zero;
        if (Input.IsActionPressed("ui_right")) direction.x = 1;
        else if (Input.IsActionPressed("ui_left")) direction.x = -1;
        if (Input.IsActionPressed("ui_up")) direction.y = -1;
        else if (Input.IsActionPressed("ui_down")) direction.y = 1;
        direction = direction.Normalized();

        MoveAndCollide(direction * speed); //internally calls delta

        //you may want to not send on every tick, but its fine, maybe create a separate thread for this
        server.SendClientPosition(Position);
    }

    public void ChangeName(string name)
    {
        GetNode<Label>("Sprite/UsernameLabel").Text = name;
    }
}
