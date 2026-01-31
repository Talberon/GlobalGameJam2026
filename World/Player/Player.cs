using Godot;
using Masquerade.World.Player.StateMachine;

namespace Masquerade.World.Player;

public partial class Player : CharacterBody3D
{
	public const float WalkSpeed = 10f;
	public float WalkMomentum = 0f;
	
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] private float speed = 7.0f;
	
	[ExportGroup("Camera")]
	[Export] public Camera3D Camera;
	[Export] public Node3D VisualRoot;
	[Export] public float RotationSpeed = 10.0f;
	
	public float TargetFov = 39f;

	private PlayerStateMachine stateMachine;

	public override void _Ready()
	{
		stateMachine = new PlayerStateMachine(this);
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		stateMachine.PhysicsProcess(delta);
		MoveAndSlide();
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
