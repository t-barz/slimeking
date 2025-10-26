# 📋 Plano de Implementação: Teleporte com Efeito de Vinheta

## 🎯 Diretrizes Seguidas

- Scripts e variáveis em inglês.
- Comentários e documentação em português.
- Uso de Controller para entidades e Manager para sistemas globais.
- Evitar dependências diretas entre PlayerController e Managers.
- Utilização de corrotinas para timing.
- Logs controlados por flag.
- Simplicidade e clareza, sem over engineering.

## 🏗️ Componentes Envolvidos

### 1. TeleportPointHandler (Novo Handler)

- Script anexado ao GameObject `teleportPoint`.
- Responsável por:
  - Armazenar destino (`teleportDestination: Vector3`).
  - Detectar colisão com Player (`OnTriggerEnter2D`).
  - Acionar transição visual via Manager.
  - Reposicionar Player e aguardar 1 segundo para reverter o efeito.

### 2. SceneTransitionManager (Manager Existente)

- Responsável por acionar efeitos visuais (vinheta/circle).
- Deve expor métodos para iniciar e reverter o efeito.

### 3. PlayerController (Controller Existente)

- Reposicionamento do Player via método público.
- Sem dependência direta do Manager.

### 4. CinemachineFollow (Componente Existente)

- Câmera segue Player automaticamente após reposicionamento.

## 🔄 Fluxo do Mecanismo

1. Player colide com teleportPoint.
2. `TeleportPointHandler` aciona efeito de vinheta via `SceneTransitionManager`.
3. Ao finalizar o efeito, Player é reposicionado na posição destino.
4. Câmera segue Player automaticamente.
5. Aguarda 1 segundo.
6. Aciona efeito inverso de vinheta.
7. Finaliza teleporte.

## 📝 Estrutura Recomendada

- **TeleportPointHandler**: Handler específico, sem lógica global.
- **SceneTransitionManager**: Manager Singleton, centraliza efeitos visuais.
- **PlayerController**: Controller, expõe método para reposicionamento.
- **Logs**: Controlados por flag interna.
- **Corrotinas**: Para timing e transições.

## ⚠️ Observações

- Collider do teleportPoint deve estar como Trigger.
- Não criar dependência direta do PlayerController no Manager.
- Manter documentação e comentários claros.
- Registrar atividade no Roadmap.md antes de implementar.
