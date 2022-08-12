using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;

namespace VolumeBox.Toolbox
{
	public class LevelHandler : Singleton<LevelHandler>
	{
		[SerializeField] private bool _skipSetup;
		[SerializeField] private bool _isGameplayLevel;

		public bool IsGameplayLevel {get{return _isGameplayLevel;} private set{}}

        public virtual void SetupLevel()
        {
            
        }
    }
}
