using UnityEngine;
using UnityEngine.SceneManagement;

public class Button1Script : MonoBehaviour
{
    public void GoNext()
    {
        SceneManager.LoadScene("SelectMenu2");
    }
}
 