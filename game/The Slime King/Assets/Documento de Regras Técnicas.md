# Documento de Regras Técnicas – The Slime King

## Introdução Geral

Este documento estabelece as diretrizes técnicas fundamentais para a implementação de **The Slime King**, um jogo de aventura-RPG 2D com foco em exploração relaxante, crescimento progressivo e cooperação com companheiros.

O objetivo deste documento é fornecer especificações claras e detalhadas que garantam consistência na implementação, facilitando o trabalho da equipe de desenvolvimento e assegurando que todos os sistemas funcionem harmoniosamente para criar a experiência cozy e envolvente desejada.

Cada sistema foi projetado com três princípios fundamentais em mente:

- **Simplicidade**: Mecânicas intuitivas que não sobrecarregam o jogador
- **Integração**: Sistemas que se complementam e reforçam mutuamente
- **Escalabilidade**: Arquitetura que permite expansão futura sem reestruturação

---

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
- 6.4 Fragmentos Elementais (Prefabs)


### 7. **Sistema de Inventário**

- 7.1 Estrutura de Slots
- 7.2 Tipos de Itens
- 7.3 Uso e Consumo


### 8. **Sistema de Interação e Ambiente**

- 8.1 Detecção de Objetos
- 8.2 Feedback Visual
- 8.3 Tipos de Interação
- 8.4 Objetos Destrutíveis
- 8.5 Objetos Móveis


### 9. **Sistema de Stealth**

- 9.1 Estados de Visibilidade
- 9.2 Condições de Detecção
- 9.3 Efeitos Visuais


### 10. **Sistema de Seguidores e Mini-Slimes**

- 10.1 Gestão de Seguidores
- 10.2 IA de Companheiros
- 10.3 Sistema de Mini-Slimes


### 11. **Sistema de UI**

- 11.1 HUD Elements
- 11.2 Menus e Navegação
- 11.3 Feedback de Ações


### 12. **Sistema de Áudio**

- 12.1 Triggers Sonoros
- 12.2 Mixagem Dinâmica
- 12.3 Estados Musicais


### 13. **Sistema de Efeitos Visuais**

- 13.1 Partículas
- 13.2 Pós-Processamento
- 13.3 Shaders e Materials


### 14. **Sistema de Persistência**

- 14.1 Save Data Structure
- 14.2 Auto-Save Triggers
- 14.3 Loading States


### 15. **Sistema de Localização de Textos**

- 15.1 Estrutura de Arquivos CSV
- 15.2 Detecção Automática de Idioma
- 15.3 Sistema de Configuração

---

## **Regras Específicas**

### 1. **Sistema de Personagem**

O sistema de personagem é o coração técnico do jogo, responsável por gerenciar todos os aspectos visuais e funcionais do slime protagonista. Este sistema foi projetado para ser modular e expansível, permitindo que o personagem evolua visualmente e funcionalmente ao longo da jornada.

A arquitetura escolhida utiliza uma hierarquia de GameObjects aninhados para maximizar a flexibilidade visual, permitindo diferentes orientações direcionais e efeitos visuais contextuais sem duplicação de código ou recursos.

#### 1.1 Estrutura de GameObjects

O objeto **SlimeBaby** é estruturado da seguinte forma:

```
slimeBaby
├── back          // Sprite para movimento norte
├── vfx_back      // Efeitos visuais traseiros
├── vfx_front     // Efeitos visuais frontais
├── front         // Sprite para movimento sul
├── side          // Sprite para movimento leste/oeste
├── vfx_side      // Efeitos visuais laterais
└── shadow        // Sombra sempre visível
```

**Justificativa da Estrutura:**
Esta organização permite controle granular sobre a aparência do slime baseada na direção do movimento, criando uma sensação de profundidade e direcionamento que é essencial para a imersão em jogos top-down. A separação entre sprites principais e efeitos VFX facilita a manutenção e permite que artistas trabalhem independentemente em diferentes camadas visuais.

#### 1.2 Regras de Visibilidade

O sistema de visibilidade direcional é fundamental para criar a ilusão de movimento tridimensional em um mundo 2D. As regras são aplicadas automaticamente baseadas no vetor de movimento do jogador:

- **No start do jogo**: somente os objetos com **front** no nome e **shadow** devem estar visíveis
- **Ao se deslocar para o norte**: somente os objetos com **back** e **shadow** no nome devem estar visíveis
- **Quando se deslocar para as laterais**: somente os objetos com **side** e **shadow** no nome devem estar visíveis
- **Quando se deslocar para a esquerda**: deve-se realizar um flip horizontal no sprite e voltar ao padrão quando se deslocar para a direita

**Importância Técnica:**
Este sistema reduz significativamente o número de sprites necessários (utilizando flip horizontal para oeste) e garante transições visuais suaves que mantêm a coerência direcional sem quebrar a imersão.

#### 1.3 Componentes Obrigatórios

| Componente | Função | Configuração |
| :-- | :-- | :-- |
| `PlayerMovement` | Controle de movimento e física | Rigidbody2D + Collider2D |
| `PlayerGrowth` | Sistema de evolução | Array de sprites por estágio |
| `PlayerVisualManager` | Controle de visibilidade direcional | Gerencia objetos front/back/side |
| `ElementalEnergyManager` | Armazenamento de energia | Barras por elemento |
| `InventoryManager` | Sistema de inventário evolutivo | 1-4 slots conforme crescimento |

**Arquitetura Modular:**
Cada componente é responsável por um aspecto específico do personagem, permitindo desenvolvimento paralelo, testing independente e manutenção simplificada. Esta separação também facilita a extensão do sistema conforme novas funcionalidades são adicionadas.

### 2. **Sistema de Animação**

O sistema de animação em **The Slime King** é projetado para ser expressivo e responsivo, comunicando efetivamente o estado atual do slime através de animações fluidas e contextuais. Utilizando o Animator Controller da Unity, o sistema balanceia simplicidade de implementação com flexibilidade criativa.

O objetivo principal é criar animações que reforcem a personalidade "gelatinosa" do slime enquanto fornecem feedback claro sobre suas ações e estado interno, contribuindo para a atmosfera cozy do jogo.

#### 2.1 Parâmetros do Animator

O sistema utiliza **7 parâmetros** cuidadosamente selecionados para controlar todas as transições de animação necessárias:

- `isSleeping` (bool) - Estado de descanso do slime
- `isHiding` (bool) - Estado de stealth ativo
- `isWalking` (bool) - Movimento ativo
- `Shrink` (Trigger) - Animação de encolhimento para passar por espaços
- `Jump` (Trigger) - Animação de salto
- `Attack01` (Trigger) - Ataque básico
- `Attack02` (Trigger) - Ataque especial

**Design Philosophy:**
Este conjunto mínimo de parâmetros evita complexidade desnecessária no Animator Controller, reduzindo chances de bugs e facilitando debugging. Cada parâmetro tem uma responsabilidade clara e bem definida.

#### 2.2 Regras dos Parâmetros

O sistema de regras garante que as animações respondam correta e consistentemente às ações do jogador:

- **isHiding**: Quando o jogador realizar um `InputAction.Crouch`, `isHiding` é setado `true` e quando deixar de realizar essa ação, volta para `false`. **IMPORTANTE**: Durante o estado `isHiding`, o movimento do slime deve ser completamente desabilitado, impedindo qualquer deslocamento.
- **isWalking**: Ao se deslocar para qualquer direção, o parâmetro `isWalking` deve ser setado `true` e retornar para `false` quando o movimento parar
- **Attack01**: É ativado quando realizar um `InputAction.Attack`
- **Attack02**: É ativado quando realizar um `InputAction.Special`
- **Shrink e Jump**: São ativados ao realizar um `InputAction.Interact`. Se será `Shrink` ou `Jump` depende do objeto com o qual o Slime está interagindo

