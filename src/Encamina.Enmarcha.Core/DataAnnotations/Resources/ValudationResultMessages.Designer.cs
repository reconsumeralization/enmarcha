﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Encamina.Enmarcha.Core.DataAnnotations.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ValudationResultMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ValudationResultMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Encamina.Enmarcha.Core.DataAnnotations.Resources.ValudationResultMessages", typeof(ValudationResultMessages).Assembly);
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
        ///   Looks up a localized string similar to The value of &apos;{0}&apos; must be a valid and not empty or whitespace string!.
        /// </summary>
        internal static string ValueIsEmptyOrWhiteSpace {
            get {
                return ResourceManager.GetString("ValueIsEmptyOrWhiteSpace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of &apos;{0}&apos; must be a valid GUID and it must not be the empty GUID (i.e., 00000000-0000-0000-0000-000000000000)!.
        /// </summary>
        internal static string ValueIsInvalidGuid {
            get {
                return ResourceManager.GetString("ValueIsInvalidGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of &apos;{0}&apos; must be a valid and well formed URI!.
        /// </summary>
        internal static string ValuieIsInvalidUri {
            get {
                return ResourceManager.GetString("ValuieIsInvalidUri", resourceCulture);
            }
        }
    }
}
