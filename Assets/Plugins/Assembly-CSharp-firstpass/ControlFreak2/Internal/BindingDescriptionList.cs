using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2.Internal
{
	public class BindingDescriptionList : List<BindingDescription>
	{
		public delegate string NameFormatter(InputBindingBase bind, string name);

		public BindingDescription.BindingType typeMask;

		public bool addUnusedBindings;

		public int axisInputSourceMask;

		public NameFormatter menuNameFormatter;

		public BindingDescriptionList(BindingDescription.BindingType typeMask, bool addUnusedBindings, int axisInputSourceMask, NameFormatter menuNameFormatter)
			: base(16)
		{
			Setup(typeMask, addUnusedBindings, axisInputSourceMask, menuNameFormatter);
		}

		public void Setup(BindingDescription.BindingType typeMask, bool addUnusedBindings, int axisInputSourceMask, NameFormatter menuNameFormatter)
		{
			this.typeMask = typeMask;
			this.addUnusedBindings = addUnusedBindings;
			this.menuNameFormatter = menuNameFormatter;
			this.axisInputSourceMask = axisInputSourceMask;
		}

		public void Add(InputBindingBase binding, string name, string menuPath, Object undoObject)
		{
			BindingDescription.BindingType bindingType = ((binding is AxisBinding) ? BindingDescription.BindingType.Axis : ((binding is DigitalBinding) ? BindingDescription.BindingType.Digital : ((binding is EmuTouchBinding) ? BindingDescription.BindingType.EmuTouch : ((binding is MousePositionBinding) ? BindingDescription.BindingType.MousePos : BindingDescription.BindingType.All))));
			string text = ((menuNameFormatter != null) ? menuNameFormatter(binding, name) : name);
			if ((bindingType & typeMask) == bindingType && bindingType != BindingDescription.BindingType.All)
			{
				Add(new BindingDescription
				{
					type = bindingType,
					name = name,
					nameFormatted = text,
					menuPath = menuPath,
					undoObject = undoObject,
					binding = binding
				});
			}
			menuPath = menuPath + text + "/";
			binding.GetSubBindingDescriptions(this, undoObject, menuPath);
		}

		public void Add(AxisBinding binding, InputRig.InputSource sourceType, string name, string menuPath, Object undoObject)
		{
			if ((typeMask & BindingDescription.BindingType.Axis) != 0 && (axisInputSourceMask & (1 << (int)sourceType)) != 0)
			{
				string nameFormatted = ((menuNameFormatter != null) ? menuNameFormatter(binding, name) : name);
				Add(new BindingDescription
				{
					type = BindingDescription.BindingType.Axis,
					axisSource = sourceType,
					name = name,
					nameFormatted = nameFormatted,
					menuPath = menuPath,
					undoObject = undoObject,
					binding = binding
				});
			}
		}
	}
}
