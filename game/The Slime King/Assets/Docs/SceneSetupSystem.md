# 🛠️ Scene Setup System - Plano de Implementação

## 📋 Visão Geral

O Scene Setup System é uma ferramenta integrada ao Extra Tools que automatiza a configuração de cenas no projeto The Slime King, garantindo que todas as cenas tenham os componentes e configurações necessários para funcionar corretamente.

## 🎯 Objetivos

- **Consistência**: Todas as cenas seguem o mesmo padrão de configuração
- **Produtividade**: Setup automático economiza tempo de desenvolvimento
- **Manutenção**: Fácil atualização de configurações em massa
- **Validação**: Detecção automática de problemas de configuração
- **Flexibilidade**: Configurações customizáveis por tipo de cena

## 📁 Estrutura de Arquivos

```
Assets/Code/
├── Editor/
│   ├── ProjectSetup.cs (existente)
│   ├── SceneSetupWindow.cs (Fase 3)
│   └── SceneSetupMenuItems.cs (Fase 1)
├── Systems/
│   ├── [Managers existentes]
│   └── SceneSetupManager.cs (Fase 1)
├── Tools/ (nova pasta - Fase 2)
│   ├── SceneSetup/
│   │   ├── SceneSetupData.cs
│   │   ├── SceneValidator.cs
│   │   └── Modules/
│   │       ├── CameraSetupModule.cs
│   │       ├── AudioSetupModule.cs
│   │       ├── InputSetupModule.cs
│   │       ├── PostProcessSetupModule.cs
│   │       └── ManagerSetupModule.cs (Fase 1)
│   └── Templates/ (Fase 3)
│       ├── SceneTemplate.cs
│       ├── GameplaySceneTemplate.asset
│       └── MenuSceneTemplate.asset
```

## 🚀 Fase 1 - Core (Implementação Imediata)

### **Objetivos da Fase 1**

Resolver o problema imediato da InitialCave e criar a base do sistema.

### **Componentes:**

#### **1. SceneSetupManager.cs**

- **Localização**: `Assets/Code/Systems/SceneSetupManager.cs`
- **Funcionalidade**: Component principal que detecta e configura automaticamente
- **Features**:
  - Detecção automática do tipo de cena (Gameplay, Menu, Cutscene)
  - Setup automático no Awake (configurável)
  - Logging detalhado e configurável
  - Configurações serializadas no Inspector
  - Context menu para setup manual

#### **2. ManagerSetupModule.cs**

- **Localização**: `Assets/Code/Tools/SceneSetup/Modules/ManagerSetupModule.cs`
- **Funcionalidade**: Garante que os Managers essenciais existam
- **Responsabilidades**:
  - Verificar e criar GameManager se necessário
  - Verificar e criar AudioManager se necessário
  - Verificar e criar InputManager se necessário
  - Logging de cada ação realizada

#### **3. SceneSetupMenuItems.cs**

- **Localização**: `Assets/Code/Editor/SceneSetupMenuItems.cs`
- **Funcionalidade**: Integração com menu Extra Tools
- **Menu Items**:
  - `Extra Tools/Scene Setup/Setup Current Scene`
  - `GameObject/Extra Tools/Add Scene Setup Manager`

### **Tipos de Cena Suportados:**

```csharp
public enum SceneType
{
    Auto,      // Detecção automática baseada no nome
    Gameplay,  // Cenas de jogo (InitialCave, etc.)
    Menu,      // Cenas de interface (TitleScreen, etc.)
    Cutscene   // Cenas cinematográticas
}
```

### **Configurações por Tipo:**

- **Gameplay**: GameManager + AudioManager + InputManager + EventSystem
- **Menu**: GameManager + AudioManager + InputManager + EventSystem + UI específico
- **Cutscene**: AudioManager + InputManager básico

## 🔧 Fase 2 - Modules (Expansão) - ✅ REVISADO

### **Objetivos da Fase 2**

Substituir os métodos básicos do SceneSetupManager por módulos especializados para configuração avançada.

### **Arquitetura Revisada:**

Baseado na implementação da Fase 1, os módulos da Fase 2 seguirão o padrão do `ManagerSetupModule`:

