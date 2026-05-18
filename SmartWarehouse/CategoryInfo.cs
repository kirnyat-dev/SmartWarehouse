using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWarehouse
{
    public readonly struct CategoryInfo
    {
        private readonly string _name;
        private readonly string _code;

        public CategoryInfo(string name, string code)
        {
            _name = name;
            _code = code;
        }

        public string Name => _name;

        public string Code => _code;

        public override string ToString() => $"{_name} (код: {_code})";
    }
}