**Lógica de Priorização:**
Triggers têm prioridade sobre bools, permitindo que ações específicas interrompam estados contínuos quando necessário. Isso garante que animações de ação (como ataques) sempre sejam executadas, mesmo durante movimento.

**Restrição de Movimento em Stealth:**
Quando `isHiding` está ativo, o sistema de movimento deve verificar esta condição e impedir qualquer input de movimento, mantendo o slime completamente estático durante o stealth.

#### 2.3 Estados de Transição

| Estado Origem | Estado Destino | Condição |
| :-- | :-- | :-- |
| Idle | Walking | `isWalking == true` |
| Walking | Idle | `isWalking == false` |
| Any State | Hiding | `isHiding == true` |
| Hiding | Idle | `isHiding == false` |
| Idle | Attack01 | `Attack01 trigger` |
| Idle | Shrink/Jump | `Shrink/Jump trigger` |

**Timing Considerations:**
As transições são calibradas para parecerem naturais e responsivas. Transições muito rápidas podem parecer bruscas, enquanto transições muito lentas fazem o jogo parecer lento. Os valores escolhidos foram testados para oferecer a melhor sensação de responsividade.

### 3. **Sistema de Movimento**

O sistema de movimento é fundamental para a experiência de jogo, sendo o meio primário de interação do jogador com o mundo. Em **The Slime King**, o movimento deve parecer fluido e orgânico, refletindo a natureza gelatinosa do protagonista enquanto oferece controle preciso e responsivo.

O sistema suporta múltiplas plataformas e dispositivos de entrada, garantindo que a experiência de movimento seja consistente e satisfatória independentemente de como o jogador escolhe jogar.

#### 3.1 Input Actions

O mapeamento de controles foi cuidadosamente projetado para ser intuitivo em todas as plataformas, seguindo convenções estabelecidas pela indústria enquanto acomoda as necessidades específicas do gameplay de **The Slime King**.

**Mapeamento Universal por Função**


| Ação | Função | Teclado | Gamepad PC (Xbox) |
| :-- | :-- | :-- | :-- |
| Move | Movimento em 8 direções | WASD / Arrows | Left Stick |
| Attack | Ataque básico | Space | B |
| Interact | Interagir com objetos | E | A |
| Crouch | Stealth/esconder | Q | X |
| Use Item | Usar item selecionado | Left Alt | Y |
| Change Item | Alternar item selecionado | Scroll Mouse / Tab | D-pad (qualquer direção) |
| Ability 1 | Habilidade elemental 1 | 1 | LB |
| Ability 2 | Habilidade elemental 2 | 2 | LT |
| Ability 3 | Habilidade elemental 3 | 3 | RB |
| Ability 4 | Habilidade elemental 4 | 4 | RT |
| Menu | Menu principal | Enter | Menu |
| Inventory | Menu de inventário | Right Shift | View |

**Filosofia de Design dos Controles:**

- **Ações frequentes** (movimento, ataque) estão nas posições mais acessíveis
- **Habilidades elementais** usam os gatilhos para acesso rápido durante combate
- **Menus e inventário** utilizam botões menos críticos para evitar ativação acidental

**Mapeamento Específico por Plataforma de Console**

O sistema reconhece automaticamente diferentes controles de console e adapta o layout de botões conforme apropriado:

**Nintendo Switch**


| Ação | Botão Switch | Função |
| :-- | :-- | :-- |
| Move | Left Stick | Movimento em 8 direções |
| Attack | B | Ataque básico |
| Interact | A | Interagir com objetos |
| Crouch | X | Stealth/esconder |
| Use Item | Y | Usar item selecionado |
| Change Item | D-pad (qualquer direção) | Alternar item selecionado |
| Ability 1 | L | Habilidade elemental 1 |
| Ability 2 | ZL | Habilidade elemental 2 |
| Ability 3 | R | Habilidade elemental 3 |
| Ability 4 | ZR | Habilidade elemental 4 |
| Menu | + (Plus) | Menu principal |
| Inventory | - (Minus) | Menu de inventário |

**PlayStation (DualShock/DualSense)**


| Ação | Botão PlayStation | Função |
| :-- | :-- | :-- |
| Move | Left Stick | Movimento em 8 direções |
| Attack | Circle | Ataque básico |
| Interact | X | Interagir com objetos |
| Crouch | Square | Stealth/esconder |
| Use Item | Triangle | Usar item selecionado |
| Change Item | D-pad (qualquer direção) | Alternar item selecionado |
| Ability 1 | L1 | Habilidade elemental 1 |
| Ability 2 | L2 | Habilidade elemental 2 |
| Ability 3 | R1 | Habilidade elemental 3 |
| Ability 4 | R2 | Habilidade elemental 4 |
| Menu | Options | Menu principal |
| Inventory | Share/Create | Menu de inventário |

**Xbox (Console)**


| Ação | Botão Xbox | Função |
| :-- | :-- | :-- |
| Move | Left Stick | Movimento em 8 direções |
| Attack | B | Ataque básico |
| Interact | A | Interagir com objetos |
| Crouch | X | Stealth/esconder |
| Use Item | Y | Usar item selecionado |
| Change Item | D-pad (qualquer direção) | Alternar item selecionado |
| Ability 1 | LB | Habilidade elemental 1 |
| Ability 2 | LT | Habilidade elemental 2 |
| Ability 3 | RB | Habilidade elemental 3 |
| Ability 4 | RT | Habilidade elemental 4 |
| Menu | Menu | Menu principal |
| Inventory | View | Menu de inventário |

#### 3.2 Configurações de Física

O sistema de física foi calibrado para cada estágio de crescimento do slime, criando uma sensação de progressão tangível não apenas visual, mas também na forma como o personagem se move pelo mundo.


| Propriedade | Baby Slime | Young Slime | Adult Slime | Elder Slime |
| :-- | :-- | :-- | :-- | :-- |
| Velocidade Base | 3.0 | 4.0 | 4.5 | 5.0 |
| Massa | 0.5 | 1.0 | 1.5 | 2.0 |
| Drag Linear | 5.0 | 4.0 | 3.0 | 2.0 |
| Freeze Rotation | true | true | true | true |

**Justificativa das Configurações:**

- **Velocidade crescente** reflete o ganho de poder e confiança
- **Massa aumentada** torna o slime mais estável em estágios avançados
- **Drag reduzido** permite movimento mais fluido conforme evolui
- **Rotation freezing** mantém orientação visual consistente

**Restrição de Movimento durante Stealth:**
O sistema de movimento deve incluir uma verificação obrigatória do estado `isHiding`. Quando este parâmetro estiver ativo, todos os inputs de movimento devem ser ignorados, mantendo o slime completamente imóvel.

```csharp
// Exemplo de implementação da restrição de movimento
void HandleMovement()
{
    // Verificar se está em modo stealth
    if (animator.GetBool("isHiding"))
    {
        // Bloquear movimento completamente
        rigidbody2D.velocity = Vector2.zero;
        return;
    }
    
    // Processar movimento normal
    Vector2 movementInput = inputActions.Move.ReadValue<Vector2>();
    // ... resto da lógica de movimento
}
```


#### 3.3 Direcionamento Visual

O sistema de direcionamento visual é crucial para comunicar claramente a orientação do slime e manter coerência visual durante o movimento. Este sistema trabalha em conjunto com a estrutura de GameObjects definida no Sistema de Personagem.

