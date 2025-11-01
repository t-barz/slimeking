# Dialogues Directory

This directory contains JSON files for the NPC dialogue system.

## JSON File Format

Each dialogue file should follow this structure:

```json
{
  "dialogueId": "unique_dialogue_id",
  "shortDescription": "Brief description of the dialogue",
  "localizations": [
    {
      "language": "EN",
      "pages": [
        { "text": "First page of dialogue in English" },
        { "text": "Second page of dialogue in English" }
      ]
    },
    {
      "language": "BR",
      "pages": [
        { "text": "Primeira página do diálogo em Português" },
        { "text": "Segunda página do diálogo em Português" }
      ]
    }
  ]
}
```

## Supported Languages

- BR - Português Brasil
- EN - English
- ES - Español
- CH - Chinese
- RS - Russian
- FR - Français
- IT - Italiano
- DT - Deutsch
- JP - Japanese
- KR - Korean

## Language Fallback

The system uses the following fallback order:

1. Requested language
2. English (EN)
3. First available language in the file

## File Naming

JSON files should be named with the dialogue ID: `{dialogueId}.json`

Example: `npc_merchant_greeting.json`
