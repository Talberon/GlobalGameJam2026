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
	[Export] public bool IsRomeo;
	[Export] public bool IsJuliette;

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

	public bool IsCompatibleWith(MaskTypes maskType) => currentMaskType switch
	{
		MaskTypes.Jester when IsRomeo => maskType is MaskTypes.Sun,
		MaskTypes.Jester when IsJuliette => maskType is MaskTypes.Moon,
		MaskTypes.Happy => maskType is MaskTypes.Jester,
		MaskTypes.Flower => maskType is MaskTypes.Happy,
		MaskTypes.Hummingbird => maskType is MaskTypes.Flower,
		MaskTypes.Crown => maskType is MaskTypes.Hummingbird,
		MaskTypes.Sun => maskType is MaskTypes.Crown,
		MaskTypes.Sad => maskType is MaskTypes.Jester,
		MaskTypes.Donkey => maskType is MaskTypes.Sad,
		MaskTypes.CheshireCat => maskType is MaskTypes.Donkey,
		MaskTypes.Owl => maskType is MaskTypes.CheshireCat,
		MaskTypes.Moon => maskType is MaskTypes.Owl,
		_ => false
	};

	public override void _Ready()
	{
		MaskType = currentMaskType;

		//TODO: Tint the mask based on whether is Romeo/Juliette
		//TODO: Tint the mask based on whether is Romeo/Juliette
		//TODO: Tint the mask based on whether is Romeo/Juliette
		//TODO: Tint the mask based on whether is Romeo/Juliette

		base._Ready();
	}
}
