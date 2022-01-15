using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Smr.Extensions;

namespace Game.Editor {
    public static class ReflectionHelper {
        public static IEnumerable<Type> GetDerivedTypes(Type ancestorType, string[] assembliesPrefixes) =>
            from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
            where domainAssembly.FullName.StartsWithAny(assembliesPrefixes)
            from assemblyType in domainAssembly.GetTypes()
            where ancestorType.IsAssignableFrom(assemblyType) && assemblyType != ancestorType
            select assemblyType;

        public static List<Type> GetHeirs(Type type) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var heirs = new List<Type>();

            var assemblyTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(type));

            heirs.AddRange(assemblyTypes);

            return heirs;
        }
        
        public static object CreateInstance(Type type) {
            try {
                return Activator.CreateInstance(type);
            } catch {
                return FormatterServices.GetUninitializedObject(type);
            }
        }
    }
}