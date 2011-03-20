using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_9_Date_Objects_15_9_5_Properties_of_the_Date_Prototype_Object_15_9_5_22_Date_prototype_getSeconds : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.9_Date_Objects\15.9.5_Properties_of_the_Date_Prototype_Object\15.9.5.22_Date.prototype.getSeconds"); }
    [TestMethod] public void S15_9_5_22_A1_T1_js() { RunFile(@"S15.9.5.22_A1_T1.js"); }
    [TestMethod] public void S15_9_5_22_A1_T2_js() { RunFile(@"S15.9.5.22_A1_T2.js"); }
    [TestMethod] public void S15_9_5_22_A1_T3_js() { RunFile(@"S15.9.5.22_A1_T3.js"); }
    [TestMethod] public void S15_9_5_22_A2_T1_js() { RunFile(@"S15.9.5.22_A2_T1.js"); }
    [TestMethod] public void S15_9_5_22_A3_T1_js() { RunFile(@"S15.9.5.22_A3_T1.js"); }
    [TestMethod] public void S15_9_5_22_A3_T2_js() { RunFile(@"S15.9.5.22_A3_T2.js"); }
    [TestMethod] public void S15_9_5_22_A3_T3_js() { RunFile(@"S15.9.5.22_A3_T3.js"); }
  }
}