using System;
using Godot;
using Godot.Collections;

namespace Masquerade.World.Pose;

public partial class ActorPose : Node3D
{
	public enum Poses
	{
		Cossack,
		Ballet,
		Leading,
		Salutation,
	}


	[ExportGroup("Posing")]
	[Export]
	public Poses CurrentPose
	{
		get => currentPose;
		set
		{
			currentPose = value;
			AnimationPlayer.Play(AnimationForPose(currentPose));
			poseLabel.Text = value.ToString();
		}
	}

	private Poses currentPose = Poses.Salutation;
	[Export] private Label3D poseLabel;
	[Export] public AnimationPlayer AnimationPlayer;

	public override void _Ready()
	{
		CurrentPose = currentPose;
		base._Ready();
	}

	private string AnimationForPose(Poses pose) => pose switch
	{
		Poses.Cossack => "cossack",
		Poses.Ballet => "ballet",
		Poses.Leading => "ballroom_lead",
		Poses.Salutation => "salutation",
		_ => throw new ArgumentOutOfRangeException(nameof(pose), pose, null)
	};


	public void HandlePoseInput()
	{
		if (Input.IsActionPressed("pose_up"))
		{
			CurrentPose = (Poses.Salutation);
		}

		if (Input.IsActionPressed("pose_down"))
		{
			CurrentPose = Poses.Cossack;
		}

		if (Input.IsActionPressed("pose_left"))
		{
			CurrentPose = Poses.Ballet;
		}

		if (Input.IsActionPressed("pose_right"))
		{
			CurrentPose = Poses.Leading;
		}
	}
}
