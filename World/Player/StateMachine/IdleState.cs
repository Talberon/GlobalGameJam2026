using Godot;

namespace Masquerade.World.Player.StateMachine;

public class IdleState(PlayerStateMachine stateMachine, Player player) : IPlayerState
{
    
    public void OnEnter()
    {
        player.AnimationPlayer.Play("Idle");
        player.AnimationPlayer.SpeedScale = 1;
    }

    public void Update(double delta)
    {
        Vector3 direction = player.GetMoveDirection();
        if (!direction.IsZeroApprox())
        {
            stateMachine.CurrentState = PlayerStateMachine.States.Walking;
        }
        
        PosingState.SetPose(player);

        Vector3 velocity = Vector3.Zero;
        velocity.X = Mathf.MoveToward(player.Velocity.X, 0, Player.WalkSpeed);
        velocity.Z = Mathf.MoveToward(player.Velocity.Z, 0, Player.WalkSpeed);
        player.Velocity = velocity;
    }
}