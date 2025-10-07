# Alpha Test Checklist - The Slime King Demo Alpha

**Data Criação:** 07/Out/2025  
**Meta Conclusão:** Meados de Novembro/2025 (6 semanas)  
**Escopo:** Demo Alpha jogável com loop de gameplay completo  

## 📋 Informações Gerais

### Objetivo da Demo Alpha

Entregar uma versão jogável mínima contendo todos os sistemas core necessários para validar o loop principal de gameplay: exploração → coleta → combate → progressão → interação.

### Critérios de Aceite Globais

- ✅ Zero erros de compilação
- ✅ Zero exceções no Console durante fluxo de teste
- ✅ FPS estável (≥60) em cena de teste com 20 objetos ambientais + 5 inimigos
- ✅ Tempo para entender controles básicos ≤ 1 minuto (teste interno)
- ✅ Ciclo completo de teste executável em ≤ 5 minutos

---

## 🧪 Checklist de Teste por Sistema

### 1. Movimento Base (PlayerController existente)

- [ ] **Movimento básico**: WASD/Gamepad move o personagem suavemente
- [ ] **Animações**: Sprites mudam conforme direção de movimento
- [ ] **Colisões**: Player não atravessa objetos sólidos
- [ ] **Rotação**: Personagem vira na direção do movimento

### 2. Sistema de Inventário Core

- [ ] **Estrutura**: Inventory Core instanciado e acessível via singleton/manager
- [ ] **Coleta automática**: Itens próximos são coletados automaticamente (raio configurável)
- [ ] **Coleta manual**: Interação com itens funciona via Input System
- [ ] **Armazenamento**: Itens coletados aparecem na estrutura interna do inventário
- [ ] **Slots limitados**: Sistema respeita limite de 4 slots para consumíveis

### 3. HUD Básico

- [ ] **Vida**: Barra/valor de HP visível e atualizada em tempo real
- [ ] **Slots consumíveis**: 4 slots visíveis mostrando itens equipados
- [ ] **Feedback coleta**: Animação/efeito visual ao coletar item
- [ ] **Responsividade**: HUD se adapta a diferentes resoluções
- [ ] **Estilo cozy**: Interface segue paleta suave e bordas orgânicas

### 4. Uso de Itens (ItemUsageManager)

- [ ] **Binding correto**: UseItem1-4 (Input Actions) funcionam
- [ ] **Consumo**: Item é removido do slot após uso
- [ ] **Efeito aplicado**: Buff/efeito simples é aplicado (ex: +20 HP temporário)
- [ ] **Feedback visual**: Animação/partícula confirma uso do item
- [ ] **Cooldown básico**: Não permite spam (opcional, se implementado)

### 5. Sistema de Inimigos Base

- [ ] **Spawn**: Inimigo aparece na cena sem erros
- [ ] **Estado Patrol**: Inimigo se move em padrão básico quando não detecta player
- [ ] **Estado Chase**: Inimigo persegue player quando detectado (range configurável)
- [ ] **Estado Attack**: Inimigo executa ataque quando próximo do player
- [ ] **Estado Death**: Inimigo morre ao HP chegar a zero
- [ ] **Dano ao player**: Ataque do inimigo reduz HP do player
- [ ] **Recebe dano**: Inimigo perde HP quando atacado pelo player
- [ ] **Drop ao morrer**: Inimigo solta item/recurso ao morrer (opcional)

### 6. Combate (AttackHandler + PlayerController)

- [ ] **Ataque básico**: Player ataca na direção correta
- [ ] **Área de dano**: AttackHandler detecta inimigos na área configurada
- [ ] **Aplicação de dano**: Inimigos perdem HP quando atingidos
- [ ] **Feedback hit**: Efeito visual/sonoro ao acertar inimigo
- [ ] **Feedback miss**: Efeito diferente quando ataque não acerta nada
- [ ] **Knockback**: Inimigo é empurrado ao receber dano (se implementado)

### 7. Growth System (Stub)

- [ ] **Estrutura básica**: Enum de estágios (Filhote → Adulto → Grande Slime → Rei Slime)
- [ ] **Mudança forçada**: Método/debug para alterar estágio manualmente
- [ ] **Modificação atributos**: Mudança de estágio altera HP/Attack/Defense
- [ ] **Persistência**: Estágio atual mantido durante sessão
- [ ] **Feedback visual**: Log ou HUD mostra estágio atual

### 8. Skill Tree Base

- [ ] **Estrutura nós**: Pelo menos 1 nó configurado e acessível
- [ ] **Condição desbloqueio**: Nó desbloqueado baseado em estágio do Growth
- [ ] **Aplicação efeito**: Nó desbloqueado aplica modificador (+Attack, +HP, etc.)
- [ ] **Visualização**: Interface placeholder mostra nó desbloqueado/bloqueado
- [ ] **Integração Growth**: Skill Tree reage a mudanças no Growth System

### 9. UI Navigation & EventSystem

- [ ] **EventSystem configurado**: InputSystemUIInputModule funcional
- [ ] **Navegação teclado**: Tab/Arrows navegam entre elementos UI
- [ ] **Navegação gamepad**: D-Pad/Sticks navegam elementos UI
- [ ] **Submit/Cancel**: Enter/Esc funcionam em menus
- [ ] **Highlight visual**: Elemento selecionado tem destaque visual

### 10. Sistema de Diálogo Mínimo