#### Regras de Ativação de Sprites por Direção

**Movimento para o Norte (Y > 0)**

- Ativar sprites: `back` e `shadow`
- Desativar sprites: `front`, `side`, `vfx_front`, `vfx_side`
- Manter ativo: `vfx_back`

**Movimento para o Sul (Y < 0)**

- Ativar sprites: `front` e `shadow`
- Desativar sprites: `back`, `side`, `vfx_back`, `vfx_side`
- Manter ativo: `vfx_front`

**Movimento para o Leste (X > 0)**

- Ativar sprites: `side` e `shadow`
- Desativar sprites: `front`, `back`, `vfx_front`, `vfx_back`
- Manter ativo: `vfx_side`
- **Orientação**: `flipX = false` (sprite normal)

**Movimento para o Oeste (X < 0)**

- Ativar sprites: `side` e `shadow`
- Desativar sprites: `front`, `back`, `vfx_front`, `vfx_back`
- Manter ativo: `vfx_side`
- **Orientação**: `flipX = true` (sprite espelhado horizontalmente)


#### Regras para Movimento Diagonal

O tratamento de movimento diagonal é essencial para evitar "flicker" visual quando o jogador move o analog stick em direções intermediárias:

**Priorização de Eixo**

- Calcular a magnitude de cada eixo: `Mathf.Abs(movement.x)` vs `Mathf.Abs(movement.y)`
- Aplicar a regra do eixo com **maior magnitude**
- Em caso de empate, priorizar movimento vertical (Y)

**Benefícios da Abordagem:**
Esta metodologia garante transições visuais estáveis e previsíveis, evitando mudanças rápidas de orientação que podem ser visualmente perturbadoras.

#### Implementação Técnica Sugerida

```csharp
void UpdateVisualDirection(Vector2 movement)
{
    // Determinar direção principal
    bool isVerticalMovement = Mathf.Abs(movement.y) >= Mathf.Abs(movement.x);
    
    if (isVerticalMovement)
    {
        if (movement.y > 0) // Norte
        {
            ActivateSprites("back");
        }
        else if (movement.y < 0) // Sul
        {
            ActivateSprites("front");
        }
    }
    else
    {
        ActivateSprites("side");
        spriteRenderer.flipX = movement.x < 0; // Oeste = flip, Leste = normal
    }
}
```


### 4. **Sistema de Crescimento**

O sistema de crescimento é o núcleo da progressão em **The Slime King**, representando tanto a evolução mecânica quanto a narrativa do protagonista. Este sistema foi projetado para ser visualmente satisfatório e mecanicamente significativo, criando momentos de celebração quando o jogador atinge novos marcos.

O crescimento não é apenas cosmético - cada estágio desbloqueia novas capacidades, expande possibilidades de gameplay e permite acesso a áreas previamente inacessíveis, fundamentando a estrutura metroidvania do jogo.

#### 4.1 Estágios de Evolução

O sistema utiliza quatro estágios distintos, cada um representando uma fase significativa na jornada do slime:


| Estágio | Energia Necessária | Slots Inventário | Seguidores Máx | Mini-Slimes Máx |
| :-- | :-- | :-- | :-- | :-- |
| Baby Slime | 0 | 1 | 0 | 0 |
| Young Slime | 200 | 2 | 1 | 1 |
| Adult Slime | 600 | 3 | 3 | 2 |
| Elder/King | 1200 | 4 | 4 | 3 |

**Design Philosophy:**

- **Baby Slime**: Vulnerável e limitado, foca na sobrevivência e aprendizado básico
- **Young Slime**: Primeiro crescimento significativo, introduz mecânicas de companheirismo
- **Adult Slime**: Fase de maior desenvolvimento, desbloqueia capacidades cooperativas avançadas
- **Elder/King**: Forma final majestosa, acesso completo a todas as mecânicas

**Curva de Progressão Atualizada:**
Os requisitos de energia foram dobrados para criar uma progressão mais desafiadora e prolongar a satisfação de cada conquista. Cada estágio demora significativamente mais para ser alcançado, criando marcos de progressão mais substanciais e recompensadores.

#### 4.2 Mudanças Visuais

O sistema de mudanças visuais assegura que o crescimento seja imediatamente perceptível e satisfatório:

```csharp
// Exemplo de configuração por estágio
[System.Serializable]
public class GrowthStageData 
{
    public Sprite frontSprite;      // Sprite frontal específico do estágio
    public Sprite backSprite;       // Sprite traseiro específico do estágio
    public Sprite sideSprite;       // Sprite lateral específico do estágio
    public Vector3 scale;           // Escala do GameObject
    public GameObject[] vfxPrefabs; // Efeitos visuais únicos do estágio
    public AudioClip growthSound;   // Som de transformação
}
```

**Implementação Visual:**

- **Sprites únicos** para cada direção e estágio
- **Escala progressiva** que torna o slime visivelmente maior
- **Efeitos VFX** específicos que refletem o poder crescente
- **Audio cues** que celebram cada transformação


#### 4.3 Eventos de Crescimento

Os eventos de crescimento são momentos especiais projetados para criar impacto emocional e senso de conquista:

- **Trigger**: Energia elemental acumulada ≥ threshold
- **Efeito Visual**: Partículas especiais + screen flash suave
- **Efeito Sonoro**: Som harmônico ascendente
- **Duração**: 2 segundos de animação
- **Durante Crescimento**: Input desabilitado, invulnerabilidade temporária

**Importância dos Momentos de Crescimento:**
Estes eventos interrompem temporariamente o gameplay para focar a atenção do jogador na transformação, criando um momento de pausa e reflexão que enfatiza o progresso alcançado.

### 5. **Sistema de Combate**

O sistema de combate em **The Slime King** foi desenvolvido com foco na filosofia "cozy", priorizando neutralização ao invés de destruição. Este design choice reforça os temas de harmonia e crescimento conjunto que permeiam todo o jogo.

O combate serve múltiplos propósitos: defesa pessoal, resolução de conflitos, e principalmente como meio de obter energia elemental para crescimento. Cada confronto é uma oportunidade de progressão, não apenas um obstáculo.

#### 5.1 Tipos de Ataque

O sistema oferece três tipos principais de ataque, cada um servindo diferentes situações táticas:


| Tipo | Input | Dano Base | Alcance | Cooldown | Efeito |
| :-- | :-- | :-- | :-- | :-- | :-- |
| Ataque Básico | Attack | 10 + Nível | 1.0 unity | 0.5s | Knockback |
| Dash Attack | Hold Attack | 15 + Nível | 2.0 unity | 1.0s | Movimento + Dano |
| Ataque Especial | Special | 20 + Especial | 1.5 unity | 2.0s | Efeito elemental |

**Design Rationale:**

- **Ataque Básico**: Rápido e confiável, para situações gerais
- **Dash Attack**: Combina movimento e combate, útil para reposicionamento
- **Ataque Especial**: Mais poderoso mas limitado, usa energia elemental

**Escalamento de Poder:**
O dano base aumenta com o nível do slime, garantindo que o combate permaneça relevante e satisfatório mesmo em estágios avançados.

#### 5.2 Feedback Visual

O feedback visual é crucial para comunicar claramente o que está acontecendo durante o combate, especialmente importante em um jogo que evita violência gráfica:

- **Hit Effect**: Sprite pisca branco por 0.1s
- **Knockback**: Força aplicada contrária ao atacante
- **Invulnerabilidade**: 0.5s pós-hit com sprite piscando
- **Efeitos de Partícula**: Por elemento (fogo, água, terra, ar)

