# The Slime King - Game Design Document

## 1. Conceito e Visão Geral

### Conceito Central
**The Slime King** é um RPG de aventura em 2D top-down com pixel art, que propõe uma jornada de crescimento, descoberta e pertencimento em um mundo de fantasia habitado por criaturas lendárias conhecidas como Reis. O jogador assume o papel de um raro slime branco recém-nascido, guiando-o desde sua forma mais frágil até se tornar o lendário Rei Slime.

### Premissa Narrativa
A história se inicia com o slime acordando em uma caverna escura e protegida, após escapar de uma criatura agressiva. Vulnerável e sozinho, ele busca uma saída segura, dando início à sua jornada de sobrevivência e autodescoberta.

**Progressão Narrativa:**
- **Objetivo Inicial:** Sobreviver e encontrar uma rota de fuga da caverna
- **Desenvolvimento:** Aprender que não precisa enfrentar os perigos sozinho
- **Formação de Laços:** Criar uma pequena família de slimes companheiros através de atos de bondade

### Experiência de Jogo
- **Atmosfera Cozy e Contemplativa:** Ambiente acolhedor sem pressão de tempo
- **Rejogabilidade:** Desafios repetíveis com recompensas variadas
- **Metroidvania Progressivo:** Crescimento desbloqueia novas áreas em regiões já exploradas
- **Mecânica de Seguidores:** Companheiros auxiliam em desafios cooperativos

## 2. Configurações Técnicas

### Ferramentas e Engine
- **Unity 6:** Versão mais recente com melhorias para jogos 2D
- **Universal Render Pipeline (URP):** Otimizado para performance multiplataforma
- **Input System:** Sistema moderno de entrada com suporte universal

### Performance Target
- **PC High-End:** 120 FPS como objetivo principal
- **Consoles:** 60 FPS estáveis

### Otimizações
- **Object Pooling:** Para inimigos, projéteis e efeitos visuais
- **Resource Caching:** Cache inteligente de recursos frequentemente usados

## 3. Personagem e Progressão

### Sistema de Níveis
- **Progressão:** Níveis aumentam conforme coleta de energia elemental
- **Fórmula de Atributos:** Atributo Atual = Valor Base × Nível Atual
- **Evoluções Visuais:**
  - **Nível 5:** Primeira evolução - sprites maiores e mais detalhados
  - **Nível 15:** Segunda evolução - sprites ainda maiores com efeitos visuais
  - **Nível 30:** Terceira evolução - sprites muito maiores e máximo detalhamento

### Atributos Base
- **Pontos de Vida (PV):** Dano suportado antes da derrota
- **Defesa:** Reduz dano recebido
- **Ataque Básico:** Dano de ataques normais
- **Ataque Especial:** Dano de ataques especiais

## 4. Controles e Interface

### Esquema de Controle (Gamepad)

| Botão | Função | Descrição |
|:------|:-------|:----------|
| **L Stick** | Movimentação | Move o slime em oito direções |
| **D-Pad** | Elementos/Ataques | Direita/Esquerda: muda elemento; Cima/Baixo: muda ataque especial |
| **A** | Atacar | Executa ataque básico |
| **B** | Interagir | Interage com pontos do cenário |
| **X** | Abaixar | Esconde atrás de objetos (não pode se mover) |
| **Y** | Ataque Especial | Executa ataque especial do elemento selecionado |
| **LB/LT/RB/RT** | Usar Itens | Usa itens dos slots 1, 2, 3 e 4 respectivamente |
| **Menu** | Opções | Abre menu de configurações e salvar |
| **Inventário** | Inventário | Gerencia itens e slots |

### Design da Interface
- **Estética Cozy:** Tons pastéis e cores suaves
- **Bordas Orgânicas:** Formas arredondadas e naturais
- **Animações Suaves:** Transições gentis sem quebrar imersão
- **Responsividade:** Ajuste automático para diferentes resoluções
- **Navegação Universal:** Funciona com teclado, mouse, gamepad e touch

## 5. Sistema de Combate

### Cálculo de Dano
**Fórmula:** Dano Recebido = Máx[(Ataque do Atacante – Defesa do Alvo), 0]

### Sistema de Tags e Colisões

| Tag | Aplicação | Função |
|:----|:----------|:-------|
| **Destructible** | Objetos destrutíveis | Identifica objetos que podem ser destruídos |
| **Enemy** | Inimigos | Identifica entidades hostis ao jogador |
| **Attack** | Ataques do slime | Ataques básicos do jogador |
| **SpecialAttack** | Ataques especiais do slime | Ataques especiais do jogador |
| **EnemyAttack** | Ataques básicos de inimigos | Ataques básicos dos inimigos |
| **EnemySpecialAttack** | Ataques especiais de inimigos | Ataques especiais dos inimigos |

### Regras de Colisão
- **Ataques do Slime:** Attack/SpecialAttack colidem com Enemy/Destructible
- **Ataques dos Inimigos:** EnemyAttack/EnemySpecialAttack colidem com o slime
- **Após colisão:** Cálculo de dano e destruição do objeto de ataque

## 6. Sistema de Áudio

### Feedback Sonoro

| Ação | Efeito Sonoro | Observação |
|:-----|:--------------|:-----------|
| **Movimentação** | Passos variados por terreno | Volume e tom mudam conforme o piso |
| **Ataque** | Sons de ataque básico/especial | Diferentes para cada tipo de ataque |
| **Receber Dano** | Impacto e dano | Sons distintos para slime, inimigos e objetos |
| **Coletar Objetos** | Som de coleta satisfatório | Feedback rápido e positivo |

## 7. Mecânicas de Jogo

### Interações Especiais (Botão B)
- **Ponto de Esgueirar:** Animação de esgueiro + deslocamento automático
- **Ponto de Pulo:** Animação de pulo + transporte automático
- **Ponto de Empurra:** Animação de empurrar + deslocamento de objetos
- **Ponto de Diálogo:** Inicia diálogos com caixa de texto e emoticons

### Sistema de Inventário
- **Capacidade:** 4 itens carregáveis
- **Acesso Rápido:** Botões LB, LT, RB, RT para usar itens
- **Gerenciamento:** Menu dedicado para organizar itens

### Estados do Personagem
- **Idle:** Animação de repouso quando parado
- **Movimento:** Animação ativa durante deslocamento
- **Abaixar:** Esconde atrás de objetos, impede movimento
- **Ataque:** Animações específicas para ataques básicos e especiais

## 8. Diretrizes de Desenvolvimento

### Modificadores de Atributos
- Todos os atributos podem receber modificadores de várias origens
- Tipos: aditivos (soma/subtração) ou multiplicativos (percentual)
- Origens: buffs, debuffs, equipamentos, efeitos temporários, habilidades

### Instanciação de Ataques
1. **Execução:** Ação de ataque → Instanciação do objeto com tag apropriada
2. **Colisão:** Detecção de colisão com alvos válidos
3. **Dano:** Cálculo seguindo fórmula de dano
4. **Destruição:** Objeto de ataque é destruído após colisão

### Recomendações Técnicas
- **Testes de Performance:** Verificar estabilidade nos targets de FPS
- **Acessibilidade:** Opções de ajuste de escala, contraste e feedback auditivo
- **Documentação:** Manter padrões visuais e técnicas atualizadas
- **Consistência:** Aplicar tags corretamente para funcionamento dos sistemas