# 🎬 Easy Transition - Guia de Configuração

## ✅ **Implementação Completa**

A transição cellular entre **TitleScreen** e **InitialCave** foi implementada usando o **Easy Transition** existente no projeto.

## Componentes Necessários

### 1. SceneTransitioner (Singleton)

- **Prefab**: `Assets/External/AssetStore/Easy Transition/Prefabs/SceneTransitioner.prefab`
- **Função**: Gerencia todas as transições entre cenas
- **Configuração**: Deve ser adicionado na primeira cena (TitleScreen)

### 2. CellularEffect Asset

- **Asset**: `Assets/External/AssetStore/Easy Transition/Transition Effects/CellularEffect.asset`
- **Material**: `Assets/External/AssetStore/Easy Transition/Materials/CellularEffectMaterial.mat`
- **Configuração**: Já pré-configurado com efeito cellular

## 🔧 **Configuração no Inspector**

### **TitleScreenController**

1. Abra a cena **TitleScreen**
2. Selecione o GameObject que contém o **TitleScreenController**
3. No Inspector, na seção **Scene Transition**:
   - **Game Scene Name**: `"InitialCave"`
   - **Cellular Transition Effect**: Arraste o asset `CellularEffect.asset`

### **SceneTransitioner na Cena**

1. Arraste o prefab `SceneTransitioner.prefab` para a cena **TitleScreen**
2. Configure no Inspector:
   - **Transition Image Prefab**: `TransitionImage.prefab` (já configurado)
   - **Default Transition**: `CellularEffect.asset` (opcional)

## 🎯 **Como Funciona**

```csharp
// No TitleScreenController.cs - StartGame()
SceneTransitioner.Instance.LoadScene(gameSceneName, cellularTransitionEffect);
```

1. **Fade Out**: Efeito cellular cobrindo a tela (obscure)
2. **Scene Load**: Carrega a nova cena (InitialCave)
3. **Fade In**: Efeito cellular revelando a nova cena (reveal)

## ⚙️ **Configurações do CellularEffect**

- **Duration**: 3 segundos (total da transição)
- **Fade Out Animation**: Obscure (cobre a tela)
- **Fade In Animation**: Reveal (revela a nova cena)
- **Cell Density**: 10.0 (densidade das células)
- **Cell Speed**: 15.0 (velocidade da animação)
- **Smoothness**: 0.1 (suavidade das bordas)

## 🚀 **Vantagens da Implementação**

- ✅ **Plug & Play**: Usa assets já existentes
- ✅ **Performance**: Sistema otimizado com shaders
- ✅ **Flexibilidade**: Fácil trocar efeitos
- ✅ **Consistência**: Mesmo sistema para todas as transições
- ✅ **Manutenibilidade**: Código limpo e simples

## 🔄 **Próximos Passos**

1. Adicionar SceneTransitioner.prefab na TitleScreen
2. Configurar cellularTransitionEffect no Inspector
3. Testar a transição TitleScreen → InitialCave
4. Ajustar timing se necessário (duration, cell density)

---
*Implementação concluída usando Easy Transition v1.0 - Sistema de transições profissional*
