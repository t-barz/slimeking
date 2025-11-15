using UnityEngine;
using UnityEditor;
using TheSlimeKing.Quest;

namespace TheSlimeKing.Editor.Quest
{
    /// <summary>
    /// Custom Property Drawer para ItemReward
    /// Melhora visualização no Inspector dividindo em duas colunas
    /// </summary>
    [CustomPropertyDrawer(typeof(ItemReward))]
    public class ItemRewardDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Inicia property
            EditorGUI.BeginProperty(position, label, property);
            
            // Desenha label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            // Não indenta campos filhos
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            // Calcula rects para os campos
            // Item: 70% da largura
            // Quantity: 30% da largura
            float itemWidth = position.width * 0.70f;
            float quantityWidth = position.width * 0.30f;
            float spacing = 5f;
            
            Rect itemRect = new Rect(position.x, position.y, itemWidth - spacing, position.height);
            Rect quantityRect = new Rect(position.x + itemWidth, position.y, quantityWidth, position.height);
            
            // Encontra propriedades
            SerializedProperty itemProp = property.FindPropertyRelative("item");
            SerializedProperty quantityProp = property.FindPropertyRelative("quantity");
            
            // Desenha campos sem labels
            EditorGUI.PropertyField(itemRect, itemProp, GUIContent.none);
            EditorGUI.PropertyField(quantityRect, quantityProp, GUIContent.none);
            
            // Restaura indent
            EditorGUI.indentLevel = indent;
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Usa altura padrão de uma linha
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
