using Godot;

namespace Masquerade.World.Player.StateMachine;

public class WalkingState(PlayerStateMachine stateMachine, Player player) : IPlayerState
{
    private const float WalkingFov = 39f;
    private static float MomentumSpeed => 0.05f;

    public void OnEnter()
    {
        player.AnimationPlayer.Play("Walk");
        const float runningAnimationSpeed = 0.12f;
        player.AnimationPlayer.SpeedScale = Player.WalkSpeed * runningAnimationSpeed;
    }

    public void Update(double delta)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        if (inputDir.IsZeroApprox())
        {
            stateMachine.CurrentState = PlayerStateMachine.States.Idle;
            return;
        }

        player.WalkMomentum = Mathf.MoveToward(player.WalkMomentum, 1, MomentumSpeed);
        MoveAndFaceDirection(player, delta, player.WalkMomentum);

        player.AnimationPlayer.Play("Walk");
        const float runningAnimationSpeed = 0.12f;
        player.AnimationPlayer.SpeedScale = Player.WalkSpeed * runningAnimationSpeed;
    }

    public static void MoveAndFaceDirection(Player player, double delta, float multiplier = 1.0f)
    {
        Vector3 direction = player.GetMoveDirection();
        Vector3 velocity = player.Velocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Player.WalkSpeed * multiplier;
            velocity.Z = direction.Z * Player.WalkSpeed * multiplier;
        }
        else
        {
            velocity.X = Mathf.MoveToward(player.Velocity.X, 0, Player.WalkSpeed * multiplier);
            velocity.Z = Mathf.MoveToward(player.Velocity.Z, 0, Player.WalkSpeed * multiplier);
        }


        FaceDirection(player, delta, direction, player.RotationSpeed);

        player.TargetFov = WalkingFov;
        player.Velocity = velocity;
    }

    public static void FaceDirection(Player player, double delta, Vector3 direction, float rotationRate)
    {
        if (direction.Length() > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.X, direction.Z);

            float currentAngle = player.Rotation.Y;
            float newAngle = (float)Mathf.LerpAngle(currentAngle, targetAngle, delta * rotationRate);
            player.Rotation = new Vector3(0, newAngle, 0);
        }
    }

    public void OnExit()
    {
        player.WalkMomentum = 0f;
    }
}