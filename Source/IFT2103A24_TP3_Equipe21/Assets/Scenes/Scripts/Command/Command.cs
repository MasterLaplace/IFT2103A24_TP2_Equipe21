public abstract class Command
{
    public virtual void Setup(params object[] args) { }
    public abstract void Execute();
    public abstract void Undo();
}
