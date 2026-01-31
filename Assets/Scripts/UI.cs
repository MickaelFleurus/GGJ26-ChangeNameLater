using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    [SerializeField] public UIDocument UIDocument;


    private Label mHints;
    private Label mAmountCollected;
    private Label mLootValue;

    void Start()
    {
        mHints = UIDocument.rootVisualElement.Q<Label>("Hints");
        mAmountCollected = UIDocument.rootVisualElement.Q<VisualElement>("Collected").Q<Label>("Amount");
        mLootValue = UIDocument.rootVisualElement.Q<Label>("ObjectValue");

        mHints.visible = false;
        mLootValue.visible = false;

        mAmountCollected.text = "0";
    }

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        float rayDistance = 5f;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                mLootValue.visible = true;
                mLootValue.text = interactable.GetValue().ToString();
                return;
            }

        }
        if (mLootValue.visible)
        {
            mLootValue.visible = false;
        }

    }

}
