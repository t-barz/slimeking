using UnityEngine;

namespace SlimeKing.Systems.Core
{
    /// <summary>
    /// Sistema de cache estático para referências do Player.
    /// Evita chamadas caras de Find() em múltiplos scripts.
    /// 
    /// PERFORMANCE: Substitui GameObject.Find() que é O(n) por acesso O(1)
    /// </summary>
    public static class PlayerCache
    {
        private static Transform s_playerTransform;
        private static GameObject s_playerGameObject;
        private static bool s_initialized = false;

        /// <summary>
        /// Obtém o Transform do Player (cached)
        /// </summary>
        public static Transform PlayerTransform
        {
            get
            {
                if (!s_initialized || s_playerTransform == null)
                {
                    Initialize();
                }
                return s_playerTransform;
            }
        }

        /// <summary>
        /// Obtém o GameObject do Player (cached)
        /// </summary>
        public static GameObject PlayerGameObject
        {
            get
            {
                if (!s_initialized || s_playerGameObject == null)
                {
                    Initialize();
                }
                return s_playerGameObject;
            }
        }

        /// <summary>
        /// Verifica se o Player existe na cena
        /// </summary>
        public static bool HasPlayer => PlayerTransform != null;

        /// <summary>
        /// Inicializa o cache buscando o Player na cena
        /// </summary>
        private static void Initialize()
        {
            s_playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (s_playerGameObject != null)
            {
                s_playerTransform = s_playerGameObject.transform;
            }
            s_initialized = true;
        }

        /// <summary>
        /// Força reinicialização do cache (útil após mudança de cena)
        /// </summary>
        public static void Refresh()
        {
            s_initialized = false;
            Initialize();
        }

        /// <summary>
        /// Limpa o cache
        /// </summary>
        public static void Clear()
        {
            s_playerTransform = null;
            s_playerGameObject = null;
            s_initialized = false;
        }
    }
}
