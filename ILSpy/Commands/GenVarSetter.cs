using System.Collections.Generic;
using System.Text;
using System.Windows;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy;

[ExportContextMenuEntry(Header = "Generate Var Setter", Icon = "images/Copy", Order = 9999)]
public class GenVarSetter : IContextMenuEntry
{
	public bool IsVisible(TextViewContext context)
	{
		return true;
	}

	public bool IsEnabled(TextViewContext context) => true;

	public void Execute(TextViewContext context)
	{
		var writer = new StringBuilder();

		foreach (var node in context.SelectedTreeNodes)
		{
			if (node is IMemberTreeNode memberNode)
			{
				var member = memberNode.Member;
				if (member is IField field)
				{
					writer.AppendLine($"$var1$.{field.Name} = $var2$.{field.Name};");
				}

				if (member is IProperty { CanSet: true } prop)
				{
					writer.AppendLine($"$var1$.{prop.Name} = $var2$.{prop.Name};");
				}
			}
		}
		
		Clipboard.SetText(writer.ToString());
	}
}