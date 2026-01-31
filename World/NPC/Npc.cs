using Godot;
using Masquerade.World.Player;

public partial class Npc : Node3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	[Export] private CharacterBody3D characterBody3D;

	[Export] private Metronome metronome;
	[Export] private Facemask.MaskTypes initialMask = Facemask.MaskTypes.Jester;
	[Export] public Facemask CurrentMask;
	[Export] public MeshInstance3D TimingCircle;

	private float targetHeight = 0;
	[Export] private float maxRadius = 4.185f;
	private float beatTimer = 0;

	[Export] private float onBeatHeight = 0f;
	[Export] private float offBeatHeight = 1f;
	[Export] private float lerpSpeed = 10f;
	private float danceBeatSpeed = 10f;

	[Export] private Area3D dancePartnerZone;

	private SphereMesh ZoneMesh => TimingCircle.Mesh as SphereMesh;

	public override void _Ready()
	{
		metronome.OnBeat += () =>
		{
			targetHeight = onBeatHeight;
			danceBeatSpeed = 50f;
		};
		metronome.OffBeat += () =>
		{
			targetHeight = offBeatHeight;
			ZoneMesh.Radius = 0;
			beatTimer = 0f;
			danceBeatSpeed = lerpSpeed;
		};

		dancePartnerZone.BodyEntered += (other) =>
		{
			if (other is Player player)
			{
				//TODO: Play good particle (hearts) if we are matched correctly and allowed to trade
				//TODO: Play bad particle (teardrops) if we are NOT a match

				GD.Print($"New Partner: {CurrentMask.Label.Text}");
				player.SetDancePartner(this);
			}
		};
		dancePartnerZone.BodyExited += (other) =>
		{
			if (other is Player player && player.CurrentDancePartner == this)
			{
				player.SetDancePartner(null);
			}
		};

		CurrentMask.MaskType = initialMask;
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = characterBody3D.Velocity;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && characterBody3D.IsOnFloor())
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
			velocity.X = Mathf.MoveToward(characterBody3D.Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(characterBody3D.Velocity.Z, 0, Speed);
		}

		characterBody3D.Velocity = velocity;

		//Adjust npc height
		float nextHeight = Mathf.Lerp(characterBody3D.Position.Y, targetHeight, (float)delta * danceBeatSpeed);
		characterBody3D.Position = characterBody3D.Position with { Y = nextHeight };

		//Adjust beat indicator
		float beatDuration = metronome.BeatDelaySeconds;
		beatTimer = Mathf.Min(beatTimer + (float)delta, beatDuration);
		float t = beatTimer / beatDuration;
		float easedT = t * t;
		ZoneMesh.Radius = Mathf.Lerp(0, maxRadius, easedT);
	}
}
