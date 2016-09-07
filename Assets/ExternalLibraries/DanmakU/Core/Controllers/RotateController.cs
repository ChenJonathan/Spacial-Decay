using UnityEngine;
using Vexe.Runtime.Types;

namespace DanmakU.Controllers
{

    public class RotateController : IDanmakuController
    {
        [SerializeField, Show]
        private float rotateSpeed;
        public float RotateSpeed
        {
            get
            {
                return rotateSpeed;
            }
            set
            {
                rotateSpeed = value;
            }
        }

        public RotateController(float rotateSpeed = 0f) : base() {
            this.RotateSpeed = rotateSpeed;
        }

        #region IDanmakuController implementation

        public void Update(Danmaku danmaku, float dt)
        {
            if (rotateSpeed != 0)
            {
                float rotationChange = 360f * RotateSpeed * dt;
                danmaku.rotation += rotationChange;
            }
        }

        #endregion

    }


}