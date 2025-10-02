# Próximos Passos - The Slime King

## 🎯 **Tarefas Prioritárias**

### **1. Limpeza do AudioManager**

- [x] **Remover referências à música de Splash**
  - ✅ Remover `splashMusic` AudioClip field
  - ✅ Remover método `PlaySplashMusic()`
  - ✅ Remover propriedade `IsPlayingSplashMusic`
  - ✅ Atualizar comentários e documentação
  - ✅ Simplificar lógica de crossfade (sem splash)

### **2. Revisão Geral do Código**

- [x] **GameManager**
  - ✅ Verificar se ainda há referências órfãs ao sistema de Splash
  - ✅ Validar fluxo de estados (MainMenu → Options → Loading → Exploring)
  - ✅ Revisar comentários XML e inline
  - ✅ Testar todas as transições de estado

- [x] **AudioManager**
  - ✅ Limpar código após remoção do Splash
  - ✅ Validar configurações de volume
  - ✅ Testar crossfade entre Menu e Gameplay
  - ✅ Verificar persistência entre cenas

- [x] **TitleScreenController**
  - ✅ Validar sequência de animações
  - ✅ Testar controles de skip
  - ✅ Verificar integração com AudioManager
  - ✅ Ajustar timings se necessário

- [x] **Estrutura de Arquivos**
  - ✅ Remover `SplashScreenController.cs` (já removido)
  - ✅ Atualizar documentação (SplashScreen_Setup.md não existe mais)
  - ✅ Verificar imports e using statements desnecessários

### **3. Implementação do Input System**

- [ ] **Setup Inicial**
  - ✅ Input System package já instalado
  - Criar Input Action Asset principal
  - Configurar esquemas de controle (Keyboard, Gamepad)

- [ ] **Actions Principais**
  - **UI Navigation**: Navigate, Submit, Cancel
  - **Gameplay**: Movement, Jump, Attack, Interact
  - **System**: Pause, Menu, Skip

- [ ] **Integração**
  - Substituir `Input.GetKeyDown()` por Input Actions
  - Implementar input handling no TitleScreenController
  - Configurar input para navegação de menus
  - Adicionar suporte a gamepad

- [ ] **Configurações**
  - Sistema de rebinding de teclas
  - Profiles de input por jogador
  - Sensibilidade e dead zones

### **4. Post Processing**

- [ ] **Setup do URP**
  - Verificar se Universal Render Pipeline está configurado
  - Criar Volume Profile global
  - Configurar Volume Component na cena

- [ ] **Efeitos Base**
  - **Bloom**: Para elementos mágicos e cristais
  - **Color Grading**: Tom geral do jogo
  - **Vignette**: Atmosfera nas bordas
  - **Chromatic Aberration**: Sutil para polish visual

- [ ] **Efeitos por Bioma**
  - Volume Profiles específicos por área
  - Transições suaves entre biomas
  - Efeitos de profundidade (Depth of Field)

- [ ] **Efeitos de Gameplay**
  - Screen shake (via Cinemachine)
  - Hit effects (flash, desaturação)
  - Evolução visual (particle effects + post processing)

## 🔧 **Tarefas Técnicas Complementares**

### **5. Otimização e Performance**

- [ ] **Audio**
  - Verificar se AudioSources estão sendo pooled
  - Configurar compressão adequada dos AudioClips
  - Implementar fade in/out otimizado

- [ ] **Scene Management**
  - Implementar loading real com async operations
  - Sistema de preload inteligente
  - Garbage collection otimizada

### **6. UI/UX Melhorias**

- [ ] **TitleScreen**
  - Adicionar particle effects nas animações
  - Implementar input responsivo ("Press Any Key")
  - Melhorar feedback visual nos elementos

- [ ] **Menu System**
  - Navegação por teclado/gamepad
  - Animações de transição entre menus
  - Sistema de configurações visuais

### **7. Architecture & Code Quality**

- [ ] **Event System**
  - Revisar todos os eventos customizados
  - Implementar unsubscribe automático
  - Documentar event flows

- [ ] **Singleton Management**
  - Verificar lifecycle dos singletons
  - Implementar cleanup adequado
  - Testar comportamento entre cenas

## 📅 **Cronograma Sugerido**

### **Semana 1: Limpeza**

- ✅ Remover Splash do AudioManager
- ✅ Revisão completa do código existente
- ✅ Testes de integração

### **Semana 2: Input System**

- Setup e configuração básica
- Implementação nas telas existentes
- Testes com keyboard e gamepad

### **Semana 3: Post Processing**

- Setup do URP e Volume Profiles
- Implementação de efeitos base
- Ajustes visuais e polish

### **Semana 4: Polish & Testing**

- Otimizações finais
- Testes de performance
- Documentação atualizada

## 🎯 **Critérios de Sucesso**

- ✅ **Música tocando corretamente na TitleScreen**
- ✅ **Transições fluidas entre cenas**
- ✅ **Input responsivo em todos os contextos**
- ✅ **Visual polish com post processing**
- ✅ **Código limpo e bem documentado**
- ✅ **Performance estável (60 FPS target)**

---

**Observações:**

- Priorizar sempre funcionalidade sobre visual
- Testar em build além do editor
- Manter documentação atualizada a cada mudança significativa
- Fazer commits frequentes com mensagens descritivas
