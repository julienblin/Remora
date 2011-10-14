using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Remora.Impl;

namespace Remora
{
    public class RemoraConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new RemoraConfig();

            config.RequestProcessorType = LoadCustomComponent<IRequestProcessor>(section, "/*/requestProcessor",
                                                                                 typeof (RequestProcessor));

            config.CategoryResolverType = LoadCustomComponent<ICategoryResolver>(section, "/*/categoryResolver",
                                                                                 typeof(CategoryResolver));

            config.Categories = LoadCategories(section);

            return config;
        }

        private Type LoadCustomComponent<T>(XmlNode section, string xpath, Type defaultType)
        {
            var node = section.SelectSingleNode(xpath);
            if (node == null)
                return defaultType;
            
            var loadedType = Type.GetType(node.Attributes["type"].Value);

            if(!typeof(T).IsAssignableFrom(loadedType))
                throw new RemoraException(string.Format("Unable to use type {0} as {1}", loadedType, typeof(T)));

            return loadedType;
        }

        private IEnumerable<Category> LoadCategories(XmlNode section)
        {
            var result = new List<Category>();
            var categoryNodes = section.SelectNodes("/*/categories/category");

            foreach (XmlNode categoryNode in categoryNodes)
            {
                var category = new Category();

                foreach (XmlAttribute attribute in categoryNode.Attributes)
                {
                    if (attribute.Name == "name")
                    {
                        category.Name = attribute.Value;
                    }
                    else
                    {
                        category.Properties[attribute.Name] = attribute.Value;
                    }
                }

                if (string.IsNullOrEmpty(category.Name))
                    category.Name = "default";

                LoadPipelineComponents(categoryNode, category);

                result.Add(category);
            }

            return result;
        }

        private void LoadPipelineComponents(XmlNode categoryNode, Category category)
        {
            var componentNodes = categoryNode.SelectNodes("component");

            foreach (XmlNode componentNode in componentNodes)
            {
                var def = new PipelineComponentDefinition();

                foreach (XmlAttribute attribute in componentNode.Attributes)
                {
                    if (attribute.Name == "type")
                    {
                        var loadedType = Type.GetType(attribute.Value);

                        if(!typeof(IPipelineComponent).IsAssignableFrom(loadedType))
                            throw new RemoraException(string.Format("Unable to use type {0} as a pipeline component: must implement the IPipelineComponent interface ", loadedType));
                        def.Type = loadedType;
                    }
                    else
                    {
                        def.Properties[attribute.Name] = attribute.Value;
                    }
                }

                category.PipelineComponents.Add(def);
            }
        }
    }
}
