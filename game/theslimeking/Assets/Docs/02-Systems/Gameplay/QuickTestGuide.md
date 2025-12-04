# Guia Rápido de Teste - Sistema de Pickup Item

## Setup Rápido (5 minutos)

### 1. Adicionar Script de Teste ao Player

1. Selecione o GameObject do Player na Hierarchy
2. `Add Component > Test Pickup System`
3. Pronto! O script já está configurado

### 2. Criar um Item de Teste

**Opção A - Usar item existente:**
- Se já tem um ItemData criado, pule para o passo 3

**Opção B - Criar novo ItemData:**
1. Project: `Assets/Data/Items/` (crie a pasta se não existir)
2. Botão direito > `Create > Inventory > Item Data`
3. Nomeie: "TestApple"
4. Configure:
   - Item Name: "Maçã de Teste"
   - Type: Consumable
   - Is Stackable: true

### 3. Criar GameObject do Item na Cena

1. Hierarchy > Botão direito > `Create Empty`
2. Nomeie: "TestItem"
3. Position: Próximo ao player (ex: 2, 0, 0)
4. `Add Component > Sprite Renderer`
   - Sprite: Qualquer sprite (ou deixe em branco por enquanto)
5. `Add Component > Pickup Item`
   - Item Data: Arraste o TestApple
   - Pause Duration: 0.5
6. `Add Component > Circle Collider 2D`
   - Is Trigger: ✓ (marque!)
   - Radius: 0.5

### 4. Executar Testes

Execute o jogo (Play) e use as teclas de teste:

#### Teste 1: Verificar Estado do Player
**Tecla: Y**
```
Esperado no Console:
=== TESTE: Estado do Player ===
Player pode se mover: ✓ SIM
Tem interação disponível: ✗ NÃO (ou ✓ SIM se estiver perto do item)
```

#### Teste 2: Aproximar do Item
1. Mova o player para perto do item (use WASD)
2. Pressione Y novamente
```
Esperado:
Tem interação disponível: ✓ SIM
Prompt: Pressione E para coletar: Maçã de Teste
```

#### Teste 3: Coletar com E (Normal)
1. Esteja próximo ao item
2. Pressione E
```
Esperado:
- Item desaparece
- Player para por 0.5s
- Item vai para o inventário
- Console mostra logs de sucesso
```

#### Teste 4: Coletar com T (Forçado)
**Tecla: T**
- Coleta o item mais próximo independente da distância
- Útil para testar sem precisar mover o player

#### Teste 5: Toggle Movimento
**Tecla: U**
- Pausa/retoma movimento manualmente
- Útil para testar se o sistema de pausa funciona

## Checklist de Verificação

Execute o jogo e verifique:

```
[ ] Ao pressionar Y, vejo logs no Console
[ ] Ao me aproximar do item, "Tem interação disponível" muda para SIM
[ ] Ao pressionar E perto do item, ele é coletado
[ ] Player para por 0.5 segundos
[ ] Player volta a se mover automaticamente
[ ] Item desaparece da cena
[ ] Item aparece no inventário (pressione I para abrir)
```

## Logs Esperados (Coleta Bem-Sucedida)

```
=== Ao iniciar o jogo ===
[PickupItem] TestItem inicializado - ItemData: Maçã de Teste

=== Ao pressionar E próximo ao item ===
[PickupItem] TryInteract chamado para TestItem
[PickupItem] Tentando adicionar Maçã de Teste ao inventário
[InventoryManager] Item 'Maçã de Teste' adicionado ao slot 0
[PickupItem] Item Maçã de Teste coletado com sucesso
[PickupItem] PausePlayerMovement chamado
[PickupItem] PlayerController encontrado, pausando movimento por 0.5s
[PickupItem] Aguardando 0.5s antes de retomar movimento
[PickupItem] Retomando movimento do player
```

## Problemas Comuns

### "Nenhum PickupItem encontrado na cena"
- Você esqueceu de criar o item de teste
- Volte ao passo 3

### "Player não encontrado"
- Seu player não tem a tag "Player"
- Selecione o player > Inspector > Tag > Player

### "Tem interação disponível: NÃO" mesmo perto do item
- Collider não está como Trigger
- Selecione o item > Circle Collider 2D > Is Trigger ✓

### "Item não desaparece ao pressionar E"
- Verifique os logs do Console
- Veja o arquivo `PickupItemTroubleshooting.md` para mais detalhes

## Próximos Passos

Depois que o teste funcionar:

1. **Remova o TestPickupSystem** do player (opcional)
2. **Crie itens reais** com sprites e dados corretos
3. **Configure múltiplos itens** na cena
4. **Teste com inventário cheio** (adicione 20 itens)
5. **Ajuste pauseDuration** para diferentes tipos de itens

## Dicas

- Mantenha o Console aberto durante os testes
- Use "Clear" no Console antes de cada teste
- Teste com diferentes valores de pauseDuration (0.1, 0.5, 1.0, 2.0)
- Teste coletar múltiplos itens seguidos
- Teste com inventário quase cheio

## Atalhos Úteis

- **Play/Stop**: Ctrl+P (Windows) / Cmd+P (Mac)
- **Pause**: Ctrl+Shift+P / Cmd+Shift+P
- **Console**: Ctrl+Shift+C / Cmd+Shift+C
- **Inspector**: Ctrl+3 / Cmd+3
