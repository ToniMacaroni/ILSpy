using System.Collections.Generic;
using System.Text;
using System.Windows;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy;

[ExportContextMenuEntry(Header = "Generate Method Getter", Icon = "images/Copy", Order = 9999)]
public class GenerateMethodGetter : IContextMenuEntry
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
		var methodName = member.Name;
		string methodAccess = member.Accessibility switch
		{
			Decompiler.TypeSystem.Accessibility.Public => "BindingFlags.Public",
			Decompiler.TypeSystem.Accessibility.Private => "BindingFlags.NonPublic",
			Decompiler.TypeSystem.Accessibility.Protected => "BindingFlags.NonPublic",
			Decompiler.TypeSystem.Accessibility.Internal => "BindingFlags.NonPublic",
			_ => "BindingFlags.NonPublic"
		};
		
		string isStatic = " | " + (member.IsStatic ? "BindingFlags.Static" : "BindingFlags.Instance");
		
		Clipboard.SetText($"typeof({declFullName}).GetMethod(\"{methodName}\", {methodAccess}{isStatic})");
	}

	private IMemberTreeNode GetMemberNodeFromContext(TextViewContext context)
	{
		return context.SelectedTreeNodes?.Length == 1 ? context.SelectedTreeNodes[0] as IMemberTreeNode : null;
	}
}