using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;

namespace VolumeBox.Toolbox
{
	public class SceneHandler: MonoCached
	{
		[SerializeField] private bool _skipSetup;
		[SerializeField] private bool _isGameplayLevel;

		public bool IsGameplayScene => _isGameplayLevel;

        public virtual void SetupLevel(SceneArgs args)
        {
            
        }
    }

    public class SceneArgs
    {

    }
}
