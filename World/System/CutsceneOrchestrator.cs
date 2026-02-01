using System.Threading.Tasks;
using Godot;
using Masquerade.World.Player;

public partial class CutsceneOrchestrator : Node3D
{
	[ExportGroup("Camera Config")] [Export]
	public Camera3D CutsceneCamera;

	[Export] public Camera3D FinalCamera;

	[Export] public Camera3D PlayerCamera;

	[ExportGroup("Points of Interest")] [Export]
	public Npc Romeo;

	[Export] public Npc Juliet;

	[Export] public Npc Sol;
	[Export] public Npc Luna;
	[Export] public Player Player;

	[ExportGroup("EclipseItems")] [Export] public MeshInstance3D Sun;
	[Export] public MeshInstance3D Moon;
	[Export] public Node3D EclipseTarget;
	[Export] public Landscape LandscapeScene;
	[Export] public ColorRect whiteoutScreen;

	[Export] public Path3D WestStairsPath;
	[Export] public Path3D EastStairsPath;
	[Export] public Path3D FinalScenePath;

	[Export] public Node3D Upstairs;

	private Node3D? followWithCamera;

	private bool RomeoDelivered;
	private bool JulietDelivered;

	public override void _Ready()
	{
		//TODO: Re-enable me when game is done
		// PlayIdentifyTargetsCutscene();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("reset"))
		{
			GetTree().ReloadCurrentScene();
		}

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

		if (followWithCamera is not null)
		{
			var offset = new Vector3(0, 10, 10);
			CutsceneCamera.GlobalTransform = CalculateViewTransform(followWithCamera.GlobalPosition, offset);
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

		ReturnCameraToPlayer(tween);

		await ToSignal(tween, Tween.SignalName.Finished);

		PlayerCamera.MakeCurrent();
	}

	private const float StairsSceneDurationSecs = 8f;

	public async void PlaySunRisesInEastCutscene()
	{
		// 1. Setup: Match the cutscene camera to the player's current view so the transition is seamless
		CutsceneCamera.GlobalTransform = PlayerCamera.GlobalTransform;
		CutsceneCamera.MakeCurrent(); // Take over the screen

		Tween tween = CreateTween();

		tween.SetParallel(false); // Run steps one after another
		tween.SetTrans(Tween.TransitionType.Cubic);
		tween.SetEase(Tween.EaseType.InOut);

		MakeActorFollowPath(tween, EastStairsPath, StairsSceneDurationSecs);
		followWithCamera = Romeo;

		ReturnCameraToPlayer(tween);

		await ToSignal(tween, Tween.SignalName.Finished);

		PlayerCamera.MakeCurrent();
		followWithCamera = null;
		RomeoDelivered = true;

		if (JulietDelivered)
		{
			PlayEndingCutscene();
		}
	}

	public async void PlayMoonRisesInWestCutscene()
	{
		// 1. Setup: Match the cutscene camera to the player's current view so the transition is seamless
		CutsceneCamera.GlobalTransform = PlayerCamera.GlobalTransform;
		CutsceneCamera.MakeCurrent(); // Take over the screen

		Tween tween = CreateTween();

		tween.SetParallel(false); // Run steps one after another
		tween.SetTrans(Tween.TransitionType.Cubic);
		tween.SetEase(Tween.EaseType.InOut);

		MakeActorFollowPath(tween, WestStairsPath, StairsSceneDurationSecs);
		followWithCamera = Juliet;

		ReturnCameraToPlayer(tween);

		await ToSignal(tween, Tween.SignalName.Finished);

		PlayerCamera.MakeCurrent();
		followWithCamera = null;
		JulietDelivered = true;

		if (RomeoDelivered)
		{
			PlayEndingCutscene();
		}
	}

	public async void PlayEndingCutscene()
	{
		GD.Print("Playing final cutscene");
	
		FinalCamera.MakeCurrent(); 
	
		// Create a tween for the movement of Romeo/Juliet on the Final Path
		Tween pathTween = CreateTween();
		MakeActorFollowPath(pathTween, FinalScenePath, 10);

		await PlayEclipseAnimation();

		// Wait for the path movement to finish (or the eclipse, since both are 10s)
		await ToSignal(pathTween, Tween.SignalName.Finished);
	
		GD.Print("The Eclipse is Complete.");
	}

	private async Task PlayEclipseAnimation()
	{
		Tween eclipseTween = CreateTween().SetParallel(true);
		eclipseTween.SetTrans(Tween.TransitionType.Quart);
		eclipseTween.SetEase(Tween.EaseType.InOut);

		var sunTarget = new Vector3(EclipseTarget.GlobalPosition.X, EclipseTarget.GlobalPosition.Y, Sun.GlobalPosition.Z);
		var moonTarget = new Vector3(EclipseTarget.GlobalPosition.X, EclipseTarget.GlobalPosition.Y, Moon.GlobalPosition.Z);

		eclipseTween.TweenProperty(Sun, "global_position", sunTarget, 10.0f);
		eclipseTween.TweenProperty(Moon, "global_position", moonTarget, 10.0f);

		var solid = new Color(1f, .69f, .35f, 1f);
		eclipseTween.TweenProperty(whiteoutScreen, "color", solid, 10f);
		
		
		await ToSignal(eclipseTween, Tween.SignalName.Finished);
	
		// Switch the scene assets
		LandscapeScene.EnableEclipseScene();

		var endingTween = CreateTween();
		var empty = new Color(1f, .69f, .35f, 0f);
		endingTween.TweenProperty(whiteoutScreen, "color", empty, 1f);
		await ToSignal(endingTween, Tween.SignalName.Finished);
	}

	private void ReturnCameraToPlayer(Tween tween)
	{
		//Return to player camera
		tween.TweenProperty(CutsceneCamera, "global_transform", PlayerCamera.GlobalTransform, 1.5f);
	}

	private void MakeActorFollowPath(Tween tween, Path3D path3D, float durationSecs)
	{
		// Animates the progress from 0 to 1 over the duration
		tween.TweenProperty(path3D.GetChild<PathFollow3D>(0), "progress_ratio", 1.0f, durationSecs)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.InOut);
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
