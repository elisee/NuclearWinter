﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NuclearWinter.UI
{
    /*
     * An EditBox to enter some text
     */
    public class EditBox: Widget
    {
        SpriteFont mFont;
        public SpriteFont       Font
        {
            get { return mFont; }
            
            set
            {
                mFont = value;
                UpdateContentSize();
            }
        }

        string  mstrText;
        Point   mpTextPosition;

        int     miCaretOffset;
        int     CaretOffset {
            get { return miCaretOffset; }

            set {
                miCaretOffset = (int)MathHelper.Clamp( value, 0, mstrText.Length );
                miCaretX = miCaretOffset > 0 ? (int)mFont.MeasureString( mstrDisplayedText.Substring( 0, miCaretOffset ) ).X : 0;
                mfTimer = 0f;
            }
        }
        int     miCaretX;

        bool    mbIsHovered;
        float   mfTimer;

        bool    mbHideContent;

        public bool HideContent {
            get { return mbHideContent; }
            set {
                mbHideContent = value;
                UpdateContentSize();
            }
        }

        public string Text
        {
            get { return mstrText; }
            set
            {
                mstrText = value;
                CaretOffset = Math.Min( miCaretOffset, mstrText.Length );

                UpdateContentSize();
            }
        }

        string                  mstrDisplayedText;

        public Action<EditBox>  ValidateHandler;

        //----------------------------------------------------------------------
        public EditBox( Screen _screen, string _strText )
        : base( _screen )
        {
            mstrText    = _strText;
            mFont       = _screen.Style.MediumFont;
            mPadding    = new Box( 15 );

            UpdateContentSize();
        }

        //----------------------------------------------------------------------
        public EditBox( Screen _screen )
        : this( _screen, "" )
        {
        }

        //----------------------------------------------------------------------
        protected override void UpdateContentSize()
        {
            mstrDisplayedText = (!mbHideContent) ? Text : "".PadLeft( Text.Length, '●' );

            ContentWidth = (int)Font.MeasureString( mstrDisplayedText ).X + Padding.Left + Padding.Right;
            ContentHeight = (int)( Font.LineSpacing * 0.9f ) + Padding.Top + Padding.Bottom;
        }

        //----------------------------------------------------------------------
        public override void DoLayout( Rectangle _rect )
        {
            Position = _rect.Location;
            Size = new Point( _rect.Width, _rect.Height );

            HitBox = new Rectangle( Position.X, Position.Y, Size.X, Size.Y );

            Point pCenter = new Point( Position.X + Size.X / 2, Position.Y + Size.Y / 2 );

            mpTextPosition = new Point(
                Position.X + Padding.Left,
                pCenter.Y - ContentHeight / 2 + Padding.Top
            );
        }

        //----------------------------------------------------------------------
        public override void OnMouseEnter( Point _hitPoint )
        {
            base.OnMouseEnter( _hitPoint );
            mbIsHovered = true;
        }

        public override void OnMouseOut( Point _hitPoint )
        {
            base.OnMouseOut( _hitPoint );
            mbIsHovered = false;
        }

        //----------------------------------------------------------------------
        public override void OnMouseDown( Point _hitPoint )
        {
            base.OnMouseDown( _hitPoint );

            Screen.Focus( this );
            OnActivateDown();
        }

        public override void OnMouseUp( Point _hitPoint )
        {
            if( HitTest( _hitPoint ) == this )
            {
                OnActivateUp();
            }
            else
            {
            }
        }

        //----------------------------------------------------------------------
        public override void OnTextEntered( char _char )
        {
            if( ! char.IsControl( _char ) )
            {
                Text = Text.Insert( CaretOffset, _char.ToString() );
                CaretOffset++;
            }
        }

        public override void OnKeyPress( Keys _key )
        {
            switch( _key )
            {
                case Keys.Enter:
                    if( ValidateHandler != null ) ValidateHandler( this );
                    break;
                case Keys.Back:
                    if( Text.Length > 0 && CaretOffset > 0 )
                    {
                        CaretOffset--;
                        Text = Text.Remove( CaretOffset, 1 );
                    }
                    break;
                case Keys.Delete:
                    if( Text.Length > 0 && CaretOffset < Text.Length )
                    {
                        Text = Text.Remove( CaretOffset, 1 );
                    }
                    break;
                case Keys.Left:
                    CaretOffset--;
                    break;
                case Keys.Right:
                    CaretOffset++;
                    break;
                case Keys.End:
                    CaretOffset = Text.Length;
                    break;
                case Keys.Home:
                    CaretOffset = 0;
                    break;
            }
        }

        //----------------------------------------------------------------------
        public override void OnFocus()
        {
            Screen.AddWidgetToUpdateList( this );
        }

        public override bool Update( float _fElapsedTime )
        {
            if( ! HasFocus )
            {
                mfTimer = 0f;
                return false;
            }

            mfTimer += _fElapsedTime;

            return true;
        }

        //----------------------------------------------------------------------
        public override void Draw()
        {
            Screen.DrawBox( Screen.Style.EditBoxFrame, new Rectangle( Position.X, Position.Y, Size.X, Size.Y ), Screen.Style.EditBoxCornerSize, Color.White );

            if( Screen.IsActive && mbIsHovered )
            {
                Screen.DrawBox( Screen.Style.ButtonFramePressed, new Rectangle( Position.X, Position.Y, Size.X, Size.Y ), Screen.Style.EditBoxCornerSize, Color.White );
            }

            Screen.Game.DrawBlurredText( Screen.Style.BlurRadius, mFont, mstrDisplayedText, new Vector2( mpTextPosition.X, mpTextPosition.Y ), Color.White );

            const float fBlinkInterval = 0.3f;

            if( Screen.IsActive && HasFocus && mfTimer % (fBlinkInterval * 2) < fBlinkInterval )
            {
                Screen.Game.SpriteBatch.Draw( Screen.Game.WhitePixelTex, new Rectangle( mpTextPosition.X + miCaretX, Position.Y + Padding.Top, 3, Size.Y - Padding.Vertical ), Color.White );
            }
        }
    }
}
