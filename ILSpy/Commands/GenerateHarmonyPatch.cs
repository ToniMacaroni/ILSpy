using System.Collections.Generic;
using System.Text;
using System.Windows;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy;

[ExportContextMenuEntry(Header = "Generate Harmony", Icon = "images/Copy", Order = 9999)]
public class GenerateHarmonyPatch : IContextMenuEntry
{
	public bool IsVisible(TextViewContext context)
	{
		return GetMemberNodeFromContext(context) != null;
	}

	public bool IsEnabled(TextViewContext context) => true;

	public void Execute(TextViewContext context)
	{
		if (GetMemberNodeFromContext(context)?.Member is not IMethod member)
			return;

		var decl = member.DeclaringType.GetDefinition();
		var declFullName = decl!.FullName;
		var declName = decl!.Name;

		List<string> typeList = new();
		List<string> parList = new();
		List<string> logPars = new();

		foreach (var par in member.Parameters)
		{
			var typeName = par.Type.Name;
			if (typeName == "String")
				typeName = "string";
			else if(typeName == "Single")
				typeName = "float";
			else if(typeName == "Boolean")
				typeName = "bool";
				
			typeList.Add($"typeof({typeName})");
			parList.Add($"{typeName} {par.Name}");
			logPars.Add($"{{{par.Name}}}");
		}
			
		var types = typeList.Count > 0 ? ", " + string.Join(", ", typeList) : "";
		var parTypes = parList.Count > 0 ? ", " + string.Join(", ", parList) : "";
		var logParTypes = logPars.Count > 0 ? string.Join(", ", logPars) : "";

		StringBuilder sb = new();
		sb.AppendLine("[HarmonyPatch(typeof(" + declFullName + "), nameof(" + declFullName + "." + member.Name + ")" + types +")]");
		sb.AppendLine("public static class " + member.Name + "Patch");
		sb.AppendLine("{");
		sb.AppendLine("\tpublic static void Postfix(" + declFullName + " __instance" + parTypes +")");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tRLog.Msg(System.Drawing.Color.Orange, $\"" + declName + "." + member.Name + "(" + logParTypes + ")\");");
		sb.AppendLine("\t}");
		sb.AppendLine("}");
		Clipboard.SetText(sb.ToString());

		// Clipboard.SetText("[HarmonyPatch(typeof(TestsManager), nameof(TestsManager.OnEnable))]" +
		//        "public static class CustomGameStart" +
		//        "{" +
		//        "\tpublic static void Postfix(TestsManager __instance)" +
		//        "\t{" +
		//        "\t\tExtensions.Log(\"TestManager enabled...\");" +
		//        "\t}" +
		//        "}");
	}

	private IMemberTreeNode GetMemberNodeFromContext(TextViewContext context)
	{
		return context.SelectedTreeNodes?.Length == 1 ? context.SelectedTreeNodes[0] as IMemberTreeNode : null;
	}
}