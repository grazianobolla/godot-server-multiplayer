using Godot;
using System;

public class Player : Node2D
{
    private PlayerMovement player_movement;

    public override void _Ready()
    {
        player_movement = GetNode("PlayerMovement") as PlayerMovement;
    }

    public void Setup(int id, string name, Vector2 initial_position)
    {
        this.Name = id.ToString();
        SetLabelText(name);
        this.Position = initial_position;
        this.SetNetworkMaster(id);
    }

    public void SetLabelText(string text)
    {
        GetNode<Label>("UsernameLabel").Text = text;
    }

    public void UpdatePosition(Vector2 pos)
    {
        player_movement.received_net_pos = pos;
    }
}