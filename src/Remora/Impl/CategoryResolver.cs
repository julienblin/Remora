using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using Remora.Configuration;

namespace Remora.Impl
{
    public class CategoryResolver : ICategoryResolver
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CategoryResolver).Name);

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

            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Resolving category for url {0}...", url);

            var result = (from cat in _config.Categories
                         where Regex.IsMatch(url, cat.UrlMatcher)
                         select cat).FirstOrDefault();

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Found category: {0}...", result != null ? result.Name : "<not found>");

            if(result == null)
                throw new RemoraException(url + " could not be matched with any category.");

            return result;
        }
    }
}
