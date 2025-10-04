# Post Processing Extra Tools - The Slime King

## 🛠️ **Novo Sistema de Configuração Automática**

O sistema Extra Tools agora inclui ferramentas automáticas para configurar Post Processing nas cenas do projeto.

## 📋 **Funcionalidades Implementadas**

### **Menu Extra Tools > Post Processing**

#### **Setup Volume in Scene**

- ✅ Configura automaticamente um Volume global na cena ativa
- ✅ Aplica o GlobalVolumeProfile.asset com efeitos base
- ✅ Valida se URP está configurado corretamente
- ✅ Verifica se já existe Volume na cena (evita duplicação)
- ✅ Registra operação para Undo/Redo
- ✅ Seleciona automaticamente o GameObject criado

#### **Setup Forest/Cave/Crystal Biome**

- ✅ Cria Volume específico para cada bioma
- ✅ Aplica Volume Profile correspondente do bioma
- ✅ Configura prioridade 1 (maior que global)
- ✅ Adiciona Box Collider como trigger para delimitar área
- ✅ Posiciona na origem para ajuste manual

#### **Setup Gameplay Effects**

- ✅ Cria volumes para Hit Effect e Evolution Effect
- ✅ Configura prioridades diferentes (Hit: 10, Evolution: 15)
- ✅ Inicia desabilitados (weight = 0, GameObject inativo)
- ✅ Preparados para ativação via script

## 🎯 **Como Usar**

### **1. Setup Básico de Post Processing**

```
1. Abra uma cena no Unity
2. Vá no menu: Extra Tools > Post Processing > Setup Volume in Scene
3. Um GameObject "Global Volume" será criado automaticamente
4. O Volume Profile global será aplicado com efeitos base
```

### **2. Configurar Biomas Específicos**

```
1. Para cada área do jogo, use:
   - Extra Tools > Post Processing > Setup Forest Biome
   - Extra Tools > Post Processing > Setup Cave Biome  
   - Extra Tools > Post Processing > Setup Crystal Biome

2. Ajuste o Box Collider de cada Volume para delimitar a área
3. O sistema aplicará automaticamente os efeitos quando o player entrar na área
```

### **3. Configurar Efeitos de Gameplay**

```
1. Use: Extra Tools > Post Processing > Setup Gameplay Effects
2. Isso criará volumes para:
   - Hit Effect Volume (inativo por padrão)
   - Evolution Effect Volume (inativo por padrão)

3. Ative via script quando necessário:
   ```csharp
   // Exemplo para Hit Effect
   GameObject hitVolume = GameObject.Find("Hit Effect Volume");
   if (hitVolume != null)
   {
       hitVolume.SetActive(true);
       hitVolume.GetComponent<Volume>().weight = 1f;
   }
   ```

```

## ⚙️ **Validações Automáticas**

O sistema inclui validações que verificam:

- ✅ **URP Ativo**: Verifica se Universal Render Pipeline está configurado
- ✅ **Volume Profiles**: Confirma se os assets existem na pasta correta
- ✅ **Duplicação**: Evita criar volumes duplicados na mesma cena
- ✅ **Feedback**: Exibe diálogos informativos sobre o processo

## 📁 **Estrutura de Arquivos**

Os Volume Profiles devem estar organizados em:

```

Assets/Settings/PostProcessing/
├── GlobalVolumeProfile.asset          # Volume global (setup básico)
├── Biomes/
│   ├── ForestBiome_Volume.asset
│   ├── CaveBiome_Volume.asset
│   └── CrystalBiome_Volume.asset
└── Gameplay/
    ├── HitEffect_Volume.asset
    └── EvolutionEffect_Volume.asset  

```

## 🔧 **Configurações Técnicas**

### **Prioridades dos Volumes**
- **Global Volume**: Prioridade 0 (base)
- **Biome Volumes**: Prioridade 1 (sobrescreve global)
- **Hit Effect**: Prioridade 10 (efeito temporário)
- **Evolution Effect**: Prioridade 15 (efeito especial)

### **Volume Settings**
- **Global**: `isGlobal = true`, sempre ativo
- **Biomes**: `isGlobal = false`, ativo por área (BoxCollider)
- **Gameplay**: `isGlobal = true`, controlado por script

## 🚀 **Integração com Gameplay**

### **Exemplo: Sistema de Dano**

```csharp
public class PlayerHealth : MonoBehaviour
{
    private Volume hitEffectVolume;
    
    void Start()
    {
        // Encontra o Volume de Hit Effect
        GameObject hitVolumeGO = GameObject.Find("Hit Effect Volume");
        if (hitVolumeGO != null)
            hitEffectVolume = hitVolumeGO.GetComponent<Volume>();
    }
    
    public void TakeDamage(int damage)
    {
        // Ativa efeito visual de dano
        if (hitEffectVolume != null)
        {
            StartCoroutine(HitEffectCoroutine());
        }
    }
    
    private IEnumerator HitEffectCoroutine()
    {
        hitEffectVolume.gameObject.SetActive(true);
        hitEffectVolume.weight = 1f;
        
        yield return new WaitForSeconds(0.3f);
        
        hitEffectVolume.weight = 0f;
        hitEffectVolume.gameObject.SetActive(false);
    }
}
```

### **Exemplo: Sistema de Evolução**

```csharp
public class SlimeEvolution : MonoBehaviour
{
    public void Evolve()
    {
        StartCoroutine(EvolutionEffect());
    }
    
    private IEnumerator EvolutionEffect()
    {
        GameObject evolutionVolume = GameObject.Find("Evolution Effect Volume");
        Volume volume = evolutionVolume?.GetComponent<Volume>();
        
        if (volume != null)
        {
            evolutionVolume.SetActive(true);
            
            // Fade in do efeito
            float timer = 0f;
            while (timer < 1f)
            {
                volume.weight = Mathf.Lerp(0f, 1f, timer);
                timer += Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(2f);
            
            // Fade out do efeito
            timer = 0f;
            while (timer < 1f)
            {
                volume.weight = Mathf.Lerp(1f, 0f, timer);
                timer += Time.deltaTime;
                yield return null;
            }
            
            evolutionVolume.SetActive(false);
        }
    }
}
```

## ✅ **Status da Implementação**

- ✅ **Menu Items**: 5 funções implementadas no Extra Tools
- ✅ **Setup Global**: Configuração automática de Volume global
- ✅ **Setup Biomas**: Configuração automática para 3 biomas
- ✅ **Setup Gameplay**: Configuração automática para efeitos especiais
- ✅ **Validações**: Verificação completa de URP e assets
- ✅ **Feedback**: Diálogos informativos e logs detalhados
- ✅ **Undo Support**: Todas as operações registradas no sistema Undo
- ✅ **Error Handling**: Tratamento robusto de erros e casos edge

## 🎯 **Próximos Passos**

1. **Testar as funções** nas cenas do projeto
2. **Ajustar Volume Profiles** conforme necessário
3. **Integrar com sistemas de gameplay** existentes
4. **Documentar padrões de uso** para a equipe

---

**Nota**: Este sistema complementa a documentação existente em `PostProcessing_Setup.md` fornecendo ferramentas automáticas para acelerar o setup de Post Processing em novas cenas.
