using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta para validar se há menus duplicados no sistema Extra Tools
    /// Ajuda a evitar warnings de "menu item with the same name already exists"
    /// </summary>
    public class MenuDuplicateValidator
    {
        [MenuItem("Extra Tools/Debug/🔍 Validate Menu Duplicates")]
        public static void ValidateMenuDuplicates()
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var menuMethods = new System.Collections.Generic.List<(string path, string method, string type)>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes().Where(t => t.IsClass);

                    foreach (var type in types)
                    {
                        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                        foreach (var method in methods)
                        {
                            var menuItemAttributes = method.GetCustomAttributes(typeof(MenuItem), false);

                            foreach (MenuItem menuItem in menuItemAttributes)
                            {
                                if (menuItem.menuItem.StartsWith("Extra Tools/"))
                                {
                                    menuMethods.Add((menuItem.menuItem, method.Name, type.FullName));
                                }
                            }
                        }
                    }
                }
                catch (System.Exception)
                {
                    // Ignora assemblies que não podem ser carregados
                }
            }

            // Agrupa por caminho de menu para encontrar duplicatas
            var duplicates = menuMethods.GroupBy(m => m.path)
                                       .Where(g => g.Count() > 1)
                                       .ToList();

            if (duplicates.Any())
            {
                string report = "<color=red>⚠️ MENUS DUPLICADOS ENCONTRADOS:</color>\n\n";

                foreach (var duplicate in duplicates)
                {
                    report += $"<b>{duplicate.Key}</b>\n";
                    foreach (var item in duplicate)
                    {
                        report += $"  • {item.type}.{item.method}()\n";
                    }
                    report += "\n";
                }

                report += "\n<color=yellow>AÇÃO NECESSÁRIA:</color>\n";
                report += "Remova os menus duplicados deixando apenas um ativo por caminho.";

                Debug.LogError(report);
                EditorUtility.DisplayDialog("Menus Duplicados Encontrados",
                    $"Foram encontrados {duplicates.Count()} menus duplicados. Verifique o Console para detalhes.", "OK");
            }
            else
            {
                string report = "<color=green>✓ VALIDAÇÃO CONCLUÍDA COM SUCESSO</color>\n\n";
                report += $"Foram verificados {menuMethods.Count} itens de menu do Extra Tools.\n";
                report += "Nenhuma duplicata foi encontrada.\n\n";
                report += "<color=cyan>Menus ativos:</color>\n";

                var sortedMenus = menuMethods.OrderBy(m => m.path).ToList();
                foreach (var menu in sortedMenus)
                {
                    report += $"  • {menu.path}\n";
                }

                Debug.Log(report);
                EditorUtility.DisplayDialog("Validação Concluída",
                    $"✓ Nenhum menu duplicado encontrado!\n\n{menuMethods.Count} menus validados com sucesso.", "OK");
            }
        }

        [MenuItem("Extra Tools/Debug/📋 List All Extra Tools Menus")]
        public static void ListAllExtraToolsMenus()
        {
            ValidateMenuDuplicates();
        }
    }
}