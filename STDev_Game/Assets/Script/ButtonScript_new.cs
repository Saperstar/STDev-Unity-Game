using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript_new : MonoBehaviour
{
    public void GoNext()
    {
        SceneManager.LoadScene("SelectMenu2");  
    }
}
