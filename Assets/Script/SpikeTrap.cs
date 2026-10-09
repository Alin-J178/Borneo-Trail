
using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    public AnimationClip riseClip;
    public AnimationClip retractClip;

    [Header("Timing")]
    public float hiddenDelay = 2f;
    public float dangerousDelay = 1f;

    private void Start()
    {
        StartCoroutine(SpikeCycle());
    }

    private IEnumerator SpikeCycle()
    {
        while (true)
        {
            // Wait while the spikes are hidden
            yield return new WaitForSeconds(hiddenDelay);

            // Raise the spikes
            animator.Play(riseClip.name, 0, 0f);
            yield return new WaitForSeconds(riseClip.length);

            // Keep the spikes raised
            yield return new WaitForSeconds(dangerousDelay);

            // Retract the spikes
            animator.Play(retractClip.name, 0, 0f);
            yield return new WaitForSeconds(retractClip.length);
        }
    }
}