using System;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiPath.FlowProfiler.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();

            builder.AddCustomAttributes(typeof(ProfileActivity), new DesignerAttribute(typeof(ProfilerDesigner)));
            var profileCategoryAttribute =
                new CategoryAttribute($"Workflow.Profiling");
            builder.AddCustomAttributes(typeof(ProfileActivity), profileCategoryAttribute);
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
