using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export] float speed = 4;
	
	Server server;
	public override void _Ready()
	{
		server = (Server)GetNode("/root/Server");
	}

	public override void _PhysicsProcess(float delta)
	{
		if(IsNetworkMaster()){
			if(Input.IsActionPressed("ui_right")) Translate(Vector2.Right * delta * speed);
			else if(Input.IsActionPressed("ui_left")) Translate(Vector2.Left * delta * speed);
			server.SendClientPosition(Position);
		}
	}

	public void ChangeName(string name){
		GetNode<Label>("Sprite/UsernameLabel").Text = name;
	}
}
