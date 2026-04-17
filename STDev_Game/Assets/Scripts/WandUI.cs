using UnityEngine;
using UnityEngine.UI;

public class WandUI : MonoBehaviour
{
    [Header("지팡이 UI 슬롯 연결")]
    public Image[] wandSlots;

    [Header("자물쇠 오브젝트 연결")]
    public GameObject[] lockObjects;

    void Start()
    {
        if (GameManager.Instance == null) return;

        for (int i = 0; i < 3; i++)
        {
            bool isAllowed = GameManager.Instance.allowedWands[i];

            if (isAllowed)
            {
                // 해금됨: 밝게, 자물쇠 끄기
                if (wandSlots != null && i < wandSlots.Length && wandSlots[i] != null)
                    wandSlots[i].color = Color.white;

                if (lockObjects != null && i < lockObjects.Length && lockObjects[i] != null)
                    lockObjects[i].SetActive(false);
            }
            else
            {
                // 잠김: 어둡게, 자물쇠 켜기
                if (wandSlots != null && i < wandSlots.Length && wandSlots[i] != null)
                    wandSlots[i].color = new Color(0.3f, 0.3f, 0.3f, 0.9f);

                if (lockObjects != null && i < lockObjects.Length && lockObjects[i] != null)
                    lockObjects[i].SetActive(true);
            }
        }
    }
}