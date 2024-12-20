using UnityEngine;

public class JumpCommand : Command
{
    private readonly Transform actor;
    private readonly float jumpForce = 10.0f;

    public JumpCommand(Transform actor, params object[] args)
    {
        if (args.Length > 0 && args[0] is float v)
        {
            jumpForce = v;
        }

        this.actor = actor;
    }

    public override void Execute()
    {
        SoundManager.Instance.PlaySpatialSoundEffect(actor.gameObject, SoundManager.ChooseRandomTrackSoundEffect(), 1.0f, 1.0f, 100.0f);

        _ = Pool.Instance.Get<Particle>(actor);

        if (actor.TryGetComponent(out Jumping jumping))
        {
            jumping.PerformAnimation(jumpForce);
        }

        Camera.main.GetComponent<CameraShake>().Shake(0.1f, 0.1f);
    }

    public override void Undo()
    {
    }
}
