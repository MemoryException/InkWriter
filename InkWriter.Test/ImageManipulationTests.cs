using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using InkWriter.Algorithms;

namespace InkWriter.Test
{
    [TestClass]
    public class ImageManipulationTests
    {
        [TestMethod]
        public void TestVectorCut()
        {
            Vector vector = new Vector(new PointF(100, 100), new PointF(900, 900));
            PointF point = vector.Cut(0.1f);
            Assert.AreEqual(180, point.X, "X-Coordinate differs.");
            Assert.AreEqual(180, point.Y, "Y-Coordinate differs.");
        }

        [TestMethod]
        public void TestIntersection()
        {
            Vector verticalVector = new Vector(new PointF(500, 100), new PointF(500, 900));
            Vector horizontalVector = new Vector(new PointF(100, 500), new PointF(900, 500));

            PointF intersection = verticalVector.Intersect(horizontalVector);
            Assert.AreEqual(500, intersection.X, "X-Coordinate differs.");
            Assert.AreEqual(500, intersection.Y, "Y-Coordinate differs.");
        }
    }
}
