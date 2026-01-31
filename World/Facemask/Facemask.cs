using System;
using Godot;

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
	[Export] public AnimationPlayer AnimationPlayer;

	[Export]
	public MaskTypes MaskType
	{
		get => currentMaskType;
		set
		{
			currentMaskType = value;
			AnimationPlayer.Play(AnimationForMask(currentMaskType));
			Label.Text = currentMaskType.ToString();
		}
	}

	private MaskTypes currentMaskType = MaskTypes.Jester;

	private string AnimationForMask(MaskTypes type) => type switch
	{
		MaskTypes.Jester => "Jester",
		MaskTypes.Happy => "Happy",
		MaskTypes.Flower => "Flower",
		MaskTypes.Hummingbird => "Hummingbird",
		MaskTypes.Crown => "Crown",
		MaskTypes.Sun => "Sun",
		MaskTypes.Sad => "Sad",
		MaskTypes.Donkey => "Donkey",
		MaskTypes.CheshireCat => "CheshireCat",
		MaskTypes.Owl => "Owl",
		MaskTypes.Moon => "Moon",
		_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
	};

	public override void _Ready()
	{
		MaskType = currentMaskType;
		base._Ready();
	}
}
