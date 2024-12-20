using UnityEngine;

public class Particle : Poolable
{
    public ParticleSystem particle;

    // Démarre l'effet de particules avec des paramètres personnalisés.
    public override void OnUnpool()
    {
        base.OnUnpool();
        particle.Play();
    }

    // Remet les particules dans leur état initial.
    public override void OnPool()
    {
        base.OnPool();
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Update()
    {
        // Retourne automatiquement les particules au pool une fois terminées.
        if (!particle.IsAlive())
        {
            Pool.Instance.Return(this);
        }
    }
}
