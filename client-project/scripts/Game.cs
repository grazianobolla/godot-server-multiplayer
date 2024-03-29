using Godot;
using System;
using System.Collections.Generic;

/*
							Note:
Through this script, dummy and player is used interchangeably,
a dummy is a representation of a player on the server,
while the Player is the model / scene itself.

I will update that in the future as to not generate more confusion.
*/

public class Game : Node
{
	//dictionary of stored players as id : instance_id
	Dictionary<int, ulong> dummy_clients = new Dictionary<int, ulong>();

	//adds a player model to the scene
	public void AddModel(int id, string name, Vector2 position)
	{
		PackedScene player_scene = (PackedScene)ResourceLoader.Load("res://scenes/Player.tscn");
		
		//configure model
		Player player = (Player)player_scene.Instance();
		player.Setup(id, name, position);

		//add to scene
		GetTree().Root.GetNode("Scene/Clients").AddChild(player);

		//add player to dictionary
		dummy_clients.Add(id, player.GetInstanceId());
		GD.Print($"registered player {id} path {player.GetPath()}");
	}

	//returns the player from the dictionary based on id (its a macro)
	private Player GetModel(int id)
	{
		if (dummy_clients.ContainsKey(id))
			return (Player)GD.InstanceFromId(dummy_clients[id]);

		return null;	
	}

	//deletes a player
	public void DeleteModel(int id)
	{
		Player player = GetModel(id);

		if(player == null)
			return;

		player.QueueFree();
		dummy_clients.Remove(id);
	}

	//updates position value on a dummy
	public void MoveModel(int id, uint tick, Vector2 position)
	{
		Player player = GetModel(id);

		if(player == null)
			return;

		player.UpdatePosition(tick, position);
	}
}
