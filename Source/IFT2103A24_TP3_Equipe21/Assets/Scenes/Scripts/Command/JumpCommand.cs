using UnityEngine;

public class JumpCommand : Command
{
    private readonly Transform actor;

    public JumpCommand(Transform actor, params object[] args)
    {
        this.actor = actor;
    }

    public override void Execute()
    {
        SoundManager.Instance.PlaySpatialSoundEffect(actor.gameObject, SoundManager.ChooseRandomTrackSoundEffect(), 1.0f, 1.0f, 100.0f);

        _ = Pool.Instance.Get<Particle>(actor);

        // animation.SetTrigger("Jump");

        Camera.main.GetComponent<CameraShake>().Shake(0.1f, 0.1f);
    }

    public override void Undo()
    {
    }
}
