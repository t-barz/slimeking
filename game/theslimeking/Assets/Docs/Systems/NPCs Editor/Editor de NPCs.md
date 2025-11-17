O Editor de NPCs deve ser uma view que permite configurar um Prefab/GameObject para atuar de forma independente no jogo e responder à determinados eventos.
Além de configurar o Prefab, deve-se também criar um Behavior Graph para controlar a transição de comportamentos.
### Estrutura do Editor

1. **Movimentação**
   - Checkbox para ficar parado.
   - Lista visual editável para pontos da patrulha fixa.
   - Campos para patrulha em área circular (centro e raio).
   - Tempo entre pontos sorteados na área circular.
   - Configuração para ativar ataque, escolher tags de alvos e distância.

2. **Atributos**
   - Campos para HP, Defesa, Velocidade e Ataque.
   - Lista de itens e taxas para drop pós-derrota.

3. **Interações**
   - Configurar entrega de item com condições.
   - Referência ou ID para ativação de quests.
   - Definição de pares de troca de itens.

4. **Eventos e Condições**
   - Configuração simples de gatilhos para alterar comportamento, enviar eventos para o Behavior Graph, executar diálogos ou ações.

### Funcionamento no Runtime

- O Behavior Graph lê e utiliza os dados configurados para controlar o NPC.
- Mudanças no editor refletem diretamente nos parâmetros usados pelo Behavior Graph.
- Eventos gerados pelas interações remetem entradas para o Behavior Graph, que determina ações subsequentes.

---

## Benefícios da Integração com Behavior Graph

- Separação clara entre dados/configurações (editor) e lógica de comportamento (Behavior Graph).
- Facilidade para criar e ajustar comportamentos complexos visualmente e com dados parametrizados.
- Maior controle granular e reusabilidade dos mesmos dados para diferentes NPCs que usam o mesmo Behavior Graph.
- Melhoria na manutenção do código e escalabilidade do sistema de NPCs.
