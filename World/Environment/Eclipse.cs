using Godot;
using System;

public partial class Eclipse : Node3D
{
	[Export] private Node3D preEclipse;
	[Export] private Node3D postEclipse;

	[Export] private AnimationPlayer sunAnimation;

	public override void _Ready()
	{
		sunAnimation.Play("sun");
		base._Ready();
	}
}
