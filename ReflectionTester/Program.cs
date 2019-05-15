using System;
using System.Reflection;

// This console app was created to experiment with the code described at https://mattwarren.org/2016/12/14/Why-is-Reflection-slow/, some of which can be
// found here: https://gist.github.com/mattwarren/be21d80a016043ea5c462415b81d9b69.
namespace ReflectionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing TestClass object...\n");

            TestTestClass();

            Console.WriteLine("Testing anonymous object...\n");

            TestAnonymousClass();

            Console.WriteLine("Goodbye.  Press any key please.");
            Console.ReadKey();
        }

        private static void TestTestClass()
        {
            TestClass testClassObject = new TestClass();
            testClassObject.TestProperty0 = 5;
            testClassObject.TestProperty1 = "Winter is coming.";

            Type t = testClassObject.GetType();

            Console.WriteLine("Getting cache info to begin with...\n");

            GetRuntimeCacheInfo(t);
            Console.WriteLine();

            FieldInfo fi = t.GetField("privateTestField0");

            Console.WriteLine("Getting cache info after calling GetField(\"privateTestField0\")...\n");
            GetRuntimeCacheInfo(t);
            Console.WriteLine();

            fi = t.GetField("privateTestField1");

            Console.WriteLine("Getting cache info after calling GetField(\"privateTestField1\")...\n");
            GetRuntimeCacheInfo(t);
            Console.WriteLine();

            t.GetField("publicTestField0");

            Console.WriteLine("Getting cache info after calling GetField(\"publicTestField0\")...\n");
            GetRuntimeCacheInfo(t);
            Console.WriteLine();

            fi = t.GetField("publicTestField1");

            Console.WriteLine("Getting cache info after calling GetField(\"publicTestField1\")...\n");
            GetRuntimeCacheInfo(t);
            Console.WriteLine();

            fi = t.GetField("staticTestField");

            Console.WriteLine("Getting cache info after calling GetField(\"staticTestField\")...\n");
            GetRuntimeCacheInfo(t);
            Console.WriteLine();

            PropertyInfo pi = t.GetProperty("TestProperty0");

            Console.WriteLine("Getting cache info after calling GetProperty(\"TestProperty0\")...\n");
            GetRuntimeCacheInfo(t);
            Console.WriteLine();

            pi = t.GetProperty("TestProperty1");

            Console.WriteLine("Getting cache info after calling GetProperty(\"TestProperty1\")...\n");
            GetRuntimeCacheInfo(t);
            Console.WriteLine();
        }

        private static void TestAnonymousClass()
        {
            string messageToWrap = "This is a test message!";
            object wrappedString = messageToWrap.Wrap();

            Type t = wrappedString.GetType();
            GetRuntimeCacheInfo(t);
            Console.WriteLine();

            object unwrappedString = wrappedString.Unwrap();
            GetRuntimeCacheInfo(t);
            Console.WriteLine();
        }

        private static void GetRuntimeCacheInfo(Type typeToTest)
        {
            var expectedType = "System.RuntimeType";
            if (typeToTest.GetType().FullName != expectedType)
            {
                Console.WriteLine("  Unexpected type, Expected: {0}, Got: {1}", expectedType, typeToTest.GetType().FullName);
                return;
            }

            Console.WriteLine("  Type: {0}", typeToTest.FullName);
            Console.WriteLine("  Reflection Type: {0} (BaseType: {1})", typeToTest.GetType(), typeToTest.GetType().BaseType);
            var runtimeTypeCache = typeToTest.GetType()
                .GetProperty("Cache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .GetGetMethod(nonPublic: true)
                .Invoke(typeToTest, null);
            if (runtimeTypeCache.GetType().FullName != "System.RuntimeType+RuntimeTypeCache")
            {
                Console.WriteLine("  Unexpected type for 'Cache', Expected: {0}, Got: {1}", "System.RuntimeType+RuntimeTypeCache", runtimeTypeCache.GetType().FullName);
                return;
            }

            GetFieldInfoCache(runtimeTypeCache);
            GetPropertyInfoCache(runtimeTypeCache);
        }

        private static void GetFieldInfoCache(object runtimeTypeCache)
        {
            var fieldInfoCache = runtimeTypeCache.GetType()
                .GetField("m_fieldInfoCache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .GetValue(runtimeTypeCache);
            if (fieldInfoCache == null)
            {
                Console.WriteLine("  m_fieldInfoCache is null, cache has not been initialised yet");
                return;
            }

            var isFieldInfoCacheComplete = fieldInfoCache.GetType()
                .GetField("m_cacheComplete", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .GetValue(fieldInfoCache);
            var fieldInfoCacheMembers = fieldInfoCache.GetType()
                .GetField("m_allMembers", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .GetValue(fieldInfoCache) as Array;

            Console.WriteLine("  m_fieldInfoCache: {0}, m_cacheComplete = {1}, contains {2} item(s)", runtimeTypeCache, isFieldInfoCacheComplete, fieldInfoCacheMembers.Length);
            for (int i = 0; i < fieldInfoCacheMembers.Length; i++)
            {
                var item = fieldInfoCacheMembers.GetValue(i);
                var fieldInfo = item as FieldInfo;
                Console.WriteLine("    [{0}] - {1} - {2}", i, item, string.Join(", ", fieldInfo?.Attributes));
            }
        }

        private static void GetPropertyInfoCache(object runtimeTypeCache)
        {
            var propertyInfoCache = runtimeTypeCache.GetType()
                .GetField("m_propertyInfoCache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .GetValue(runtimeTypeCache);
            if (propertyInfoCache == null)
            {
                Console.WriteLine("  m_propertyInfoCache is null, cache has not been initialised yet");
                return;
            }

            var isPropertyInfoCacheComplete = propertyInfoCache.GetType()
                .GetField("m_cacheComplete", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .GetValue(propertyInfoCache);
            var propertyInfoCacheMembers = propertyInfoCache.GetType()
                .GetField("m_allMembers", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .GetValue(propertyInfoCache) as Array;

            Console.WriteLine("  m_propertyInfoCache: {0}, m_cacheComplete = {1}, contains {2} item(s)", runtimeTypeCache, isPropertyInfoCacheComplete, propertyInfoCacheMembers.Length);
            for (int i = 0; i < propertyInfoCacheMembers.Length; i++)
            {
                var item = propertyInfoCacheMembers.GetValue(i);
                Console.WriteLine("    [{0}] - {1}", i, item);
            }
        }
    }
}
