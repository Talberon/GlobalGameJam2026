namespace Masquerade.World.Player.StateMachine;

public interface IPlayerState
{
    public void OnEnter()
    {
    }

    public void Update(double delta);

    public void OnExit()
    {
    }
}