using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Arrow.Configuration;
using Arrow.Xml;
using Arrow.Xml.ObjectCreation;

namespace Arrow.Net
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// <!<![CDATA[
    /// 
    /// <UriManager>
    ///     <Template name="foo">
    ///         <Scheme>https</Scheme>
    ///         <Host>www.service.com</Host>
    ///         <Port>8080</Port>
    ///     </Template>
    ///     
    ///     <Endpoint name="getUsers" template="foo">
    ///         <Path>GetUsers/All</Path>
    ///     </Endpoint>
    /// </UriManager>
    /// 
    /// ]]>
    /// </example>
    public sealed partial class UriManager : IUriManager, ICustomXmlInitialization
    {
        private static readonly Lazy<IUriManager> s_FromAppConfig = new(DoFromAppConfig, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly Dictionary<string, Uri> m_Uris = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Registers a new uri, replacing any existing registration
        /// </summary>
        /// <param name="name"></param>
        /// <param name="uri"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Register(string name, Uri uri)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty", nameof(name));
            if(uri is null) throw new ArgumentNullException(nameof(uri));

            m_Uris[name] = uri;
        }

        /// <inheritdoc/>
        public Uri GetUri(string name)
        {
            if(TryGetUri(name, out var uri)) return uri;

            throw new ArrowException($"could not find {name}");
        }

        /// <inheritdoc/>
        public bool TryGetUri(string name, [NotNullWhen(true)] out Uri? uri)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));

            return m_Uris.TryGetValue(name, out uri);
        }

        /// <summary>
        /// Loads the uri manager stores in the app.config
        /// </summary>
        /// <returns></returns>
        public static IUriManager FromAppConfig()
        {
            return s_FromAppConfig.Value;
        }

        /// <summary>
        /// Creates a manager from xml
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static UriManager FromXml(XmlNode node)
        {
            if(node is null) throw new ArgumentNullException(nameof(node));

            var manager = new UriManager();
            manager.DoFromXml(node, new CustomXmlCreation());
            
            return manager;
        }

        private static IUriManager DoFromAppConfig()
        {
            var node = AppConfig.GetSectionXml(ArrowSystem.Name, "Arrow.Net/UriManager");
            if(node is null) throw new ArrowException("could not find Arrow.Net/UriManager");

            return FromXml(node);
        }

        private void DoFromXml(XmlNode node, ICustomXmlCreation factory)
        {
            if(node is null) throw new ArgumentNullException(nameof(node));

            var templates = factory.CreateList<EndpointDetails>(node.SelectNodesOrEmpty("Template"))
                                   .Where(d => d.Name is not null)
                                   .ToDictionary(d => d.Name!, StringComparer.OrdinalIgnoreCase);

            var endpoints = factory.CreateList<EndpointDetails>(node.SelectNodesOrEmpty("Endpoint"));

            foreach(var endpoint in endpoints)
            {
                if(string.IsNullOrWhiteSpace(endpoint.Name)) throw new ArrowException("no name for endpoint");

                var uri = MakeUri(templates, endpoint);
                Register(endpoint.Name!, uri);
            }
        }

        private static Uri MakeUri(IReadOnlyDictionary<string, EndpointDetails> templates, EndpointDetails endpoint)
        {
            // A template is optional...
            EndpointDetails? template = BuildTemplate(templates, endpoint.Template);

            UriBuilder? builder = null;

            if(endpoint.Uri is not null)
            {
                builder = new(endpoint.Uri);
            }
            else
            {
                builder = new();
                if(template is not null) ApplyOptional(builder, template);

                if(template is not null && string.IsNullOrWhiteSpace(template.Scheme) == false) builder.Scheme = template.Scheme;

                if(builder.Scheme is null) throw new UriFormatException("no scheme");

                ApplyOptional(builder, endpoint);
            }

            /*
             * When building up the query we'll apply the uri query first (if present)
             * then any template parameters and finally the endpoint parameters.
             * After chewing on it this seems like the best approach!!
             */

            var query = Enumerable.Empty<EndpointParameter>();

            if(endpoint.Uri is not null)
            {
                var uriQuery = endpoint.Uri.QueryParameters()
                                           .Select(d => new EndpointParameter(){Name = d.Name, Value = d.Value, IgnoreIfEmpty = false});

                query = query.Concat(uriQuery);
            }

            if(template is not null)
            {
                query = query.Concat(template.Query);
            }

            query = query.Concat(endpoint.Query);
            builder.Query = MakeQuery(query);

            return builder.Uri;
        }

        private static string MakeQuery(IEnumerable<EndpointParameter> parameters)
        {
            var parts = parameters.Where(p => ShouldInclude(p))
                                  .Select(p => Encode(p));

            return string.Join("&", parts);

            static bool ShouldInclude(EndpointParameter p)
            {
                if(p.IgnoreIfEmpty)
                {
                    if(string.IsNullOrWhiteSpace(p.Value)) return false;
                }

                return true;
            }

            static string Encode(EndpointParameter p)
            {
                if(string.IsNullOrWhiteSpace(p.Name)) throw new ArrowException("invalid parameter name");

                if(p.Value is null)
                {
                    return Uri.EscapeDataString(p.Name);
                }
                else
                {
                    return $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.Value)}";
                }
            }
        }

        private static EndpointDetails? BuildTemplate(IReadOnlyDictionary<string, EndpointDetails> templates, string? template)
        {
            if(string.IsNullOrWhiteSpace(template)) return null;

            var acc = new EndpointDetails();
            var seenSoFar = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            RecursiveBuild(templates, template, acc, seenSoFar);
            return acc;
            
            static void RecursiveBuild(IReadOnlyDictionary<string, EndpointDetails> templates, string? templateName, EndpointDetails acc, HashSet<string> seenSoFar)
            {
                if(string.IsNullOrWhiteSpace(templateName)) return;

                if(templates.TryGetValue(templateName!, out var template))
                {
                    if(seenSoFar.Add(templateName!) == false) throw new ArrowException($"circular template reference encountere when processing {templateName}");
                    RecursiveBuild(templates, template.Template, acc, seenSoFar);
                    ApplyTemplate(acc, template);
                }
                else
                {
                    throw new ArrowException($"no such template: {templateName}");
                }
            }
        }

        private static void ApplyTemplate(EndpointDetails target, EndpointDetails source)
        {
            if(!string.IsNullOrWhiteSpace(source.Scheme)) target.Scheme = source.Scheme;
            if(!string.IsNullOrWhiteSpace(source.Host)) target.Host = source.Host;
            if(!string.IsNullOrWhiteSpace(source.Username)) target.Username = source.Username;
            if(!string.IsNullOrWhiteSpace(source.Password)) target.Password = source.Password;
            if(source.Port.HasValue) target.Port = source.Port;
            if(!string.IsNullOrWhiteSpace(source.Path)) target.Path = source.Path;

            if(source.Query.Count != 0) target.Query.AddRange(source.Query);
        }

        private static void ApplyOptional(UriBuilder builder, EndpointDetails source)
        {
            if(!string.IsNullOrWhiteSpace(source.Host)) builder.Host = source.Host;
            if(!string.IsNullOrWhiteSpace(source.Username)) builder.UserName = source.Username;
            if(!string.IsNullOrWhiteSpace(source.Password)) builder.Password = source.Password;
            if(source.Port.HasValue) builder.Port = source.Port.GetValueOrDefault();
            if(!string.IsNullOrWhiteSpace(source.Path)) builder.Path = source.Path;
        }

        void ICustomXmlInitialization.InitializeObject(XmlNode rootNode, ICustomXmlCreation factory)
        {
            DoFromXml(rootNode, factory);
        }
    }
}
