using Godot;
using System;
using System.Collections.Generic;

public class Game : Node
{
	//dictionary of stored players as id : nodepath
	Dictionary<int, string> clients = new Dictionary<int, string>();

	//adds a player model to the scene
	public void AddModel(int id, string name, Vector2 position){
		PackedScene player_scene = (PackedScene)ResourceLoader.Load("res://Player.tscn");
		
		//configure model
		Player player = (Player)player_scene.Instance();
		player.Name = id.ToString(); //node name
		player.ChangeName(name); //label name
		player.Position = position;
		player.SetNetworkMaster(id);

		//add to scene
		GetTree().Root.GetNode("Scene/Clients").AddChild(player);

		//add player to dictionary
		clients.Add(id, player.GetPath());
		GD.Print($"Registered player {id} path {player.GetPath()}");
	}

	//returns the player from the dictionary based on id (its a macro)
	Player GetModel(int id){
		return (Player)GetNode(clients[id]);
	}

	//deletes a player
	public void DeleteModel(int id){
		GetModel(id).QueueFree();
		clients.Remove(id);
	}

	//updates position value on a dummy
	public void MoveModel(int id, Vector2 position){
		GetModel(id).received_net_pos = position;
	}
}
