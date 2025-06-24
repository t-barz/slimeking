using System.Collections;
using UnityEngine;

namespace TheSlimeKing.Gameplay.Movement
{
    /// <summary>
    /// Classe base abstrata para movimentos especiais do slime (ex: salto, esgueirar)
    /// </summary>
    public abstract class SpecialMovement : MonoBehaviour
    {
        protected Transform _playerTransform;
        protected bool _isMovementInProgress = false;
        protected float _movementDuration = 0.7f;

        protected virtual void Awake()
        {
            // Busca referência ao jogador (pode ser ajustado conforme arquitetura do projeto)
            if (_playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    _playerTransform = player.transform;
            }
        }

        /// <summary>
        /// Inicia o movimento especial
        /// </summary>
        public virtual void StartSpecialMovement()
        {
            if (!_isMovementInProgress)
            {
                _isMovementInProgress = true;
                StartCoroutine(PerformSpecialMovement());
            }
        }

        /// <summary>
        /// Coroutine que executa o movimento especial (deve ser implementada nas subclasses)
        /// </summary>
        protected abstract IEnumerator PerformSpecialMovement();

        /// <summary>
        /// Finaliza o movimento especial
        /// </summary>
        protected virtual void CompleteMovement()
        {
            _isMovementInProgress = false;
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos() { }
#endif
    }
}
