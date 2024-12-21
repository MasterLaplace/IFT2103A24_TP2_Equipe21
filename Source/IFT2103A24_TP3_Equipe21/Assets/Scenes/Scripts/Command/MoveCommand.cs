using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    private Vector3 previousPosition;
    private readonly Transform actor;

    public MoveCommand(Transform actor, params object[] args) : base($"Move_{actor.name}")
    {
        previousPosition = actor.position;
        this.actor = actor;
    }

    public override void Execute()
    {
        if (actor.TryGetComponent(out Movement mouv))
        {
            mouv.PerformAnimation();
        }
    }

    public override void Undo()
    {
        actor.position = previousPosition;
    }
}
