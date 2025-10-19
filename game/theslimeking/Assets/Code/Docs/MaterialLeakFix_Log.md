# 🔧 Correção: Material Leak no Editor

## ⚠️ Problema Identificado

**Erro**: `Instantiating material due to calling renderer.material during edit mode. This will leak materials into the scene.`

**Causa**: O Custom Inspector `PuddleReflectionControllerEditor` estava usando `renderer.material` ao invés de `renderer.sharedMaterial` durante o edit mode, causando vazamento de materiais.

## ✅ Correção Aplicada

### Antes (Problemático)

```csharp
// Linha 109 e 113 - PuddleReflectionControllerEditor.cs
else if (spriteRenderer.material == null)
{
    issues.Add("Material não atribuído ao SpriteRenderer");
}
else if (!spriteRenderer.material.shader.name.Contains("PuddleReflection"))
{
    issues.Add("Material não usa shader de reflexo de poça");
}
```

### Depois (Corrigido)

```csharp
// Linha 109 e 113 - PuddleReflectionControllerEditor.cs
else if (spriteRenderer.sharedMaterial == null)
{
    issues.Add("Material não atribuído ao SpriteRenderer");
}
else if (!spriteRenderer.sharedMaterial.shader.name.Contains("PuddleReflection"))
{
    issues.Add("Material não usa shader de reflexo de poça");
}
```

## 📖 Explicação Técnica

### Diferença entre `material` e `sharedMaterial`

- **`renderer.material`**: Cria uma cópia única do material para aquela instância específica
- **`renderer.sharedMaterial`**: Referencia o material original compartilhado

### Por que isso causava vazamento

- No **edit mode**, acessar `renderer.material` cria instâncias desnecessárias de material
- Essas instâncias ficam "órfãs" na memória e são vazadas para a cena
- Durante **runtime** isso é normal, mas no **edit mode** deve-se usar `sharedMaterial`

### Quando usar cada um

- **Edit Mode / Inspector**: Sempre usar `sharedMaterial` para leitura/validação
- **Runtime**: Usar `material` quando quiser modificar propriedades por instância
- **Runtime**: Usar `sharedMaterial` quando quiser apenas ler ou modificar o material base

## 🎯 Resultado

- ❌ **Vazamento de material**: CORRIGIDO
- ✅ **Validação do Inspector**: Mantida funcional
- ✅ **Performance**: Melhorada (sem instanciação desnecessária)
- ✅ **Memory leaks**: Eliminados

---
**Data**: 19/10/2024  
**Arquivo**: `PuddleReflectionControllerEditor.cs`  
**Status**: ✅ Material leak corrigido

---

## ⚠️ Nota Sobre SRP Batcher e _TexelSize /_ST

O aviso:
"Material 'PuddleReflectionMaterial (...)' has _TexelSize /_ST texture properties which are not supported by 2D SRP Batcher".

### Por que aparece?

Essas propriedades (_TexelSize,_MainTex_ST) são injetadas automaticamente pelo Unity para auxiliar cálculos de UV. O SRP Batcher para 2D não consegue agrupar materiais que dependem desses valores dinâmicos.

### Impacto real

- O batching para esses sprites específicos é desativado.
- Como a poça normalmente é um número pequeno de instâncias, o impacto na performance é mínimo/negligenciável.

### Quando se preocupar

Somente se houver centenas de poças simultâneas na tela. Caso contrário, o custo é irrelevante.

### Alternativas (se preciso otimizar)

1. Evitar usar múltiplas instâncias de material customizado (usar `sharedMaterial`).
2. Remover sampling adicional de texturas e usar apenas `_MainTex` se possível.
3. Substituir o reflexo dinâmico por sprite espelhado estático para cenários não interativos.

### Conclusão

É um aviso benigno para este caso e pode ser ignorado com segurança.
