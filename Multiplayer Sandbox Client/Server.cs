using Godot;
using System;

public class Server : Node
{
	Game game;
    public override void _Ready(){
		game = GetNode<Game>("/root/Game");
		GetTree().Connect("connected_to_server", this, "onConnection");
		GetTree().Connect("connection_failed", this, "onConnectionFailed");
	
		ConnectClient("localhost", 3074);
	}

	void ConnectClient(string address, int port){
		NetworkedMultiplayerENet peer = new NetworkedMultiplayerENet();
		peer.CreateClient(address, port);
		GetTree().NetworkPeer = peer;
		GD.Print($"Connecting to server {address}:{port}");
	}

	void onConnection(){
		GD.Print("Connected to server!");
	}

	void onConnectionFailed(){
		GD.Print("Couldn't connect server!");
	}

	[Remote] void Spawn(string name, Vector2 position){
		GD.Print($"Spawn() name {name}, position {position}");
		game.AddModel(GetTree().GetNetworkUniqueId(), name, position);
	}

	[Remote] void SpawnDummy(int id, string name, Vector2 position){
		GD.Print($"SpawnDummy() id {id}, name {name}, position {position}");
		game.AddModel(id, name, position);
	}

	[Remote] void DeleteDummy(int id){
		GD.Print($"DeleteDummy() id {id}");
		game.DeleteModel(id);
	}

	[Remote] void UpdateDummyPosition(int id, Vector2 new_position){
		game.MoveModel(id, new_position);
	}

	public void SendClientPosition(Vector2 position){
		RpcUnreliableId(1, "UpdateClientPosition", GetTree().GetNetworkUniqueId(), position);
	}
}
