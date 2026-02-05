using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class EndScene : MonoBehaviour, MenuInputs.IMenuActions
{
    [SerializeField] public UIDocument UIDocument;
    private MenuInputs mInputs;
    void Start()
    {
        mInputs = new MenuInputs();
        mInputs.Menu.SetCallbacks(this);
        mInputs.Enable();

        Label score = UIDocument.rootVisualElement.Q<VisualElement>("Content").Q<Label>("Amount");
        score.text = score.text.Replace("{}", Score.Instance.GetScore());
        Label time = UIDocument.rootVisualElement.Q<VisualElement>("Content").Q<Label>("Time");
        time.text = time.text.Replace("{}", Score.Instance.GetDurationAsString());
    }
    void OnDestroy()
    {
        mInputs.Disable();

        mInputs.Dispose();
    }


    void MenuInputs.IMenuActions.OnSelect(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("MainScene");
        GameEvents.InvokeInGame();
    }

    void MenuInputs.IMenuActions.OnBack(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void MenuInputs.IMenuActions.OnMove(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
