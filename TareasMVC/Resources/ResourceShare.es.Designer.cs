//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TareasMVC.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    ///   This class was generated by MSBuild using the GenerateResource task.
    ///   To add or remove a member, edit your .resx file then rerun MSBuild.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Build.Tasks.StronglyTypedResourceBuilder", "15.1.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ResourceShare_es {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResourceShare_es() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TareasMVC.Resources.ResourceShare.es", typeof(ResourceShare_es).Assembly);
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
        ///   Looks up a localized string similar to El campo debe ser un correo electrónico válido.
        /// </summary>
        internal static string Error_Email {
            get {
                return ResourceManager.GetString("Error.Email", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to El campo {0} es requerido.
        /// </summary>
        internal static string Error_Required {
            get {
                return ResourceManager.GetString("Error.Required", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Recuérdame.
        /// </summary>
        internal static string Remember {
            get {
                return ResourceManager.GetString("Remember", resourceCulture);
            }
        }
    }
}
