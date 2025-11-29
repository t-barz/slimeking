# TechMapping.md

## Mapeamento de Prefabs, Scripts e Componentes Utilizados

| Prefab                                      | Scripts/Componentes MonoBehaviour Utilizados                                                                                   | Outros Componentes (Unity)                                                                                   |
|----------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------|
| GameManager                                 | SlimeKing.Core.GameManager                                                                                                    | Transform                                                                                                   |
| SceneTransitioner                           | guid: 77b525215f79aa7468598d85400f45f5                                                                                       | Transform                                                                                                   |
| TeleportManager                             | PixeLadder.EasyTransition.TeleportManager                                                                                     | Transform                                                                                                   |
| teleportPoint                               | guid: 99109c29d6cadb347b2dff11b3924618                                                                                       | Transform, BoxCollider2D                                                                                    |
| Main Camera                                 | Unity.Cinemachine.CinemachineCamera, Unity.Cinemachine.CinemachineFollow, guid: a79441f348de89743a2939f4d699eac1             | Transform, Camera, AudioListener                                                                            |
| Characters/chr_whiteslime                    | Unity.InputSystem.PlayerInput, PlayerAttributesHandler, PlayerController                                                      | Transform, SpriteRenderer, AudioSource, Rigidbody2D, CircleCollider2D                                       |
| Characters/absorve_vfx                      | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| Characters/attack01_vfx                     | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| Characters/hit01_vfx                        | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| Characters/notHit01_vfx                     | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| Items/crystalA                              | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, PolygonCollider2D                                                                |
| Items/item_RedFruit                         | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer                                                                                   |
| Props/caveEntrance                          | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, PolygonCollider2D                                                                                |
| Props/escadaria                             | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, BoxCollider2D                                                                                    |
| External/AssetStore/SlimeMec/_Prefabs/Environment/WindManager      | guid: 8634c46eb9f224d46b148e48ccf6fb2e                                                                                       | Transform                                                                                                   |
| External/AssetStore/SlimeMec/_Prefabs/Environment/wind_vfx         | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| External/AssetStore/SlimeMec/_Prefabs/Environment/bushA2           | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| External/AssetStore/SlimeMec/_Prefabs/Environment/env_brown_rockA  | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| External/AssetStore/SlimeMec/_Prefabs/Systems/jumpPoint            | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| External/AssetStore/SlimeMec/_Prefabs/Systems/shrinkPoint          | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| External/AssetStore/SlimeMec/_Prefabs/UI/interactivePointAct1      | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |
| External/AssetStore/SlimeMec/_Prefabs/UI/interactivePointAct2      | Nenhum script personalizado (apenas componentes padrão)                                                                       | Transform, SpriteRenderer, Animator                                                                         |

# TechMapping.md

## Mapeamento de Classes e Dependências

### Assets/Code

- **TeleportManager**
  - Objetivo: Gerencia teleporte de objetos/jogador entre pontos/cenas.
  - Utiliza: PlayerController, Rigidbody2D

- **PuddleDrop**
  - Objetivo: Controla o comportamento de gotas em poças (efeito visual/gameplay).
  - Utiliza: WaitForSeconds

- **BushQuickConfig**
  - Objetivo: Ferramenta de configuração rápida para arbustos no editor.
  - Utiliza: WindEmulator

- **BushSetupWizard**
  - Objetivo: Janela do editor para setup de arbustos.
  - Utiliza: BushQuickConfig

- **GizmosHelper**
  - Objetivo: Auxilia na visualização de gizmos customizados.
  - Utiliza: Collider2D

- **InitialCaveScreenController**
  - Objetivo: Controla tela inicial da caverna.
  - Utiliza: MonoBehaviour

- **TeleportTransitionHelper**
  - Objetivo: Utilitário para transições visuais de teleporte.
  - Utiliza: Material

- **TeleportPoint**
  - Objetivo: Define pontos de teleporte no mapa.
  - Utiliza: PlayerController, BoxCollider2D

