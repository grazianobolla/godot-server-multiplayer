using Godot;
using System;
using System.Collections.Generic;

//data that the server its gonna track
struct ClientData
{
    public string name;
    public Vector2 position;

    public void SetPos(Vector2 new_position)
    {
        position = new_position;
    }
};

public class Server : Node
{
    //player id/data dictionary
    Dictionary<int, ClientData> clients = new Dictionary<int, ClientData>();

    [Export] int server_port = 3074;
    [Export] int max_players = 6;

    public override void _Ready()
    {
        CreateServer();
        GetTree().Connect("network_peer_connected", this, "onNetworkClientConnect");
        GetTree().Connect("network_peer_disconnected", this, "onNetworkClientDisconnect");
    }

    private void CreateServer()
    {
        NetworkedMultiplayerENet peer = new NetworkedMultiplayerENet();
        peer.CreateServer(server_port, max_players);
        GetTree().NetworkPeer = peer;
        GD.Print("Server listening on 3074");
    }

    //called when a client connects the server
    private void onNetworkClientConnect(int connected_client_id)
    {
        GD.Print($"Client {connected_client_id} connected");
    }

    //called when a client disconnects
    private void onNetworkClientDisconnect(int disconnected_client_id)
    {
        //send delete call on all clients
        foreach (int id in clients.Keys)
        {
            if (disconnected_client_id != id)
                RpcId(id, "DeleteDummy", disconnected_client_id);
        }

        clients.Remove(disconnected_client_id);
        GD.Print($"Client {disconnected_client_id} disconnected!");
    }

    [Remote]
    private void UpdateClientPosition(int sender_id, Vector2 new_position)
    {
        clients[sender_id].SetPos(new_position);

        foreach (int id in clients.Keys)
        {
            if (id != sender_id)
                RpcUnreliableId(id, "UpdateDummyPosition", sender_id, new_position);
        }
    }

    [Remote]
    private void StartClient(int client_id, string name)
    {
        GD.Print($"Client {client_id} ({name}) requested start");

        //define new connected client
        ClientData new_client = new ClientData
        {
            name = name,
            position = new Vector2(350, 200) //defines spawn position, do n
        };

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
};