using TMPro;
using UnityEngine;

public class ScoreField : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text;

    private void Awake()
    {
        GameManager.Instance.RegisterScoreField(this);
    }

    public void SetText(string text)
    {
        m_text.text = text;
    }
}
