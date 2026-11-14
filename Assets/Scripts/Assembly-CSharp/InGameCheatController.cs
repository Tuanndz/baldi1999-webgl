using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameCheatController : MonoBehaviour
{
    private static InGameCheatController instance;

    // Trạng thái các cheat
    public static bool godMode = false;
    public static bool infiniteStamina = false;
    public static bool speedHack = false;
    public static bool freezeBaldi = false;
    public static bool deafBaldi = false;
    public static bool hideMobileOnPC = true;

    private static bool unlocked = false;
    private static bool showGui = false;
    private static string inputPass = "";
    private static string statusMsg = "";

    private static GUIStyle titleStyle;
    private static GUIStyle labelStyle;
    private static GUIStyle btnStyle;
    private static GUIStyle toggleStyle;
    private static GUIStyle boxStyle;
    private static GUIStyle numBtnStyle;
    private static bool stylesInitialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        if (instance != null) return;
        GameObject go = new GameObject("InGameCheatController");
        instance = go.AddComponent<InGameCheatController>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetupSceneUI());
    }

    private IEnumerator SetupSceneUI()
    {
        yield return new WaitForSeconds(0.1f);
        CreateMenuCheatButton();
    }

    private void Start()
    {
        StartCoroutine(SetupSceneUI());
    }

    // Tự động tạo nút CHEAT ngay bên dưới nút CREDITS trong MainMenu
    public void CreateMenuCheatButton()
    {
        GameObject creditsBtn = GameObject.Find("CreditsButton");
        if (creditsBtn == null) return;

        Transform parent = creditsBtn.transform.parent;
        if (parent == null) return;

        // Nếu đã có nút CheatButton rồi thì không tạo lại
        Transform existing = parent.Find("CheatButton");
        if (existing != null) return;

        // Nhân bản từ nút CreditsButton để giữ chuẩn animation, âm thanh và kích thước
        GameObject cheatBtn = Instantiate(creditsBtn, parent);
        cheatBtn.name = "CheatButton";

        RectTransform rt = cheatBtn.GetComponent<RectTransform>();
        RectTransform credRt = creditsBtn.GetComponent<RectTransform>();
        if (rt != null && credRt != null)
        {
            // Đặt vị trí ngay bên dưới CREDITS (y thấp hơn 64 đơn vị)
            rt.anchoredPosition = new Vector2(credRt.anchoredPosition.x, credRt.anchoredPosition.y - 64f);
        }

        // Đổi màu nền nút CHEAT nổi bật và xóa sprite cũ
        Image img = cheatBtn.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = null;
            img.color = new Color(0.15f, 0.75f, 0.25f, 0.95f);
        }

        // Tạo hoặc cập nhật Text hiển thị chữ "CHEAT"
        Text txt = cheatBtn.GetComponentInChildren<Text>();
        if (txt == null)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(cheatBtn.transform, false);
            RectTransform textRt = textObj.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;
            textRt.anchoredPosition = Vector2.zero;
            txt = textObj.AddComponent<Text>();
        }
        txt.text = "CHEAT";
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 36;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;

        // Gắn sự kiện click mở menu cheat
        Button btn = cheatBtn.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                showGui = true;
            });
        }
    }

    private void Update()
    {
        // Nhấn phím 'C' hoặc 'Tab' trên PC để bật/tắt Cheat Menu
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Tab))
        {
            showGui = !showGui;
        }

        // Đảm bảo nút CHEAT luôn hiện diện bên dưới CREDITS ở Menu
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu")
        {
            GameObject creditsBtn = GameObject.Find("CreditsButton");
            if (creditsBtn != null && creditsBtn.transform.parent != null)
            {
                if (creditsBtn.transform.parent.Find("CheatButton") == null)
                {
                    CreateMenuCheatButton();
                }
            }
        }

        // Xử lý tự động ẩn/hiện chuột thông minh trên PC:
        if (showGui || Time.timeScale == 0f)
        {
            // Khi mở menu Cheat hoặc khi tạm dừng (Pause): Hiện con trỏ chuột
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (currentScene == "School" || currentScene == "Secret" || currentScene == "TestRoom")
        {
            // Khi vào trong trận đấu: Tự động khóa và ẩn con trỏ chuột trên PC
            if (!Application.isMobilePlatform && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else
        {
            // Khi ở ngoài menu chính hoặc thoát trận: Luôn mở khóa con trỏ chuột
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        // Ẩn nút cảm ứng Mobile nếu đang chơi trên PC
        if (!Application.isMobilePlatform && hideMobileOnPC)
        {
            GameObject cf2 = GameObject.Find("CF2-Canvas");
            if (cf2 != null && cf2.activeSelf)
            {
                cf2.SetActive(false);
            }
        }

        // Áp dụng các cheat liên tục trong gameplay
        ApplyCheats();
    }

    private void ApplyCheats()
    {
        // 1. Can thiệp vào PlayerScript
        PlayerScript player = FindObjectOfType<PlayerScript>();
        if (player != null)
        {
            // Vô hạn thể lực (Infinite Stamina)
            if (infiniteStamina)
            {
                player.stamina = 100f;
                player.maxStamina = 100f;
            }

            // Chạy siêu nhanh (Speed Hack)
            if (speedHack)
            {
                player.runSpeed = 65f;
                player.walkSpeed = 30f;
            }

            // Bất tử (God Mode)
            if (godMode)
            {
                player.gameOver = false;
                if (player.gc != null)
                {
                    player.gc.debugMode = true;
                }
            }
        }

        // 2. Can thiệp vào BaldiScript
        BaldiScript baldi = FindObjectOfType<BaldiScript>();
        if (baldi != null)
        {
            // Đóng băng Baldi (Freeze Baldi)
            if (freezeBaldi)
            {
                baldi.speed = 0f;
                baldi.baldiWait = 999999f;
                if (baldi.agent != null && baldi.agent.isOnNavMesh)
                {
                    baldi.agent.isStopped = true;
                    baldi.agent.velocity = Vector3.zero;
                }
            }

            // Baldi điếc / Không bao giờ nghe tiếng mở cửa (Deaf Baldi)
            if (deafBaldi)
            {
                baldi.antiHearing = true;
                baldi.antiHearingTime = 999999f;
            }
        }
    }

    private void InitStyles()
    {
        if (stylesInitialized) return;

        // Tạo style chuẩn retro cho Baldi
        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(22 * GetScale()),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        titleStyle.normal.textColor = Color.yellow;

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(15 * GetScale()),
            alignment = TextAnchor.MiddleLeft
        };
        labelStyle.normal.textColor = Color.white;

        btnStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = Mathf.RoundToInt(16 * GetScale()),
            fontStyle = FontStyle.Bold
        };

        numBtnStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = Mathf.RoundToInt(18 * GetScale()),
            fontStyle = FontStyle.Bold
        };

        toggleStyle = new GUIStyle(GUI.skin.toggle)
        {
            fontSize = Mathf.RoundToInt(16 * GetScale()),
            fontStyle = FontStyle.Bold
        };
        toggleStyle.normal.textColor = Color.white;
        toggleStyle.onNormal.textColor = Color.green;

        boxStyle = new GUIStyle(GUI.skin.box);

        stylesInitialized = true;
    }

    private float GetScale()
    {
        float scale = Screen.height / 720f;
        return Mathf.Clamp(scale, 0.8f, 2.5f);
    }

    private void OnGUI()
    {
        InitStyles();
        float scale = GetScale();

        // Nút bấm nhỏ ở góc trên màn hình khi đang trong màn chơi để mở Cheat Menu
        if (!showGui && SceneManager.GetActiveScene().name == "School")
        {
            float btnW = 100 * scale;
            float btnH = 35 * scale;
            if (GUI.Button(new Rect(Screen.width - btnW - 10, 10, btnW, btnH), "⚡ CHEAT", btnStyle))
            {
                showGui = true;
            }
        }

        if (!showGui) return;

        // Vẽ hộp thoại Cheat Menu chính ở giữa màn hình
        float winW = Mathf.Min(420 * scale, Screen.width * 0.95f);
        float winH = Mathf.Min(480 * scale, Screen.height * 0.95f);
        float winX = (Screen.width - winW) / 2f;
        float winY = (Screen.height - winH) / 2f;

        GUI.Box(new Rect(winX, winY, winW, winH), "", boxStyle);
        GUI.Box(new Rect(winX + 2, winY + 2, winW - 4, winH - 4), "", boxStyle);

        GUILayout.BeginArea(new Rect(winX + 15 * scale, winY + 15 * scale, winW - 30 * scale, winH - 30 * scale));

        GUILayout.Label("⚡ BALDI 1999 CHEAT MOD ⚡", titleStyle);
        GUILayout.Space(10 * scale);

        // NẾU CHƯA NHẬP ĐÚNG MẬT KHẨU (YÊU CẦU PASS 3636)
        if (!unlocked)
        {
            GUILayout.Label("YÊU CẦU MẬT KHẨU ĐỂ MỞ KHÓA:", labelStyle);
            GUILayout.Space(5 * scale);

            // Ô hiển thị mật khẩu
            GUILayout.Box(string.IsNullOrEmpty(inputPass) ? "---" : inputPass, btnStyle, GUILayout.Height(35 * scale));

            if (!string.IsNullOrEmpty(statusMsg))
            {
                GUIStyle errStyle = new GUIStyle(labelStyle);
                errStyle.normal.textColor = Color.red;
                errStyle.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(statusMsg, errStyle);
            }
            GUILayout.Space(8 * scale);

            // Bàn phím số trên màn hình (thuận tiện cho cả Mobile touch và PC)
            string[] keys = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "XÓA", "0", "OK" };
            for (int r = 0; r < 4; r++)
            {
                GUILayout.BeginHorizontal();
                for (int c = 0; c < 3; c++)
                {
                    int idx = r * 3 + c;
                    string k = keys[idx];
                    if (GUILayout.Button(k, numBtnStyle, GUILayout.Height(36 * scale)))
                    {
                        if (k == "XÓA")
                        {
                            if (inputPass.Length > 0) inputPass = inputPass.Substring(0, inputPass.Length - 1);
                            statusMsg = "";
                        }
                        else if (k == "OK")
                        {
                            CheckPassword();
                        }
                        else
                        {
                            if (inputPass.Length < 6) inputPass += k;
                            statusMsg = "";
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10 * scale);
            if (GUILayout.Button("ĐÓNG", btnStyle, GUILayout.Height(35 * scale)))
            {
                showGui = false;
            }
        }
        else
        {
            // ĐÃ NHẬP ĐÚNG MẬT KHẨU 3636 -> HIỂN THỊ CÁC TÍNH NĂNG CHEAT
            godMode = GUILayout.Toggle(godMode, " 🛡️ Bất tử (God Mode)", toggleStyle, GUILayout.Height(32 * scale));
            infiniteStamina = GUILayout.Toggle(infiniteStamina, " ⚡ Vô hạn thể lực (Infinite Stamina)", toggleStyle, GUILayout.Height(32 * scale));
            speedHack = GUILayout.Toggle(speedHack, " 🚀 Chạy siêu nhanh (Speed Hack)", toggleStyle, GUILayout.Height(32 * scale));
            freezeBaldi = GUILayout.Toggle(freezeBaldi, " ❄️ Đóng băng Baldi (Freeze Baldi)", toggleStyle, GUILayout.Height(32 * scale));
            deafBaldi = GUILayout.Toggle(deafBaldi, " 🔇 Baldi điếc (Không nghe tiếng cửa)", toggleStyle, GUILayout.Height(32 * scale));
            hideMobileOnPC = GUILayout.Toggle(hideMobileOnPC, " 📱 Ẩn nút Mobile trên PC", toggleStyle, GUILayout.Height(32 * scale));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("ĐÓNG MENU", btnStyle, GUILayout.Height(40 * scale)))
            {
                showGui = false;
            }
        }

        GUILayout.EndArea();
    }

    private void CheckPassword()
    {
        if (inputPass == "3636")
        {
            unlocked = true;
            statusMsg = "";
        }
        else
        {
            statusMsg = "Sai mật khẩu! Vui lòng nhập lại.";
            inputPass = "";
        }
    }
}
