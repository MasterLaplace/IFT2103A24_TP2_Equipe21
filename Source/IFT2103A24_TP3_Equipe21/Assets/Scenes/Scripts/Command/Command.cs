public abstract class Command
{
    public string name;

    public Command(string name)
    {
        this.name = name;
    }
    public virtual void Setup(params object[] args) { }
    public abstract void Execute();
    public abstract void Undo();
}
