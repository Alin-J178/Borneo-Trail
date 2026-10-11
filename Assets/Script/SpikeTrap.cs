using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    public AnimationClip riseClip;
    public AnimationClip retractClip;
    

    [Header("Damage")]
    public Collider2D damageZone;
    public SpikeDamageZone spikeDamage;

    [Header("Timing")]
    public float hiddenDelay = 2f;
    public float dangerousDelay = 1f;

    private void Start()
    {
        damageZone.enabled = false;
        StartCoroutine(SpikeCycle());
    }

    private IEnumerator SpikeCycle()
    {
        while (true)
        {
            // Wait while hidden
            damageZone.enabled = false;
            yield return new WaitForSeconds(hiddenDelay);

            // Raise spikes
            animator.Play(riseClip.name, 0, 0f);
            yield return new WaitForSeconds(riseClip.length);

            // Spikes are now dangerous
            damageZone.enabled = true;
            spikeDamage.CheckForDamage();

            // Stay dangerous
            yield return new WaitForSeconds(dangerousDelay);

            // Retract spikes
            damageZone.enabled = false;
            animator.Play(retractClip.name, 0, 0f);
            yield return new WaitForSeconds(retractClip.length);
        }
    }
}