using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
namespace VolumeBox.Toolbox
{
	public class LevelHandler<TArgs>: MonoCached 
        where TArgs : LevelArgs, new()
    {
        protected TArgs Args { get; private set; }
    }
}
