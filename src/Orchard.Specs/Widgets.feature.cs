﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Orchard.Specs
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Widgets")]
    public partial class WidgetsFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Widgets.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Widgets", "  In order to add and manage widgets on my site\r\n  As an author\r\n  I want to crea" +
                    "te and edit widgets and layers", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("I can add a new layer and that layer is active when I\'m redirected to the widget " +
            "management page")]
        public virtual void ICanAddANewLayerAndThatLayerIsActiveWhenIMRedirectedToTheWidgetManagementPage()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("I can add a new layer and that layer is active when I\'m redirected to the widget " +
                    "management page", ((string[])(null)));
#line 28
this.ScenarioSetup(scenarioInfo);
#line 29
    testRunner.Given("I have installed Orchard", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 30
 testRunner.When("I go to \"Admin/ContentTypes/Edit/Layer\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table1.AddRow(new string[] {
                        "ContentTypeSettingsViewModel.Draftable",
                        "true"});
#line 31
  testRunner.And("I fill in", ((string)(null)), table1, "And ");
#line 34
  testRunner.And("I hit \"Save\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 35
  testRunner.And("I am redirected", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 36
 testRunner.Then("I should see \"\\\"Layer\\\" settings have been saved.\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 37
    testRunner.When("I go to \"Admin/Widgets\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 38
        testRunner.And("I follow \"Add a new layer...\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 39
    testRunner.Then("I should see \"<h1[^>]*>Add Layer</h1>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table2.AddRow(new string[] {
                        "LayerPart.Name",
                        "For awesome stuff"});
            table2.AddRow(new string[] {
                        "LayerPart.LayerRule",
                        "url \"~/awesome*\""});
#line 40
    testRunner.When("I fill in", ((string)(null)), table2, "When ");
#line 44
        testRunner.And("I hit \"Save Draft\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 45
        testRunner.And("I am redirected", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 46
    testRunner.Then("I should see \"Your Layer has been created.\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 47
        testRunner.And("I should see \"<option[^>]+selected=\"selected\"[^>]+value=\"\\d+\">For awesome stuff</" +
                    "option>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("I can delete a layer")]
        public virtual void ICanDeleteALayer()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("I can delete a layer", ((string[])(null)));
#line 49
this.ScenarioSetup(scenarioInfo);
#line 50
    testRunner.Given("I have installed Orchard", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 51
    testRunner.When("I go to \"Admin/Widgets\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 52
    testRunner.Then("I should see \"<option[^>]*>Default</option>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 53
    testRunner.When("I follow \"Edit\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 54
    testRunner.Then("I should see \"<input[^>]*name=\"LayerPart.Name\"[^>]*value=\"Default\"[^>]*>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 55
    testRunner.When("I hit \"Delete\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 56
        testRunner.And("I am redirected", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 57
    testRunner.Then("I should see \"Layer was successfully deleted\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 58
        testRunner.And("I should not see \"<option[^>]*>Default</option>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("I can add a widget to a specific zone in a specific layer")]
        public virtual void ICanAddAWidgetToASpecificZoneInASpecificLayer()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("I can add a widget to a specific zone in a specific layer", ((string[])(null)));
#line 60
this.ScenarioSetup(scenarioInfo);
#line 61
    testRunner.Given("I have installed Orchard", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 62
 testRunner.When("I go to \"Admin/ContentTypes/Edit/HtmlWidget\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table3.AddRow(new string[] {
                        "ContentTypeSettingsViewModel.Draftable",
                        "true"});
#line 63
  testRunner.And("I fill in", ((string)(null)), table3, "And ");
#line 66
  testRunner.And("I hit \"Save\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 67
  testRunner.And("I am redirected", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 68
 testRunner.Then("I should see \"\\\"Html Widget\\\" settings have been saved.\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 69
    testRunner.When("I go to \"Admin/Widgets\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table4.AddRow(new string[] {
                        "layerId",
                        "Disabled"});
#line 70
        testRunner.And("I fill in", ((string)(null)), table4, "And ");
#line 73
    testRunner.Then("I should see \"<option[^>]+selected=\"selected\"[^>]+value=\"\\d+\">Default</option>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 74
    testRunner.When("I follow \"Add\" where href has \"zone=Header\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 75
    testRunner.Then("I should see \"<h1[^>]*>Choose A Widget</h1>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 76
    testRunner.When("I follow \"<h2>Html Widget</h2>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 77
    testRunner.Then("I should see \"<h1[^>]*>Add Widget</h1>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table5.AddRow(new string[] {
                        "WidgetPart.Title",
                        "Flashy HTML Widget"});
            table5.AddRow(new string[] {
                        "Body.Text",
                        "<p><blink>hi</blink></p>"});
#line 78
    testRunner.When("I fill in", ((string)(null)), table5, "When ");
#line 82
        testRunner.And("I hit \"Save Draft\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 83
        testRunner.And("I am redirected", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 84
    testRunner.Then("I should see \"Your Html Widget has been added.\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 85
        testRunner.And("I should see \"<option[^>]+selected=\"selected\"[^>]+value=\"\\d+\">Default</option>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 86
        testRunner.And("I should see \"<li[^>]*class=\"[^\"]*widgets-this-layer[^\"]*\"[^>]*>\\s*<form[^>]*>\\s*" +
                    "<h3[^>]*>\\s*<a[^>]*>Flashy HTML Widget</a>\\s*</h3>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
