using Godot;
using System;
using System.Collections.Generic;

public class Network : Node
{
    [Export] int server_port = 3074;
    [Export] int max_players = 6;
    [Export] float net_rate_hz = 30;
    [Export] PackedScene player_model;

    //player id/data dictionary
    Dictionary<int, Player> players = new Dictionary<int, Player>();

    public override void _Ready()
    {
        CreateServer();
        GetTree().Connect("network_peer_connected", this, "OnNetworkClientConnect");
        GetTree().Connect("network_peer_disconnected", this, "OnNetworkClientDisconnect");

        GetNode<Timer>("Timer").WaitTime = 1.0f / net_rate_hz;
    }

    private void OnTimerOut()
    {
        foreach (var entry in players)
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

    private void OnNetworkClientConnect(int connected_client_id)
    {
        GD.Print($"client {connected_client_id} connected");
    }

    private void OnNetworkClientDisconnect(int disconnected_client_id)
    {
        //send delete call on all clients
        foreach (int id in players.Keys)
        {
            if (disconnected_client_id != id)
                RpcId(id, "DeleteDummy", disconnected_client_id);
        }

        players[disconnected_client_id].QueueFree();
        players.Remove(disconnected_client_id);

        GD.Print($"client {disconnected_client_id} disconnected!");
    }

    [Remote]
    private void StartClient(int client_id, string name)
    {
        GD.Print($"client {client_id} ({name}) requested start");

        Player client = player_model.Instance() as Player;
        GetNode("/root/Scene").AddChild(client);
        
        client.name = name;
        client.Position = new Vector2(0,0);

        //spawn the client on all already connected clients
        Rpc("SpawnDummy", client_id, client.name, client.Position);

        //spawn connected clients on his side
        foreach (var player in players)
        {
            RpcId(client_id, "SpawnDummy", player.Key, player.Value.name, player.Value.Position);
        }

        players.Add(client_id, client); //add to the server dictionary
    }

    [Remote]
    private void ProcessClientMovementInstruction(int id, int instruction)
    {
        Player client = players[id];

        if(client == null)
            return;

        client.movement_instructions = instruction;
    }

    [Remote]
    private void RequestPing(int id)
    {
        RpcId(id, "ReceivePing");
    }
}