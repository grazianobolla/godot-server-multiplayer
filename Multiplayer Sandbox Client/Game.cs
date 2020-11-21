using Godot;
using System;
using System.Collections.Generic;

public class Game : Node
{
	Dictionary<int, string> clients = new Dictionary<int, string>();

	public void AddModel(int id, string name, Vector2 position){
		PackedScene player_scene = (PackedScene)ResourceLoader.Load("res://Player.tscn");
		
		//configure model
		Player player = (Player)player_scene.Instance();
		player.Name = id.ToString(); //node name
		player.ChangeName(name); //label name
		player.Position = position;
		GetTree().Root.GetNode("Scene/Clients").AddChild(player);
		player.SetNetworkMaster(id);

		//add to dictionary
		clients.Add(id, player.GetPath());

		GD.Print($"Registered player {id} path {player.GetPath()}");
	}

	Player GetModel(int id){
		return (Player)GetNode(clients[id]);
	}

	public void DeleteModel(int id){
		GetModel(id).QueueFree();
		clients.Remove(id);
	}

	public void MoveModel(int id, Vector2 position){
		GetModel(id).Position = position;
	}
}
