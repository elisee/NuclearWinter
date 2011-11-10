﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using NuclearWinter.Collections;

namespace NuclearWinter.UI
{
    //--------------------------------------------------------------------------
    public class NotebookTab: Widget
    {
        //----------------------------------------------------------------------
        Notebook                mNotebook;
        public object           Tag;

        //----------------------------------------------------------------------
        Label                   mLabel;
        Image                   mIcon;
        Button                  mCloseButton;

        public bool             Active { get { return mNotebook.Tabs[mNotebook.ActiveTabIndex] == this; } }
        bool                    mbIsHovered;
        //bool                    mbIsPressed;

        public bool             IsUnread;

        //----------------------------------------------------------------------
        bool                    mbClosable;
        public bool             Closable
        {
            get { return mbClosable; }
            set { mbClosable = value; UpdateContentSize(); }
        }

        //----------------------------------------------------------------------
        public string           Text
        {
            get
            {
                return mLabel.Text;
            }
            
            set
            {
                mLabel.Text = value;

                UpdatePaddings();
                UpdateContentSize();
            }
        }

        public Texture2D        Icon
        {
            get {
                return mIcon.Texture;
            }

            set
            {
                mIcon.Texture = value;
                UpdatePaddings();
                UpdateContentSize();
            }
        }

        void UpdatePaddings()
        {
            if( mIcon.Texture != null )
            {
                mIcon.Padding = mLabel.Text != "" ? new Box( 10, 0, 10, 10 ) : new Box( 10, 0, 10, 20 );
                mLabel.Padding = mLabel.Text != "" ? new Box( 10, 20, 10, 10 ) : new Box( 10, 20, 10, 0 );
            }
            else
            {
                mLabel.Padding = new Box( 10, 20 );
            }
        }

        public Color TextColor
        {
            get { return mLabel.Color; }
            set { mLabel.Color = value; }
        }

        //----------------------------------------------------------------------
        public Group            PageGroup        { get; private set; }

        //----------------------------------------------------------------------
        public NotebookTab( Notebook _notebook, string _strText, Texture2D _iconTex )
        : base( _notebook.Screen )
        {
            mNotebook       = _notebook;

            mLabel          = new Label( Screen, "", Screen.Style.DefaultTextColor );
            mIcon           = new Image( Screen, _iconTex );

            mCloseButton    = new Button( Screen, new Button.ButtonStyle( 5, null, null, Screen.Style.NotebookTabCloseHover, Screen.Style.NotebookTabCloseDown, null, 0, 0 ), "", Screen.Style.NotebookTabClose, Anchor.Center );
            mCloseButton.Padding = new Box(0);
            mCloseButton.ClickHandler = delegate {
                mNotebook.Tabs.Remove( this );
                
                Screen.Focus( mNotebook );

                if( mNotebook.TabClosedHandler != null )
                {
                    mNotebook.TabClosedHandler( this );
                }
            };

            Text            = _strText;

            PageGroup       = new Group( Screen );
        }

        //----------------------------------------------------------------------
        internal override void UpdateContentSize()
        {
            if( mIcon.Texture != null )
            {
                ContentWidth    = mIcon.ContentWidth + mLabel.ContentWidth + Padding.Left + Padding.Right;
            }
            else
            {
                ContentWidth    = mLabel.ContentWidth + Padding.Left + Padding.Right;
            }

            if( Closable )
            {
                ContentWidth += Screen.Style.NotebookTabClose.Width;
            }

            ContentHeight   = Math.Max( mIcon.ContentHeight, mLabel.ContentHeight ) + Padding.Top + Padding.Bottom;

            base.UpdateContentSize();
        }

        //----------------------------------------------------------------------
        internal override void Update( float _fElapsedTime )
        {
            mCloseButton.Update( _fElapsedTime );
            PageGroup.Update( _fElapsedTime );

            base.Update( _fElapsedTime );
        }

