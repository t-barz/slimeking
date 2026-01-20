# The Slime King - Roadmap Alfa (5 Dias)
## 4 horas por dia com apoio GenAI

---

## DIA 1 - QUICKSLOT + INTEGRAÇÃO EM MUNDO (4h)

### 0:00-1:00 (1h) - Setup UI + Binding com Inventário
- [x] Criar prefab QuickSlot (4 slots direcionais: cima, baixo, esq, dir)
- [x] Integrar evento `OnInventoryChanged` para atualizar visual dos slots
- [x] Testar binding: pegar item → atualizar slot auitomaticamente
- [x] **Commit**: `feat: quickslot ui binding base`

### 1:00-2:00 (1h) - Implementar Uso de Itens
- [ ] Script `QuickSlotManager`: detectar input direcional
- [ ] Chamar `InventoryManager.UseQuickSlot(direction)` com efeito sonoro/visual
- [ ] Consumíveis: remover do inventário + aplicar healing (existente)
- [ ] Testar: equipar poção → apertar direcional → vida + drop item
- [ ] **Commit**: `feat: quickslot item usage with feedback`

### 2:00-3:30 (1.5h) - Teste de Integração + Colisão
- [ ] Verificar se quickslot funciona enquanto jogador se move
- [ ] Testar uso de itens em diferentes cenários (perto de inimigos, obstáculos)
- [ ] Ajustar cooldown/feedback se necessário
- [ ] Playtest rápido: equipar → usar → verificar efeito
- [ ] **Commit**: `fix: quickslot integration and feedback tuning`

### 3:30-4:00 (0.5h) - Build & Commit Final
- [ ] Build teste do jogo
- [ ] Verificar logs de erro
- [ ] Commit final com mensagem clara
- [ ] **Tag**: `day-1-quickslot-done`

---

## DIA 2 - CENÁRIO + BLOQUEIOS (4h)

### 0:00-1:30 (1.5h) - Reposicionar Cogumelo + Ajustar Pedras
- [x] Mover cogumelo para melhor posição visual no mapa
- [x] Reduzir quantidade de pedras após slide (verificar prefab spawn)
- [x] Corrigir loot: pedras pós-slide NÃO dropam pedras (remover flag de drop)
- [x] Testar: fazer slide → pedras caem → sem re-drop
- [x] **Commit**: `feat: adjust mushroom position and stone drops`

### 1:30-2:30 (1h) - Implementar Pedra Rolante + Rio/Lago
- [ ] Criar prefab Pedra Rolante com colisor (bloqueio inicial)
- [ ] Posicionar na entrada da floresta
- [ ] Criar tileset para rio/lago (reutilizar tiles existentes)
- [ ] Prototipar rio como barreira transponível (walkable com tile especial)
- [ ] Testar colisão player com pedra rolante
- [ ] **Commit**: `feat: rolling stone barrier and river prototype`

### 2:30-3:30 (1h) - Testes de Navegação
- [ ] Playtest: andar pela floresta → tentar passar pela pedra rolante → bloqueado
- [ ] Verificar colisores e visual do rio
- [ ] Ajustar tamanho/posição se necessário
- [ ] Testar com quickslot do Dia 1: usar item perto de obstáculos
- [ ] **Commit**: `fix: level design navigation and collisions`

### 3:30-4:00 (0.5h) - Playtest Completo Dia 1 + 2
- [ ] Playthrough: começar jogo → usar quickslot → explorar cenário → testar bloqueios
- [ ] Verificar bugs gráficos ou de física
- [ ] Build final
- [ ] **Tag**: `day-2-level-design-done`

---

## DIA 3 - QUEST + TUTORIAL NARRATIVO (4h)

### 0:00-1:15 (1.25h) - Implementar Quest Recolher Pedras
- [ ] Criar `Quest_CollectStones` (objetivo: recolher 5 pedras)
- [ ] Integrar com sistema de quest existente
- [ ] Adicionar marcador no inventário: "Pedras: 0/5"
- [ ] Testar: recolher pedras → contador atualiza
- [ ] **Commit**: `feat: stone collection quest base`

### 1:15-2:15 (1h) - Diálogos Carvolha com Dicas
- [ ] Criar diálogo Carvolha: "Preciso de pedras! Encontre para mim."
- [ ] Adicionar diálogo condicional: se quest não começou → oferecer quest
- [ ] Se em progresso → dica de onde achar pedras (ex: "Procure perto do slide")
- [ ] Se concluída → agradecer e teaser de próxima etapa
- [ ] Testar: falar com Carvolha em diferentes estados de quest
- [ ] **Commit**: `feat: carvolha dialogue with quest conditions`

### 2:15-3:30 (1.25h) - Tutorial de Esconder
- [ ] Criar area "TutorialHide" na floresta com inimigo dummy
- [ ] Implementar trigger: primeira vez que jogador entra → mostrar dica "Pressione [key] para esconder"
- [ ] Se quest ativa: integrar com quest (ex: "Esconda para pegar esta pedra")
- [ ] Testar: entrar na área → dica aparece → esconder próximo ao inimigo
- [ ] **Commit**: `feat: hide tutorial integrated with quest`

### 3:30-4:00 (0.5h) - Playtest Dia 3 + Fixes
- [ ] Playthrough: aceitar quest → falar com Carvolha → coletar pedras → tutorial de hide
- [ ] Verificar condições de diálogo e fluxo de quest
- [ ] Build final
- [ ] **Tag**: `day-3-quest-and-narrative-done`

---

## DIA 4 - SLIME + PODER + FLORESTA (4h)