**Função do Feedback:**

- **Confirma** que o ataque conectou
- **Comunica** o tipo de dano através de cores/efeitos
- **Previne** spam de ataques através de frames de invulnerabilidade
- **Mantém** clareza visual durante confrontos


#### 5.3 Sistema de Dano

O sistema utiliza uma fórmula simples mas efetiva para cálculo de dano:

```csharp
int CalculateDamage(int baseDamage, int defense) 
{
    int realDamage = Mathf.Max(baseDamage - defense, 1);
    return realDamage;
}
```

**Garantias do Sistema:**

- Todo ataque causa pelo menos 1 de dano (evita ataques inúteis)
- Defesa reduz dano linearmente até o mínimo
- Simplicidade facilita balanceamento e compreensão


### 6. **Sistema de Absorção Elemental**

O sistema de absorção elemental é único de **The Slime King**, servindo como mecânica principal de progressão, diferenciação de gameplay e conexão temática com o mundo. Este sistema transforma cada fragmento coletado em uma pequena celebração de progresso.

A absorção elemental conecta diretamente combate, exploração e crescimento, criando um loop de gameplay satisfatório onde cada ação contribui para o desenvolvimento do protagonista.

#### 6.1 Tipos de Energia

Cada elemento oferece tanto habilidades ativas quanto benefícios passivos crescentes, criando estratégias de especialização:


| Elemento | Cor Principal | Cor Secundária | Habilidade | Efeito Passivo Base |
| :-- | :-- | :-- | :-- | :-- |
| Terra | \#8B4513 | \#DEB887 | Quebrar rochas | +1 Defense a cada 10 pontos |
| Água | \#4169E1 | \#87CEEB | Nadar/crescer plantas | +1 Regeneração a cada 10 pontos |
| Fogo | \#FF4500 | \#FFA500 | Iluminar/derreter | +1 Attack a cada 10 pontos |
| Ar | \#E6E6FA | \#F0F8FF | Planar/ativar eólicos | +1 Speed a cada 10 pontos |

**Design Philosophy Atualizada:**

- **Terra**: Proteção e resistência, permite quebrar barreiras. Defesa cresce progressivamente.
- **Água**: Cura e crescimento, facilita navegação aquática. Regeneração aumenta continuamente.
- **Fogo**: Poder ofensivo e iluminação, remove obstáculos de gelo. Força de ataque se desenvolve.
- **Ar**: Velocidade e mobilidade, ativa mecanismos eólicos. Agilidade melhora consistentemente.

**Sistema de Progressão Elemental:**
A cada 10 pontos de energia elemental de um tipo específico acumulados, o efeito passivo correspondente aumenta em 1 ponto. Esta mecânica incentiva especialização elemental e cria progressão contínua além do crescimento de estágios.

```csharp
// Exemplo de implementação do sistema progressivo
void UpdateElementalPassives()
{
    int earthBonus = earthEnergy / 10;
    int waterBonus = waterEnergy / 10;
    int fireBonus = fireEnergy / 10;
    int airBonus = airEnergy / 10;
    
    defense = baseDefense + earthBonus;
    regeneration = baseRegeneration + waterBonus;
    attack = baseAttack + fireBonus;
    speed = baseSpeed + airBonus;
}
```


#### 6.4 Fragmentos Elementais (Prefabs)

Os fragmentos elementais são os building blocks do sistema de progressão, projetados para serem visualmente atraentes e mecanicamente satisfatórios:


| Componente | Regra de Implementação |
| :-- | :-- |
| **Prefab Base** | `ElementalFragment` (Script + SpriteRenderer + Rigidbody2D opcional) |
| **Variantes de Sprite** | Três tamanhos: `Small`, `Medium`, `Large` <br>Array `spriteVariants` no Inspector |
| **Cor Dinâmica** | Parâmetros `colorA` e `colorB` recebidos ao instanciar <br>Na `Awake()`: `Color.Lerp(colorA, colorB, Random.value)` |
| **Energia Gerada** | `[Small = 1] [Medium = 3] [Large = 7]` (configurável via ScriptableObject) |
| **Sorteio de Sprite** | Ao instanciar, sortear tamanho usando `DropTable` do objeto origem |
| **Configuração de Drop** | `allowedSizes[]` + `dropChance[]` por tamanho em cada inimigo/objeto |
| **Comportamento Física** | `AddImpulse` aleatório para "pular" do ponto de origem |
| **Sistema de Atração** | **Fragmentos se deslocam automaticamente em direção ao slime quando ele passa por perto** <br>Raio de detecção: 2.0 units <br>Velocidade de movimento: 8.0 units/segundo |
| **Detecção de Proximidade** | `OverlapCircle` contínuo para detectar slime na área <br>Quando detectado, ativar movimento em direção ao slime |
| **Colisão e Absorção** | **Trigger2D detecta colisão com slime** <br>**Efeito de absorção é exibido instantaneamente** <br>**Valores de energia elemental são adicionados ao slime** <br>**Fragmento é destruído após absorção** |
| **Efeitos Visuais** | Trail renderer durante movimento + partículas de absorção na colisão |
| **Efeitos Sonoros** | Tom musical baseado no elemento ao ser absorvido |

**Innovation: Sistema de Atração Automática**
Os fragmentos sendo magneticamente atraídos para o slime cria uma sensação de poder crescente e torna a coleta mais fluida e satisfatória.

#### Implementação Técnica Detalhada

```csharp
public class ElementalFragment : MonoBehaviour
{
    [Header("Configuração")]
    public ElementType elementType;
    public int energyValue = 1;
    public float detectionRadius = 2.0f;
    public float moveSpeed = 8.0f;
    
    [Header("Efeitos")]
    public GameObject absorptionEffect;
    public AudioClip absorptionSound;
    
    private Transform slimeTarget;
    private bool isMovingToSlime = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(DetectSlimeNearby());
    }
    
    void Update()
    {
        if (isMovingToSlime && slimeTarget != null)
        {
            // Mover em direção ao slime
            transform.position = Vector2.MoveTowards(
                transform.position, 
                slimeTarget.position, 
                moveSpeed * Time.deltaTime
            );
        }
    }
    
    IEnumerator DetectSlimeNearby()
    {
        while (!isMovingToSlime)
        {
            // Detectar slime na área
            Collider2D slimeCollider = Physics2D.OverlapCircle(
                transform.position, 
                detectionRadius, 
                LayerMask.GetMask("Player")
            );
            
            if (slimeCollider != null)
            {
                slimeTarget = slimeCollider.transform;
                isMovingToSlime = true;
                break;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Efeito de absorção
            if (absorptionEffect != null)
            {
                Instantiate(absorptionEffect, transform.position, Quaternion.identity);
            }
            
            // Som de absorção
            if (absorptionSound != null)
            {
                AudioSource.PlayClipAtPoint(absorptionSound, transform.position);
            }
            
            // Adicionar energia ao slime
            ElementalEnergyManager energyManager = other.GetComponent<ElementalEnergyManager>();
            if (energyManager != null)
            {
                energyManager.AbsorbEnergy(elementType, energyValue);
            }
            
            // Destruir fragmento
            Destroy(gameObject);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualizar raio de detecção no editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCircle(transform.position, detectionRadius);
    }
}
```


#### Configurações Adicionais

**Timing de Absorção**

- **Detecção**: 0.1s de intervalo para verificação de proximidade
- **Movimento**: Suave e contínuo em direção ao slime
- **Absorção**: Instantânea ao colidir
- **Feedback**: Efeito visual + sonoro + incremento de energia simultâneos

**Balanceamento**

