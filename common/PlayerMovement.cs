using Godot;
using System;

public static class PlayerMovement
{
    public const float SPEED = 300;

    //returns a byte with player input encoded on it
    public static byte ReadInput()
    {
        /*
            1 byte = 8 bits
            0 0 0 0 0 0 0 0
            we have 8 places to encode our input,
            we will use the first 4 bits
        */
        
        byte input = 0;

        if (Input.IsActionPressed("ui_right"))
            input ^= (1 << 0);

        else if (Input.IsActionPressed("ui_left"))
            input ^= (1 << 1);

        if (Input.IsActionPressed("ui_up"))
            input ^= (1 << 2);

        else if (Input.IsActionPressed("ui_down"))
            input ^= (1 << 3);

        return input;
    }

    //converts the `instruction` byte containing input instruction to a Vector2 `direction`
    public static Vector2 ProcessMovement(byte instruction)
    {
        Vector2 direction = Vector2.Zero;

        if ((instruction & (1 << 0)) != 0)
            direction.x = 1;

        else if ((instruction & (1 << 1)) != 0)
            direction.x = -1;

        if ((instruction & (1 << 2)) != 0)
            direction.y = -1;

        else if ((instruction & (1 << 3)) != 0)
            direction.y = 1;

        direction = direction.Normalized();

        return direction;
    }
}
