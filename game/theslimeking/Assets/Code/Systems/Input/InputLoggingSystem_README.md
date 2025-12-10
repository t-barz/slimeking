# InputLoggingSystem - Documentação

**Versão:** 1.0  
**Data:** 10 de Dezembro de 2025  
**Localização:** `Assets/Code/Systems/Input/InputLoggingSystem.cs`

## 📋 Visão Geral

`InputLoggingSystem` é um componente de debug que monitora e registra todos os inputs do jogador em tempo real. Útil para:

- ✅ Validar mapeamentos de controles
- ✅ Verificar detecção de dispositivos
- ✅ Testar inputs durante desenvolvimento
- ✅ Documentar sequências de inputs para bugs
- ✅ Monitorar compatibilidade entre diferentes dispositivos

## 🎮 Funcionalidades

### 1. **Detecção Automática de Tipo de Controle**

Ao ativar, o sistema detecta automaticamente qual tipo de dispositivo está sendo usado:

```
🎮 TIPO DE CONTROLE DETECTADO: Xbox Controller (XInput)
🎮 TIPO DE CONTROLE DETECTADO: Teclado e Mouse
🎮 TIPO DE CONTROLE DETECTADO: PlayStation Controller
🎮 TIPO DE CONTROLE DETECTADO: Nintendo Switch Controller
```

**Tipos Suportados:**

- Xbox Controller (XInput)
- PlayStation Controller (DualShock/DualSense)
- Nintendo Switch Controller
- Joystick genérico
- Teclado + Mouse

### 2. **Log de Todos os Inputs**

Registra todos os botões pressionados com timestamp:

```
[14:35:22.123] ▶ MOVIMENTO (Cima Direita)
[14:35:23.456] ▶ ATAQUE (Espaço / B (Gamepad))
[14:35:24.789] ▶ USAR ITEM 1 (1 / LB (Gamepad))
[14:35:25.234] ▶ AGACHAR (X / X (Gamepad))
[14:35:26.567] ▶ AGACHAR CANCELADO (X / X (Gamepad))
```

**Inputs Monitorados:**

- **Movimento:** WASD / Setas / Analog Stick
- **Ataque:** Espaço / B (Gamepad)
- **Interagir:** E / A (Gamepad)
- **Agachar:** X / X (Gamepad)
- **Abrir Inventário:** I / Select (Gamepad)
- **Quick Slots 1-4:** 1-4 / LB/LT/RB/RT (Gamepad)
- **Pausar:** Esc / Start (Gamepad)

### 3. **Logs Coloridos**

Utiliza cores no console para facilitar leitura:

- 🔵 **Ciano:** Detecção de tipo de controle
- 🟡 **Amarelo:** Inputs registrados

## 🔧 Como Usar

### Método 1: Via Menu Editor

1. Abra a cena `3_InitialForest`
2. Vá ao menu `Extra Tools → Setup → Create InputLoggingSystem`
3. Clique para criar o sistema

O GameObject `[Debug] InputLoggingSystem` será criado automaticamente.

### Método 2: Manual

1. Crie um GameObject vazio na cena
2. Adicione o componente `InputLoggingSystem`
3. Configure as opções no Inspector (todas habilitadas por padrão)

## ⚙️ Configurações no Inspector

```
┌─────────────────────────────────────────┐
│ InputLoggingSystem                      │
├─────────────────────────────────────────┤
│ ☑ Enable Logging                        │
│ ☑ Log Input Type                        │
│ ☑ Log Button Presses                    │
│ ☑ Use Colored Logs                      │
└─────────────────────────────────────────┘
```

**Opções:**

- **Enable Logging:** Ativa/desativa o sistema completamente
- **Log Input Type:** Registra o tipo de controle detectado na inicialização
- **Log Button Presses:** Registra cada botão pressionado
- **Use Colored Logs:** Usa cores no console (recomendado)

## 📊 Exemplo de Uso

### Cenário 1: Testar Mapeamento de Controles

