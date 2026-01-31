using System;
using Godot;
using Masquerade.World.Player;
using Masquerade.World.Pose;

public partial class Npc : Node3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	[Export] private CharacterBody3D characterBody3D;

	[Export] private Metronome metronome;
	[Export] private Facemask.MaskTypes initialMask = Facemask.MaskTypes.Jester;
	[Export] public Facemask CurrentMask;
	[Export] public MeshInstance3D TimingCircle;

	[Export] private ActorPose actorPose;

	private float targetHeight = 0;
	[Export] private float maxRadius = 4.185f;
	private float beatTimer = 0;

	[Export] private float onBeatHeight = 0f;
	[Export] private float offBeatHeight = 1f;
	[Export] private float lerpSpeed = 10f;
	private float danceBeatSpeed = 10f;

	[Export] private Area3D dancePartnerZone;

	[Export] public int TestsRemaining = 3;

	private SphereMesh ZoneMesh => TimingCircle.Mesh as SphereMesh;
	private const float TimingCircleTransparency = 0.5f;

	private Player? playerPartner;
	private readonly Color defaultColor = new(.29f, .38f, 1f, TimingCircleTransparency);

	[ExportGroup("Particles")] [Export] private CpuParticles3D loveParticle;
	[Export] private CpuParticles3D tearsParticle;

	public override void _Ready()
	{
		metronome.OnBeat += () =>
		{
			targetHeight = onBeatHeight;
			TestWithCurrentPose(playerPartner);
		};
		metronome.OffBeat += () =>
		{
			targetHeight = offBeatHeight;
			ZoneMesh.Radius = 0;
			beatTimer = 0f;
			danceBeatSpeed = lerpSpeed;
			if (TimingCircle.GetActiveMaterial(0) is StandardMaterial3D material)
			{
				material.AlbedoColor = defaultColor;
			}

			actorPose.CurrentPose = (ActorPose.Poses)(GD.Randi() % Enum.GetValues<ActorPose.Poses>().Length);
		};

		dancePartnerZone.BodyEntered += (other) =>
		{
			if (other is Player player)
			{
				playerPartner = player;
				//TODO: Play good particle (hearts) if we are matched correctly and allowed to trade
				//TODO: Play bad particle (teardrops) if we are NOT a match

				GD.Print($"New Partner: {CurrentMask.Label.Text}");
				player.SetDancePartner(this);
				TimingCircle.Visible = true;
			}
		};
		dancePartnerZone.BodyExited += (other) =>
		{
			if (other is Player player)
			{
				playerPartner = null;
				if (player.CurrentDancePartner == this)
				{
					player.SetDancePartner(null);
				}

				TimingCircle.Visible = false;
			}
		};

		TimingCircle.Visible = false;
		CurrentMask.MaskType = initialMask;
		base._Ready();
	}

	private void TestWithCurrentPose(Player? player)
	{
		if (player is null) return;

		if (actorPose.CurrentPose == player.ActorPose.CurrentPose)
		{
			TestsRemaining--;
			if (TimingCircle.GetActiveMaterial(0) is StandardMaterial3D material)
			{
				material.AlbedoColor = new Color(.01f, .5f, 0.01f, TimingCircleTransparency);
				//TODO: Play particle/sfx on success
			}

			if (TestsRemaining <= 0)
			{
				//TODO: Spin this dancer as they trade
				player.TradeMasksWith(this);
			}
		}
		else
		{
			if (TimingCircle.GetActiveMaterial(0) is StandardMaterial3D material)
			{
				material.AlbedoColor = new Color(.5f, .01f, 0.01f, TimingCircleTransparency);
				//TODO: Play particle/sfx on success
			}
			//TODO: Play particle/sfx on failure
		}
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

		//Adjust beat indicator
		float beatDuration = metronome.BeatDelaySeconds;
		beatTimer = Mathf.Min(beatTimer + (float)delta, beatDuration);
		float t = beatTimer / beatDuration;
		float easedT = t * t;
		ZoneMesh.Radius = Mathf.Lerp(0, maxRadius, easedT);


		// Stomp
		float arc = Mathf.Sin(t * Mathf.Pi);
		float stompArc = Mathf.Pow(arc, 1f);
		float currentY = Mathf.Lerp(onBeatHeight, offBeatHeight, stompArc);
		characterBody3D.Position = characterBody3D.Position with { Y = currentY };
	}
}
