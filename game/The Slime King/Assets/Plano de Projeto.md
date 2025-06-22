<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" class="logo" width="120"/>

# The Slime King - Game Design Document Completo e Plano de Desenvolvimento

## 1. Visão Geral do Projeto

### Conceito Central

**The Slime King** é um jogo de aventura-RPG 2D top-down com visual pixel art que combina exploração relaxante, crescimento orgânico do personagem e descoberta cooperativa [^1]. O jogador controla um slime branco recém-nascido em sua jornada de crescimento através de um mundo governado por criaturas lendárias conhecidas como Reis [^1].

### Elementos Únicos

- **Crescimento Visual Progressivo**: O slime evolui fisicamente de forma visível, mudando tamanho, aparência e capacidades [^1]
- **Sistema de Absorção Elemental**: Mecânica única de coleta de fragmentos elementais para crescimento e habilidades [^1]
- **Sistema de Seguidores Cooperativos**: Companheiros únicos que auxiliam em puzzles e criam vínculos emocionais [^1]
- **Experiência Cozy**: Atmosfera relaxante sem elementos estressantes ou pressão de tempo [^1]


### Plataformas Alvo

- **Primária**: PC (Steam) com Unity 6 e URP 2D [^1]
- **Secundária**: Nintendo Switch, PlayStation, Xbox [^1]
- **Especificações Técnicas**: Unity 6.0+, URP 2D, Pixel Art, Input System novo [^1]


## 2. Mecânicas Principais

### Sistema de Crescimento em 4 Estágios

| Estágio | Energia Necessária | Slots Inventário | Seguidores Máx | Habilidades Especiais |
| :-- | :-- | :-- | :-- | :-- |
| Baby Slime | 0 | 1 | 0 | Squeeze básico |
| Young Slime | 200 | 2 | 1 | Bounce, elementais básicas |
| Adult Slime | 600 | 3 | 3 | Divisão, elementais intermediárias |
| Elder/King | 1200 | 4 | 4 | Grande Divisão, elementais avançadas |

### Sistema de Absorção Elemental

**Tipos de Energia** [^1]:

