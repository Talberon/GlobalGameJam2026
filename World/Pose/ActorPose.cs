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


	[ExportGroup("Posing")] [Export] public Poses CurrentPose = Poses.Salutation;
	[Export] private Label3D poseLabel;
	[Export] public AnimationPlayer AnimationPlayer;

	public override void _Ready()
	{
		SetPose(CurrentPose);
		base._Ready();
	}

	public void SetPose(Poses pose)
	{
		CurrentPose = pose;
		AnimationPlayer.Play(AnimationForPose(CurrentPose));
		poseLabel.Text = pose.ToString();
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
			SetPose(Poses.Salutation);
		}

		if (Input.IsActionPressed("pose_down"))
		{
			SetPose(Poses.Cossack);
		}

		if (Input.IsActionPressed("pose_left"))
		{
			SetPose(Poses.Ballet);
		}

		if (Input.IsActionPressed("pose_right"))
		{
			SetPose(Poses.Leading);
		}
	}
}
