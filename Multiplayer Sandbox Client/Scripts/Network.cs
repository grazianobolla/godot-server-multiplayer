using Godot;
using System;

//networking class,
//has to be the same name as the one in the server
//due to how Godot RPC and Networking works

public class Network : Node
{
    [Signal] public delegate void ReceivedPing();
    public bool isConnected = false;

    private Game game_singleton;
    private int unique_local_id = -1;

    public override void _Ready()
    {
        game_singleton = GetNode<Game>("/root/Game");

        //client godot-api signals
        GetTree().Connect("connected_to_server", this, "OnConnection");
        GetTree().Connect("server_disconnected", this, "OnDisconnection");
    }

    //creates a peer and attempts to connect a server
    public void ConnectClient(string address, int port)
    {
        NetworkedMultiplayerENet peer = new NetworkedMultiplayerENet();
        peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.Zlib;
        peer.CreateClient(address, port);
        GetTree().NetworkPeer = peer;
        GD.Print($"connecting to server {address}:{port}");
    }

    //called on connection success
    private void OnConnection()
    {
        GD.Print("connected to server!");
        unique_local_id = GetTree().GetNetworkUniqueId();
        isConnected = true;
    }
    
    //called when this client looses connection
    private void OnDisconnection()
    {
        isConnected = false;
    }
    
    //spawns a dummy player
    [Remote]
    private void SpawnDummy(int id, string name, Vector2 position)
    {
        GD.Print($"spawning player id {id}, name {name}, position {position}");
        game_singleton.AddModel(id, name, position);
    }

    //removes a dummy
    [Remote]
    private void DeleteDummy(int id)
    {
        GD.Print($"removing dummy id {id}");
        game_singleton.DeleteModel(id);
    }

    //changes the position of a dummy
    [Remote]
    private void UpdateDummyPosition(int id, Vector2 new_position)
    {
        game_singleton.MoveModel(id, new_position);
    }

    public void SendClientMovementInstructions(int instruction)
    {
        RpcUnreliableId(1, "ProcessClientMovementInstruction", unique_local_id, instruction);
    }

    public void RequestStart(string name)
    {
        RpcId(1, "StartClient", unique_local_id, name);
    }

    public void CalculateLatency()
    {
        RpcId(1, "RequestPing", unique_local_id);
    }

    [Remote]
    private void ReceivePing()
    {
        EmitSignal(nameof(ReceivedPing));
    }
}
