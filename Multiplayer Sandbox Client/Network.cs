using Godot;
using System;

//networking class, has to be the same name as the one in the server
public class Network : Node
{
    Game game_singleton;
    int unique_id;

    public override void _Ready()
    {
        //game singleton
        game_singleton = GetNode<Game>("/root/Game");

        //client godot-api signals
        GetTree().Connect("connected_to_server", this, "OnConnection");
        GetTree().Connect("connection_failed", this, "OnConnectionFailed");
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
        unique_id = GetTree().GetNetworkUniqueId();
    }

    //called on connection fail
    private void OnConnectionFailed()
    {
        GD.Print("couldn't connect server!");
    }

    //spawns the player
    [Remote]
    private void Spawn(string name, Vector2 position)
    {
        GD.Print($"spawning player name {name}, position {position}");
        game_singleton.AddModel(unique_id, name, position);
    }

    //spawns a dummy player
    [Remote]
    private void SpawnDummy(int id, string name, Vector2 position)
    {
        GD.Print($"spawning dummy id {id}, name {name}, position {position}");
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

    //used to send player instructions to the server
    public void SendClientMovementInstructions(int instruction)
    {
        RpcUnreliableId(1, "ProcessClientMovementInstruction", unique_id, instruction);
    }

    public void RequestStart(string name)
    {
        RpcId(1, "StartClient", unique_id, name);
    }
}
