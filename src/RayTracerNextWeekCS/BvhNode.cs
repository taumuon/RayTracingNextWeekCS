namespace RayTracerNextWeekCS
{
    public class BvhNode : IHittable
    {
        private AABB bBox = AABB.Empty;
        private IHittable left;
        private IHittable right;

        public BvhNode(Hittables hittables)
            : this(hittables.GetHittables().ToArray().AsSpan())
        {
        }

        public BvhNode(List<IHittable> objects)
            : this(objects.ToArray().AsSpan())
        {
        }

        public BvhNode(Span<IHittable> objects)
        {
            var bBoxLocal = AABB.Empty;
            foreach (var obj in objects)
            {
                bBoxLocal = new AABB(bBoxLocal, obj.BoundingBox);
            }
            // var bBoxLocal = objects.AsEnumerable().Aggregate<IHittable, AABB>(new AABB(), (current, hittable) => new AABB(current, hittable.BoundingBox));

            int axis = bBoxLocal.LongestAxis;

            Comparison<IHittable> comparison = (axis == 0)
                ? BoxXCompare
                : (axis == 1)
                    ? BoxYCompare
                    : BoxZCompare;

            var objectSpan = objects.Length;

            if (objectSpan == 1)
            {
                // Ideally callers wouldn't construct a BvhNode containing a single item
                left = right = objects[0];
            }
            else if (objectSpan == 2)
            {
                left = objects[0];
                right = objects[1];
            }
            else
            {
                objects.Sort(comparison);

                var mid = objectSpan / 2;
                var leftObjs = objects[0..mid];
                var rightObjs = objects[mid..];

                left = leftObjs.Length == 1
                    ? leftObjs[0]
                    : new BvhNode(leftObjs);
                right = rightObjs.Length == 1
                    ? rightObjs[0]
                    : new BvhNode(rightObjs);
            }

            bBox = bBoxLocal;
        }


        public AABB BoundingBox => bBox;

        public bool Hit(Ray ray, Interval rayT, ref HitRecord rec)
        {
            bool bBoxHit = bBox.Hit(ray, rayT);
            if (!bBoxHit)
            {
                return false;
            }

            bool hitLeft = false, hitRight = false;
            HitRecord hitRecordLeft = default, hitRecordRight = default;

            hitLeft = left.Hit(ray, rayT, ref hitRecordLeft);

            var rightTMax = hitLeft ? hitRecordLeft.T : rayT.Max;
            var rightT = new Interval(rayT.Min, rightTMax);
            
            hitRight = right.Hit(ray, rightT, ref hitRecordRight);

            if (hitRight)
            {
                rec = hitRecordRight;
                return true;
            }
            else if (hitLeft)
            {
                rec = hitRecordLeft;
                return true;
            }

            return false;
        }

        private static int BoxCompare(IHittable a, IHittable b, int axisIndex)
        {
            var res = a.BoundingBox.AxisIntervalMin(axisIndex).CompareTo(b.BoundingBox.AxisIntervalMin(axisIndex));
            // Sort of Span isn't stable, so ensure order by other axes
            //  so that subsequent partitioning of these axes are balanced
            if (res == 0)
            {
                axisIndex = axisIndex == 2 ? 0 : (axisIndex + 1);
                res = a.BoundingBox.AxisIntervalMin(axisIndex).CompareTo(b.BoundingBox.AxisIntervalMin(axisIndex));
                if (res == 0)
                {
                    axisIndex = axisIndex == 2 ? 0 : (axisIndex + 1);
                    res = a.BoundingBox.AxisIntervalMin(axisIndex).CompareTo(b.BoundingBox.AxisIntervalMin(axisIndex));
                }
            }
            return res;
        }

        private static int BoxXCompare(IHittable a, IHittable b)
        {
            return BoxCompare(a, b, 0);
        }

        private static int BoxYCompare(IHittable a, IHittable b)
        {
            return BoxCompare(a, b, 1);
        }

        private static int BoxZCompare(IHittable a, IHittable b)
        {
            return BoxCompare(a, b, 2);
        }
    }
}
