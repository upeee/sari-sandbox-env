using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private InputActionProperty inputTriggerAction;
    [SerializeField] private InputActionProperty gripAction;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        float triggerValue = inputTriggerAction.action.ReadValue<float>();
        float gripValue = gripAction.action.ReadValue<float>();

        animator.SetFloat("Trigger", triggerValue);
        animator.SetFloat("Grip", gripValue);
    }
}
