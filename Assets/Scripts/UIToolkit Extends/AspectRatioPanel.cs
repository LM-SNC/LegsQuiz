using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace N30R1L37
{
	public enum RatioFlex
	{
		Auto,
		ParentWidth,
		ParentHeight
	}
	
    [UnityEngine.Scripting.Preserve]
	public class AspectRatioPanel : VisualElement, IDisposable
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<AspectRatioPanel, UxmlTraits> {}

		[UnityEngine.Scripting.Preserve]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			readonly UxmlBoolAttributeDescription maintainAspectRatio = new() { name = "maintain-aspect-ratio", defaultValue = true };
			readonly UxmlEnumAttributeDescription<RatioFlex> ratioFlex = new () { name = "ratio-flex-type", defaultValue = RatioFlex.Auto};

			readonly UxmlFloatAttributeDescription aspectRatio = new() { name = "aspect-ratio", defaultValue = 1.0f };
			readonly UxmlFloatAttributeDescription scale = new() { name = "scale", defaultValue = 1.0f };
			readonly UxmlBoolAttributeDescription addLabel = new() { name = "add-label", defaultValue = false };
			readonly UxmlStringAttributeDescription text = new() { name = "text", defaultValue = "Label" };
			readonly UxmlFloatAttributeDescription fontScale = new() { name = "font-scale", defaultValue = 1.0f };
			readonly UxmlFloatAttributeDescription lableGrow = new() { name = "label-grow", defaultValue = 1.0f};
			
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init( VisualElement visualElement, IUxmlAttributes attributes, CreationContext creationContext )
			{
				base.Init( visualElement, attributes, creationContext );
				AspectRatioPanel element = (AspectRatioPanel)visualElement;
				if (element != null)
				{
					element.MaintainAspectRatio = maintainAspectRatio.GetValueFromBag(attributes, creationContext);
					element.RatioFlexType = ratioFlex.GetValueFromBag(attributes, creationContext);
					element.AspectRatio = aspectRatio.GetValueFromBag( attributes, creationContext ) == 0 ? 1 : aspectRatio.GetValueFromBag( attributes, creationContext );
					element.Scale = scale.GetValueFromBag( attributes, creationContext );
					element.Text = text.GetValueFromBag(attributes, creationContext);
					element.FontScale = fontScale.GetValueFromBag( attributes, creationContext );
					element.AddLabel = addLabel.GetValueFromBag(attributes, creationContext);
					element.LabelGrow = lableGrow.GetValueFromBag(attributes, creationContext);
					element.Redraw();
				}
			}
		}
		
		private Label _label;
		public RatioFlex RatioFlexType { get; set; } = RatioFlex.Auto;
		public float FontScale { get; set; } = 1;
		public float Scale { get; set; } = 1.0f;

		private bool _maintainAspectRatio = true;
		public bool MaintainAspectRatio
		{
			get => _maintainAspectRatio;
			set
			{
				_maintainAspectRatio = value;
				if (!value) return;
				style.width = StyleKeyword.Auto;
				style.height = StyleKeyword.Auto;
				style.minWidth = StyleKeyword.Auto;
				style.minHeight = StyleKeyword.Auto;
				style.maxWidth = StyleKeyword.Auto;
				style.maxHeight = StyleKeyword.Auto;
			}
		}
		
		
		private float _labelGrow = 1;
		public float LabelGrow
		{
			get => _labelGrow;
			set
			{
				_labelGrow = value;
				if (_label == null) return;
				_label.style.flexGrow = _labelGrow;
				MarkDirtyRepaint();
			}
		}
		
		private float _aspectRatio = 1.0f;
		public float AspectRatio
		{
			get => _aspectRatio;
			set
			{
				_aspectRatio = Mathf.Max(value, 0.01f);
				MarkDirtyRepaint();
			}
		}
		
		private bool _addLabel = false;
		public bool AddLabel
		{
			get => _addLabel;
			set
			{
				_addLabel = value;
				if(value && _label == null) AddLabelToView();
				else if(!value && _label != null)
				{
					Remove(_label);
					_label = null;
				}
			}
		}

		private string _text = "";
		public string Text
		{
			get => _text;
			set 
			{
				_text = value;
				if (_label != null) _label.text = value;
			}
		}
		
		public AspectRatioPanel()
		{
			if(AddLabel) AddLabelToView();

			RegisterCallback<AttachToPanelEvent>( OnAttachToPanelEvent );
		}

		private void AddLabelToView()
		{
			_label = new()
			{
				style =
				{
					position = Position.Relative,
					left = StyleKeyword.Auto,
					top = StyleKeyword.Auto,
					right = StyleKeyword.Auto,
					bottom = StyleKeyword.Auto,
					flexShrink = 1,
					flexGrow = LabelGrow
				},
				text = _text
			};

			Add(_label);
		}
		
		void OnAttachToPanelEvent( AttachToPanelEvent e )
		{
			parent?.RegisterCallback<GeometryChangedEvent>( OnGeometryChangedEvent );
			Redraw();
		}
		
		void OnGeometryChangedEvent( GeometryChangedEvent e )
		{
			Redraw();
		}
		
		void Redraw()
		{
			if (parent == null) return;

			var newSize = parent.localBound.size;
			
			newSize *= Scale;

			float targetW = newSize.x;
			float targetH = newSize.y;

			if (MaintainAspectRatio)
			{
				switch (RatioFlexType)
				{
					case RatioFlex.Auto:
						targetW = newSize.magnitude / AspectRatio;
						targetH = newSize.magnitude * AspectRatio;
						break;
					case RatioFlex.ParentWidth:
						targetH = targetW / AspectRatio;
						break;
					case RatioFlex.ParentHeight:
						targetW = targetH / AspectRatio;
						break;
				}
			}
			
			newSize = new Vector2(targetW, targetH);
			
			style.width = new StyleLength(newSize.x);
			style.height = new StyleLength(newSize.y);
			
			//SetBorderRadius(newSize.magnitude * 0.001f);

			if(AddLabel) ResizeFont(newSize);
		}
		
		private void ResizeFont(Vector2 v)
		{
			var fontSize = (v.magnitude * 0.01f) * FontScale;
			
			if (float.IsNaN(fontSize) || fontSize == _label.style.fontSize) return;

			_label.style.fontSize = new StyleLength (fontSize);
		}

		// private void SetBorderRadius(float scale)
		// {
		// 	style.borderBottomLeftRadius.Scale(scale);
		// 	style.borderTopLeftRadius.Scale(scale);
		// 	style.borderTopRightRadius.Scale(scale);
		// 	style.borderBottomRightRadius.Scale(scale);
		// }

		public void Dispose()
		{
			parent?.UnregisterCallback<GeometryChangedEvent>( OnGeometryChangedEvent );
			UnregisterCallback<AttachToPanelEvent>( OnAttachToPanelEvent );
		}
	}
}