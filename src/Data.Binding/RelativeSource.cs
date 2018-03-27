using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.Data
{

    public class RelativeSource
    {
        private int ancestorLevel;
        private Type ancestorType;
        private RelativeSourceMode mode;
        private static RelativeSource previousData;
        private static RelativeSource self;
        private static RelativeSource templateParent;

        public int AncestorLevel { get => ancestorLevel; set => ancestorLevel = value; }
        public Type AncestorType { get => ancestorType; set => ancestorType = value; }
        public RelativeSourceMode Mode { get => mode; set => mode = value; }

        public RelativeSource()
        {
        }

        public RelativeSource(RelativeSourceMode mode)
        {
            this.mode = mode;
        }

        public RelativeSource(Type ancestorType, int ancestorLevel)
        {
            this.ancestorType = ancestorType;
            this.ancestorLevel = ancestorLevel;
            this.mode = RelativeSourceMode.FindAncestor;
        }

        public static RelativeSource TemplateParent
        {
            get
            {
                if (templateParent == null)
                    templateParent = new RelativeSource(RelativeSourceMode.TemplateParent);
                return templateParent;
            }
        }
        public static RelativeSource Self
        {
            get
            {
                if (self == null)
                    self = new RelativeSource(RelativeSourceMode.Self);
                return self;
            }
        }
        public static RelativeSource PreviousData
        {
            get
            {
                if (previousData == null)
                    previousData = new RelativeSource(RelativeSourceMode.PreviousData);
                return previousData;
            }
        }

    }

    public enum RelativeSourceMode
    {
        FindAncestor,
        PreviousData,
        Self,
        TemplateParent,
    }
}
