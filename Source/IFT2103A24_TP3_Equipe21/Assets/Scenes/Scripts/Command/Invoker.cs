using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Invoker : Singleton<Invoker>
{
    private readonly Queue<Command> commandQueue = new();

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        commandQueue.Clear();
    }

    public void AddCommand(Command command, params object[] args)
    {
        command.Setup(args);
        commandQueue.Enqueue(command);
    }

    public void Update()
    {
        if (commandQueue.Count > 0)
        {
            Command command = commandQueue.Dequeue();
            command.Execute();
        }
    }
}