- **Terra (\#8B4513)**: +1 Defense a cada 10 pontos, quebrar rochas
- **Água (\#4169E1)**: +1 Regeneração a cada 10 pontos, nadar/crescer plantas
- **Fogo (\#FF4500)**: +1 Attack a cada 10 pontos, iluminar/derreter
- **Ar (\#E6E6FA)**: +1 Speed a cada 10 pontos, planar/ativar eólicos

**Fragmentos Elementais** [^1]:

- **Small Fragment**: 1 ponto de energia, mais comum
- **Medium Fragment**: 3 pontos de energia, frequência média
- **Large Fragment**: 7 pontos de energia, mais raro


### Sistema de Combate Cozy

O combate foca em neutralização ao invés de destruição, mantendo a atmosfera relaxante [^1]. Utiliza sistema de dano: `realDamage = max(baseDamage - defense, 1)` garantindo progresso sempre [^1].

**Tipos de Ataque** [^1]:

- **Básico**: 10 + Nível de dano, 0.5s cooldown
- **Dash**: 15 + Nível de dano, 1.0s cooldown
- **Especial**: 20 + Especial de dano, 2.0s cooldown


## 3. Sistemas de Interação

### Objetos Interativos

Sistema abrangente que cria mundo vivo e responsivo [^1]:

- **Feedback Visual**: Outline colorido e ícones superiores dinâmicos
- **Tipos**: Ativação, Coleta, Diálogo, Puzzle, Portal
- **Detecção**: OverlapCircle contínuo com ranges configuráveis


### Sistema de Portais

Mecânica elegante para deslocamento entre áreas [^1]:

- **Tipos de Ativação**: Por toque (automático) ou por interação
- **Destinos**: Mesma cena ou inter-cena
- **Efeitos**: Transições suaves com feedback visual


### Objetos Destrutíveis e Móveis

- **Destrutíveis**: Sistema de defesa e HP, drops configuráveis [^1]
- **Móveis**: Requisitos de proximidade e direcionamento, rastros opcionais [^1]


## 4. Navegação e Exploração

### Design Metroidvania

Mundo interconectado onde crescimento abre novas possibilidades [^1]:

- **Layouts Legíveis**: Caminhos claros e visuais intuitivos
- **Múltiplos Caminhos**: Rotas baseadas em estágio e habilidades
- **Segredos Escondidos**: 2-3 segredos por área principal


### Regiões Principais [^1]

1. **Caverna Inicial**: Tutorial, Terra, escape
2. **Floresta Sussurrante**: Primeira liberdade, Ar/Natureza
3. **Cavernas Cristalinas**: Terra/Cristal, exploração não-linear
4. **Lagos Serenos**: Água/Gelo, mecânicas aquáticas
5. **Picos Ventosos**: Ar, desafios verticais
6. **Núcleo Elemental**: Todos elementos, desafio final

## 5. Sistemas Técnicos

### Localização de Textos

Sistema multilíngue com arquivos CSV [^1]:

- **Idiomas Suportados**: EN, PT_BR, ES, FR, DE, JA, ZH_CN
- **Detecção Automática**: Config usuário > Sistema > Regional > EN
- **Estrutura**: Key, idiomas em colunas separadas


### Interface e Feedback Visual

- **HUD Elements**: Vida, energia elemental, inventário, seguidores [^1]
- **Ícones Superiores**: Dinâmicos baseados no dispositivo de entrada [^1]
- **Canvas**: Scale With Screen Size, 1920x1080, Match=0.5 [^1]


### Audio e Efeitos Visuais

**Sistema de Áudio** [^1]:

- Triggers sonoros para cada ação principal
- Mixagem dinâmica hierárquica
- Sons específicos por elemento e tamanho

**Efeitos Visuais** [^1]:

- Partículas para absorção, crescimento, stealth
- Pós-processamento: Bloom, Vignette, Color Grading
- Shaders customizados para outline e transformações


## 6. Plano de Desenvolvimento Estruturado

### FASE 1: FUNDAMENTOS TÉCNICOS (Semanas 1-4)

#### Semana 1: Sistema de Localização

**Tarefas Prioritárias:**

- [ ] Criar LocalizationManager singleton [^1]
- [ ] Implementar detecção automática de idioma [^1]
- [ ] Configurar estrutura CSV em StreamingAssets/Localization/ [^1]
- [ ] Desenvolver sistema de fallbacks (User > System > Regional > EN) [^1]
- [ ] Criar config.json para configurações do jogador [^1]

**Entregáveis:**

- LocalizationManager funcional
- Arquivo CSV de exemplo com pelo menos 50 strings
- Sistema de detecção de idioma testado


#### Semana 2: Sistema de Ícone Superior

**Tarefas Prioritárias:**

- [ ] Implementar DeviceDetector para input automático [^1]
- [ ] Criar prefabs de ícones para cada plataforma [^1]
- [ ] Desenvolver sistema de fade-in/fade-out (0.3s) [^1]
- [ ] Integrar troca dinâmica baseada no dispositivo [^1]

**Entregáveis:**

- IconManager funcional
- Ícones para Keyboard, Xbox, PlayStation, Switch
- Sistema de detecção automática testado


#### Semana 3: Configuração Unity Base

**Tarefas Prioritárias:**

- [ ] Configurar URP 2D pipeline [^1]
- [ ] Implementar Pixel Perfect Camera [^1]
- [ ] Configurar Input System com esquemas múltiplos [^1]
- [ ] Estabelecer convenções de nomenclatura [^1]


#### Semana 4: Testes e Integração Fase 1

**Tarefas:**

- [ ] Testes de localização em diferentes idiomas
- [ ] Validação de ícones em diferentes dispositivos
- [ ] Documentação técnica da Fase 1
- [ ] Preparação para Fase 2


### FASE 2: PERSONAGEM E MOVIMENTO (Semanas 5-8)

#### Semana 5: Estrutura do Personagem

**Tarefas Prioritárias:**

- [ ] Criar hierarquia slimeBaby com sprites direcionais [^1]
- [ ] Implementar PlayerVisualManager para visibilidade [^1]
- [ ] Configurar Rigidbody2D e Collider2D [^1]
- [ ] Desenvolver sistema de shadow sempre visível [^1]


#### Semana 6: Sistema de Movimento

**Tarefas Prioritárias:**

- [ ] Implementar Input Actions completo [^1]
- [ ] Configurar física por estágio de crescimento [^1]
- [ ] Desenvolver regras de visibilidade direcional [^1]
- [ ] Criar sistema de flip horizontal para movimento oeste [^1]


#### Semana 7: Sistema de Animação

**Tarefas Prioritárias:**

- [ ] Configurar Animator com 7 parâmetros específicos [^1]
- [ ] Implementar regra crítica: isHiding bloqueia movimento [^1]
- [ ] Criar transições de estado apropriadas [^1]
- [ ] Integrar triggers com Input Actions [^1]


#### Semana 8: Testes e Polish Fase 2

**Tarefas:**

- [ ] Testes de movimento em 8 direções
- [ ] Validação de animações e transições
- [ ] Ajustes de responsividade e feel
- [ ] Preparação para Fase 3


### FASE 3: PROGRESSÃO BÁSICA (Semanas 9-12)

#### Semana 9: Sistema Elemental

**Tarefas Prioritárias:**

- [ ] Criar fragmentos elementais com 3 tamanhos [^1]
- [ ] Implementar sistema de cores dinâmicas [^1]
- [ ] Desenvolver atração magnética (2.0 units, 8.0 units/s) [^1]
- [ ] Configurar DropTable para objetos [^1]


#### Semana 10: Sistema de Crescimento

**Tarefas Prioritárias:**

- [ ] Implementar 4 estágios com thresholds específicos [^1]
- [ ] Criar sequência de evolução com efeitos visuais [^1]
- [ ] Desenvolver mudanças de sprite e escala [^1]
- [ ] Integrar invulnerabilidade temporária [^1]


#### Semana 11: Sistema de Inventário

**Tarefas Prioritárias:**

- [ ] Criar inventário evolutivo (1-4 slots) [^1]
- [ ] Implementar stack máximo de 10 por item [^1]
- [ ] Desenvolver navegação com D-pad [^1]
- [ ] Criar auto-reorganização de slots vazios [^1]


#### Semana 12: Integração e Testes Fase 3

**Tarefas:**

- [ ] Testes de coleta e absorção elemental
- [ ] Validação de crescimento automático
- [ ] Balanceamento de progressão
- [ ] Preparação para Fase 4


### FASE 4: COMBATE E INTERAÇÃO (Semanas 13-16)

#### Semana 13: Sistema de Combate

**Tarefas Prioritárias:**

- [ ] Implementar 3 tipos de ataque [^1]
- [ ] Desenvolver fórmula de dano balanceada [^1]
- [ ] Criar feedback visual (flash, knockback) [^1]
- [ ] Integrar invulnerabilidade pós-hit [^1]


#### Semana 14: Objetos Interativos Base

**Tarefas Prioritárias:**

- [ ] Criar sistema de detecção com Collider2D [^1]
- [ ] Implementar 5 tipos de interação básicos [^1]
- [ ] Desenvolver feedback visual com outline [^1]
- [ ] Integrar ícones superiores contextuais [^1]


#### Semana 15: Objetos Destrutíveis e Móveis

**Tarefas Prioritárias:**

- [ ] Implementar atributos de defesa e HP [^1]
- [ ] Criar sistema de drops configurável [^1]
- [ ] Desenvolver objetos móveis com direcionamento [^1]
- [ ] Integrar sistema de rastro opcional [^1]


#### Semana 16: Testes e Polish Fase 4

**Tarefas:**

- [ ] Testes de combate e balance
- [ ] Validação de interações
- [ ] Ajustes de responsividade
- [ ] Preparação para Fase 5


### FASE 5: NARRATIVA E NAVEGAÇÃO (Semanas 17-20)

#### Semana 17: Sistema de Diálogos

**Tarefas Prioritárias:**

- [ ] Criar interface de diálogo com componentes obrigatórios [^1]
- [ ] Implementar typewriter effect configurável [^1]
- [ ] Integrar com sistema de localização [^1]
- [ ] Desenvolver controles de skip e continue [^1]


#### Semana 18: Sistema de Portal

**Tarefas Prioritárias:**

- [ ] Implementar 2 tipos de ativação (toque/interação) [^1]
- [ ] Criar teleporte intra-cena e inter-cena [^1]
- [ ] Desenvolver efeitos visuais de transição [^1]
- [ ] Integrar com sistema de save [^1]


#### Semana 19: Movimento Especial

**Tarefas Prioritárias:**

- [ ] Implementar sistema Shrink/Squeeze [^1]
- [ ] Criar sistema de Jump com verificação de trajetória [^1]
- [ ] Desenvolver pontos de passagem específicos [^1]
- [ ] Integrar com animações correspondentes [^1]


#### Semana 20: Integração e Testes Fase 5

**Tarefas:**

- [ ] Testes de diálogos multilíngues
- [ ] Validação de portals e movimento especial
- [ ] Ajustes de narrativa e flow
- [ ] Preparação para Fase 6


### FASE 6: SISTEMAS AVANÇADOS (Semanas 21-24)

#### Semana 21: Sistema de Stealth

**Tarefas Prioritárias:**

- [ ] Implementar 4 estados de visibilidade [^1]
- [ ] Criar detecção de cobertura com tags específicas [^1]
- [ ] Desenvolver bloqueio de movimento durante stealth [^1]
- [ ] Integrar feedback visual com vinheta [^1]


#### Semana 22: Sistema de UI Completo

**Tarefas Prioritárias:**

- [ ] Implementar todos os HUD elements [^1]
- [ ] Configurar Canvas com especificações técnicas [^1]
- [ ] Criar responsividade para diferentes resoluções [^1]
- [ ] Integrar animações suaves [^1]


#### Semana 23: Seguidores e Mini-Slimes

**Tarefas Prioritárias:**

- [ ] Implementar 4 tipos de seguidores [^1]
- [ ] Criar IA com 5 estados específicos [^1]
- [ ] Desenvolver sistema de mini-slimes [^1]
- [ ] Integrar especialização elemental [^1]


#### Semana 24: Testes e Polish Fase 6

**Tarefas:**

- [ ] Testes de stealth e seguidores
- [ ] Validação de UI responsiva
- [ ] Balanceamento final de sistemas
- [ ] Preparação para Fase 7


### FASE 7: POLISH E SUPORTE (Semanas 25-28)

#### Semana 25-26: Sistema de Áudio

**Tarefas Prioritárias:**

- [ ] Implementar triggers sonoros para todas as ações [^1]
- [ ] Configurar mixagem dinâmica hierárquica [^1]
- [ ] Integrar variações de pitch por elemento [^1]
- [ ] Criar sistema de áudio responsivo [^1]


#### Semana 27-28: Efeitos Visuais

**Tarefas Prioritárias:**

- [ ] Implementar sistema de partículas completo [^1]
- [ ] Configurar pós-processamento (Bloom, Vignette) [^1]
- [ ] Criar shaders customizados [^1]
- [ ] Integrar efeitos com todas as ações [^1]


### FASE 8: PERSISTÊNCIA E FINALIZAÇÃO (Semanas 29-32)

#### Semana 29-30: Sistema de Persistência

**Tarefas Prioritárias:**

- [ ] Implementar estrutura de SaveData completa [^1]
- [ ] Criar auto-save triggers em pontos específicos [^1]
- [ ] Desenvolver sistema de backup anti-corrupção [^1]
- [ ] Integrar com todos os sistemas implementados [^1]


#### Semana 31-32: Testes Finais e Release

**Tarefas:**

- [ ] Testes completos de integração
- [ ] Validação em múltiplas plataformas
- [ ] Otimização de performance final
- [ ] Preparação para distribuição


## 7. Marcos e Entregáveis

### Marco 1 (Semana 4): Base Técnica

- Sistema de localização funcional
- Detecção de dispositivos implementada
- Pipeline Unity configurado


### Marco 2 (Semana 8): Personagem Jogável

- Slime completamente funcional
- Sistema de movimento responsivo
- Animações integradas


### Marco 3 (Semana 12): Progressão Básica

- Sistema elemental completo
- Crescimento funcionando
- Inventário operacional


### Marco 4 (Semana 16): Interação Completa

- Combate balanceado
- Objetos interativos funcionais
- Mundo responsivo


### Marco 5 (Semana 20): Narrativa Integrada

- Diálogos multilíngues
- Sistema de portais
- Movimento especial


### Marco 6 (Semana 24): Sistemas Avançados

- Stealth operacional
- UI completa
- Seguidores funcionais


### Marco 7 (Semana 28): Polish Completo

- Áudio integrado
- Efeitos visuais finalizados
- Performance otimizada


### Marco 8 (Semana 32): Release Candidate

- Jogo completo e testado
- Multiplataforma validado
- Pronto para distribuição


## 8. Recursos e Equipe Necessária

### Perfis Técnicos Recomendados

- **1 Programador Unity Senior**: Sistemas core e arquitetura
- **1 Programador Unity Pleno**: Gameplay e UI
- **1 Artist 2D**: Pixel art e animações
- **1 Designer de Som**: Áudio e efeitos sonoros
- **1 Game Designer**: Balance e level design


### Ferramentas e Tecnologias

- **Engine**: Unity 6.0+ com URP 2D [^1]
- **Controle de Versão**: Git com LFS para assets
- **Arte**: Aseprite para pixel art e animações
- **Áudio**: FMOD ou Unity Audio Mixer
- **Gestão**: Trello/Jira para task management

Este plano de desenvolvimento estruturado garante implementação incremental e testável, onde cada fase constrói sobre as anteriores, assegurando qualidade e funcionalidade em todos os sistemas de **The Slime King** [^1].

<div style="text-align: center">⁂</div>

[^1]: Documento-de-Regras-Tecnicas.md

[^2]: Game-Design-Document.md