- Classes estáticas com método `Setup(SceneSetupManager)`
- Métodos privados para configurações específicas
- Logs detalhados e configuráveis
- Integração com o sistema de validação existente

### **Módulos Especializados:**

#### **CameraSetupModule.cs**

- **Localização**: `Assets/Code/Tools/SceneSetup/Modules/CameraSetupModule.cs`
- **Funcionalidades**:
  - Setup de Main Camera com configurações para pixel art
  - Adição de CinemachineBrain para cenas de gameplay
  - Configuração de PixelPerfectCamera para sprite art perfeito
  - Setup de Post Processing Layer na câmera
  - Configuração otimizada de Clear Flags e Background Color
  - Configurações específicas por tipo de cena (Gameplay/Menu/Cutscene)

#### **PostProcessSetupModule.cs**

- **Localização**: `Assets/Code/Tools/SceneSetup/Modules/PostProcessSetupModule.cs`
- **Funcionalidades**:
  - Criação e configuração de Global Volume
  - Setup de profile básico com Vignette e Color Adjustments
  - Configurações específicas para pixel art (anti-aliasing, upsampling)
  - Integração com sistema de transições existente
  - Configurações diferenciadas por tipo de cena

#### **InputSetupModule.cs**

- **Localização**: `Assets/Code/Tools/SceneSetup/Modules/InputSetupModule.cs`
- **Funcionalidades**:
  - Criação e configuração de EventSystem
  - Setup de InputSystemUIInputModule com configurações otimizadas
  - Validação de Input Actions configurados
  - Configuração de Canvas para UI responsiva (Menu scenes)
  - Setup de Input para diferentes tipos de cena

#### **LightingSetupModule.cs**

- **Localização**: `Assets/Code/Tools/SceneSetup/Modules/LightingSetupModule.cs`
- **Funcionalidades**:
  - Setup de Global Light 2D para cenas 2D
  - Configurações de intensidade otimizadas para pixel art
  - Validação e aplicação de Render Pipeline Asset
  - Configurações de ambient lighting
  - Configurações específicas por tipo de cena

### **Integração com SceneSetupManager:**

Os métodos `SetupCamera()`, `SetupInput()`, `SetupPostProcessing()` e `SetupLighting()` serão refatorados para chamar os módulos especializados, mantendo compatibilidade com a Fase 1 mas adicionando funcionalidades avançadas.

## 🎨 Fase 3 - Tools (Interface Avançada) - ✅ REVISADO

### **Objetivos da Fase 3**

Criar ferramentas visuais e templates para maximizar produtividade, baseado na arquitetura modular da Fase 2.

### **Arquitetura Revisada:**

Baseado na implementação das Fases 1 e 2, a Fase 3 deve:

- Integrar com os módulos especializados implementados
- Fornecer controle granular sobre configurações de cada módulo
- Resolver problemas de compilação através de carregamento dinâmico
- Oferecer interface visual intuitiva para gerenciamento em massa

### **Componentes Principais:**

#### **SceneSetupWindow.cs**

- **Localização**: `Assets/Code/Editor/SceneSetupWindow.cs`
- **Funcionalidades Expandidas**:
  - Lista de todas as cenas do projeto com status detalhado
  - **Controle por módulo**: Habilitar/desabilitar módulos específicos
  - Preview de configurações antes da aplicação
  - **Batch operations**: Aplicar configurações a múltiplas cenas
  - **Module testing**: Testar módulos individuais
  - Backup automático antes de modificações em massa
  - Estatísticas de projeto (cenas configuradas, problemas encontrados)

#### **Sistema de Templates Avançado**

- **SceneSetupTemplate.cs**: ScriptableObject base expandido
- **Templates Específicos por Módulo**:
  - `CameraTemplate`: Configurações de PixelPerfectCamera, Cinemachine
  - `PostProcessTemplate`: Profiles de Vignette, Color Adjustments por cena
  - `InputTemplate`: Configurações de EventSystem e Input Actions
  - `LightingTemplate`: Configurações de Global Light 2D e ambiente
