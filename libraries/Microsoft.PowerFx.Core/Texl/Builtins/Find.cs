﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.AppMagic.Authoring.Texl
{
    // Find(find_text:s, within_text:s, [start_index:n])
    // Equivalent DAX function: Find
    internal sealed class FindFunction : BuiltinFunction
    {
        public override bool RequiresErrorContext => true;
        public override bool IsSelfContained => true;

        public FindFunction()
            : base("Find", TexlStrings.AboutFind, FunctionCategories.Text, DType.Number, 0, 2, 3, DType.String, DType.String, DType.Number)
        { }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new [] { TexlStrings.FindArg1, TexlStrings.FindArg2 };
            yield return new [] { TexlStrings.FindArg1, TexlStrings.FindArg2, TexlStrings.FindArg3 };
        }
    }

    // Find(find_text:s|*[s], within_text:s|*[s], [start_index:n|*[n]])
    internal sealed class FindTFunction : BuiltinFunction
    {
        public override bool RequiresErrorContext => true;
        public override bool IsSelfContained => true;

        public FindTFunction()
            : base("Find", TexlStrings.AboutFindT, FunctionCategories.Table, DType.EmptyTable, 0, 2, 3)
        { }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new [] { TexlStrings.FindTArg1, TexlStrings.FindTArg2 };
            yield return new [] { TexlStrings.FindTArg1, TexlStrings.FindTArg2, TexlStrings.FindTArg3 };
        }

        public override string GetUniqueTexlRuntimeName(bool isPrefetching = false)
        {
            return GetUniqueTexlRuntimeName(suffix: "_T");
        }

        public override bool CheckInvocation(TexlNode[] args, DType[] argTypes, IErrorContainer errors, out DType returnType)
        {
            Contracts.AssertValue(args);
            Contracts.AssertAllValues(args);
            Contracts.AssertValue(argTypes);
            Contracts.Assert(args.Length == argTypes.Length);
            Contracts.AssertValue(errors);
            Contracts.Assert(MinArity <= args.Length && args.Length <= MaxArity);

            bool fValid = base.CheckInvocation(args, argTypes, errors, out returnType);

            DType type0 = argTypes[0];
            DType type1 = argTypes[1];

            // Arg0 should be either a string or a column of strings.
            if (type0.IsTable)
            {
                // Ensure we have a one-column table of strings.
                fValid &= CheckStringColumnType(type0, args[0], errors);
            }
            else if (!DType.String.Accepts(type0))
            {
                fValid = false;
                errors.EnsureError(DocumentErrorSeverity.Severe, args[0], TexlStrings.ErrStringExpected);
            }

            // Arg1 should be either a string or a column of strings.
            if (type1.IsTable)
            {
                fValid &= CheckStringColumnType(type1, args[1], errors);
            }
            else if (!DType.String.Accepts(type1))
            {
                fValid = false;
                errors.EnsureError(DocumentErrorSeverity.Severe, args[1], TexlStrings.ErrStringExpected);
            }

            returnType = DType.CreateTable(new TypedName(DType.Number, OneColumnTableResultName));

            bool hasStartIndex = argTypes.Length == 3;

            if (hasStartIndex)
            {
                DType type2 = argTypes[2];

                // Arg2 should be either a number or a column of numbers.
                if (argTypes[2].IsTable)
                {
                    fValid &= CheckNumericColumnType(type2, args[2], errors);
                }
                else if (!DType.Number.Accepts(type2))
                {
                    fValid = false;
                    errors.EnsureError(DocumentErrorSeverity.Severe, args[2], TexlStrings.ErrNumberExpected);
                }
            }

            // At least one arg has to be a table.
            if (!(type0.IsTable || type1.IsTable) && (!hasStartIndex || !argTypes[2].IsTable))
                fValid = false;

            return fValid;
        }
    }
}