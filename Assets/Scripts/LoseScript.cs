using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScript : MonoBehaviour
{
    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Enemy"))
        {
            RestartCurrentScene();
        }
    }

    public void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
