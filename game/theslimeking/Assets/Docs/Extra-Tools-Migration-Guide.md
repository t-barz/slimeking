# 🔄 Guia de Migração - Extra Tools

## 📋 Resumo

As ferramentas de desenvolvimento foram unificadas em um único menu **Extra Tools** para melhor organização e facilidade de uso.

## 🎯 O Que Mudou

### Antes (3 menus separados)

```
QuickWinds/
  └─ NPC Quick Config

Tools/SlimeKing/Camera Setup/
  ├─ Add Camera Manager to Scene
  ├─ Add Scene Validator to Scene
  ├─ Setup Complete Scene
  ├─ Validate Current Scene
  └─ Force Camera Refresh

The Slime King/
  ├─ Project/
  │   ├─ Create Folder Structure
  │   ├─ Reorganize Assets
  │   └─ Complete Setup
  ├─ Post Processing/
  │   ├─ Setup Global Volume
  │   ├─ Setup Forest/Cave/Crystal Volume
  │   └─ Setup Gameplay Effects
  └─ Debug/
      ├─ Toggle Logs
      └─ Export Scene Structure
```

### Agora (1 menu unificado)

```
Extra Tools/
  ├─ 🏠 Open Extra Tools Window
  ├─ NPC/
  │   ├─ 🎭 NPC Quick Config
  │   └─ 📊 NPC Batch Configurator
  ├─ Camera/
  │   ├─ 📷 Add Camera Manager
  │   ├─ ✅ Add Scene Validator
  │   └─ 🎬 Setup Complete Scene
  ├─ Project/
  │   ├─ 📁 Create Folder Structure
  │   ├─ 🔄 Reorganize Assets
  │   └─ ✨ Complete Setup
  ├─ Post Processing/
  │   ├─ 🌐 Setup Global Volume
  │   ├─ 🌲 Setup Forest Volume
  │   ├─ 🏔️ Setup Cave Volume
  │   ├─ 💎 Setup Crystal Volume
  │   └─ ⚡ Setup Gameplay Effects
  └─ Debug/
      ├─ 🔊 Toggle Logs
      └─ 📊 Export Scene Structure
```

## 🚀 Como Migrar

### Opção 1: Usar o Menu Unificado

Acesse diretamente pelo menu do Unity:

```
Extra Tools → [Categoria] → [Ferramenta]
```

### Opção 2: Usar a Janela (Recomendado)

1. Abra a janela: `Extra Tools → 🏠 Open Extra Tools Window`
2. Navegue pelas abas: NPC, Camera, Project, Post Processing, Debug
3. Clique nos botões para executar as ferramentas

## 📊 Tabela de Equivalência

| Menu Antigo | Novo Caminho |
|-------------|--------------|
| `QuickWinds/NPC Quick Config` | `Extra Tools/NPC/🎭 NPC Quick Config` |
| `Tools/.../Add Camera Manager` | `Extra Tools/Camera/📷 Add Camera Manager` |
| `Tools/.../Add Scene Validator` | `Extra Tools/Camera/✅ Add Scene Validator` |
| `Tools/.../Setup Complete Scene` | `Extra Tools/Camera/🎬 Setup Complete Scene` |
| `The Slime King/Project/Create Folder Structure` | `Extra Tools/Project/📁 Create Folder Structure` |
| `The Slime King/Project/Reorganize Assets` | `Extra Tools/Project/🔄 Reorganize Assets` |
| `The Slime King/Project/Complete Setup` | `Extra Tools/Project/✨ Complete Setup` |
| `The Slime King/Post Processing/Setup Global Volume` | `Extra Tools/Post Processing/🌐 Setup Global Volume` |
| `The Slime King/Debug/Toggle Logs` | `Extra Tools/Debug/🔊 Toggle Logs` |
| `The Slime King/Debug/Export Scene Structure` | `Extra Tools/Debug/📊 Export Scene Structure` |

## ⚠️ Menus Legados

Os menus antigos ainda estão disponíveis com o sufixo "(Use Extra Tools)" ou "(Legacy)" para compatibilidade, mas recomenda-se usar o novo menu unificado.

## ✨ Benefícios da Migração

- ✅ **Organização**: Todas as ferramentas em um só lugar
- ✅ **Descoberta**: Mais fácil encontrar ferramentas
- ✅ **Consistência**: Interface unificada
- ✅ **Produtividade**: Menos cliques, mais trabalho
- ✅ **Emojis**: Identificação visual rápida

## 🔧 Para Desenvolvedores

Se você criou scripts que referenciam os menus antigos:

### Antes

```csharp
[MenuItem("QuickWinds/My Tool")]
public static void MyTool() { }
```

### Agora

```csharp
[MenuItem("Extra Tools/Category/🔧 My Tool")]
public static void MyTool() { }
```

## 📝 Próximos Passos

1. ✅ Familiarize-se com o novo menu
2. ✅ Use a janela Extra Tools para acesso rápido
3. ✅ Atualize seus bookmarks/atalhos
4. ⏳ Os menus legados serão removidos em versão futura

## 🆘 Suporte

Se encontrar algum problema ou tiver sugestões:

1. Verifique o arquivo `EXTRA_TOOLS_README.md`
2. Consulte a documentação do projeto
3. Reporte issues no sistema de controle de versão

---

**Data de Migração**: 30/10/2025  
**Versão**: 1.0  
**Status**: ✅ Ativo
