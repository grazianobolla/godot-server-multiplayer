using Godot;
using System;
using System.Collections.Generic;

public class Player : Node2D
{
    private Network network;
    private List<Instruction> instructions;

    [Export] private float max_deviation_allowed = 4;
    [Export] private float interpolation_speed = 24;

    private bool is_master = false;

    //debug variables
    private Vector2 net_position;
    private Vector2 predicted_pos;

    public override void _Ready()
    {
        instructions = new List<Instruction>();
        network = GetNode("/root/Network") as Network;

        is_master = IsNetworkMaster();

        if (is_master == false)
            GetNode<Label>("CanvasLayer/Label").Visible = false;
    }

    //TODO: remove, debug: it draws the server position received by the client,
    //it helps visualize interpolation or client / side prediction.
    public override void _Draw()
    {
        if (is_master)
            DrawCircle(predicted_pos - GlobalPosition, 8, new Color(1,1,0));
        
        DrawCircle(net_position - GlobalPosition, 6, new Color(1,0,1));
    }

    public override void _PhysicsProcess(float delta)
    {
        Update(); //for debug, may want to remove

        //if we are the master, we predict and reconciliate
        if (is_master)
        {
            ProcessClientInput(delta);
            return;
        }
        
        //if we are a 'dummy' (other clients representation) we interpolate to the received position
        Position = Position.LinearInterpolate(net_position, delta * interpolation_speed);
    }
    
    private void ProcessClientInput(float delta)
    {
        uint current_tick = OS.GetTicksMsec();

        //read input and save to history
        byte instruction = PlayerMovement.ReadInput();
        instructions.Add(new Instruction {tick = current_tick, data = instruction});

        //send input to the server
        network.SendClientMovementInstructions(current_tick, instruction);

        //predict the input
        Position += PlayerMovement.Movement(instruction, delta);
    }
    
    //called when the client receives a new position update from the server
    public void UpdatePosition(uint tick, Vector2 position)
    {
        net_position = position;
        
        //when we receive a update from the server, we want to check and reconciliate if needed
        if(is_master)
            Reconciliate(tick, position);
    }

    private void Reconciliate(uint tick, Vector2 position)
    {
        Vector2 prediction = position;
        //we take the received position and apply all the yet-non processed inputs
        foreach(var instr in instructions)
            if(instr.tick > tick)
                prediction += PlayerMovement.Movement(instr.data, 1.0f / 60.0f); //aprox delta

        instructions.RemoveAll (i => (i.tick <= tick)); //delete the used inputs

        //for debug stuff
        predicted_pos = prediction;

        //the difference between the predicted position and the player (0 means a totally accurate prediction)
        float deviation = (prediction - Position).Length();

        //if the deviation is too big, we snap back to the server received position
        if (deviation > max_deviation_allowed)
            Position = position;

        UpdateDebugLabel($"rec {tick} pos {position} curr {Position} dev {deviation}");
    }

    private void UpdateDebugLabel(string text)
    {
        GetNode<Label>("CanvasLayer/Label").Text = text;
    }

    public void Setup(int id, string name, Vector2 initial_position)
    {
        this.Name = id.ToString();
        GetNode<Label>("UsernameLabel").Text = name;
        this.Position = initial_position;
        this.SetNetworkMaster(id);
    }
}