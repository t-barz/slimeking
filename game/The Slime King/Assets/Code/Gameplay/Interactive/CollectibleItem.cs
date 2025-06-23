using UnityEngine;
using TheSlimeKing.Core.Inventory;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Objeto interativo que pode ser coletado e adicionado ao inventário
    /// </summary>
    public class CollectibleItem : InteractableObject
    {
        [Header("Configurações de Item")]
        [SerializeField] private ItemData _itemData;
        [SerializeField] private int _quantity = 1;

        [Header("Efeitos")]
        [SerializeField] private GameObject _collectEffectPrefab;
        [SerializeField] private AudioClip _collectSound;
        [SerializeField] private float _collectSoundVolume = 1f;

        protected override void Awake()
        {
            base.Awake();

            // Configura sprite do item, se houver
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null && _itemData != null)
            {
                spriteRenderer.sprite = _itemData.Icon;
            }
        }

        public override void Interact(GameObject interactor)
        {
            // Chama método base para eventos
            base.Interact(interactor);

            // Tenta adicionar ao inventário
            if (InventoryManager.Instance != null)
            {
                bool addedSuccessfully = InventoryManager.Instance.AddItem(_itemData, _quantity);

                if (addedSuccessfully)
                {
                    // Efeito de coleta
                    if (_collectEffectPrefab != null)
                    {
                        Instantiate(_collectEffectPrefab, transform.position, Quaternion.identity);
                    }

                    // Som de coleta
                    if (_collectSound != null)
                    {
                        AudioSource.PlayClipAtPoint(_collectSound, transform.position, _collectSoundVolume);
                    }

                    // Mensagem para o jogador
                    Debug.Log($"Coletado: {_quantity}x {_itemData.NameKey}");

                    // Destroi o objeto após coletar
                    Destroy(gameObject);
                }
                else
                {
                    // Feedback de inventário cheio
                    Debug.Log("Inventário cheio!");
                }
            }
            else
            {
                Debug.LogError("InventoryManager não encontrado!");
            }
        }

        /// <summary>
        /// Retorna o prompt de interação com o nome do item
        /// </summary>
        public override string GetInteractionPrompt()
        {
            if (_itemData != null)
            {
                return $"Pressione E para coletar {_itemData.NameKey}";
            }

            return base.GetInteractionPrompt();
        }

        /// <summary>
        /// Configura o item a ser coletado
        /// </summary>
        public void SetItem(ItemData itemData, int quantity = 1)
        {
            _itemData = itemData;
            _quantity = Mathf.Max(1, quantity);

            // Atualiza sprite
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null && _itemData != null)
            {
                spriteRenderer.sprite = _itemData.Icon;
            }
        }
    }
}
