using System;
using UnityEngine;

namespace Kogane
{
    /// <summary>
    /// マウスもしくはタップの入力を検知するコンポーネント
    /// </summary>
    public sealed class SingleTouchEvent : MonoBehaviour
    {
        //==============================================================================
        // 列挙型
        //==============================================================================
        private enum State
        {
            NONE,    // タッチされていない
            STARTED, // タッチされた
            MOVED,   // タッチされている
            ENDED,   // タッチされ終わった
        }

        //================================================================================
        // デリゲート
        //================================================================================
        public delegate void TouchEvent( in Vector2 screenPoint );

        //==============================================================================
        // イベント
        //==============================================================================
        /// <summary>
        /// タッチされた時に呼び出されます
        /// </summary>
        public event TouchEvent OnStarted;

        /// <summary>
        /// タッチされている時に呼び出されます
        /// </summary>
        public event TouchEvent OnMoved;

        /// <summary>
        /// タッチされ終わった時に呼び出されます
        /// </summary>
        public event TouchEvent OnEnded;

        //================================================================================
        // 関数
        //================================================================================
        /// <summary>
        /// 破棄される時に呼び出されます
        /// </summary>
        private void OnDestroy()
        {
            OnStarted = null;
            OnMoved   = null;
            OnEnded   = null;
        }

        /// <summary>
        /// 毎フレーム呼び出されます
        /// </summary>
        private void Update()
        {
            var state = GetState();

            if ( state == State.NONE ) return;

            var touchEvent = state switch
            {
                State.STARTED => OnStarted,
                State.MOVED   => OnMoved,
                State.ENDED   => OnEnded,
                _             => throw new ArgumentOutOfRangeException()
            };

            var screenPoint = Application.isEditor
                    ? ( Vector2 )Input.mousePosition
                    : Input.GetTouch( 0 ).position
                ;

            touchEvent?.Invoke( screenPoint );
        }

        /// <summary>
        /// 現在の状態を返します
        /// </summary>
        private static State GetState()
        {
            if ( Application.isEditor )
            {
                if ( Input.GetMouseButtonDown( 0 ) ) return State.STARTED;
                if ( Input.GetMouseButton( 0 ) ) return State.MOVED;
                if ( Input.GetMouseButtonUp( 0 ) ) return State.ENDED;
                return State.NONE;
            }

            if ( Input.touchCount <= 0 ) return State.NONE;

            return Input.GetTouch( 0 ).phase switch
            {
                TouchPhase.Began      => State.STARTED,
                TouchPhase.Moved      => State.MOVED,
                TouchPhase.Stationary => State.MOVED,
                TouchPhase.Canceled   => State.ENDED,
                TouchPhase.Ended      => State.ENDED,
                _                     => throw new ArgumentOutOfRangeException()
            };
        }
    }
}