- [ ] **Abertura**: Diálogo abre via interação ou trigger
- [ ] **Bloqueio input**: Input de gameplay bloqueado durante diálogo
- [ ] **Avanço texto**: Submit avança para próxima fala
- [ ] **Fechamento**: Diálogo fecha ao terminar todas as falas
- [ ] **Retorno controle**: Input de gameplay retorna após fechar diálogo
- [ ] **Interface**: Caixa de texto visível e legível

### 11. ~~Camera Follow & Bounds~~ ❌ REMOVIDO

**Sistema removido do escopo Alpha.**

- **Motivo**: Cinemachine Follow já disponível no projeto
- **Substituto**: Configuração Cinemachine existente
- **Impacto**: Zero - funcionalidade mantida via Cinemachine

### 12. ~~Teleport Point~~ ❌ REMOVIDO

**Sistema removido do escopo Alpha.**

- **Motivo**: Implementação futura específica preferida
- **Status**: A ser desenvolvido posteriormente
- **Impacto**: Zero nos sistemas core Alpha

---

## 🔄 Fluxo de Teste Completo (Ciclo Principal)

### Sequência de Validação (5 minutos máximo)

1. **[30s] Inicialização**
   - Abrir cena `TestArenaAlpha.unity`
   - Verificar Console limpo (0 erros/warnings)
   - Player spawna corretamente

2. **[60s] Movimento & Coleta**
   - Mover player por toda área de teste
   - Coletar pelo menos 3 itens diferentes
   - Verificar HUD atualizado com itens

3. **[90s] Combate & Progressão**
   - Atacar e derrotar pelo menos 1 inimigo
   - Verificar drop de item (se implementado)
   - Forçar mudança de estágio (Growth) via debug
   - Verificar nó desbloqueado no Skill Tree

4. **[60s] Uso de Itens & UI**
   - Usar pelo menos 2 consumíveis diferentes
   - Verificar efeitos aplicados (HP, buffs)
   - Navegar interface com teclado e gamepad

5. **[30s] Interação & Diálogo**
   - Ativar diálogo com NPC/trigger
   - Verificar bloqueio de input durante diálogo
   - Testar avanço e fechamento de diálogo

6. **[30s] Validação Final**
   - Verificar Console ainda limpo
   - Confirmar FPS estável (F3 ou similar)
   - Player ainda controlável normalmente

---

## ⚠️ Critérios de Falha

### Bloqueadores Críticos (Demo não passível)

- [ ] Erro de compilação ou exceção no Console
- [ ] Player não consegue se mover
- [ ] Game freeze ou crash durante teste
- [ ] FPS consistentemente abaixo de 30

### Issues Sérios (Requerem fix antes do release)

- [ ] HUD não atualiza corretamente
- [ ] Itens não podem ser usados
- [ ] Inimigo não reage ao player
- [ ] Diálogo não bloqueia input

### Issues Menores (Podem ser adiados)

- [ ] Feedback visual ausente em algumas ações
- [ ] Balanceamento de dano
- [ ] Performance ocasionalmente abaixo de 60 FPS
- [ ] UI não responsiva em certas resoluções

---

## 📊 Métricas de Sucesso

| Métrica | Valor Alvo | Método de Verificação |
|---------|------------|----------------------|
| FPS Médio | ≥ 60 | Unity Profiler ou overlay |
| Erros Console | 0 | Visual do Console |
| Tempo Ciclo Teste | ≤ 5 min | Cronômetro manual |
| Crash Rate | 0% | Múltiplas execuções |
| Input Responsiveness | ≤ 100ms | Teste manual lag |

---

## 🔧 Setup para Teste

### Pré-requisitos

- Unity 6.3+ aberto
- Projeto "The Slime King" carregado
- Input System configurado
- Cena `TestArenaAlpha.unity` configurada

### Configuração da Cena de Teste

- [ ] 1 Player spawn point
- [ ] 3-5 itens coletáveis posicionados
- [ ] 2-3 inimigos básicos spawned
- [ ] 1 NPC/trigger para diálogo
- [ ] HUD Canvas configurado
- [ ] ~~1 teleport point~~ ❌ Removido do escopo
- [ ] ~~Bounds da câmera~~ ❌ Cinemachine gerencia

### Ferramentas de Debug

- [ ] Console aberto (verificar erros)
- [ ] Profiler disponível (verificar FPS)
- [ ] Scene view (verificar colisões)
- [ ] Inspector (modificar parâmetros se necessário)

---

## 📅 Cronograma de Validação

### Teste Diário (Durante desenvolvimento)

- Executar checklist reduzido (itens críticos apenas)
- Foco em sistema sendo desenvolvido no dia

### Teste Semanal (Fim de cada fase)

- Checklist completo
- Documentar issues encontrados
- Atualizar status no roadmap

### Teste Final (Semana 6)

- 3 execuções completas do checklist
- Zero tolerance para critérios de falha críticos
- Documentação final de conhecidos issues menores

---

## 📝 Log de Testes

### Formato de Entrada

```
Data: DD/MM/YYYY
Testador: Nome
Versão: Commit/Tag
Status: PASS/FAIL/PARTIAL
Issues Encontrados: Lista
Tempo Execução: X min
Observações: Notas adicionais
```

### Histórico

_(Será preenchido durante desenvolvimento)_

---

**Última Atualização:** 07/Out/2025  
**Responsável:** Equipe The Slime King  
**Próxima Revisão:** Após implementação da Fase 1 (Inventory Core)
