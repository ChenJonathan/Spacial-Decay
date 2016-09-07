// Copyright (c) 2015 James Liu
//	
// See the LISCENSE file for copying permission.

using UnityEngine;
using Vexe.Runtime.Types;

namespace DanmakU.Controllers
{

    public class CthuluController : IDanmakuController
    {

        [SerializeField, Show]
        public float RotateSpeed
        {
            get;
            set;
        }

        [SerializeField, Show]
        public Vector2 Target
        {
            get;
            set;
        }

        #region IDanmakuController implementation

        public void Update(Danmaku danmaku, float dt)
        {
            danmaku.AngularSpeed = 0f;

            danmaku.Rotate(RotateSpeed);
            danmaku.MoveTowards(Target, Time.deltaTime);
        }

        #endregion

    }


}