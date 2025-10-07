# Sistema de Progressão - Implementação Alpha

## 📋 Status

- **Growth System:** 🔜 Não iniciado
- **Skill Tree:** 🔜 Não iniciado  
- **Integração PlayerAttributesSystem:** 🔜 Não iniciado

## 🎯 Objetivo

Criar sistema de progressão (Growth + Skill Tree) que integra com PlayerAttributesSystem existente, sem modificar código existente.

## 🔧 Implementação

### Scripts Necessários (todos novos na Alpha/)

#### 1. GrowthSystem.cs (NOVO)

```csharp
// Gerencia estágios de evolução do Slime
// Filhote → Adulto → Grande Slime → Rei Slime
// Integra com PlayerAttributesSystem via eventos
```

#### 2. SkillTreeManager.cs (NOVO)

```csharp
// Gerencia nós de habilidades desbloqueáveis
// Condições baseadas no Growth System
// Aplica modificadores nos atributos
```

#### 3. AlphaProgressionHUD.cs (NOVO)

```csharp
// UI mostrando estágio atual + skill points
// Botão debug para forçar growth (Alpha only)
```

#### 4. AlphaProgressionSetup.cs (NOVO)

```csharp
// Setup automático dos sistemas de progressão
// Configura eventos e referências
```

### Fluxo de Integração

1. **Crescimento (Para Alpha: manual/debug)**

   ```
   Debug button/trigger →
   GrowthSystem.AdvanceStage() →
   PlayerAttributesSystem events (modificar atributos) →
   SkillTreeManager.OnGrowthChanged()
   ```

2. **Skill Tree Unlock**

   ```
   GrowthSystem stage change →
   SkillTreeManager verifica condições →
   Desbloqueia novos nós →
   Aplica modificadores
   ```

3. **Modificação de Atributos**

   ```
   Skill desbloqueada →
   SkillTreeManager aplica effect →
   PlayerAttributesSystem.ModifyAttribute() →
   HUD atualizado
   ```

## 📝 TODOs Específicos

### GrowthSystem.cs (criar novo)

- [ ] Enum SlimeStage (Filhote, Adulto, GrandeSlime, ReiSlime)
- [ ] Método AdvanceStage() para debug
- [ ] Eventos OnStageChanged
- [ ] Integração com PlayerAttributesSystem para stat boosts

### SkillTreeManager.cs (criar novo)

- [ ] Estrutura de dados para skill nodes
- [ ] Sistema de prerequisitos (baseado em growth stage)
- [ ] Aplicação de modificadores de atributo
- [ ] UI placeholder para mostrar skills desbloqueadas

### AlphaProgressionHUD.cs (criar novo)

- [ ] Display do estágio atual
- [ ] Botão debug "Force Next Stage"
- [ ] Lista de skills ativas (text-based para Alpha)
- [ ] Integração com eventos dos sistemas

### AlphaProgressionSetup.cs (criar novo)

- [ ] Auto-setup na cena
- [ ] Conecta events entre Growth e SkillTree
- [ ] Configura HUD reference

## 🔗 Pontos de Integração

### Com PlayerAttributesSystem (NÃO MODIFICAR)

- Usar eventos existentes para modificar HP, Attack, Defense
- GrowthSystem.OnStageChanged → modificar base stats
- SkillTreeManager → aplicar temporary/permanent buffs

### Com Input System (USAR EXISTENTE, SE NECESSÁRIO)

- Para Alpha: botão debug apenas
- Futuro: poderia usar Menu action para abrir skill tree

### Com HUD Existente (NÃO MODIFICAR)

- AlphaProgressionHUD será separado
- Pode ser integrado depois se necessário

## ⚙️ Configuração na Cena

### Setup Automático via Extra Tools > Alpha

1. Cria GrowthSystem singleton
2. Configura SkillTreeManager
3. Setup AlphaProgressionHUD
4. Conecta eventos automaticamente

### Configuração Manual

- Adicionar AlphaProgressionSetup em GameObject vazio
- Configurar skill tree data (ScriptableObject)
- Definir modificadores por estágio

## 🧪 Teste de Validação

1. **Growth:** Botão debug avança estágio → stats aumentam
2. **Skills:** Novo estágio → skills desbloqueadas → efeitos aplicados
3. **Integração:** Mudanças refletidas no PlayerAttributesSystem
4. **HUD:** Interface mostra estado atual corretamente

## 📊 MVP para Alpha

### Growth System

- 4 estágios definidos
- Botão debug para avançar
- Cada estágio +20% em todos os atributos base

### Skill Tree  

- 1 skill por estágio (4 total)
- Efeitos simples: +HP, +Attack, +Speed, +Special
- Desbloqueio automático por estágio

### HUD

- Text simples: "Stage: Adulto"
- Lista: "Skills: +HP, +Attack"
- Botão: "DEBUG: Next Stage"

## 📊 Métricas de Sucesso

- [ ] 4 estágios funcionais
- [ ] Skills desbloqueiam automaticamente
- [ ] Atributos modificados via PlayerAttributesSystem
- [ ] HUD mostra estado atual
- [ ] Zero modificações no código existente
