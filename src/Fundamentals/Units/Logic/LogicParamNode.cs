﻿using Ludiq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bolt.Addons.Community.Fundamentals
{
    public abstract class LogicParamNode : VariadicNode<bool>
    {
        public enum BranchType { And, Or, GreaterThan, LessThan, Equal }


        [SerializeAs(nameof(BranchingType))]
        private BranchType _branchingType;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Condition")]
        public BranchType BranchingType { get { return _branchingType; } set { _branchingType = value; } }



        [DoNotSerialize]
        public bool supportsEqual => BranchingType == BranchType.LessThan || BranchingType == BranchType.GreaterThan;



        [Serialize]
        [InspectorLabel("Allow Equals [[Less, Greater]]")]
        [Inspectable]
        //[InspectableIf(nameof(supportsEqual))]
        protected bool AllowEquals = false;



        [DoNotSerialize]
        public bool supportsNumeric => BranchingType == BranchType.Equal;


        [Serialize]
        [Inspectable]
        [InspectorLabel("Numeric [[Equals]]")]
        //[InspectableIf(nameof(supportsNumeric))]
        protected bool Numeric = false;


        protected override void Definition()
        {
            arguments = new List<ValueInput>();

            switch (BranchingType)
            {
                case BranchType.And:
                case BranchType.Or:
                    ConstructArgs<bool>();
                    break;
                case BranchType.GreaterThan:
                case BranchType.LessThan:
                    ConstructArgs<float>();
                    break;
                case BranchType.Equal:
                    if (Numeric)
                        ConstructArgs<float>();
                    else
                        ConstructArgs<object>();
                    break;
                default:
                    break;
            }
        }

        private void ConstructArgs<T>()
        {
            for (var i = 0; i < Math.Min(argumentCount, ArgumentLimit()); i++)
            {
                var argument = ValueInput<T>(GetArgumentName(i));
                arguments.Add(argument);
                BuildRelations(argument);
            }
        }

        protected virtual string GetArgumentName(int index)
        {
            return "Arg_" + index;
        }

        protected override int ArgumentLimit()
        {
            if (BranchingType == BranchType.GreaterThan || BranchingType == BranchType.LessThan)
                return 2;
            return base.ArgumentLimit();
        }
    }
}