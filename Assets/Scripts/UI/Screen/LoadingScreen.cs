using System.Collections;
using System.Collections.Generic;
using HoakleEngine.Core.Graphics;
using RetroRush.Camera;
using RetroRush.Engine;
using RetroRush.UI.Screen;
using UnityEngine;

namespace RetroRush
{
    public class LoadingScreen : GraphicalUserInterface
    {
        private GraphicsEngineImpl _GraphicsEngine;
        public override void OnReady()
        {
            _GraphicsEngine = _GuiEngine.GetEngine<GraphicsEngineImpl>();
            
            base.OnReady();
            
            _GraphicsEngine.CreateGraphicalRepresentation<MainMenu>("MainMenu", null, menu => Dispose());
        }
    }
}
