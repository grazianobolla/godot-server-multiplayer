using Godot;
using System;

//process and applies information received from the server, like
//interpolation or client / side prediction
public class PlayerMovement : Node2D
{
    //the last position received from the server
    public Vector2 received_net_pos;

    [Export] private bool enable_interpolation = true;
    [Export] private float net_interpolation_speed = 50; //speed of the movement interpolation

    private Node2D player_node;

    public override void _Ready()
    {
        player_node = GetParent() as Node2D;
    }

    public override void _Process(float delta)
    {
        Update(); //TODO: remove, debug
        
        //interpolates to the last server received position for a smoother movement
        if (enable_interpolation)
            player_node.Position = player_node.Position.LinearInterpolate(received_net_pos, delta * net_interpolation_speed);
        else
            player_node.Position = received_net_pos;
    }

    //TODO: remove, debug: it draws the server position received by the client,
    //it helps visualize interpolation or client / side prediction.
    public override void _Draw()
    {
        DrawCircle(received_net_pos - GlobalPosition, 8, new Color(1,0,1));
    }
}