/*
===============================================================================
SOLUÇÃO PARA PROBLEMAS DE POST PROCESSING - SLIME KING
===============================================================================

PROBLEMA ORIGINAL:
- Post Processing às vezes não funciona na Game View ao trocar da cena 1_TitleScreen 
  para 2_InitialCave, apesar de funcionar na Scene View
- Comportamento inconsistente e sem padrão identificável

CAUSA RAIZ IDENTIFICADA:
1. Múltiplas câmeras "Main Camera" ativas simultaneamente
2. Conflitos de renderização URP durante transições
3. Global Volumes não sendo reconhecidos corretamente

SOLUÇÃO IMPLEMENTADA:
===============================================================================

1. CAMERA MANAGER (CameraManager.cs)
   - Detecta automaticamente a câmera principal com maior depth
   - Remove câmeras duplicadas ou conflitantes
   - Força refresh do Post Processing e Global Volumes
   - Integra-se com o GameManager para ativação automática

2. SCENE SETUP VALIDATOR (SceneSetupValidator.cs)
   - Valida configurações de cena automaticamente
   - Corrige configurações incorretas de URP
   - Verifica Global Volumes e suas prioridades
   - Executa validação ao iniciar a cena

3. INTEGRAÇÃO COM GAMEMANAGER
   - CameraManager é ativado automaticamente após carregamento de cena
   - Refresh das configurações de câmera integrado ao fluxo de cleanup existente

4. FERRAMENTAS DE SETUP (CameraSetupTools.cs)
   - Menu Tools/SlimeKing/Camera Setup com várias opções
   - Adiciona componentes às cenas facilmente
   - Permite validação manual e refresh forçado

COMO USAR:
===============================================================================

SETUP INICIAL (uma vez por cena problemática):
1. Abra a cena 2_InitialCave
2. Menu: Tools → SlimeKing → Camera Setup → Setup Complete Scene
3. Isso adicionará CameraManager e SceneSetupValidator automaticamente

VERIFICAÇÃO:
1. Execute o jogo e faça a transição TitleScreen → InitialCave
2. Verifique se Post Processing funciona consistentemente
3. Console mostrará logs do CameraManager (se enableLogs = true)

SOLUÇÃO DE PROBLEMAS:
- Se ainda houver problemas: Tools → SlimeKing → Camera Setup → Force Camera Refresh
- Para validar cena: Tools → SlimeKing → Camera Setup → Validate Current Scene
- Verifique o Console para logs detalhados

CONFIGURAÇÕES AUTOMÁTICAS APLICADAS:
===============================================================================
- Câmera com maior depth definida como principal
- Câmeras duplicadas automaticamente desabilitadas
- UniversalAdditionalCameraData adicionado se necessário
- renderPostProcessing = true forçado
- Global Volumes reativados para refresh
- Timing correto após carregamento de cena

ARQUIVOS CRIADOS:
===============================================================================
- Assets/💻 Code/Systems/Managers/CameraManager.cs
- Assets/💻 Code/Editor/SceneSetupValidator.cs  
- Assets/💻 Code/Editor/CameraSetupTools.cs
- Assets/💻 Code/Editor/README_PostProcessingFix.cs (este arquivo)

MODIFICAÇÕES EM ARQUIVOS EXISTENTES:
===============================================================================
- GameManager.cs: Integração com CameraManager após ativação de cena

NOTAS TÉCNICAS:
===============================================================================
- Solução segue padrões do projeto (namespace SlimeKing.Core)
- Usa ManagerSingleton pattern para consistência
- Logs controlados por flag enableLogs
- Editor tools apenas disponíveis em #if UNITY_EDITOR
- Compatível com URP e Unity 6000.2.7f2

TESTE RECOMENDADO:
===============================================================================
1. Teste transições múltiplas vezes
2. Verifique Post Processing em diferentes resoluções
3. Teste em Build (não apenas Editor)
4. Verifique performance (não deve haver impacto significativo)

Se ainda houver problemas após implementação, verifique:
- Se há outras câmeras ocultas na hierarquia
- Configurações específicas do URP Asset
- Ordem de execução de scripts (CameraManager deve executar após GameManager)
===============================================================================
*/

// Este arquivo é apenas documentação - pode ser removido após implementação
using UnityEngine;

namespace SlimeKing.Core.Documentation
{
    public class README_PostProcessingFix : MonoBehaviour
    {
        [TextArea(10, 20)]
        public string documentation = @"
Veja o código fonte deste arquivo para documentação completa 
da solução de Post Processing implementada.

Esta solução resolve problemas intermitentes de Post Processing
durante transições de cena no projeto SlimeKing.
        ";
    }
}
