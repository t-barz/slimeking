# Guia de Implementação - Sistema de Teletransporte

## 🎯 Visão Geral

Sistema simples de teletransporte com transição visual suave, implementado em uma única classe seguindo o princípio KISS.

## 📦 O Que Será Criado

### 1. TeleportPoint.cs

**Localização:** `Assets/Code/Gameplay/TeleportPoint.cs`  
**Linhas:** ~250-300  
**Responsabilidade:** Detectar Player, executar fade, reposicionar Player e câmera

### 2. Canvas de Transição

**Hierarquia:**

```
Canvas (Screen Space - Overlay)
└── FadePanel (Image)
    - Anchor: Stretch
    - Color: Black (0,0,0,255)
    - CanvasGroup (alpha: 0)
```

### 3. Prefab TeleportPoint

**Localização:** `Assets/Prefabs/Gameplay/TeleportPoint.prefab`  
**Componentes:**

- BoxCollider2D (Is Trigger: true)
- TeleportPoint script

## 🔧 Implementação do TeleportPoint

### Estrutura Completa

```csharp
using UnityEngine;
using System.Collections;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Ponto de teletransporte que move o Player instantaneamente para um destino
    /// com transição visual suave.
    /// </summary>
    public class TeleportPoint : MonoBehaviour
    {
        #region Serialized Fields
        
        [Header("Teleport Configuration")]
        [Tooltip("Posição de destino do teletransporte")]
        [SerializeField] private Vector3 destinationPosition;
        
        [Tooltip("Tempo de espera após reposicionamento antes do fade in (segundos)")]
        [SerializeField] private float delayBeforeFadeIn = 1f;
        
        [Tooltip("Duração do fade out/in (segundos)")]
        [SerializeField] private float fadeDuration = 0.5f;
        
        [Header("Transition Visual")]
        [Tooltip("Painel de fade (CanvasGroup com Image preta fullscreen)")]
        [SerializeField] private CanvasGroup fadePanel;
        
        [Header("Debug")]
        [Tooltip("Habilita logs de debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        [Tooltip("Habilita visualização de Gizmos")]
        [SerializeField] private bool enableGizmos = true;
        
        [Tooltip("Cor do Gizmo")]
        [SerializeField] private Color gizmoColor = Color.cyan;
        
        #endregion
        
        #region Private Fields
        
        private BoxCollider2D triggerCollider;
        private bool isTeleporting = false;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Cache do BoxCollider2D
            triggerCollider = GetComponent<BoxCollider2D>();
            
            // Validação
            if (triggerCollider == null)
            {
                Debug.LogError($"TeleportPoint: BoxCollider2D não encontrado em {gameObject.name}");
            }
            else if (!triggerCollider.isTrigger)
            {
                Debug.LogWarning($"TeleportPoint: BoxCollider2D em {gameObject.name} não está marcado como Trigger. Corrigindo...");
                triggerCollider.isTrigger = true;
            }
            
            // Validação do fadePanel
            if (fadePanel == null)
            {
                Debug.LogError($"TeleportPoint: FadePanel não atribuído em {gameObject.name}");
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se é o Player
            if (!other.CompareTag("Player"))
                return;
            
            // Verifica se já está teletransportando
            if (isTeleporting)
            {
                if (enableDebugLogs)
                    Debug.Log($"TeleportPoint: Teletransporte já em andamento, ignorando colisão.");
                return;
            }
            
            // Valida antes de iniciar
            if (!ValidateTeleport())
                return;
            
            if (enableDebugLogs)
                Debug.Log($"TeleportPoint: Player detectado, iniciando teletransporte para {destinationPosition}");
            
            // Inicia teletransporte
            StartCoroutine(ExecuteTeleport());
        }
        
        #endregion
        
        #region Teleport Logic
        
        /// <summary>
        /// Executa o processo completo de teletransporte
        /// </summary>
        private IEnumerator ExecuteTeleport()
        {
            isTeleporting = true;
            
            // 1. Desabilitar controle do Player
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.DisableMovement();
                if (enableDebugLogs)
                    Debug.Log("TeleportPoint: Movimento do Player desabilitado");
            }
            
            // 2. Obter referências
            Camera mainCamera = GetMainCamera();
            if (mainCamera == null)
            {
                Debug.LogError("TeleportPoint: Câmera principal não encontrada!");
                isTeleporting = false;
                yield break;
            }
            
            // Calcular offset da câmera em relação ao Player
            Vector3 cameraOffset = mainCamera.transform.position - 
                                  PlayerController.Instance.transform.position;
            
            // 3. Fade Out (vinheta fechando)
            if (enableDebugLogs)
                Debug.Log("TeleportPoint: Iniciando fade out");
            
            yield return StartCoroutine(FadeOut());
            
            // 4. Reposicionar Player e Câmera (invisível para o jogador)
            if (enableDebugLogs)
                Debug.Log($"TeleportPoint: Reposicionando Player para {destinationPosition}");
            
            PlayerController.Instance.transform.position = destinationPosition;
            mainCamera.transform.position = destinationPosition + cameraOffset;
            
            // 5. Aguardar delay
            if (enableDebugLogs)
                Debug.Log($"TeleportPoint: Aguardando {delayBeforeFadeIn}s antes do fade in");
            
            yield return new WaitForSeconds(delayBeforeFadeIn);
            
            // 6. Fade In (vinheta abrindo)
            if (enableDebugLogs)
                Debug.Log("TeleportPoint: Iniciando fade in");
            
            yield return StartCoroutine(FadeIn());
            
            // 7. Reabilitar controle do Player
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.EnableMovement();
                if (enableDebugLogs)
                    Debug.Log("TeleportPoint: Movimento do Player reabilitado");
            }
            
            isTeleporting = false;
            
            if (enableDebugLogs)
                Debug.Log("TeleportPoint: Teletransporte concluído");
        }
        
        /// <summary>
        /// Executa fade out (escurece a tela)
        /// </summary>
        private IEnumerator FadeOut()
        {
            fadePanel.gameObject.SetActive(true);
            
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadePanel.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            
            fadePanel.alpha = 1f;
        }
        
        /// <summary>
        /// Executa fade in (clareia a tela)
        /// </summary>
        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadePanel.alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
                yield return null;
            }
            
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Obtém a câmera principal
        /// Tenta usar CameraManager se existir, caso contrário usa Camera.main
        /// </summary>
        private Camera GetMainCamera()
        {
            // Tentar usar CameraManager se existir
            // TODO: Descomentar quando CameraManager estiver implementado
            // if (CameraManager.Instance != null)
            //     return CameraManager.Instance.GetMainCamera();
            
            // Fallback para Camera.main
            return Camera.main;
        }
        
        /// <summary>
        /// Valida se o teletransporte pode ser executado
        /// </summary>
        private bool ValidateTeleport()
        {
            // Validar destino
            if (destinationPosition == Vector3.zero)
            {
                Debug.LogWarning($"TeleportPoint: Destino não configurado em {gameObject.name}");
                return false;
            }
            
            // Validar fadePanel
            if (fadePanel == null)
            {
                Debug.LogError($"TeleportPoint: FadePanel não atribuído em {gameObject.name}");
                return false;
            }
            
            // Validar PlayerController
            if (PlayerController.Instance == null)
            {
                Debug.LogError("TeleportPoint: PlayerController.Instance não encontrado!");
                return false;
            }
            
            return true;
        }
        
        #endregion
        
        #region Gizmos
        
        private void OnDrawGizmos()
        {
            if (!enableGizmos)
                return;
            
            // Desenhar área do trigger
            if (triggerCollider != null)
            {
                Gizmos.color = gizmoColor;
                Vector3 center = transform.position + (Vector3)triggerCollider.offset;
                Vector3 size = triggerCollider.size;
                Gizmos.DrawWireCube(center, size);
            }
            
            // Desenhar linha para destino
            if (destinationPosition != Vector3.zero)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(transform.position, destinationPosition);
                
                // Desenhar esfera no destino
                Gizmos.DrawWireSphere(destinationPosition, 0.5f);
                
                #if UNITY_EDITOR
                // Label no destino
                UnityEditor.Handles.Label(
                    destinationPosition + Vector3.up * 0.5f,
                    "Destino",
                    new GUIStyle()
                    {
                        normal = new GUIStyleState() { textColor = gizmoColor },
                        fontSize = 12
                    }
                );
                #endif
            }
        }
        
        #endregion
    }
}
```

