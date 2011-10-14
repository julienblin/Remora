using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Remora.Configuration;

namespace Remora.Impl
{
    public class CategoryResolver : ICategoryResolver
    {
        private readonly IRemoraConfig _config;

        public CategoryResolver(IRemoraConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");
            Contract.EndContractBlock();

            _config = config;
        }

        public Category Resolve(string url)
        {
            if (url == null) throw new ArgumentNullException("url");
            Contract.EndContractBlock();

            var result = (from cat in _config.Categories
                         where Regex.IsMatch(url, cat.UrlMatcher)
                         select cat).FirstOrDefault();

            if(result == null)
                throw new RemoraException(url + " could not be matched with any category.");

            return result;
        }
    }
}
