# Quest System Audio Clips

Este diretório deve conter os AudioClips para o Quest System.

## Sons Necessários

### Objective Complete Sounds (3-5 variações)

Sons curtos e positivos para quando um objetivo é completado:

- `quest_objective_complete_01.wav`
- `quest_objective_complete_02.wav`
- `quest_objective_complete_03.wav`

**Características:**

- Duração: 0.5-1 segundo
- Tom: Positivo, leve
- Exemplos: "ding", "chime", "bell"

### Quest Complete Sounds (3-5 variações)

Sons mais elaborados para quando uma quest inteira é completada:

- `quest_complete_01.wav`
- `quest_complete_02.wav`
- `quest_complete_03.wav`

**Características:**

- Duração: 1-2 segundos
- Tom: Triunfante, satisfatório
- Exemplos: "fanfare", "success jingle", "achievement sound"

## Como Adicionar

1. Coloque os arquivos de áudio (.wav, .mp3, .ogg) neste diretório
2. No Unity, selecione os arquivos
3. No Inspector, configure:
   - Load Type: Decompress On Load (para sons curtos)
   - Preload Audio Data: ✓
   - Compression Format: Vorbis (para .ogg) ou PCM (para .wav)
4. Arraste os AudioClips para as listas no QuestNotificationController:
   - `objectiveCompleteSounds` - sons de objetivo completado
   - `questCompleteSounds` - sons de quest completada

## Fontes de Sons Gratuitos

- **Freesound.org** - <https://freesound.org/>
- **OpenGameArt.org** - <https://opengameart.org/>
- **Zapsplat** - <https://www.zapsplat.com/>
- **Mixkit** - <https://mixkit.co/free-sound-effects/>

## Notas

O sistema escolhe aleatoriamente entre os sons disponíveis para evitar repetição e manter a experiência fresca para o jogador.
