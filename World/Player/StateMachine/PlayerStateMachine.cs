using System.Collections.Generic;
using Godot;

namespace Masquerade.World.Player.StateMachine;

public class PlayerStateMachine
{
    public enum States
    {
        Idle,
        Walking,
        Posing,
        //TODO: Poses
    }

    public States CurrentState
    {
        get => currentState;
        set
        {
            states[CurrentState].OnExit();
            GD.Print($"STATE: [{currentState}] -> [{value}].");
            currentState = value;
            states[CurrentState].OnEnter();
        }
    }

    private readonly Dictionary<States, IPlayerState> states = [];
    private States currentState = States.Idle;

    public PlayerStateMachine(Player player)
    {
        states.Add(States.Idle, new IdleState(this, player));
        states.Add(States.Walking, new WalkingState(this, player));
        states.Add(States.Posing, new PosingState(this, player));
    }

    public void PhysicsProcess(double delta) => states[CurrentState].Update(delta);
}