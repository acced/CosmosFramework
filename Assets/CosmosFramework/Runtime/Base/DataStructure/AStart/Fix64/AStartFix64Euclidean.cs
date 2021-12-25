﻿using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class AStartFix64Euclidean: AStartFix64
    {
        public AStartFix64Euclidean(Fix64 gridCenterX, Fix64 gridCenterY, int xCount, int yCount, Fix64 nodeSideLength) 
            : base(gridCenterX, gridCenterY, xCount, yCount, nodeSideLength){}
        protected override int GetDistance(Node a, Node b)
        {
            return GetEuclideanDistance(a, b);
        }
    }
}
