using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    [SerializeField] public UIDocument UIDocument;

    private Button mContinueButton;
    private Button mQuitButton;

    public VisualElement LastSelectedElement { get; set; }

    void Start()
    {
        Label score = UIDocument.rootVisualElement.Q<Label>("Amount");
        score.text = score.text.Replace("{}", Score.Instance.GetScore());
        Label time = UIDocument.rootVisualElement.Q<Label>("Time");
        time.text = time.text.Replace("{}", Score.Instance.GetDurationAsString());

        mContinueButton = UIDocument.rootVisualElement.Q<Button>("Restart");
        mQuitButton = UIDocument.rootVisualElement.Q<Button>("Exit");

        mContinueButton.clicked += OnContinue;
        mQuitButton.clicked += OnExit;

        UIDocument.rootVisualElement.RegisterCallback<NavigationMoveEvent>(OnMove);

        SetupFocusGuard(UIDocument.rootVisualElement);
        mContinueButton.schedule.Execute(() =>
       {
           mContinueButton.Focus();
       });

    }

    void OnContinue()
    {
        Debug.Log("Restart");
        SceneManager.LoadScene("MainScene");
        GameEvents.InvokeInGame();
    }

    void OnExit()
    {
        Debug.Log("Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnMove(NavigationMoveEvent evt)
    {
        if (UIDocument.rootVisualElement.focusController.focusedElement == mContinueButton)
        {
            mQuitButton.schedule.Execute(() =>
            {
                mQuitButton.Focus();
            });
        }
        else
        {
            mContinueButton.schedule.Execute(() =>
            {
                mContinueButton.Focus();
            });
        }
    }

    public void SetupFocusGuard(VisualElement root)
    {
        root.RegisterCallback<FocusInEvent>(evt =>
        {
            if (evt.target is VisualElement ve && ve.focusable)
                LastSelectedElement = ve;
        });

        var catcher = root.Q<VisualElement>("FocusFallback");

        catcher.RegisterCallback<PointerDownEvent>(_ =>
        {
            if (LastSelectedElement != null)
            {
                LastSelectedElement.schedule.Execute(() =>
                    LastSelectedElement.Focus());
            }
        });
    }

}
