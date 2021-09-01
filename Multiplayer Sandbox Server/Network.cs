using Godot;
using System;
using System.Collections.Generic;

public class Network : Node
{
    [Export] int server_port = 3074;
    [Export] int max_players = 6;
    [Export] float net_rate_hz = 60;
    [Export] PackedScene player_model;

    //player id/data dictionary
    Dictionary<int, Player> clients = new Dictionary<int, Player>();

    private float net_cooldown = 0;
    
    public override void _Ready()
    {
        CreateServer();
        GetTree().Connect("network_peer_connected", this, "OnNetworkClientConnect");
        GetTree().Connect("network_peer_disconnected", this, "OnNetworkClientDisconnect");
    }

    public override void _Process(float delta)
    {
        //broadcasts game state
        if (net_cooldown < 1.0f / net_rate_hz)
        {
            net_cooldown += delta;
            return;
        }
        else net_cooldown = 0;

        foreach (var entry in clients)
        {
            RpcUnreliable("UpdateDummyPosition", entry.Key, entry.Value.Position);
        }
    }

    private void CreateServer()
    {
        NetworkedMultiplayerENet peer = new NetworkedMultiplayerENet();
        peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.Zlib;
        peer.CreateServer(server_port, max_players);
        GetTree().NetworkPeer = peer;
        GD.Print("server listening on 3074");
    }

    //called when a client connects the server
    private void OnNetworkClientConnect(int connected_client_id)
    {
        GD.Print($"client {connected_client_id} connected");
    }

    //called when a client disconnects
    private void OnNetworkClientDisconnect(int disconnected_client_id)
    {
        //send delete call on all clients
        foreach (int id in clients.Keys)
        {
            if (disconnected_client_id != id)
                RpcId(id, "DeleteDummy", disconnected_client_id);
        }

        clients.Remove(disconnected_client_id);
        GD.Print($"client {disconnected_client_id} disconnected!");
    }

    [Remote]
    private void StartClient(int client_id, string name)
    {
        GD.Print($"client {client_id} ({name}) requested start");

        Player client = player_model.Instance() as Player;
        this.AddChild(client);
        
        client.name = name;
        client.Position = new Vector2(0,0);

        //spawn the client on all already connected clients
        foreach (int id in clients.Keys)
        {
            RpcId(id, "SpawnDummy", client_id, client.name, client.Position);
        }

        RpcId(client_id, "Spawn", client.name, client.Position);

        //spawn connected clients on his side
        foreach (var item in clients)
        {
            RpcId(client_id, "SpawnDummy", item.Key, item.Value.name, item.Value.Position);
        }

        clients.Add(client_id, client); //add to the server dictionary
    }

    [Remote]
    private void ProcessClientMovementInstruction(int id, int instruction)
    {
        Player client = clients[id];

        if(client == null)
            return;

        client.movement_instructions = instruction;
    }
}