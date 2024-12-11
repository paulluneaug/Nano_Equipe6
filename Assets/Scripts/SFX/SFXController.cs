using System.Collections;
using UnityEngine;
using UnityUtility.MathU;


public class SFXController : MonoBehaviour
{
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private Vector2 m_pitchRange;

    private void Start()
    {
        m_audioSource ??= GetComponent<AudioSource>();
    }

    public void StartSFXLifeCycle(SFXControllerPool pool)
    {
        _ = StartCoroutine(ReleaseCoroutine(pool));
    }

    private IEnumerator ReleaseCoroutine(SFXControllerPool pool)
    {
        float length = m_audioSource.clip.length;
        m_audioSource.pitch = Random.value.RemapFrom01(m_pitchRange);
        yield return new WaitForSeconds(length);

        pool.Release(this);
    }
}