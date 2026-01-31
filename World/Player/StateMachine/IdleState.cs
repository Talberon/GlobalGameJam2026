using Godot;

namespace Masquerade.World.Player.StateMachine;

public class IdleState(PlayerStateMachine stateMachine, Player player) : IPlayerState
{
	
	public void OnEnter()
	{
	}

	public void Update(double delta)
	{
		Vector3 direction = player.GetMoveDirection();
		if (!direction.IsZeroApprox())
		{
			stateMachine.CurrentState = PlayerStateMachine.States.Walking;
		}
		
		player.ActorPose.HandlePoseInput();

		Vector3 velocity = Vector3.Zero;
		velocity.X = Mathf.MoveToward(player.CharacterBody3D.Velocity.X, 0, Player.WalkSpeed);
		velocity.Z = Mathf.MoveToward(player.CharacterBody3D.Velocity.Z, 0, Player.WalkSpeed);
		player.CharacterBody3D.Velocity = velocity;
	}
}
