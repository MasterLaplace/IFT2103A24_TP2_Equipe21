using UnityEngine;

public class JumpCommand : Command
{
    private readonly Transform actor;

    public JumpCommand(Transform actor, params object[] args)
    {
        this.actor = actor;
    }

            // // Play a sound effect
            // AudioSource audio = GetComponent<AudioSource>();
            // audio.Play();

            // // Play a particle effect
            // ParticleSystem particles = GetComponent<ParticleSystem>();
            // particles.Play();

            // // Play an animation
            // Animator animator = GetComponent<Animator>();
            // animator.SetTrigger("Jump");

            // Play a screen shake
            // CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
            // cameraShake.Shake(0.1f, 0.1f);

    public override void Execute()
    {
        SoundManager.Instance.PlaySpatialSoundEffect(actor.gameObject, SoundManager.ChooseRandomTrackSoundEffect(), 1.0f, 1.0f, 100.0f);

        // animation.SetTrigger("Jump");

        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        cameraShake.Shake(0.1f, 0.1f);
    }

    public override void Undo()
    {
    }
}
