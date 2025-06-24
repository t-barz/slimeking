using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

namespace TheSlimeKing.Core.UI.Icons
{
    /// <summary>
    /// Detecta e notifica mudanças no dispositivo de entrada que está sendo usado pelo jogador.
    /// </summary>
    public class DeviceDetector : MonoBehaviour
    {
        /// <summary>
        /// Evento disparado quando o dispositivo de entrada é alterado
        /// </summary>
        [Serializable]
        public class DeviceChangedEvent : UnityEvent<DeviceType> { }

        [SerializeField] private float _checkInterval = 0.1f;

        public DeviceChangedEvent OnDeviceChanged = new DeviceChangedEvent();
        private DeviceType _currentDevice = DeviceType.Keyboard;
        private float _lastCheckTime;

        private void Start()
        {
            // Inicializa com uma verificação
            CheckDeviceType();
        }

        private void Update()
        {
            // Verifica o dispositivo em intervalos regulares para não sobrecarregar
            if (Time.time - _lastCheckTime > _checkInterval)
            {
                _lastCheckTime = Time.time;
                CheckDeviceType();
            }
        }

        /// <summary>
        /// Verifica e atualiza o dispositivo de entrada atual
        /// </summary>
        private void CheckDeviceType()
        {
            DeviceType detectedDevice = DetectCurrentDevice();

            if (detectedDevice != _currentDevice)
            {
                _currentDevice = detectedDevice;
                OnDeviceChanged.Invoke(_currentDevice);
                Debug.Log($"Dispositivo alterado para: {_currentDevice}");
            }
        }

        /// <summary>
        /// Detecta qual tipo de dispositivo está sendo usado no momento
        /// </summary>
        /// <returns>O tipo de dispositivo detectado</returns>
        private DeviceType DetectCurrentDevice()
        {
            // Verifica se há um controle de gamepad ativo
            if (Gamepad.current != null)
            {
                // Verifica o tipo específico de gamepad                // Verifica o tipo específico de gamepad
                if (Gamepad.current is XInputController)
                {
                    return DeviceType.Xbox;
                }
                else if (Gamepad.current is DualShockGamepad)
                {
                    return DeviceType.PlayStation;
                }
                else if (Gamepad.current.description.interfaceName.Contains("Switch"))
                {
                    return DeviceType.Switch;
                }
                else
                {
                    return DeviceType.Generic;
                }
            }
            // Se houver atividade no teclado ou mouse, assume que é o dispositivo atual
            if (Keyboard.current != null && (Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed))
            {
                return DeviceType.Keyboard;
            }

            // Mantém o dispositivo atual se não houver mudança detectada
            return _currentDevice;
        }

        /// <summary>
        /// Retorna o dispositivo atual detectado
        /// </summary>
        public DeviceType GetCurrentDevice()
        {
            return _currentDevice;
        }
    }
}
