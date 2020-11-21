using Godot;
using System;
using System.Collections.Generic;

//data that the server its gonna track
struct Client {
	public string name;
	public Vector2 position;

	public void UpdatePosition(Vector2 pos){
		position = pos;
	}
}

public class Server : Node
{
	//player id/data dictionary
	Dictionary<int, Client> clients = new Dictionary<int, Client>();

	public override void _Ready()
	{
		CreateServer();
		GetTree().Connect("network_peer_connected", this, "onNetworkClientConnect");
		GetTree().Connect("network_peer_disconnected", this, "onNetworkClientDisconnect");
	}

	void CreateServer(){
		NetworkedMultiplayerENet peer = new NetworkedMultiplayerENet();
		peer.CreateServer(3074, 6);
		GetTree().NetworkPeer = peer;
		GD.Print("Server listening on 3074");
	}

	//called when a client connects the server
	void onNetworkClientConnect(int connected_client_id){
		GD.Print($"Client {connected_client_id} connected");
	}

	//called when a client disconnects
	void onNetworkClientDisconnect(int disconnected_client_id){
		//send delete call on all clients
		foreach (int id in clients.Keys)
		{
			if(disconnected_client_id != id) RpcId(id, "DeleteDummy", disconnected_client_id);	
		}

		clients.Remove(disconnected_client_id);
		GD.Print($"Client {disconnected_client_id} disconnected!");
	}

	[Remote] void UpdateClientPosition(int sender_id, Vector2 new_position){
		clients[sender_id].UpdatePosition(new_position);
		
		foreach (int id in clients.Keys)
		{
			if(id != sender_id) RpcUnreliableId(id, "UpdateDummyPosition", sender_id, new_position);
		}
	}

	[Remote] void StartClient(int client_id, string name){
		GD.Print($"Client {client_id} ({name}) requested start");

		//define new connected client
		Client new_client = new Client();

		new_client.name = name;
		new_client.position = new Vector2(350, 200);

		//spawn the client on all already connected clients
		foreach (int id in clients.Keys)
		{
			RpcId(id, "SpawnDummy", client_id, new_client.name, new_client.position);
		}
		
		RpcId(client_id, "Spawn", new_client.name, new_client.position);

		//spawn connected clients on his side
		foreach (var item in clients)
		{
			RpcId(client_id, "SpawnDummy", item.Key, item.Value.name, item.Value.position);
		}

		clients.Add(client_id, new_client); //add to the server dictionary
	}
}