        //----------------------------------------------------------------------
        internal override void DoLayout( Rectangle _rect )
        {
            base.DoLayout( _rect );

            HitBox = _rect;

            Point pCenter = LayoutRect.Center;

            if( mIcon.Texture != null )
            {
                mIcon.DoLayout ( new Rectangle( LayoutRect.X + Padding.Left, pCenter.Y - mIcon.ContentHeight / 2, mIcon.ContentWidth, mIcon.ContentHeight ) );
            }

            mLabel.DoLayout(
                new Rectangle(
                    LayoutRect.X + Padding.Left + ( mIcon.Texture != null ? mIcon.ContentWidth : 0 ), pCenter.Y - mLabel.ContentHeight / 2,
                    mLabel.ContentWidth, mLabel.ContentHeight
                )
            );

            if( Closable )
            {
                mCloseButton.DoLayout( new Rectangle(
                    LayoutRect.Right - 10 - Screen.Style.NotebookTabClose.Width,
                    pCenter.Y - Screen.Style.NotebookTabClose.Height / 2,
                    mCloseButton.ContentWidth, mCloseButton.ContentHeight )
                );
            }
        }

        //----------------------------------------------------------------------
        public override Widget HitTest( Point _point )
        {
            return mCloseButton.HitTest( _point ) ?? base.HitTest( _point );
        }

        internal override void OnMouseDown( Point _hitPoint, int _iButton )
        {
            if( _iButton != 0 ) return;

            Screen.Focus( this );
            //OnActivateDown();
        }

        internal override void OnMouseUp( Point _hitPoint, int _iButton )
        {
            if( _iButton != 0 ) return;

            if( _hitPoint.Y < mNotebook.LayoutRect.Y + mNotebook.TabHeight /* && IsInTab */ )
            {
                if( _hitPoint.X > LayoutRect.X && _hitPoint.X < LayoutRect.Right )
                {
                    OnActivateUp();
                }
            }
            /*else
            {
                ResetPressState();
            }*/
        }

        internal override void OnMouseEnter( Point _hitPoint )
        {
            mbIsHovered = true;
        }

        internal override void OnMouseOut( Point _hitPoint )
        {
            mbIsHovered = false;
        }

        internal override void OnMouseMove( Point _hitPoint )
        {
        }

        internal override void OnPadMove( Direction _direction )
        {
            int iTabIndex = mNotebook.Tabs.IndexOf( this );

            if( _direction == Direction.Left && iTabIndex > 0 )
            {
                NotebookTab tab = mNotebook.Tabs[iTabIndex - 1];
                Screen.Focus( tab );
            }
            else
            if( _direction == Direction.Right && iTabIndex < mNotebook.Tabs.Count - 1 )
            {
                NotebookTab tab = mNotebook.Tabs[iTabIndex  + 1];
                Screen.Focus( tab );
            }
            else
            {
                base.OnPadMove( _direction );
            }
        }

        internal override void OnActivateUp()
        {
            mNotebook.SetActiveTab( this );
        }

        //----------------------------------------------------------------------
        internal override void Draw()
        {
            bool bIsActive = Active;

            Screen.DrawBox( bIsActive ? mNotebook.Style.ActiveTab : mNotebook.Style.Tab, LayoutRect, mNotebook.Style.TabCornerSize, Color.White );

            if( mbIsHovered && ! bIsActive ) // && ! mbIsPressed && mPressedAnim.IsOver )
            {
                if( Screen.IsActive )
                {
                    Screen.DrawBox( Screen.Style.ButtonHover, LayoutRect, mNotebook.Style.TabCornerSize, Color.White );
                }
            }

            if( IsUnread )
            {
                    Screen.DrawBox( mNotebook.Style.UnreadTabMarker, LayoutRect, mNotebook.Style.TabCornerSize, Color.White );
            }

            if( Screen.IsActive && HasFocus )
            {
                Screen.DrawBox( bIsActive ? mNotebook.Style.ActiveTabFocus : mNotebook.Style.TabFocus, LayoutRect, mNotebook.Style.TabCornerSize, Color.White );
            }

            mLabel.Draw();
            mIcon.Draw();

            if( Closable )
            {
                mCloseButton.Draw();
            }
        }

        public void Close()
        {
            mNotebook.Tabs.Remove( this );

            if( HasFocus )
            {
                Screen.Focus( mNotebook );
            }
        }
    }

    //--------------------------------------------------------------------------
    public class Notebook: Widget
    {
        //----------------------------------------------------------------------
        public struct NotebookStyle
        {
            public NotebookStyle( int _iTabCornerSize, Texture2D _tab, Texture2D _tabFocus, Texture2D _activeTab, Texture2D _activeTabFocus, Texture2D _unreadTabMarker )
            {
                TabCornerSize   = _iTabCornerSize;
                Tab             = _tab;
                TabFocus        = _tabFocus;
                ActiveTab       = _activeTab;
                ActiveTabFocus  = _activeTabFocus;
                UnreadTabMarker = _unreadTabMarker;
            }

