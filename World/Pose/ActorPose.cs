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

	[Export] private MeshInstance3D mesh;

	public override void _Ready()
	{
		CurrentPose = currentPose;
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (spinDuration <= 0)
		{
			mesh.Rotation = mesh.Rotation with { Y = 0 };
		}
		else
		{
			//Spin the mesh around 
			mesh.Rotation = mesh.Rotation with { Y = mesh.Rotation.Y + (float)delta * 10f };
			spinDuration -= delta;
		}

		if (mesh.GetActiveMaterial(0) is ShaderMaterial shaderMat)
		{
			shaderMat.SetShaderParameter("spin_rotation", mesh.Rotation.Y);
		}

		if (Input.IsActionJustPressed("debug_trade"))
		{
			GD.Print("Debug: Spin for seconds");
			SpinForSeconds(2d);
		}

		base._PhysicsProcess(delta);
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

	private double spinDuration = 0d;

	public void SpinForSeconds(double duration)
	{
		spinDuration = duration;
		if (mesh.GetActiveMaterial(0) is StandardMaterial3D material)
		{
			material.SetBillboardMode(BaseMaterial3D.BillboardModeEnum.Disabled);
		}
	}
}
