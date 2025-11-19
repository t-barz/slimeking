# 🎭 Animator Export Tool

## Visão Geral

Ferramenta para exportação completa da configuração de Animators do Unity. Gera relatórios detalhados sobre parâmetros, estados, transições e configurações do Animator Controller.

## Como Usar

### Método 1: Menu Contexto (Recomendado)

1. **Selecione** um GameObject com componente Animator na hierarquia
2. **Clique direito** no GameObject
3. **Navegue**: `Extra Tools > Export Animator Configuration`
4. **Pronto!** O arquivo será gerado em `Assets/AuxTemp/`

### Método 2: Menu Debug

1. **Selecione** um GameObject com componente Animator
2. **Menu**: `Extra Tools > Debug > Export Animator Configuration`
3. **Resultado**: Arquivo de configuração exportado

## Validação

- ✅ **Funciona**: Somente quando há um GameObject selecionado COM componente Animator
- ❌ **Desabilitado**: Quando nenhum GameObject está selecionado ou não possui Animator

## Informações Exportadas

### 📋 Informações Básicas do Animator

- Controller configurado
- Avatar (se aplicável)
- Culling Mode, Update Mode
- Apply Root Motion, Animate Physics
- Contagens de layers e parâmetros

### 🎯 Controller Information

- Nome e tipo do controller
- Caminho do asset
- Lista de Animation Clips com duração e FPS

### ⚙️ Parâmetros Detalhados

Para cada parâmetro:

- Nome e tipo (Bool, Float, Int, Trigger)
- Valor padrão vs valor atual
- Hash do parâmetro

### 🔄 Estados e Layers

Para cada layer:

- Peso padrão vs atual
- Configurações de sincronização
- IK Pass, Avatar Mask, Blending Mode
- Lista completa de estados com:
  - Tag, velocidade, motion
  - Informações de Animation Clips (duração, FPS, loop)
  - Número de transições

### 🌊 Transições Detalhadas

- Transições entre estados
- Any State Transitions
- Entry Transitions
- Para cada transição:
  - Duração, offset, exit time
  - Condições (parâmetro, modo, threshold)
  - Configurações de interrupção

### 🎮 Estado Atual (Runtime)

- Estado atual por layer
- Tempo normalizado e duração
- Velocidade e tag
- Informações de transição ativa

## Formato de Saída

```
AnimatorConfig_[NomeDoGameObject]_[DataHora].txt
```

**Exemplo**: `AnimatorConfig_NPC_art_beeA_20251119_143022.txt`

## Localização dos Arquivos

- **Diretório**: `Assets/AuxTemp/`
- **Auto-abertura**: O arquivo é automaticamente revelado no explorer após a exportação

## Casos de Uso

### 🐛 Debug de Animações

- Verificar se parâmetros estão sendo setados corretamente
- Validar transições entre estados
- Debugar problemas de sincronização de layers

### 📚 Documentação

- Criar documentação técnica do sistema de animação
- Backup das configurações do Animator
- Referência para outros desenvolvedores

### 🔄 Migration & Backup

- Documentar configurações antes de mudanças grandes
- Comparar diferentes versões do Animator
- Transferir conhecimento entre projetos

### 🎯 Optimization

- Identificar parâmetros não utilizados
- Analisar complexidade das transições
- Verificar configurações de performance

## Exemplos Práticos

### NPC com Movimento

```
GameObject: NPC_art_beeA
Animator Controller: art_beeA
Parâmetros: isWalking (Bool), FacingRight (Bool)
Estados: Idle, Walking
Transições: Idle ⟷ Walking baseado em isWalking
```

### Player com Combate

```
GameObject: Player
Animator Controller: PlayerController
Parâmetros: isWalking, isAttacking, FacingRight
Estados: Idle, Walk, Attack01, Attack02
Transições: Complexas com múltiplas condições
```

## Tips & Best Practices

### ⚡ Performance

- Use a ferramenta em modo Play para capturar estados runtime
- Exporte antes de fazer mudanças grandes no Animator

### 🔍 Debugging

- Compare arquivos antes/depois de mudanças
- Use em conjunto com console logs para debug completo

### 📁 Organização

- Arquivos são salvos com timestamp automático
- Mantenha os exports organizados por versão/feature

## Troubleshooting

### ❌ "O GameObject selecionado não possui um componente Animator"

**Solução**: Selecione um GameObject que tenha o componente Animator

### ❌ Menu não aparece no contexto

**Solução**: Certifique-se que um GameObject está selecionado

### ⚠️ "Error reading parameters/states"

**Possível**: Animator Controller corrompido ou incompatível
**Solução**: Verifique a integridade do Animator Controller

---

## Changelog

### v1.0.0 (19/11/2024)

- ✅ Implementação inicial
- ✅ Export completo de configuração do Animator
- ✅ Menu contexto e debug
- ✅ Validação automática
- ✅ Documentação detalhada