- **Raio de Detecção**: 2.0 units (ajustável por tipo de fragmento)
- **Velocidade**: 8.0 units/s (mais rápida que movimento do slime)
- **Priorização**: Fragmentos mais próximos se movem primeiro em caso de múltiplos na área


#### 6.2 Sistema de Atração

O sistema de atração é calibrado para ser satisfatório sem ser excessivamente automático:

- **Raio de Atração**: 2.0 units para slime
- **Velocidade de Atração**: 8.0 units/segundo
- **Efeito Visual**: Trail renderer + partículas em direção ao slime
- **Efeito Sonoro**: Tom musical baseado no elemento


#### 6.3 Armazenamento e UI

```csharp
[System.Serializable]
public class ElementalEnergy 
{
    public ElementType type;
    public float current;
    public float maximum;
    public Color barColor;
    public int passiveBonus; // Novo: bônus passivo atual baseado na energia acumulada
}
```


### 7. **Sistema de Inventário**

O sistema de inventário em **The Slime King** é projetado para ser simples e intuitivo, evitando a complexidade de management que pode quebrar a atmosfera relaxante do jogo. O inventário cresce junto com o slime, simbolizando sua capacidade aumentada de carregar e gerenciar recursos.

A simplicidade do sistema permite que jogadores foquem na exploração e descoberta ao invés de microgerenciamento de itens, mantendo o fluxo cozy de gameplay.

#### 7.1 Estrutura de Slots

O sistema evolutivo de slots cria uma sensação tangível de progressão:


| Estágio | Slots Disponíveis | Tipos Permitidos | Stack Máximo |
| :-- | :-- | :-- | :-- |
| Baby | 1 | Qualquer | 10 |
| Young | 2 | Qualquer | 10 |
| Adult | 3 | Qualquer | 10 |
| Elder | 4 | Qualquer | 10 |

**Design Philosophy:**

- **Crescimento gradual** recompensa progressão
- **Flexibilidade total** de tipos permite experimentação
- **Stack de 10** oferece quantidade útil sem excessos


#### 7.2 Tipos de Itens

O sistema de itens é flexível e configurável, permitindo a criação de diversos tipos de itens através de ScriptableObjects. Cada item possui propriedades customizáveis que definem seu comportamento e efeitos.

**Estrutura Base dos Itens:**

```csharp
[CreateAssetMenu(fileName = "New Item", menuName = "Game/Items/Base Item")]
[System.Serializable]
public class ItemData : ScriptableObject
{
    [Header("Identificação")]
    public string itemName;
    public string description;
    public Sprite icon;
    public ItemType type;
    
    [Header("Propriedades de Uso")]
    public bool isConsumable = true;
    public EffectType effectType;
    public int effectValue;
    public float duration;
    
    [Header("Propriedades de Stack")]
    public int maxStackSize = 10;
    public bool isUnique = false;
    
    [Header("Efeitos Especiais")]
    public GameObject useEffect;
    public AudioClip useSound;
    public string customEffect; // Para efeitos especiais programados
}

public enum ItemType
{
    Consumable,    // Itens de uso imediato
    Tool,          // Ferramentas especiais
    Material,      // Materiais de construção/crafting
    Special        // Itens únicos com efeitos especiais
}

public enum EffectType
{
    RestoreHP,           // Restaura pontos de vida
    RestoreEnergy,       // Restaura energia elemental
    BoostSpeed,          // Aumenta velocidade temporariamente
    BoostAttack,         // Aumenta ataque temporariamente
    BoostDefense,        // Aumenta defesa temporariamente
    PlantSeed,           // Planta uma semente
    Illuminate,          // Ilumina área temporariamente
    CustomEffect         // Efeito customizado via script
}
```

**Exemplos de Configuração de Itens:**


| Item | Tipo | Efeito | Valor | Duração | Descrição |
| :-- | :-- | :-- | :-- | :-- | :-- |
| Maçã | Consumable | RestoreHP | 20 | Instantâneo | Restaura 20 pontos de vida |
| Flor Mágica | Consumable | RestoreEnergy | 10 | Instantâneo | Adiciona 10 pontos de energia elemental aleatória |
| Cristal de Cura | Consumable | RestoreHP | 50 | Instantâneo | Restaura 50 pontos de vida |
| Semente | Tool | PlantSeed | 1 | Permanente | Planta uma árvore no local atual |
| Poção de Velocidade | Consumable | BoostSpeed | 2 | 30s | Dobra a velocidade por 30 segundos |
| Cristal Luminoso | Tool | Illuminate | 5 | 60s | Ilumina área por 1 minuto |

**Sistema de Configuração Flexível:**

O sistema permite que designers criem novos itens facilmente através do editor da Unity, definindo:

- **Propriedades básicas**: Nome, descrição, ícone
- **Comportamento de uso**: Efeito, valor, duração
- **Propriedades de stack**: Quantidade máxima, unicidade
- **Efeitos audiovisuais**: Partículas e sons de uso
- **Efeitos customizados**: Scripts específicos para comportamentos únicos

**Implementação de Efeitos Customizados:**

```csharp
public class ItemEffectHandler : MonoBehaviour
{
    public void ApplyItemEffect(ItemData item, GameObject user)
    {
        switch (item.effectType)
        {
            case EffectType.RestoreHP:
                user.GetComponent<Health>().Heal(item.effectValue);
                break;
                
            case EffectType.RestoreEnergy:
                user.GetComponent<ElementalEnergyManager>().AddRandomEnergy(item.effectValue);
                break;
                
            case EffectType.PlantSeed:
                PlantSeedAt(user.transform.position);
                break;
                
            case EffectType.CustomEffect:
                ExecuteCustomEffect(item.customEffect, user);
                break;
        }
        
        // Efeitos visuais e sonoros
        if (item.useEffect != null)
            Instantiate(item.useEffect, user.transform.position, Quaternion.identity);
            
        if (item.useSound != null)
            AudioSource.PlayClipAtPoint(item.useSound, user.transform.position);
    }
}
```

**Vantagens do Sistema Configurável:**

- **Flexibilidade**: Novos itens podem ser criados sem alterações de código
- **Balanceamento**: Valores podem ser ajustados facilmente durante desenvolvimento
- **Expansibilidade**: Sistema suporta adição de novos tipos de efeito
- **Modding**: Estrutura preparada para suporte futuro a mods da comunidade


#### 7.3 Seleção e Uso

O sistema de uso é otimizado para não interromper o fluxo de gameplay:

- **Navegação**: D-pad ou Change Item input para alternar slot selecionado
- **Uso**: Use Item input consome 1 unidade do item selecionado
- **Feedback**: Animação de uso + efeito de partícula baseado no item
- **Auto-descarte**: Slots vazios são automaticamente reorganizados
- **Informações**: Hover/seleção mostra descrição e efeitos do item


### 8. **Sistema de Interação e Ambiente**

O sistema de interação é fundamental para criar um mundo que se sente vivo e responsivo. Em **The Slime King**, cada objeto no ambiente pode potencialmente ser interativo, criando um senso de descoberta e experimentação que reforça a exploração curiosa.

O sistema é projetado para fornecer feedback claro sobre o que pode ser interagido, evitando frustração enquanto encoraja a exploração ativa do ambiente.

#### 8.1 Detecção de Objetos

O sistema usa múltiplos ranges de detecção para criar uma experiência fluida:


| Distância | Tipo | Feedback Visual |
| :-- | :-- | :-- |
| ≤ 1.5 units | Interativo | Outline branco |
| ≤ 1.0 units | Coletável | Brilho suave |
| ≤ 2.0 units | NPC | Prompt de diálogo |

