using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static readonly Color[] PlayerColors =
    {
        new(0.859f, 0.439f, 0.439f),  // #DB7070
        new(0.439f, 0.741f, 0.859f),  // #70BDDB
        new(0.706f, 0.439f, 0.859f),  // #B470DB
        new(0.439f, 0.859f, 0.463f),  // #70DB76
    };

    [SerializeField] [Range(0, 3)] private int playerId;
    [SerializeField] private int money;

    [SerializeField] private Transform cartParentTransform;

    [Header("Character Selection")]
    [SerializeField] private Character selectedCharacter;
    [SerializeField] private int selectedSkinIndex;

    private Dictionary<ItemTemplate, int> _shoppingList = new();
    private Dictionary<ItemTemplate, int> _itemsInCart = new();
    private Dictionary<ItemTemplate, int> _boughtItems = new();

    public int PlayerId
    {
        get => playerId;
        set
        {
            playerId = Mathf.Clamp(value, 0, 3);
            UpdateCartModel();
        }
    }

    public int Money
    {
        get => money;
        set => money = value;
    }

    public Character SelectedCharacter
    {
        get => selectedCharacter;
        set => selectedCharacter = value;
    }

    public int SelectedSkinIndex
    {
        get => selectedSkinIndex;
        set => selectedSkinIndex = value;
    }

    public Dictionary<ItemTemplate, int> ShoppingList => _shoppingList;
    public Dictionary<ItemTemplate, int> ItemsInCart => _itemsInCart;

    /// <summary>
    /// Assigns a new shopping list to this player. Replaces any existing list.
    /// </summary>
    public void SetShoppingList(Dictionary<ItemTemplate, int> list)
    {
        _shoppingList.Clear();
        if (list == null) return;
        foreach (var kvp in list)
            _shoppingList[kvp.Key] = kvp.Value;
    }
    public Dictionary<ItemTemplate, int> BoughtItems => _boughtItems;

    public Color Color => PlayerColors[PlayerId];


    private void UpdateCartModel()
    {
        if (cartParentTransform == null) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.PlayerCartPrefabs == null || PlayerId >= GameManager.Instance.PlayerCartPrefabs.Length) return;

        foreach (Transform child in cartParentTransform)
        {
            Destroy(child.gameObject);
        }

        var cartPrefab = GameManager.Instance.PlayerCartPrefabs[PlayerId];
        if (cartPrefab == null) return;

        Instantiate(cartPrefab, cartParentTransform);
    }
    void Start()
    {
    }

    void Update()
    {
    }
}
