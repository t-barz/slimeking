# Troubleshooting: Sistema de Pickup Item

## Problema: "Não funcionou"

Quando o sistema não funciona, siga este checklist passo a passo:

### 1. Verificar Logs do Console

Ative o Console do Unity (`Window > General > Console`) e procure por mensagens com `[PickupItem]`.

**Logs esperados ao iniciar o jogo:**
```
[PickupItem] Apple_Pickup inicializado - ItemData: Maçã
```

**Logs esperados ao coletar:**
```
[PickupItem] TryInteract chamado para Apple_Pickup
[PickupItem] Tentando adicionar Maçã ao inventário
[PickupItem] Item Maçã coletado com sucesso
[PickupItem] PausePlayerMovement chamado
[PickupItem] PlayerController encontrado, pausando movimento por 0.5s
[PickupItem] Aguardando 0.5s antes de retomar movimento
[PickupItem] Retomando movimento do player
```

### 2. Checklist de Configuração

#### ✅ GameObject do Item
- [ ] GameObject existe na cena
- [ ] Está ativo (checkbox marcado no Inspector)
- [ ] Tem o componente `PickupItem` anexado
- [ ] Tem um `Collider2D` (CircleCollider2D ou BoxCollider2D)

#### ✅ Collider2D
- [ ] Propriedade "Is Trigger" está marcada
- [ ] Tamanho é adequado (radius/size > 0)
- [ ] Não está em uma layer que o player ignora

#### ✅ PickupItem Component
- [ ] `Item Data` está preenchido (não é None)
- [ ] `Quantity` é maior que 0
- [ ] `Pause Duration` é maior que 0 (ex: 0.5)

#### ✅ Player
- [ ] Tem o componente `PlayerController`
- [ ] Tem o componente `InteractionHandler`
- [ ] Tem um `Collider2D` para detectar triggers
- [ ] Está na layer correta

#### ✅ InventoryManager
- [ ] Existe na cena
- [ ] `InventoryManager.Instance` não é null
- [ ] Inventário não está cheio (tem slots vazios)

### 3. Testes Específicos

#### Teste 1: Item é detectado?
1. Execute o jogo
2. Aproxime o player do item
3. **Esperado**: Deve aparecer um prompt de interação na UI

**Se não aparecer:**
- Verifique se o `InteractionHandler` está no player
- Verifique se o Collider2D do item é Trigger
- Verifique os logs: `[InteractionHandler]`

#### Teste 2: Interação funciona?
1. Aproxime do item
2. Pressione E
3. **Esperado**: Item desaparece e vai para o inventário

**Se não funcionar:**
- Verifique os logs do Console
- Verifique se o Input System está configurado (tecla E)
- Teste com outro item

#### Teste 3: Movimento pausa?
1. Colete um item enquanto está se movendo
2. **Esperado**: Player para por 0.5s e depois volta a se mover

**Se não pausar:**
- Verifique se `pauseDuration > 0`
- Verifique os logs: deve mostrar "pausando movimento"
- Verifique se o PlayerController tem o método `SetCanMove`

### 4. Problemas Comuns

#### Problema: "Item não é detectado"
**Causa**: Collider não está como Trigger
**Solução**: 
```
1. Selecione o item
2. Inspector > Collider2D
3. Marque "Is Trigger"
```

#### Problema: "ItemData não configurado"
**Causa**: Campo Item Data está vazio
**Solução**:
```
1. Crie um ItemData: Create > Inventory > Item Data
2. Configure o ItemData
3. Arraste para o campo Item Data do PickupItem
```

#### Problema: "Inventário cheio"
**Causa**: Todos os 20 slots estão ocupados
**Solução**:
```
1. Abra o inventário (I)
2. Descarte alguns itens
3. Ou use itens consumíveis
```

#### Problema: "Player não para ao coletar"
**Causa 1**: pauseDuration = 0
**Solução**: Configure pauseDuration > 0 (ex: 0.5)

**Causa 2**: Método SetCanMove não existe
**Solução**: Verifique se o PlayerController foi atualizado com o método

#### Problema: "Movimento não retorna"
**Causa**: Corrotina foi interrompida
**Solução**: Já corrigido - a corrotina agora roda no PlayerController

### 5. Debug Avançado

#### Ativar Logs Detalhados

No Inspector do PickupItem:
- Marque "Enable Debug Logs" (se disponível)

No Console:
- Clique em "Clear" para limpar logs antigos
- Marque "Collapse" para agrupar mensagens repetidas

#### Verificar no Scene View

1. Selecione o item
2. No Scene View, deve aparecer:
   - Gizmo amarelo (esfera wireframe)
   - Label com nome do item

#### Testar Manualmente via Script

Crie um script de teste:
```csharp
using UnityEngine;
using SlimeKing.Gameplay;

public class TestPickup : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PickupItem item = FindObjectOfType<PickupItem>();
            if (item != null)
            {
                Transform player = GameObject.FindGameObjectWithTag("Player").transform;
                bool success = item.TryInteract(player);
                Debug.Log($"Teste manual: {(success ? "SUCESSO" : "FALHOU")}");
            }
        }
    }
}
```

### 6. Verificação Final

Execute este checklist completo:

```
[ ] Console não mostra erros vermelhos
[ ] Item aparece na cena
[ ] Player consegue se aproximar do item
[ ] Prompt de interação aparece
[ ] Pressionar E coleta o item
[ ] Item desaparece da cena
[ ] Item aparece no inventário
[ ] Player para por 0.5s
[ ] Player volta a se mover normalmente
```

### 7. Ainda não funciona?

Se seguiu todos os passos e ainda não funciona:

1. **Copie os logs do Console** (botão direito > Copy)
2. **Tire screenshots** do Inspector do item
3. **Verifique a versão do Unity** (deve ser compatível)
4. **Teste com um item novo** em uma cena vazia

### 8. Exemplo de Configuração Funcional

```
GameObject: Apple_Pickup
├─ Transform
│  └─ Position: (0, 0, 0)
├─ Sprite Renderer
│  └─ Sprite: apple_sprite
├─ Circle Collider 2D
│  ├─ Is Trigger: ✓
│  └─ Radius: 0.5
└─ Pickup Item
   ├─ Item Data: Apple (ItemData)
   ├─ Quantity: 1
   ├─ Pause Duration: 0.5
   ├─ Interaction Prompt: "Pressione E para coletar"
   └─ Interaction Priority: 10
```

## Contato

Se o problema persistir, forneça:
- Logs completos do Console
- Screenshots da configuração
- Versão do Unity
- Descrição detalhada do comportamento
