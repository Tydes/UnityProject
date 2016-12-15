// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
    public sealed class PaletteWindow : PaletteParent
    {
        public PaletteWindow(List<ContextMenuItem> items) : base(items,0, 0, 250, 0, string.Empty, MenuAnchor.TOP_RIGHT, MenuAutoSize.MATCH_VERTICAL)
        {
			SortElements();
			m_initialSeparatorAmount = 4;
			SetMinimizedArea( -225, 0, 260, 0 );
		}
		public override void Draw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId )
		{
			if ( m_isMaximized )
			{
				base.Draw( parentPosition, mousePosition, mouseButtonId );
			}
			else
			{
				InitDraw( parentPosition, mousePosition, mouseButtonId );
			}
			PostDraw();
		}
	}
}
