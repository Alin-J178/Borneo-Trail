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
    [Min(0f)] public float initialDelayVariation = 2f;
    [Min(0f)] public float hiddenDelayVariation = 0.5f;
    [Min(0f)] public float dangerousDelayVariation = 0.25f;

    private float instanceHiddenDelay;
    private float instanceDangerousDelay;

    private void Start()
    {
        damageZone.enabled = false;

        instanceHiddenDelay = Random.Range(
            Mathf.Max(0f, hiddenDelay - hiddenDelayVariation),
            hiddenDelay + hiddenDelayVariation
        );
        instanceDangerousDelay = Random.Range(
            Mathf.Max(0f, dangerousDelay - dangerousDelayVariation),
            dangerousDelay + dangerousDelayVariation
        );

        StartCoroutine(SpikeCycle());
    }

    private IEnumerator SpikeCycle()
    {
        yield return new WaitForSeconds(Random.Range(0f, initialDelayVariation));

        while (true)
        {
            // Wait while hidden
            damageZone.enabled = false;
            yield return new WaitForSeconds(instanceHiddenDelay);

            // Raise spikes
            animator.Play(riseClip.name, 0, 0f);
            yield return new WaitForSeconds(riseClip.length);

            // Spikes are now dangerous
            damageZone.enabled = true;
            spikeDamage.CheckForDamage();

            // Stay dangerous
            yield return new WaitForSeconds(instanceDangerousDelay);

            // Retract spikes
            damageZone.enabled = false;
            animator.Play(retractClip.name, 0, 0f);
            yield return new WaitForSeconds(retractClip.length);
        }
    }
}