```
1. Abra a cena 3_InitialForest
2. Crie o InputLoggingSystem via menu
3. Rode o jogo (Play)
4. Pressione cada botão e veja o log no Console
5. Valide se os inputs estão corretos
```

### Cenário 2: Verificar Compatibilidade de Gamepad

```
1. Conecte diferentes controllers (Xbox, PS5, Switch)
2. Vire um de cada vez enquanto o jogo está rodando
3. Observe qual tipo é detectado no console
4. Teste inputs de cada controller
```

### Cenário 3: Documentar Bug de Input

```
1. Rode o jogo com InputLoggingSystem ativo
2. Reproduza o bug
3. Copie o log do console mostrando a sequência de inputs
4. Reporte incluindo o log de inputs
```

## 💡 Exemplo de Log Completo

```
🎮 TIPO DE CONTROLE DETECTADO: Xbox Controller (XInput)
[14:35:22.001] ▶ MOVIMENTO (Direita)
[14:35:22.456] ▶ MOVIMENTO (Direita Cima)
[14:35:23.234] ▶ ATAQUE (Espaço / B (Gamepad))
[14:35:23.678] ▶ MOVIMENTO PARADO (---)
[14:35:24.123] ▶ USAR ITEM 1 (1 / LB (Gamepad))
[14:35:24.567] ▶ AGACHAR (X / X (Gamepad))
[14:35:25.234] ▶ AGACHAR CANCELADO (X / X (Gamepad))
[14:35:25.789] ▶ ABRIR INVENTÁRIO (I / Select (Gamepad))
[14:35:26.123] ▶ PAUSAR JOGO (Esc / Start (Gamepad))
```

## 🔍 Troubleshooting

### "Tipo de controle não detectado"

**Solução:** Verifique se o InputSystem_Actions.inputactions está configurado corretamente.

### "Log não aparece no console"

**Solução:**

1. Verifique se `Enable Logging` está marcado
2. Verifique se `Log Button Presses` está marcado
3. Verifique se não há outros sistemas filtrando console

### "Detectou tipo errado de gamepad"

**Solução:** Isso é normal com emuladores. Drivers desatualizados podem causar isto. Tente atualizar drivers do seu controller.

## 📝 Implementação Técnica

### Estrutura de Subscrição

O sistema subscreve aos eventos do InputSystem_Actions:

```csharp
// Movimento
_inputActions.Gameplay.Move.performed += LogMovementInput;
_inputActions.Gameplay.Move.canceled += LogMovementCanceled;

// Ações principais
_inputActions.Gameplay.Attack.performed += LogButton("ATAQUE", ...);
_inputActions.Gameplay.Interact.performed += LogButton("INTERAGIR", ...);
// ... etc
```

### Detecção de Dispositivo

```csharp
if (Gamepad.current != null)
{
    // Xbox, PS5, Switch, etc.
    deviceType = GetGamepadType();
}
else if (Keyboard.current != null)
{
    deviceType = "Teclado e Mouse";
}
```

### Performance

- ✅ Mínimo overhead (apenas logs)
- ✅ Event-driven (não poleia inputs)
- ✅ Seguro para builds finais (pode ser desabilitado via Inspector)

## 🎯 Casos de Uso Recomendados

| Caso | Recomendação |
|------|-------------|
| Desenvolvimento | ✅ Sempre ativar |
| Testes QA | ✅ Ativar para reportar bugs |
| Build Final | ❌ Desativar ou remover |
| Multiplayer | ⚠️ Apenas servidor/debug |

## 📦 Arquivo de Suporte

**Editor Tool:** `Assets/Code/Editor/ExtraTools/Setup/InputLoggingSystemCreator.cs`

Cria automaticamente o GameObject com o componente via menu do Editor.

## 🔄 Changelog

**v1.0 (10/12/2025)**

- ✅ Detecção automática de tipo de controle
- ✅ Log de todos os inputs com timestamp
- ✅ Suporte a cores no console
- ✅ Editor tool para criar automaticamente
- ✅ Configurações ajustáveis via Inspector

---

**Autor:** Equipe de Desenvolvimento SlimeKing  
**Status:** ✅ Produção
