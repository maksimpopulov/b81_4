using BugPro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace BugTests
{
[TestClass]
public class BugWorkflowTests
{
    [TestMethod]
    public void Test_InitialState_ShouldBeNew()
    {
        var bug = new Bug();

        Assert.AreEqual(BugState.New, bug.State);
    }

    [TestMethod]
    public void Test_New_To_Triaging_ValidTransition()
    {
        var bug = new Bug();

        bug.Fire(BugTrigger.StartTriaging);

        Assert.AreEqual(BugState.Triaging, bug.State);
    }

    [TestMethod]
    public void Test_Triaging_To_NoTime_ValidTransition()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);

        bug.Fire(BugTrigger.NoTimeNow);

        Assert.AreEqual(BugState.NoTime, bug.State);
    }

    [TestMethod]
    public void Test_Triaging_To_SeparateSolution_ValidTransition()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);

        bug.Fire(BugTrigger.SeparateSolutionReq);

        Assert.AreEqual(BugState.SeparateSolution, bug.State);
    }

    [TestMethod]
    public void Test_SeparateSolution_To_ProblemSolved_ValidTransition()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.SeparateSolutionReq);

        bug.Fire(BugTrigger.MarkSolved);

        Assert.AreEqual(BugState.ProblemSolved, bug.State);
    }

    [TestMethod]
    public void Test_ProblemSolved_To_Closed_WithConfirmation()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.StartFix);
        bug.Fire(BugTrigger.MarkSolved);

        bug.Fire(BugTrigger.ConfirmSolved);

        Assert.AreEqual(BugState.Closed, bug.State);
    }

    [TestMethod]
    public void Test_ProblemSolved_To_Reopened_WhenNotSolved()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.StartFix);
        bug.Fire(BugTrigger.MarkSolved);

        bug.Fire(BugTrigger.NotSolved);

        Assert.AreEqual(BugState.Reopened, bug.State);
    }

    [TestMethod]
    public void Test_Reopened_To_Triaging_Valid()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.StartFix);
        bug.Fire(BugTrigger.MarkSolved);
        bug.Fire(BugTrigger.NotSolved);

        bug.Fire(BugTrigger.StartTriaging);

        Assert.AreEqual(BugState.Triaging, bug.State);
    }

    [TestMethod]
    public void Test_Reopened_To_Fixing_Valid()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.StartFix);
        bug.Fire(BugTrigger.MarkSolved);
        bug.Fire(BugTrigger.NotSolved);

        bug.Fire(BugTrigger.StartFix);

        Assert.AreEqual(BugState.Fixing, bug.State);
    }

    [TestMethod]
    public void Test_Triaging_To_NeedMoreInfo_Then_Back()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);

        bug.Fire(BugTrigger.RequestMoreInfo);
        Assert.AreEqual(BugState.NeedMoreInfo, bug.State);

        bug.Fire(BugTrigger.InfoProvided);
        Assert.AreEqual(BugState.Triaging, bug.State);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_InvalidTransition_FromClosed_ToFixing_Throws()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.Close);

        bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.OtherProductIssue);
        bug.Fire(BugTrigger.Close);
        bug.Fire(BugTrigger.StartFix);
    }

    [TestMethod]
    public void Test_CanFire_BeforeFiring_ReturnsExpected()
    {
        var bug = new Bug();

        Assert.IsTrue(bug.CanFire(BugTrigger.StartTriaging));
        Assert.IsFalse(bug.CanFire(BugTrigger.Close));
        Assert.IsFalse(bug.CanFire(BugTrigger.StartFix));
    }

    [TestMethod]
    public void Test_NoTime_To_Triaging_ValidTransition()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.NoTimeNow);
        Assert.AreEqual(BugState.NoTime, bug.State);

        bug.Fire(BugTrigger.StartTriaging);
        Assert.AreEqual(BugState.Triaging, bug.State);
    }

    [TestMethod]
    public void Test_OtherProduct_To_Closed_ValidTransition()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.OtherProductIssue);
        Assert.AreEqual(BugState.OtherProduct, bug.State);

        bug.Fire(BugTrigger.Close);
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    [TestMethod]
    public void Test_StateChangedEvent_Fires_OnTransition()
    {
        var bug = new Bug();
        BugState? fromState = null;
        BugState? toState = null;

        bug.StateChanged += (from, to) =>
        {
            fromState = from;
            toState = to;
        };

        bug.Fire(BugTrigger.StartTriaging);

        Assert.AreEqual(BugState.New, fromState);
        Assert.AreEqual(BugState.Triaging, toState);
    }

    [TestMethod]
    public void Test_FullHappyPath_NewToClosed()
    {
        var bug = new Bug();

        bug.Fire(BugTrigger.StartTriaging);
        bug.Fire(BugTrigger.StartFix);
        bug.Fire(BugTrigger.MarkSolved);
        bug.Fire(BugTrigger.ConfirmSolved);

        Assert.AreEqual(BugState.Closed, bug.State);
    }
}
}
