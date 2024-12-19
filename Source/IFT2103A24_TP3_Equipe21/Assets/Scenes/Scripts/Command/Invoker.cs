using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Invoker : MonoBehaviour
{
    private readonly Queue<Command> commandQueue = new();

    public void AddCommand(Command command)
    {
        commandQueue.Enqueue(command);
    }

    void Update()
    {
        if (commandQueue.Count > 0)
        {
            Command command = commandQueue.Dequeue();
            command.Execute();
        }
    }
}
