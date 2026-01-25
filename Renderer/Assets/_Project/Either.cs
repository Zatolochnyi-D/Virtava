using System;

namespace DenZ.DevelopmentTools.Options
{
    public readonly struct Either<TLeft, TRight>
    {
        private readonly TLeft _leftValue;
        private readonly TRight _rightValue;
        private readonly bool _isLeft;

        public bool IsLeft => _isLeft;
        public bool IsRight => !_isLeft;
        public TLeft LeftValue => _isLeft ? _leftValue : throw new ArgumentException("Cannot read left value of Right.");
        public TRight RightValue => !_isLeft ? _rightValue : throw new ArgumentException("Cannot read right value of Left.");

        public Either(TLeft value)
        {
            _leftValue = value;
            _rightValue = default;
            _isLeft = true;
        }

        public Either(TRight value)
        {
            _rightValue = value;
            _leftValue = default;
            _isLeft = false;
        }
    }
}