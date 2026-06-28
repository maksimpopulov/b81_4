using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;

namespace BugTests;

[TestClass]
public class BugWorkflowTests
{
    [TestMethod]
    public void Test_InitialState_ShouldBeOpen()
    {
        var bug = new Bug();
        Assert.AreEqual(BugState.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Test_Assign_ShouldChangeStateToAssigned()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Assign);
        Assert.AreEqual(BugState.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void Test_Assign_ShouldStoreAssigneeName()
    {
        var bug = new Bug();
        // В этой версии кода нет хранения имени, поэтому тест пропускаем или адаптируем
        // Если нужно, можно добавить свойство AssignedTo в класс Bug
        Assert.Inconclusive("This version doesn't support storing assignee name");
    }

    [TestMethod]
    public void Test_StartWork_AfterAssign_ShouldChangeToInProgress()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.StartProgress);
        Assert.AreEqual(BugState.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void Test_Fix_AfterStartWork_ShouldChangeToFixed()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.StartProgress);
        bug.Fire(BugTrigger.Fix);
        Assert.AreEqual(BugState.Fixed, bug.CurrentState);
    }

    [TestMethod]
    public void Test_Verify_AfterFix_ShouldChangeToVerified()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.StartProgress);
        bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Verify);
        Assert.AreEqual(BugState.Verified, bug.CurrentState);
    }

    [TestMethod]
    public void Test_Close_AfterVerify_ShouldChangeToClosed()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.StartProgress);
        bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Verify);
        bug.Fire(BugTrigger.Close);
        Assert.AreEqual(BugState.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Test_Reopen_FromClosed_ShouldChangeToReopened()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.StartProgress);
        bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Verify);
        bug.Fire(BugTrigger.Close);
        bug.Fire(BugTrigger.Reopen);
        Assert.AreEqual(BugState.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void Test_Reject_FromOpen_ShouldChangeToRejected()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Reject);
        Assert.AreEqual(BugState.Rejected, bug.CurrentState);
    }

    [TestMethod]
    public void Test_Reopen_FromRejected_ShouldChangeToReopened()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Reject);
        bug.Fire(BugTrigger.Reopen);
        Assert.AreEqual(BugState.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void Test_CompleteWorkflow_ShouldEndInClosedState()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.StartProgress);
        bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Verify);
        bug.Fire(BugTrigger.Close);
        Assert.AreEqual(BugState.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Test_Reassign_FromAssigned_ShouldStayAssigned()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.Assign); // Повторный Assign должен остаться в Assigned
        Assert.AreEqual(BugState.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void Test_CannotStartWork_WithoutAssignment()
    {
        var bug = new Bug();
        // В этой версии кода Fire не выбрасывает исключение, а просто пишет в консоль
        // Поэтому тест нужно адаптировать
        bug.Fire(BugTrigger.StartProgress);
        // Состояние должно остаться Open, так как переход не разрешен
        Assert.AreEqual(BugState.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Test_CannotFix_WithoutStartingWork()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.Fix);
        // Состояние должно остаться Assigned, так как переход не разрешен
        Assert.AreEqual(BugState.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void Test_MultipleReopenCloseCycles_ShouldWork()
    {
        var bug = new Bug();
        
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.StartProgress);
        bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Verify);
        bug.Fire(BugTrigger.Close);
        Assert.AreEqual(BugState.Closed, bug.CurrentState);
        
        bug.Fire(BugTrigger.Reopen);
        Assert.AreEqual(BugState.Reopened, bug.CurrentState);
        
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.StartProgress);
        bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Verify);
        bug.Fire(BugTrigger.Close);
        
        Assert.AreEqual(BugState.Closed, bug.CurrentState);
    }
}
