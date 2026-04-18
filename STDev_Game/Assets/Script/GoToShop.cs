using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 관리하는 라이브러리

public class GoToShop : MonoBehaviour
{
    // 버튼을 눌렀을 때 실행될 함수입니다.
    public void ChangeScene()
    {
        // "ShopMenu"라는 이름의 씬으로 이동합니다.
        // 이동하려는 씬의 이름이 'ShopMenu'가 맞는지 꼭 확인하세요!
        SceneManager.LoadScene("Shop1Menu");
        
        // 일시정지(TimeScale = 0) 상태에서 씬을 이동할 경우를 대비해 
        // 시간을 다시 흐르게(1) 설정합니다.
        Time.timeScale = 1f;
    }
}