# 🛠️ HUD Context Menu Editor Tool

## Visão Geral

A ferramenta `HUDContextMenu.cs` fornece menus de contexto no Unity Editor para configuração automática dos sistemas de HUD do SlimeKing. Esta ferramenta permite criar configurações completas de UI com apenas alguns cliques.

## Funcionalidades

### 1. **Configure Crystal Counters** 💎

- Cria um sistema completo de contadores de cristais elementais
- Posicionamento automático no canto superior direito
- Layout vertical organizado com cores diferenciadas por elemento
- Integração automática com `CrystalCounterUI` e `GameManager`

### 2. **Configure Heart HUD** ❤️

- Cria um sistema completo de corações para exibição de vida
- Posicionamento automático no canto superior esquerdo  
- Layout em grade responsivo (máx. 10 corações por linha)
- Integração automática com `HealthUIManager` e `PlayerAttributesHandler`

### 3. **Configure Complete HUD** 🎯

- Combina ambos os sistemas acima
- Configuração completa de HUD em uma única operação
- Posicionamento otimizado para não haver sobreposições

## Como Usar

### Método 1: Menu GameObject

1. Selecione um objeto Canvas na hierarquia
2. Vá para `GameObject → SlimeKing HUD`
3. Escolha a opção desejada:
   - `Configure Crystal Counters`
   - `Configure Heart HUD`
   - `Configure Complete HUD`

### Método 2: Context Menu (Recomendado)

1. Clique com o botão direito em qualquer Canvas na hierarquia
2. No menu de contexto, escolha:
   - `SlimeKing: Configure Crystal Counters`
   - `SlimeKing: Configure Heart HUD`
   - `SlimeKing: Configure Complete HUD`

## Estrutura Criada

### Crystal Counters

```
CrystalCounters_Container
├── VerticalLayoutGroup (spacing: 5px)
├── CrystalCounterUI (component)
├── Crystal_Nature
│   ├── HorizontalLayoutGroup
│   ├── Icon (Image - verde)
│   └── Count_Text (TextMeshPro)
├── Crystal_Fire
│   ├── HorizontalLayoutGroup  
│   ├── Icon (Image - vermelho-laranja)
│   └── Count_Text (TextMeshPro)
├── [... outros cristais]
```

### Heart HUD

```
HeartHUD_Container
├── GridLayoutGroup (10 colunas, 40x40px por célula)
├── HealthUIManager (component)
└── [Corações criados dinamicamente em runtime]
```

## Posicionamento Automático

| Sistema | Posição | Anchor | Offset |
|---------|---------|--------|--------|
| Crystal Counters | Canto superior direito | (1, 1) | (-20, -20) |
| Heart HUD | Canto superior esquerdo | (0, 1) | (20, -20) |

## Configurações Técnicas

### Crystal Counters

- **Container Size**: 300x150px
- **Element Size**: 280x20px por cristal
- **Spacing**: 5px vertical
- **Icons**: 16x16px com cores elemental-themed
- **Text**: TextMeshPro, size 14, bold, branco

### Heart HUD

- **Container Size**: 400x100px  
- **Grid Cell Size**: 40x40px
- **Grid Spacing**: 5x5px
- **Max Columns**: 10 corações por linha
- **Heart Sprites**: Carregados de `Resources/UI/` ou fallback para squares coloridos

## Integração com Sistemas

### GameManager Integration

- Crystal counters automaticamente se conectam aos eventos:
  - `OnCrystalAdded`
  - `OnCrystalSpent`
  - `OnCrystalCountChanged`

### PlayerAttributesHandler Integration  

- Heart HUD automaticamente se conecta ao evento:
  - `OnHealthChanged`

## Sprites e Resources

### Sprites Esperados

A ferramenta busca sprites em:

- `Resources/UI/heart_full.png`
- `Resources/UI/heart_empty.png`

### Fallback System

Se os sprites não forem encontrados, a ferramenta cria:

- Quadrados coloridos vermelhos para corações cheios
- Quadrados semi-transparentes para corações vazios
- Ícones coloridos para cristais baseados em cores elemental-themed

## Cores dos Cristais

| Tipo | Cor | Hex/RGB |
|------|-----|---------|
| Nature | Verde | (0.2, 0.8, 0.2) |
| Fire | Vermelho-Laranja | (1.0, 0.3, 0.1) |
| Water | Azul | (0.1, 0.5, 1.0) |
| Shadow | Roxo | (0.4, 0.2, 0.8) |
| Earth | Marrom | (0.6, 0.4, 0.2) |
| Air | Azul Claro | (0.8, 0.9, 1.0) |

## Validação e Logs

### Validações

- ✅ Verifica se objeto selecionado é um Canvas
- ✅ Exibe mensagem de erro se seleção inválida
- ✅ Marca cena como "dirty" para salvar mudanças

### Logs de Debug

- `✅ Crystal Counter UI configured successfully on Canvas: {name}`
- `✅ Heart HUD configured successfully on Canvas: {name}`
- `🎯 Complete HUD configured successfully on Canvas: {name}`
- `⚠️ Heart sprites not found in Resources/UI/. Using colored squares as fallback.`

## Troubleshooting

### Canvas não aparece no menu

**Problema**: Menu de contexto não exibe opções SlimeKing
**Solução**: Certifique-se que o objeto selecionado possui componente Canvas

### Sprites não carregam

**Problema**: Corações aparecem como quadrados coloridos
**Solução**: Adicione sprites `heart_full.png` e `heart_empty.png` em `Assets/Resources/UI/`

### Componentes não funcionam

**Problema**: UI aparece mas não atualiza com dados do jogo
**Solução**: Verifique se `GameManager` e `PlayerAttributesHandler` estão presentes na cena

### Layout não responsivo

**Problema**: Elementos ficam fora da tela em diferentes resoluções
**Solução**: Use Canvas com Canvas Scaler configurado para Scale With Screen Size

## Arquivo de Implementação

```
Assets/💻 Code/Editor/HUDContextMenu.cs
```

### Namespace

```csharp
namespace SlimeKing.Core.Editor
```

### Dependências

- UnityEngine
- UnityEngine.UI  
- UnityEditor
- TMPro
- SlimeKing.Core.UI

---

**Versão**: 1.0  
**Data**: Novembro 2025  
**Compatibilidade**: Unity 2022.3+ LTS  
**Status**: ✅ Implementado e funcional
