# Guia - Menu Local Save Reset

## Visão Geral

O menu "Local Save Reset" é uma ferramenta de desenvolvimento que permite resetar dados de salvamento locais diretamente no Unity Editor.

## Localização

**Menu: Extras > Local Save Reset**

## Funcionalidades Disponíveis

### 1. Reset All Save Data

- **Descrição**: Deleta TODOS os dados de salvamento
- **Inclui**: Auto-Save, Slots 0-2, Respawn, Portal
- **Uso**: Limpeza completa para testes

### 2. Reset Auto-Save Only  

- **Descrição**: Deleta apenas o auto-save
- **Preserva**: Slots manuais (0, 1, 2)
- **Uso**: Teste específico do sistema de auto-save

### 3. Reset Manual Slots Only

- **Descrição**: Deleta slots manuais (0, 1, 2)
- **Preserva**: Auto-save
- **Uso**: Teste específico dos slots manuais

### 4. Reset Respawn Data

- **Descrição**: Deleta pontos de respawn salvos
- **Uso**: Teste do sistema de respawn

### 5. Reset Portal Data

- **Descrição**: Deleta coordenadas de portal temporárias
- **Uso**: Limpeza de dados de transição entre cenas

### 6. Show Save Info

- **Descrição**: Mostra informações detalhadas sobre saves existentes
- **Inclui**: Data/hora dos saves, posições de respawn/portal
- **Uso**: Debug e verificação de dados

### 7. DANGER - Delete ALL PlayerPrefs

- **Descrição**: ⚠️ Deleta TODOS os PlayerPrefs
- **CUIDADO**: Remove também configurações, preferências, etc.
- **Uso**: Apenas em casos extremos

## Como Usar

### Durante Desenvolvimento

1. **Para testar save/load**: Use "Reset All Save Data"
2. **Para verificar dados**: Use "Show Save Info"
3. **Para testes específicos**: Use os resets individuais

### Antes de Build

1. Verifique dados com "Show Save Info"
2. Limpe dados de teste se necessário

### Resolução de Problemas

1. **Save não funciona**: Use "Show Save Info" para verificar
2. **Dados corrompidos**: Use "Reset All Save Data"
3. **Portal/respawn bugado**: Use resets específicos

## Segurança

### Confirmações

- Todos os comandos pedem confirmação
- Comandos perigosos têm dupla confirmação
- Avisos claros sobre o que será deletado

### Logs

- Todas as ações são logadas no Console
- Feedback visual com caixas de diálogo
- Informações detalhadas sobre o que foi feito

## Exemplo de Uso

```
1. Abra Unity Editor
2. Vá em: Extras > Local Save Reset
3. Escolha a opção desejada:
   
   Para limpeza completa:
   - "Reset All Save Data"
   
   Para verificar dados:
   - "Show Save Info"
   
   Para teste específico:
   - "Reset Auto-Save Only"
```

## Saída do Show Save Info

```
=== INFORMAÇÕES DE SALVAMENTO ===

✅ Auto-Save: EXISTE
   Data: 18/07/2025 14:30:15

✅ Slot 0: EXISTE  
   Data: 18/07/2025 14:25:30

❌ Slot 1: VAZIO

❌ Slot 2: VAZIO

✅ Respawn: (10.50, 5.75, 0.00) na cena 'InitialCave'

❌ Portal: NÃO DEFINIDO
```

## Notas Importantes

### Editor Only

- Este menu só funciona no Unity Editor
- Não aparece em builds do jogo
- Ideal para desenvolvimento e debug

### PlayerPrefs

- Utiliza PlayerPrefs para armazenamento
- Dados persistem entre sessões do Editor
- Localização varia por plataforma de desenvolvimento

### Integração

- Funciona com SavePlayerManager
- Compatível com CheckpointManager
- Respeita as convenções de nomes dos sistemas

## Resolução de Problemas

### Menu não aparece

1. Verifique se o arquivo está em Assets/Code/Editor/
2. Confirme que não há erros de compilação
3. Tente reimportar o script

### Confirmações não funcionam

1. Verifique se está no Unity Editor (não Play Mode)
2. Confirme que não há scripts com erros

### Dados não são deletados

1. Verifique logs no Console
2. Confirme que PlayerPrefs.Save() foi chamado
3. Teste com "Show Save Info" depois da operação