**Layered Approach:**
Diferentes tipos de objetos têm ranges específicos baseados na importância e frequência de interação, garantindo que elementos críticos sejam facilmente identificáveis.

#### 8.4 Objetos Destrutíveis

Os objetos destrutíveis adicionam uma camada de interatividade ativa ao ambiente, permitindo que o jogador modifique o mundo através de ações diretas:


| Componente | Regra de Implementação |
| :-- | :-- |
| **Prefab Base** | `DestructibleObject` (BoxCollider2D + SpriteRenderer) |
| **Atributos** | `defense` (int) – reduz dano <br>`maxHP` (int) – vida total |
| **Receber Dano** | `ApplyDamage(int amount)`: `realDamage = max(amount - defense, 1)` |
| **Feedback** | Piscar sprite vermelho ao sofrer dano |
| **Break()** | SFX + VFX + spawn de drops via `DropTable` |
| **Configuração** | `dropTableSO` com chances por tamanho/elemento |

**Purpose and Function:**

- **Progression**: Remove obstáculos para acesso a novas áreas
- **Resources**: Fonte de fragmentos elementais e itens
- **Expression**: Permite que jogadores impactem o ambiente


#### 8.5 Objetos Móveis

Os objetos móveis introduzem puzzles simples e interação física com o ambiente:


| Componente | Regra de Implementação |
| :-- | :-- |
| **Prefab Base** | `MovableObject` (Collider2D + Rigidbody2D Kinematic) |
| **Interação** | `InputAction.Interact` se ≤ `interactDistance` **E** olhando para objeto |
| **Verificação Direção** | Angular difference ≤ 45° entre forward do slime e direção do objeto |
| **Deslocamento** | `moveOffset` (Vector2) + `moveDuration` (float) via Lerp Coroutine |
| **Restrições** | Durante movimento: sem colisão com slime + bloquear nova interação |
| **Rastro Opcional** | `trailPrefab` instanciado a cada `trailStep` metros |

**Design Considerations:**

- **Direção matters**: Jogador deve estar posicionado apropriadamente
- **Visual feedback**: Rastros opcionais comunicam o movimento
- **Safety**: Prevenção de colisões durante movimento


### 9. **Sistema de Stealth**

O sistema de stealth em **The Slime King** oferece uma abordagem não-violenta para lidar com situações perigosas, alinhando-se perfeitamente com a filosofia cozy do jogo. Este sistema permite que jogadores evitem confrontos através de estratégia e posicionamento cuidadoso.

O stealth não é apenas uma mecânica isolada, mas integra-se com o sistema de seguidores, criando desafios cooperativos únicos onde todo o grupo deve ser considerado.

#### 9.1 Estados de Visibilidade

O sistema utiliza estados claros e bem definidos:


| Estado | Condição | Efeito Visual | Detectável | Movimento |
| :-- | :-- | :-- | :-- | :-- |
| Normal | Default | Sprite normal | Sim | Permitido |
| Crouched | Crouch pressionado | Sprite agachado | Não (se em cobertura) | **BLOQUEADO** |
| Hidden | Crouch + Cobertura | Vinheta escura | Não | **BLOQUEADO** |
| Exposed | Crouch sem cobertura | Sprite agachado | Sim | **BLOQUEADO** |

**State Logic Atualizada:**

- **Normal**: Estado padrão, completamente visível, movimento livre
- **Crouched**: Preparação para stealth, movimento completamente bloqueado
- **Hidden**: Stealth efetivo, requer cobertura adequada, sem movimento
- **Exposed**: Tentativa de stealth falhada, ainda sem movimento

**Restrição de Movimento:**
Quando o slime ativa o modo stealth (crouch), o movimento é imediatamente bloqueado para simular a necessidade de permanecer imóvel para não ser detectado. Esta restrição é fundamental para o balanceamento estratégico do stealth.

#### 9.2 Objetos de Cobertura

O sistema reconhece automaticamente objetos que podem fornecer cobertura:

- **Tags Reconhecidas**: "Grass", "Bush", "Rock", "Tree"
- **Detecção**: OverlapCircle com raio 1.0 unit
- **Feedback**: Ícone de olho riscado quando escondido
- **Limitação**: Não pode se mover enquanto escondido

**Environmental Integration:**
O level design deve considerar a distribuição de objetos de cobertura para criar oportunidades táticas interessantes.

#### 9.3 Integração com Seguidores

O sistema de stealth cooperativo adiciona profundidade estratégica:

- **Stealth Cooperativo**: Todos os seguidores devem caber na área de cobertura
- **Feedback Visual**: Indicador mostra se todos estão cobertos
- **Falha Parcial**: Se algum seguidor não cabe, stealth não funciona
- **Movimento Sincronizado**: Seguidores também ficam imóveis durante stealth

**Strategic Implications:**
Jogadores devem considerar o tamanho do grupo e a cobertura disponível, criando decisões táticas interessantes sobre quando usar stealth.

### 10. **Sistema de Seguidores e Mini-Slimes**

O sistema de seguidores é uma das inovações centrais de **The Slime King**, transformando uma jornada solitária em uma experiência de construção de comunidade. Este sistema não apenas adiciona companhia, mas fundamentalmente expande as possibilidades de gameplay através de cooperação.

O design do sistema equilibra utilidade mecânica com conexão emocional, garantindo que cada seguidor seja tanto funcionalmente útil quanto narrativamente significativo.

#### 10.1 Gestão de Seguidores

Cada tipo de seguidor oferece habilidades únicas que se integram com diferentes aspectos do gameplay:


| Tipo | Origem | Habilidade Única | Comandos |
| :-- | :-- | :-- | :-- |
| Pássaro | Quest resgate | Ativar switches altos | Seguir, Ficar, Ir Para |
| Coelho | Quest escolta | Passar por túneis | Seguir, Ficar, Escavar |
| Peixe | Quest limpeza | Controlar água | Seguir, Ficar, Nadar |
| Cristal | Quest reparo | Iluminar áreas | Seguir, Ficar, Brilhar |

**Design Philosophy:**

- **Unique Utility**: Cada seguidor resolve problemas específicos
- **Quest Integration**: Origem através de narrativa significativa
- **Simple Commands**: Interface não-complexa mantém foco no gameplay


#### 10.2 IA de Companheiros

O sistema de IA é projetado para ser útil sem ser intrusivo:

```csharp
// Estados da IA
enum FollowerState 
{
    Following,      // Seguindo em formação
    Commanded,      // Executando comando específico
    Cooperative,    // Participando de puzzle
    Resting,        // Recuperando energia
    Distressed      // Preso ou em perigo
}
```

**AI Behavior Priorities:**

1. **Segurança**: Evitar perigos e se manter próximo ao slime
2. **Utilidade**: Reconhecer oportunidades para usar habilidades especiais
3. **Formação**: Manter posicionamento organizado
4. **Responsividade**: Responder rapidamente a comandos

#### 10.3 Sistema de Mini-Slimes

Os mini-slimes representam uma extensão única do protagonista, oferecendo versatilidade tática:


| Estágio | Quantidade | Duração | Custo Energia | Cooldown |
| :-- | :-- | :-- | :-- | :-- |
| Young | 1 | 45s | 30% | 45s |
| Adult | 2 | 60s | 25% | 30s |
| Elder | 3 | 90s | 20% | 20s |

**Especialização Elemental**:

- **Terra**: +25% peso, quebra obstáculos pequenos
- **Água**: Nada 2x mais rápido, não se afoga
- **Fogo**: Ilumina 3x maior área, derrete gelo
- **Ar**: Move-se 100% mais rápido, plana pequenas distâncias

