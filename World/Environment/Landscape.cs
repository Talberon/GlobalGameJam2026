using Godot;

public partial class Landscape : Node3D
{
	[Export] private Node3D preEclipse;
	[Export] private Node3D postEclipse;

	[Export] private AnimationPlayer sunAnimation;

	public void EnableEclipseScene()
	{
		preEclipse.Visible = false;
		postEclipse.Visible = true;
	}
	
	public override void _Ready()
	{
		base._Ready();
	}
}
