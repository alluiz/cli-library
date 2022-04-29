using System;
using Clysh.Data;
using Moq;
using NUnit.Framework;

namespace Clysh.Tests;

public class ClyshServiceTests
{
    private readonly Mock<IClyshView> frontMock = new();
    private readonly Mock<IClyshCommand> rootCommandMock = new();


    [SetUp]
    public void Setup()
    {
        frontMock.Reset();
        rootCommandMock.Reset();
    }

    [Test]
    public void SuccessfulCreateCLIAndRootCommand()
    {
        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);
        Assert.AreEqual(rootCommandMock.Object, cli.RootCommand);
        Assert.AreEqual(frontMock.Object, cli.View);
    }

    [Test]
    public void SuccessfulCreateCommand()
    {
        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        Action<ClyshMap<ClyshOption>, IClyshView> newAction = (x, y) => { };
        string name = "new";
        string description = "new command for test";

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand command = builder.Id(name).Description(description).Action(newAction).Build();
        
        Assert.AreEqual(name, command.Id);
        Assert.AreEqual(description, command.Description);
        Assert.AreEqual(newAction, command.Action);
    }



    [Test]
    public void SuccessfulExecuteRootWithNoArgs()
    {
        string[] args = Array.Empty<string>();

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        cli.Execute(args);

        rootCommandMock.Verify(x => x.Action, Times.Once);
    }

    [Test]
    public void SuccessfulExecuteRootWithOneEmptyArg()
    {
        string[] args = new string[] { "" };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        cli.Execute(args);

        rootCommandMock.Verify(x => x.Action, Times.Once);
    }

    [Test]
    public void SuccessfulExecuteRootWithOneSpaceArg()
    {
        string[] args = new string[] { "  " };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        cli.Execute(args);

        rootCommandMock.Verify(x => x.Action, Times.Once);
    }

    [Test]
    public void SuccessfulExecuteRootWithMultiSpacesArg()
    {
        string[] args = new string[] { "  ", "   ", "         ", "  " };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        cli.Execute(args);

        rootCommandMock.Verify(x => x.Action, Times.Once);

    }

    [Test]
    public void SuccessfulExecuteRootWithSomeOption()
    {
        const string someOption = "some-option";
        const string someAbbrevOption = "s";
        const string someOptionDescription = "awesome option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }
        
        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);

        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Shortcut(someAbbrevOption).Build());

        cli.Execute(args);

        Assert.NotNull(expectedOptions);
        Assert.NotNull(expectedCliFront);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someAbbrevOption, expectedOptions?.Get(someOption).Shortcut);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);

        Assert.AreEqual(cli.View, expectedCliFront);
    }

    [Test]
    public void SuccessfulExecuteRootWithSomeOptionAndParameter()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "testarg:mytest" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);

        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Parameters(ClyshParameters.Create(new ClyshParameter("testarg", 6, 10))).Build());

        cli.Execute(args);

        Assert.NotNull(expectedCliFront);
        Assert.NotNull(expectedOptions);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);
        Assert.AreEqual("mytest", expectedOptions?.Get(someOption).Parameters.Get("testarg").Data);

        Assert.AreEqual(cli.View, expectedCliFront);
    }

    [Test]
    public void SuccessfulExecuteRootWithSomeOptionAndParameterByPosition()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "mytest" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);

        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Parameters(ClyshParameters.Create(new ClyshParameter("testarg", 6, 10))).Build());

        cli.Execute(args);

        Assert.NotNull(expectedCliFront);
        Assert.NotNull(expectedOptions);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);
        Assert.AreEqual("mytest", expectedOptions?.Get(someOption).Parameters.Get("testarg").Data);

        Assert.AreEqual(cli.View, expectedCliFront);
    }

    [Test]
    public void SuccessfulExecuteRootWithSomeOptionAndMultiParameter()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "testarg:mytest", "testarg2:mytest2", "testarg3:mytest3" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();
        
        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);

        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Parameters(ClyshParameters.Create(new("testarg", 6, 10)
            , new("testarg2", 6, 10)
            , new("testarg3", 6, 10))).Build());

        cli.Execute(args);

        Assert.NotNull(expectedCliFront);
        Assert.NotNull(expectedOptions);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);
        Assert.AreEqual("mytest", expectedOptions?.Get(someOption).Parameters.Get("testarg").Data);
        Assert.AreEqual("mytest2", expectedOptions?.Get(someOption).Parameters.Get("testarg2").Data);
        Assert.AreEqual("mytest3", expectedOptions?.Get(someOption).Parameters.Get("testarg3").Data);

        Assert.AreEqual(cli.View, expectedCliFront);
    }

    [Test]
    public void SuccessfulExecuteRootWithSomeOptionAndOptionalParameter()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "testarg:mytest" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);
        
        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Parameters(ClyshParameters.Create(new ClyshParameter("testarg", 6, 10, false))).Build());

        cli.Execute(args);

        Assert.NotNull(expectedCliFront);
        Assert.NotNull(expectedOptions);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);
        Assert.AreEqual("mytest", expectedOptions?.Get(someOption).Parameters.Get("testarg").Data);

        Assert.AreEqual(cli.View, expectedCliFront);
    }

    [Test]
    public void SuccessfulExecuteRootWithSomeOptionAndMultiOptionalParameter()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "testarg:mytest", "testarg2:mytest2", "testarg3:mytest3" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);
        
        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Parameters(ClyshParameters.Create(new ClyshParameter("testarg", 6, 10, false)
            , new("testarg2", 6, 10, false)
            , new("testarg3", 6, 10, false))).Build());

        cli.Execute(args);

        Assert.NotNull(expectedCliFront);
        Assert.NotNull(expectedOptions);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);
        Assert.AreEqual("mytest", expectedOptions?.Get(someOption).Parameters.Get("testarg").Data);
        Assert.AreEqual("mytest2", expectedOptions?.Get(someOption).Parameters.Get("testarg2").Data);
        Assert.AreEqual("mytest3", expectedOptions?.Get(someOption).Parameters.Get("testarg3").Data);

        Assert.AreEqual(cli.View, expectedCliFront);
    }

    [Test]
    public void SuccessfulExecuteRootWithSomeOptionAndRequiredAndOptionalParameter()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "testarg:mytest", "testreq:myreq" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);
        
        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Parameters(ClyshParameters.Create(new ClyshParameter("testarg", 6, 10, false),
            new("testreq", 5, 10))).Build());

        cli.Execute(args);

        Assert.NotNull(expectedCliFront);
        Assert.NotNull(expectedOptions);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);
        Assert.AreEqual("mytest", expectedOptions?.Get(someOption).Parameters.Get("testarg").Data);
        Assert.AreEqual("myreq", expectedOptions?.Get(someOption).Parameters.Get("testreq").Data);

        Assert.AreEqual(cli.View, expectedCliFront);
    }

    [Test]
    public void SuccessfulExecuteRootWithSomeOptionAndMultiOptionalAndRequiredParameter()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "testarg:mytest", "testarg2:mytest2", "testarg3:mytest3", "testarg4:mytest4", "testarg5:mytest5" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);
        
        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Parameters(ClyshParameters.Create(new ClyshParameter("testarg", 6, 10, false)
            , new("testarg2", 6, 10)
            , new("testarg3", 6, 10, false)
            , new("testarg4", 6, 10, false)
            , new("testarg5", 6, 10))).Build());

        cli.Execute(args);

        Assert.NotNull(expectedCliFront);
        Assert.NotNull(expectedOptions);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);
        Assert.AreEqual("mytest", expectedOptions?.Get(someOption).Parameters.Get("testarg").Data);
        Assert.AreEqual("mytest2", expectedOptions?.Get(someOption).Parameters.Get("testarg2").Data);
        Assert.AreEqual("mytest3", expectedOptions?.Get(someOption).Parameters.Get("testarg3").Data);
        Assert.AreEqual("mytest4", expectedOptions?.Get(someOption).Parameters.Get("testarg4").Data);
        Assert.AreEqual("mytest5", expectedOptions?.Get(someOption).Parameters.Get("testarg5").Data);

        Assert.AreEqual(cli.View, expectedCliFront);
    }

    [Test]
    public void SuccessfulExecuteRootWithMultiOptionAndMultiOptionalAndRequiredParameter()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";
        const string someOptionWithDashes = $"--{someOption}";

        const string someOption2 = "some-option2";
        const string someOptionDescription2 = "awesome option2";
        const string someOptionWithDashes2 = $"--{someOption2}";

        string[] args = new string[] { someOptionWithDashes, "testarg:mytest", "testarg2:mytest2", someOptionWithDashes2, "testarg3:mytest3", "testarg4:mytest4", "testarg5:mytest5" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);
        
        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Parameters(ClyshParameters.Create(new ClyshParameter("testarg", 6, 10, false), new("testarg2", 6, 10))).Build());
        cli.RootCommand.AddOption(optionBuilder.Id(someOption2).Description(someOptionDescription2).Parameters(ClyshParameters.Create(new ClyshParameter("testarg3", 6, 10, false),
            new("testarg4", 6, 10, false),
            new("testarg5", 6, 10))).Build());

        cli.Execute(args);

        Assert.NotNull(expectedCliFront);
        Assert.NotNull(expectedOptions);

        Assert.AreEqual(2, expectedOptions?.Itens.Count);

        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.IsTrue(expectedOptions?.Has(someOption2));

        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);
        Assert.AreEqual("mytest", expectedOptions?.Get(someOption).Parameters.Get("testarg").Data);
        Assert.AreEqual("mytest2", expectedOptions?.Get(someOption).Parameters.Get("testarg2").Data);

        Assert.AreEqual(someOption2, expectedOptions?.Get(someOption2).Id);
        Assert.AreEqual(someOptionDescription2, expectedOptions?.Get(someOption2).Description);
        Assert.AreEqual("mytest3", expectedOptions?.Get(someOption2).Parameters.Get("testarg3").Data);
        Assert.AreEqual("mytest4", expectedOptions?.Get(someOption2).Parameters.Get("testarg4").Data);
        Assert.AreEqual("mytest5", expectedOptions?.Get(someOption2).Parameters.Get("testarg5").Data);

        Assert.AreEqual(cli.View, expectedCliFront);
    }


    [Test]
    public void SuccessfulExecuteRootWithCustomCommandAndMultiOptionAndMultiOptionalAndRequiredParameter()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";
        const string someOptionWithDashes = $"--{someOption}";

        const string someOption2 = "some-option2";
        const string someOptionDescription2 = "awesome option2";
        const string someOptionWithDashes2 = $"--{someOption2}";

        string[] args = new string[] { someOptionWithDashes, "testarg:mytest", "testarg2:mytest2", "test", someOptionWithDashes2, "testarg3:mytest3", "testarg4:mytest4", "testarg5:mytest5" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        ClyshMap<ClyshOption>? expectedOptionsCustom = null;
        IClyshView? expectedCliFrontCustom = null;

        ClyshCommandBuilder builder = new ClyshCommandBuilder();
        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();

        ClyshCommand customCommand = builder
            .Id("test")
            .Description("test command description")
            .Action((o, c) =>
            {
                expectedOptionsCustom = o;
                expectedCliFrontCustom = c;

            })
            .Option(optionBuilder
                .Id(someOption2)
                .Description(someOptionDescription2)
                .Parameters(ClyshParameters.Create(new("testarg3", 6, 10, false),
                    new("testarg4", 6, 10, false),
                    new("testarg5", 6, 10)))
                .Build())
            .Build();
        
        ClyshCommand rootCommand = builder
            .Id("root")
            .Description("root command")
            .Action((o, c) =>
            {
                expectedOptions = o;
                expectedCliFront = c;
            })
            .Option(optionBuilder
                .Id(someOption)
                .Description(someOptionDescription)
                .Parameters(ClyshParameters.Create(new ClyshParameter("testarg", 6, 10, false),
                    new ClyshParameter("testarg2", 6, 10)))
                .Build())
            .Child(customCommand)
            .Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);

        cli.Execute(args);

        Assert.NotNull(expectedCliFront);
        Assert.NotNull(expectedOptions);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.AreEqual(1, expectedOptionsCustom?.Itens.Count);

        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.IsTrue(expectedOptionsCustom?.Has(someOption2));

        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);
        Assert.AreEqual("mytest", expectedOptions?.Get(someOption).Parameters.Get("testarg").Data);
        Assert.AreEqual("mytest2", expectedOptions?.Get(someOption).Parameters.Get("testarg2").Data);

        Assert.AreEqual(someOption2, expectedOptionsCustom?.Get(someOption2).Id);
        Assert.AreEqual(someOptionDescription2, expectedOptionsCustom?.Get(someOption2).Description);
        Assert.AreEqual("mytest3", expectedOptionsCustom?.Get(someOption2).Parameters.Get("testarg3").Data);
        Assert.AreEqual("mytest4", expectedOptionsCustom?.Get(someOption2).Parameters.Get("testarg4").Data);
        Assert.AreEqual("mytest5", expectedOptionsCustom?.Get(someOption2).Parameters.Get("testarg5").Data);

        Assert.AreEqual(cli.View, expectedCliFront);
        Assert.AreEqual(cli.View, expectedCliFrontCustom);

        Assert.AreEqual(0, cli.RootCommand.Order);
        Assert.AreEqual(1, customCommand.Order);
    }

    [Test]
    public void SuccessfulExecuteRootWithSomeAbbrevOption()
    {

        const string someOption = "some-option";
        const string someAbbrevOption = "s";
        const string someOptionDescription = "awesome option";

        const string someAbbrevOptionWithDash = $"-{someAbbrevOption}";

        string[] args = new string[] { someAbbrevOptionWithDash };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);

        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Shortcut(someAbbrevOption).Build());

        cli.Execute(args);

        Assert.NotNull(expectedOptions);
        Assert.NotNull(expectedCliFront);

        Assert.AreEqual(1, expectedOptions?.Itens.Count);
        Assert.IsTrue(expectedOptions?.Has(someOption));
        Assert.AreEqual(someOption, expectedOptions?.Get(someOption).Id);
        Assert.AreEqual(someAbbrevOption, expectedOptions?.Get(someOption).Shortcut);
        Assert.AreEqual(someOptionDescription, expectedOptions?.Get(someOption).Description);

        Assert.AreEqual(cli.View, expectedCliFront);
    }

    [Test]
    public void ExecuteRootWithSomeInvalidOptionError()
    {
        const string invalidOption = "invalid-option";

        const string invalidOptionWithDashes = $"--{invalidOption}";

        string[] args = new string[] { invalidOptionWithDashes };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        rootCommandMock.Setup(x => x.HasOption(invalidOption)).Returns(false);

        cli.Execute(args);

        frontMock.Verify(x => x.PrintHelp(rootCommandMock.Object, It.Is<InvalidOperationException>(x => x.Message == $"The option '{invalidOptionWithDashes}' is invalid.")), Times.Once);
    }

    [Test]
    public void ExecuteRootWithSomeInvalidAbbrevOptionError()
    {
        const string invalidOption = "i";

        const string invalidOptionWithDashes = $"-{invalidOption}";

        string[] args = new string[] { invalidOptionWithDashes };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        rootCommandMock.Setup(x => x.HasOption(invalidOption)).Returns(false);

        cli.Execute(args);

        frontMock.Verify(x => x.PrintHelp(rootCommandMock.Object, It.Is<InvalidOperationException>(x => x.Message == $"The option '{invalidOptionWithDashes}' is invalid.")), Times.Once);
    }

    [Test]
    public void ExecuteRootWithValidOptionButNoArgsError()
    {
        const string someOption = "some-option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        rootCommandMock.Setup(x => x.HasOption(someOption)).Returns(true);
        rootCommandMock.Setup(x => x.GetOption(someOption)).Returns(optionBuilder
            .Id("some-option")
            .Description("some option")
            .Parameters(ClyshParameters.Create(new ClyshParameter("arg1", 6, 10)))
            .Build());

        cli.Execute(args);

        frontMock.Verify(x => x.PrintHelp(rootCommandMock.Object, It.Is<InvalidOperationException>(x => x.Message == $"Required parameters [arg1] is missing for option: some-option")), Times.Once);
    }

    [Test]
    public void ExecuteRootWithHelpOption()
    {
        const string HELP_OPTION = "help";

        const string HELP_OPTION_WITH_DASHES = $"--{HELP_OPTION}";

        string[] args = new string[] { HELP_OPTION_WITH_DASHES };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        rootCommandMock.Setup(x => x.HasOption(HELP_OPTION)).Returns(true);
        
        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        rootCommandMock.Setup(x => x.GetOption(HELP_OPTION)).Returns(optionBuilder
            .Id(HELP_OPTION)
            .Description("Show help on screen")
            .Shortcut("h")
            .Build());

        cli.Execute(args);

        frontMock.Verify(x => x.PrintHelp(rootCommandMock.Object), Times.Once);
    }

    [Test]
    public void ExecuteRootWithArgsAndNoOptionError()
    {

        string[] args = new string[] { "testarg:mytest" };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        cli.Execute(args);

        frontMock.Verify(x => x.PrintHelp(rootCommandMock.Object, It.Is<InvalidOperationException>(x => x.Message == "You can't put parameters without any option that accept it.")), Times.Once);
    }

    [Test]
    public void ExecuteRootWithSomeOptionAndInvalidArgsError()
    {
        const string someOption = "some-option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "testarg:mytest" };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        rootCommandMock.Setup(x => x.HasOption(someOption)).Returns(true);
        
        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        rootCommandMock.Setup(x => x.GetOption(someOption)).Returns(optionBuilder
            .Id(someOption)
            .Description("some option")
            .Build());

        cli.Execute(args);

        frontMock.Verify(x => x.PrintHelp(rootCommandMock.Object, It.Is<InvalidOperationException>(x => x.Message == $"The parameter 'testarg' is invalid for option: {someOption}.")), Times.Once);
    }

    [Test]
    public void ExecuteRootWithSomeOptionAndInvalidArgsByPositionError()
    {
        const string someOption = "some-option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "testarg" };

        IClyshService cli = new ClyshService(rootCommandMock.Object, frontMock.Object);

        rootCommandMock.Setup(x => x.HasOption(someOption)).Returns(true);
        
        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        rootCommandMock.Setup(x => x.GetOption(someOption)).Returns(optionBuilder
            .Id(someOption)
            .Description("some option")
            .Build());

        cli.Execute(args);

        frontMock.Verify(x => x.PrintHelp(rootCommandMock.Object, It.Is<InvalidOperationException>(x => x.Message == $"The parameter data 'testarg' is out of bound for option: {someOption}.")), Times.Once);
    }

    [Test]
    public void ExecuteRootWithSomeOptionAndParameterByPositionAlreadyFilledError()
    {
        const string someOption = "some-option";
        const string someOptionDescription = "awesome option";

        const string someOptionWithDashes = $"--{someOption}";

        string[] args = new string[] { someOptionWithDashes, "mytest", "testarg:mytest" };

        ClyshMap<ClyshOption>? expectedOptions = null;
        IClyshView? expectedCliFront = null;

        void action(ClyshMap<ClyshOption> options, IClyshView cliFront)
        {
            expectedOptions = options;
            expectedCliFront = cliFront;
        }

        ClyshCommandBuilder builder = new ClyshCommandBuilder();

        IClyshCommand rootCommand = builder.Id("root").Description("root command").Action(action).Build();

        IClyshService cli = new ClyshService(rootCommand, frontMock.Object);

        ClyshOptionBuilder optionBuilder = new ClyshOptionBuilder();
        
        cli.RootCommand.AddOption(optionBuilder.Id(someOption).Description(someOptionDescription).Parameters(ClyshParameters.Create(new ClyshParameter("testarg", 6, 10))).Build());

        cli.Execute(args);

        frontMock.Verify(x => x.PrintHelp(rootCommand, It.Is<InvalidOperationException>(x => x.Message == $"The parameter 'testarg' is already filled for option: {someOption}.")), Times.Once);
    }
}