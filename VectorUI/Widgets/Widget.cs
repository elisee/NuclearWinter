using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VectorUI.Widgets
{
    public abstract class Widget
    {
        //----------------------------------------------------------------------
        public Widget( string _strName, UISheet _uiSheet )
        {
            Name    = _strName;
            UISheet = _uiSheet;
        }

        //----------------------------------------------------------------------
        public abstract void Update( float _fElapsedTime );
        public abstract void Draw();

        //----------------------------------------------------------------------
        public string       Name        { get; private set; }
        public UISheet      UISheet     { get; private set; }

        //----------------------------------------------------------------------
        public Vector2              Offset  = Vector2.Zero;
        public float                Opacity = 1f;

        //----------------------------------------------------------------------
        public Action<Widget>       Click;
        public Action<Widget,int>   SelectItem;
    }
}
