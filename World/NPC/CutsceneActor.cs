using Godot;

public partial class CutsceneActor : Node3D
{
	[Export] private Facemask facemask;

	public override void _Ready()
	{
		facemask.MaskType = Facemask.MaskTypes.Sun;
		base._Ready();
	}
}
