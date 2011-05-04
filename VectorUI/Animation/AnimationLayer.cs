﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorUI.Animation
{
    // FIXME: See to merge part of this with NuclearWinter.Animation.Timeline
    public class AnimationLayer
    {
        //----------------------------------------------------------------------
        public AnimationLayer( UISheet _uiSheet )
        {
            UISheet             = _uiSheet;
            AnimationBlocks     = new Dictionary<string,AnimationBlock>();
            Time                = 0f;
        }

        //----------------------------------------------------------------------
        public void Play()
        {
            foreach( AnimationBlock block in AnimationBlocks.Values )
            {
                block.ResolveWidgets();
            }

            HasStarted = true;
        }

        //----------------------------------------------------------------------
        public void Update( float _fElapsedTime )
        {
            if( ! HasStarted ) return;
            
            Time += _fElapsedTime;

            IsDone = true;
            foreach( AnimationBlock block in AnimationBlocks.Values )
            {
                if( ! block.Update( Time ) )
                {
                    IsDone = false;
                }
            }
        }

        //----------------------------------------------------------------------
        public bool     IsDone      { get; private set; }
        public bool     HasStarted  { get; private set; }

        public float    Time        { get; private set; }

        //----------------------------------------------------------------------
        public UISheet                                      UISheet;

        public Dictionary<string,AnimationBlock>            AnimationBlocks     { get; private set; }
    }
}
