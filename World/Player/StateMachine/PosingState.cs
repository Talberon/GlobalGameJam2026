using Godot;

namespace Masquerade.World.Player.StateMachine;

public class PosingState(PlayerStateMachine stateMachine, Player player) : IPlayerState
{
    public void OnEnter()
    {
        //Do nothing.
    }

    public void Update(double delta)
    {
        Vector3 direction = player.GetMoveDirection();
        if (!direction.IsZeroApprox())
        {
            stateMachine.CurrentState = PlayerStateMachine.States.Walking;
        }

        Vector3 velocity = Vector3.Zero;
        velocity.X = Mathf.MoveToward(player.Velocity.X, 0, Player.WalkSpeed);
        velocity.Z = Mathf.MoveToward(player.Velocity.Z, 0, Player.WalkSpeed);
        player.Velocity = velocity;
    }

    public static void SetPose(Player player)
    {
        if (Input.IsActionPressed("pose_up"))
        {
            player.SetPose(Player.Poses.Ballet);
        }

        if (Input.IsActionPressed("pose_down"))
        {
            player.SetPose(Player.Poses.Cossack);
        }

        if (Input.IsActionPressed("pose_left"))
        {
            player.SetPose(Player.Poses.Leading);
        }

        if (Input.IsActionPressed("pose_right"))
        {
            player.SetPose(Player.Poses.Salutation);
        }
    }
}