### 0:00-1:00 (1h) - Implementar Crescimento do Slime
- [ ] Criar `SlimeGrowthSystem`: 3 estágios (pequeno → médio → grande)
- [ ] Integrar com quest: ao completar "Recolher Pedras" → Slime cresce
- [ ] Ajustar tamanho visual + colisor do player
- [ ] Atualizar stats (ex: força +1 por estágio)
- [ ] Testar: quest completa → Slime cresce → visual + stats mudam
- [ ] **Commit**: `feat: slime growth system with 3 stages`

### 1:00-2:00 (1h) - Novo Poder Desbloqueado
- [ ] Criar novo poder (ex: "Slime Push" - empurrar objetos)
- [ ] Implementar como desbloqueio automático após crescimento
- [ ] Integrar com quickslot ou tecla dedicada
- [ ] Testar: usar poder → efeito funciona
- [ ] **Commit**: `feat: new power system unlocked after growth`

### 2:00-3:15 (1.25h) - Desbloqueio da Floresta
- [ ] Modificar pedra rolante: se player tem novo poder → pode remover/empurrar
- [ ] Ajustar lógica de bloqueio: verificar `PlayerAttributes.hasPower`
- [ ] Testar: crescer Slime → usar poder → pedra se move → acesso à floresta
- [ ] Adicionar feedback visual quando poder é ativado
- [ ] **Commit**: `feat: unlock forest with new power`

### 3:15-4:00 (0.75h) - Playtest End-to-End Dias 1-4
- [ ] Playthrough completo: quickslot → quest → crescimento → novo poder → floresta
- [ ] Verificar transições entre sistemas
- [ ] Fixes rápidos se algo quebrou
- [ ] Build final
- [ ] **Tag**: `day-4-slime-and-power-done`

---

## DIA 5 - TESTES GLOBAIS + FIXES + BUILD ALFA (4h)

### 0:00-1:00 (1h) - Playthrough End-to-End Completo
- [ ] Teste completo do fluxo: início → quickslot → quest → crescimento → floresta
- [ ] Verificar todos os NPCs respondendo corretamente
- [ ] Testar tutorial de hide funcionando
- [ ] Confirmação visual de todas as mudanças (Slime, power, desbloqueio)
- [ ] **Log**: anotações de bugs encontrados

### 1:00-2:00 (1h) - Fixes em Drops, Tutoriais e Feedback
- [ ] Ajustar drops de itens se necessário (pedras, consumíveis)
- [ ] Revisar clareza do tutorial de hide
- [ ] Adicionar feedback visual/sonoro em transições importantes
- [ ] Corrigir qualquer bug encontrado no playthrough
- [ ] **Commit**: `fix: final adjustments for alpha stability`

### 2:00-3:00 (1h) - Testes de Performance + Estabilidade
- [ ] Usar Profiler Unity para verificar picos de CPU/memória
- [ ] Testar em diferentes resoluções (teste em resolução inferior se possível)
- [ ] Verificar se há memory leaks em transições de quest/diálogos
- [ ] Playtest rápido final: tudo rodando sem stutters
- [ ] **Commit**: `perf: profiling and stability validation`

### 3:00-4:00 (1h) - Build Alfa + Preparação para Release
- [ ] Criar build final (File → Build Settings)
- [ ] Testar build em máquina limpa se possível
- [ ] Gerar logs de build sem erros críticos
- [ ] Documentar última lista de features implementadas
- [ ] **Tag**: `alpha-v1.0-release-ready`
- [ ] **Commit final**: `release: The Slime King Alpha v1.0`

---

## Dicas Rápidas por Dia

| Dia | Context | Dica |
|-----|---------|------|
| 1 | Quickslot pode ficar complexo | Use GenAI para boilerplate de UI binding, você refina |
| 2 | Level design pode ter overlap | Termine level primeiro, teste com Quickslot do Dia 1 |
| 3 | Quest pode travar diálogos | Reutilize sistema de diálogo já existente no jogo |
| 4 | Poder precisa integrar com bloqueio | Faça antes de testes globais, menos refação |
| 5 | Performance é subestimada | 30min mínimo com Profiler antes do build final |

---

## Commits Resumidos (para git)

```bash
# Dia 1
git commit -m "feat: quickslot ui binding base"
git commit -m "feat: quickslot item usage with feedback"
git commit -m "fix: quickslot integration and feedback tuning"
git tag day-1-quickslot-done

# Dia 2
git commit -m "feat: adjust mushroom position and stone drops"
git commit -m "feat: rolling stone barrier and river prototype"
git commit -m "fix: level design navigation and collisions"
git tag day-2-level-design-done

# Dia 3
git commit -m "feat: stone collection quest base"
git commit -m "feat: carvolha dialogue with quest conditions"
git commit -m "feat: hide tutorial integrated with quest"
git tag day-3-quest-and-narrative-done

# Dia 4
git commit -m "feat: slime growth system with 3 stages"
git commit -m "feat: new power system unlocked after growth"
git commit -m "feat: unlock forest with new power"
git tag day-4-slime-and-power-done

# Dia 5
git commit -m "fix: final adjustments for alpha stability"
git commit -m "perf: profiling and stability validation"
git tag alpha-v1.0-release-ready
git commit -m "release: The Slime King Alpha v1.0"
```

---

## Notas Finais

- **4h é curto**: Se passar de 4h em um dia, priorize tasks em negrito e deixe "nice-to-have" para depois.
- **GenAI acelera**: Use para gerar boilerplate de scripts UI/quest, você refina.
- **Playteste frequente**: 10-15min a cada 1h30 de dev apanha bugs cedo.
- **Backup antes de Dia 5**: Faça backup do projeto antes de testes globais em caso de revert urgente.

---

**Boa sorte com a alfa! 🎮🍄**