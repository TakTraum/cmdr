using cmdr.Editor.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cmdr.Editor.ViewModels
{
    public class ConditionDescription
    {
        public string Condition { get; set; }
        public string Description { get; set; }
    }

    public class ConditionDescriptions
    {
        private static List<ConditionDescription> _dict = new List<ConditionDescription>();
        public static List<ConditionDescription> Dict
        {
            get { return ConditionDescriptions._dict; }
            set { ConditionDescriptions._dict = value; }
        }


        public static void Generate(IEnumerable<MappingViewModel> mappingViewModels)
        {
            var expressions = mappingViewModels.Where(m => !String.IsNullOrWhiteSpace(m.ConditionExpression)).Select(m => m.ConditionExpression).Distinct();
            expressions = expressions.Select(e => String.Join(" AND ", e.Split(new[] { " AND " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(ex => ex))).Distinct().OrderBy(e => e);
            var descriptions = expressions.Select(e => new ConditionDescription { Condition = e, Description = e });
            
            _dict = descriptions.ToList();
        }

        public static void Edit()
        {
            ConditionDescriptionsEditor editor = new ConditionDescriptionsEditor
            {
                DataContext = Dict
            };
            editor.ShowDialog();
        }
    }
}
