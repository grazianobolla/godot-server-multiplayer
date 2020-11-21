using Godot;
using System;

//networking class, has to be the same name as the one in the server
public class Server : Node
{
	Game game_singleton;
	int unique_id;

    public override void _Ready(){
		//game singleton
		game_singleton = GetNode<Game>("/root/Game");

		//client godot-api signals
		GetTree().Connect("connected_to_server", this, "onConnection");
		GetTree().Connect("connection_failed", this, "onConnectionFailed");
	}

	//creates a peer and attempts to connect a server
	public void ConnectClient(string address, int port){
		NetworkedMultiplayerENet peer = new NetworkedMultiplayerENet();
		peer.CreateClient(address, port);
		GetTree().NetworkPeer = peer;
		GD.Print($"Connecting to server {address}:{port}");
	}

	//called on connection success
	void onConnection(){
		GD.Print("Connected to server!");
		unique_id = GetTree().GetNetworkUniqueId();
	}

	//called on connection fail
	void onConnectionFailed(){
		GD.Print("Couldn't connect server!");
	}

	//spawns the player
	[Remote] void Spawn(string name, Vector2 position){
		GD.Print($"Spawn() name {name}, position {position}");
		game_singleton.AddModel(unique_id, name, position);
	}

	//spawns a dummy player
	[Remote] void SpawnDummy(int id, string name, Vector2 position){
		GD.Print($"SpawnDummy() id {id}, name {name}, position {position}");
		game_singleton.AddModel(id, name, position);
	}

	//removes a dummy
	[Remote] void DeleteDummy(int id){
		GD.Print($"DeleteDummy() id {id}");
		game_singleton.DeleteModel(id);
	}

	//changes the position of a dummy
	[Remote] void UpdateDummyPosition(int id, Vector2 new_position){
		game_singleton.MoveModel(id, new_position);
	}

	//used for sending updates of our position to the server
	public void SendClientPosition(Vector2 position){
		RpcUnreliableId(1, "UpdateClientPosition", unique_id, position);
	}

	public void RequestStart(string name){
		RpcId(1, "StartClient", unique_id, name);
	}
}
