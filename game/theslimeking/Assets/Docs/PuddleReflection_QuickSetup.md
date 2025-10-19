# 🌊 Configuração Rápida - Sistema de Reflexo de Poça

## ⚡ Setup em 5 Passos

### 1️⃣ **Criar o GameObject da Poça**

```
GameObject > Create Empty
Nome: "WaterPuddle"
```

### 2️⃣ **Adicionar Componentes Básicos**

- **SpriteRenderer**: Para exibir a textura da poça
- **Collider2D** (Box/Circle): Para detectar objetos
  - ✅ **Marcar como Trigger** (isTrigger = true)
  - Ajustar tamanho para cobrir a área da poça

### 3️⃣ **Adicionar Componentes do Sistema**

- **PuddleReflectionTrigger**: Detecta objetos
- **PuddleReflectionController**: Gerencia o reflexo

### 4️⃣ **Configurar Material**

- Criar Material usando shader **"SlimeKing/2D/PuddleReflection"**
- Aplicar ao SpriteRenderer
- Definir textura da poça no parâmetro **"Puddle Sprite"**

### 5️⃣ **Configurar Tags dos Objetos**

- Garantir que Slime/Player tem tag **"Player"**
- Inimigos devem ter tag **"Enemy"**
- Adicionar outras tags conforme necessário

---

## 🎛️ Configurações Recomendadas

### 📱 **Mobile/Performance**

```
Texture Size: 256
Update Interval: 0.05s (20fps)
Reflection Strength: 0.5
Distortion Amount: 0.01
```

### 🖥️ **Desktop/Qualidade**

```
Texture Size: 512
Update Interval: 0.033s (30fps)
Reflection Strength: 0.6
Distortion Amount: 0.015
```

### 🌟 **High-End/Visual**

```
Texture Size: 1024
Update Interval: 0.025s (40fps)
Reflection Strength: 0.7
Distortion Amount: 0.02
```

---

## 🎨 Parâmetros Visuais

| Parâmetro | Função | Valores | Efeito |
|-----------|--------|---------|--------|
| **Reflection Strength** | Intensidade do reflexo | 0.4-0.6 (dia)<br/>0.7-0.9 (noite) | Mais alto = reflexo mais visível |
| **Distortion Amount** | Ondas na água | 0.005-0.02 | Mais alto = água mais agitada |
| **Fade Start** | Onde o reflexo desaparece | 0.2-0.4 | Mais baixo = fade mais cedo |
| **Darken Factor** | Escurecimento | 0.2-0.4 | Mais alto = reflexo mais escuro |

---

## 🔧 Ajustes por Cenário

### 🌅 **Poça no Dia**

```
Reflection Strength: 0.4-0.5
Darken Factor: 0.3
Distortion Amount: 0.01 (água mais calma)
```

### 🌙 **Poça à Noite**

```
Reflection Strength: 0.7-0.8
Darken Factor: 0.15
Distortion Amount: 0.015
```

### 🌊 **Poça Agitada**

```
Distortion Amount: 0.025-0.04
Wave Frequency: 15-20 (no material)
Speed: 2-3 (no material)
```

### 💧 **Poça Calma**

```
Distortion Amount: 0.005-0.01
Wave Frequency: 8-12 (no material)
Speed: 0.8-1.2 (no material)
```

---

## 🐛 Solução Rápida de Problemas

### ❌ **Reflexo não aparece**

1. Material usa shader correto? ✅
2. Objetos têm tags corretas? ✅
3. Collider2D é trigger? ✅
4. Enable Logs = true para debug ✅

### 🐌 **Performance baixa**

1. Reduzir Texture Size para 256 ✅
2. Aumentar Update Interval para 0.05s ✅
3. Verificar quantos objetos estão sendo refletidos ✅

### 🌀 **Reflexo distorcido demais**

1. Reduzir Distortion Amount ✅
2. Ajustar Wave Frequency no material ✅
3. Verificar tamanho da câmera ortográfica ✅

### 🔇 **Sem detecção de objetos**

1. Collider2D configurado como trigger? ✅
2. Tags dos objetos corretas? ✅
3. Enable Gizmos = true para visualizar ✅

---

## 📋 Checklist Final

- [ ] GameObject tem SpriteRenderer + Collider2D (trigger)
- [ ] Componentes PuddleReflectionTrigger + PuddleReflectionController adicionados
- [ ] Material com shader SlimeKing/2D/PuddleReflection aplicado
- [ ] Tags dos objetos configuradas (Player, Enemy, etc.)
- [ ] Parâmetros visuais ajustados conforme cenário
- [ ] Testado com objeto entrando/saindo da poça
- [ ] Performance adequada para plataforma alvo

✅ **Sistema pronto para uso!**
