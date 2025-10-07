# 🗺️ Roadmap Geral - The Slime King

Atualizado para refletir o estado real cruzando: `ProximosPassos.md`, `Mecanismos.md` e **GDD v3.0**. Este roadmap foca em: clareza de status, dependências e alinhamento evolutivo por fase (Demo Alpha → V3).

## 📌 Objetivos

| Objetivo | Descrição |
|----------|-----------|
| Visibilidade | Consolidar evolução incremental e evitar redundâncias |
| Alinhamento | Garantir coerência entre código, GDD e planejamento técnico |
| Priorização | Evidenciar gargalos e dependências críticas |
| Manutenção | Estrutura pronta para atualizações rápidas de status |

## 🗂️ Legenda de Status

| Status | Símbolo | Significado | Critério |
|--------|---------|------------|---------|
| DONE | ✅ | Concluído / estável | Implementado e testado minimamente |
| WIP | 🟡 | Em desenvolvimento | Parcialmente funcional / faltam fluxos |
| PLANNED | 🔜 | Planejado | Definido em documentação sem implementação |
| BACKLOG | 💤 | Adiado | Relevante mas sem prioridade atual |
| REVIEW | 🔍 | Revisar | Reavaliar necessidade / refatorar |

---

## 🚀 Foco Atual (Sprint Ativa)

Todos os sistemas core planejados para a Demo Alpha inicial foram implementados. Sprint atual migra para fase de Validação & Polish.

| Item | Status | Categoria | Notas |
|------|--------|----------|-------|
| Inventory Core (estrutura + coleta) | ✅ | Inventário | Eventos + API pública + integração coleta |
| Item Usage (slots 1–4) | ✅ | Consumíveis | Cooldown + efeitos básicos (HP / Speed) |
| UI Navigation (EventSystem + InputSystemUIInputModule) | ✅ | Input / UI | Funcional (suporte diálogo / HUD) |
| HUD Básico (vida + slots) | ✅ | UI / UX | AlphaHUDManager + InventoryHUD |
| SpecialMovementPoint (despriorizado) | 💤 | Movimento Especial | Backlog após loop estável |
| Enemy System (integração básica) | ✅ | Combate / AI | AlphaEnemyIntegration (FSM completa futura) |
| Growth System | ✅ | Progressão | Estágios + multiplicadores + eventos |
| Skill Tree Base (nós + unlock) | ✅ | Skills | 3 tiers + dependências + efeitos |
| Diálogo Minimal (caixa + avanço) | ✅ | Conversação | Typewriter + bloqueio input gameplay |
| Camera Follow + Bounds | 💤 | Movimento / UX | Cinemachine Follow já disponível no projeto |
| Teleport Point | 💤 | Movimento / Fluxo | Fora de escopo Alpha - implementação futura |
| Rebinding básico (opcional Alpha) | 💤 | QoL | Mantido para pós-polish |

### 📁 Scripts Implementados (Assets/Alpha/)

- ✅ **InventoryCore.cs**
- ✅ **ItemUsageManager.cs**
- ✅ **AlphaEnemyIntegration.cs**
- ✅ **GrowthSystem.cs**
- ✅ **SkillTreeManager.cs**
- ✅ **AlphaHUDManager.cs**
- ✅ **InventoryHUD.cs**
- ✅ **DialogueController.cs**
- ✅ **AlphaItemAdapter.cs** (bridge com `ItemCollectable` legacy em `_OLD/`)
- ✅ **AlphaSetupMenuItems.cs** (ferramentas)

**Removidos do escopo Alpha:**

- 💤 **CameraFollow.cs** (Cinemachine Follow já disponível)
- 💤 **TeleportPoint.cs** (implementação futura específica)

Não há scripts pendentes desta fase; próximos passos focam em testes e refinamento visual.

### 📚 Documentação Detalhada Criada (Assets/Docs/Alpha/)

- ✅ **ImplementationPlan.md** - Estratégia geral de implementação Alpha
- ✅ **InventorySystem.md** - Plano detalhado do sistema de inventário
- ✅ **EnemySystem.md** - Arquitetura FSM para inimigos
- ✅ **ProgressionSystem.md** - Growth System e Skill Tree
- ✅ **UISystem.md** - HUD Manager e sistema de diálogo
- ✅ **CameraAndTeleport.md** - Camera Follow e pontos de teletransporte
- ✅ **AlphaTestChecklist.md** - Checklist completo de validação

---

## 🧪 Demo Alpha (Scope Mecânico Base)