- **SceneSetupValidator**
  - Objetivo: Valida configuração da cena.
  - Utiliza: MonoBehaviour

- **OutlineExample**
  - Objetivo: Exemplo de uso de outline em sprites.
  - Utiliza: MonoBehaviour

- **CameraSetupTools**
  - Objetivo: Ferramenta de setup de câmera no editor.
  - Utiliza: CameraManager

- **CameraManager**
  - Objetivo: Gerencia câmeras e transições.
  - Utiliza: ManagerSingleton

- **TitleScreenController**
  - Objetivo: Controla tela de título/menu inicial.
  - Utiliza: AudioSource

- **GameManager**
  - Objetivo: Gerencia ciclo de vida do jogo, preload e ativação de cenas.
  - Utiliza: ManagerSingleton

- **OutlineUtility**
  - Objetivo: Utilitário para outline de sprites.
  - Utiliza: Material

- **ItemQuickConfig**
  - Objetivo: Ferramenta de configuração rápida de itens no editor.
  - Utiliza: StringBuilder

- **ExtraTools**
  - Objetivo: Ferramentas extras para o editor.
  - Utiliza: HashSet, StreamWriter

- **ManagerSingleton**
  - Objetivo: Base para managers globais persistentes.
  - Utiliza: MonoBehaviour

- **PolygonGizmosHelper**
  - Objetivo: Gizmos para polígonos customizados.
  - Utiliza: PolygonCollider2D

- **ProjectSettingsExporterWindow**
  - Objetivo: Exporta configurações do projeto Unity.
  - Utiliza: SerializedObject

- **SceneTransitionManager**
  - Objetivo: Gerencia transições de cena com efeitos visuais.
  - Utiliza: GameObject, Material

