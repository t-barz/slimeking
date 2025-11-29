# 🛠️ MCP Tools Reference - The Slime King

Referência completa de todas as ferramentas disponíveis através dos servidores MCP (Model Context Protocol) configurados no projeto.

**Última Atualização**: 28/11/2025  
**Versão**: 1.0

---

## 📋 Índice

1. [Unity MCP](#unity-mcp) - Ferramentas específicas do Unity
2. [Context7](#context7) - Documentação de bibliotecas
3. [Sequential Thinking](#sequential-thinking) - Raciocínio estruturado
4. [Memory](#memory) - Grafo de conhecimento
5. [Git](#git) - Controle de versão
6. [Filesystem](#filesystem) - Sistema de arquivos

---

## 🎮 Unity MCP

**Servidor**: `unity-mcp-server.exe`  
**Status**: ✅ Ativo  
**Propósito**: Ferramentas específicas para desenvolvimento Unity

### Ferramentas Disponíveis

*Nota: Este servidor é específico do Unity e fornece ferramentas para manipulação de cenas, assets e configurações do projeto.*

**Uso Típico**:
- Manipular cenas Unity
- Gerenciar assets
- Configurar projeto Unity
- Executar comandos específicos do Unity

---

## 📚 Context7

**Servidor**: `@upstash/context7-mcp`  
**Status**: ✅ Ativo  
**Propósito**: Buscar documentação atualizada de bibliotecas e frameworks

### Ferramentas Disponíveis

#### 1. `resolve_library_id`

**Descrição**: Resolve o nome de uma biblioteca para um ID compatível com Context7.

**Quando Usar**:
- Antes de buscar documentação de uma biblioteca
- Para encontrar o ID correto de um pacote/framework
- Quando não souber o formato exato do ID

**Parâmetros**:
- `libraryName` (string, obrigatório): Nome da biblioteca a buscar

**Exemplo de Uso**:
```
Preciso da documentação do Unity Input System
→ Tool resolve_library_id com libraryName="Unity Input System"
→ Retorna: /unity/input-system
```

**Processo de Seleção**:
1. Analisa similaridade de nome
2. Verifica relevância da descrição
3. Prioriza cobertura de documentação
4. Considera reputação da fonte
5. Avalia Benchmark Score (qualidade)


#### 2. `get_library_docs`

**Descrição**: Busca documentação atualizada de uma biblioteca específica.

**Quando Usar**:
- Para consultar API references
- Para ver exemplos de código
- Para entender conceitos e arquitetura
- Para resolver dúvidas sobre implementação

**Parâmetros**:
- `context7CompatibleLibraryID` (string, obrigatório): ID da biblioteca (obtido via resolve_library_id)
- `mode` (string, opcional): Tipo de documentação
  - `"code"` (padrão): API references e exemplos de código
  - `"info"`: Guias conceituais e arquitetura
- `topic` (string, opcional): Tópico específico a focar
- `page` (number, opcional): Número da página (1-10, padrão: 1)

**Exemplos de Uso**:

```
# Buscar API do Unity Input System
→ get_library_docs(
    context7CompatibleLibraryID="/unity/input-system",
    mode="code",
    topic="PlayerInput"
  )

# Buscar guia conceitual
→ get_library_docs(
    context7CompatibleLibraryID="/unity/input-system",
    mode="info",
    topic="getting started"
  )

# Próxima página se contexto insuficiente
→ get_library_docs(
    context7CompatibleLibraryID="/unity/input-system",
    mode="code",
    topic="PlayerInput",
    page=2
  )
```

**Dicas**:
- Use `mode="code"` para implementação
- Use `mode="info"` para entender conceitos
- Se contexto insuficiente, incremente `page`
- Seja específico no `topic` para melhores resultados

---

## 🧠 Sequential Thinking

**Servidor**: `@modelcontextprotocol/server-sequential-thinking`  
**Status**: ✅ Ativo (auto-aprovado)  
**Propósito**: Raciocínio estruturado e resolução de problemas complexos

### Ferramentas Disponíveis

#### 1. `sequentialthinking`

**Descrição**: Ferramenta para análise e resolução de problemas através de pensamento sequencial estruturado.

**Quando Usar**:
- Problemas complexos que requerem múltiplos passos
- Análise que pode precisar de correção de curso
- Situações onde o escopo não está claro inicialmente
- Tarefas que precisam manter contexto entre etapas
- Filtrar informações irrelevantes

**Parâmetros**:
- `thought` (string, obrigatório): Passo atual de raciocínio
- `nextThoughtNeeded` (boolean, obrigatório): Se precisa de mais pensamento
- `thoughtNumber` (integer, obrigatório): Número do pensamento atual
- `totalThoughts` (integer, obrigatório): Estimativa total de pensamentos
- `isRevision` (boolean, opcional): Se revisa pensamento anterior
- `revisesThought` (integer, opcional): Qual pensamento está sendo revisado
- `branchFromThought` (integer, opcional): Ponto de ramificação
- `branchId` (string, opcional): Identificador do branch
- `needsMoreThoughts` (boolean, opcional): Se precisa de mais pensamentos

**Características**:
- Pode ajustar `totalThoughts` durante o processo
- Pode questionar ou revisar pensamentos anteriores
- Pode adicionar mais pensamentos mesmo após "fim"
- Pode expressar incerteza e explorar alternativas
- Gera hipóteses e as verifica
- Repete até satisfeito com a solução

**Exemplo de Fluxo**:
```
Thought 1/5: Analisando o problema...
Thought 2/5: Identificando dependências...
Thought 3/7: (ajustou total) Percebo que preciso considerar X...
Thought 4/7: (revisão) Revisando pensamento 2, na verdade...
Thought 5/7: Gerando hipótese de solução...
Thought 6/7: Verificando hipótese...
Thought 7/7: Solução validada, resposta final.
```

---

## 🧠 Memory

**Servidor**: `@modelcontextprotocol/server-memory`  
**Status**: ✅ Ativo  
**Propósito**: Grafo de conhecimento persistente para armazenar informações

### Ferramentas Disponíveis

#### 1. `create_entities`

**Descrição**: Cria múltiplas entidades no grafo de conhecimento.

**Quando Usar**:
- Armazenar informações sobre pessoas, lugares, conceitos
- Criar registros de decisões de design
- Documentar padrões de código
- Registrar preferências do usuário

**Parâmetros**:
- `entities` (array, obrigatório): Lista de entidades
  - `name` (string): Nome da entidade
  - `entityType` (string): Tipo da entidade
  - `observations` (array): Lista de observações

**Exemplo**:
```json
{
  "entities": [
    {
      "name": "PlayerController",
      "entityType": "UnityComponent",
      "observations": [
        "Controla movimento do jogador",
        "Usa Unity Input System",
        "Implementa mecânica de agachar"
      ]
    }
  ]
}
```

#### 2. `create_relations`

**Descrição**: Cria relações entre entidades no grafo.

**Parâmetros**:
- `relations` (array, obrigatório): Lista de relações
  - `from` (string): Entidade de origem
  - `to` (string): Entidade de destino
  - `relationType` (string): Tipo de relação (voz ativa)

**Exemplo**:
```json
{
  "relations": [
    {
      "from": "PlayerController",
      "to": "InputSystem",
      "relationType": "uses"
    }
  ]
}
```

#### 3. `add_observations`

**Descrição**: Adiciona observações a entidades existentes.

**Parâmetros**:
- `observations` (array, obrigatório):
  - `entityName` (string): Nome da entidade
  - `contents` (array): Lista de observações

#### 4. `delete_entities`

**Descrição**: Remove entidades e suas relações.

**Parâmetros**:
- `entityNames` (array, obrigatório): Lista de nomes de entidades

#### 5. `delete_observations`

**Descrição**: Remove observações específicas de entidades.

**Parâmetros**:
- `deletions` (array, obrigatório):
  - `entityName` (string): Nome da entidade
  - `observations` (array): Observações a remover

#### 6. `delete_relations`

**Descrição**: Remove relações específicas do grafo.

**Parâmetros**:
- `relations` (array, obrigatório): Lista de relações a remover

#### 7. `read_graph`

**Descrição**: Lê o grafo de conhecimento completo.

**Quando Usar**:
- Ver todas as entidades e relações
- Fazer backup do conhecimento
- Analisar estrutura do grafo

#### 8. `search_nodes`

**Descrição**: Busca nós no grafo baseado em query.

**Parâmetros**:
- `query` (string, obrigatório): Texto de busca

**Quando Usar**:
- Encontrar entidades específicas
- Buscar por observações
- Filtrar por tipo de entidade

#### 9. `open_nodes`

**Descrição**: Abre nós específicos por nome.

**Parâmetros**:
- `names` (array, obrigatório): Lista de nomes de entidades

---

## 🔀 Git

**Servidor**: `@cyanheads/git-mcp-server`  
**Status**: ✅ Ativo  
**Propósito**: Controle de versão completo via Git

### Ferramentas Disponíveis


#### Git - Operações Básicas

##### 1. `git_status`

**Descrição**: Mostra o status do working tree (arquivos staged, unstaged, untracked).

**Parâmetros**:
- `path` (string, opcional): Caminho do repositório (padrão: diretório de trabalho)
- `includeUntracked` (boolean, opcional): Incluir arquivos não rastreados (padrão: true)

**Quando Usar**:
- Verificar mudanças antes de commit
- Ver arquivos modificados
- Identificar arquivos não rastreados

##### 2. `git_add`

**Descrição**: Adiciona arquivos ao staging area.

**Parâmetros**:
- `files` (array, obrigatório): Lista de arquivos (use ["."] para todos)
- `path` (string, opcional): Caminho do repositório
- `force` (boolean, opcional): Adicionar arquivos ignorados
- `update` (boolean, opcional): Apenas arquivos modificados/deletados
- `all` (boolean, opcional): Incluir todos os arquivos

**Exemplo**:
```
# Adicionar arquivo específico
git_add(files=["Assets/Code/NewScript.cs"])

# Adicionar todos os arquivos
git_add(files=["."])

# Adicionar apenas modificados
git_add(files=["."], update=true)
```

##### 3. `git_commit`

**Descrição**: Cria um commit com as mudanças staged.

**Parâmetros**:
- `message` (string, obrigatório): Mensagem do commit
- `path` (string, opcional): Caminho do repositório
- `amend` (boolean, opcional): Emendar commit anterior
- `allowEmpty` (boolean, opcional): Permitir commit vazio
- `noVerify` (boolean, opcional): Pular hooks
- `sign` (boolean, opcional): Assinar com GPG
- `author` (object, opcional): Sobrescrever autor
  - `name` (string): Nome do autor
  - `email` (string): Email do autor
- `filesToStage` (array, opcional): Arquivos para stage+commit atômico

**Exemplo**:
```
# Commit simples
git_commit(message="feat: adiciona sistema de habilidades")

# Commit multi-linha
git_commit(message="feat: sistema de habilidades\n\nImplementa:\n- AbilityManager\n- 4 habilidades Tier 1")

# Stage + commit atômico
git_commit(
  message="fix: corrige bug no PlayerController",
  filesToStage=["Assets/Code/PlayerController.cs"]
)
```

##### 4. `git_diff`

**Descrição**: Mostra diferenças entre commits, branches ou working tree.

**Parâmetros**:
- `path` (string, opcional): Caminho do repositório
- `source` (string, opcional): Commit/branch de origem
- `target` (string, opcional): Commit/branch de destino
- `staged` (boolean, opcional): Diff de mudanças staged
- `paths` (array, opcional): Limitar a arquivos específicos
- `stat` (boolean, opcional): Mostrar resumo ao invés de diff completo
- `nameOnly` (boolean, opcional): Apenas nomes de arquivos
- `contextLines` (number, opcional): Linhas de contexto (padrão: 3)
- `includeUntracked` (boolean, opcional): Incluir arquivos não rastreados

**Exemplo**:
```
# Diff de mudanças não staged
git_diff()

# Diff de mudanças staged
git_diff(staged=true)

# Diff entre branches
git_diff(source="main", target="feature/abilities")

# Diff de arquivo específico
git_diff(paths=["Assets/Code/PlayerController.cs"])

# Resumo de mudanças
git_diff(stat=true)
```

##### 5. `git_log`

**Descrição**: Mostra histórico de commits.

**Parâmetros**:
- `path` (string, opcional): Caminho do repositório
- `branch` (string, opcional): Branch específico
- `maxCount` (number, opcional): Número máximo de commits (1-1000)
- `skip` (number, opcional): Pular N commits (paginação)
- `oneline` (boolean, opcional): Formato resumido
- `patch` (boolean, opcional): Incluir diff de cada commit
- `stat` (boolean, opcional): Incluir estatísticas de mudanças
- `author` (string, opcional): Filtrar por autor
- `since` (string, opcional): Data inicial (ISO 8601)
- `until` (string, opcional): Data final (ISO 8601)
- `grep` (string, opcional): Filtrar por mensagem (regex)
- `filePath` (string, opcional): Commits que afetaram arquivo
- `showSignature` (boolean, opcional): Mostrar assinatura GPG

**Exemplo**:
```
# Últimos 10 commits
git_log(maxCount=10)

# Commits de hoje
git_log(since="2025-11-28")

# Commits de autor específico
git_log(author="Thiago")

# Commits que afetaram arquivo
git_log(filePath="Assets/Code/PlayerController.cs")

# Buscar por mensagem
git_log(grep="fix.*bug")
```

#### Git - Branches

##### 6. `git_branch`

**Descrição**: Gerencia branches (listar, criar, deletar, renomear).

**Parâmetros**:
- `operation` (string, opcional): Operação (list, create, delete, rename, show-current)
- `name` (string, opcional): Nome do branch
- `newName` (string, opcional): Novo nome (para rename)
- `startPoint` (string, opcional): Commit inicial (para create)
- `force` (boolean, opcional): Forçar operação
- `all` (boolean, opcional): Mostrar branches remotos também
- `remote` (boolean, opcional): Apenas branches remotos
- `merged` (boolean/string, opcional): Apenas branches merged
- `noMerged` (boolean/string, opcional): Apenas branches não merged

**Exemplo**:
```
# Listar branches
git_branch(operation="list")

# Branch atual
git_branch(operation="show-current")

# Criar branch
git_branch(operation="create", name="feature/abilities")

# Deletar branch
git_branch(operation="delete", name="old-feature")

# Renomear branch
git_branch(operation="rename", name="old-name", newName="new-name")
```

##### 7. `git_checkout`

**Descrição**: Troca de branch ou restaura arquivos.

**Parâmetros**:
- `target` (string, obrigatório): Branch/commit/tag
- `createBranch` (boolean, opcional): Criar novo branch
- `force` (boolean, opcional): Forçar checkout
- `track` (boolean, opcional): Configurar tracking com remote
- `paths` (array, opcional): Restaurar arquivos específicos

**Exemplo**:
```
# Trocar de branch
git_checkout(target="main")

# Criar e trocar para novo branch
git_checkout(target="feature/new", createBranch=true)

# Restaurar arquivo
git_checkout(target="HEAD", paths=["Assets/Code/Script.cs"])
```

##### 8. `git_merge`

**Descrição**: Merge de branches.

**Parâmetros**:
- `branch` (string, obrigatório): Branch a fazer merge
- `message` (string, opcional): Mensagem de merge customizada
- `noFastForward` (boolean, opcional): Criar merge commit
- `squash` (boolean, opcional): Squash todos os commits
- `strategy` (string, opcional): Estratégia de merge (ort, recursive, octopus, ours, subtree)
- `abort` (boolean, opcional): Abortar merge em progresso

**Exemplo**:
```
# Merge simples
git_merge(branch="feature/abilities")

# Merge sem fast-forward
git_merge(branch="feature/abilities", noFastForward=true)

# Merge com squash
git_merge(branch="feature/abilities", squash=true)

# Abortar merge
git_merge(abort=true)
```

##### 9. `git_rebase`

**Descrição**: Reaplica commits em cima de outro base.

**Parâmetros**:
- `mode` (string, opcional): Modo (start, continue, abort, skip)
- `upstream` (string, opcional): Branch upstream (obrigatório para start)
- `branch` (string, opcional): Branch a rebase
- `onto` (string, opcional): Rebase em commit diferente
- `interactive` (boolean, opcional): Rebase interativo
- `preserve` (boolean, opcional): Preservar merge commits

**Exemplo**:
```
# Rebase em main
git_rebase(mode="start", upstream="main")

# Continuar após resolver conflitos
git_rebase(mode="continue")

# Abortar rebase
git_rebase(mode="abort")
```

#### Git - Remotes

##### 10. `git_remote`

**Descrição**: Gerencia repositórios remotos.

**Parâmetros**:
- `mode` (string, opcional): Operação (list, add, remove, rename, get-url, set-url)
- `name` (string, opcional): Nome do remote
- `newName` (string, opcional): Novo nome (para rename)
- `url` (string, opcional): URL do remote
- `push` (boolean, opcional): Configurar push URL separadamente

**Exemplo**:
```
# Listar remotes
git_remote(mode="list")

# Adicionar remote
git_remote(mode="add", name="origin", url="https://github.com/user/repo.git")

# Remover remote
git_remote(mode="remove", name="old-origin")

# Obter URL
git_remote(mode="get-url", name="origin")
```

##### 11. `git_fetch`

**Descrição**: Baixa objetos e refs de remote.

**Parâmetros**:
- `remote` (string, opcional): Nome do remote (padrão: origin)
- `prune` (boolean, opcional): Remover refs que não existem mais
- `tags` (boolean, opcional): Fetch de todas as tags
- `depth` (number, opcional): Shallow clone com N commits

**Exemplo**:
```
# Fetch do origin
git_fetch()

# Fetch com prune
git_fetch(prune=true)

# Fetch de todas as tags
git_fetch(tags=true)
```

##### 12. `git_pull`

**Descrição**: Fetch + integra mudanças no branch atual.

**Parâmetros**:
- `remote` (string, opcional): Nome do remote (padrão: origin)
- `branch` (string, opcional): Branch (padrão: atual)
- `rebase` (boolean, opcional): Usar rebase ao invés de merge
- `fastForwardOnly` (boolean, opcional): Falhar se não puder fast-forward

**Exemplo**:
```
# Pull simples
git_pull()

# Pull com rebase
git_pull(rebase=true)

# Pull de branch específico
git_pull(remote="origin", branch="main")
```

##### 13. `git_push`

**Descrição**: Envia commits para remote.

**Parâmetros**:
- `remote` (string, opcional): Nome do remote (padrão: origin)
- `branch` (string, opcional): Branch (padrão: atual)
- `remoteBranch` (string, opcional): Branch remoto diferente
- `force` (boolean, opcional): Force push (sobrescreve histórico)
- `forceWithLease` (boolean, opcional): Force push seguro
- `setUpstream` (boolean, opcional): Configurar tracking
- `tags` (boolean, opcional): Push de todas as tags
- `delete` (boolean, opcional): Deletar branch remoto
- `dryRun` (boolean, opcional): Simular sem executar

**Exemplo**:
```
# Push simples
git_push()

# Push com set upstream
git_push(setUpstream=true)

# Push de tags
git_push(tags=true)

# Force push seguro
git_push(forceWithLease=true)

# Deletar branch remoto
git_push(branch="old-feature", delete=true)
```

#### Git - Outras Operações

##### 14. `git_stash`

**Descrição**: Salva mudanças temporariamente.

**Parâmetros**:
- `mode` (string, opcional): Operação (list, push, pop, apply, drop, clear)
- `message` (string, opcional): Mensagem do stash
- `includeUntracked` (boolean, opcional): Incluir arquivos não rastreados
- `keepIndex` (boolean, opcional): Não reverter mudanças staged
- `stashRef` (string, opcional): Referência do stash (ex: stash@{0})

**Exemplo**:
```
# Salvar mudanças
git_stash(mode="push", message="WIP: feature")

# Listar stashes
git_stash(mode="list")

# Aplicar último stash
git_stash(mode="pop")

# Aplicar stash específico
git_stash(mode="apply", stashRef="stash@{1}")

# Limpar todos os stashes
git_stash(mode="clear")
```

##### 15. `git_tag`

**Descrição**: Gerencia tags.

**Parâmetros**:
- `mode` (string, opcional): Operação (list, create, delete)
- `tagName` (string, opcional): Nome da tag
- `commit` (string, opcional): Commit para tag (padrão: HEAD)
- `message` (string, opcional): Mensagem (cria tag anotada)
- `annotated` (boolean, opcional): Criar tag anotada
- `force` (boolean, opcional): Sobrescrever tag existente

**Exemplo**:
```
# Listar tags
git_tag(mode="list")

# Criar tag
git_tag(mode="create", tagName="v1.0.0")

# Criar tag anotada
git_tag(mode="create", tagName="v1.0.0", message="Release 1.0.0", annotated=true)

# Deletar tag
git_tag(mode="delete", tagName="old-tag")
```

##### 16. `git_reset`

**Descrição**: Reseta HEAD para estado específico.

**Parâmetros**:
- `target` (string, opcional): Commit alvo (padrão: HEAD)
- `mode` (string, opcional): Modo (soft, mixed, hard, merge, keep)
- `paths` (array, opcional): Resetar arquivos específicos

**Exemplo**:
```
# Unstage tudo (mixed)
git_reset()

# Soft reset (mantém mudanças staged)
git_reset(target="HEAD~1", mode="soft")

# Hard reset (descarta tudo)
git_reset(target="HEAD~1", mode="hard")

# Resetar arquivo específico
git_reset(paths=["Assets/Code/Script.cs"])
```

##### 17. `git_show`

**Descrição**: Mostra detalhes de um objeto Git.

**Parâmetros**:
- `object` (string, obrigatório): Commit/tree/blob/tag
- `filePath` (string, opcional): Ver arquivo específico no commit
- `stat` (boolean, opcional): Mostrar diffstat
- `format` (string, opcional): Formato de saída (raw, json)

**Exemplo**:
```
# Ver último commit
git_show(object="HEAD")

# Ver arquivo em commit específico
git_show(object="abc123", filePath="Assets/Code/Script.cs")

# Ver commit com estatísticas
git_show(object="HEAD", stat=true)
```

##### 18. `git_cherry_pick`

**Descrição**: Aplica commits de outros branches.

**Parâmetros**:
- `commits` (array, obrigatório): Lista de commits
- `noCommit` (boolean, opcional): Apenas stage mudanças
- `mainline` (number, opcional): Parent para merge commits
- `strategy` (string, opcional): Estratégia de merge
- `signoff` (boolean, opcional): Adicionar Signed-off-by
- `continueOperation` (boolean, opcional): Continuar após resolver conflitos
- `abort` (boolean, opcional): Abortar cherry-pick

**Exemplo**:
```
# Cherry-pick commit
git_cherry_pick(commits=["abc123"])

# Cherry-pick múltiplos
git_cherry_pick(commits=["abc123", "def456"])

# Continuar após resolver conflitos
git_cherry_pick(continueOperation=true)
```

##### 19. `git_reflog`

**Descrição**: Mostra log de referências (útil para recuperar commits perdidos).

**Parâmetros**:
- `ref` (string, opcional): Referência específica (padrão: HEAD)
- `maxCount` (number, opcional): Número máximo de entradas

**Exemplo**:
```
# Ver reflog do HEAD
git_reflog()

# Ver reflog de branch
git_reflog(ref="main")

# Limitar entradas
git_reflog(maxCount=20)
```

##### 20. `git_clean`

**Descrição**: Remove arquivos não rastreados.

**Parâmetros**:
- `force` (boolean, obrigatório): Confirmação de segurança
- `directories` (boolean, opcional): Remover diretórios também
- `ignored` (boolean, opcional): Remover arquivos ignorados
- `dryRun` (boolean, opcional): Simular sem executar

**Exemplo**:
```
# Ver o que seria removido
git_clean(force=true, dryRun=true)

# Remover arquivos não rastreados
git_clean(force=true)

# Remover arquivos e diretórios
git_clean(force=true, directories=true)
```

##### 21. `git_blame`

**Descrição**: Mostra quem modificou cada linha de um arquivo.

**Parâmetros**:
- `file` (string, obrigatório): Caminho do arquivo
- `startLine` (number, opcional): Linha inicial
- `endLine` (number, opcional): Linha final
- `ignoreWhitespace` (boolean, opcional): Ignorar mudanças de espaço

**Exemplo**:
```
# Blame de arquivo completo
git_blame(file="Assets/Code/PlayerController.cs")

# Blame de linhas específicas
git_blame(file="Assets/Code/PlayerController.cs", startLine=10, endLine=50)
```

##### 22. `git_worktree`

**Descrição**: Gerencia múltiplas working trees.

**Parâmetros**:
- `mode` (string, opcional): Operação (list, add, remove, move, prune)
- `worktreePath` (string, opcional): Caminho da worktree
- `branch` (string, opcional): Branch para checkout
- `commitish` (string, opcional): Commit base
- `detach` (boolean, opcional): Criar com HEAD detached
- `force` (boolean, opcional): Forçar operação
- `newPath` (string, opcional): Novo caminho (para move)
- `dryRun` (boolean, opcional): Simular prune
- `verbose` (boolean, opcional): Saída detalhada

**Exemplo**:
```
# Listar worktrees
git_worktree(mode="list")

# Adicionar worktree
git_worktree(mode="add", worktreePath="../feature-work", branch="feature/new")

# Remover worktree
git_worktree(mode="remove", worktreePath="../feature-work")
```

#### Git - Configuração e Utilidades

##### 23. `git_init`

**Descrição**: Inicializa novo repositório Git.

**Parâmetros**:
- `path` (string, opcional): Caminho do repositório
- `bare` (boolean, opcional): Criar repositório bare
- `initialBranch` (string, opcional): Nome do branch inicial

**Exemplo**:
```
# Inicializar repositório
git_init()

# Inicializar com branch main
git_init(initialBranch="main")
```

##### 24. `git_clone`

**Descrição**: Clona repositório remoto.

**Parâmetros**:
- `url` (string, obrigatório): URL do repositório
- `localPath` (string, obrigatório): Caminho local
- `branch` (string, opcional): Branch específico
- `depth` (number, opcional): Shallow clone
- `bare` (boolean, opcional): Clone bare
- `mirror` (boolean, opcional): Clone mirror

**Exemplo**:
```
# Clone simples
git_clone(url="https://github.com/user/repo.git", localPath="./repo")

# Clone de branch específico
git_clone(url="https://github.com/user/repo.git", localPath="./repo", branch="develop")

# Shallow clone
git_clone(url="https://github.com/user/repo.git", localPath="./repo", depth=1)
```

##### 25. `git_set_working_dir`

**Descrição**: Define diretório de trabalho padrão para comandos Git.

**Parâmetros**:
- `path` (string, obrigatório): Caminho absoluto do repositório
- `validateGitRepo` (boolean, opcional): Validar se é repositório Git
- `initializeIfNotPresent` (boolean, opcional): Inicializar se não for repositório
- `includeMetadata` (boolean, opcional): Incluir metadados na resposta

**Exemplo**:
```
# Definir working directory
git_set_working_dir(path="G:/GameDev/slimeking/game/theslimeking")

# Com metadados
git_set_working_dir(path="G:/GameDev/slimeking/game/theslimeking", includeMetadata=true)
```

##### 26. `git_clear_working_dir`

**Descrição**: Limpa configuração de diretório de trabalho.

**Parâmetros**:
- `confirm` (string, obrigatório): Confirmação ("Y", "y", "Yes", "yes")

**Exemplo**:
```
git_clear_working_dir(confirm="yes")
```

##### 27. `git_wrapup_instructions`

**Descrição**: Fornece instruções de workflow para finalizar trabalho.

**Parâmetros**:
- `acknowledgement` (string, obrigatório): Confirmação ("Y", "y", "Yes", "yes")
- `createTag` (boolean, opcional): Criar tag após commit
- `updateAgentMetaFiles` (string, opcional): Atualizar meta files

**Exemplo**:
```
git_wrapup_instructions(acknowledgement="yes")
```

---

## 📁 Filesystem

**Servidor**: `@modelcontextprotocol/server-filesystem`  
**Status**: ✅ Ativo  
**Diretório Permitido**: `G:\GameDev\slimeking\game\theslimeking`  
**Propósito**: Operações de sistema de arquivos

### Ferramentas Disponíveis


#### Filesystem - Leitura

##### 1. `read_text_file`

**Descrição**: Lê conteúdo completo de arquivo de texto.

**Parâmetros**:
- `path` (string, obrigatório): Caminho do arquivo
- `head` (number, opcional): Ler apenas