| Sistema (Mecanismos.md) | Status | Alinhamento GDD | Observações |
|-------------------------|--------|------------------|-------------|
| Movimentação & Animação Base | ✅ (OK) | Sec. 11.1 / 2.1 | PlayerController robusto implementado |
| Special Movement (Shrink/Jump) | � (BACKLOG) | GDD (progressão futura) | Despriorizado para Alpha; manter apenas placeholder se necessário |
| Vento | ✅ (OK) | Ambientação biomas | Código legado funcionando (avaliar migração futura) |
| Árvores Reativas | ✅ (OK) | Biomas (Sec. 3) | Interação visual básica concluída |
| Grama & Arbustos (Vegetação) | ✅ (OK) | Feedback ambiental | Responde a vento/contato; revisar otimização culling |
| Pedras Danificáveis | ✅ (OK) | Sistema destrutível | RockDestruct funcional com progressão visual |
| Dropping Items | ✅ (OK) | Itens / Loop coleta | Mecânica base de queda implementada |
| Coleta Automática (Auto Pickup) | ✅ (OK) | UX / Qualidade de Vida | Raio configurável; adicionar VFX polidos depois |
| Coleta de Itens → Inventário | ✅ | Inventário (Sec. 6) | Adapter + InventoryCore ativos |
| Pontos Interativos | ✅ (OK) | Interação contextual | Base + diferenciação de input (teclado/gamepad) |
| Destaque (Outline) | ✅ (OK) | UX Feedback | Já funcional; unificar estilo de shader depois |
| Combate Direcional (Player) | ✅ | Combate (Sec. 10) | PlayerController + AttackHandler base (falta inimigos) |
| Sistema de Inimigos | ✅ | Combate / AI | Integração básica (FSM full futura) |
| Crescimento / Evolução do Slime | ✅ | Sec. 2.2 / 2.3 / 9 | GrowthSystem completo |
| PlayerAttributesSystem | ✅ (parcial) | Sec. 9 | Atributos base + eventos; falta buffs / evolução dinâmica |
| Árvore de Habilidades | ✅ | Skills futura | SkillTreeManager implementado |
| Uso de Itens (Consumíveis) | ✅ | Inventário / Itens | ItemUsageManager funcional |
| Interface/HUD Base | ✅ | UI / UX | AlphaHUDManager + InventoryHUD |
| Câmera que Segue | 💤 | Movimento / UX | Cinemachine Follow já disponível |
| Ponto de Teletransporte | 💤 | Movimento / Level Flow | Fora de escopo Alpha - implementação futura |

---

## 📅 Plano Detalhado Demo Alpha

Objetivo: Entregar uma Demo Alpha jogável até meados de Novembro/2025 (meta: 6 semanas a partir de 07/Out/2025) contendo todos os mecanismos listados como "OK" ou implementados em versão mínima funcional (MVP) conforme `Mecanismos.md`.

### 🎯 Critérios de Aceite Gerais da Demo Alpha

1. Movimento + ataque funcionando (Shrink/Jump backlog controlado).
2. Inimigo simples reage a dano e morre (estados avançados posteriores).
3. Inventário: coleta → slots (4) → uso de consumível com efeitos (vida/velocidade/teste).
4. HUD: vida, slots, feedback coleta/progressão.
5. Diálogo: abre/fecha, typewriter, bloqueio input gameplay.
6. Growth: alteração de estágio aplica modificadores e dispara eventos.
7. Skill Tree: desbloqueio condicionado a estágios, efeitos aplicados.
8. Câmera: Cinemachine Follow já disponível (sistema personalizado desnecessário).
9. Build sem erros de compilação (qa pendente após merge final).
10. Performance aceitável (métricas finais após cena consolidada).

### 🧱 Fases e Sequenciamento

