using System;
using System.Reflection;
using EugeneC.Singleton;
using UnityEngine;

namespace EugeneC.Utilities {

    public static partial class HelperCollection {

        // Can be non-static but the use case is rare, usually do interface instead
        public static void CallStaticMethod(string className, string methodName) {
            var classType = Type.GetType(className);

            if (classType is not null) {
                var method = classType.GetMethod(methodName);

                if (method is not null)
                    method.Invoke(className, null);
                else
                    Debug.LogWarning($"Method '{methodName}' not found on {className}.");
            }
            else {
                Debug.LogWarning($"Class '{className}' not found.");
            }
        }

        // Use this if the singleton is self-declared
        public static void CallInstanceMethod(string instanceClassName, string methodName) {
            var classType = Assembly.GetExecutingAssembly().GetType(instanceClassName);

            if (classType is not null) {
                var property = classType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

                if (property is not null) {
                    var classInstance = property.GetValue(null);

                    if (classInstance is not null) {
                        var method = classType.GetMethod(methodName);

                        if (method is not null)
                            method.Invoke(classInstance, null);
                        else
                            Debug.LogWarning($"Method '{methodName}' not found on {instanceClassName}.");
                    }
                    else {
                        Debug.LogWarning($"Instance of class '{instanceClassName}' not found.");
                    }
                }
                else {
                    Debug.LogWarning($"Static 'Instance' is not found in {instanceClassName}");
                }
            }
            else {
                Debug.LogWarning($"Class '{instanceClassName}' not found.");
            }
        }

        // For any singleton inherited from generic singleton
        public static void CallGenericInstanceMethod(string instanceClassName, string methodName) {
            var classType = Assembly.GetExecutingAssembly().GetType(instanceClassName);

            if (classType is not null && typeof(MonoBehaviour).IsAssignableFrom(classType)) {
                var genericSingletonType = typeof(GenericSingleton<>).MakeGenericType(classType);

                var instanceProperty =
                    genericSingletonType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

                if (instanceProperty is not null) {
                    var classInstance = instanceProperty.GetValue(null);

                    if (classInstance != null) {
                        var method = classType.GetMethod(methodName);

                        if (method is not null)
                            method.Invoke(classInstance, null);
                        else
                            Debug.LogWarning($"Method '{methodName}' not found on {instanceClassName}.");
                    }
                    else {
                        Debug.LogWarning($"Instance of class '{instanceClassName}' is null.");
                    }
                }
                else {
                    Debug.LogWarning($"Static 'Instance' property not found on class '{instanceClassName}'.");
                }
            }
            else {
                Debug.LogWarning($"Class '{instanceClassName}' not found or does not inherit from MonoBehaviour.");
            }
        }

    }

}