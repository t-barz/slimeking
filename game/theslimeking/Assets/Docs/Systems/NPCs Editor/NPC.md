# Documento de Definição de NPC para Jogo 2D Top-Down na Unity 6.2 com Behavior Graph

---

## Definição de NPC

NPC (Non-Playable Character) é um personagem do jogo não controlado diretamente pelo jogador, gerenciado por scripts e/ou sistemas visuais de comportamento como o **Behavior Graph da Unity**. Nesse contexto, o NPC tem suas ações e decisões definidas em comunhão com o Behavior Graph, que orquestra estados, transições e ações baseando-se nas configurações feitas.

### Função do Behavior Graph

- Gerenciar estados do NPC (Idle, Patrulha, Ataque, Interação), transições entre eles e controle detalhado do fluxo de comportamento.
- Executar comandos de movimentação, anime e ações conforme dados configurados no editor.
- Prover flexibilidade para criar comportamentos complexos por meio de uma interface visual sem necessidade de alterar código diretamente.

---

## Movimentação e Atributos de NPC Integrados ao Behavior Graph

- **Movimentação:**
  - Ficar parado
  - Patrulha por pontos fixos (rota definida no editor e usada pelo Behavior Graph)
  - Patrulha em área circular (definida por centro e raio no editor, Behavior Graph controla sorteio e navegação)
  - Ataque a alvos (Behavior Graph escuta detecção de alvos e executa ações de perseguição e ataque)

- **Atributos:**
  - HP, Defesa (valor inteiro, não porcentagem), Velocidade e Ataque definidos no editor afetam as decisões e resultados do Behavior Graph, como resistência em combate e velocidade de movimentação.
  - Pontos de Relacionamento com o jogador (definidos no editor) influenciam interações e diálogos gerenciados pelo Behavior Graph. Números acima de 10 indicam amizade, abaixo de 0 indicam hostilidade.

- **Interações e Eventos:**
  - Entrega de itens, ativação de quests e trocas configuradas no editor disparam eventos que o Behavior Graph utiliza para mudar comportamento.

- **Drop de itens:** Listagem definida no editor usada para spawn no evento "derrota".

---
