using Godot;

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
	[Export] public Poses CurrentPose = Poses.Salutation;
	[Export] private Label3D poseLabel;
	
	[Export] public AnimationPlayer AnimationPlayer;
	
	public void SetPose(Poses pose)
	{
		CurrentPose = pose;
		AnimationPlayer.Play($"Pose-{(int)pose}");
		poseLabel.Text = pose.ToString();
	}
	

	public void HandlePoseInput()
	{
		if (Input.IsActionPressed("pose_up"))
		{
			SetPose(Poses.Ballet);
		}

		if (Input.IsActionPressed("pose_down"))
		{
			SetPose(Poses.Cossack);
		}

		if (Input.IsActionPressed("pose_left"))
		{
			SetPose(Poses.Leading);
		}

		if (Input.IsActionPressed("pose_right"))
		{
			SetPose(Poses.Salutation);
		}
	}
}
