using System;

namespace DenZ.DevelopmentTools.Options
{
    public static class Either
    {
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value)
        {
            return new(value);
        }

        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value)
        {
            return new(value);
        }


        public static TLeft ReadLeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, TLeft defaultValue)
        {
            if (either.IsLeft)
                return either.LeftValue;
            else
                return defaultValue;
        }

        public static TRight ReadRightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, TRight defaultValue)
        {
            if (either.IsRight)
                return either.RightValue;
            else
                return defaultValue;
        }

        public static void ApplyElse<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft> leftAction, Action<TRight> rightAction)
        {
            if (either.IsLeft)
                leftAction(either.LeftValue);
            else
                rightAction(either.RightValue);
        }
    }
}