using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Contracts.Categories
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
