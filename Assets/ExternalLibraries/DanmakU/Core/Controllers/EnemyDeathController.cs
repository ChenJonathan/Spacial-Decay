using UnityEngine;
using Vexe.Runtime.Types;

namespace DanmakU.Controllers
{

    // A Danmaku controller that deactivates bullets if the linked unit dies
    [System.Serializable]
    public class EnemyDeathController : IDanmakuController
    {
        
        private Enemy link;

        public EnemyDeathController(Enemy link = null)
        {
            this.link = link;
        }
        
        #region IDanmakuController implementation

        public void Update(Danmaku danmaku, float dt)
        {
            if (!link)
            {
                danmaku.IsActive = false;
            }
        }

        #endregion

    }

}