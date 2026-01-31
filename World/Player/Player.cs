using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] private AnimationPlayer animationPlayer;
	[Export] private float speed = 5.0f;
	
	[ExportGroup("Camera")]
	[Export] public Camera3D Camera;
	[Export] public Node3D VisualRoot;
	[Export] public float RotationSpeed = 10.0f;

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	public Vector3 GetMoveDirection()
	{
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
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
