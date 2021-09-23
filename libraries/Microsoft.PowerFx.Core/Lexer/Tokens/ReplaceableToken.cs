//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.AppMagic.Authoring.Texl
{
    internal class ReplaceableToken : Token
    {
        private readonly string _val;

        public ReplaceableToken(string val, Span span)
            : base(TokKind.ReplaceableLit, span)
        {
            Contracts.AssertValue(val);
            _val = val;
        }

        protected ReplaceableToken(string value, TokKind kind, Span span)
            : base(kind, span)
        {
            Contracts.AssertValue(value);
            _val = value;
        }

        protected ReplaceableToken(ReplaceableToken tok, Span newSpan)
            : this(tok.Value, newSpan)
        {
        }

        public string Value => _val;

        public override string ToString() => _val;

        public override Token Clone(Span ts) => new ReplaceableToken(this, ts);

        public override bool Equals(Token that)
        {
            Contracts.AssertValue(that);

            if (!(that is ReplaceableToken))
                return false;

            return Value == that.As<ReplaceableToken>().Value && base.Equals(that);
        }
    }
}