            public int              TabCornerSize;

            public Texture2D        Tab;
            public Texture2D        TabFocus;
            public Texture2D        ActiveTab;
            public Texture2D        ActiveTabFocus;
            public Texture2D        UnreadTabMarker;
        }

        //----------------------------------------------------------------------
        public NotebookStyle        Style;
        public Action<NotebookTab>  TabClosedHandler;

        Panel                       mPanel;

        public ObservableList<NotebookTab>  Tabs            { get; private set; }
        public int                          ActiveTabIndex  { get; private set; }

        public int                  TabHeight = 50;

        //----------------------------------------------------------------------
        public Notebook( Screen _screen )
        : base( _screen )
        {
            Style = new NotebookStyle(
                Screen.Style.NotebookTabCornerSize,
                Screen.Style.NotebookTab,
                Screen.Style.NotebookTabFocus,
                Screen.Style.NotebookActiveTab,
                Screen.Style.NotebookActiveTabFocus,
                Screen.Style.NotebookUnreadTabMarker
            );

            mPanel = new Panel( Screen, Screen.Style.Panel, Screen.Style.PanelCornerSize );
            Tabs = new ObservableList<NotebookTab>();

            Tabs.ListChanged += delegate {
                ActiveTabIndex = Math.Min( Tabs.Count - 1, ActiveTabIndex );
                Tabs[ActiveTabIndex].IsUnread = false;
            };
        }

        //----------------------------------------------------------------------
        internal override void UpdateContentSize()
        {
        }

        //----------------------------------------------------------------------
        internal override void DoLayout( Rectangle _rect )
        {
            base.DoLayout( _rect );
            HitBox = LayoutRect;

            NotebookTab activeTab = Tabs[ActiveTabIndex];

            Rectangle contentRect = new Rectangle( LayoutRect.X, LayoutRect.Y + ( TabHeight - 10 ), LayoutRect.Width, LayoutRect.Height - ( TabHeight - 10 ) );

            mPanel.DoLayout( contentRect );

            int iTabX = 0;
            foreach( NotebookTab tab in Tabs )
            {
                int iTabWidth = tab.ContentWidth;

                Rectangle tabRect = new Rectangle(
                    LayoutRect.X + 20 + iTabX,
                    LayoutRect.Y,
                    iTabWidth,
                    TabHeight
                    );

                tab.DoLayout( tabRect );

                iTabX += iTabWidth;
            }

            activeTab.PageGroup.DoLayout( contentRect );
        }

        //----------------------------------------------------------------------
        public void SetActiveTab( NotebookTab _tab )
        {
            Debug.Assert( Tabs.Contains( _tab ) );

            ActiveTabIndex = Tabs.IndexOf( _tab );
            Tabs[ActiveTabIndex].IsUnread = false;
        }

        //----------------------------------------------------------------------
        public override Widget HitTest( Point _point )
        {
            if( _point.Y < LayoutRect.Y + TabHeight )
            {
                if( _point.X < LayoutRect.X + 20 ) return null;

                int iTabX = 0;
                int iTab = 0;

                foreach( NotebookTab tab in Tabs )
                {
                    int iTabWidth = tab.ContentWidth;

                    if( _point.X - LayoutRect.X - 20 < iTabX + iTabWidth )
                    {
                        return Tabs[ iTab ].HitTest( _point );
                    }

                    iTabX += iTabWidth;
                    iTab++;
                }

                return null;
            }
            else
            {
                return Tabs[ActiveTabIndex].PageGroup.HitTest( _point );
            }
        }

        internal override bool OnPadButton( Buttons _button, bool _bIsDown )
        {
            return Tabs[ActiveTabIndex].OnPadButton( _button, _bIsDown );
        }

        internal override void Update( float _fElapsedTime )
        {
            Tabs[ActiveTabIndex].Update( _fElapsedTime );
        }

        //----------------------------------------------------------------------
        internal override void Draw()
        {
            mPanel.Draw();

            foreach( NotebookTab tab in Tabs )
            {
                tab.Draw();
            }

            Tabs[ActiveTabIndex].PageGroup.Draw();
        }
    }
}
