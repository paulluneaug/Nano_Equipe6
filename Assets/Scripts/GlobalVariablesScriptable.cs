using UnityEngine;
using UnityUtility.Singletons;

[CreateAssetMenu(fileName = "GlobalVariablesScriptable", menuName = "Scriptable Objects/GlobalVariablesScriptable")]
public class GlobalVariablesScriptable : ScriptableSingleton<GlobalVariablesScriptable>
{
    public float ScrollSpeed => m_scrollSpeed;

    [SerializeField] private float m_scrollSpeed = 1.0f;

    protected GlobalVariablesScriptable()
    {
    }
}
