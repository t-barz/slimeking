using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia os estados visuais do jogador, incluindo sprites e VFX.
    /// Responsável por controlar quais sprites estão visíveis e seus estados.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerVisualManager : MonoBehaviour
    {
        #region Campos Privados
        private GameObject[] frontObjects;
        private GameObject[] backObjects;
        private GameObject[] sideObjects;
        private GameObject vfxFront;
        private GameObject vfxBack;
        private GameObject vfxSide; private bool isFacingLeft = false;
        #endregion

        #region Propriedades Públicas
        public bool IsFacingLeft => isFacingLeft;
        #endregion

        #region Métodos Unity
        private void Awake()
        {
            InitializeVisualObjects();
        }
        #endregion

        #region Inicialização
        private void InitializeVisualObjects()
        {
            frontObjects = GetObjectsByNameContains("front");
            backObjects = GetObjectsByNameContains("back");
            sideObjects = GetObjectsByNameContains("side");

            vfxFront = transform.Find("vfx_front")?.gameObject;
            vfxBack = transform.Find("vfx_back")?.gameObject;
            vfxSide = transform.Find("vfx_side")?.gameObject;

            DisableAllVfx();
        }

        public void DisableAllVfx()
        {
            if (vfxFront) vfxFront.SetActive(false);
            if (vfxBack) vfxBack.SetActive(false);
            if (vfxSide) vfxSide.SetActive(false);
        }
        #endregion

        #region Gerenciamento Visual
        public void UpdateVisualState(Vector2 direction)
        {
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);

            if (absX > absY)
            {
                HandleHorizontalMovement(direction.x);
            }
            else
            {
                HandleVerticalMovement(direction.y);
            }
        }

        private void HandleHorizontalMovement(float directionX)
        {
            SetActiveObjects(sideObjects, true);
            SetActiveObjects(frontObjects, false);
            SetActiveObjects(backObjects, false);

            bool shouldFaceLeft = directionX < 0;
            if (shouldFaceLeft != isFacingLeft)
            {
                isFacingLeft = shouldFaceLeft;
                FlipSideObjects(isFacingLeft);
            }
        }

        private void HandleVerticalMovement(float directionY)
        {
            if (directionY > 0)
            {
                SetActiveObjects(backObjects, true);
                SetActiveObjects(frontObjects, false);
                SetActiveObjects(sideObjects, false);
            }
            else
            {
                SetActiveObjects(frontObjects, true);
                SetActiveObjects(backObjects, false);
                SetActiveObjects(sideObjects, false);
            }
        }
        public GameObject GetActiveVfx()
        {
            GameObject activeVfx = null;

            // Log do estado atual para debug
            LogVfxState();

            // Verifica se há objetos na array antes de tentar acessar
            if (frontObjects.Length > 0 && frontObjects[0] != null && frontObjects[0].activeSelf && vfxFront != null)
            {
                activeVfx = vfxFront;
                Debug.Log("Retornando VFX Front");
            }
            else if (backObjects.Length > 0 && backObjects[0] != null && backObjects[0].activeSelf && vfxBack != null)
            {
                activeVfx = vfxBack;
                Debug.Log("Retornando VFX Back");
            }
            else if (sideObjects.Length > 0 && sideObjects[0] != null && sideObjects[0].activeSelf && vfxSide != null)
            {
                activeVfx = vfxSide;
                UpdateSideVfxScale(activeVfx);
                Debug.Log("Retornando VFX Side");
            }

            return activeVfx;
        }

        private void UpdateSideVfxScale(GameObject vfx)
        {
            Vector3 scale = vfx.transform.localScale;
            scale.x = isFacingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            vfx.transform.localScale = scale;
        }
        #endregion

        #region Métodos Auxiliares
        private void SetActiveObjects(GameObject[] objects, bool active)
        {
            foreach (var obj in objects)
            {
                if (obj != null)
                    obj.SetActive(active);
            }
        }

        private void FlipSideObjects(bool faceLeft)
        {
            foreach (var obj in sideObjects)
            {
                if (obj != null)
                {
                    Vector3 scale = obj.transform.localScale;
                    scale.x = faceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                    obj.transform.localScale = scale;
                }
            }
        }

        private GameObject[] GetObjectsByNameContains(string nameContains)
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>();
            int matchCount = 0;

            foreach (Transform child in allChildren)
            {
                if (child != transform && child.name.ToLower().Contains(nameContains))
                    matchCount++;
            }

            GameObject[] matchingObjects = new GameObject[matchCount];
            int index = 0;

            foreach (Transform child in allChildren)
            {
                if (child != transform && child.name.ToLower().Contains(nameContains))
                {
                    matchingObjects[index] = child.gameObject;
                    index++;
                }
            }

            return matchingObjects;
        }
        #endregion

        #region Debug
        private void LogVfxState()
        {
            if (frontObjects.Length > 0)
            {
                Debug.Log($"Front Object Active: {frontObjects[0]?.activeSelf}, VFX Front: {(vfxFront != null ? vfxFront.activeSelf.ToString() : "null")}");
            }
            if (backObjects.Length > 0)
            {
                Debug.Log($"Back Object Active: {backObjects[0]?.activeSelf}, VFX Back: {(vfxBack != null ? vfxBack.activeSelf.ToString() : "null")}");
            }
            if (sideObjects.Length > 0)
            {
                Debug.Log($"Side Object Active: {sideObjects[0]?.activeSelf}, VFX Side: {(vfxSide != null ? vfxSide.activeSelf.ToString() : "null")}");
            }
        }
        #endregion
    }
}