| Fase | Semana (estimada) | Foco | Entregáveis Principais | Dependências | Riscos Principais |
|------|-------------------|------|------------------------|--------------|-------------------|
| 0. Stabilization (parcial) | (Concluída) | Limpeza & Input | Input Asset 2 mapas, remoção System, ajuste PlayerController | — | Divergência asset/classe (mitigado) |
| 1. Core Interactions | Sem 1 (7–11 Out) | Inventário & Coleta | Inventory Core (estrutura), integração Coleta, HUD slots placeholder | ItemCollectable | Escopo inventário inflar |
| 2. Consumíveis | Sem 2 (14–18 Out) | Uso de Itens | ItemUsageManager MVP | Fase 1 | Escopo de efeitos crescer demais |
| 3. Progressão & Inimigos | Sem 3 (21–25 Out) | Enemy + Growth Stub | EnemyController base, Growth System, SkillTree stub | Fase 2 | AI path simples insuficiente |
| 4. UI & Diálogo | Sem 4 (28 Out–1 Nov) | UI Navigation + Diálogo | EventSystem configurado, Diálogo mínimo, HUD refinado | Fase 1 & 3 | UI focus/perda de input |
| 5. Mundo & Fluxo | Sem 5 (4–8 Nov) | Câmera & Teleporte | CameraFollow com bounds, TeleportPoint funcional, polish Visual | Fases 1–4 | Definição de bounds ruim |
| 6. Integração & Polish | Sem 6 (11–15 Nov) | Refino & Testes | Test checklist, balance leve, logs retirados | Todas | Acúmulo bugs tardios |

Meta: Freeze de features ao final da Semana 6; somente hotfixes depois.

### 🔗 Dependências Técnicas (Resumo)

| Sistema | Depende de | Justificativa |
|---------|------------|---------------|
| Inventory Core | ItemCollectable | Fonte de dados de coleta |
| Item Usage | Inventory Core, Input System | Necessita slots e bindings |
| (Removido) | — | — |
| Enemy System | AttackHandler, PlayerAttributesSystem | Reuso lógica de dano/atributos |
| Growth System | PlayerAttributesSystem | Modifica atributos |
| Skill Tree | Growth System | Condições de desbloqueio |
| Diálogo | UI Navigation | Foco e bloqueio de gameplay |
| HUD | Inventory Core, PlayerAttributesSystem | Exibição de vida/slots |
| Camera Follow | PlayerController, Scene Bounds | Posição alvo e limites |
| Teleport Point | Scene Management | Troca de posição/cena |

### ⏱️ Estimativas Detalhadas (Esforço)

Esforço em dias focados (d):

| Tarefa | Estimativa | Notas |
|--------|------------|-------|
| Inventory Core (estrutura + coleta + UI slots básicos) | 2d | Scriptable model + simples UI grid |
| ItemUsageManager MVP (efeito dummy) | 1d | Consumir item + evento buff placeholder |
| (Removido) | — | Excluído do ciclo Alpha |
| Camera Follow + Bounds | 1d | Lerp + clamp em rect; opcional Cinemachine profile |
| TeleportPoint | 0.5d | Trigger + fade opcional (adiável) |
| EnemyController base (FSM simples) | 2d | Estados + dano + morte |
| Growth System stub | 1d | Enum stage + aplicação de modificadores |
| SkillTree stub (1 nó funcional) | 1d | Estrutura dados + unlock flow |
| HUD refinado (vida + slots + feedback coleta) | 1d | Vincular a eventos |
| Diálogo mínimo (caixa + avançar + bloquear input) | 1.5d | UI + state lock |
| UI Navigation (EventSystem + highlight) | 0.5d | Config padrão InputSystemUIInputModule |
| Integração & Polish (test pass, remover logs) | 2d | Checklist + pequenas correções |
| Reserva/Risco (buffer) | 2d | Para overruns |
Total Aproximado | 16d (~3.2 semanas) | Remoção de movimento especial libera margem adicional |

### 🧪 Checklist de Teste (Alpha Pass)

1. Spawn → Movimento → Attack → Enemy reage e morre.
2. Coletar 3 itens → aparecem nos slots → usar item → efeito logado/aplicado.
3. Executar Shrink + atravessar espaço pequeno sem colisão.
4. Jump em ponto válido → animação/curva consistente.
5. Abrir diálogo → bloqueia movimento → fechar → controle retorna.
6. Alterar estágio (simulado) → Skill nó desbloqueado → atributo modificado.
7. Teleport → posição atualizada e câmera recalc aplicada.
8. Camera Follow mantém jogador centralizado sem jitter nas bordas.
9. HUD atualiza vida quando dano recebido (inimigo ataca ou debug).
10. Nenhum log de erro em Console durante ciclo completo.

### ⚠️ Riscos & Mitigações

| Risco | Impacto | Mitigação |
|-------|---------|-----------|
| Escopo de Inventory expandir (UI complexa) | Atraso | Definir MVP: lista interna + 4 slots visuais apenas |
| AI complexidade (pathfinding) | Desvio de esforço | Usar movimento direto (aproximação linear) na Alpha |
| Shrink colisão inconsistências | Soft-lock | Testar com colliders debug + fallback disable collider |
| Skill Tree sobre-engenharia | Retrabalho | Apenas 1 nó + pipeline de extensão futura |
| Diálogo escalando para sistema completo | Atraso UI | MVP sem branching (texto sequencial) |
| Camera jitter pixel art | UX negativa | Pixel Snap ou Cinemachine PixelPerfect conf. |
| Teleport causando clipping | Quebra visual | Pequeno fade-in/out opcional posteriormente |