**Balancing Philosophy:**
Mini-slimes são poderosos mas temporários e custosos, requerindo decisões estratégicas sobre quando e como usá-los.

### 11. **Sistema de UI**

O sistema de interface em **The Slime King** é projetado seguindo os princípios de design minimalista e contextual, garantindo que a UI apoie a experiência sem dominá-la. Cada elemento tem um propósito específico e aparece apenas quando necessário.

A UI deve ser informativa sem ser intrusiva, mantendo a atmosfera cozy através de design visual suave e não-agressivo.

#### 11.1 HUD Elements

A disposição dos elementos foi cuidadosamente planejada para seguir padrões de leitura natural:


| Elemento | Posição | Conteúdo | Comportamento |
| :-- | :-- | :-- | :-- |
| Barra de Vida | Superior Esquerda | HP atual/máximo | Animação suave de mudança |
| Energia Elemental | Superior Direita | 4 barras coloridas | Pulse ao absorver |
| Inventário | Inferior Centro | 1-4 slots | Escala com crescimento |
| Seguidores | Inferior Esquerda | Ícones + status | Aparece quando tem seguidores |
| Crescimento | Superior Centro | Barra de progresso | Glow ao aproximar de evolução |

**Layout Rationale:**

- **Saúde no canto superior esquerdo**: Posição tradicional e facilmente visível
- **Energia no canto superior direito**: Balanceia a composição visual
- **Inventário centralizado**: Fácil acesso visual para item ativo
- **Seguidores próximo ao inventário**: Agrupamento de informações do "grupo"


#### 11.2 Configurações de Canvas

```csharp
// Canvas Scaler Settings
UI Scale Mode: Scale With Screen Size
Reference Resolution: 1920x1080
Screen Match Mode: Match Width Or Height
Match: 0.5
```

**Technical Considerations:**
Esta configuração garante que a UI seja legível em diferentes resoluções e aspect ratios, mantendo proporções adequadas em todas as plataformas.

#### 11.3 Responsividade

O sistema adapta-se automaticamente a diferentes métodos de input:

- **PC**: Mouse + Keyboard navigation
- **Console**: D-pad navigation com auto-focus
- **Mobile**: Touch controls com área aumentada
- **Escala**: 100%-150% ajustável para acessibilidade


### 12. **Sistema de Áudio**

O sistema de áudio em **The Slime King** é fundamental para criar e manter a atmosfera cozy que define a experiência. Cada som foi escolhido e calibrado para contribuir para um ambiente sonoro relaxante e imersivo.

O audio design segue uma filosofia de "menos é mais", onde cada som tem propósito específico e contribui para a harmonia geral da experiência.

#### 12.1 Triggers Sonoros

Cada ação importante possui feedback audio correspondente:


| Ação | Som | Volume | Pitch Variation |
| :-- | :-- | :-- | :-- |
| Movimento Slime | Squish suave | 0.3 | ±0.1 por tamanho |
| Absorção Fragmento | Tom musical | 0.5 | Por elemento |
| Crescimento | Harmonia ascendente | 0.8 | - |
| Ataque | Whoosh | 0.4 | ±0.2 |
| Comando Seguidor | Chirp/Call | 0.3 | Por tipo |

**Audio Design Principles:**

- **Musical elements**: Sons harmônicos ao invés de ruídos
- **Gentle volume**: Nunca agressivo ou perturbador
- **Pitch variation**: Adiciona organicidade e evita repetição


#### 12.2 Mixagem Dinâmica

```csharp
// Audio Mixer Groups
Master
├── Music (0.7)
├── SFX (0.8)
│   ├── Player (0.6)
│   ├── Followers (0.4)
│   └── Environment (0.5)
└── UI (0.5)
```

**Hierarchical Mixing:**
Esta estrutura permite controle granular sobre diferentes categorias de som, facilitando balanceamento e oferecendo opções de customização para jogadores.

#### 12.3 Estados Musicais

A música responde dinamicamente ao estado do jogo:

- **Exploração**: Ambient loop com instrumentos orgânicos
- **Puzzle**: Camada adicional com tensão suave
- **Crescimento**: Stinger musical + retorno ao loop
- **Seguidores**: Harmonias adicionais baseadas no número ativo


### 13. **Sistema de Efeitos Visuais**

O sistema de efeitos visuais em **The Slime King** serve múltiplas funções: feedback de gameplay, reforço atmosférico e celebração de momentos importantes. Cada efeito é calibrado para ser visualmente satisfatório sem ser overwhelming.

O design visual segue a estética pixel art do jogo, garantindo coerência estética enquanto adiciona polimento e responsividade visual.

#### 13.1 Partículas

| Efeito | Trigger | Duração | Configuração |
| :-- | :-- | :-- | :-- |
| Absorção Elemental | Fragmento coletado | 1s | Trail + burst por cor |
| Crescimento | Evolution trigger | 2s | Espiral ascendente |
| Stealth | Enter/Exit hiding | 0.5s | Fade particles |
| Comando Seguidor | Command issued | 0.3s | Directional sparkles |

#### 13.2 Pós-Processamento

| Efeito | Uso | Intensidade | Trigger |
| :-- | :-- | :-- | :-- |
| Bloom | Elementos mágicos | 0.3 | Sempre ativo |
| Vignette | Stealth state | 0.2 | isHiding = true |
| Color Grading | Atmosfera regional | Variable | Por região |

#### 13.3 Shaders Customizados

- **Outline Shader**: Para destacar objetos interativos
- **Dissolve Shader**: Para efeitos de crescimento/transformação
- **Water Shader**: Para áreas aquáticas com movimento
- **Crystal Shader**: Para elementos cristalinos com refração


### 14. **Sistema de Persistência**

O sistema de persistência garante que o progresso do jogador seja preservado de forma segura e eficiente. Em **The Slime King**, onde crescimento e relacionamentos são centrais, a persistência robusta é crucial para manter a confiança do jogador.

O sistema é projetado para ser à prova de falhas, com multiple safeguards contra perda de dados e corrupção.

#### 14.1 Save Data Structure

```csharp
[System.Serializable]
public class SaveData 
{
    // Player Data
    public int currentStage;
    public Vector3 position;
    public float[] elementalEnergy;
    public int[] elementalPassiveBonuses; // Novo: bônus passivos por elemento
    public InventoryData inventory;
    
    // World Data
    public List<string> destroyedObjects;
    public List<QuestData> completedQuests;
    public List<FollowerData> activeFollowers;
    
    // Settings
    public float musicVolume;
    public float sfxVolume;
    public int uiScale;
}
```


#### 14.2 Auto-Save Triggers

O sistema salva automaticamente em momentos estratégicos:

- **Crescimento**: Após evolução completa
- **Nova Região**: Ao entrar em nova área
- **Quest Completa**: Após ganhar novo seguidor
- **Tempo**: A cada 5 minutos de gameplay
- **Checkpoints**: Em pontos de descanso específicos


#### 14.3 Loading States

- **New Game**: Inicialização com valores padrão
- **Continue**: Carregamento do save mais recente
- **Load Specific**: Seleção de slot de save específico
- **Backup**: Sistema de backup automático para prevenir perda


### 15. **Sistema de Localização de Textos**

O sistema de localização em **The Slime King** foi projetado para oferecer uma experiência multilíngue fluida e acessível, garantindo que jogadores de diferentes regiões possam desfrutar completamente da atmosfera cozy e narrativa do jogo. O sistema utiliza arquivos CSV para facilitar a tradução e manutenção dos textos.

