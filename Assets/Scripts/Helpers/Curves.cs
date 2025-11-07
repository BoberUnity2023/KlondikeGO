using UnityEngine;

namespace BloomLines.Helpers
{
    public static class Curves
    {
        public static AnimationCurve Lerp(AnimationCurve a, AnimationCurve b, float alpha, int resolution = 20)
        {
            AnimationCurve result = new AnimationCurve();

            float startTime = Mathf.Min(a.keys[0].time, b.keys[0].time);
            float endTime = Mathf.Max(a.keys[a.length - 1].time, b.keys[b.length - 1].time);

            for (int i = 0; i <= resolution; i++)
            {
                float t = Mathf.Lerp(startTime, endTime, (float)i / resolution);
                float aValue = a.Evaluate(t);
                float bValue = b.Evaluate(t);
                float lerpedValue = Mathf.Lerp(aValue, bValue, alpha);

                result.AddKey(new Keyframe(t, lerpedValue));
            }

            return result;
        }
    }
}