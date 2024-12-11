using System.Collections;
using UnityEngine;


public class SFXController : MonoBehaviour
{
    [SerializeField] private AudioSource m_audioSource;

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
        yield return new WaitForSeconds(length);

        pool.Release(this);
    }
}