Este sistema é fundamental para o alcance global do jogo, permitindo que a experiência emocional e os vínculos narrativos sejam preservados independente do idioma do jogador.

#### 15.1 Estrutura de Arquivos CSV

O sistema utiliza arquivos CSV organizados para facilitar tanto o desenvolvimento quanto o processo de tradução por terceiros.

**Organização dos Arquivos CSV**

Todos os textos do jogo são armazenados em arquivos CSV localizados no diretório:

```
/Assets/StreamingAssets/Localization/
```

**Estrutura do Arquivo CSV**


| Key | EN | PT_BR | ES | FR | DE | JA | ZH_CN |
| :-- | :-- | :-- | :-- | :-- | :-- | :-- | :-- |
| ui_start_game | Start Game | Iniciar Jogo | Iniciar Juego | Commencer | Spiel Starten | ゲーム開始 | 开始游戏 |
| ui_continue | Continue | Continuar | Continuar | Continuer | Weiter | 続ける | 继续 |
| ui_settings | Settings | Configurações | Configuración | Paramètres | Einstellungen | 設定 | 设置 |
| item_apple_name | Apple | Maçã | Manzana | Pomme | Apfel | りんご | 苹果 |
| item_apple_desc | Restores 20 HP | Restaura 20 de vida | Restaura 20 de vida | Restaure 20 PV | Stellt 20 LP wieder her | HPを20回復 | 恢复20生命值 |

**Convenções de Nomenclatura para Keys**

- **UI Elements**: `ui_[elemento]_[acao]` (ex: `ui_button_start`, `ui_menu_pause`)
- **Items**: `item_[nome]_[propriedade]` (ex: `item_apple_name`, `item_sword_description`)
- **Diálogos**: `dialog_[npc]_[contexto]_[numero]` (ex: `dialog_fairy_greeting_01`)
- **Tutoriais**: `tutorial_[sistema]_[passo]` (ex: `tutorial_movement_basic`)
- **Mensagens**: `message_[tipo]_[contexto]` (ex: `message_error_save`, `message_success_evolution`)

**Idiomas Suportados**


| Código | Idioma | Região |
| :-- | :-- | :-- |
| EN | English | Global |
| PT_BR | Português | Brasil |
| ES | Español | América Latina |
| FR | Français | França |
| DE | Deutsch | Alemanha |
| JA | 日本語 | Japão |
| ZH_CN | 中文 | China |

#### 15.2 Detecção Automática de Idioma

O sistema implementa detecção automática inteligente do idioma preferido do usuário com fallbacks apropriados.

**Hierarquia de Detecção de Idioma**

1. **Configuração do Usuário**: Verificar `config.json` para preferência explícita
2. **Idioma do Sistema**: Detectar idioma configurado no dispositivo
3. **Fallback Regional**: Se idioma específico não disponível, usar variante regional
4. **Fallback Global**: Se nenhuma opção anterior funcionar, usar Inglês (EN)

**Implementação Técnica**

```csharp
public class LocalizationManager : MonoBehaviour
{
    [Header("Configuração")]
    public string defaultLanguage = "EN";
    public string csvFileName = "localization.csv";
    
    private Dictionary<string, Dictionary<string, string>> localizationData;
    private string currentLanguage;
    private GameConfig gameConfig;
    
    void Start()
    {
        LoadCSVData();
        SetLanguage(DetermineLanguage());
    }
    
    private string DetermineLanguage()
    {
        // 1. Verificar configuração do usuário
        if (gameConfig != null && !string.IsNullOrEmpty(gameConfig.preferredLanguage))
        {
            if (IsLanguageSupported(gameConfig.preferredLanguage))
                return gameConfig.preferredLanguage;
        }
        
        // 2. Detectar idioma do sistema
        string systemLanguage = GetSystemLanguage();
        if (IsLanguageSupported(systemLanguage))
            return systemLanguage;
        
        // 3. Tentar fallback regional
        string regionalFallback = GetRegionalFallback(systemLanguage);
        if (IsLanguageSupported(regionalFallback))
            return regionalFallback;
        
        // 4. Usar fallback global (Inglês)
        return defaultLanguage;
    }
    
    private string GetSystemLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Portuguese: return "PT_BR";
            case SystemLanguage.Spanish: return "ES";
            case SystemLanguage.French: return "FR";
            case SystemLanguage.German: return "DE";
            case SystemLanguage.Japanese: return "JA";
            case SystemLanguage.ChineseSimplified: return "ZH_CN";
            default: return "EN";
        }
    }
    
    private string GetRegionalFallback(string language)
    {
        // Exemplos de fallbacks regionais
        if (language.StartsWith("PT")) return "PT_BR";
        if (language.StartsWith("ES")) return "ES";
        if (language.StartsWith("ZH")) return "ZH_CN";
        return language;
    }
}
```


#### 15.3 Sistema de Configuração

O sistema utiliza um arquivo `config.json` para armazenar configurações do jogo, incluindo preferências de idioma do usuário.

**Estrutura do config.json**

```json
{
    "gameSettings": {
        "version": "1.0.0",
        "preferredLanguage": "",
        "audioSettings": {
            "masterVolume": 1.0,
            "musicVolume": 0.8,
            "sfxVolume": 0.9,
            "uiVolume": 0.7
        },
        "videoSettings": {
            "fullscreen": true,
            "resolution": "1920x1080",
            "vsync": true,
            "targetFramerate": 60
        },
        "accessibilitySettings": {
            "uiScale": 1.0,
            "highContrast": false,
            "reduceMotion": false,
            "subtitlesEnabled": true
        },
        "inputSettings": {
            "autoDetectInputType": true,
            "keyboardMouseEnabled": true,
            "gamepadEnabled": true
        }
    }
}
```


---

## **Observações de Implementação Atualizadas**

### **Convenções de Nomenclatura**

A consistência na nomenclatura é fundamental para manutenibilidade do código e colaboração efetiva da equipe:

- GameObjects: camelCase (ex: `slimeBaby`)
- Scripts: PascalCase (ex: `PlayerMovement`, `LocalizationManager`)
- Variáveis públicas: camelCase (ex: `moveSpeed`, `localizationKey`)
- Variáveis privadas: camelCase com _ (ex: `_currentHP`, `_localizationData`)
- Constantes: UPPER_CASE (ex: `MAX_FOLLOWERS`, `DEFAULT_LANGUAGE`)
- Chaves de Localização: snake_case (ex: `ui_start_game`, `dialog_fairy_greeting_01`)


### **Configurações Específicas para PC**

#### **Detecção Automática de Input**

O sistema PC deve ser flexível e responsivo a diferentes métodos de entrada:

- O sistema deve detectar automaticamente se o jogador está usando teclado ou gamepad
- Prompts de UI devem alternar dinamicamente entre ícones de teclado e gamepad
- Suporte simultâneo para jogadores que alternam entre dispositivos
- Textos de UI adaptam-se automaticamente ao idioma selecionado


#### **Configuração Unity Input System**

```csharp
// Exemplo de configuração para múltiplos dispositivos
[InputAction("Attack")]
[SerializeField] private InputAction attackAction;

void Start()
{
    // Configurar bindings para múltiplos dispositivos
    attackAction.AddBinding("<Keyboard>/space");
    attackAction.AddBinding("<Gamepad>/buttonSouth"); // B no Xbox
    attackAction.Enable();
}
```


#### **Controles de PC Específicos**

- **Scroll Mouse**: Funciona como alternativa ao D-pad para mudança de itens
- **Tab**: Alternativa de teclado para navegação em inventário
- **ESC**: Equivalente ao botão Menu para abrir pausa