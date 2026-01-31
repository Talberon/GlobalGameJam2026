using Godot;
using Masquerade.World.Player.StateMachine;
using Masquerade.World.Pose;

namespace Masquerade.World.Player;

public partial class Player : Node3D
{
	public const float WalkSpeed = 10f;

	[Export] public CharacterBody3D CharacterBody3D;
	
	public float WalkMomentum = 0f;
	[Export] private float speed = 7.0f;

	[Export] public Facemask CurrentMask;

	[ExportGroup("Camera")] [Export] public Camera3D Camera;
	[Export] public Node3D VisualRoot;
	[Export] public float RotationSpeed = 0.0f;

	[Export] public ActorPose ActorPose;

	public Npc? CurrentDancePartner;

	public float TargetFov = 39f;


	private PlayerStateMachine stateMachine;

	public override void _Ready()
	{
		stateMachine = new PlayerStateMachine(this);
		ActorPose.CurrentPose = ActorPose.Poses.Salutation;
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		stateMachine.PhysicsProcess(delta);

		if (CurrentDancePartner is not null && Input.IsActionJustPressed("debug_trade"))
		{
			GD.Print($"Trading with {CurrentDancePartner?.CurrentMask.Label.Text}");
			TradeMasksWith(CurrentDancePartner);
		}

		CharacterBody3D.MoveAndSlide();
	}

	public void SetDancePartner(Npc? partner)
	{
		CurrentDancePartner = partner;
	}


	public void TradeMasksWith(Npc npc)
	{
		//TODO: Restrict which masks can be traded		

		(CurrentMask.MaskType, npc.CurrentMask.MaskType) = (npc.CurrentMask.MaskType, CurrentMask.MaskType);
		//TODO: Play some effect
	}

	public Vector3 GetMoveDirection()
	{
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Vector3 forward = Camera.GlobalTransform.Basis.Z.Normalized();
		Vector3 right = Camera.GlobalTransform.Basis.X.Normalized();

		//Scrub vertical angle in calculation
		forward.Y = 0;
		right.Y = 0;

		//Renormalize after scrubbing vertical angle
		forward = forward.Normalized();
		right = right.Normalized();

		Vector3 direction = (forward * inputDir.Y) + (right * inputDir.X);
		return direction;
	}
}
