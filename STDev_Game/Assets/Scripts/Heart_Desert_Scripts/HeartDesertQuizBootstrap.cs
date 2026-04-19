using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HeartDesertQuizBootstrap : MonoBehaviour
{
    private const int RandomQuestionSignal = -100;
    private const int RetryCurrentQuestionSignal = -200;

    private enum SpeakerType
    {
        Narration,
        Player,
        Sphinx
    }

    [System.Serializable]
    private class DialogueChoice
    {
        public string choiceText;
        public int nextNodeIndex = -1;
    }

    [System.Serializable]
    private class DialogueNode
    {
        public SpeakerType speaker;
        public string speakerName;
        [TextArea(2, 5)] public string line;
        public Sprite portraitSprite;
        public bool showPortrait = true;
        public int nextNodeIndex = -1;
        public DialogueChoice[] choices;
    }

    [Header("Scene References")]
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private Transform sphinxTransform;
    [SerializeField] private SpriteRenderer sphinxRenderer;

    [Header("Dialogue UI References")]
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private Image dialogueBoxImage;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private RectTransform choicesRoot;

    [Header("Choice Button Template")]
    [SerializeField] private Button choiceButtonPrefab;

    [Header("Character Sprites")]
    [SerializeField] private Sprite playerIdleSprite;
    [SerializeField] private Sprite[] playerWalkSprites;
    [SerializeField] private Sprite playerPortraitSprite;
    [SerializeField] private Sprite sphinxNeutralPortraitSprite;
    [SerializeField] private Sprite sphinxWarmPortraitSprite;
    [SerializeField] private Sprite sphinxStrictPortraitSprite;

    [Header("Auto Walk")]
    [SerializeField] private float walkSpeed = 2.25f;
    [SerializeField] private float stopOffsetFromSphinx = 12.0f;
    [SerializeField] private float walkFrameInterval = 0.12f;
    [SerializeField] private float cameraFollowOffsetX = 4f;
    [SerializeField] private float cameraFollowSmoothness = 4f;

    [Header("Dialogue")]
    [SerializeField] private DialogueNode[] dialogueNodes;

    [Header("Question Node Indices")]
    [SerializeField] private int[] questionNodeIndices;

    private readonly List<Button> choiceButtons = new List<Button>();
    private int currentDialogueIndex = -1;
    private int currentQuestionNodeIndex = -1;
    private bool dialogueStarted;
    private bool autoWalkFinished;

    private void Awake()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

        EnsureDialogueSeedData();
        EnsureEventSystem();

        if (dialogueCanvas != null)
        {
            dialogueCanvas.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        StartCoroutine(RunIntroSequence());
    }

    private void Update()
    {
        if (!dialogueStarted)
        {
            UpdateCameraFollow();
            return;
        }

        if (currentDialogueIndex < 0 || currentDialogueIndex >= dialogueNodes.Length)
        {
            return;
        }

        DialogueNode currentNode = dialogueNodes[currentDialogueIndex];
        bool hasChoices = currentNode.choices != null && currentNode.choices.Length > 0;

        if (!hasChoices && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            AdvanceDialogue();
        }

        if (hasChoices)
        {
            for (int i = 0; i < currentNode.choices.Length && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    SelectChoice(i);
                    break;
                }
            }
        }
    }

    private bool ValidateReferences()
    {
        bool isValid = true;

        if (sceneCamera == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Scene Camera가 연결되지 않았습니다.");
            isValid = false;
        }

        if (playerTransform == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Player Transform이 연결되지 않았습니다.");
            isValid = false;
        }

        if (playerRenderer == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Player Renderer가 연결되지 않았습니다.");
            isValid = false;
        }

        if (sphinxTransform == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Sphinx Transform이 연결되지 않았습니다.");
            isValid = false;
        }

        if (dialogueCanvas == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Dialogue Canvas가 연결되지 않았습니다.");
            isValid = false;
        }

        if (portraitImage == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Portrait Image가 연결되지 않았습니다.");
            isValid = false;
        }

        if (speakerNameText == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Speaker Name Text가 연결되지 않았습니다.");
            isValid = false;
        }

        if (dialogueText == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Dialogue Text가 연결되지 않았습니다.");
            isValid = false;
        }

        if (hintText == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Hint Text가 연결되지 않았습니다.");
            isValid = false;
        }

        if (choicesRoot == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Choices Root가 연결되지 않았습니다.");
            isValid = false;
        }

        if (choiceButtonPrefab == null)
        {
            Debug.LogError("[HeartDesertQuizBootstrap] Choice Button Prefab이 연결되지 않았습니다.");
            isValid = false;
        }

        return isValid;
    }

    private IEnumerator RunIntroSequence()
    {
        float walkTimer = 0f;
        int walkFrameIndex = 0;
        float stopX = sphinxTransform.position.x - stopOffsetFromSphinx;

        while (playerTransform.position.x < stopX)
        {
            walkTimer += Time.deltaTime;

            if (walkTimer >= walkFrameInterval)
            {
                walkTimer = 0f;
                walkFrameIndex = (walkFrameIndex + 1) % Mathf.Max(1, playerWalkSprites.Length);

                if (playerWalkSprites.Length > 0)
                {
                    playerRenderer.sprite = playerWalkSprites[walkFrameIndex];
                }
            }

            Vector3 position = playerTransform.position;
            position.x = Mathf.MoveTowards(position.x, stopX, walkSpeed * Time.deltaTime);
            playerTransform.position = position;

            UpdateCameraFollow();
            yield return null;
        }

        yield return StartCoroutine(PlayStopAnimation());

        autoWalkFinished = true;
        dialogueStarted = true;

        FocusCameraOnConversation();
        BeginDialogue(0);
    }

    private IEnumerator PlayStopAnimation()
    {
        if (playerWalkSprites == null || playerWalkSprites.Length == 0)
        {
            if (playerIdleSprite != null)
            {
                playerRenderer.sprite = playerIdleSprite;
            }

            yield break;
        }

        playerRenderer.sprite = playerWalkSprites[playerWalkSprites.Length - 1];
        yield return new WaitForSeconds(0.08f);

        playerRenderer.sprite = playerWalkSprites[Mathf.Max(0, playerWalkSprites.Length - 2)];
        yield return new WaitForSeconds(0.08f);

        if (playerIdleSprite != null)
        {
            playerRenderer.sprite = playerIdleSprite;
        }
    }

    private void UpdateCameraFollow()
    {
        if (sceneCamera == null || playerTransform == null || autoWalkFinished)
        {
            return;
        }

        float targetX = playerTransform.position.x + cameraFollowOffsetX;
        Vector3 cameraPosition = sceneCamera.transform.position;
        cameraPosition.x = Mathf.Lerp(cameraPosition.x, targetX, Time.deltaTime * cameraFollowSmoothness);
        sceneCamera.transform.position = cameraPosition;
    }

    private void FocusCameraOnConversation()
    {
        if (sceneCamera == null || playerTransform == null || sphinxTransform == null)
        {
            return;
        }

        Vector3 cameraPosition = sceneCamera.transform.position;
        cameraPosition.x = (playerTransform.position.x + sphinxTransform.position.x) * 0.5f;
        sceneCamera.transform.position = cameraPosition;
    }

    private void BeginDialogue(int nodeIndex)
    {
        if (dialogueNodes == null || dialogueNodes.Length == 0)
        {
            return;
        }

        if (nodeIndex == RandomQuestionSignal)
        {
            nodeIndex = GetRandomQuestionNodeIndex();
        }

        currentDialogueIndex = Mathf.Clamp(nodeIndex, 0, dialogueNodes.Length - 1);
        ApplyNode(dialogueNodes[currentDialogueIndex]);
    }

    private void ApplyNode(DialogueNode node)
    {
        dialogueCanvas.gameObject.SetActive(true);

        if (speakerNameText != null)
        {
            speakerNameText.text = GetSpeakerName(node);
        }

        if (dialogueText != null)
        {
            dialogueText.text = node.line;
        }

        bool shouldShowPortrait = node.showPortrait && node.portraitSprite != null;

        if (portraitImage != null)
        {
            portraitImage.gameObject.SetActive(shouldShowPortrait);
            portraitImage.sprite = node.portraitSprite;
        }

        ClearChoiceButtons();

        bool hasChoices = node.choices != null && node.choices.Length > 0;
        hintText.text = hasChoices ? "숫자키 1~9 또는 버튼 클릭" : "Space / Enter";

        if (hasChoices)
        {
            for (int i = 0; i < node.choices.Length; i++)
            {
                CreateChoiceButton(node.choices[i], i);
            }
        }
    }

    private void AdvanceDialogue()
    {
        if (currentDialogueIndex < 0 || currentDialogueIndex >= dialogueNodes.Length)
        {
            return;
        }

        DialogueNode node = dialogueNodes[currentDialogueIndex];

        if (node.choices != null && node.choices.Length > 0)
        {
            return;
        }

        int targetIndex = node.nextNodeIndex;

        if (targetIndex == RetryCurrentQuestionSignal)
        {
            targetIndex = currentQuestionNodeIndex;
        }

        if (targetIndex == RandomQuestionSignal || (targetIndex >= 0 && targetIndex < dialogueNodes.Length))
        {
            BeginDialogue(targetIndex);
        }
        else
        {
            hintText.text = "여기를 눌러 나가기";
        }
    }

    private void SelectChoice(int choiceIndex)
    {
        if (currentDialogueIndex < 0 || currentDialogueIndex >= dialogueNodes.Length)
        {
            return;
        }

        DialogueNode node = dialogueNodes[currentDialogueIndex];

        if (node.choices == null || choiceIndex < 0 || choiceIndex >= node.choices.Length)
        {
            return;
        }

        DialogueChoice choice = node.choices[choiceIndex];

        if (choice.nextNodeIndex >= 0 && choice.nextNodeIndex < dialogueNodes.Length)
        {
            BeginDialogue(choice.nextNodeIndex);
        }
    }

    private void CreateChoiceButton(DialogueChoice choice, int choiceIndex)
    {
        Button button = Instantiate(choiceButtonPrefab, choicesRoot);
        button.gameObject.SetActive(true);

        TMP_Text label = button.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            label.text = (choiceIndex + 1) + ". " + choice.choiceText;
        }

        button.onClick.RemoveAllListeners();

        int cachedIndex = choiceIndex;
        button.onClick.AddListener(() => SelectChoice(cachedIndex));

        choiceButtons.Add(button);
    }

    private void ClearChoiceButtons()
    {
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (choiceButtons[i] != null)
            {
                Destroy(choiceButtons[i].gameObject);
            }
        }

        choiceButtons.Clear();
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private string GetSpeakerName(DialogueNode node)
    {
        if (!string.IsNullOrWhiteSpace(node.speakerName))
        {
            return node.speakerName;
        }

        switch (node.speaker)
        {
            case SpeakerType.Player:
                return "Player";
            case SpeakerType.Sphinx:
                return "Sphinx";
            default:
                return string.Empty;
        }
    }

    private int GetRandomQuestionNodeIndex()
    {
        if (questionNodeIndices == null || questionNodeIndices.Length == 0)
        {
            Debug.LogWarning("[HeartDesertQuizBootstrap] questionNodeIndices가 비어 있습니다. 기본 문제 인덱스 2를 사용합니다.");
            currentQuestionNodeIndex = 2;
            return currentQuestionNodeIndex;
        }

        int randomArrayIndex = Random.Range(0, questionNodeIndices.Length);
        currentQuestionNodeIndex = questionNodeIndices[randomArrayIndex];
        return currentQuestionNodeIndex;
    }

    private void EnsureDialogueSeedData()
    {
        if (dialogueNodes != null && dialogueNodes.Length > 0)
        {
            return;
        }

        dialogueNodes = new[]
        {
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "먼길을 왔군. 사막의 심장을 갖고싶다면 내 질문에 답해야 한다.",
                portraitSprite = sphinxNeutralPortraitSprite,
                nextNodeIndex = 1,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Player,
                speakerName = "Player",
                line = "좋아. 문제를 내 봐.",
                portraitSprite = playerPortraitSprite,
                nextNodeIndex = RandomQuestionSignal,
                showPortrait = true
            },

            // 문제 1 시작 인덱스 = 2
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "문제다. 사막에서 지면의 온도증가에 먼 물체가 가까이 보이는 현상의 이름을 답해라",
                portraitSprite = sphinxStrictPortraitSprite != null ? sphinxStrictPortraitSprite : sphinxNeutralPortraitSprite,
                showPortrait = true,
                choices = new[]
                {
                    new DialogueChoice { choiceText = "신기루", nextNodeIndex = 3 },
                    new DialogueChoice { choiceText = "하이루", nextNodeIndex = 5 },
                    new DialogueChoice { choiceText = "나만 해줘~", nextNodeIndex = 5 }
                }
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "정답이다. 신기루는 가짜 사막의 오아시스를 만들어내는 재밌는 현상이지",
                portraitSprite = sphinxWarmPortraitSprite != null ? sphinxWarmPortraitSprite : sphinxNeutralPortraitSprite,
                nextNodeIndex = 4,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "하트를 가져가도 좋다. 좋은 모험을 기원하지.",
                portraitSprite = sphinxWarmPortraitSprite != null ? sphinxWarmPortraitSprite : sphinxNeutralPortraitSprite,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "성급함은 사막에서 가장 위험한 선택이다. 다시 생각해 보거라.",
                portraitSprite = sphinxStrictPortraitSprite != null ? sphinxStrictPortraitSprite : sphinxNeutralPortraitSprite,
                nextNodeIndex = RetryCurrentQuestionSignal,
                showPortrait = true
            },

            // 문제 2 시작 인덱스 = 6
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "문제다. 밤 사막에서 방향을 찾을 때 전통적으로 참고하던 별은 무엇이지?",
                portraitSprite = sphinxStrictPortraitSprite != null ? sphinxStrictPortraitSprite : sphinxNeutralPortraitSprite,
                showPortrait = true,
                choices = new[]
                {
                    new DialogueChoice { choiceText = "북극성", nextNodeIndex = 7 },
                    new DialogueChoice { choiceText = "금성", nextNodeIndex = 9 },
                    new DialogueChoice { choiceText = "유성", nextNodeIndex = 9 }
                }
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "좋은 답이다. 북극성은 오래전부터 길잡이였지.",
                portraitSprite = sphinxWarmPortraitSprite != null ? sphinxWarmPortraitSprite : sphinxNeutralPortraitSprite,
                nextNodeIndex = 8,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "하트를 가져가도 좋다. 별빛이 네 길을 비춰 주길 바란다.",
                portraitSprite = sphinxWarmPortraitSprite != null ? sphinxWarmPortraitSprite : sphinxNeutralPortraitSprite,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "그 답으로는 방향을 찾기 어렵다. 다시 생각해 보거라.",
                portraitSprite = sphinxStrictPortraitSprite != null ? sphinxStrictPortraitSprite : sphinxNeutralPortraitSprite,
                nextNodeIndex = RetryCurrentQuestionSignal,
                showPortrait = true
            },

            // 문제 3 시작 인덱스 = 10
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "문제다. 사막의 낮과 밤 기온 차가 큰 가장 큰 이유는 무엇이지?",
                portraitSprite = sphinxStrictPortraitSprite != null ? sphinxStrictPortraitSprite : sphinxNeutralPortraitSprite,
                showPortrait = true,
                choices = new[]
                {
                    new DialogueChoice { choiceText = "습도가 낮아서", nextNodeIndex = 11 },
                    new DialogueChoice { choiceText = "모래가 많아서", nextNodeIndex = 13 },
                    new DialogueChoice { choiceText = "낙타가 있어서", nextNodeIndex = 13 }
                }
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "옳다. 습도가 낮아 열을 오래 붙잡지 못하지.",
                portraitSprite = sphinxWarmPortraitSprite != null ? sphinxWarmPortraitSprite : sphinxNeutralPortraitSprite,
                nextNodeIndex = 12,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "하트를 가져가도 좋다. 지혜로운 답변이었어.",
                portraitSprite = sphinxWarmPortraitSprite != null ? sphinxWarmPortraitSprite : sphinxNeutralPortraitSprite,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "핵심을 놓쳤다. 다시 생각해 보거라.",
                portraitSprite = sphinxStrictPortraitSprite != null ? sphinxStrictPortraitSprite : sphinxNeutralPortraitSprite,
                nextNodeIndex = RetryCurrentQuestionSignal,
                showPortrait = true
            },

            // 문제 4 시작 인덱스 = 14
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "문제다. 사막에서 낮 동안 기온이 매우 빠르게 상승하는 가장 큰 이유는 무엇이지?",
                portraitSprite = sphinxStrictPortraitSprite,
                showPortrait = true,
                choices = new[]
                {
                    new DialogueChoice { choiceText = "구름이 거의 없어서", nextNodeIndex = 15 },
                    new DialogueChoice { choiceText = "낙타가 많아서", nextNodeIndex = 17 },
                    new DialogueChoice { choiceText = "모래 색이 어두워서", nextNodeIndex = 17 }
                }
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "옳다. 구름이 적어 태양 복사가 그대로 지면에 도달하기 때문이지.",
                portraitSprite = sphinxWarmPortraitSprite,
                nextNodeIndex = 16,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "하트를 가져가도 좋다. 태양을 이해하는 자는 사막을 이해하지.",
                portraitSprite = sphinxWarmPortraitSprite,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "사막의 본질을 놓쳤다. 다시 생각해 보거라.",
                portraitSprite = sphinxStrictPortraitSprite,
                nextNodeIndex = RetryCurrentQuestionSignal,
                showPortrait = true
            },
            // 문제 5 시작 인덱스 = 18
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "문제다. 선인장이 잎 대신 가시를 가진 가장 중요한 이유는 무엇이지?",
                portraitSprite = sphinxStrictPortraitSprite,
                showPortrait = true,
                choices = new[]
                {
                    new DialogueChoice { choiceText = "수분 증발을 줄이기 위해", nextNodeIndex = 19 },
                    new DialogueChoice { choiceText = "빛을 더 받기 위해", nextNodeIndex = 21 },
                    new DialogueChoice { choiceText = "모래를 모으기 위해", nextNodeIndex = 21 }
                }
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "정답이다. 가시는 표면적을 줄여 수분 손실을 최소화하지.",
                portraitSprite = sphinxWarmPortraitSprite,
                nextNodeIndex = 20,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "하트를 가져가도 좋다. 생존의 지혜를 이해했구나.",
                portraitSprite = sphinxWarmPortraitSprite,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "사막의 식물은 그렇게 단순하지 않다. 다시 생각해 보거라.",
                portraitSprite = sphinxStrictPortraitSprite,
                nextNodeIndex = RetryCurrentQuestionSignal,
                showPortrait = true
            },
            // 문제 6 시작 인덱스 = 22
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "문제다. 사막의 밤 기온이 급격히 떨어지는 가장 큰 이유는 무엇이지?",
                portraitSprite = sphinxStrictPortraitSprite,
                showPortrait = true,
                choices = new[]
                {
                    new DialogueChoice { choiceText = "대기 중 수증기가 적어서", nextNodeIndex = 23 },
                    new DialogueChoice { choiceText = "모래가 차가워서", nextNodeIndex = 25 },
                    new DialogueChoice { choiceText = "바람이 멈춰서", nextNodeIndex = 25 }
                }
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "옳다. 수증기가 적으면 열이 빠르게 우주로 방출되지.",
                portraitSprite = sphinxWarmPortraitSprite,
                nextNodeIndex = 24,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "하트를 가져가도 좋다. 밤의 사막도 이해했구나.",
                portraitSprite = sphinxWarmPortraitSprite,
                showPortrait = true
            },
            new DialogueNode
            {
                speaker = SpeakerType.Sphinx,
                speakerName = "Sphinx",
                line = "사막의 공기를 더 생각해 보거라.",
                portraitSprite = sphinxStrictPortraitSprite,
                nextNodeIndex = RetryCurrentQuestionSignal,
                showPortrait = true
            },
        };

        if (questionNodeIndices == null || questionNodeIndices.Length == 0)
        {
            questionNodeIndices = new[] {2, 6, 10, 14, 18, 22};
        }
    }
}