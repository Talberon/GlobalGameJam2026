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
	[Export] private Node3D visuals;

	private float targetHeight = 0;
	[Export] private float maxRadius = 4.185f;
	private float beatTimer = 0;

	[Export] private float onBeatHeight = 0f;
	[Export] private float offBeatHeight = 1f;
	[Export] private float lerpSpeed = 10f;
	private float danceBeatSpeed = 10f;

	private Vector3 originalPos;
	[Export] private FastNoiseLite shakeNoise;
	[Export] private float maxShakeIntensity = 1.5f;
	public float shakeIntensity = 0f;
	public float shakeSpeed = 100f;

	[Export] private Area3D dancePartnerZone;

	[Export] public int TestsRemaining = 3;

	private SphereMesh ZoneMesh => TimingCircle.Mesh as SphereMesh;
	private const float TimingCircleTransparency = 0.5f;

	private Player? playerPartner;
	private readonly Color defaultColor = new(.29f, .38f, 1f, TimingCircleTransparency);

	[ExportGroup("Particles")] [Export] private CpuParticles3D loveParticle;
	[Export] private CpuParticles3D cryParticle;

	private bool HasTraded = false;

	public override void _Ready()
	{
		originalPos = visuals.Position;
		shakeNoise.Seed = (int)GD.Randi();
		shakeNoise.Frequency = 0.5f;
		shakeNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;

		loveParticle.Emitting = false;
		cryParticle.Emitting = false;

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
				if (CurrentMask.IsCompatibleWith(player.CurrentMask.MaskType))
				{
					GD.Print($"New Partner: {CurrentMask.Label.Text}");

					// Play good particle (hearts) if we are matched correctly and allowed to trade
					loveParticle.Emitting = true;

					playerPartner = player;
					player.SetDancePartner(this);
					TimingCircle.Visible = true;
				}
				else
				{
					// Play bad particle (teardrops) if we are NOT a match
					cryParticle.Emitting = true;
					shakeIntensity = maxShakeIntensity;
				}
			}
		};
		dancePartnerZone.BodyExited += (other) =>
		{
			if (other is Player player)
			{
				DisconnectPartner(player);
			}
		};

		TimingCircle.Visible = false;
		CurrentMask.MaskType = initialMask;
		actorPose.SpinForSeconds(3f);
		base._Ready();
	}

	private void DisconnectPartner(Player player)
	{
		playerPartner = null;
		if (player.CurrentDancePartner == this)
		{
			player.SetDancePartner(null);
		}

		loveParticle.Emitting = false;
		cryParticle.Emitting = false;

		TimingCircle.Visible = false;
	}

	private void TestWithCurrentPose(Player? player)
	{
		if (player is null) return;
		if (HasTraded) return;

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
				HasTraded = true;
				actorPose.SpinForSeconds(0.7f);
				loveParticle.Emitting = true;
				DisconnectPartner(player);
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

		if (shakeIntensity > 0)
		{
			float time = Time.GetTicksMsec() * shakeSpeed * 0.01f;
			float shakeX = shakeNoise.GetNoise2D(time, 0);
			float shakeY = shakeNoise.GetNoise2D(0, time);

			visuals.Position += new Vector3(shakeX * shakeIntensity, shakeY * shakeIntensity, 0);

			shakeIntensity = Mathf.MoveToward(shakeIntensity, 0, (float)delta * 5f);
		}
		else if (characterBody3D.Position != originalPos)
		{
			visuals.Position = originalPos;
		}
	}
}