- **README_PostProcessingFix**

  # Mapeamento Técnico das Classes

  ## Assets/Code

  ### TeleportManager

  | Objetivo | Gerencia teleporte de objetos/jogador entre pontos/cenas |
  | Utiliza  | PlayerController, Rigidbody2D |

  | Função | Descrição |
  |---|---|
  | PlayTeleportSound(AudioClip clip) | Toca som de teleporte |
  | ExecuteCrossSceneTeleport(...) | Executa teleporte entre cenas |

  ### PuddleDrop

  | Objetivo | Controla o comportamento de gotas em poças (efeito visual/gameplay) |
  | Utiliza  | WaitForSeconds |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ### BushQuickConfig

  | Objetivo | Ferramenta de configuração rápida para arbustos no editor |
  | Utiliza  | WindEmulator |

  | Função | Descrição |
  |---|---|
  | ConfigureAsBush(MenuCommand) | Configura objeto como arbusto |
  | ValidateConfigureAsBush() | Valida configuração de arbusto |
  | ConfigureBushComponents(GameObject) | Configura componentes de arbusto |
  | ShowBushSetupWizard() | Exibe assistente de setup |
  | ValidateBushSetupWizard() | Valida assistente |
  | ShowBushInfo() | Exibe informações do arbusto |
  | ValidateShowBushInfo() | Valida exibição de info |
  | ConfigureBushAnimatorStatesAndTriggers() | Configura animações |
  | ShowWindow() | Exibe janela do editor |

  ### BushSetupWizard

  | Objetivo | Janela do editor para setup de arbustos |
  | Utiliza  | BushQuickConfig |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ### GizmosHelper

  | Objetivo | Auxilia na visualização de gizmos customizados |
  | Utiliza  | Collider2D |

  | Função | Descrição |
  |---|---|
  | RefreshGizmos() | Atualiza gizmos na cena |
  | ShowColliderInfo() | Exibe informações dos colliders |

  ### InitialCaveScreenController

  | Objetivo | Controla tela inicial da caverna |
  | Utiliza  | MonoBehaviour |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ### TeleportTransitionHelper

  | Objetivo | Utilitário para transições visuais de teleporte |
  | Utiliza  | Material |

  | Função | Descrição |
  |---|---|
  | ExecuteTransition(...) | Executa corrotina de transição visual |

  ### TeleportPoint

  | Objetivo | Define pontos de teleporte no mapa |
  | Utiliza  | PlayerController, BoxCollider2D |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ### SceneSetupValidator

  | Objetivo | Valida configuração da cena |
  | Utiliza  | MonoBehaviour |

  | Função | Descrição |
  |---|---|
  | ValidateScene() | Valida a cena atual |

  ### OutlineExample

  | Objetivo | Exemplo de uso de outline em sprites |
  | Utiliza  | MonoBehaviour |

  | Função | Descrição |
  |---|---|
  | ActivateOutline() | Ativa outline |
  | DeactivateOutline() | Desativa outline |
  | ToggleOutline() | Alterna estado do outline |
  | ChangeOutlineColor(Color newColor) | Altera cor do outline |

  ### CameraSetupTools

  | Objetivo | Ferramenta de setup de câmera no editor |
  | Utiliza  | CameraManager |

  | Função | Descrição |
  |---|---|
  | AddCameraManagerToScene() | Adiciona CameraManager à cena |
  | AddSceneValidatorToScene() | Adiciona validador à cena |
  | SetupCompleteScene() | Configura cena completa |
  | ValidateCurrentScene() | Valida cena atual |
  | ForceCameraRefresh() | Força refresh da câmera |
  | CleanOldFiles() | Limpa arquivos antigos |

  ### CameraManager

  | Objetivo | Gerencia câmeras e transições |
  | Utiliza  | ManagerSingleton |

  | Função | Descrição |
  |---|---|
  | RefreshMainCamera() | Atualiza referência da câmera principal |
  | OnSceneLoaded() | Executa ações ao carregar cena |
  | ForceRefresh() | Força atualização da câmera |
  | GetMainCamera() | Retorna câmera principal |
  | GetMainCameraData() | Retorna dados da câmera principal |

  ### TitleScreenController

  | Objetivo | Controla tela de título/menu inicial |
  | Utiliza  | AudioSource |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ### GameManager

  | Objetivo | Gerencia ciclo de vida do jogo, preload e ativação de cenas |
  | Utiliza  | ManagerSingleton |

  | Função | Descrição |
  |---|---|
  | HasPreloadedScene(string) | Verifica se há cena pré-carregada |
  | PreloadScene(string) | Pré-carrega cena |
  | ActivatePreloadedScene(Action) | Ativa cena pré-carregada |

  ### OutlineUtility

  | Objetivo | Utilitário para outline de sprites |
  | Utiliza  | Material |

  | Função | Descrição |
  |---|---|
  | SetupAutoOutline(...) | Cria outline automático |
  | SetupManualOutline(...) | Cria outline manual |
  | SetupOutlineMaterial(SpriteRenderer) | Configura material de outline |
  | SetDefaultOutlineProperties(Material) | Define propriedades padrão |
  | RemoveOutline(GameObject) | Remove outline |
  | HasValidOutline(GameObject) | Verifica se há outline válido |

  ### ItemQuickConfig

  | Objetivo | Ferramenta de configuração rápida de itens no editor |
  | Utiliza  | StringBuilder |

  | Função | Descrição |
  |---|---|
  | ConfigureAsItem(MenuCommand) | Configura objeto como item |
  | ValidateConfigureAsItem() | Valida configuração de item |
  | ConfigureItemComponents(GameObject) | Configura componentes de item |
  | ExportGameObjectStructure(MenuCommand) | Exporta estrutura de objeto |

  ### ExtraTools

  | Objetivo | Ferramentas extras para o editor |
  | Utiliza  | HashSet, StreamWriter |

  | Função | Descrição |
  |---|---|
  | ShowWindow() | Exibe janela de ferramentas |
  | MenuCreateFolderStructure() | Cria estrutura de pastas |
  | MenuReorganizeAssets() | Reorganiza assets |
  | MenuCompleteSetup() | Setup completo |
  | MenuToggleLogs() | Alterna logs |
  | MenuExportSceneStructure() | Exporta estrutura de cena |
  | MenuSetupGlobalVolume() | Setup volume global |
  | MenuSetupForestVolume() | Setup volume floresta |
  | MenuSetupCaveVolume() | Setup volume caverna |
  | MenuSetupCrystalVolume() | Setup volume cristal |
  | MenuSetupGameplayEffects() | Setup efeitos de gameplay |

  ### ManagerSingleton

  | Objetivo | Base para managers globais persistentes |
  | Utiliza  | MonoBehaviour |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ### PolygonGizmosHelper

  | Objetivo | Gizmos para polígonos customizados |
  | Utiliza  | PolygonCollider2D |

  | Função | Descrição |
  |---|---|
  | RefreshPolygonCache() | Atualiza cache de polígonos |
  | ShowPolygonStatistics() | Exibe estatísticas dos polígonos |

  ### ProjectSettingsExporterWindow

  | Objetivo | Exporta configurações do projeto Unity |
  | Utiliza  | SerializedObject |

  | Função | Descrição |
  |---|---|
  | Open() | Abre janela de exportação |

  ### SceneTransitionManager

  | Objetivo | Gerencia transições de cena com efeitos visuais |
  | Utiliza  | GameObject, Material |

  | Função | Descrição |
  |---|---|
  | LoadSceneWithTransition(string) | Carrega cena com transição visual |
  | LoadSceneWithTransition(int) | Carrega cena por índice com transição |

  ### README_PostProcessingFix

  | Objetivo | Documentação sobre correção de pós-processamento |
  | Utiliza  | MonoBehaviour |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ## Assets/External/AssetStore/SlimeMec/_Scripts

  ### OutlineController

  | Objetivo | Controla outline visual de sprites |
  | Utiliza  | SpriteRenderer, Material |

  | Função | Descrição |
  |---|---|
  | ShowOutline(bool) | Exibe ou oculta outline |
  | ActivateOutline() | Ativa outline |
  | DeactivateOutline() | Desativa outline |
  | ToggleOutline() | Alterna outline |
  | UpdateOutlineColor(Color) | Atualiza cor do outline |
  | UpdateOutlineSize(float) | Atualiza tamanho do outline |
  | UpdateAlphaThreshold(float) | Atualiza threshold alfa |
  | SetDetectionRadius(float) | Define raio de detecção |
  | SetAutoDetection(bool) | Ativa detecção automática |
  | SetFadeEnabled(bool) | Ativa fade |

  ### AttackHandler

  | Objetivo | Gerencia ataques do jogador/inimigos |
  | Utiliza  | Collider2D, BushDestruct, RockDestruct |

  | Função | Descrição |
  |---|---|
  | PerformAttack(...) | Executa ataque |
  | ClearComponentCache() | Limpa cache de componentes |

  ### BounceHandler

  | Objetivo | Gerencia física de quique/bounce de objetos |
  | Utiliza  | Rigidbody2D, ItemCollectable |

  | Função | Descrição |
  |---|---|
  | LaunchItem() | Lança item |
  | LaunchItem(float, float) | Lança item com força/ângulo |
  | StopMovementManually() | Para movimento manualmente |
  | ResetLaunch() | Reseta lançamento |
  | EnableColliders() | Ativa colliders |
  | DisableColliders() | Desativa colliders |

  ### BushDestruct

  | Objetivo | Gerencia destruição de arbustos |
  | Utiliza  | Animator, DropController |

  | Função | Descrição |
  |---|---|
  | TakeDamage() | Aplica dano ao arbusto |
  | DestroyObject() | Destroi arbusto |

  ### BushShake

  | Objetivo | Gerencia animação de shake em arbustos |
  | Utiliza  | Animator |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ### CollectableItemData

  | Objetivo | Dados de itens coletáveis |
  | Utiliza  | ScriptableObject |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ### DropController

  | Objetivo | Gerencia drops de itens |
  | Utiliza  | MonoBehaviour |

  | Função | Descrição |
  |---|---|
  | DropItems() | Realiza drop de itens |

  ### InteractivePointHandler

  | Objetivo | Gerencia pontos interativos no mapa |
  | Utiliza  | OutlineController |

  | Função | Descrição |
  |---|---|
  | (Sem funções públicas detectadas) |  |

  ### ItemBuffHandler

  | Objetivo | Gerencia buffs de itens |
  | Utiliza  | PlayerAttributesHandler |

  | Função | Descrição |
  |---|---|
  | AddBuff(CollectableItemData) | Adiciona buff |
  | ClearAllBuffs() | Limpa buffs |
  | GetTotalAttackBuff() | Retorna buff de ataque |
  | GetTotalDefenseBuff() | Retorna buff de defesa |
  | GetTotalSpeedBuff() | Retorna buff de velocidade |
  | GetActiveBuffCount() | Retorna quantidade de buffs ativos |
  | GetDebugInfo() | Retorna info de debug |

  ### ItemCollectable

  | Objetivo | Gerencia itens coletáveis |
  | Utiliza  | BounceHandler, SpriteRenderer, Collider2D |

  | Função | Descrição |
  |---|---|
  | CollectItem(GameObject) | Coleta item |
  | SetItemData(CollectableItemData) | Define dados do item |
  | GetItemData() | Retorna dados do item |
  | ForceCollect() | Força coleta |
  | SetAttractionEnabled(bool) | Ativa atração |
  | ForceActivateAttraction() | Força ativação de atração |
  | RestartActivationDelay() | Reinicia delay de ativação |
  | GetRemainingActivationTime() | Retorna tempo restante |
  | SetColliderEnabled(bool) | Ativa/desativa collider |

  ### PlayerAttributesHandler

  | Objetivo | Gerencia atributos do jogador |
  | Utiliza  | MonoBehaviour |

  | Função | Descrição |
  |---|---|
  | TakeDamage(int, bool) | Aplica dano ao jogador |
  | Heal(int) | Cura jogador |
  | AddSkillPoints(int) | Adiciona pontos de habilidade |
  | SpendSkillPoints(int) | Gasta pontos de habilidade |
  | FullHeal() | Cura total |
  | GetAttributesSummary() | Retorna resumo dos atributos |

  ### PlayerController

  | Objetivo | Controla o jogador (input, física, animação) |
  | Utiliza  | Rigidbody2D, Animator, SpriteRenderer, PlayerAttributesHandler, InputSystem_Actions |

  | Função | Descrição |
  |---|---|
  | DisableMovement() | Desabilita movimento |
  | EnableMovement() | Habilita movimento |

  ### RandomStyle

  | Objetivo | Aplica estilos visuais aleatórios |
  | Utiliza  | SpriteRenderer, Renderer |

  | Função | Descrição |
  |---|---|
  | ApplyRandomStyle() | Aplica estilo aleatório |
  | RestoreOriginalStyle() | Restaura estilo original |
  | ForceRandomize() | Força randomização |
  | SetCustomScale(float) | Define escala customizada |
  | SetCustomColor(Color) | Define cor customizada |
  | RandomizeScaleOnly() | Randomiza apenas escala |
  | RandomizeColorOnly() | Randomiza apenas cor |

  ### RockDestruct

  | Objetivo | Gerencia destruição de rochas |
  | Utiliza  | Animator, DropController |

  | Função | Descrição |
  |---|---|
  | TakeDamage(Vector3?) | Aplica dano à rocha |
  | TakeDamage() | Aplica dano à rocha |
  | DestroyObject() | Destroi rocha |

  ### ScreenEffectsManager

  | Objetivo | Gerencia efeitos visuais de tela |
  | Utiliza  | GameObject |

  | Função | Descrição |
  |---|---|
  | PlayVignetteTransition(...) | Executa transição de vinheta |
  | CloseVignette(...) | Fecha vinheta |
  | OpenVignette(...) | Abre vinheta |
  | StopVignetteTransition() | Para transição |
  | ResetVignette() | Reseta vinheta |
  | IsVignetteTransitionInProgress() | Verifica se há transição |
  | ReinitializePostProcessing() | Re-inicializa pós-processamento |
  | IsVignetteAvailable() | Verifica disponibilidade |
  | SetPendingTeleport(GameObject, Vector3) | Define teleporte pendente |
  | ProcessPendingTeleport() | Processa teleporte pendente |
  | HasPendingTeleport() | Verifica se há teleporte pendente |

  ### SelfDestruct

  | Objetivo | Gerencia autodestruição de objetos |
  | Utiliza  | MonoBehaviour |

  | Função | Descrição |
  |---|---|
  | DestroyObject() | Destroi objeto |
  | DestroyObject(float) | Destroi objeto com delay |

  ### SetupVisualEnvironment

  | Objetivo | Configura ambiente visual de objetos |
  | Utiliza  | SpriteRenderer |

  | Função | Descrição |
  |---|---|
  | ApplyRandomVariations() | Aplica variações visuais |
  | ResetToOriginal() | Reseta para original |
  | ForceNewVariations() | Força novas variações |

  ### SpecialMovementPoint

  | Objetivo | Ponto especial de movimento (herda de InteractivePointHandler) |
  | Utiliza  | Collider2D |

  | Função | Descrição |
  |---|---|
  | GetMovementType() | Retorna tipo de movimento |
  | GetDestinationPoint() | Retorna ponto de destino |
  | GetDestinationPosition() | Retorna posição de destino |
  | GetMovementDuration() | Retorna duração do movimento |
  | IsPlayerInContact() | Verifica contato com jogador |
  | GetPlayerCollider() | Retorna collider do jogador |
  | GetMovementName() | Nome do movimento |
  | GetMovementDescription() | Descrição do movimento |
  | IsValidMovementPoint() | Verifica se é ponto válido |
  | GetDistanceToDestination() | Distância ao destino |
  | GetRequiredSpeed() | Velocidade necessária |
  | SetMovementType(MovementType) | Define tipo de movimento |
  | SetDestinationPoint(Transform) | Define ponto de destino |
  | SetMovementDuration(float) | Define duração do movimento |

  ### WindController

  | Objetivo | Gerencia vento e efeitos em objetos |
  | Utiliza  | SpriteRenderer, Collider2D |

  | Função | Descrição |
  |---|---|
  | SetSpeed(float) | Define velocidade do vento |
  | SetDirection(MovementDirection) | Define direção do vento |
  | Stop() | Para movimento |
  | Resume() | Retoma movimento |
  | ReverseDirection() | Inverte direção |

  ### WindEmulator

  | Objetivo | Emula efeitos de vento |
  | Utiliza  | Animator |

  | Função | Descrição |
  |---|---|
  | TriggerShake() | Dispara shake |
  | StartAutoShake() | Inicia shake automático |
  | StopAutoShake() | Para shake automático |
  | ForceShake() | Força shake |

  ### WindManager

  | Objetivo | Gerencia instâncias de vento no mapa |
  | Utiliza  | GameObject |

  | Função | Descrição |
  |---|---|
  | StartSpawning() | Inicia spawn de vento |
  | StopSpawning() | Para spawn de vento |
  | SpawnWind() | Spawna vento |
  | ClearAllActiveWinds() | Limpa ventos ativos |
  | SetWindSpawnFrequency(float) | Define frequência de spawn |
  | ForceSpawnWind() | Força spawn de vento |

  ---

  > Este mapeamento foi gerado automaticamente a partir dos arquivos nas pastas `Assets/Code` e `Assets/External/AssetStore/SlimeMec/_Scripts`. Para detalhes ou atualização, revise os arquivos fonte diretamente.
