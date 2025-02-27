﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "Prefabs/Tiles", order = 1)]
    public class Tiles : ScriptableObject
    {
        public GameObject Collider;
        public RuleTile Green;
        public RuleTile Brown;
    }
}