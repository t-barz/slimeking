# Documento de Regras Técnicas - The Slime King

## Estrutura de Tópicos

### 1. **Sistema de Personagem**
- 1.1 Estrutura de GameObjects
- 1.2 Hierarquia de Componentes
- 1.3 Estados do Slime

### 2. **Sistema de Animação**
- 2.1 Parâmetros do Animator
- 2.2 Transições de Estado
- 2.3 Triggers e Condições

### 3. **Sistema de Movimento**
- 3.1 Input Actions
- 3.2 Física e Velocidade
- 3.3 Direcionamento Visual

### 4. **Sistema de Crescimento**
- 4.1 Estágios de Evolução
- 4.2 Mudanças Visuais
- 4.3 Desbloqueio de Habilidades

### 5. **Sistema de Combate**
- 5.1 Tipos de Ataque
- 5.2 Feedback Visual
- 5.3 Cooldowns e Invulnerabilidade

### 6. **Sistema de Absorção Elemental**
- 6.1 Tipos de Fragmentos
- 6.2 Detecção e Coleta
- 6.3 Armazenamento de Energia

### 7. **Sistema de Inventário**
- 7.1 Estrutura de Slots
- 7.2 Tipos de Itens
- 7.3 Uso e Consumo

### 8. **Sistema de Interação**
- 8.1 Detecção de Objetos
- 8.2 Feedback Visual
- 8.3 Tipos de Interação

### 9. **Sistema de Stealth**
- 9.1 Estados de Visibilidade
- 9.2 Condições de Detecção
- 9.3 Efeitos Visuais

### 10. **Sistema de UI**
- 10.1 HUD Elements
- 10.2 Menus e Navegação
- 10.3 Feedback de Ações

### 11. **Sistema de Áudio**
- 11.1 Triggers Sonoros
- 11.2 Mixagem Dinâmica
- 11.3 Estados Musicais

### 12. **Sistema de Efeitos Visuais**
- 12.1 Partículas
- 12.2 Pós-Processamento
- 12.3 Shaders e Materials

### 13. **Sistema de Persistência**
- 13.1 Save Data Structure
- 13.2 Auto-Save Triggers
- 13.3 Loading States

---

## **Regras Específicas**

### 1. **Sistema de Personagem**

#### 1.1 Estrutura de GameObjects

O objeto **SlimeBaby** é estruturado da seguinte forma:
```
slimeBaby
├── back
├── vfx_back
├── vfx_front
├── front
├── side
├── vfx_side
└── shadow
```

#### 1.2 Regras de Visibilidade

Esses subobjetos não ficarão todos visíveis ao mesmo tempo. As regras para exibição são as seguintes:

- **No start do jogo**: somente os objetos com **front** no nome e **shadow** devem estar visíveis
- **Ao se deslocar para o norte**: somente os objetos com **back** e **shadow** no nome devem estar visíveis
- **Quando se deslocar para as laterais**: somente os objetos com **side** e **shadow** no nome devem estar visíveis
- **Quando se deslocar para a esquerda**: deve-se realizar um flip horizontal no sprite e voltar ao padrão quando se deslocar para a direita

### 2. **Sistema de Animação**

#### 2.1 Parâmetros do Animator

Existem **7 parâmetros** que interferem nas animações:
- `isSleeping` (bool)
- `isHiding` (bool)
- `isWalking` (bool)
- `Shrink` (Trigger)
- `Jump` (Trigger)
- `Attack01` (Trigger)
- `Attack02` (Trigger)

#### 2.2 Regras dos Parâmetros

As regras para esses parâmetros são as seguintes:

- **isHiding**: Quando o jogador realizar um `InputAction.Crouch`, `isHiding` é setado `true` e quando deixar de realizar essa ação, volta para `false`

- **isWalking**: Ao se deslocar para qualquer direção, o parâmetro `isWalking` deve ser setado `true` e retornar para `false` quando o movimento parar

- **Attack01**: É ativado quando realizar um `InputAction.Attack`

- **Attack02**: É ativado quando realizar um `InputAction.Special`

- **Shrink e Jump**: São ativados ao realizar um `InputAction.Interact`. Se será `Shrink` ou `Jump` depende do objeto com o qual o Slime está interagindo

---

## **Observações de Implementação**

### Convenções de Nomenclatura
- Todos os GameObjects seguem padrão camelCase
- Efeitos VFX são prefixados com `vfx_`
- Direções seguem padrão: `front`, `back`, `side`

### Prioridades de Renderização
- `shadow`: Render Order mais baixo
- Sprites principais: Render Order médio
- `vfx_`: Render Order mais alto

### Estados Mutuamente Exclusivos
- Apenas um conjunto de sprites de direção pode estar ativo por vez
- Parâmetros de animação devem ser resetados adequadamente
- Transições de estado devem ser suaves e responsivas