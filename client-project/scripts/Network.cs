using Godot;
using System;

/*
    Networking Class,
    for client-side prediction to work, it needs packages to arrieve in order,
    I couldn't come up with a way to use UDP (RpcUnreliable) packets for it
    so I switched to TCP, but there is probably a way to do it, just not as simple.
*/

public class Network : Node
{
    [Signal]
    public delegate void ReceivedPing();
    public bool isConnected = false;

    private Game game;
    private int net_id = -1;

    public override void _Ready()
    {
        game = GetNode<Game>("/root/Game");

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
        net_id = GetTree().GetNetworkUniqueId();
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
        game.AddModel(id, name, position);
    }

    //removes a dummy
    [Remote]
    private void DeleteDummy(int id)
    {
        GD.Print($"removing dummy id {id}");
        game.DeleteModel(id);
    }

    //changes the position of a dummy
    [Remote]
    private void UpdateDummyPosition(int id, uint tick, Vector2 position)
    {
        game.MoveModel(id, tick, position);
    }

    [Remote]
    private void ReceivePing()
    {
        EmitSignal(nameof(ReceivedPing));
    }

    //sends input information to the server for processing
    public void SendClientMovementInstructions(uint tick, byte instruction)
    {
        RpcId(1, "ProcessClientMovementInstruction", net_id, tick, instruction);
    }

    public void RequestStart(string name)
    {
        RpcId(1, "StartClient", net_id, name);
    }

    public void CalculateLatency()
    {
        RpcId(1, "RequestPing", net_id);
    }
}
