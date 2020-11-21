using Godot;
using System;
using System.Collections.Generic;

//data that the server its gonna track
struct ClientData{
	public string name;
	public Vector2 position;
}

public class Server : Node
{
	//player id/data dictionary
	Dictionary<int, ClientData> clients = new Dictionary<int, ClientData>();

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
		//define new connected client
		ClientData data = new ClientData();
		data.name = connected_client_id.ToString();
		data.position = new Vector2(100, 100); //TODO: change spawn point 

		//spawn the client on all already connected clients
		foreach (int id in clients.Keys){
			RpcId(id, "SpawnDummy", connected_client_id, data.name, data.position);
		}
		
		RpcId(connected_client_id, "Spawn", data.name, data.position);

		//spawn connected clients on his side
		foreach (var item in clients){
			RpcId(connected_client_id, "SpawnDummy", item.Key, item.Value.name, item.Value.position);
		}

		clients.Add(connected_client_id, data);
	}

	//called when a client disconnects
	void onNetworkClientDisconnect(int disconnected_client_id){
		//send delete call on all clients
		foreach (int id in clients.Keys){
			if(disconnected_client_id != id) RpcId(id, "DeleteDummy", disconnected_client_id);	
		}

		clients.Remove(disconnected_client_id);
		GD.Print($"Client {disconnected_client_id} disconnected!");
	}

	[Remote] void UpdateClientPosition(int sender_id, Vector3 new_position){
		foreach (var client in clients){
			if(sender_id != client.Key) RpcUnreliableId(client.Key, "UpdateDummyPosition", sender_id, new_position);	
		}
	}
}