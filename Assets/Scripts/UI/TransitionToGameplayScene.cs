using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionToGameplayScene : MonoBehaviour
{
    public void GoToGameplayScene() //Call in animation event
    {
        SceneManager.LoadScene(2);
    }
}
