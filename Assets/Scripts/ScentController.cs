using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentController : MonoBehaviour
{
    [Header("Generating Parameters")]
    /// <summary>
    /// Number of existing scent particles.
    /// </summary>
    [SerializeField] private int chainLength = 15;

    /// <summary>
    /// How long will scent particle exist until going back to initial location.
    /// </summary>
    [SerializeField] private float particleLifeTime = 15f;

    /// <summary>
    /// Location under the map where inactive scent particles are stored.
    /// </summary>
    [SerializeField] private Vector3 scentParticleInitialPosition;

    //[SerializeField] private bool generateSmellParticles = true;
    [SerializeField] private GameObject scentParticlePrefab;

    /// <summary>
    /// Number of scent particles, that are currently used.
    /// </summary>
    private int activeChainLength = 0;

    /// <summary>
    /// Array storing scent particles transforms.
    /// </summary>
    private Transform[] scentParticles;

    /// <summary>
    /// Parent object that contains scent particles as children.
    /// </summary>
    private GameObject scentParticlesContainer;

    private void Start()
    {
        scentParticles = new Transform[chainLength];
        scentParticlesContainer = new GameObject();
        scentParticlesContainer.name = transform.name + " Scent Container";
        for(int i = 0; i < chainLength; i++)
        {
            scentParticles[i] = Instantiate(scentParticlePrefab.transform, scentParticlesContainer.transform);
        }
    }

    private void Update()
    {
    }
}
