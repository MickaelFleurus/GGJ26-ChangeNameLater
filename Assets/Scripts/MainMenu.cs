using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, MenuInputs.IMenuActions
{
    [SerializeField] public UIDocument UIDocument;

    private VisualElement mMainMenu;
    private VisualElement mCreditsMenu;
    private VisualElement mCreditsView;
    private VisualElement[] mButtons;

    private MenuInputs mInputs;

    private int? mSelectedItemIndex = 0;
    private Vector2 mLastMousePosition = Vector2.zero;

    void Awake()
    {
        mInputs = new MenuInputs();

        mButtons = new VisualElement[3];
        mMainMenu = UIDocument.rootVisualElement.Q<VisualElement>("MainMenu");
        mCreditsMenu = UIDocument.rootVisualElement.Q<VisualElement>("Credits");

        mMainMenu.visible = true;
        mCreditsMenu.visible = false;
        mCreditsView = UIDocument.rootVisualElement.Q<ScrollView>("CreditText");

        mButtons[0] = mMainMenu.Q<VisualElement>("Start");
        mButtons[1] = mMainMenu.Q<VisualElement>("Credit");
        mButtons[2] = mMainMenu.Q<VisualElement>("Close");
        LoadCredits();
        UnityEngine.Cursor.visible = false;
    }

    private void LoadCredits()
    {
        TextAsset creditsFile = Resources.Load<TextAsset>("Credits");

        if (creditsFile != null)
        {
            string[] lines = creditsFile.text.Split('\n');

            foreach (string line in lines)
            {
                if (line.StartsWith("#"))
                {
                    Label titleLabel = new Label(line.Substring(2));
                    titleLabel.AddToClassList("credit_title");
                    mCreditsView.Add(titleLabel);
                }
                else if (line.StartsWith("@"))
                {
                    Label sectionLabel = new Label(line.Substring(2));
                    sectionLabel.AddToClassList("credit_section");
                    mCreditsView.Add(sectionLabel);
                }
                else
                {
                    Label creditLabel = new Label(line);
                    creditLabel.AddToClassList("credit_text");
                    mCreditsView.Add(creditLabel);
                }
            }
        }
        else
        {
            Debug.LogError("Credits file not found in Resources folder");
        }
    }

    void OnEnable()
    {
        mInputs.Menu.SetCallbacks(this);
        mInputs.Menu.Enable();
    }

    void OnDisable()
    {
        mInputs.Menu.Disable();
        mInputs.Menu.RemoveCallbacks(this);
    }

    void OnDestroy()
    {
        mInputs.Dispose();
    }

    void Start()
    {
        ResolveActive();
    }

    void Update()
    {
        if (!mMainMenu.visible) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        if (mLastMousePosition == Vector2.zero)
        {
            mLastMousePosition = mousePos;
            return;
        }

        if (mLastMousePosition == mousePos) return;
        mLastMousePosition = mousePos;
        mousePos.y = Screen.height - mousePos.y;

        UnityEngine.Cursor.visible = true;
        // Find which button is under the mouse
        for (int i = 0; i < mButtons.Length; i++)
        {
            if (mButtons[i].worldBound.Contains(mousePos))
            {
                mSelectedItemIndex = i;
                ResolveActive();
                return;
            }
        }
        mSelectedItemIndex = null;
        ResolveActive();
    }

    private void ResolveActive()
    {
        if (!mSelectedItemIndex.HasValue)
        {
            foreach (VisualElement btn in mButtons)
            {
                btn.AddToClassList("unselected");
                btn.RemoveFromClassList("selected");
                btn.RemoveFromClassList("pressed");
            }
        }

        for (int i = 0; i < mButtons.Length; i++)
        {
            if (i == mSelectedItemIndex)
            {
                mButtons[i].AddToClassList("selected");
                mButtons[i].RemoveFromClassList("unselected");
                mButtons[i].RemoveFromClassList("pressed");
            }
            else
            {
                mButtons[i].AddToClassList("unselected");
                mButtons[i].RemoveFromClassList("selected");
                mButtons[i].RemoveFromClassList("pressed");
            }

        }
    }

    public void OnUp(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        UnityEngine.Cursor.visible = false;
        if (!mSelectedItemIndex.HasValue) { mSelectedItemIndex = 0; }
        else
        {
            mSelectedItemIndex--;
            if (mSelectedItemIndex < 0)
            {
                mSelectedItemIndex = mButtons.Length - 1;
            }
        }
        ResolveActive();
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        UnityEngine.Cursor.visible = false;
        if (!mSelectedItemIndex.HasValue) { mSelectedItemIndex = 0; }
        else
        {
            mSelectedItemIndex++;
            if (mSelectedItemIndex >= mButtons.Length)
            {
                mSelectedItemIndex = 0;
            }
        }
        ResolveActive();
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!mSelectedItemIndex.HasValue || !context.performed) return;

        switch (mSelectedItemIndex)
        {
            case 0:
                SceneManager.LoadScene("MainScene");
                break;
            case 1:
                if (mMainMenu.visible)
                {
                    mMainMenu.visible = false;
                    mCreditsMenu.visible = true;
                }
                else
                {
                    mMainMenu.visible = true;
                    mCreditsMenu.visible = false;
                }
                break;
            case 2:
                CloseGame();
                break;
        }
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        switch (mSelectedItemIndex)
        {
            case 1:
                mMainMenu.visible = true;
                mCreditsMenu.visible = false;
                break;
            default:
                CloseGame();
                break;
        }
    }

    private void CloseGame()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