### 📈 Métricas de Sucesso (Alpha Interna)

| Métrica | Alvo |
|---------|------|
| FPS médio cena teste | ≥ 60 |
| Erros/Exceções Console | 0 no fluxo padrão |
| Tempo médio ciclo teste (checklist) | < 5 min |
| Crash Rate | 0 |
| Tempo inicial para entender controles (playtester interno) | < 1 min |

### ✅ Saída Planejada (Deliverables)

1. Scripts novos: InventoryCore, ItemUsageManager, AlphaEnemyIntegration, GrowthSystem, SkillTreeManager, DialogueController, AlphaHUDManager, InventoryHUD, AlphaItemAdapter.
2. Prefabs: EnemyBasic, TeleportPoint, SpecialMovementPoint (final), HUDCanvas.
3. UI: HUD (vida + slots), DialoguePanel, InventoryPanel simples.
4. Documentação: Atualização `Mecanismos.md`, adicionar `AlphaTestChecklist.md`.
5. Scenes: TestArenaAlpha.unity com todos elementos.

### 🛣️ Caminho Crítico

Inventory Core → Item Usage → HUD → Enemy System / Growth → Skill Tree → Diálogo → Camera & Teleport → Polish.

### 🔄 Revisões Semanais

- Fim de cada semana: atualizar status nas tabelas e burn-down de tarefas restantes.
- Semana 6: somente bugfix, sem novas features.

---

## 🧱 Demo Beta (Ampliação de Sistemas)

| Sistema | Status | Dependências | Observações |
|---------|--------|--------------|-------------|
| Minions (companheiros) | 🔜 | AI / Inventário | Planejado para pós-inimigos básicos |
| Quest / Missões | 🔜 | Diálogo / Save | Estrutura narrativa ainda não iniciada |
| Puzzle Quests | 🔜 | Quest System | Depende de sistema de objetivos modular |
| Sistema de Diálogo | 🔜 | UI / Texto | GDD prevê; nenhuma base técnica |
| Inventory UI Completa | 🔜 | Inventory Core | Após protótipo básico Alpha |

---

## ⚙️ Demo Next (Infraestrutura & Persistência)

| Sistema | Status | Observações |
|---------|--------|-------------|
| Save System (Slots + Autosave) | 🔜 | Conceitual (GDD Sec. 12) |
| Difficulty Manager | 🔜 | Requer baseline de combate e inimigos |
| Opções de Áudio Avançadas | 🔜 | AudioManager parcial; expandir mixer |
| Fishing Minigame | 🔜 | Isolado; pode ser prototipado depois |
| Steam Deck Ready | 🔜 | Performance + Input Profiles |

---

## 🧩 Versão 1 (Lançamento PC / Steam)

| Sistema | Status | Dependências | Notas |
|---------|--------|--------------|-------|
| **Multiplayer Local (até 4 jogadores)** | 🔜 | **Input System, Camera System** | **Sistema completo de co-op split-screen** |
| ├─ Input Múltiplo (4 controles) | 🔜 | Input System base | Gerenciamento independente de 4 PlayerInput |
| ├─ Câmera Dinâmica Split-Screen | 🔜 | Cinemachine | Divisão automática 1/2/3/4 telas conforme jogadores |
| ├─ PlayerController Multi-Instância | 🔜 | PlayerController base | Adaptação para múltiplas instâncias simultâneas |
| ├─ Inventário Compartilhado | 🔜 | Inventory System | Sistema unificado acessível por todos os jogadores |
| ├─ UI Multiplayer (HUD individual) | 🔜 | UI System | HUD separado por jogador (vida, itens, etc.) |
| ├─ Sistema de Join/Leave Dinâmico | 🔜 | Input Detection | Hot-join e hot-leave durante gameplay |
| └─ Balanceamento Co-op | 🔜 | Combat/Progress Systems | Ajuste de dificuldade e progressão para múltiplos jogadores |
| Sistema de Colheita (Harvest) | 🔜 | Time + Resource Systems | Depende de Time + Resource Systems |
| Base Customization | 🔜 | Friendship System | Conectado à progressão social (GDD Sec. 5) |
| Sistema de Recompensas | 🔜 | Drop System | Sinergia com Drops / Achievements |
| Achievements | 🔜 | Save + Tracking | Requer Save + Tracking infra |

