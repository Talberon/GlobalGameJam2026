using Godot;
using Masquerade.World.Player;

public partial class CutsceneOrchestrator : Node3D
{
	[ExportGroup("Camera Config")] [Export]
	public Camera3D CutsceneCamera;

	[Export] public Camera3D PlayerCamera;

	[ExportGroup("Points of Interest")] [Export]
	public Npc Romeo;

	[Export] public Npc Juliet;

	[Export] public Npc Sol;
	[Export] public Npc Luna;
	[Export] public Player Player;

	[Export] public Node3D WestStairs;
	[Export] public Node3D EastStairs;

	[Export] public Node3D Upstairs;
	//TODO: Balcony

	public override void _Ready()
	{
		PlayIdentifyTargetsCutscene();
	}

	public override void _Process(double delta)
	{
		//Debug cutscenes
		if (Input.IsActionJustPressed("debug_cutscene1"))
		{
			PlayIdentifyTargetsCutscene();
		}

		if (Input.IsActionJustPressed("debug_cutscene2"))
		{
			PlaySunRisesInEastCutscene();
		}

		if (Input.IsActionJustPressed("debug_cutscene3"))
		{
			PlayMoonRisesInWestCutscene();
		}

		if (Input.IsActionJustPressed("debug_cutscene4"))
		{
			PlayEndingCutscene();
		}

		base._Process(delta);
	}

	public async void PlayIdentifyTargetsCutscene()
	{
		// 1. Setup: Match the cutscene camera to the player's current view so the transition is seamless
		CutsceneCamera.GlobalTransform = PlayerCamera.GlobalTransform;
		CutsceneCamera.MakeCurrent(); // Take over the screen

		Tween tween = CreateTween();

		tween.SetParallel(false); // Run steps one after another
		tween.SetTrans(Tween.TransitionType.Cubic);
		tween.SetEase(Tween.EaseType.InOut);

		LookAtNpc(tween, Romeo);
		LookAtNpc(tween, Sol);
		LookAtNpc(tween, Juliet);
		LookAtNpc(tween, Luna);
		LookAtNpc(tween, Player);

		//Return to player camera
		tween.TweenProperty(CutsceneCamera, "global_transform", PlayerCamera.GlobalTransform, 1.5f);

		await ToSignal(tween, Tween.SignalName.Finished);

		PlayerCamera.MakeCurrent();
	}

	public void PlaySunRisesInEastCutscene()
	{
		//TODO Use Tweens to move camera and then release to character camera
	}

	public void PlayMoonRisesInWestCutscene()
	{
		//TODO Use Tweens to move camera and then release to character camera
	}

	public void PlayEndingCutscene()
	{
		//TODO Use Tweens to move camera and end the game
	}

	private void LookAtNpc(Tween tween, Node3D actor)
	{
		Transform3D npcView = CalculateViewTransform(actor.GlobalPosition, new Vector3(0, 10, 10));
		double durationSec = 0.5f;
		tween.TweenProperty(CutsceneCamera, "global_transform", npcView, durationSec);
		tween.TweenInterval(1.0f); // Hover for a second
	}

	private Transform3D CalculateViewTransform(Vector3 targetPos, Vector3 offset)
	{
		Vector3 eye = targetPos + offset;
		return Transform3D.Identity.LookingAt(targetPos - eye, Vector3.Up).Translated(eye);
	}

	//FIXME
	public void TiltTowardsPlayer(Tween tween, Vector3 cameraPos, Vector3 lookAtThis)
	{
		// 1. Calculate the direction from camera to player
		// Note: In Godot, the camera looks down the -Z axis.
		Vector3 forward = (lookAtThis - cameraPos).Normalized();

		// 2. Calculate the Right vector (perpendicular to forward and world up)
		Vector3 right = Vector3.Up.Cross(forward).Normalized();

		// 3. Calculate the local Up vector (perpendicular to forward and right)
		Vector3 up = forward.Cross(right).Normalized();

		// 4. Create the Basis (Rotation matrix)
		// We map our vectors to the X, Y, and Z axes
		Basis rotationBasis = new Basis(right, up, -forward);

		// 5. Create the full Transform
		Transform3D targetTransform = new Transform3D(rotationBasis, cameraPos);

		// 6. Tween the camera to this specific rotation
		tween.TweenProperty(CutsceneCamera, "global_transform", targetTransform, 1.0f)
			.SetTrans(Tween.TransitionType.Sine);
	}
}
