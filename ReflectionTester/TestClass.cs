using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionTester
{
    class TestClass
    {
        private static string staticTestField;

        private int privateTestField0;
        private string privateTestField1;

        public int publicTestField0;
        public string publicTestField1;

        public int TestProperty0 { get; set; }
        public string TestProperty1 { get; set; }
    }
}
