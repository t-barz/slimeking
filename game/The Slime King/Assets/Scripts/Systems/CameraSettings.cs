using System.Collections;
using UnityEngine;

namespace SlimeKing.Systems
{
    /// <summary>
    /// Modos de câmera disponíveis
    /// </summary>
    public enum CameraMode
    {
        Follow,      // Segue um alvo
        Fixed,       // Posição fixa
        Cinematic,   // Modo cinemático
        FreeRoam     // Movimento livre
    }
    
    /// <summary>
    /// Configurações da câmera
    /// </summary>
    [System.Serializable]
    public class CameraSettings
    {
        [Header("Follow Settings")]
        public float followSpeed = 5f;
        public Vector3 defaultOffset = new Vector3(0, 5, -10);
        public bool smoothFollow = true;
        
        [Header("Zoom Settings")]
        public float zoomSpeed = 2f;
        public float minZoom = 3f;
        public float maxZoom = 10f;
        public bool enableZoom = true;
        
        [Header("Bounds")]
        public bool useBounds = false;
        public Bounds cameraBounds = new Bounds(Vector3.zero, new Vector3(20, 20, 20));
        
        [Header("Shake Settings")]
        public float shakeDecay = 5f;
        public float maxShakeIntensity = 1f;
    }
}