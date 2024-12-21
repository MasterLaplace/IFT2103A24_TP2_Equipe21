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

    public override void Setup(params object[] args)
    {
    }

    public override void Execute()
    {
    }

    public override void Undo()
    {
        actor.position = previousPosition;
    }
}
