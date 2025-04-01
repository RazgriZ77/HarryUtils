using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace HarryUtils.UI {
    public class InvertedMask : Image {
        #region Public Variables
        public override Material materialForRendering {
            get {
                Material _forRendering = new Material(base.materialForRendering);
                _forRendering.SetInt(StencilComp, (int)CompareFunction.NotEqual);

                return _forRendering;
            }
        }
        #endregion

        #region Private Variables
        private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");
        #endregion
    }
}