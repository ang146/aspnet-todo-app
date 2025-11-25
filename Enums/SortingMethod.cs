using System.ComponentModel;

namespace TodoApp.Enums
{
    public enum SortingMethod
    {
        [Description("By Task Status")]
        IsDone,
        [Description("By Task Title")]
        Alphabet,
        [Description("By Task Priority")]
        Priority,
        [Description("By Task Due Date")]
        Deadline,
    }
}