- **Templates Compostos**:
  - `GameplaySceneTemplate`: Combinação de todos os módulos para gameplay
  - `MenuSceneTemplate`: Configurações otimizadas para UI
  - `CutsceneSceneTemplate`: Configurações cinematográficas

#### **SceneSetupValidator.cs**

- **Localização**: `Assets/Code/Tools/SceneSetup/SceneSetupValidator.cs`
- **Funcionalidades**:
  - **Validação modular**: Usar métodos `ValidateXXX()` dos módulos especializados
  - **Relatórios detalhados**: Problemas por módulo e por cena
  - **Auto-fix suggestions**: Botões para corrigir problemas automaticamente
  - **Continuous validation**: Validação em background durante desenvolvimento
  - **Export reports**: Relatórios exportáveis para documentação

#### **SceneSetupPresets.cs**

- **Nova funcionalidade**: Sistema de presets para configurações rápidas
- **Presets incluídos**:
  - `PixelArt2D_Preset`: Configurações otimizadas para pixel art
  - `HighRes2D_Preset`: Configurações para sprite art alta resolução
  - `Retro_Preset`: Configurações com Film Grain e efeitos retro
  - `Mobile_Preset`: Configurações otimizadas para mobile

### **Integração com Fase 2:**

- **Module Integration**: Interface para controlar módulos especializados individualmente
- **Dynamic Loading**: Carregamento dinâmico de módulos para resolver problemas de compilação
- **Configuration Inheritance**: Templates que herdam configurações dos módulos da Fase 2

## 📊 Benefícios por Fase

### **Fase 1 - Benefícios Imediatos:**

- ✅ Resolve problema da InitialCave
- ✅ Padroniza configuração básica de managers
- ✅ Integra com menu Extra Tools existente
- ✅ Base sólida para expansão futura

### **Fase 2 - Benefícios de Produtividade:**

- 🎯 Setup completo de câmeras e post-processing
- 🎯 Configurações otimizadas para pixel art
- 🎯 Consistência visual entre cenas
- 🎯 Redução significativa de tempo de setup

### **Fase 3 - Benefícios de Escala:**

- 🚀 Interface visual intuitiva
- 🚀 Templates reutilizáveis
- 🚀 Validação automática de projeto
- 🚀 Workflow otimizado para equipes

## 🎯 Casos de Uso

### **Para InitialCave (Fase 1):**

1. Adicionar `SceneSetupManager` à cena
2. Configurar como `SceneType.Gameplay`
3. Executar setup automático
4. Sistema cria: GameManager, AudioManager, InputManager, EventSystem

### **Para Novas Cenas (Fase 2):**

1. Criar nova cena
2. Adicionar `SceneSetupManager`
3. Sistema detecta tipo automaticamente
4. Configura câmera, post-processing, iluminação automaticamente

### **Para Projeto Completo (Fase 3):**

1. Abrir Scene Setup Window
2. Visualizar status de todas as cenas
3. Aplicar template específico a múltiplas cenas
4. Validar e corrigir problemas automaticamente

## 📈 Cronograma de Implementação

### **Fase 1 - Imediata (1-2 dias)**

- Criação do core system
- Resolução do problema da InitialCave
- Integração básica com Extra Tools

### **Fase 2 - Curto Prazo (3-5 dias)**

- Implementação de módulos especializados
- Configurações avançadas de cena
- Otimizações para pixel art

### **Fase 3 - Médio Prazo (1-2 semanas)**

- Interface visual completa
- Sistema de templates
- Ferramentas de validação avançada

## 🔍 Considerações Técnicas

### **Performance:**

- Setup executado apenas quando necessário
- Cache de validações para evitar reprocessamento
- Lazy loading de módulos especializados

### **Compatibilidade:**

- Unity 6.3+ (conforme boas práticas)
- URP (Universal Render Pipeline)
- Input System (sem detecção direta de input)

### **Manutenibilidade:**

- Arquitetura modular para fácil extensão
- Documentação inline detalhada
- Logs configuráveis para debug
- Testes automatizados (Fase 3)

---

*Este documento será atualizado conforme o progresso da implementação.*