---

## 🌐 Versões Futuras (V2 / V3)

| Versão | Sistema | Status | Notas |
|--------|---------|--------|-------|
| V2 | Multiplayer Remoto | 💤 (BACKLOG) | Só após loop core estável |
| V2 | Viagem Rápida (Wagon Travel) | 🔜 | Depende de Scene Streaming |
| V2 | Xbox Ready | 🔜 | Porta técnica + compliance |
| V2 | Photo Mode | 🔜 | Requer Post Processing + Free Camera |
| V3 | Nintendo Switch Ready | 🔜 | Otimizações e input adaptations |
| V3 | PlayStation Ready | 🔜 | Port + compliance |

---

## 🧪 Técnico & Performance Cross-Phase

| Item | Status | Dependências | Observações |
|------|--------|--------------|-------------|
| **Input System Multiplayer** | 🔜 | **Input System base** | **Extensão para suporte 4 jogadores simultâneos** |
| ├─ PlayerInputManager Setup | 🔜 | Unity Input System | Gerenciamento automático de múltiplos PlayerInput |
| ├─ Device Assignment | 🔜 | Input Device Detection | Atribuição automática gamepad/teclado por jogador |
| ├─ Input Conflict Resolution | 🔜 | Input Actions | Evitar conflitos entre inputs de diferentes jogadores |
| └─ Gamepad Hot-Swap | 🔜 | Device Management | Conectar/desconectar gamepads sem quebrar gameplay |
| GC Alloc Minimização | 🔍 (REVIEW) | Performance | Medir pontos quentes após combate / coleta |
| Physics2D Layer Strategy | 🔍 | Physics | Revisar colisões redundantes |
| Input Rebinding Persistência | 🔜 | Input System | Implementar JSON + fallback PlayerPrefs |
| AudioSource Pooling | 🔜 | Audio System | Pré-requisito para expansão de SFX |
| Profiling PlayerController | 🔍 | Performance | Estabelecer baseline antes de otimizar |

---

## ✅ Concluídos (Consolidados)

| Item | Data (aprox.) | Notas |
|------|---------------|-------|
| Input Action Asset (2 mapas) | ✅ | Estrutura estável UI / Gameplay (System removido; ações globais realocadas) |
| PlayerController Core | ✅ | Movimento, combate base, VFX direcionais |
| InputManager Wrapper | ✅ | Contexto UI/System separado |
| TitleScreen Skip | ✅ | Integração via InputManager.OnSkip |
| PlayerAttributesSystem (base) | ✅ | HP/Attack/Defense/Speed + eventos |
| AttackHandler Base | ✅ | Área retangular + offsets dinâmicos |
| Guia InputSystem_Guide.md | ✅ | Documentação estruturada |
| Renomeação Padrões (AttributesSystem) | ✅ | Conformidade Boas Práticas |

---

## 🔄 Próximos Gate Milestones

| Milestone | Critério de Aceite | Bloqueadores Atuais |
|-----------|-------------------|---------------------|
| Alpha Slice Jogável | Movimento, combate, 1 bioma ativo, coleta funcional | Inventory básico, UI Navigation |
| Core Loop Vertical | Save inicial + progressão mínima + pausa | Save System, Pause Menu |
| Beta Expandido | Inimigos + quests básicas + inventário completo | Enemy System, Quest System |
| **Multiplayer Ready** | **Co-op local até 4 jogadores estável** | **Input Multiplayer, Camera System, UI Multi-instance** |

---

## 🔁 Procedimento de Atualização

1. Marcar alterações de status diretamente nas tabelas (manter símbolos)
2. Mover itens 100% estáveis para "Concluídos"
3. Referenciar novas decisões com comentário breve (<!-- nota -->)
4. Sincronizar com GDD se impactar design macro

---

## 📎 Referências Rápidas

| Documento | Propósito |
|-----------|-----------|
| GDD v3.0 | Visão macro de mundos, sistemas e progressão |
| ProximosPassos.md | Micro tarefas priorizadas |
| Mecanismos.md | Lista granular de mecânicas por fase |
| InputSystem_Guide.md | Detalhamento técnico do Input System |

---
**Próxima revisão recomendada após:** Implementação de UI Navigation + protótipo de Inventory.

_Roadmap sincronizado em: (atualizar manualmente a cada commit relevante)_
