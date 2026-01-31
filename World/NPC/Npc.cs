using Godot;
using Masquerade.World.Player;

public partial class Npc : CharacterBody3D
{
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;

    [Export] private Metronome metronome;
    [Export] private Facemask.MaskTypes initialMask = Facemask.MaskTypes.Jester;
    [Export] public Facemask CurrentMask;

    private float targetHeight = 0;

    [Export] private float onBeatHeight = 0f;
    [Export] private float offBeatHeight = 1f;
    [Export] private float lerpSpeed = 10f;

    [Export] private Area3D dancePartnerZone;

    public override void _Ready()
    {
        metronome.OnBeat += () => { targetHeight = onBeatHeight; };
        metronome.OffBeat += () => { targetHeight = offBeatHeight; };

        dancePartnerZone.BodyEntered += (other) =>
        {
            if (other is Player player)
            {
                GD.Print($"New Partner: {CurrentMask.Label.Text}");
                player.SetDancePartner(this);
            }
        };

        CurrentMask.SetMaskType(initialMask);
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;

        float nextHeight = Mathf.Lerp(Position.Y, targetHeight, (float)delta * lerpSpeed);
        Position = Position with { Y = nextHeight };
    }
}