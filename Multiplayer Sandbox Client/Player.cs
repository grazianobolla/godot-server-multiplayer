using Godot;
using System;

//handles movement both for the local client and the dummies
public class Player : KinematicBody2D
{
	[Export] float speed = 6;
	[Export] float net_interpolation_speed = 20;

	Server server; //server singleton
	public Vector2 received_net_pos; //last position received from the server

	public override void _Ready()
	{
		server = (Server)GetNode("/root/Server");
	}

	public override void _PhysicsProcess(float delta)
	{
		if(IsNetworkMaster() == false){
			//interpolates the positions to get a smoother gameplay
			Position = Position.LinearInterpolate(received_net_pos, delta * net_interpolation_speed);
			return;
		}

		//very simple input
		if(Input.IsActionPressed("ui_right")) Translate(Vector2.Right * delta * speed);
		else if(Input.IsActionPressed("ui_left")) Translate(Vector2.Left * delta * speed);

		if(Input.IsActionPressed("ui_up")) Translate(Vector2.Up * delta * speed);
		else if(Input.IsActionPressed("ui_down")) Translate(Vector2.Down * delta * speed);
		
		server.SendClientPosition(Position);
	}

	public void ChangeName(string name){
		GetNode<Label>("Sprite/UsernameLabel").Text = name;
	}
}
