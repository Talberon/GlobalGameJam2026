using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        var horizontalMove = Input.GetAxis("move_left", "move_right");
        //TODO: Game logic
        base._PhysicsProcess(delta);
    }
}