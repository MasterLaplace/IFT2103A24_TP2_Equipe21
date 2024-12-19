using UnityEngine;

public class JumpCommand : Command
{
    private Vector3 previousPosition;
    private AudioSource audioEffect;
    private ParticleSystem particleEffect;
    private new Animator animation;
    private Transform actor;
    private float jumpHeight;

    public JumpCommand(Transform actor, params object[] args)
    {
        if (args.Length > 0 && args[0] is float jumpHeight)
        {
            previousPosition = actor.position;
            this.jumpHeight = jumpHeight;
        }
        if (args.Length > 1 && args[1] is AudioSource audio)
        {
            audioEffect = audio;
        }

        if (args.Length > 2 && args[2] is ParticleSystem particles)
        {
            particleEffect = particles;
        }

        if (args.Length > 3 && args[3] is Animator animator)
        {
            animation = animator;
        }
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
        audioEffect.Play();

        particleEffect.Play();

        // animation.SetTrigger("Jump");

        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        cameraShake.Shake(0.1f, 0.1f);

        actor.Translate(Vector3.up * jumpHeight);
    }

    public override void Undo()
    {
        actor.position = previousPosition;
    }
}