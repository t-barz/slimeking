# Guia de Configuração - PlayerSpawnManager

## Problema Resolvido

O `PlayerSpawnManager` resolve o problema de reposicionamento do jogador quando transitioning entre cenas através de portais.

## Como Usar

### 1. Adicionar o PlayerSpawnManager à Cena

1. Crie um GameObject vazio na cena de destino
2. Nomeie como "PlayerSpawnManager"
3. Adicione o script `PlayerSpawnManager` ao GameObject

### 2. Configuração no Inspector

- **Player Tag**: Tag do jogador (padrão: "Player")
- **Default Spawn Position**: Posição padrão se não houver destino de portal
- **Enable Debug Logs**: Ativa logs para debug (recomendado durante desenvolvimento)

### 3. Integração Automática

O `PlayerSpawnManager` funciona automaticamente:

- Verifica se há coordenadas de portal salvas nos PlayerPrefs
- Reposiciona o jogador na posição correta
- Limpa as coordenadas para evitar reutilização

### 4. Funcionamento

1. Quando o jogador entra em um portal, o `PortalManager` salva as coordenadas de destino
2. A nova cena é carregada
3. O `PlayerSpawnManager` detecta as coordenadas salvas
4. Reposiciona o jogador na posição correta
5. Limpa as coordenadas para evitar problemas futuros

## Logs de Debug

Com debug ativado, você verá mensagens como:

```
[PlayerSpawnManager] Reposicionando jogador via portal para: (10.5, 5.2, 0.0)
[PlayerSpawnManager] Jogador encontrado: Player
[PlayerSpawnManager] Posição atual: (0.0, 0.0, 0.0)
[PlayerSpawnManager] Nova posição: (10.5, 5.2, 0.0)
[PlayerSpawnManager] Posição final: (10.5, 5.2, 0.0)
```

## Resolução de Problemas

### Jogador não foi reposicionado

1. Verifique se o GameObject do jogador tem a tag "Player"
2. Confirme que o PlayerSpawnManager está presente na cena de destino
3. Ative os logs de debug para verificar o funcionamento

### Posição incorreta

1. Verifique as coordenadas definidas no portal
2. Confirme que a posição de destino está dentro dos limites da cena
3. Teste a posição manualmente no editor

### Múltiplas transições

O sistema automaticamente limpa as coordenadas após o uso, evitando reutilização indesejada.

## Integração com PortalManager

O `PortalManager` foi atualizado para trabalhar automaticamente com o `PlayerSpawnManager`:

- Usa `PlayerSpawnManager.SetPortalDestination()` para definir coordenadas
- Mantém compatibilidade com o sistema anterior de PlayerPrefs
- Fornece dupla garantia de reposicionamento

## Notas Técnicas

- O componente deve estar ativo na cena de destino antes do jogador chegar
- Utiliza PlayerPrefs para persistir coordenadas entre cenas
- Para a velocidade do Rigidbody2D antes do reposicionamento
- Thread-safe e funciona com carregamento assíncrono de cenas