## 📋 Checklist de Implementação

### Passo 1: Criar Canvas de Transição

- [ ] Criar Canvas na cena (Screen Space - Overlay)
- [ ] Adicionar Image chamada "FadePanel"
- [ ] Configurar Image:
  - Anchor: Stretch (preenche tela)
  - Color: Black (0, 0, 0, 255)
- [ ] Adicionar componente CanvasGroup
  - Alpha: 0
  - Interactable: false
  - Block Raycasts: false
- [ ] Desativar GameObject FadePanel inicialmente

### Passo 2: Criar Script TeleportPoint

- [ ] Criar arquivo `Assets/Code/Gameplay/TeleportPoint.cs`
- [ ] Copiar código acima
- [ ] Verificar namespace (ajustar se necessário)
- [ ] Salvar e compilar

### Passo 3: Criar Prefab

- [ ] Criar GameObject vazio "TeleportPoint"
- [ ] Adicionar BoxCollider2D
  - Is Trigger: ✓
  - Size: (2, 2) ou conforme necessário
- [ ] Adicionar script TeleportPoint
- [ ] Arrastar FadePanel para o campo no Inspector
- [ ] Salvar como prefab em `Assets/Prefabs/Gameplay/`

### Passo 4: Configurar Cena de Teste

- [ ] Criar nova cena "TeleportTest"
- [ ] Adicionar Player na cena
- [ ] Adicionar Canvas de transição
- [ ] Adicionar 2 TeleportPoints
- [ ] Configurar destinos:
  - TeleportPoint A → posição de B
  - TeleportPoint B → posição de A
