using Godot;
using System;

public partial class Facemask : Node3D
{
	public enum MaskTypes
	{
		//STARTING MASK
		Jester,

		//LIGHT ASPECT
		Happy,
		Flower,
		Hummingbird,
		Crown,
		Sun,

		//DARK ASPECT
		Sad,
		Donkey,
		CheshireCat,
		Owl,
		Moon,
	}

	[Export] public Label3D Label;
	[Export] public MaskTypes MaskType = MaskTypes.Jester;

	public void SetMaskType(MaskTypes type)
	{
		MaskType = type;
		Label.Text = type.ToString();
	}

	public override void _Ready()
	{
		Label.Text = MaskType.ToString();
		base._Ready();
	}
}
