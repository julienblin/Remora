﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Remora {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrorResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Remora.ErrorResources", typeof(ErrorResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///  &lt;body&gt;
        ///    &lt;h1&gt;Internal error: {0}&lt;/h1&gt;
        ///    &lt;h2&gt;{1}&lt;/h2&gt;
        ///    &lt;p&gt;Check the log for more information.&lt;/p&gt;
        ///  &lt;/body&gt;
        ///&lt;/html&gt;.
        /// </summary>
        internal static string GenericHtmlError {
            get {
                return ResourceManager.GetString("GenericHtmlError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;s:Envelope xmlns:s=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot;&gt;
        ///  &lt;s:Header /&gt;
        ///  &lt;s:Body&gt;
        ///    &lt;s:Fault&gt;
        ///      &lt;faultcode&gt;{0}&lt;/faultcode&gt;
        ///      &lt;faultstring&gt;{1}&lt;/faultstring&gt;
        ///    &lt;/s:Fault&gt;
        ///  &lt;/s:Body&gt;
        ///&lt;/s:Envelope&gt;.
        /// </summary>
        internal static string SoapError {
            get {
                return ResourceManager.GetString("SoapError", resourceCulture);
            }
        }
    }
}
