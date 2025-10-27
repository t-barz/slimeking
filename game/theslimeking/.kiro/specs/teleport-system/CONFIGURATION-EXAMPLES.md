# Exemplos de Configuração - Sistema de Teletransporte

Este documento fornece exemplos práticos de configuração para diferentes cenários de uso do Sistema de Teletransporte.

## Índice

1. [Configuração Básica](#configuração-básica)
2. [Teletransporte Bidirecional](#teletransporte-bidirecional)
3. [Hub Central com Múltiplos Destinos](#hub-central-com-múltiplos-destinos)
4. [Teletransporte Vertical (Plataformas)](#teletransporte-vertical-plataformas)
5. [Teletransporte com Área Grande](#teletransporte-com-área-grande)
6. [Teletransporte Rápido (Sem Delay)](#teletransporte-rápido-sem-delay)
7. [Teletransporte Lento (Dramático)](#teletransporte-lento-dramático)
8. [Teletransporte com Debug Ativo](#teletransporte-com-debug-ativo)
9. [Teletransporte Secreto](#teletransporte-secreto)
10. [Teletransporte em Dungeon](#teletransporte-em-dungeon)

---

## Configuração Básica

**Cenário:** Teletransporte simples de um ponto A para um ponto B.

### TeleportPoint - "Entrada da Caverna"

```
Transform:
  Position: (0, 0, 0)
  Rotation: (0, 0, 0)
  Scale: (1, 1, 1)

TeleportPoint Component:
  Teleport Configuration:
    Destination Position: (20, 0, 0)
    Transition Effect: CircleEffect
    Delay Before Fade In: 1

  Trigger Configuration:
    Trigger Size: (1, 1)
    Trigger Offset: (0, 0)

  Debug:
    Enable Debug Logs: false
    Enable Gizmos: true
    Gizmo Color: Cyan (0, 1, 1, 1)

BoxCollider2D:
  Is Trigger: ✓
  Size: (1, 1)
  Offset: (0, 0)
```

**Resultado:** Player que entrar na posição (0, 0) será teletransportado para (20, 0) com transição suave.

---

## Teletransporte Bidirecional

**Cenário:** Dois pontos que permitem ir e voltar entre duas áreas.

### TeleportPoint A - "Floresta para Caverna"

```
Transform:
  Position: (0, 0, 0)

TeleportPoint Component:
  Destination Position: (50, 10, 0)
  Transition Effect: CircleEffect
  Delay Before Fade In: 1
  Trigger Size: (2, 2)
  Gizmo Color: Green (0, 1, 0, 1)
```

### TeleportPoint B - "Caverna para Floresta"

```
Transform:
  Position: (50, 10, 0)

TeleportPoint Component:
  Destination Position: (0, 0, 0)
  Transition Effect: CircleEffect
  Delay Before Fade In: 1
  Trigger Size: (2, 2)
  Gizmo Color: Green (0, 1, 0, 1)
```

**Dica:** Use a mesma cor de Gizmo para pontos conectados para facilitar visualização.

---

## Hub Central com Múltiplos Destinos

**Cenário:** Área central com 4 portais para diferentes níveis.

### Hub - Centro (0, 0, 0)

#### Portal Norte - "Para Montanhas"

```
Position: (0, 3, 0)
Destination: (0, 50, 0)
Trigger Size: (1.5, 2)
Gizmo Color: Blue (0, 0, 1, 1)
```

#### Portal Sul - "Para Deserto"

```
Position: (0, -3, 0)
Destination: (0, -50, 0)
Trigger Size: (1.5, 2)
Gizmo Color: Yellow (1, 1, 0, 1)
```

#### Portal Leste - "Para Floresta"

```
Position: (3, 0, 0)
Destination: (50, 0, 0)
Trigger Size: (2, 1.5)
Gizmo Color: Green (0, 1, 0, 1)
```

#### Portal Oeste - "Para Oceano"

```
Position: (-3, 0, 0)
Destination: (-50, 0, 0)
Trigger Size: (2, 1.5)
Gizmo Color: Cyan (0, 1, 1, 1)
```

### Retornos ao Hub

Cada área tem um portal de retorno:

```
Montanhas (0, 50, 0) → Hub (0, 1, 0)
Deserto (0, -50, 0) → Hub (0, -1, 0)
Floresta (50, 0, 0) → Hub (1, 0, 0)
Oceano (-50, 0, 0) → Hub (-1, 0, 0)
```

**Nota:** Os destinos de retorno são ligeiramente deslocados do centro para evitar que o Player caia imediatamente em outro portal.

---

## Teletransporte Vertical (Plataformas)

**Cenário:** Teletransporte entre andares de uma torre.

### Andar 1 para Andar 2

```
Position: (10, 0, 0)
Destination: (10, 20, 0)
Trigger Size: (2, 0.5)
Trigger Offset: (0, 0)
Delay Before Fade In: 0.5
Gizmo Color: Magenta (1, 0, 1, 1)
```

### Andar 2 para Andar 3

```
Position: (10, 20, 0)
Destination: (10, 40, 0)
Trigger Size: (2, 0.5)
Trigger Offset: (0, 0)
Delay Before Fade In: 0.5
Gizmo Color: Magenta (1, 0, 1, 1)
```

### Andar 3 para Andar 1 (Descida Rápida)

```
Position: (15, 40, 0)
Destination: (10, 0, 0)
Trigger Size: (1, 1)
Delay Before Fade In: 0.3
Gizmo Color: Red (1, 0, 0, 1)
```

**Dica:** Use Trigger Size horizontal (2, 0.5) para plataformas, facilitando a ativação ao andar sobre elas.

---

## Teletransporte com Área Grande

**Cenário:** Portal grande que cobre uma área ampla (ex: entrada de castelo).

```
Transform:
  Position: (0, 0, 0)

TeleportPoint Component:
  Destination Position: (100, 50, 0)
  Transition Effect: CircleEffect
  Delay Before Fade In: 1.5

  Trigger Configuration:
    Trigger Size: (5, 8)
    Trigger Offset: (0, 0)

  Debug:
    Enable Gizmos: true
    Gizmo Color: Purple (0.5, 0, 0.5, 1)
```

**Uso:** Ideal para:

- Entradas de castelos
- Portões grandes
- Áreas de transição entre biomas

---

## Teletransporte Rápido (Sem Delay)

**Cenário:** Transição instantânea para ação rápida.

```
Destination Position: (30, 0, 0)
Transition Effect: CircleEffect
Delay Before Fade In: 0
Trigger Size: (1, 1)
```

**Uso:** Ideal para:

- Teletransportes de combate
- Mecânicas de dash
- Transições rápidas em puzzles

**Aviso:** Pode ser muito abrupto. Teste cuidadosamente.

---

## Teletransporte Lento (Dramático)

**Cenário:** Transição lenta para momentos importantes.

```
Destination Position: (0, 100, 0)
Transition Effect: CircleEffect
Delay Before Fade In: 3
Trigger Size: (2, 2)
Gizmo Color: Gold (1, 0.84, 0, 1)
```

**Uso:** Ideal para:

- Entrada em boss rooms
- Descoberta de áreas secretas
- Momentos narrativos importantes

---

## Teletransporte com Debug Ativo

**Cenário:** Configuração para desenvolvimento e troubleshooting.

```
Destination Position: (25, 15, 0)
Transition Effect: CircleEffect
Delay Before Fade In: 1

Debug:
  Enable Debug Logs: true
  Enable Gizmos: true
  Gizmo Color: Red (1, 0, 0, 1)
```

**Logs Esperados no Console:**

```
TeleportPoint 'TeleportPoint_Test' inicializado com sucesso.
TeleportPoint: Player detectado, iniciando teletransporte para (25, 15, 0)
TeleportPoint: Iniciando teletransporte para (25, 15, 0)
TeleportTransitionHelper: Iniciando transição com efeito 'CircleEffect'
TeleportTransitionHelper: Material aplicado - CircleEffect (Instance)
TeleportTransitionHelper: Executando fade out...
TeleportTransitionHelper: Fade out completo
TeleportTransitionHelper: Executando callback de reposicionamento...
TeleportPoint: Offset da câmera calculado: (0, 0, -10)
TeleportPoint: Player reposicionado de (0, 0, 0) para (25, 15, 0)
TeleportPoint: Câmera reposicionada para (25, 15, -10)
TeleportTransitionHelper: Callback completo
TeleportTransitionHelper: Aguardando 1s antes do fade in...
TeleportTransitionHelper: Executando fade in...
TeleportTransitionHelper: Fade in completo
TeleportTransitionHelper: Transição completa!
TeleportPoint: Teletransporte completo!
```

---

## Teletransporte Secreto

**Cenário:** Portal escondido com área de ativação pequena.

```
Transform:
  Position: (45, 12, 0)

TeleportPoint Component:
  Destination Position: (100, 100, 0)
  Transition Effect: CircleEffect
  Delay Before Fade In: 2

  Trigger Configuration:
    Trigger Size: (0.5, 0.5)
    Trigger Offset: (0, 0)

  Debug:
    Enable Debug Logs: false
    Enable Gizmos: false
    Gizmo Color: Black (0, 0, 0, 1)
```

**Dica:** Desabilite Gizmos para manter o segredo durante desenvolvimento. Ative apenas quando precisar editar.

---

## Teletransporte em Dungeon

**Cenário:** Sistema de teletransporte para dungeon com múltiplas salas.

### Sala 1 - Entrada

```
Position: (0, 0, 0)
Name: "Dungeon_Entrance_To_Room2"
Destination: (20, 0, 0)
Trigger Size: (1.5, 2)
Gizmo Color: White (1, 1, 1, 1)
```

### Sala 2 - Corredor

```
Position: (20, 0, 0)
Name: "Dungeon_Room2_To_Room3"
Destination: (40, 0, 0)
Trigger Size: (1.5, 2)
Gizmo Color: White (1, 1, 1, 1)
```

### Sala 3 - Boss Room

```
Position: (40, 0, 0)
Name: "Dungeon_Room3_To_BossRoom"
Destination: (60, 20, 0)
Trigger Size: (2, 2)
Delay Before Fade In: 2
Gizmo Color: Red (1, 0, 0, 1)
```

### Boss Room - Saída

```
Position: (60, 25, 0)
Name: "Dungeon_BossRoom_Exit"
Destination: (0, -5, 0)
Trigger Size: (2, 2)
Delay Before Fade In: 1.5
Gizmo Color: Green (0, 1, 0, 1)
```

**Organização na Hierarquia:**

```
Dungeon/
├── Teleports/
│   ├── Entrance_To_Room2
│   ├── Room2_To_Room3
│   ├── Room3_To_BossRoom
│   └── BossRoom_Exit
└── Rooms/
    ├── Room1_Entrance
    ├── Room2_Corridor
    ├── Room3_PreBoss
    └── Room4_BossRoom
```

---

## Configuração com Offset de Trigger

**Cenário:** Trigger deslocado para ativação em posição específica.

### Porta Elevada

```
Transform:
  Position: (10, 5, 0)

TeleportPoint Component:
  Destination Position: (30, 5, 0)
  
  Trigger Configuration:
    Trigger Size: (1, 2)
    Trigger Offset: (0, 1)
```

**Resultado:** O trigger fica 1 unidade acima do GameObject, ideal para portas elevadas.

### Plataforma com Trigger Abaixo

```
Transform:
  Position: (15, 10, 0)

TeleportPoint Component:
  Destination Position: (35, 10, 0)
  
  Trigger Configuration:
    Trigger Size: (2, 0.5)
    Trigger Offset: (0, -0.5)
```

**Resultado:** O trigger fica abaixo da plataforma, ativando quando o Player pisa nela.

---

## Tabela de Referência Rápida

| Cenário | Trigger Size | Delay | Gizmo Color | Notas |
|---------|-------------|-------|-------------|-------|
| Básico | (1, 1) | 1s | Cyan | Configuração padrão |
| Porta | (1.5, 2) | 1s | White | Vertical, fácil de entrar |
| Plataforma | (2, 0.5) | 0.5s | Magenta | Horizontal, ativa ao pisar |
| Portal Grande | (5, 8) | 1.5s | Purple | Área ampla |
| Secreto | (0.5, 0.5) | 2s | Black | Difícil de encontrar |
| Boss Room | (2, 2) | 2s | Red | Transição dramática |
| Retorno | (1, 1) | 0.5s | Green | Rápido para voltar |
| Hub | (1.5, 1.5) | 1s | Variado | Use cores diferentes |

---

## Dicas de Configuração

### 1. Escolha do Trigger Size

- **Pequeno (0.5 - 1):** Requer precisão, bom para segredos
- **Médio (1 - 2):** Padrão, fácil de ativar
- **Grande (2 - 5):** Difícil de evitar, bom para transições obrigatórias
- **Muito Grande (5+):** Cobre área inteira, use com cuidado

### 2. Escolha do Delay

- **0s:** Instantâneo, pode ser abrupto
- **0.5s:** Rápido, bom para gameplay dinâmico
- **1s:** Padrão, equilibrado
- **1.5-2s:** Lento, bom para momentos importantes
- **3s+:** Muito lento, apenas para cutscenes

### 3. Escolha da Cor do Gizmo

Use cores para categorizar:

- **Cyan/Blue:** Teletransportes normais
- **Green:** Retornos/saídas
- **Red:** Boss rooms/perigo
- **Yellow:** Áreas especiais
- **Purple:** Portais grandes
- **Magenta:** Plataformas verticais
- **White:** Dungeon/interior
- **Black:** Secretos (desabilite Gizmos)

### 4. Posicionamento de Destinos

**Regra de Ouro:** Sempre posicione o destino FORA da área de trigger de outro TeleportPoint.

**Bom:**

```
TeleportPoint A (0, 0) → Destino (20, 0)
TeleportPoint B (22, 0) → Destino (0, 0)
```

**Ruim:**

```
TeleportPoint A (0, 0) → Destino (20, 0)
TeleportPoint B (20, 0) → Destino (0, 0)
```

*Player ficará preso em loop infinito!*

### 5. Nomenclatura

Use nomes descritivos que indicam origem e destino:

```
TeleportPoint_[Origem]_To_[Destino]

Exemplos:
- TeleportPoint_Forest_To_Cave
- TeleportPoint_Hub_To_Level1
- TeleportPoint_BossRoom_Exit
```

---

## Checklist de Configuração

Antes de finalizar um TeleportPoint, verifique:

- [ ] Destination Position configurado (não é 0,0,0)
- [ ] Transition Effect atribuído (CircleEffect)
- [ ] Trigger Size apropriado para o contexto
- [ ] BoxCollider2D marcado como "Is Trigger"
- [ ] GameObject do Player tem tag "Player"
- [ ] SceneTransitioner presente na cena
- [ ] Destino está FORA de outros triggers
- [ ] Gizmos habilitados para visualização
- [ ] Nome descritivo no GameObject
- [ ] Testado em ambas as direções (se bidirecional)

---

## Troubleshooting de Configuração

### Problema: Teletransporte não ativa

**Verificar:**

1. Trigger Size não é muito pequeno?
2. Player tem tag "Player"?
3. BoxCollider2D está como Trigger?
4. Destination Position está configurado?

### Problema: Loop infinito de teletransporte

**Solução:**

- Destino está dentro de outro trigger
- Mova o destino para fora da área de trigger

### Problema: Transição muito rápida/lenta

**Solução:**

- Ajuste o campo "Delay Before Fade In"
- 0s = instantâneo
- 1s = padrão
- 2s+ = lento

### Problema: Não consigo ver o trigger no Editor

**Solução:**

- Habilite "Enable Gizmos"
- Selecione o GameObject no Hierarchy
- Verifique se Scene view está em modo Shaded/Wireframe

---

## Conclusão

Este guia fornece exemplos práticos para a maioria dos cenários de uso. Experimente diferentes configurações para encontrar o que funciona melhor para seu jogo.

**Lembre-se:** Sempre teste seus teletransportes em ambas as direções e verifique se os destinos fazem sentido no contexto do level design!
