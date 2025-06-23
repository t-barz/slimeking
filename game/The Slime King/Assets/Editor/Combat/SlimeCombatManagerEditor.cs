using UnityEditor;
using UnityEngine;
using TheSlimeKing.Gameplay.Combat;
using TheSlimeKing.Core.Combat;
using TheSlimeKing.Core.Elemental;

[CustomEditor(typeof(SlimeCombatManager))]
public class SlimeCombatManagerEditor : Editor
{
    private ElementalType _selectedElementalType = ElementalType.None;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SlimeCombatManager manager = (SlimeCombatManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("[Debug/Testing]", EditorStyles.boldLabel);

        // Cooldowns
        EditorGUILayout.LabelField("Cooldowns:");
        EditorGUILayout.LabelField($"Básico: {manager.GetRemainingCooldown(AttackType.Basic):0.00}s");
        EditorGUILayout.LabelField($"Dash: {manager.GetRemainingCooldown(AttackType.Dash):0.00}s");
        EditorGUILayout.LabelField($"Especial: {manager.GetRemainingCooldown(AttackType.Special):0.00}s");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Está atacando? (animação): {IsAttacking(manager)}");

        EditorGUILayout.Space();
        if (GUILayout.Button("Executar Ataque Básico"))
        {
            manager.PerformBasicAttack();
        }
        if (GUILayout.Button("Executar Dash Attack"))
        {
            manager.PerformDashAttack();
        }

        // Ataque especial com seleção de elemento
        _selectedElementalType = (ElementalType)EditorGUILayout.EnumPopup("Elemento Especial", _selectedElementalType);
        if (GUILayout.Button("Executar Ataque Especial"))
        {
            manager.PerformSpecialAttack(_selectedElementalType);
        }
    }

    // Helper para exibir se está atacando (baseado em trigger de animação)
    private bool IsAttacking(SlimeCombatManager manager)
    {
        var animator = manager.GetComponent<Animator>();
        if (animator == null) return false;
        // Considera atacando se qualquer trigger de ataque está ativo
        return animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") ||
               animator.GetCurrentAnimatorStateInfo(0).IsTag("Dash") ||
               animator.GetCurrentAnimatorStateInfo(0).IsTag("Special");
    }
}
