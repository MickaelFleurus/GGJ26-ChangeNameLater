using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    [SerializeField] public UIDocument UIDocument;

    private Button mContinueButton;
    private Button mQuitButton;

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

}
