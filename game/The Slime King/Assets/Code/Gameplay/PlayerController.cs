using UnityEngine;
using System;

namespace TheSlimeKing.Gameplay
{
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        private Vector2 _direction = Vector2.right;
        private float _scale = 1f;

        public void SetScale(float scale)
        {
            _scale = scale;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        public void SetDirection(Vector2 direction)
        {
            if (direction != Vector2.zero)
                _direction = direction.normalized;
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public Vector2 GetDirection()
        {
            return _direction;
        }

        // Métodos obrigatórios da interface (stubs, caso precise expandir)
        public void DisableControl() { /* Implementação opcional */ }
        public void EnableControl() { /* Implementação opcional */ }
        public void MoveToPosition(Vector2 position, bool immediate = false)
        {
            if (immediate)
                transform.position = position;
            else
                transform.position = Vector2.Lerp(transform.position, position, 0.5f);
        }

        public void SetControlEnabled(bool enabled)
        {
            if (enabled)
                EnableControl();
            else
                DisableControl();
        }
    }
}