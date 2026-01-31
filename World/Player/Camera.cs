using Godot;
using System;

public partial class Camera : Camera3D
{
	[Export] private CharacterBody3D character;
	[Export] private float distanceBehind = 11f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GlobalPosition with { X = character.GlobalPosition.X, Z = character.GlobalPosition.Z + distanceBehind};
	}
}
