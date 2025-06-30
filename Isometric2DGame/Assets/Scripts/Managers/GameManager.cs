using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField]
    public UIManager UIManager { get; private set; }

    [SerializeField]
    private InputActionAsset inputActions;        //Used to get key bindings

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (UIManager == null)
        {
            // Find and assign manager components in children or scene
            UIManager = FindAnyObjectByType<UIManager>();
            if (UIManager == null)
            {
                Debug.LogWarning("UIManager not found in scene.");
            }
        }
    }

    public String GetKeyFor(string action) 
    {
        InputAction inputAction = inputActions.FindAction(action);
        String[] strings = inputAction.bindings[0].effectivePath.Split('/');

        return strings[strings.Length - 1].ToUpper();
    }
}