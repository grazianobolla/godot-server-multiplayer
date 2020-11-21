using Godot;
using System;

//handles movement both for the local client and the dummies
public class Player : KinematicBody2D
{
	[Export] float speed = 250;
	[Export] float net_interpolation_speed = 20;

	public Vector2 received_net_pos; //last position received from the server

	Server server; //server singleton

	public override void _Ready()
	{
		server = (Server)GetNode("/root/Server");
	}

	public override void _PhysicsProcess(float delta)
	{
		//if its a dummy
		if(IsNetworkMaster() == false){
			//interpolates the positions to get a smoother gameplay
			Position = Position.LinearInterpolate(received_net_pos, delta * net_interpolation_speed);
			return;
		}

		//very simple input
		Vector2 direction = Vector2.Zero;
		if(Input.IsActionPressed("ui_right")) direction.x = 1;
		else if(Input.IsActionPressed("ui_left")) direction.x = -1;
		if(Input.IsActionPressed("ui_up")) direction.y = -1;
		else if(Input.IsActionPressed("ui_down")) direction.y = 1;
		direction = direction.Normalized();

		MoveAndCollide(direction * speed); //internally calls delta

		server.SendClientPosition(Position);
	}

	public void ChangeName(string name){
		GetNode<Label>("Sprite/UsernameLabel").Text = name;
	}
}
