using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class TitleScreen : MonoBehaviour
{
    [SerializeField] public UIDocument titleScreenDocument;
    private ScrollView mCreditsView;
    private VisualElement mMainMenuButtons;
    private VisualElement mCreditsButtons;
    private VisualElement mOptionsButtons;

    private VisualElement mCreditsParent;
    private VisualElement mOptionsParent;

    private Button mMainMenuFocusedButton;
    private Button mCreditsFocusedButton;
    private Button mOptionsFocusedButton;

    private OptionsPanel mOptionPanel;

    void Awake()
    {
        mCreditsView = titleScreenDocument.rootVisualElement.Q<ScrollView>("CreditsView");

        mCreditsParent = titleScreenDocument.rootVisualElement.Q<VisualElement>("Credits");
        mOptionsParent = titleScreenDocument.rootVisualElement.Q<VisualElement>("Options");
        mMainMenuButtons = titleScreenDocument.rootVisualElement.Q<VisualElement>("MainMenuButtons");
        mCreditsButtons = titleScreenDocument.rootVisualElement.Q<VisualElement>("CreditsButtons");
        mOptionsButtons = titleScreenDocument.rootVisualElement.Q<VisualElement>("OptionsButtons");

        mMainMenuFocusedButton = titleScreenDocument.rootVisualElement.Q<Button>("Start");
        mCreditsFocusedButton = titleScreenDocument.rootVisualElement.Q<Button>("BackOptions");
        mOptionsFocusedButton = titleScreenDocument.rootVisualElement.Q<Button>("BackCredits");
        mOptionPanel = new OptionsPanel(titleScreenDocument.rootVisualElement.Q<VisualElement>("Options"), titleScreenDocument.rootVisualElement.Q<Button>("Apply"));
        mOptionPanel.CanCloseOptions += BackToTitle;
        mMainMenuFocusedButton.Focus();
        mMainMenuFocusedButton.clicked += StartGame;
        titleScreenDocument.rootVisualElement.Q<Button>("Exit").clicked += CloseGame;
        titleScreenDocument.rootVisualElement.Q<Button>("OptionsButton").clicked += ShowOptions;
        titleScreenDocument.rootVisualElement.Q<Button>("OptionsButton").clicked += mOptionPanel.Show;
        titleScreenDocument.rootVisualElement.Q<Button>("CreditsButton").clicked += ShowCredits;
        mCreditsFocusedButton.clicked += BackToTitle;
        mOptionsFocusedButton.clicked += BackToTitle;


        LoadCredits();
    }

    void Start()
    {
        SoundManager.Instance.StartMainMenuMusic();
    }

    private void StartGame()
    {
        SceneManager.LoadScene("MainScene");
        GameEvents.InvokeInGame();
    }

    private void ShowCredits()
    {
        mMainMenuButtons.style.display = DisplayStyle.None;
        mOptionsButtons.style.display = DisplayStyle.None;
        mCreditsButtons.style.display = DisplayStyle.Flex;

        mOptionsParent.style.display = DisplayStyle.None;
        mCreditsParent.style.display = DisplayStyle.Flex;
        mCreditsFocusedButton.Focus();
    }

    private void ShowOptions()
    {
        mMainMenuButtons.style.display = DisplayStyle.None;
        mOptionsButtons.style.display = DisplayStyle.Flex;
        mCreditsButtons.style.display = DisplayStyle.None;

        mOptionsParent.style.display = DisplayStyle.Flex;
        mCreditsParent.style.display = DisplayStyle.None;
        mOptionsFocusedButton.Focus();
    }

    private void BackToTitle()
    {
        mMainMenuButtons.style.display = DisplayStyle.Flex;
        mOptionsButtons.style.display = DisplayStyle.None;
        mCreditsButtons.style.display = DisplayStyle.None;

        mOptionsParent.style.display = DisplayStyle.None;
        mCreditsParent.style.display = DisplayStyle.None;
        mOptionsFocusedButton.Focus();
    }

    private void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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


}
