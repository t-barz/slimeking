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

| Item | Status | Categoria | Notas |
|------|--------|----------|-------|
| UI Navigation (InputSystemUIInputModule) | 🔜 | Input / UI | Mapas e wrapper prontos; falta EventSystem configurado |
| Pause (habilitar/desabilitar mapas) | 🔜 | Sistema | Depende de UI Navigation básica |
| Inventory (slots 1–4 inicial) | 🔜 | Gameplay / UI | GDD define conceito; ainda não iniciado |
| Rebinding básico | 🔜 | QoL | Guia criado; implementar fluxo persistência PlayerPrefs |
| SpecialMovementPoint integração final | 🟡 | Movimento Especial | Classe criada; validar triggers e animações |
| AttackHandler + PlayerController sincronização | 🟡 | Combate | Handler existente; falta ligar a inimigos / destrutíveis completos |
| **Multiplayer Local (até 4 jogadores)** | 🔜 | **Co-op** | **Sistema de input múltiplo + câmera dinâmica + inventário compartilhado** |

---

## 🧪 Demo Alpha (Scope Mecânico Base)

| Sistema (Mecanismos.md) | Status | Alinhamento GDD | Observações |
|-------------------------|--------|------------------|-------------|
| Movimentação & Animação Base | ✅ (OK) | Sec. 11.1 / 2.1 | PlayerController robusto implementado |
| Special Movement (Shrink/Slide/Jump) | 🟡 (WIP) | GDD (progressão futura) | Estrutura parcial; validar colisor dinâmico |
| Vento | ✅ (OK) | Ambientação biomas | Código legado funcionando (avaliar migração futura) |
| Árvores Reativas | ✅ (OK) | Biomas (Sec. 3) | Interação visual básica concluída |
| Grama & Arbustos (Vegetação) | ✅ (OK) | Feedback ambiental | Responde a vento/contato; revisar otimização culling |
| Pedras Danificáveis | ✅ (OK) | Sistema destrutível | RockDestruct funcional com progressão visual |
| Dropping Items | ✅ (OK) | Itens / Loop coleta | Mecânica base de queda implementada |
| Coleta Automática (Auto Pickup) | ✅ (OK) | UX / Qualidade de Vida | Raio configurável; adicionar VFX polidos depois |
| Coleta de Itens → Inventário | 🔜 (TO DO) | Inventário (Sec. 6) | Aguardando implementação do Inventory System |
| Pontos Interativos | ✅ (OK) | Interação contextual | Base + diferenciação de input (teclado/gamepad) |
| Destaque (Outline) | ✅ (OK) | UX Feedback | Já funcional; unificar estilo de shader depois |
| Combate Direcional (Player) | ✅ | Combate (Sec. 10) | PlayerController + AttackHandler base (falta inimigos) |
| Sistema de Inimigos | 🔜 | Combate / AI | Arquitetura ainda não iniciada |
| Crescimento / Evolução do Slime | 🔜 | Sec. 2.2 / 2.3 / 9 | Apenas design (GDD); nenhum código ainda |
| PlayerAttributesSystem | ✅ (parcial) | Sec. 9 | Atributos base + eventos; falta buffs / evolução dinâmica |
| Árvore de Habilidades | 🔜 | Skills futura | Totalmente conceitual |
| Uso de Itens (Consumíveis) | 🔜 | Inventário / Itens | Depende de Inventory System |
| Interface/HUD Base | 🔜 | UI / UX | Ainda não iniciado (apenas TitleScreen) |

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
| Input Action Asset (3 mapas) | ✅ | Estrutura estável UI / Gameplay / System |
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
