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

    public void OnUp(InputAction.CallbackContext context) { }

    public void OnDown(InputAction.CallbackContext context) { }

    public void OnSelect(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("MainScene");
        GameEvents.InvokeInGame();
    }

    public void OnBack(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnIncrease(InputAction.CallbackContext context) { }
    public void OnLower(InputAction.CallbackContext context) { }
}