- [ ] Atribuir FadePanel em ambos

### Passo 5: Testar

- [ ] Testar colisão do Player com TeleportPoint
- [ ] Verificar fade out/in
- [ ] Verificar reposicionamento
- [ ] Verificar que câmera segue
- [ ] Verificar controle bloqueado durante transição
- [ ] Verificar Gizmos no Editor

## 🐛 Troubleshooting

### Player não teleporta

- Verificar se GameObject tem tag "Player"
- Verificar se BoxCollider2D está como Trigger
- Verificar logs de debug (habilitar enableDebugLogs)

### Fade não aparece

- Verificar se FadePanel está atribuído
- Verificar se Canvas está ativo
- Verificar se FadePanel está na hierarquia correta

### Câmera não segue

- Verificar se Camera.main está retornando câmera correta
- Verificar cálculo do offset

### Controle não volta

- Verificar se EnableMovement() está sendo chamado
- Verificar se corrotina não está sendo interrompida

## 📊 Estimativa de Tempo

| Tarefa | Tempo Estimado |
|--------|----------------|
| Criar Canvas | 15 min |
| Criar Script | 2-3 horas |
| Criar Prefab | 15 min |
| Configurar Teste | 30 min |
| Testar e Ajustar | 1-2 horas |
| **Total** | **4-6 horas** |

## ✅ Critérios de Aceitação

- [ ] Player teleporta ao colidir com TeleportPoint
- [ ] Fade out ocorre antes do reposicionamento
- [ ] Player e câmera são reposicionados corretamente
- [ ] Delay de 1 segundo ocorre após reposicionamento
- [ ] Fade in ocorre revelando nova posição
- [ ] Controle do Player é bloqueado durante transição
- [ ] Controle é restaurado após transição
- [ ] Gizmos mostram área do trigger e destino
- [ ] Não há erros no Console
- [ ] Performance mantém 60 FPS

## 🚀 Próximos Passos Após Implementação

1. Testar em diferentes cenários
2. Ajustar timings se necessário
3. Adicionar sons de teletransporte (opcional)
4. Criar variações de efeitos (opcional)
5. Documentar uso para level designers

---

**Pronto para implementar!** 🎉
