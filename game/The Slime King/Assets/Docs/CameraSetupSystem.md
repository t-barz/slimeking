# Planejamento: Sistema de Configuração de Câmera Independente

## Objetivo

Criar um sistema acessível via menu `Extra Tools > Camera Setup` que configure automaticamente uma câmera otimizada para Pixel Art, com Cinemachine, Post Processing e pronta para Follow.

---

## Funcionalidades

1. **Menu Rápido no Editor**
   - Opção: `Extra Tools > Camera Setup > Setup Pixel Art Camera`
   - Executa o setup completo da câmera na cena ativa

2. **Configuração da Main Camera**
   - Garante existência da Main Camera
   - Adiciona/Configura:
     - `CinemachineBrain`
     - `UniversalAdditionalCameraData` (URP)
     - PixelPerfectCamera (se disponível)
   - Ativa Post Processing
   - Ajusta propriedades para Pixel Art (ortográfica, background, HDR/MSAA off)

3. **Criação da CinemachineVirtualCamera**
   - Cria se não existir
   - Configura prioridade
   - Configura Follow para objeto com tag `Player` (se existir)
   - Ajusta Blend, Framing, DeadZone para Pixel Art

4. **Configuração de Post Processing**
   - Garante que o Post Processing está ativo na Main Camera
   - Configura Volume Layer Mask
   - Sugere/Cria Global Volume se necessário

5. **Pixel Art Otimização**
   - Ativa PixelPerfectCamera (se disponível)
   - Configura PPU, resolução de referência, upscaleRT, pixelSnapping

6. **Logs e Feedback**
   - Exibe logs detalhados no Console
   - Informa sucesso, warnings e erros

---

## Estrutura de Código

- Arquivo principal: `CameraSetupMenuItems.cs` (Editor)
- Função principal: `SetupPixelArtCamera()`
- Utiliza métodos utilitários para cada etapa
- Modularizado para fácil expansão (ex: presets para outros estilos)

---

## Fluxo de Execução

1. Usuário acessa menu `Extra Tools > Camera Setup > Setup Pixel Art Camera`
2. Sistema executa:
   - Garante Main Camera
   - Adiciona/configura componentes necessários
   - Cria/configura CinemachineVirtualCamera
   - Ativa Post Processing
   - Otimiza para Pixel Art
   - Exibe logs
3. Usuário pode rodar quantas vezes quiser para garantir conformidade

---

## Extensões Futuras

- Suporte a presets para outros estilos (HighRes2D, Retro, Mobile)
- Interface visual para ajustes finos
- Integração com templates e validação

---

## Arquivos Envolvidos

- `Assets/Code/Editor/CameraSetupMenuItems.cs` (novo)
- `Assets/Docs/CameraSetupSystem.md` (este planejamento)

---

## Critérios de Aceitação

- Menu aparece no Editor
- Setup executa corretamente todos os passos
- Câmera fica pronta para Pixel Art, Cinemachine e Post Processing
- Logs claros e informativos
