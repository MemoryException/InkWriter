using System;
using System.Drawing;

namespace InkWriter.Algorithms
{
    public class Vector
    {
        public Vector(PointF startingPoint, PointF endingPoint)
        {
            this.StartingPoint = startingPoint;
            this.EndingPoint = endingPoint;
        }

        public PointF EndingPoint { get; private set; }

        public PointF StartingPoint { get; private set; }

        public float DeltaX
        {
            get
            {
                return this.EndingPoint.X - this.StartingPoint.X;
            }
        }

        public float DeltaY
        {
            get
            {
                return this.EndingPoint.Y - this.StartingPoint.Y;
            }
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(Math.Pow(this.DeltaX, 2) + Math.Pow(this.DeltaY, 2));
            }
        }

        public PointF Cut(float percentage)
        {
            return new PointF(
                this.StartingPoint.X + this.DeltaX * percentage,
                this.StartingPoint.Y + this.DeltaY * percentage);
        }

        public Vector Merge(Vector otherVector, float percentage)
        {
            Vector startingPoints = new Vector(this.StartingPoint, otherVector.StartingPoint);
            Vector endingPoints = new Vector(this.EndingPoint, otherVector.EndingPoint);

            return new Vector(startingPoints.Cut(percentage), endingPoints.Cut(percentage));
        }

        public PointF Intersect(Vector otherVector)
        {
            float dy1 = this.EndingPoint.Y - this.StartingPoint.Y;
            float dx1 = this.EndingPoint.X - this.StartingPoint.X;
            float dy2 = otherVector.EndingPoint.Y - otherVector.StartingPoint.Y;
            float dx2 = otherVector.EndingPoint.X - otherVector.StartingPoint.X;

            if (dy1 * dx2 == dy2 * dx1)
            {
                return PointF.Empty;
            }

            float x = ((otherVector.StartingPoint.Y - this.StartingPoint.Y) * dx1 * dx2 + dy1 * dx2 * this.StartingPoint.X - dy2 * dx1 * otherVector.StartingPoint.X) / (dy1 * dx2 - dy2 * dx1);
            float y = ((otherVector.StartingPoint.X - this.StartingPoint.X) * dy1 * dy2 + dx1 * dy2 * this.StartingPoint.Y - dx2 * dy1 * otherVector.StartingPoint.Y) / (dx1 * dy2 - dx2 * dy1);
            return new PointF(x, y);
        }
    }
}
