# 🗂️ Scene Organizer Tool

Ferramenta de editor para organizar automaticamente a hierarquia de cenas seguindo os padrões definidos no Coding Standards.

## 📋 Funcionalidades

- ✅ Cria estrutura padronizada de organizadores
- ✅ Reorganiza GameObjects existentes nas categorias corretas
- ✅ Renomeia objetos fora do padrão (remove prefixos, converte para PascalCase)
- ✅ Cria separadores visuais (--- CATEGORIA ---)
- ✅ Suporta Undo/Redo (Ctrl+Z)
- ✅ Preview da estrutura antes de aplicar

## 🚀 Como Usar

### Acesso

`Menu > Extra Tools > Organize > Scene Hierarchy`

### Workflow

1. **Abra a cena** que deseja organizar no Unity Editor
2. **Abra a ferramenta** via menu Extra Tools
3. **Configure opções**:
   - ✅ Criar Separadores: Adiciona separadores visuais `--- CATEGORIA ---`
   - ✅ Renomear Objetos: Remove prefixos (`art_`, `env_`, `prop_`) e converte para PascalCase
   - ✅ Agrupar por Categoria: Organiza objetos nas categorias apropriadas
   - ✅ Mostrar Preview: Exibe estrutura que será criada
4. **Clique em "🚀 Organizar Cena"**
5. **Confirme a operação** no diálogo
6. **Verifique o resultado** na hierarquia

### Categorização Automática

A ferramenta analisa o nome dos GameObjects e os categoriza automaticamente:

| Categoria | Palavras-chave detectadas |
|-----------|---------------------------|
| **SYSTEMS** | manager, eventsystem |
| **Background** | background, sky |
| **Grid** | grid, tilemap |
| **Scenario** | scenario, rock, grass, mushroom, cave, prop, env_ |
| **Player** | player |
| **NPCs** | npc, rick, helpy |
| **Enemies** | enemy, bee, gobu |
| **Items** | apple, crystal, item |
| **Mechanics** | teleport, shrink, rolling, mechanics, puzzle |
| **SpawnPoints** | spawn |
| **Triggers** | trigger |
| **Lighting** | light, global volume |
| **ParticleSystems** | particle |
| **PostProcessing** | postprocess, volume |
| **UI** | canvas, hud |

## 📝 Estrutura Criada

```text
Root Scene Hierarchy:
├── --- SYSTEMS ---
├── --- ENVIRONMENT ---
├── Background
├── Grid
├── Scenario
├── --- GAMEPLAY ---
├── Player
├── NPCs
├── Enemies
├── Items
├── --- MECHANICS ---
├── Mechanics
├── SpawnPoints
├── Triggers
├── --- EFFECTS ---
├── Lighting
├── ParticleSystems
├── PostProcessing
└── --- UI ---
```

## 🔧 Renomeação Automática

### Antes
```
art_rickA
env_brown_rockA2 (3)
prop_puddle
item_appleA
teleportPoint
```

### Depois
```
RickA
BrownRockA2_03
Puddle
AppleA
TeleportPoint
```

### Regras de Renomeação

1. **Remove prefixos**: `art_`, `env_`, `prop_`, `item_`
2. **Converte para PascalCase**: primeira letra maiúscula
3. **Substitui ` (N)` por `_NN`**: `RockA2 (3)` → `RockA2_03`
4. **Mantém estrutura de nomes descritivos**

## ⚠️ Importante

- A ferramenta **NÃO modifica** a estrutura interna dos GameObjects (componentes, scripts, etc.)
- Apenas **reorganiza a hierarquia** e **renomeia GameObjects**
- **Suporta Undo** (Ctrl+Z) - você pode desfazer a operação
- **Marca cena como modificada** - lembre de salvar após organizar
- **Não exclui objetos** - apenas move para organizadores apropriados

## 📖 Referências

- [Coding Standards - Organização de Hierarquia de Cenas](../Docs/CodingStandards.md#-organização-de-hierarquia-de-cenas)
- [Coding Standards - Nomenclatura de Prefabs](../Docs/CodingStandards.md#nomenclatura-de-prefabs)

## 🐛 Troubleshooting

**Problema**: Objetos não foram categorizados
- **Solução**: Verifique se o nome contém palavras-chave da tabela de categorização. Caso contrário, mova manualmente.

**Problema**: Renomeação incorreta
- **Solução**: Desfaça (Ctrl+Z) e renomeie manualmente antes de usar a ferramenta, ou desative a opção "Renomear Objetos".

**Problema**: Estrutura criada parcialmente
- **Solução**: Execute a ferramenta novamente. Ela detecta organizadores existentes e não duplica.

## 🔄 Atualizações Futuras

- [ ] Suporte para sub-categorias automáticas em Scenario (Rocks/, Vegetation/)
- [ ] Templates personalizados de estrutura
- [ ] Batch processing para múltiplas cenas
- [ ] Validação de nomenclatura com sugestões
- [ ] Export/Import de configurações de organização
